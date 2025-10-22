Contract Monthly Claim System (CMCS) - Part 2

Student Name: Grace Agbor  
Student Number: ST10066225  
Module: PROG6212 - Programming 2B  
GitHub Repository: [https://github.com/IIEMSA/prog6212-part2-AgborGrace.git
](https://github.com/AgborGrace/PROG6212-Part2-AgborGrace.git)
---

 Table of Contents
1. [Introduction](#introduction)
2. [Changes from Part 1](#changes-from-part-1)
3. [System Features](#system-features)
4. [Installation & Setup](#installation--setup)
5. [How to Use the Application](#how-to-use-the-application)
6. [Technical Implementation](#technical-implementation)
7. [Testing](#testing)
8. [Error Handling](#error-handling)
9. [Security Features](#security-features)
10. [Known Limitations](#known-limitations)

---

## Introduction

The Contract Monthly Claim System (CMCS) is a fully functional ASP.NET Core MVC web application designed to streamline the process of submitting, verifying, and approving monthly claims for independent contractor lecturers. This Part 2 submission builds upon the Part 1 prototype by implementing complete functionality, secure file handling, comprehensive error management, and extensive testing.

---

## Changes from Part 1

Based on lecturer feedback from Part 1, the following improvements have been implemented:

### Design Improvements
- **Enhanced UI/UX:** Modern, intuitive interface with better color schemes and user flow
- **Responsive Design:** Fully responsive layout that works on desktop, tablet, and mobile devices
- **Status Tracking:** Visual progress tracker with percentage bars and timeline views
- **Better Navigation:** Clear navigation menu with role-based access

### Functional Enhancements
- **Real-time Calculation:** Automatic total amount calculation as users input hours and rates
- **File Upload Validation:** Client-side and server-side validation for file types and sizes
- **Status Updates:** Real-time status tracking showing claim progression through approval workflow
- **Document Management:** Secure encrypted file storage with download capability
- **Comprehensive Error Handling:** Meaningful error messages throughout the application

### Technical Improvements
- **Encryption Service:** AES encryption for uploaded documents
- **In-Memory Data Store:** Thread-safe singleton data store for claims and documents
- **Dependency Injection:** Proper service registration and lifecycle management
- **Model Validation:** Comprehensive data annotations for input validation
- **Unit Testing:** 8 comprehensive unit tests covering critical functionality

---

## System Features

### 1. Lecturer Features
- Submit monthly claims with hours worked and hourly rate
- Upload supporting documents (PDF, DOCX, XLSX)
- Track claim status in real-time with visual progress indicators
- View all submitted claims with filtering by status
- Download uploaded documents
- View coordinator and manager comments

### 2. Programme Coordinator Features
- View all pending claims awaiting verification
- Review claim details including uploaded documents
- Verify claims and forward to Academic Manager
- Reject claims with mandatory comments
- View claim history
- Download and review supporting documents

### 3. Academic Manager Features
- View all coordinator-verified claims
- Final approval or rejection authority
- Review complete claim workflow history
- View coordinator verification comments
- Approve claims for payment processing
- Reject claims with detailed reasoning

### 4. Document Management
- Secure file upload with encryption
- File type validation (PDF, DOCX, XLSX only)
- File size limit (5MB per file)
- Multiple document upload support
- Encrypted storage on server
- Secure download with decryption

### 5. Status Tracking
- **Pending:** Claim submitted, awaiting coordinator review
- **Coordinator Verified:** Verified by coordinator, awaiting manager approval
- **Coordinator Rejected:** Rejected by coordinator with comments
- **Manager Approved:** Approved by manager, ready for payment
- **Manager Rejected:** Rejected by manager with comments

---

## Installation & Setup

### Prerequisites
- Visual Studio 2022 (or later)
- .NET 8.0 SDK (or .NET 7.0)
- Git for version control

### Step-by-Step Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/IIEMSA/prog6212-part1-AgborGrace.git
   cd prog6212-part1-AgborGrace
   ```

2. **Open in Visual Studio**
   - Launch Visual Studio
   - File → Open → Project/Solution
   - Select `ContractMonthlyClaimSystem.sln`

3. **Restore NuGet Packages**
   - Visual Studio will automatically restore packages
   - Or manually: Tools → NuGet Package Manager → Restore NuGet Packages

4. **Build the Solution**
   - Build → Build Solution (Ctrl+Shift+B)
   - Ensure no build errors

5. **Run the Application**
   - Press F5 or click the green "Run" button
   - Application will open in your default browser
   - Default URL: `https://localhost:7xxx` (port may vary)

### Folder Structure
```
ContractMonthlyClaimSystem/
├── Controllers/          # MVC Controllers
│   ├── LecturerController.cs
│   ├── CoordinatorController.cs
│   ├── ManagerController.cs
│   └── HomeController.cs
├── Models/              # Data models
│   ├── Claim.cs
│   └── Document.cs
├── Views/               # Razor views
│   ├── Lecturer/
│   ├── Coordinator/
│   ├── Manager/
│   ├── Home/
│   └── Shared/
├── Data/                # Data layer
│   └── InMemoryDataStore.cs
├── Services/            # Business services
│   └── FileEncryptionService.cs
├── wwwroot/             # Static files
│   ├── css/
│   ├── js/
│   └── lib/
├── UploadedDocuments/   # Encrypted files (created at runtime)
└── Program.cs           # Application entry point
```

---

## How to Use the Application

### For Lecturers

1. **Submit a New Claim**
   - Navigate to: Lecturer → Submit New Claim
   - Fill in your details:
     - Full Name
     - Email Address
     - Select Month/Year
     - Enter Hours Worked (1-744)
     - Enter Hourly Rate (R50-R5000)
     - Add optional notes
   - Upload supporting documents (optional)
   - Click "Submit Claim"

2. **View Your Claims**
   - Navigate to: Lecturer → My Claims
   - View dashboard with claim statistics
   - See all claims with current status
   - Click "View" to see detailed information

3. **Track Claim Status**
   - Click on any claim to view details
   - See visual progress tracker
   - View progress bar showing completion percentage
   - Read comments from coordinators and managers

### For Programme Coordinators

1. **Review Pending Claims**
   - Navigate to: Programme Coordinator
   - View list of all pending claims
   - See claim summaries with key information

2. **Verify a Claim**
   - Click "Review" on any pending claim
   - Review all claim details
   - Download and check supporting documents
   - Add optional verification comments
   - Click "Verify and Forward to Manager"

3. **Reject a Claim**
   - In the review screen, scroll to rejection section
   - Enter mandatory rejection reason
   - Click "Reject Claim"
   - Confirm rejection

### For Academic Managers

1. **Review Verified Claims**
   - Navigate to: Academic Manager
   - View all coordinator-verified claims
   - See total amount pending approval

2. **Approve a Claim**
   - Click "Review" on any verified claim
   - Review complete claim information
   - Check coordinator comments
   - Add optional approval comments
   - Click "Approve Claim for Payment"
   - Confirm approval

3. **Reject a Claim**
   - In the review screen, scroll to rejection section
   - Enter mandatory rejection reason
   - Click "Reject Claim"
   - Confirm rejection

---

## Technical Implementation

### Architecture
- **Pattern:** Model-View-Controller (MVC)
- **Framework:** ASP.NET Core 8.0
- **Data Storage:** In-memory thread-safe storage
- **Encryption:** AES 256-bit encryption for files

### Key Components

#### 1. Models
**Claim Model:**
- Properties: ClaimId, LecturerName, LecturerEmail, MonthYear, HoursWorked, HourlyRate, Status, etc.
- Validation: Data annotations for required fields, ranges, and formats
- Calculated Property: TotalAmount (HoursWorked × HourlyRate)

**Document Model:**
- Properties: DocumentId, ClaimId, FileName, EncryptedFilePath, FileSize, FileType
- Relationships: Linked to corresponding Claim

#### 2. Services

**FileEncryptionService:**
- Encrypts uploaded files using AES encryption
- Generates unique filenames with GUIDs
- Stores IV (Initialization Vector) with encrypted file
- Decrypts files for download

**InMemoryDataStore:**
- Thread-safe singleton service
- Stores claims and documents in memory
- Provides CRUD operations
- Includes filtering methods (GetPendingClaims, GetCoordinatorVerifiedClaims)

#### 3. Controllers

**LecturerController:**
- Create: Submit new claims with document upload
- Index/MyClaims: View all submitted claims
- Details: View claim details with status tracking
- DownloadDocument: Download encrypted documents

**CoordinatorController:**
- Index: View pending claims
- Review: Review claim details
- Verify: Approve and forward to manager
- Reject: Reject claim with comments

**ManagerController:**
- Index: View verified claims
- Review: Review claim details
- Approve: Final approval for payment
- Reject: Final rejection

#### 4. Views
- Responsive Bootstrap 5 design
- Font Awesome icons for visual appeal
- Client-side JavaScript for real-time calculations
- Form validation with error messages
- Progress trackers and status badges

### Validation

**Server-Side Validation:**
- Data annotations on models
- ModelState validation in controllers
- File type and size validation
- Business rule validation

**Client-Side Validation:**
- jQuery validation
- Real-time form feedback
- File upload preview
- Calculation updates

### Error Handling

**Global Error Handling:**
- Try-catch blocks in all controller actions
- Meaningful error messages via TempData
- Logging using ILogger interface
- User-friendly error displays

**File Upload Errors:**
- File size limit enforcement
- File type validation
- Empty file detection
- Encryption/decryption error handling

---

## Testing

### Unit Tests (8 Tests Implemented)

1. **Test_Claim_TotalAmount_CalculatesCorrectly**
   - Verifies automatic calculation of total amount
   - Tests: 100 hours × R450 = R45,000

2. **Test_DataStore_AddClaim_IncreasesClaimCount**
   - Verifies claims are added to data store
   - Checks claim count increases

3. **Test_DataStore_UpdateClaim_ChangesStatus**
   - Verifies claim status updates
   - Checks status change from Pending to Verified

4. **Test_DataStore_AddDocument_LinksToCorrectClaim**
   - Verifies documents link to claims
   - Checks document retrieval by ClaimId

5. **Test_Claim_Validation_RequiresLecturerName**
   - Verifies required field validation
   - Tests empty lecturer name fails validation

6. **Test_Claim_Validation_HoursWorkedWithinRange**
   - Verifies range validation
   - Tests hours > 744 fails validation

7. **Test_DataStore_GetPendingClaims_ReturnsOnlyPending**
   - Verifies filtering functionality
   - Checks only pending claims returned

8. **Test_DataStore_GetCoordinatorVerifiedClaims_ReturnsOnlyVerified**
   - Verifies filtering functionality
   - Checks only verified claims returned

### Running Tests

**In Visual Studio:**
1. Open Test Explorer: Test → Test Explorer
2. Click "Run All Tests"
3. View results in Test Explorer window

**Via Command Line:**
```bash
dotnet test
```

### Manual Testing Checklist

- [ ] Submit claim with valid data
- [ ] Submit claim with invalid data (test validation)
- [ ] Upload documents (PDF, DOCX, XLSX)
- [ ] Try to upload invalid file types
- [ ] Try to upload files > 5MB
- [ ] Verify claim as coordinator
- [ ] Reject claim as coordinator
- [ ] Approve claim as manager
- [ ] Reject claim as manager
- [ ] Download encrypted documents
- [ ] Track claim status updates
- [ ] Test responsive design on mobile

---

## Error Handling

### User-Facing Errors
- **Success Messages:** Green alerts for successful actions
- **Error Messages:** Red alerts for failures
- **Warning Messages:** Yellow alerts for partial success
- **Validation Errors:** Inline red text under form fields

### Common Error Scenarios

1. **File Upload Errors:**
   - "File exceeds maximum size of 5MB"
   - "Invalid file type. Only PDF, DOCX, and XLSX allowed"
   - "File is empty"

2. **Validation Errors:**
   - "Lecturer name is required"
   - "Hours must be between 1 and 744"
   - "Hourly rate must be between R50 and R5000"

3. **Business Logic Errors:**
   - "Claim not found"
   - "This claim has already been processed"
   - "Please provide a reason for rejection"

---

## Security Features

### File Encryption
- **Algorithm:** AES (Advanced Encryption Standard)
- **Key Size:** 256-bit
- **Mode:** CBC (Cipher Block Chaining)
- **IV:** Randomly generated per file

### File Validation
- File type whitelist (PDF, DOCX, XLSX)
- File size limit (5MB)
- Encrypted storage
- Secure file naming with GUIDs

### Input Validation
- Server-side validation for all inputs
- SQL injection prevention (no database used)
- XSS prevention through Razor encoding
- CSRF tokens on all forms

---

## Known Limitations

1. **No Database:** Uses in-memory storage; data is lost on application restart
2. **No Authentication:** No login system implemented (as per requirements)
3. **No Email Notifications:** Status updates not sent via email
4. **Single Server:** No support for distributed/load-balanced environments
5. **File Storage:** Encrypted files stored locally, not in cloud storage

---

## Future Enhancements (Part 3)

1. Implement database with Entity Framework Core
2. Add user authentication and role-based authorization
3. Email notifications for status changes
4. PDF report generation
5. Dashboard analytics and statistics
6. Claim history export to Excel
7. Automated hourly rate lookup
8. Integration with payroll systems

---





---



 References
- Microsoft. (2024). ASP.NET Core MVC Documentation. https://learn.microsoft.com/aspnet/core/mvc
- Microsoft. (2024). Data Encryption in .NET. https://learn.microsoft.com/dotnet/standard/security/cryptography-model
- Bootstrap Team. (2024). Bootstrap 5 Documentation. https://getbootstrap.com/docs/5.3/

---



