using Microsoft.AspNetCore.Mvc;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Services;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ManagerController : Controller
    {
        private readonly IDataStore _dataStore;
        private readonly IFileEncryptionService _fileEncryption;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(
            IDataStore dataStore,
            IFileEncryptionService fileEncryption,
            ILogger<ManagerController> logger)
        {
            _dataStore = dataStore;
            _fileEncryption = fileEncryption;
            _logger = logger;
        }

        // GET: Manager/Index
        public IActionResult Index()
        {
            try
            {
                var verifiedClaims = _dataStore.GetCoordinatorVerifiedClaims();
                return View(verifiedClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading verified claims for manager");
                TempData["Error"] = "An error occurred while loading verified claims.";
                return View(new List<Claim>());
            }
        }

        // GET: Manager/Review/5
        public IActionResult Review(int id)
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
                _logger.LogError(ex, $"Error loading claim for review, ID: {id}");
                TempData["Error"] = "An error occurred while loading the claim.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Manager/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string comments)
        {
            try
            {
                var claim = _dataStore.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (claim.Status != ClaimStatus.CoordinatorVerified)
                {
                    TempData["Error"] = "This claim cannot be approved in its current status.";
                    return RedirectToAction(nameof(Index));
                }

                // Update claim status
                claim.Status = ClaimStatus.ManagerApproved;
                claim.ManagerReviewedAt = DateTime.Now;
                claim.ManagerComments = comments ?? "Approved by Academic Manager";

                _dataStore.UpdateClaim(claim);

                TempData["Success"] = $"Claim approved successfully! Total amount: R{claim.TotalAmount:N2}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving claim ID: {id}");
                TempData["Error"] = "An error occurred while approving the claim.";
                return RedirectToAction(nameof(Review), new { id });
            }
        }

        // POST: Manager/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id, string comments)
        {
            try
            {
                var claim = _dataStore.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (claim.Status != ClaimStatus.CoordinatorVerified)
                {
                    TempData["Error"] = "This claim cannot be rejected in its current status.";
                    return RedirectToAction(nameof(Index));
                }

                if (string.IsNullOrWhiteSpace(comments))
                {
                    TempData["Error"] = "Please provide a reason for rejection.";
                    return RedirectToAction(nameof(Review), new { id });
                }

                // Update claim status
                claim.Status = ClaimStatus.ManagerRejected;
                claim.ManagerReviewedAt = DateTime.Now;
                claim.ManagerComments = comments;

                _dataStore.UpdateClaim(claim);

                TempData["Success"] = "Claim rejected successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting claim ID: {id}");
                TempData["Error"] = "An error occurred while rejecting the claim.";
                return RedirectToAction(nameof(Review), new { id });
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

                var decryptedBytes = await _fileEncryption.DecryptFileAsync(document.EncryptedFilePath);

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

        // GET: All Claims (for review history)
        public IActionResult AllClaims()
        {
            try
            {
                var claims = _dataStore.GetAllClaims();
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all claims");
                TempData["Error"] = "An error occurred while loading claims.";
                return View(new List<Claim>());
            }
        }
    }
}