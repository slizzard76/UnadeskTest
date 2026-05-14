using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using UnadeskTest.BusinessLogic.Services;

namespace UnadeskTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // Контроллер для управления документами (PDF).
    public class DocumentsController : ControllerBase
    {
        
        // Сервис для бизнес-логики работы с документами.
        private readonly IDocumentService _documentService;
        
        
        // Конструктор контроллера. Внедряет зависимость IDocumentService.
        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// 1. Загрузка PDF и инициирование процесса.
        /// Принимает файл, сохраняет его и запускает асинхронную обработку.
        /// </summary>
        /// <param name="file">Загруженный файл (IFormFile).</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Результат операции: Accepted, если процесс запущен.</returns>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadPdf(IFormFile file, CancellationToken cancellationToken)
        {
            // Проверка наличия файла.
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            byte[] fileBytes;
            // Используем MemoryStream для чтения содержимого файла в массив байтов.
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            // Инициируем создание документа и его обработку в фоновом режиме.
            var documentId = await _documentService.CreateNewDocumentAsync(file.FileName, fileBytes, cancellationToken);
            
            // Возвращаем статус Accepted, указывая, что процесс запущен.
            return Accepted(new
            {
                Message = "Document processing initiated.",
                DocumentId = documentId
            });
        }

        /// <summary>
        /// Получение списка всех документов.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Список объектов PdfDocument.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PdfDocument>>> GetDocuments(CancellationToken cancellationToken)
        {
            // Получаем все документы из сервиса.
            var documents = await _documentService.GetAllDocumentsAsync(cancellationToken);
            return Ok(documents);
        }

        /// <summary>
        /// Получение текстового содержимого по ID.
        /// Проверяет статус документа перед предоставлением текста.
        /// </summary>
        /// <param name="id">GUID идентификатор документа.</param>
        /// <param name="cancellationToken">Токен отмены операции.</param>
        /// <returns>Текстовое содержимое документа или соответствующий код ошибки.</returns>
        [HttpGet("{id:Guid}/text")]
        public async Task<ActionResult<string>> GetDocumentText(Guid id, CancellationToken cancellationToken)
        {
            // Получаем детали документа.
            var document = await _documentService.GetDocumentDetailsAsync(id, cancellationToken);
            
            // Проверка существования документа.
            if (document == null) return NotFound("Document not found.");

            // Проверка статуса: текст доступен только после завершения обработки.
            if (document.FileStatus != FileStatus.Completed)
            {
                return StatusCode(409, $"Processing status is currently {document.FileStatus}.");
            }

            // Проверка наличия самого текстового контента.
            if (string.IsNullOrWhiteSpace(document.TextContent))
            {
                return NotFound("Text content is not available.");
            }

            // Возвращаем успешно извлеченный текст.
            return Ok(document.TextContent);
        }
    }   
}