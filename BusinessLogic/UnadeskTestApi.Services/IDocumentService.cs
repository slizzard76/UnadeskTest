using DataAccess.Models;
using UnadeskTest.BusinessLogic.Dto.Models;


namespace UnadeskTest.BusinessLogic.Services
{
public interface IDocumentService
    {
        /// <summary>
        /// Создает новый документ в системе.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="fileContent">Бинарное содержимое файла.</param>
        /// <param name="cancellation">Токен отмены операции.</param>
        /// <returns>GUID созданного документа.</returns>
        Task<Guid> CreateNewDocumentAsync(string fileName, byte[] fileContent, CancellationToken cancellation);

        /// <summary>
        /// Получает подробные сведения о документе по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор документа.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Объект PdfDocument с деталями документа.</returns>
        Task<PdfDocument> GetDocumentDetailsAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Получает список всех доступных документов.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Коллекция DTO всех документов.</returns>
        Task <IEnumerable<PdfDocumentDto>> GetAllDocumentsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Обновляет статус документа.
        /// </summary>
        /// <param name="id">Идентификатор документа.</param>
        /// <param name="status">Новый статус документа.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>True, если обновление прошло успешно.</returns>
        Task<bool> UpdateStatusAsync(Guid id, FileStatus status, CancellationToken cancellationToken);

        /// <summary>
        /// Отмечает завершение обработки документа.
        /// </summary>
        /// <param name="id">Идентификатор документа.</param>
        /// <param name="status">Статус завершения.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        Task UpdateCompletedAsync(Guid id, string status, CancellationToken cancellationToken);
    }   
}
