using Microsoft.AspNetCore.Mvc;
using DocumentManagementlibrary.Models;
using DocumentManagementlibrary.RequestModel;
using DocumentManagementlibrary.Services;

namespace DocumentManagementApi.Controllers
{
    [ApiController]
    [Route("api/documents/[action]")]
    public class AddDocumentController : ControllerBase
    {
        private const long MaxFileSize = 10 * 1024 * 1024; 
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".docx", ".xlsx", ".txt", ".png", ".jpg", ".jpeg"
        };

        private readonly AddDocumentServices _services;

        public AddDocumentController(AddDocumentServices services)
        {
            _services = services;
        }

        private static string UploadFolder => Path.Combine(Directory.GetCurrentDirectory(), "Documents");
        private static object ToResponse(DocumentModel d) => new
        {
            id = d.Id,
            originalFileName = d.OriginalFileName,
            contentType = d.ContentType,
            fileSize = d.FileSize,
            uploadedAtUtc = d.UploadedAtUtc
        };
        [HttpPost]
        public IActionResult AddDocument(IFormFile file)
        {
            var originalFileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(originalFileName);
            if (file == null || file.Length == 0)   return BadRequest(new { error = "No file uploaded or file is empty." });
            if (file.Length > MaxFileSize) return StatusCode(StatusCodes.Status413PayloadTooLarge,  new { error = "File exceeds the 10 MB limit." });
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))  return StatusCode(StatusCodes.Status415UnsupportedMediaType, new { error = $"File type '{extension}' is not supported." });

            Directory.CreateDirectory(UploadFolder);
            var storedFileName = Guid.NewGuid() + extension.ToLowerInvariant();
            var fullPath = Path.Combine(UploadFolder, storedFileName);

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.CreateNew))  { file.CopyTo(stream); }

                int id = _services.AddDocument(new DocumentRequestModel
                {
                    OriginalFileName = originalFileName,
                    StoredFileName = storedFileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length
                });

                return Ok(new
                {
                    id,
                    originalFileName,
                    contentType = file.ContentType,
                    fileSize = file.Length
                });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_services.GetAllDocuments().Select(ToResponse));
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var doc = _services.GetDocumentById(id);
            if (doc == null)   return NotFound(new { error = $"Document {id} was not found." });  return Ok(ToResponse(doc));
        }

      
        [HttpGet]
        public IActionResult Download(int id)
        {
            var doc = _services.GetDocumentById(id);
            if (doc == null)     return NotFound(new { error = $"Document {id} was not found." });

            var fullPath = Path.Combine(UploadFolder, Path.GetFileName(doc.StoredFileName));
            if (!System.IO.File.Exists(fullPath))  return NotFound(new { error = "The document record exists, but its file was not found in storage." });
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, doc.ContentType, doc.OriginalFileName);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                _services.DeleteDocument(id);
            }
            catch(Exception ex) {
                throw ex;
            }
                
        

            return NoContent();
        }
    }
}