namespace UnadeskTest.PdfProcessor
{
    public interface IPdfProcessor
    {
         /// <summary>
         /// Асинхронно извлекает текст из PDF-файла.
         /// </summary>
         /// <param name="filePath">Полный путь к PDF-файлу.</param>
         /// <param name="cancellationToken">Токен отмены для отслеживания прогресса операции.</param>
         /// <returns>Задача, которая возвращает извлеченный текст в виде строки.</returns>
         Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken);
    }   
}
