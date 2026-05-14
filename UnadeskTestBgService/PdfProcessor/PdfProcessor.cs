using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace UnadeskTest.PdfProcessor
{
    /// <summary>
    /// Класс для обработки PDF-документов и извлечения текста.
    /// Реализует интерфейс IPdfProcessor.
    /// </summary>
    public class PdfProcessor : IPdfProcessor
    {
        /// <summary>
        /// Асинхронно извлекает весь текст из PDF-файла.
        /// </summary>
        /// <param name="filePath">Полный путь к PDF-файлу.</param>
        /// <param name="cancellationToken">Токен отмены для отслеживания операции.</param>
        /// <returns>Задача, которая возвращает извлеченный текст в виде строки.</returns>
        public async Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken)
        {
            // Читаем все байты файла асинхронно.
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var text = new StringBuilder();

            // Используем 'using' для гарантированного освобождения ресурсов документа.
            using (PdfDocument document = PdfDocument.Open(fileBytes))
            {
                // Итерируем по всем страницам документа.
                foreach (Page page in document.GetPages())
                {
                    // Извлекаем текст страницы, используя специализированный экстрактор.
                    string pageText = ContentOrderTextExtractor.GetText(page);
                    
                    // Добавляем извлеченный текст страницы к общему результату.
                    text.Append(pageText);
                    // Добавляем перенос строки для разделения текста страниц.
                    text.AppendLine();
                }
            }
            // Возвращаем итоговую строку с текстом.
            return text.ToString();
        }
    }   
}
