using DataAccess.Context;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace UnadeskTest.BusinessLogic.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly UnadeskDbContext _context;

        // Конструктор репозитория. Принимает контекст базы данных.
        public DocumentRepository(UnadeskDbContext context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<Guid> CreateNewDocumentAsync(string originalFileName, CancellationToken cancellationToken)
        {
            var newDoc = new PdfDocument
            {
                Id = Guid.NewGuid(),
                OriginalFileName = originalFileName,
                FileStatus = FileStatus.Pending, // Изначальный статус: Ожидание обработки
                TextContent = null,
            };

            _context.Documents.Add(newDoc);
            await _context.SaveChangesAsync();
            return newDoc.Id;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<PdfDocument>> GetAllDocumentsAsync(CancellationToken cancellationToken)
        {
            // Извлекает все записи из таблицы Documents. Обрезаем контент для читаемости результата.
            var result = await _context.Documents.
                Select(x => new PdfDocument()
                {
                    Id = x.Id,
                    FileStatus = x.FileStatus,
                    OriginalFileName = x.OriginalFileName,
                    ProcessedOn = x.ProcessedOn,
                }).ToListAsync(cancellationToken);
            return result;
        }

        ///<inheritdoc/>
        public async Task<PdfDocument> GetDocumentDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            // Использует FindAsync для быстрого поиска по первичному ключу.
            return await _context.Documents.FindAsync(id, cancellationToken);
        }

        ///<inheritdoc/>
        public async Task<bool> UpdateStatusAsync(Guid documentId, FileStatus status, CancellationToken  cancellationToken)
        {
            // Получаем текущую запись документа.
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return false;

            // Обновляем только поле статуса.
            document.FileStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        ///<inheritdoc/>
        public async Task UpdateCompletedAsync(Guid documentId, string textContent, CancellationToken cancellationToken)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return;

            // Обновляем все поля, связанные с завершением обработки.
            document.TextContent = textContent;
            document.FileStatus = FileStatus.Completed;
            document.ProcessedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }   
}

