using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileUploadP;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP
{

    public class SalaryUploadModelRepository : GenericRepository<SalaryUploadModel, TransactionContext>, ISalaryUploadModelRepository
    {
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<SalaryUploadModelRepository> _logger;
        public SalaryUploadModelRepository(
            IUnitOfWork<TransactionContext> unitOfWork,
            IFileUploadRepository fileUploadRepository,
            ILogger<SalaryUploadModelRepository> logger,
            PathHelper pathHelper,
            UserInfoToken userInfoToken) : base(unitOfWork)
        {
            _uow = unitOfWork;
            _fileUploadRepository = fileUploadRepository;
            _logger = logger;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        public async Task<SalaryUploadModelSummaryDto> ExtractExcelDataToDatabase(IFormFile file, string SalaryType, List<CustomerDto> customers, List<BranchDto> branches)
        {

            if (customers==null)
            {
                customers=new List<CustomerDto>();
            }

            string filePath = string.Empty;
            try
            {
                _logger.LogInformation("Starting Excel data extraction for {FileName}.", file.FileName);


                var fileHash = BaseUtilities.ComputeFileHash(file); // Hash to check for duplicate file
                await CheckDuplicateFile(fileHash); // Ensure file has not already been processed

                filePath = await SaveUploadedFile(file); // Save file locally
                string fileUploadId = BaseUtilities.GenerateInsuranceUniqueNumber(10, $"SM{_userInfoToken.BranchCode}");
                var SalaryUploadModels = await ExtractSalaryModel(file, fileUploadId, SalaryType, customers); // Process file data
                await _fileUploadRepository.SaveFileUploadDetails(file, filePath, fileHash, _userInfoToken.BranchID, _userInfoToken.BranchName, fileUploadId, FileCategory.SalaryModelExtraction, SalaryProcessingStatus.Extraction, SalaryUploadModels.FirstOrDefault().SalaryCode); // Log file upload to DB
                var salaryUploadModelSummary = new SalaryUploadModelSummaryDto { TotalMembers = SalaryUploadModels.Count(), TotalNetSalary = SalaryUploadModels.Sum(x => x.NetSalary) };
                SaveSalaryUploadModelData(SalaryUploadModels, branches); // Save extracted data to DB

                await _uow.SaveAsync();

                _logger.LogInformation("Data extraction and file upload completed successfully for {FileName}.", file.FileName);

                return salaryUploadModelSummary;
            }
            catch (Exception ex)
            {
                // Cleanup temporary file
                if (filePath != null && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                string errorMessage = $"Error during Excel data extraction for file: {file.FileName}. Error: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryFileUploading, LogLevelInfo.Error, null);
                throw new InvalidOperationException(errorMessage);
            }
        }

        //public async Task<SalaryUploadModelSummaryDto> ExtractExcelDataToDatabaseCivilServantsNoBankAccount(IFormFile file, string SalaryType, List<CustomerDto> customers)
        //{
        //    string filePath = string.Empty;
        //    try
        //    {
        //        _logger.LogInformation("Starting Text data extraction for {FileName}.", file.FileName);

        //        FileProcessor.ValidateFileAsyncSalaryModelTextFile(file, _logger);  // Validate file type

        //        var fileHash = BaseUtilities.ComputeFileHash(file); // Hash to check for duplicate file
        //        await CheckDuplicateFile(fileHash); // Ensure file has not already been processed

        //        filePath = await SaveUploadedFile(file); // Save file locally
        //        string fileUploadId = BaseUtilities.GenerateInsuranceUniqueNumber(10, $"SM{_userInfoToken.BranchCode}");


        //        var SalaryUploadModels = await ExtractSalaryModelTextFile(file, fileUploadId, SalaryType, customers); // Process file data
        //        await _fileUploadRepository.SaveFileUploadDetails(file, filePath, fileHash, _userInfoToken.BranchID, _userInfoToken.BranchName, fileUploadId, FileCategory.SalaryModelExtraction, SalaryProcessingStatus.Extraction); // Log file upload to DB
        //        var salaryUploadModelSummary = new SalaryUploadModelSummaryDto { TotalMembers = SalaryUploadModels.Count(), TotalNetSalary = SalaryUploadModels.Sum(x => x.NetSalary) };
        //        SaveSalaryUploadModelData(SalaryUploadModels); // Save extracted data to DB

        //        await _uow.SaveAsync();

        //        _logger.LogInformation("Data extraction and file upload completed successfully for {FileName}.", file.FileName);

        //        return salaryUploadModelSummary;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Cleanup temporary file
        //        if (filePath != null && File.Exists(filePath))
        //        {
        //            File.Delete(filePath);
        //        }
        //        string errorMessage = $"Error during Excel data extraction for file: {file.FileName}. Error: {ex.Message}";
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryFileUploading, LogLevelInfo.Error, null);
        //        throw new InvalidOperationException(errorMessage);
        //    }
        //}


        private void ValidateFilexxxxxx(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("File is empty or not provided.");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var lines = reader.ReadToEnd().Split(Environment.NewLine);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Length < 22)
                    {
                        throw new InvalidOperationException("File contains invalid line structures.");
                    }

                    if (line.Substring(0, 5).Trim().Length != 5)
                    {
                        throw new InvalidOperationException("Invalid SalaryCode length in file.");
                    }

                    if (line.Substring(5, 8).Trim().Length != 8)
                    {
                        throw new InvalidOperationException("Invalid Matricule length in file.");
                    }

                    if (line.Substring(line.Length - 9).Trim().Length != 8)
                    {
                        throw new InvalidOperationException("Invalid NetSalary length in file.");
                    }
                }
            }

            _logger.LogInformation("File structure validated successfully for {FileName}.", file.FileName);
        }
        private async Task CheckDuplicateFile(string fileHash)
        {
            if (await _fileUploadRepository.FindBy(f => f.FileHash == fileHash).AnyAsync())
            {
                _logger.LogError("File already processed.");
                throw new InvalidOperationException("This file has already been processed.");
            }
        }
        private async Task<string> SaveUploadedFile(IFormFile file)
        {
            var directoryPath = $"{_pathHelper.FileUploadPath}\\{_userInfoToken.BranchCode}_{_userInfoToken.BranchName}";

            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            var filePath = Path.Combine(directoryPath, $"{file.FileName}");

            await SaveFile(file, filePath);
            return filePath;
        }
        /// <summary>
        /// Extracts salary data from an uploaded file and returns a list of salary models.
        /// Supports CSV, Excel (XLSX), and TXT file formats.
        /// </summary>
        /// <param name="file">The uploaded salary file.</param>
        /// <param name="fileUploadId">Unique identifier for the uploaded file.</param>
        /// <param name="SalaryType">Type of salary being processed.</param>
        /// <returns>A list of extracted salary upload models.</returns>
        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModel(IFormFile file, string fileUploadId, string SalaryType, List<CustomerDto> customers)
        {
            // Initialize the salary data list
            var salaryUploadModels = new List<SalaryUploadModelDto>();
            string tempExcelFilePath = null;

            try
            {
                // Step 1: Validate file format and call the appropriate extraction method
                if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
                    file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"[INFO] Extracting salary data from Excel/CSV file: {file.FileName}.");

                    salaryUploadModels = await ExtractSalaryModelExcelOrCSVOther(file, fileUploadId, SalaryType, customers);

                    string successMessage = $"[SUCCESS] Extracted {salaryUploadModels.Count} salary records from {file.FileName}.";
                    _logger.LogInformation(successMessage);
                    return salaryUploadModels;
                }
                else if (file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"[INFO] Extracting salary data from TXT file: {file.FileName}.");

                    salaryUploadModels = await ExtractSalaryModelTextFile(file, fileUploadId, SalaryType, customers);

                    string successMessage = $"[SUCCESS] Extracted {salaryUploadModels.Count} salary records from {file.FileName}.";
                    _logger.LogInformation(successMessage);
                    return salaryUploadModels;
                }
                else
                {
                    // Step 2: Handle unsupported file formats
                    string unsupportedFileMessage = $"[ERROR] Unsupported file format: {file.FileName}. Only CSV, XLSX, and TXT are allowed.";
                    _logger.LogError(unsupportedFileMessage);
                    throw new InvalidOperationException(unsupportedFileMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 3: Log detailed error messages for debugging
                string errorMessage = $"[ERROR] Salary data extraction failed for file: {file.FileName}. Reason: {ex.Message}";
                _logger.LogError(ex, errorMessage);

                throw new InvalidOperationException(errorMessage, ex);
            }
            finally
            {
                // Step 4: Cleanup temporary files if used
                if (!string.IsNullOrEmpty(tempExcelFilePath) && File.Exists(tempExcelFilePath))
                {
                    _logger.LogInformation($"[INFO] Deleting temporary file: {tempExcelFilePath}");
                    File.Delete(tempExcelFilePath);
                }
            }
        }

        /// <summary>
        /// Extracts salary data from an Excel or CSV file and returns a list of salary models.
        /// Supports `.csv` (converted to `.xlsx`) and `.xlsx` formats.
        /// </summary>
        /// <param name="file">The uploaded Excel or CSV file.</param>
        /// <param name="fileUploadId">Unique identifier for the uploaded file.</param>
        /// <param name="SalaryType">Type of salary being processed.</param>
        /// <returns>A list of extracted salary upload models.</returns>
        /// <exception cref="InvalidOperationException">Thrown if file format is invalid or data validation fails.</exception>
        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModelExcelOrCSVOther(IFormFile file, string fileUploadId, string SalaryType, List<CustomerDto> customers)
        {
            var salaryUploadModels = new List<SalaryUploadModelDto>();
            string tempExcelFilePath = null; // Path for temporary Excel file

            try
            {
                // Step 1: Convert CSV to Excel or Save the Excel file
                if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"[INFO] Converting CSV file to Excel format: {file.FileName}");
                    await BaseUtilities.LogAndAuditAsync($"Starting CSV to Excel conversion for file: {file.FileName}", file, HttpStatusCodeEnum.OK, LogAction.SalaryFileProcessing, LogLevelInfo.Information);
                    tempExcelFilePath = FileProcessor.ConvertCsvToExcel(file, _logger);
                }
                else if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"[INFO] Saving uploaded Excel file: {file.FileName}");
                    await BaseUtilities.LogAndAuditAsync($"Saving uploaded Excel file: {file.FileName}", file, HttpStatusCodeEnum.OK, LogAction.SalaryFileProcessing, LogLevelInfo.Information);
                    tempExcelFilePath = FileProcessor.SaveExcelFile(file, _logger);
                }
                else
                {
                    string fileError = $"[ERROR] Unsupported file format: {file.FileName}. Only .csv and .xlsx are allowed.";
                    _logger.LogError(fileError);
                    await BaseUtilities.LogAndAuditAsync(fileError, file, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryFileProcessing, LogLevelInfo.Error);
                    throw new InvalidOperationException(fileError);
                }

                // Step 2: Process the Excel file using ClosedXML
                using (var workbook = new XLWorkbook(tempExcelFilePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    int rowIndex = 1; // Track row numbers for better debugging

                    _logger.LogInformation($"[INFO] Processing worksheet: {worksheet.Name}, File: {file.FileName}");
                    await BaseUtilities.LogAndAuditAsync($"Processing worksheet: {worksheet.Name}", file, HttpStatusCodeEnum.OK, LogAction.SalaryFileProcessing, LogLevelInfo.Information);

                   int headerIndex= await FileProcessor.ValidateHeadersAsync(worksheet, _logger, file.FileName);
                    

                    foreach (var row in worksheet.Rows().Skip(1)) // Skip header row
                    {
                        rowIndex++;

                        // Step 3: Validate column count
                        if (row.CellsUsed().Count() != 5)
                        {
                            string columnError = $"[ERROR] Invalid column count at row {rowIndex} in {file.FileName}. Expected 6 columns.";
                            _logger.LogError(columnError);
                            await BaseUtilities.LogAndAuditAsync(columnError, file, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileProcessing, LogLevelInfo.Error);
                            throw new InvalidOperationException(columnError);
                        }

                        string salaryCode = "";
                        string matricule = "";
                        string memberReference = "";
                        string memberName = "";
                        string netSalaryCell = "";
                        string accountNumber = "";
                        // Step 4: Extract and validate cell values

                        switch (headerIndex)
                        {
                            case 0:
                                {
                                    salaryCode = row.Cell(1).GetString();
                                    matricule = row.Cell(2).GetString();
                                    memberReference = row.Cell(3).GetString();
                                    memberName = row.Cell(4).GetString();
                                    netSalaryCell = row.Cell(5).GetString();
                                }
                                break;
                            case 1:
                                {
                                    salaryCode = row.Cell(1).GetString();
                                    matricule = row.Cell(2).GetString();
                                    memberReference = "";
                                    memberName = row.Cell(3).GetString();
                                    accountNumber = row.Cell(4).GetString();
                                    netSalaryCell = row.Cell(5).GetString();
                                }
                                break;
                        }
                       

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(salaryCode) ||
                            string.IsNullOrWhiteSpace(memberReference) ||
                            string.IsNullOrWhiteSpace(memberName))
                        {
                            string fieldError = $"[ERROR] Required field missing at row {rowIndex}. Salary Code, Member Reference, and Member Name cannot be empty.";
                            _logger.LogError(fieldError);
                            await BaseUtilities.LogAndAuditAsync(fieldError, file, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileProcessing, LogLevelInfo.Error);
                            throw new InvalidOperationException(fieldError);
                        }

                        /*// Validate phone number (Cameroon format, 9 or 12 digits)
                        if (!Regex.IsMatch(phoneNumber, @"^(?:237)?(6[56789]\d{7}|2[2-9]\d{7})$"))
                        {
                            string phoneError = $"[ERROR] Invalid phone number format at row {rowIndex}. Expected 9 or 12 digits (Cameroon format).";
                            _logger.LogError(phoneError);
                            await BaseUtilities.LogAndAuditAsync(phoneError, file, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileProcessing, LogLevelInfo.Error);
                            throw new InvalidOperationException(phoneError);
                        }*/


                        // Validate Net Salary as decimal
                        if (!decimal.TryParse(netSalaryCell, out var netSalary))
                        {
                            string salaryError = $"[ERROR] Invalid Net Salary format at row {rowIndex}. Expected a valid decimal value.";
                            _logger.LogError(salaryError);
                            await BaseUtilities.LogAndAuditAsync(salaryError, file, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileProcessing, LogLevelInfo.Error);
                            throw new InvalidOperationException(salaryError);
                        }
                        // Find customer by Matricule
                        var customer = customers.FirstOrDefault(c => c.Matricule.Equals(matricule, StringComparison.OrdinalIgnoreCase));

                        // Step 5: Create salary model DTO
                        var salaryUploadModel = new SalaryUploadModelDto
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            SalaryCode = salaryCode,
                            Matricule = matricule,
                            MemberReference = memberReference,
                            Surname = memberName,
                            Name = memberName,
                            LaisonBankAccountNumber =accountNumber,
                            NetSalary = netSalary,
                            BranchId = customer.BranchId,
                            BranchName = _userInfoToken.BranchName,
                            SalaryType = SalaryType,
                            FileUploadId = fileUploadId,
                            BranchCode = _userInfoToken.BranchCode,
                            UploadedBy = _userInfoToken.FullName,
                            Date = BaseUtilities.UtcNowToDoualaTime(),
                            Phonumber = customer?.Phone ?? "n/a" // Mapping phone number if found, otherwise "N/A"
                        };

                        salaryUploadModels.Add(salaryUploadModel);
                    }
                }

                int totalRecords = salaryUploadModels.Count;
                int successfulRecords = salaryUploadModels.Count(); // Assuming IsProcessed indicates success
                int failedRecords = totalRecords - successfulRecords;

                decimal totalSalaryAmount = salaryUploadModels.Sum(s => s.NetSalary); // Assuming SalaryAmount is the salary field
                decimal averageSalary = salaryUploadModels.Average(s => s.NetSalary);
                decimal maxSalary = salaryUploadModels.Max(s => s.NetSalary);
                decimal minSalary = salaryUploadModels.Min(s => s.NetSalary);

                string extractionSuccessMessage =
                    $"[SUCCESS] Successfully extracted {totalRecords:N} salary records from {file.FileName}.\n" +
                    $"Processed: {successfulRecords}, Failed: {failedRecords:N}\n" +
                    $"Total Salary Amount: {totalSalaryAmount:N2}, Average Salary: {averageSalary:N2}\n" +
                    $"Max Salary: {maxSalary:N2}, Min Salary: {minSalary:N2}";
                _logger.LogInformation(extractionSuccessMessage);
                await BaseUtilities.LogAndAuditAsync(extractionSuccessMessage, file, HttpStatusCodeEnum.OK, LogAction.SalaryFileProcessing, LogLevelInfo.Information);
                return salaryUploadModels;
            }
            catch (Exception ex)
            {
                // Step 6: Handle and log any exceptions
                string extractionErrorMessage = $"[ERROR] Salary data extraction failed for file {file.FileName}. Reason: {ex.Message}";
                _logger.LogError(ex, extractionErrorMessage);
                await BaseUtilities.LogAndAuditAsync(extractionErrorMessage, file, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryFileProcessing, LogLevelInfo.Error);
                throw new InvalidOperationException(extractionErrorMessage, ex);
            }
            finally
            {
                // Step 7: Cleanup temporary files if used
                if (!string.IsNullOrEmpty(tempExcelFilePath) && File.Exists(tempExcelFilePath))
                {
                    _logger.LogInformation($"[INFO] Deleting temporary file: {tempExcelFilePath}");
                    await BaseUtilities.LogAndAuditAsync($"Deleting temporary file: {tempExcelFilePath}", file, HttpStatusCodeEnum.OK, LogAction.SalaryFileProcessing, LogLevelInfo.Information);
                    File.Delete(tempExcelFilePath);
                }
            }
        }




        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModelExcelOrCSV(IFormFile file, string fileUploadId, string SalaryType)
        {
            var salaryUploadModels = new List<SalaryUploadModelDto>();
            string tempExcelFilePath = null;

            try
            {
                // Convert CSV to Excel if necessary
                if (file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    tempExcelFilePath = FileProcessor.ConvertCsvToExcel(file, _logger);
                }
                else if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    tempExcelFilePath = FileProcessor.SaveExcelFile(file, _logger);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported file format. Only .csv and .xlsx are allowed.");
                }

                // Process the Excel file
                using (var workbook = new XLWorkbook(tempExcelFilePath))
                {
                    var worksheet = workbook.Worksheet(1);

                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
                    {
                        var salaryUploadModel = new SalaryUploadModelDto
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            SalaryCode = row.Cell(1).GetString(),
                            Matricule = row.Cell(2).GetString(),
                            Surname = row.Cell(3).GetString(),
                            Name = row.Cell(4).GetString(),
                            LaisonBankAccountNumber = row.Cell(5).GetString(),
                            NetSalary = BaseUtilities.GetDecimalFromCell(row.Cell(6)),
                            BranchId = _userInfoToken.BranchID,
                            BranchName = _userInfoToken.BranchName,
                            SalaryType = SalaryType,
                            FileUploadId = fileUploadId,
                            BranchCode = _userInfoToken.BranchCode,
                            UploadedBy = _userInfoToken.FullName,
                            Date = BaseUtilities.UtcNowToDoualaTime(),
                        };

                        salaryUploadModels.Add(salaryUploadModel);
                    }
                }

                _logger.LogInformation("Extracted {Count} salary records.", salaryUploadModels.Count);
                return salaryUploadModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during salary model extraction: {FileName}", file.FileName);
                throw new InvalidOperationException($"Failed to extract salary data from file: {file.FileName}.", ex);
            }
            finally
            {
                // Cleanup temporary file
                if (tempExcelFilePath != null && File.Exists(tempExcelFilePath))
                {
                    File.Delete(tempExcelFilePath);
                }
            }
        }


        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModelTextFile(IFormFile file, string fileUploadId, string SalaryType, List<CustomerDto> customers)
        {
            int columnCount = ValidateFile(file);

            if (columnCount == 4)
            {
                return await ExtractSalaryModelTextFile4Columnxx(file, fileUploadId, SalaryType, customers);
            }
            else if (columnCount == 5)
            {
                return await ExtractSalaryModelTextFile5Columnsxx(file, fileUploadId, SalaryType, customers);
            }
            else
            {
                throw new InvalidOperationException("File structure does not match either 4-column or 5-column format.");
            }
        }

        private int ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("File is empty or not provided.");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var lines = reader.ReadToEnd().Split(Environment.NewLine);
                bool isFiveColumnFile = false;
                bool isFourColumnFile = false;

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    /* if (line.Length < 22)
                     {
                         throw new InvalidOperationException("File contains invalid line structures.");
                     }*/
                    /*
                                        if (line.Substring(0, 5).Trim().Length == 5 && line.Substring(5, 8).Trim().Length == 8)
                                        {*/
                    int accountNumberStartIndex = line.IndexOf("11000005");
                    if (accountNumberStartIndex != -1 /*&& accountNumberStartIndex + 8 <= line.Length*/)
                    {
                        /* int salaryStartIndex = accountNumberStartIndex + 8;
                         if (salaryStartIndex + 8 <= line.Length)
                         {*/
                        isFiveColumnFile = true;
                        break;
                        //  }
                    }
                    else
                    {
                        isFourColumnFile = true;
                        break;
                    }
                    //}
                }

                if (isFiveColumnFile)
                {
                    _logger.LogInformation("File structure validated as a 5-column file for {FileName}.", file.FileName);
                    return 5;
                }
                else if (isFourColumnFile)
                {
                    _logger.LogInformation("File structure validated as a 4-column file for {FileName}.", file.FileName);
                    return 4;
                }
                else
                {
                    throw new InvalidOperationException("File structure does not match either 4-column or 5-column format.");
                }
            }
        }

        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModelTextFile4Columnxx(IFormFile file, string fileUploadId, string SalaryType, List<CustomerDto> customers)
        {
            var salaryUploadModels = new List<SalaryUploadModelDto>();
            string tempExcelFilePath = null;
            string tempFilePath = null;

            try
            {
                _logger.LogInformation("Starting text file data extraction for {FileName}.", file.FileName);

                // Save the uploaded file temporarily
                tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                int lineNumber = 0;
                foreach (var line in File.ReadLines(tempFilePath))
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        string fullName = "";
                        string matricule = "";
                        string postName = "";
                        string salary = "";
                        string salaryCode = "";
                        var datas = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        int n = datas.Length;

                        if (datas[0].Length > 13)
                        {
                            salary = datas[n - 1];
                            salaryCode = datas[0].Substring(0, 5);
                            matricule = datas[0].Substring(5, 8);
                            string preFullName = datas[0].Substring(13);
                            fullName = $"{preFullName}";
                            for (int i = 1; i < n - 1; i++)
                            {
                                fullName += $" {datas[i]}";
                            }
                            fullName += $" {postName}";
                        }
                        else if (datas[0].Length == 6)
                        {
                            string preMatricule = datas[0].Substring(datas[0].Length - 1);
                            salaryCode = datas[0].Substring(0, 5);
                            salary = datas[n - 1];
                            if (datas[1].Length == 7)
                            {
                                matricule += preMatricule + datas[1];
                            }
                            for (int i = 2; i < n - 1; i++)
                            {
                                fullName += $" {datas[i]}";
                            }
                            fullName += $" {postName}";
                        }
                        else if (datas[0].Length == 5)
                        {

                            salaryCode = datas[0];
                            salary = datas[n - 1];
                            matricule += datas[1];
                            for (int i = 2; i < n - 1; i++)
                            {
                                fullName += $" {datas[i]}";
                            }
                            fullName += $" {postName}";
                        }

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(salaryCode))
                        {
                            throw new InvalidOperationException($"Missing Salary Code at line {lineNumber}.");
                        }

                        if (string.IsNullOrWhiteSpace(matricule))
                        {
                            throw new InvalidOperationException($"Missing Matricule at line {lineNumber}.");
                        }

                        if (string.IsNullOrWhiteSpace(fullName))
                        {
                            throw new InvalidOperationException($"Missing Full Name at line {lineNumber}.");
                        }

                        if (!decimal.TryParse(salary, out decimal netSalary))
                        {
                            throw new InvalidOperationException($"Invalid Net Salary format at line {lineNumber}.");
                        }
                        // Find customer by Matricule
                        var customer = customers.FirstOrDefault(c =>
    c.Matricule != null && c.Matricule.Equals(matricule, StringComparison.OrdinalIgnoreCase));

                        var salaryUploadModel = new SalaryUploadModelDto
                        {
                            SalaryCode = salaryCode,
                            Matricule = matricule,
                            Surname = fullName,
                            Name = "", MemberReference=customer?.CustomerId ?? "n/a",
                            LaisonBankAccountNumber = "n/a",
                            NetSalary = netSalary,
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            BranchId = customer?.BranchId ?? _userInfoToken.BranchID,  // Assign "N/A" if customer is null
                            BranchName = customer?.BranchName ?? _userInfoToken.BranchName, // Fallback to _userInfoToken if null
                            SalaryType = SalaryType,
                            FileUploadId = fileUploadId,
                            BranchCode = customer?.BranchCode ?? _userInfoToken.BranchCode, // Assign _userInfoToken if customer is null
                            UploadedBy = _userInfoToken.FullName,
                            Date = BaseUtilities.UtcNowToDoualaTime(),
                            Phonumber = customer?.Phone ?? "n/a" // Assign "N/A" if phone number is null
                        };



                        salaryUploadModels.Add(salaryUploadModel);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error processing line {LineNumber}: {Line}. Error: {Message}", lineNumber, line, ex.Message);
                    }
                }

                _logger.LogInformation("Text file data extraction completed successfully with {Count} records.", salaryUploadModels.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during text file data extraction for {FileName}.", file.FileName);
                throw;
            }
            finally
            {
                // Cleanup temporary file
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }

            return salaryUploadModels;
        }

        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModelTextFile5Columnsxx(IFormFile file, string fileUploadId, string SalaryType, List<CustomerDto> customers)
        {
            var salaryUploadModels = new List<SalaryUploadModelDto>();
            string tempFilePath = null;

            try
            {
                _logger.LogInformation("Starting text file data extraction for {FileName}.", file.FileName);

                // Save the uploaded file temporarily
                tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                int lineNumber = 0;
                foreach (var line in File.ReadLines(tempFilePath))
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        string fullName = "";
                        string matricule = "";
                        string postFullName = "";
                        string salary = "";
                        string accountNumber = "";
                        string salaryCode = "";
                        var datas = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        int n = datas.Length;

                        if (datas[0].Length > 13)
                        {
                            salary = datas[n - 1];
                            if (datas[n - 2].Length > 8)
                            {
                                accountNumber = datas[n - 2].Substring(datas[n - 2].Length - 7);
                                postFullName = datas[n - 2].Substring(0, datas[n - 2].Length - 7);
                            }
                            else
                            {
                                accountNumber = datas[n - 2];
                            }

                            salaryCode = datas[0].Substring(0, 5);
                            matricule = datas[0].Substring(5, 8);
                            string preFullName = datas[0].Substring(13);
                            fullName = preFullName;
                            for (int i = 1; i < n - 2; i++)
                            {
                                fullName += $" {datas[i]}";
                            }
                            fullName += $" {postFullName}";
                        }
                        else if (datas[0].Length == 6)
                        {
                            string preMatricule = datas[0].Substring(datas[0].Length - 1);
                            salaryCode = datas[0].Substring(0, 5);
                            salary = datas[n - 1];
                            if (datas[1].Length == 7)
                            {
                                matricule += preMatricule + datas[1];
                            }

                            if (datas[n - 2].Length > 8)
                            {
                                accountNumber = datas[n - 2].Substring(datas[n - 2].Length - 7);
                                postFullName = datas[n - 2].Substring(0, datas[n - 2].Length - 7);
                            }
                            else
                            {
                                accountNumber = datas[n - 2];
                            }

                            for (int i = 2; i < n - 2; i++)
                            {
                                fullName += $" {datas[i]}";
                            }
                            fullName += $" {postFullName}";
                        }
                        else if (datas[0].Length == 5)
                        {

                            salaryCode = datas[0].Substring(0, 5);
                            salary = datas[n - 1];

                            matricule += datas[1];


                            if (datas[n - 2].Length > 8)
                            {
                                accountNumber = datas[n - 2].Substring(datas[n - 2].Length - 7);
                                postFullName = datas[n - 2].Substring(0, datas[n - 2].Length - 7);
                            }
                            else
                            {
                                accountNumber = datas[n - 2];
                            }

                            for (int i = 2; i < n - 2; i++)
                            {
                                fullName += $" {datas[i]}";
                            }
                            fullName += $" {postFullName}";
                        }

                        // Validations
                        if (string.IsNullOrWhiteSpace(salaryCode))
                        {
                            throw new InvalidOperationException($"Missing Salary Code at line {lineNumber}.");
                        }

                        if (string.IsNullOrWhiteSpace(matricule))
                        {
                            throw new InvalidOperationException($"Missing Matricule at line {lineNumber}.");
                        }

                        if (string.IsNullOrWhiteSpace(fullName))
                        {
                            throw new InvalidOperationException($"Missing Full Name at line {lineNumber}.");
                        }

                        if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 8)
                        {
                            throw new InvalidOperationException($"Invalid Account Number at line {lineNumber}.");
                        }

                        if (!decimal.TryParse(salary, out decimal netSalary))
                        {
                            throw new InvalidOperationException($"Invalid Net Salary format at line {lineNumber}.");
                        }
                        // Find customer by Matricule
                        var customer = customers.FirstOrDefault(c => c.Matricule.Equals(matricule, StringComparison.OrdinalIgnoreCase));
                        var salaryUploadModel = new SalaryUploadModelDto
                        {
                            SalaryCode = salaryCode,
                            Matricule = matricule,
                            Surname = fullName,
                            Name = "",
                            LaisonBankAccountNumber = accountNumber,
                            NetSalary = netSalary,
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            BranchId = customer.BranchId,
                            BranchName = _userInfoToken.BranchName,
                            SalaryType = SalaryType,
                            FileUploadId = fileUploadId,
                            BranchCode = _userInfoToken.BranchCode,
                            UploadedBy = _userInfoToken.FullName,
                            Date = BaseUtilities.UtcNowToDoualaTime(),
                            Phonumber = customer?.Phone ?? "n/a" // Mapping phone number if found, otherwise "N/A"
                        };

                        salaryUploadModels.Add(salaryUploadModel);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error processing line {LineNumber}: {Line}. Error: {Message}", lineNumber, line, ex.Message);
                    }
                }

                _logger.LogInformation("Text file data extraction completed successfully with {Count} records.", salaryUploadModels.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during text file data extraction for {FileName}.", file.FileName);
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }

            return salaryUploadModels;
        }

        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModelTextFile4Column(IFormFile file, string fileUploadId, string SalaryType)
        {
            var salaryUploadModels = new List<SalaryUploadModelDto>();
            string tempExcelFilePath = null;

            string tempFilePath = null;

            try
            {
                _logger.LogInformation("Starting text file data extraction for {FileName}.", file.FileName);


                // Save the uploaded file temporarily
                tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                foreach (var line in File.ReadLines(tempFilePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var salaryCode = line.Substring(0, 5).Trim();
                        var matricule = line.Substring(5, 8).Trim();
                        var netSalaryString = line.Substring(line.Length - 9).Trim();
                        decimal netSalary = decimal.TryParse(netSalaryString, out decimal parsedSalary) ? parsedSalary : 0;
                        var fullName = line.Substring(13, line.Length - 22).Trim();

                        var salaryUploadModel = new SalaryUploadModelDto
                        {
                            SalaryCode = salaryCode,
                            Matricule = matricule,
                            Surname = fullName,
                            Name = "",
                            LaisonBankAccountNumber = "N/A",
                            NetSalary = netSalary,
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            BranchId = _userInfoToken.BranchID,
                            BranchName = _userInfoToken.BranchName,
                            SalaryType = SalaryType,
                            FileUploadId = fileUploadId,
                            BranchCode = _userInfoToken.BranchCode,
                            UploadedBy = _userInfoToken.FullName,
                            Date = BaseUtilities.UtcNowToDoualaTime(),
                        };

                        salaryUploadModels.Add(salaryUploadModel);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error processing line: {Line}. Error: {Message}", line, ex.Message);
                    }
                }

                _logger.LogInformation("Text file data extraction completed successfully with {Count} records.", salaryUploadModels.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during text file data extraction for {FileName}.", file.FileName);
                throw;
            }
            finally
            {
                // Cleanup temporary file
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }

            return salaryUploadModels;
        }
        private async Task<List<SalaryUploadModelDto>> ExtractSalaryModelTextFile5Columns(IFormFile file, string fileUploadId, string SalaryType)
        {
            var salaryUploadModels = new List<SalaryUploadModelDto>();
            string tempFilePath = null;

            try
            {
                _logger.LogInformation("Starting text file data extraction for {FileName}.", file.FileName);



                // Save the uploaded file temporarily
                tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                foreach (var line in File.ReadLines(tempFilePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var salaryCode = line.Substring(0, 5).Trim();
                        var matricule = line.Substring(5, 8).Trim();

                        int accountNumberStartIndex = line.IndexOf("11000005");
                        if (accountNumberStartIndex == -1 || accountNumberStartIndex + 8 > line.Length)
                        {
                            _logger.LogWarning("Skipping line due to missing or invalid account number: {Line}", line);
                            continue;
                        }

                        var name = line.Substring(13, accountNumberStartIndex - 13).Trim();
                        var accountNumber = line.Substring(accountNumberStartIndex, 8).Trim();

                        int salaryStartIndex = accountNumberStartIndex + 8;
                        if (salaryStartIndex + 8 > line.Length)
                        {
                            _logger.LogWarning("Skipping line due to missing or invalid salary: {Line}", line);
                            continue;
                        }

                        var netSalaryString = line.Substring(salaryStartIndex, 8).Trim();
                        decimal netSalary = decimal.TryParse(netSalaryString, out decimal parsedSalary) ? parsedSalary : 0;

                        var salaryUploadModel = new SalaryUploadModelDto
                        {
                            SalaryCode = salaryCode,
                            Matricule = matricule,
                            Surname = name,
                            Name = "", // Adjust if needed
                            LaisonBankAccountNumber = accountNumber,
                            NetSalary = netSalary,
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            BranchId = _userInfoToken.BranchID,
                            BranchName = _userInfoToken.BranchName,
                            SalaryType = SalaryType,
                            FileUploadId = fileUploadId,
                            BranchCode = _userInfoToken.BranchCode,
                            UploadedBy = _userInfoToken.FullName,
                            Date = BaseUtilities.UtcNowToDoualaTime(),
                        };

                        salaryUploadModels.Add(salaryUploadModel);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error processing line: {Line}. Error: {Message}", line, ex.Message);
                    }
                }

                _logger.LogInformation("Text file data extraction completed successfully with {Count} records.", salaryUploadModels.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during text file data extraction for {FileName}.", file.FileName);
                throw;
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }

            return salaryUploadModels;
        }

        private void ValidateFilex(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("File is empty or not provided.");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var lines = reader.ReadToEnd().Split(Environment.NewLine);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line.Length < 22)
                    {
                        throw new InvalidOperationException("File contains invalid line structures.");
                    }

                    if (line.Substring(0, 5).Trim().Length != 5)
                    {
                        throw new InvalidOperationException("Invalid SalaryCode length in file.");
                    }

                    if (line.Substring(5, 8).Trim().Length != 8)
                    {
                        throw new InvalidOperationException("Invalid Matricule length in file.");
                    }

                    int accountNumberStartIndex = line.IndexOf("11000005");
                    if (accountNumberStartIndex == -1 || accountNumberStartIndex + 8 > line.Length)
                    {
                        throw new InvalidOperationException("Invalid Account Number length in file.");
                    }

                    int salaryStartIndex = accountNumberStartIndex + 8;
                    if (salaryStartIndex + 8 > line.Length)
                    {
                        throw new InvalidOperationException("Invalid NetSalary length in file.");
                    }
                }
            }

            _logger.LogInformation("File structure validated successfully for {FileName}.", file.FileName);
        }

        private void SaveSalaryUploadModelData(List<SalaryUploadModelDto> SalaryUploadModels, List<BranchDto> branches)
        {
            // Map each SalaryUploadModelDto to its corresponding branch
            var mappedSalaryUploads = SalaryUploadModels.Select(s =>
            {
                var branch = branches.FirstOrDefault(b => b.id == s.BranchId);

                return new SalaryUploadModel
                {
                    Id = s.Id,
                    SalaryCode = s.SalaryCode,
                    NetSalary = s.NetSalary,
                    BranchCode = branch != null ? branch.branchCode : "n/a", // Assign branchCode if found, otherwise "N/A"
                    BranchId = s.BranchId,
                    BranchName = branch != null ? branch.name : "n/a", // Assign branch name if found, otherwise "N/A"
                    MemberReference = s.MemberReference,
                    CustomerId = s.MemberReference,
                    Phonumber = s.Phonumber,
                    SalaryType = s.SalaryType,
                    FileUploadId = s.FileUploadId,
                    Matricule = s.Matricule,
                    LaisonBankAccountNumber = s.LaisonBankAccountNumber,
                    Surname = s.Surname,
                    Name = s.Name,
                    UploadedBy = s.UploadedBy,
                    Date = s.Date
                };
            }).ToList();

            // Save mapped data
            AddRange(mappedSalaryUploads);

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
                string errorMessage = $"Error while saving file: {file.FileName}. Error: {ex.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
        public async Task RemoveSalaryUploadModelDataForFile(string fileUploadId)
        {
            _logger.LogInformation("Removing salary extract data for FileUploadId: {FileUploadId}", fileUploadId);
            var fileUploaded = await _fileUploadRepository.FindAsync(fileUploadId);
            if (fileUploaded != null)
            {
                _fileUploadRepository.Remove(fileUploaded);
                var SalaryUploadModels = await FindBy(se => se.FileUploadId == fileUploadId).ToListAsync();
                if (SalaryUploadModels.Any())
                {
                    RemoveRange(SalaryUploadModels);
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
