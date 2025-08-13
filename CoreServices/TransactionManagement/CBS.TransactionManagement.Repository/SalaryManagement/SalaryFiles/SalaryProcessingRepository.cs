using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileUploadP;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles
{

    public class SalaryProcessingRepository : GenericRepository<SalaryExtract, TransactionContext>, ISalaryExecutedRepository
    {
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly ISalaryAnalysisResultRepository _salaryAnalysisResultRepository;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<SalaryProcessingRepository> _logger;

        public SalaryProcessingRepository(
            IUnitOfWork<TransactionContext> unitOfWork,
            IFileUploadRepository fileUploadRepository,
            ILogger<SalaryProcessingRepository> logger,
            PathHelper pathHelper,
            UserInfoToken userInfoToken,
            ISalaryAnalysisResultRepository salaryAnalysisResultRepository) : base(unitOfWork)
        {
            _uow = unitOfWork;
            _fileUploadRepository = fileUploadRepository;
            _logger = logger;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
            _salaryAnalysisResultRepository = salaryAnalysisResultRepository;
        }

        public async Task<bool> ExtractExcelDataToDatabase(IFormFile file, string branchId, string branchName, string branchCode, string fileUploadId)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber();
            var salaryAnalysisResult = await _salaryAnalysisResultRepository
                .FindBy(x => x.FileUploadId == fileUploadId && x.BranchId==_userInfoToken.BranchID).Include(x=>x.salaryAnalysisResultDetails)
                .FirstOrDefaultAsync();

            var fileUpload = await _fileUploadRepository.FindAsync(fileUploadId);

            if (salaryAnalysisResult == null)
            {
                string errorMessage = $"No Salary Analysis Result found for Salary File: {fileUpload.FileName}. Please ensure the file has been uploaded and analyzed correctly.";
                throw new InvalidOperationException(errorMessage);
            }


            // Proceed with processing if salaryAnalysisResult is not null
            try
            {
                string startMessage = $"Starting Excel data extraction for file: {file.FileName}. Branch: {branchCode} - {branchName}.";
                await BaseUtilities.LogAndAuditAsync(startMessage, null, HttpStatusCodeEnum.OK, LogAction.SalaryFileUploading, LogLevelInfo.Information, logReference);

                ValidateFileFormat(file); // Validate file type

                var fileHash = BaseUtilities.ComputeFileHash(file); // Generate a file hash
                await CheckDuplicateFile(fileHash); // Check for duplicate file upload

                var filePath = await SaveUploadedFile(file, branchCode); // Save the uploaded file locally
                string NewfileUploadId = BaseUtilities.GenerateInsuranceUniqueNumber(10, $"SM{_userInfoToken.BranchCode}");
                // Process the file and extract data
                var salaryExtracts = await ProcessExcelFile(file, branchId, branchName, fileUpload, branchCode, salaryAnalysisResult, NewfileUploadId);

                // Save extracted salary data to the database
                SaveSalaryExtractData(salaryExtracts);

                // Save file upload details
                await _fileUploadRepository.SaveFileUploadDetails(file, filePath, fileHash, branchId, branchName, NewfileUploadId, FileCategory.SalaryAnalysisExtract, SalaryProcessingStatus.Extraction, NewfileUploadId);

                // Save changes to the database
                await _uow.SaveAsync();

                string successMessage = $"Successfully completed salary data extraction for file: {file.FileName}. Extracted {salaryExtracts.Count} records.";
                await BaseUtilities.LogAndAuditAsync(successMessage, null, HttpStatusCodeEnum.OK, LogAction.SalaryFileUploading, LogLevelInfo.Information, logReference);

                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error during Excel data extraction for file: {file.FileName}. Branch: {branchCode} - {branchName}. Error: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryFileUploading, LogLevelInfo.Error, logReference);
                throw new InvalidOperationException(errorMessage);
            }
        }



        // Validates the file format (must be .xlsx or .xls)
        private void ValidateFileFormat(IFormFile file)
        {
            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            {
                string errorMessage = $"Invalid file format: {file.FileName}. Only Excel files (.xlsx, .xls) are supported.";
                BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }



        // Checks if the file has already been processed based on its hash
        private async Task CheckDuplicateFile(string fileHash)
        {
            if (await _fileUploadRepository.FindBy(f => f.FileHash == fileHash).AnyAsync())
            {
                string errorMessage = "Duplicate file upload detected. This file has already been processed.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }

        private async Task<string> SaveUploadedFile(IFormFile file, string branchCode)
        {
            var directoryPath = _pathHelper.FileUploadPath;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = System.IO.Path.Combine(directoryPath, $"{branchCode}_{file.FileName}");

            await SaveFile(file, filePath);

            await BaseUtilities.LogAndAuditAsync($"File saved to {filePath}.", null, HttpStatusCodeEnum.OK, LogAction.SalaryFileUploading, LogLevelInfo.Information);

            return filePath;
        }

        // Add a method to calculate the sum of values from columns D to S
        private decimal GetSumOfSpecificColumns(IXLRow row)
        {
            decimal sum = 0;

            // Sum of columns 6 to 10 (columns F to J, assuming column indices are 1-based)
            for (int i = 6; i <= 10; i++)
            {
                sum += BaseUtilities.GetDecimalFromCell(row.Cell(i));
            }

            // Add columns 14 and 15 (columns N and O)
            sum += BaseUtilities.GetDecimalFromCell(row.Cell(14)); // Column 14
            sum += BaseUtilities.GetDecimalFromCell(row.Cell(15)); // Column 15

            decimal repaymentdetail = 0;

            // Sum of columns 11 to 13 (columns K to M)
            for (int i = 11; i <= 13; i++)
            {
                repaymentdetail += BaseUtilities.GetDecimalFromCell(row.Cell(i));
            }

            decimal totalRepayment = BaseUtilities.GetDecimalFromCell(row.Cell(10)); // Column 10 (assuming column J)

            // Check if total repayment matches the repayment detail
            if (totalRepayment != repaymentdetail)
            {
                // Throw an exception with a detailed message
                string errorMessage = $"Validation error at row {row.RowNumber()}: Total repayment (Column 10) = {totalRepayment} does not match the sum of repayment details (Columns 11-13) = {repaymentdetail}. These values must be equal.";
                throw new InvalidOperationException(errorMessage);
            }

            return sum;
        }


        private decimal GetTotalNetSalary(IXLWorksheet worksheet,int rowCount,int endRow)
        {
            decimal totalNetSalary = 0;
            var rows = worksheet.Rows().GetEnumerator();
            // Loop through all the rows (skip the header rows as needed)
            while (rows.MoveNext())
            {
               
                var row = worksheet.Row(rowCount);
                // Add the NetSalary (Column 4) to the total
                totalNetSalary += BaseUtilities.GetDecimalFromCell(row.Cell(4)); // Column 4 corresponds to NetSalary
                rowCount++;

                if (rowCount > endRow)
                {
                    break;
                }
            }

            return totalNetSalary;
        }
        // Processes the uploaded Excel file and extracts salary data
        public async Task<List<SalaryExtractDto>> ProcessExcelFile(IFormFile file, string branchId, string branchName, FileUpload fileUpload, string branchCode, SalaryAnalysisResult salaryAnalysisResult, string NewfileUploadId)
        {
            var salaryExtracts = new List<SalaryExtractDto>();

            if (salaryAnalysisResult == null)
            {
                throw new InvalidOperationException($"No Salary Analysis Result found for Salary File: {fileUpload.FileName}. Please ensure the file has been uploaded and analyzed correctly.");
            }

            if (salaryAnalysisResult.BranchId != _userInfoToken.BranchID)
            {
                string errorMessage = $"The Salary Analysis Result for file '{fileUpload.FileName}' does not belong to your branch: {_userInfoToken.BranchName}. Your branch Code: {_userInfoToken.BranchCode}, but the file is associated with branch: {salaryAnalysisResult.BranchName} & Code: {salaryAnalysisResult.BranchCode}. Please ensure you are processing the correct file.";
                throw new InvalidOperationException(errorMessage);
            }

            int totalMembersInFile = 0;
            decimal totalNetSalaryInFile = 0;
            int initialExpectedNumberOfRecords = 0;
            int rowCounter = 0;
            decimal sumOfNetSalariesInFile = 0;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Access the first worksheet

                    // Get the total sum of NetSalary in the file
                  
                    var totalNetSalary = salaryAnalysisResult.TotalNetSalary;
                    var totalMembers = salaryAnalysisResult.TotalMembers;

                    var rows = worksheet.Rows().GetEnumerator();
                     rowCounter = 1;

                    

                    while (rows.MoveNext())
                    {
                        var row = worksheet.Row(rowCounter);
                       // rowCounter++;
                        if (row.Cell(1).GetString().Trim().Contains("MATRICULE") && row.Cell(2).GetString().Trim().Contains("MEMBER REFERENCE")&& row.Cell(3).GetString().Contains("MEMBER NAME"))
                        {
                            break;
                        }

                        rowCounter++;
                        if (rowCounter == 50)
                        {
                            string errorMessage = $"The header 'MATRICULE' was not found within the first 50 rows of the file.";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                            throw new InvalidOperationException(errorMessage);
                        }
                    }

                    int expectedNumberOfRecords = rowCounter + salaryAnalysisResult.salaryAnalysisResultDetails.Count;

                    initialExpectedNumberOfRecords = expectedNumberOfRecords;

                    await  ValidateExcelHeaderAsync(worksheet.Row(rowCounter));

                    sumOfNetSalariesInFile= GetTotalNetSalary(worksheet,rowCounter+1, expectedNumberOfRecords);

                    foreach (var row in worksheet.Rows().Skip(rowCounter)) // Skip the first few header rows
                    {
                        // Stop processing if the current row is the footer row containing "TOTAL"
                        if (row.Cell(1).GetString().Contains("TOTAL"))
                        {
                            break; // Exit the loop if we encounter the footer row
                        }

                        // Stop processing if the current row is empty
                        if (IsRowEmpty(row))
                        {
                            break; // Exit the loop if we encounter an empty row
                        }

                        if (!row.Cell(1).GetString().Contains("TOTAL") && expectedNumberOfRecords < 0)
                        {

                            string errorMessage = $"Number of records Mismatch";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                            throw new InvalidOperationException(errorMessage);
                        }



                        var salaryAnalysisResultDetail = await ValidateMatriculeAsync(salaryAnalysisResult, row.Cell(1).GetString());
                        await ValidateCustomerReferenceNumberAsync(salaryAnalysisResultDetail, row.Cell(2).GetString());
                        await ValidateMemberNameAsync(salaryAnalysisResultDetail, row.Cell(3).GetString());
                        await ValidateNetSalaryAsync(salaryAnalysisResultDetail, row.Cell(4).GetString());
                        await ValidateStandingOrderAsync(salaryAnalysisResultDetail, row.Cell(5).GetString());
                        await ValidateSavingsAsync(salaryAnalysisResultDetail, row.Cell(6).GetString());
                        await ValidateDepositAsync(salaryAnalysisResultDetail, row.Cell(7).GetString());
                        await ValidatePreferencesSharesAsync(salaryAnalysisResultDetail, row.Cell(8).GetString());
                        await ValidateLoanRepaymentAsync(salaryAnalysisResultDetail, row.Cell(10).GetString());
                        await ValidateLoanIdAsync(salaryAnalysisResultDetail, row.Cell(16).GetString());
                        await ValidateLoanTypeAsync(salaryAnalysisResultDetail, row.Cell(17).GetString());
                        await ValidateStandingOrderStatementAsync(salaryAnalysisResultDetail, row.Cell(18).GetString());
                        await ValidateStatusAsync(salaryAnalysisResultDetail, row.Cell(19).GetString());
                        await ValidateIsMigratedLoanAsync(salaryAnalysisResultDetail, row.Cell(20).GetString());

                        await ValidateProductNameAsync(salaryAnalysisResultDetail, row.Cell(21).GetString());
                        await ValidateProductIdAsync(salaryAnalysisResultDetail, row.Cell(22).GetString());


                        var netSalaryPerMember = BaseUtilities.GetDecimalFromCell(row.Cell(4)); // Column 4 corresponds to NetSalary
                        var sumOfDToSPerMember = GetSumOfSpecificColumns(row); // Calculate the sum of columns 5 to 9, 13, and 14

                        // Validate that NetSalary equals the sum of deductions
                        if (netSalaryPerMember != sumOfDToSPerMember)
                        {
                            string errorMessage = $"Validation error at row {row.RowNumber()}: Net Salary (Column 4 'D') = {BaseUtilities.FormatCurrency(netSalaryPerMember)}, but the sum of deductions (Columns F:J, N:O) = {BaseUtilities.FormatCurrency(sumOfDToSPerMember)}. These values must be equal.";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                            throw new InvalidOperationException(errorMessage);
                        }

                        // Update the total members and total net salary
                      
                        totalNetSalaryInFile += netSalaryPerMember;

                        var salaryExtract = new SalaryExtractDto
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            Matricule = row.Cell(1).GetString(),
                            MemberReference = row.Cell(2).GetString(),
                            MemberName = row.Cell(3).GetString(),
                            NetSalary = netSalaryPerMember,
                            Saving = BaseUtilities.GetDecimalFromCell(row.Cell(6)), // Savings from column 5
                            Deposit = BaseUtilities.GetDecimalFromCell(row.Cell(7)), // Deposit from column 6
                            Shares = BaseUtilities.GetDecimalFromCell(row.Cell(8)), // Ordinary Shares from column 7
                            PreferenceShares = BaseUtilities.GetDecimalFromCell(row.Cell(9)), // Preference Shares from column 8
                            TotalLoanRepayment = BaseUtilities.GetDecimalFromCell(row.Cell(10)), // Loan Repayment from column 9
                            LoanCapital = BaseUtilities.GetDecimalFromCell(row.Cell(11)), // Loan Capital from column 10
                            LoanInterest = BaseUtilities.GetDecimalFromCell(row.Cell(12)), // Loan Interest from column 11
                            VAT = BaseUtilities.GetDecimalFromCell(row.Cell(13)), // VAT from column 12
                            Charges = BaseUtilities.GetDecimalFromCell(row.Cell(14)), // Charges from column 13
                            Salary = BaseUtilities.GetDecimalFromCell(row.Cell(15)), // Salary Balance from column 14
                            BranchId = branchId,
                            SalaryAnalysisResultId = salaryAnalysisResult.Id,
                            BranchName = branchName,
                            FileUploadId = NewfileUploadId,
                            FileUploadIdReferenceId = fileUpload.Id,
                            AccountingDate = DateTime.MinValue,
                            BranchCode = branchCode,
                            ExecutedBy = _userInfoToken.FullName,
                            ExecutionDate = DateTime.MinValue,
                            ExtrationDate = BaseUtilities.UtcNowToDoualaTime(),
                            LoanId = row.Cell(16).GetString(),  // Loan Id from column 16
                            LoanType = row.Cell(17).GetString(),  // Loan Type from column 17
                            StandingOrderAmount = BaseUtilities.GetDecimalFromCell(row.Cell(5)),  // Standing Order from column 5 (adjusted position)
                            StandingOrderStatement = row.Cell(18).GetString(),  // Standing Order Statement from column 18
                            Status = row.Cell(19).GetString() == "Final_Analysis", // Status from column 19
                            IsOnldLoan = row.Cell(20).GetString() == "Yes" ? true : false, // Status from column 19
                            LoanProductName = row.Cell(21).GetString(),
                            LoanProductId = row.Cell(22).GetString(),
                            UploadedBy = _userInfoToken.FullName,
                            RemainingSalary = BaseUtilities.GetDecimalFromCell(row.Cell(15)) // Same as Salary
                        };

                        salaryExtracts.Add(salaryExtract);

                        expectedNumberOfRecords--;
                    }
                }
            }


            totalMembersInFile = initialExpectedNumberOfRecords - rowCounter;
            // Perform validation to ensure the total members and total net salary match
            if (totalMembersInFile != Convert.ToInt32(salaryAnalysisResult.TotalMembers))
            {
                string errorMessage = $"Total members in the file ({totalMembersInFile}) does not match the expected total ({salaryAnalysisResult.TotalMembers}) in the Salary Analysis Result.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
            
            if (sumOfNetSalariesInFile != salaryAnalysisResult.TotalNetSalary)
            {
                string errorMessage = $"Total net salary in the file ({BaseUtilities.FormatCurrency(sumOfNetSalariesInFile)}) does not match the expected total ({BaseUtilities.FormatCurrency(salaryAnalysisResult.TotalNetSalary)}) in the Salary Analysis Result.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            string successMessage = $"Processed {salaryExtracts.Count} salary records from the Excel file.";
            await BaseUtilities.LogAndAuditAsync(successMessage, null, HttpStatusCodeEnum.OK, LogAction.SalaryFileUploading, LogLevelInfo.Information);

            return salaryExtracts;
        }


        private async Task<SalaryAnalysisResultDetail> ValidateMatriculeAsync(SalaryAnalysisResult salaryAnalysisResult, string matricule)
        {
            // Null or Empty check for input Matricule
            if (string.IsNullOrWhiteSpace(matricule))
            {
                string errorMessage = "Validation failed: Matricule cannot be null or empty.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Ensure salaryAnalysisResult and its details are not null
            if (salaryAnalysisResult?.salaryAnalysisResultDetails == null || !salaryAnalysisResult.salaryAnalysisResultDetails.Any())
            {
                string errorMessage = "Validation failed: Salary Analysis Result details are missing or empty.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            // Find matching record
            var salaryAnalysisResultDetail = salaryAnalysisResult.salaryAnalysisResultDetails
                .FirstOrDefault(x => string.Equals(x.Matricule, matricule.Trim(), StringComparison.OrdinalIgnoreCase));

            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = $"Validation failed: Matricule '{matricule}' not found in the file.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            return salaryAnalysisResultDetail;
        }



        private async Task ValidateIsMigratedLoanAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string isMigratedLoan)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(isMigratedLoan))
            {
                string errorMessage = $"Validation failed: 'Is Migrated Loan' cannot be empty for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize input values (trim spaces & make case-insensitive)
            string expectedValue = salaryAnalysisResultDetail.IsOnldLoan ? "Yes" : "No";
            string providedValue = isMigratedLoan.Trim();

            if (!expectedValue.Equals(providedValue, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Is Migrated Loan' to be '{expectedValue}', but found '{providedValue}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidateLoanTypeAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileLoanType)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileLoanType))
            {
                string errorMessage = $"Validation failed: 'Loan Type' cannot be empty for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize values for comparison
            string expectedLoanType = salaryAnalysisResultDetail.LoanType?.Trim();
            string providedLoanType = fileLoanType.Trim();

            if (!string.Equals(expectedLoanType, providedLoanType, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Loan Type' to be '{expectedLoanType}', but found '{providedLoanType}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidateMemberNameAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileMemberName)
        {
            if (string.IsNullOrWhiteSpace(salaryAnalysisResultDetail.MemberName) || string.IsNullOrWhiteSpace(fileMemberName))
            {
                string errorMessage = $"Validation failed: Member Name is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            string sanitizedMemberName = NormalizeName(salaryAnalysisResultDetail.MemberName);
            string sanitizedFileMemberName = NormalizeName(fileMemberName);

            if (!sanitizedMemberName.Equals(sanitizedFileMemberName, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Member Name' to be '{sanitizedMemberName}', but found '{sanitizedFileMemberName}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }




        private async Task ValidateStandingOrderStatementAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileStandingOrderStatement)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileStandingOrderStatement))
            {
                string errorMessage = $"Validation failed: 'Standing Order Statement' cannot be empty for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize values for comparison
            string expectedStatement = salaryAnalysisResultDetail.StandingOrderStatement?.Trim();
            string providedStatement = fileStandingOrderStatement.Trim();

            if (!string.Equals(expectedStatement, providedStatement, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Standing Order Statement' to be '{expectedStatement}', but found '{providedStatement}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }



        private async Task ValidateNetSalaryAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileNetSalaryStr)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileNetSalaryStr))
            {
                string errorMessage = $"Validation failed: 'Net Salary' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Parse file net salary safely
            if (!decimal.TryParse(fileNetSalaryStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fileNetSalary))
            {
                string errorMessage = $"Validation failed: Unable to parse 'Net Salary' value '{fileNetSalaryStr}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new FormatException(errorMessage);
            }

            // Get the expected Net Salary from database
            decimal expectedNetSalary = salaryAnalysisResultDetail.NetSalary;

            // Compare values with a tolerance to avoid floating-point precision issues
            if (Math.Abs(expectedNetSalary - fileNetSalary) > 0.01m) // Adjust precision as needed
            {
                string errorMessage = $"Validation failed: Expected 'Net Salary' to be '{expectedNetSalary:N2}', but found '{fileNetSalary:N2}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidateStandingOrderAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileStandingOrderStr)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileStandingOrderStr))
            {
                string errorMessage = $"Validation failed: 'Standing Order' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Parse file standing order safely
            if (!decimal.TryParse(fileStandingOrderStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fileStandingOrder))
            {
                string errorMessage = $"Validation failed: Unable to parse 'Standing Order' value '{fileStandingOrderStr}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new FormatException(errorMessage);
            }

            // Get the expected Standing Order amount from database
            decimal expectedStandingOrder = salaryAnalysisResultDetail.StandingOrderAmount;

            // Compare values with a tolerance to handle floating-point precision issues
            if (Math.Abs(expectedStandingOrder - fileStandingOrder) > 0.01m) // Adjust precision tolerance if necessary
            {
                string errorMessage = $"Validation failed: Expected 'Standing Order' to be '{expectedStandingOrder:N2}', but found '{fileStandingOrder:N2}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }

        private async Task ValidateSalaryBalanceAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileSalaryBalanceStr)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileSalaryBalanceStr))
            {
                string errorMessage = $"Validation failed: 'Salary Balance' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Parse file salary balance safely
            if (!decimal.TryParse(fileSalaryBalanceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fileSalaryBalance))
            {
                string errorMessage = $"Validation failed: Unable to parse 'Salary Balance' value '{fileSalaryBalanceStr}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new FormatException(errorMessage);
            }

            // Get the expected salary balance from the database
            decimal expectedSalaryBalance = salaryAnalysisResultDetail.RemainingSalary;

            // Compare values with a tolerance for floating-point precision issues
            if (Math.Abs(expectedSalaryBalance - fileSalaryBalance) > 0.01m) // Adjust tolerance if necessary
            {
                string errorMessage = $"Validation failed: Expected 'Salary Balance' to be '{expectedSalaryBalance:N2}', but found '{fileSalaryBalance:N2}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidateLoanRepaymentAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileLoanRepaymentStr)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileLoanRepaymentStr))
            {
                string errorMessage = $"Validation failed: 'Loan Repayment' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Parse file loan repayment safely
            if (!decimal.TryParse(fileLoanRepaymentStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fileLoanRepayment))
            {
                string errorMessage = $"Validation failed: Unable to parse 'Loan Repayment' value '{fileLoanRepaymentStr}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new FormatException(errorMessage);
            }

            // Get the expected loan repayment from the database
            decimal expectedLoanRepayment = salaryAnalysisResultDetail.TotalLoanRepayment; // Changed from RemainingSalary to TotalLoanRepayment

            // Compare values with a tolerance for floating-point precision issues
            if (Math.Abs(expectedLoanRepayment - fileLoanRepayment) > 0.01m) // Adjust tolerance if necessary
            {
                string errorMessage = $"Validation failed: Expected 'Loan Repayment' to be '{expectedLoanRepayment:N2}', but found '{fileLoanRepayment:N2}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidateSavingsAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileSavingsStr)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileSavingsStr))
            {
                string errorMessage = $"Validation failed: 'Savings' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Parse file savings safely
            if (!decimal.TryParse(fileSavingsStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fileSavings))
            {
                string errorMessage = $"Validation failed: Unable to parse 'Savings' value '{fileSavingsStr}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new FormatException(errorMessage);
            }

            // Get the expected savings from the database
            decimal expectedSavings = salaryAnalysisResultDetail.Savings; // Fixed incorrect reference (from RemainingSalary to Savings)

            // Compare values with a tolerance to handle floating-point precision issues
            if (Math.Abs(expectedSavings - fileSavings) > 0.01m) // Adjust tolerance if necessary
            {
                string errorMessage = $"Validation failed: Expected 'Savings' to be '{expectedSavings:N2}', but found '{fileSavings:N2}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidateStatusAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileStatus)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileStatus))
            {
                string errorMessage = $"Validation failed: 'Status' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize values for comparison (trim whitespace and ignore case)
            string expectedStatus = salaryAnalysisResultDetail.Status?.Trim();
            string providedStatus = fileStatus.Trim();

            if (!string.Equals(expectedStatus, providedStatus, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Status' to be '{expectedStatus}', but found '{providedStatus}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }

        private async Task ValidateDepositAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileDepositStr)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileDepositStr))
            {
                string errorMessage = $"Validation failed: 'Deposit' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Parse file deposit safely
            if (!decimal.TryParse(fileDepositStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fileDeposit))
            {
                string errorMessage = $"Validation failed: Unable to parse 'Deposit' value '{fileDepositStr}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new FormatException(errorMessage);
            }

            // Get the expected deposit from the database
            decimal expectedDeposit = salaryAnalysisResultDetail.Deposit;

            // Compare values with a tolerance to handle floating-point precision issues
            if (Math.Abs(expectedDeposit - fileDeposit) > 0.01m) // Adjust tolerance if necessary
            {
                string errorMessage = $"Validation failed: Expected 'Deposit' to be '{expectedDeposit:N2}', but found '{fileDeposit:N2}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidatePreferencesSharesAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string filePreferencesSharesStr)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(filePreferencesSharesStr))
            {
                string errorMessage = $"Validation failed: 'Preference Shares' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Parse file preference shares safely
            if (!decimal.TryParse(filePreferencesSharesStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal filePreferencesShares))
            {
                string errorMessage = $"Validation failed: Unable to parse 'Preference Shares' value '{filePreferencesSharesStr}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new FormatException(errorMessage);
            }

            // Get the expected preference shares from the database
            decimal expectedPreferencesShares = salaryAnalysisResultDetail.PreferenceShares;

            // Compare values with a tolerance to handle floating-point precision issues
            if (Math.Abs(expectedPreferencesShares - filePreferencesShares) > 0.01m) // Adjust tolerance if necessary
            {
                string errorMessage = $"Validation failed: Expected 'Preference Shares' to be '{expectedPreferencesShares:N2}', but found '{filePreferencesShares:N2}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }



        private async Task ValidateLoanIdAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string loanId)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(loanId))
            {
                string errorMessage = $"Validation failed: 'Loan ID' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize values for comparison (trim whitespace and ignore case)
            string expectedLoanId = salaryAnalysisResultDetail.LoanId?.Trim();
            string providedLoanId = loanId.Trim();

            if (!string.Equals(expectedLoanId, providedLoanId, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Loan ID' to be '{expectedLoanId}', but found '{providedLoanId}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }


        private async Task ValidateProductIdAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string productId)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(productId))
            {
                string errorMessage = $"Validation failed: 'Loan Product ID' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize values for comparison (trim whitespace and ignore case)
            string expectedProductId = salaryAnalysisResultDetail.LoanProductId?.Trim();
            string providedProductId = productId.Trim();

            if (!string.Equals(expectedProductId, providedProductId, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Loan Product ID' to be '{expectedProductId}', but found '{providedProductId}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }

        private async Task ValidateProductNameAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string fileProductName)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(fileProductName))
            {
                string errorMessage = $"Validation failed: 'Loan Product Name' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize values for comparison (trim whitespace and ignore case)
            string expectedProductName = salaryAnalysisResultDetail.LoanProductName?.Trim();
            string providedProductName = fileProductName.Trim();

            if (!string.Equals(expectedProductName, providedProductName, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Loan Product Name' to be '{expectedProductName}', but found '{providedProductName}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }



        private async Task ValidateCustomerReferenceNumberAsync(SalaryAnalysisResultDetail salaryAnalysisResultDetail, string customerReferenceNumber)
        {
            if (salaryAnalysisResultDetail == null)
            {
                string errorMessage = "Validation failed: Salary Analysis Result Detail is null.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentNullException(nameof(salaryAnalysisResultDetail), errorMessage);
            }

            if (string.IsNullOrWhiteSpace(customerReferenceNumber))
            {
                string errorMessage = $"Validation failed: 'Customer Reference Number' value is missing for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new ArgumentException(errorMessage);
            }

            // Normalize values for comparison (trim whitespace and ignore case)
            string expectedCustomerId = salaryAnalysisResultDetail.CustomerId?.Trim();
            string providedCustomerReferenceNumber = customerReferenceNumber.Trim();

            if (!string.Equals(expectedCustomerId, providedCustomerReferenceNumber, StringComparison.OrdinalIgnoreCase))
            {
                string errorMessage = $"Validation failed: Expected 'Customer Reference Number' to be '{expectedCustomerId}', but found '{providedCustomerReferenceNumber}' for Matricule {salaryAnalysisResultDetail.Matricule}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }



        private List<string> getExpectedHeaders()
        {
            return new List<string>{
                "MATRICULE", "MEMBER REFERENCE", "MEMBER NAME", "NET SALARY", "STANDING ORDER",
                "SAVINGS", "DEPOSIT", "ORDINARY SHARES", "PREFERENCE SHARES", "LOAN REPAYMENT",
                "LOAN CAPITAL", "LOAN INTEREST", "VAT", "CHARGES", "SALARY BALANCE",
                "LOAN ID", "LOAN TYPE", "STANDING ORDER STATEMENT", "STATUS",
                "IS MIGRATED LOAN?", "LOAN PRODUCT NAME", "LOAN PRODUCT ID"
                };
        }

        /// <summary>
        /// Normalizes a name by removing accents, trimming spaces, and standardizing whitespace.
        /// </summary>
        private static string NormalizeName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Trim spaces
            input = input.Trim();

            // Remove multiple spaces and replace with a single space
            input = Regex.Replace(input, @"\s+", " ");

            // Remove accents (diacritics)
            input = RemoveDiacritics(input);

            // Remove special characters except spaces and letters
            input = Regex.Replace(input, @"[^a-zA-Z0-9\s]", "");

            return input;
        }

        /// <summary>
        /// Removes diacritics (accents) from a string.
        /// </summary>
        private static string RemoveDiacritics(string text)
        {
            return string.Concat(text.Normalize(NormalizationForm.FormD)
                                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
                         .Normalize(NormalizationForm.FormC);
        }

        private async Task ValidateExcelHeaderAsync(IXLRow xLRow)
        {
            string errorMessage = "";
            List<string> expectedHeaders = getExpectedHeaders();


            // Read actual headers from the row
            List<string> actualHeaders = xLRow.Cells().Select(c => c.GetValue<string>().Trim()).ToList();

            // Validate header count
            if (expectedHeaders.Count != actualHeaders.Count)
            {
                 errorMessage = $"Header count mismatch!";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            // Compare each header
            for (int i = 0; i < expectedHeaders.Count; i++)
            {
                if (i >= actualHeaders.Count || !string.Equals(expectedHeaders[i], actualHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = $"Mismatch at column {i + 1}: Expected '{expectedHeaders[i]}', Found '{(i < actualHeaders.Count ? actualHeaders[i] : "N/A")}'";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileUploading, LogLevelInfo.Warning);
                    throw new InvalidOperationException(errorMessage);
                }
            }

            
        }

        // Helper method to check if a row is empty
        private bool IsRowEmpty(IXLRow row)
        {
            // Check if all the relevant cells in the row are empty
            return row.Cells().All(cell => string.IsNullOrWhiteSpace(cell.GetString()));
        }
        // Salary extract data to the database
        private void SaveSalaryExtractData(List<SalaryExtractDto> salaryExtracts)
        {
            AddRange(salaryExtracts.Select(s => new SalaryExtract
            {
                Id = s.Id,
                LoanProductId = s.LoanProductId,
                IsOnldLoan = s.IsOnldLoan,
                LoanProductName = s.LoanProductName,
                MemberReference = s.MemberReference,
                MemberName = s.MemberName,  // Fixed: Correct property name
                NetSalary = s.NetSalary,
                Saving = s.Saving,
                Deposit = s.Deposit,
                StandingOrderStatement = s.StandingOrderStatement,
                StandingOrderAmount = s.StandingOrderAmount,
                Shares = s.Shares,
                Charges = s.Charges,
                LoanCapital = s.LoanCapital,  // Fixed: Corrected from LoanPrincipal
                LoanInterest = s.LoanInterest,
                TotalLoanRepayment = s.TotalLoanRepayment,  // Fixed: Corrected from LoanAmount
                VAT = s.VAT,
                Salary = s.Salary,
                FileUploadIdReferenceId = s.FileUploadIdReferenceId,
                LoanType = s.LoanType,
                Status = s.Status,
                PreferenceShares = s.PreferenceShares,
                RemainingSalary = s.RemainingSalary,
                LoanId = s.LoanId,
                BranchId = s.BranchId,
                BranchCode = _userInfoToken.BranchCode,  // Ensuring branch code consistency
                BranchName = s.BranchName,
                FileUploadId = s.FileUploadId,
                Matricule = s.Matricule,
                UploadedBy = s.UploadedBy,
                ExecutedBy = s.ExecutedBy,
                SalaryAnalysisResultId = s.SalaryAnalysisResultId,
                ExtrationDate = s.ExtrationDate,  // Fixed: Ensuring correct date field usage
                ExecutionDate = s.ExecutionDate,
                AccountingDate = s.AccountingDate
            }));

            _logger.LogInformation("Salary records successfully saved to the database.");
        }




        private async Task SaveFile(IFormFile file, string filePath)
        {
            try
            {
                // Check if file already exists
                if (File.Exists(filePath))
                {
                    // If file exists, remove it
                    File.Delete(filePath);
                    _logger.LogInformation("Existing file {FilePath} deleted.", filePath);
                }

                // Save the new file
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                _logger.LogInformation("File saved to {FilePath}.", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving file: {FileName}", file.FileName);
                throw;
            }
        }
        public async Task RemoveSalaryExtractDataForFile(string fileUploadId)
        {
            _logger.LogInformation("Removing salary extract data for FileUploadId: {FileUploadId}", fileUploadId);
            var fileUploaded = await _fileUploadRepository.FindAsync(fileUploadId);
            if (fileUploaded != null)
            {
                _fileUploadRepository.Remove(fileUploaded);
                var salaryExtracts = await FindBy(se => se.FileUploadId == fileUploadId).ToListAsync();
                if (salaryExtracts.Any())
                {
                    RemoveRange(salaryExtracts);
                    await _uow.SaveAsync();
                    _logger.LogInformation("Salary extract data deleted successfully.");
                }
                else
                {
                    _logger.LogWarning("No salary extract records found for FileUploadId: {FileUploadId}", fileUploadId);
                }
            }
            else
            {
                string errorMessage = $"File not found. {fileUploadId}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
}
