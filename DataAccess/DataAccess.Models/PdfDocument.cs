namespace DataAccess.Models
{
    public class PdfDocument
    {
        // Уникальный идентификатор документа PDF.
        public Guid Id { get; set; }

        // Исходное имя файла документа.
        public string OriginalFileName { get; set; }

        // Текущий статус обработки документа (например, "Pending", "Processed", "Failed").
        public FileStatus FileStatus { get; set; }

        // Извлеченный текстовый контент документа. Может быть null, если контент не извлечен.
        public string? TextContent { get; set; }

        // Дата и время, когда документ был обработан. Null, если обработка не завершена.
        public DateTime? ProcessedOn { get; set; }
    }   
}
