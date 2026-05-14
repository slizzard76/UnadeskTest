using DataAccess.Models;

namespace UnadeskTest.BusinessLogic.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для работы с документами.
    /// Определяет методы для создания, получения, обновления и управления документами.
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Создает новый документ в системе.
        /// </summary>
        /// <param name="fileName">Имя файла документа.</param>
        /// <param name="savePath">Путь для сохранения документа.</param>
        /// <param name="cancellation">Токен отмены операции.</param>
        /// <returns>Задача, возвращающая уникальный идентификатор (Guid) нового документа.</returns>
        Task<Guid> CreateNewDocumentAsync(string fileName, string savePath, CancellationToken cancellationToken);

        /// <summary>
        /// Получает детали конкретного документа по его идентификатору.
        /// </summary>
        /// <param name="id">Уникальный идентификатор документа.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Задача, возвращающая объект PdfDocument с деталями.</returns>
        Task<PdfDocument> GetDocumentDetailsAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Получает список всех доступных документов.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Задача, возвращающая коллекцию всех документов.</returns>
        Task<IEnumerable<PdfDocument>> GetAllDocumentsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Обновляет статус документа.
        /// </summary>
        /// <param name="id">Уникальный идентификатор документа.</param>
        /// <param name="status">Новый статус документа (например, "Обработан", "Черновик").</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Задача, возвращающая <c>true</c>, если обновление прошло успешно.</returns>
        Task<bool> UpdateStatusAsync(Guid id, FileStatus status, CancellationToken cancellationToken);

        /// <summary>
        /// Отмечает документ как завершенный (завершает обработку).
        /// </summary>
        /// <param name="id">Уникальный идентификатор документа.</param>
        /// <param name="status">Статус завершения (может быть использован для логирования).</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Задача, которая не возвращает значения (void).</returns>
        Task UpdateCompletedAsync(Guid id, string status, CancellationToken cancellationToken);
    }   
}
