using System.Reflection.Metadata;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class LecturerController : Controller
    {
        private readonly IDataStore _dataStore;
        private readonly IFileEncryptionService _fileEncryption;
        private readonly ILogger<LecturerController> _logger;
        private readonly string _uploadPath;

        // Allowed file types and max size (5MB)
        private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".xlsx" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public LecturerController(
            IDataStore dataStore,
            IFileEncryptionService fileEncryption,
            ILogger<LecturerController> logger,
            IWebHostEnvironment environment)
        {
            _dataStore = dataStore;
            _fileEncryption = fileEncryption;
            _logger = logger;
            _uploadPath = Path.Combine(environment.ContentRootPath, "UploadedDocuments");
        }

        // GET: Lecturer/Index
        public IActionResult Index()
        {
            try
            {
                var claims = _dataStore.GetAllClaims();
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lecturer claims");
                TempData["Error"] = "An error occurred while loading claims. Please try again.";
                return View(new List<Claim>());
            }
        }

        // GET: Lecturer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lecturer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, List<IFormFile> documents)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please correct the errors in the form.";
                    return View(claim);
                }

                // Validate documents if uploaded
                if (documents != null && documents.Any())
                {
                    var validationError = ValidateDocuments(documents);
                    if (!string.IsNullOrEmpty(validationError))
                    {
                        TempData["Error"] = validationError;
                        return View(claim);
                    }
                }

                // Add claim to data store
                _dataStore.AddClaim(claim);

                // Process document uploads
                if (documents != null && documents.Any())
                {
                    foreach (var file in documents)
                    {
                        try
                        {
                            // Encrypt and save file
                            var encryptedPath = await _fileEncryption.EncryptFileAsync(file, _uploadPath);

                            // Create document record
                            var document = new Document
                            {
                                ClaimId = claim.ClaimId,
                                FileName = file.FileName,
                                EncryptedFilePath = encryptedPath,
                                FileSize = file.Length,
                                FileType = Path.GetExtension(file.FileName).ToLower()
                            };

                            _dataStore.AddDocument(document);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error uploading file: {file.FileName}");
                            TempData["Warning"] = $"Claim submitted, but failed to upload file: {file.FileName}";
                        }
                    }
                }

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Details), new { id = claim.ClaimId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating claim");
                TempData["Error"] = "An error occurred while submitting the claim. Please try again.";
                return View(claim);
            }
        }

        // GET: Lecturer/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var claim = _dataStore.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Load documents
                claim.Documents = _dataStore.GetDocumentsByClaimId(id);

                return View(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading claim details for ID: {id}");
                TempData["Error"] = "An error occurred while loading claim details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Lecturer/MyClaims
        public IActionResult MyClaims()
        {
            try
            {
                var claims = _dataStore.GetAllClaims();
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lecturer's claims");
                TempData["Error"] = "An error occurred while loading your claims.";
                return View(new List<Claim>());
            }
        }

        // GET: Download document
        public async Task<IActionResult> DownloadDocument(int id)
        {
            try
            {
                var document = _dataStore.GetDocumentById(id);
                if (document == null)
                {
                    TempData["Error"] = "Document not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Decrypt file
                var decryptedBytes = await _fileEncryption.DecryptFileAsync(document.EncryptedFilePath);

                // Determine content type
                var contentType = document.FileType switch
                {
                    ".pdf" => "application/pdf",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };

                return File(decryptedBytes, contentType, document.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading document ID: {id}");
                TempData["Error"] = "An error occurred while downloading the document.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string? ValidateDocuments(List<IFormFile> documents)
        {
            foreach (var file in documents)
            {
                // Check file size
                if (file.Length > MaxFileSize)
                {
                    return $"File '{file.FileName}' exceeds the maximum size of 5MB.";
                }

                // Check file type
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    return $"File '{file.FileName}' has an invalid type. Only PDF, DOCX, and XLSX files are allowed.";
                }

                // Check if file is empty
                if (file.Length == 0)
                {
                    return $"File '{file.FileName}' is empty.";
                }
            }

            return null;
        }
    }
}
