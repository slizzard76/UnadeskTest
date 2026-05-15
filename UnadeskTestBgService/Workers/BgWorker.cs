using Bus.Shared.Consumer;
using Bus.Shared.Models;
using DataAccess.Models;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using UnadeskTest.BusinessLogic.Services;
using UnadeskTest.PdfProcessor;

namespace UnadeskTest.Workers
{
    public class BgWorker : BackgroundService
    {
        private readonly IMessageQueueConsumer _mqConsumer;
        private readonly IPdfProcessor _pdfProcessor;
        private readonly IDocumentService _documentService;

        /// <summary>
        /// Конструктор BackgroundWorker.
        /// Внедряет зависимости для работы с очередью сообщений, PDF-обработчиком и сервисом документов.
        /// </summary>
        public BgWorker(IMessageQueueConsumer mqConsumer, IPdfProcessor pdfProcessor, IDocumentService documentService)
        {
            _mqConsumer = mqConsumer;
            _pdfProcessor = pdfProcessor;
            _documentService = documentService;
        }

        /// <summary>
        /// Основной метод, который выполняется в фоновом режиме.
        /// Подключается к очереди сообщений и начинает цикл потребления задач.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Подключение к RabbitMQ и запуск цикла потребления сообщений.
            // Используется асинхронный делегат для обработки каждой полученной задачи.
            await _mqConsumer.StartConsumingAsync(async (message) =>
            {
                // 1. Десериализация Payload: Преобразование сырого тела сообщения в объект ProcessingJob.
                var job = JsonSerializer.Deserialize<ProcessingJob>(message.Body);

                // 2. Проверка существования файла чтобы избежать исключений I/O.
                if (!File.Exists(job.FilePath))
                {
                    Console.WriteLine($"Error: File not found at path {job.FilePath}");
                    // Если файл не найден, задача считается неудачной, но мы просто выходим из обработки.
                    return;
                }

                try
                {
                    // 3. Установка статуса 'Processing': Информирование системы о начале работы.
                    await _documentService.UpdateStatusAsync(job.DocumentId, FileStatus.Processing, cancellationToken);
                    Console.WriteLine($"Starting processing for Document ID: {job.DocumentId}");

                    // 4. Тяжелая операция: Парсинг PDF. Это самая ресурсоемкая часть.
                    var text = await _pdfProcessor.ExtractTextAsync(job.FilePath, cancellationToken);

                    // 5. Обновление статуса и данных в БД: Сохранение результата.
                    await _documentService.UpdateCompletedAsync(job.DocumentId, text, cancellationToken);

                    Console.WriteLine($"Successfully processed and saved Document ID: {job.DocumentId}");
                }
                catch (Exception ex)
                {
                    // 6. Обработка ошибок: Логирование исключения и установка статуса 'Failed' в БД.
                    Console.WriteLine($"Failed to process document {job.DocumentId}: {ex.Message}");
                    await _documentService.UpdateStatusAsync(job.DocumentId, FileStatus.Failed, cancellationToken);
                }
            });
        }
    }   
}
