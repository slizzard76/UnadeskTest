using DataAccess.Models;

namespace UnadeskTest.BusinessLogic.Dto.Models
{
    public class PdfDocumentDto
    {
        // Уникальный идентификатор документа.
        public Guid Id { get; set; }

        // Исходное имя файла документа.
        public string OriginalFileName { get; set; }

        // Текущий статус обработки документа (например, "Pending", "Processed").
        public string FileStatus { get; set; }

        // Дата и время, когда документ был обработан. Может быть null, если обработка еще не завершена.
        public DateTime? ProcessedOn { get; set; }
    }   
}
