using Bus.Shared.Models;
using Bus.Shared.Producer;
using DataAccess.Models;
using Microsoft.Extensions.Configuration;
using UnadeskTest.BusinessLogic.Dto.Models;
using UnadeskTest.BusinessLogic.Repositories;
using UnadeskTest.Services.Helpers;


namespace UnadeskTest.BusinessLogic.Services
{
    public class DocumentService : IDocumentService
    {

        private readonly IDocumentRepository _documentRepository;
        private readonly IMessageQueueProducer _mqProducer;
        
        private readonly string StoragePath;

        /// <summary>
        /// Конструктор сервиса документа.
        /// Принимает репозиторий для работы с данными и продюсера для отправки сообщений в очередь.
        /// </summary>
        public DocumentService(IDocumentRepository documentRepository, IMessageQueueProducer mqProducer, IConfiguration configuration)
        {
            _documentRepository = documentRepository;
        
            StoragePath = configuration.GetSection(nameof(StoragePath)).Value;
            _mqProducer = mqProducer;
        }

        ///<inheritdoc/>
        public async Task<Guid> CreateNewDocumentAsync(string fileName, byte[] fileContent, CancellationToken cancellationToken)
        {
            // 1. Сохранение файла локально
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            var savePath = Path.Combine(StoragePath, uniqueFileName);
            
            // Проверка и создание директории, если она не существует
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }
            
            // Запись содержимого файла в локальную файловую систему
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await stream.WriteAsync(fileContent);
            }

            // 2. Создание записи в БД со статусом 'Pending'
            var documentId = await _documentRepository.CreateNewDocumentAsync(fileName, cancellationToken);

            // 3. Отправка сообщения в очередь для асинхронной обработки
            var job = new ProcessingJob { DocumentId = documentId, FilePath = savePath };
            await _mqProducer.PublishJobAsync(job, cancellationToken);

            return documentId;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<PdfDocumentDto>> GetAllDocumentsAsync(CancellationToken cancellationToken)
        {
            // Получение сырых данных из репозитория
            var result = await _documentRepository.GetAllDocumentsAsync(cancellationToken);

            var list = new List<PdfDocumentDto>();
            // Маппинг сущностей в DTO для передачи в слой представления
            foreach (var document in result)
            {
                list.Add(new PdfDocumentDto
                {
                    Id = document.Id,
                    OriginalFileName = document.OriginalFileName,
                    FileStatus = document.FileStatus.GetDisplayName(),
                    ProcessedOn = document.ProcessedOn,
                });
            }

            return list;
        }

        ///<inheritdoc/>
        public async Task<PdfDocument> GetDocumentDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _documentRepository.GetDocumentDetailsAsync(id, cancellationToken); 
            return result;
        }

        ///<inheritdoc/>
        public async Task UpdateCompletedAsync(Guid id, string status, CancellationToken cancellationToken)
        {
            await _documentRepository.UpdateCompletedAsync(id, status, cancellationToken);
        }

        ///<inheritdoc/>
        public async Task<bool> UpdateStatusAsync(Guid id, FileStatus status, CancellationToken cancellationToken)
        {
            return await _documentRepository.UpdateStatusAsync(id, status, cancellationToken);
        }
    }

   
}
