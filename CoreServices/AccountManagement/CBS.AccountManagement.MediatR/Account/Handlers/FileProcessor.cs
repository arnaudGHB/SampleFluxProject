using Amazon.Auth.AccessControlPolicy;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Table.PivotTable;
using Polly.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CBS.AccountManagement.MediatR.Handlers
{
    public class TrialBalanceUploadResult
    {
        public List<AccountModelX> AccountNotPresent { get; set; }
        public List<AccountModelX> OriginalFile { get; set; }
    }
    public class FileProcessor
    {
        private readonly IWebHostEnvironment _environment;
        public readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly string _fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MembersAccountExport");

        public FileProcessor(IWebHostEnvironment environment, PathHelper pathHelper, UserInfoToken? userInfoToken)
        {
            _environment = environment;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }
        public FileProcessor(IWebHostEnvironment environment, PathHelper pathHelper)
        {
            _environment = environment;
            _pathHelper = pathHelper;

        }

        public string ProcessAndSaveFile(List<AccountModelX> books, string fileExtension, string BranchName)
        {
            string filePath = "";
            try
            {
                // Generate a unique file name
                string fileName = $"{BranchName}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

                // Get the path to the server's storage directory
                string dirPath = Path.Combine(_environment.ContentRootPath, "App_Data", "ProcessedFiles");

                // Ensure the directory exists
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                // Combine the directory path and file name
                filePath = Path.Combine(dirPath, fileName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Or LicenseContext.Commercial if you have a license

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("AccountNotPresent");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "AccountNumber";
                    worksheet.Cells[1, 2].Value = "AccountName";

                    // Add data
                    for (int i = 0; i < books.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = books[i].AccountNumber;
                        worksheet.Cells[i + 2, 2].Value = books[i].AccountName;

                    }

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();




                    File.WriteAllBytes(filePath, package.GetAsByteArray());


                }
                // Write the content to the file


            }
            catch (Exception ex)
            {
                // Log the error (implement proper error logging in production)
                Console.WriteLine($"Error saving file: {ex.Message}");

            }
            return filePath;
        }

        public string ProcessAndSaveFileForDownload(List<AccountModelX> AccountModels, string fileExtension, string BranchName)
        {
            string filePath = "";
            string webPath = "";
            try
            {
                // Generate a unique file name
                string fileName = $"{BranchName.Trim()}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";

                // Get the path to the wwwroot directory
                string wwwrootPath = $"{_environment.WebRootPath}{_pathHelper.FileUpload_MigrationPath}";

                // Ensure the directory exists
                if (!Directory.Exists(wwwrootPath))
                {
                    Directory.CreateDirectory(wwwrootPath);
                }

                // Combine the directory path and file name
                filePath = Path.Combine(wwwrootPath, fileName).Replace(" ", "_");

                // Generate the web-accessible path
                webPath = $"{_pathHelper.DomainName}{_pathHelper.FileUpload_MigrationPath}/{fileName.Replace(" ", "_")}";

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Or LicenseContext.Commercial if you have a license

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("AccountNotPresent");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "AccountNumber";
                    worksheet.Cells[1, 2].Value = "AccountName";

                    // Add data
                    for (int i = 0; i < AccountModels.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = AccountModels[i].AccountNumber;
                        worksheet.Cells[i + 2, 2].Value = AccountModels[i].AccountName;
                    }

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    // Save the Excel file
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                // Log the error (implement proper error logging in production)
                Console.WriteLine($"Error saving file: {ex.Message}");
                return null; // or throw the exception if you want to handle it at a higher level
            }

            return webPath; // Return the web-accessible path
        }

        public IFormFileCollection PrepareFileOnServer(bool IsAccountHarmonizationActivated, bool IsSuccessFull, TrialBalanceUploadResult AccountModels, string fileExtension, string BranchName)
        {
            try
            {
                var formFileCollection = new FormFileCollection();
                // Generate a unique file name
                string fileName = $"{BranchName.Trim()}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                fileName = fileName.Replace(" ", "_");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Or LicenseContext.Commercial if you have a license

                using (var package = new ExcelPackage())
                {
                    if (IsSuccessFull)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("TrialBalance");

                        // Add headers
                        worksheet.Cells[1, 1].Value = "AccountNumber";
                        worksheet.Cells[1, 2].Value = "AccountName";
                        worksheet.Cells[1, 3].Value = "BeginningDebitBalance";
                        worksheet.Cells[1, 4].Value = "BeginningCreditBalance";
                        worksheet.Cells[1, 5].Value = "DebitBalance";
                        worksheet.Cells[1, 6].Value = "CreditBalance";
                        worksheet.Cells[1, 7].Value = "EndingDebitBalance";
                        worksheet.Cells[1, 8].Value = "EndingCreditBalance";
                        // Add data
                        for (int i = 0; i < AccountModels.OriginalFile.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = AccountModels.OriginalFile[i].AccountNumber;
                            worksheet.Cells[i + 2, 2].Value = AccountModels.OriginalFile[i].AccountName;
                            worksheet.Cells[i + 2, 3].Value = AccountModels.OriginalFile[i].BeginningDebitBalance;
                            worksheet.Cells[i + 2, 4].Value = AccountModels.OriginalFile[i].BeginningCreditBalance;
                            worksheet.Cells[i + 2, 5].Value = AccountModels.OriginalFile[i].MovementDebitBalance;
                            worksheet.Cells[i + 2, 6].Value = AccountModels.OriginalFile[i].MovementCreditBalance;
                            worksheet.Cells[i + 2, 7].Value = AccountModels.OriginalFile[i].EndBalanceDebit;
                            worksheet.Cells[i + 2, 8].Value = AccountModels.OriginalFile[i].EndBalanceCredit;

                        }

                        // Auto-fit columns
                        worksheet.Cells.AutoFitColumns();

                        // Convert the Excel package to a byte array
                        byte[] fileContents = package.GetAsByteArray();

                        // Create a MemoryStream from the byte array
                        var stream = new MemoryStream(fileContents);

                        // Create an IFormFile from the MemoryStream
                        var formFile = new FormFile(stream, 0, fileContents.Length, "TrialBalance", fileName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        };

                        // Create an IFormFileCollection and add the form file to it

                        formFileCollection.Add(formFile);
                    }
                    else
                    {
                        if (IsAccountHarmonizationActivated)
                        {
                            var worksheet = package.Workbook.Worksheets.Add("AccountNotPresent");

                            // Add headers
                            worksheet.Cells[1, 1].Value = "AccountNumber";
                            worksheet.Cells[1, 2].Value = "AccountName";

                            // Add data
                            for (int i = 0; i < AccountModels.AccountNotPresent.Count; i++)
                            {
                                worksheet.Cells[i + 2, 1].Value = AccountModels.AccountNotPresent[i].AccountNumber;
                                worksheet.Cells[i + 2, 2].Value = AccountModels.AccountNotPresent[i].AccountName;
                            }

                            // Auto-fit columns
                            worksheet.Cells.AutoFitColumns();

                            // Convert the Excel package to a byte array
                            byte[] fileContents = package.GetAsByteArray();

                            // Create a MemoryStream from the byte array
                            var stream = new MemoryStream(fileContents);

                            // Create an IFormFile from the MemoryStream
                            var formFile = new FormFile(stream, 0, fileContents.Length, "AccountNotPresent", fileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                            };

                            // Create an IFormFileCollection and add the form file to it

                            formFileCollection.Add(formFile);
                        }
                        else
                        {
                            var worksheet = package.Workbook.Worksheets.Add("TrialBalance");

                            // Add headers
                            worksheet.Cells[1, 1].Value = "AccountNumber";
                            worksheet.Cells[1, 2].Value = "AccountName";
                            worksheet.Cells[1, 3].Value = "BeginningDebitBalance";
                            worksheet.Cells[1, 4].Value = "BeginningCreditBalance";
                            worksheet.Cells[1, 5].Value = "DebitBalance";
                            worksheet.Cells[1, 6].Value = "CreditBalance";
                            worksheet.Cells[1, 7].Value = "EndingDebitBalance";
                            worksheet.Cells[1, 8].Value = "EndingCreditBalance";
                            // Add data
                            for (int i = 0; i < AccountModels.OriginalFile.Count; i++)
                            {
                                worksheet.Cells[i + 2, 1].Value = AccountModels.OriginalFile[i].AccountNumber;
                                worksheet.Cells[i + 2, 2].Value = AccountModels.OriginalFile[i].AccountName;
                                worksheet.Cells[i + 2, 3].Value = AccountModels.OriginalFile[i].BeginningDebitBalance;
                                worksheet.Cells[i + 2, 4].Value = AccountModels.OriginalFile[i].BeginningCreditBalance;
                                worksheet.Cells[i + 2, 5].Value = AccountModels.OriginalFile[i].MovementDebitBalance;
                                worksheet.Cells[i + 2, 6].Value = AccountModels.OriginalFile[i].MovementCreditBalance;
                                worksheet.Cells[i + 2, 7].Value = AccountModels.OriginalFile[i].EndBalanceDebit;
                                worksheet.Cells[i + 2, 8].Value = AccountModels.OriginalFile[i].EndBalanceCredit;

                            }

                            // Auto-fit columns
                            worksheet.Cells.AutoFitColumns();

                            // Convert the Excel package to a byte array
                            byte[] fileContents = package.GetAsByteArray();

                            // Create a MemoryStream from the byte array
                            var stream = new MemoryStream(fileContents);

                            // Create an IFormFile from the MemoryStream
                            var formFile = new FormFile(stream, 0, fileContents.Length, "TrialBalance", fileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                            };

                            // Create an IFormFileCollection and add the form file to it

                            formFileCollection.Add(formFile);
                        }

                    }


                    return formFileCollection;
                }
            }
            catch (Exception ex)
            {
                // Log the error (implement proper error logging in production)
                Console.WriteLine($"Error creating file: {ex.Message}");
                return null; // or throw the exception if you want to handle it at a higher level
            }
        }
         public async Task<ReportDto> ExportAccountingGeneralLedgerDetailsFileAsync(AccountLedgerDetails AccountModels, string file, string reportType, string username)
        {
            var generator = new ExcelReportGenerator();
            string fileName = $"{file}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(_fileStoragePath, fileName);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(reportType);
                worksheet = AccountLedgerReportGenerator.GenerateLedgerReport(worksheet,AccountModels, filePath, BaseUtilities.UtcToLocal().ToString("dd-MM-yyyy hh:mm:ss"), username, "Statement of Account");


                SaveWorkbook(workbook, filePath);
            }
            // Create a new Excel workbook


            // Create and return FileDownloadInfoDto
            return CreateFileDownloadInfo(fileName, $"/FinancialReportExports/{fileName}", filePath, reportType, AccountModels.BranchName, username);
        }

        public async Task<ReportDto> ExportAccountingGeneralLedgerFileAsync(AccountDetails AccountModels, string file, string reportType, string username)
        {

            var generator = new ExcelReportGenerator();
            string fileName = $"{file}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(_fileStoragePath, fileName);
            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(reportType);

                // Set headers and branch information
                worksheet = await generator.GenerateGeneralLedgerStatement(worksheet, AccountModels, BaseUtilities.UtcToLocal().ToString("dd-MM-yyyy hh:mm:ss"), username, "Statement of Account");

                // Save the workbook to the file path
                SaveWorkbook(workbook, filePath);
            }

            // Create and return FileDownloadInfoDto
            return CreateFileDownloadInfo(fileName, $"/FinancialReportExports/{fileName}", filePath, reportType, AccountModels.BranchName, username);
        }

        private ReportDto CreateFileDownloadInfo(string fileName, string downloadPath, string filePath, string reportType, string branchname, string username)
        {
            return new ReportDto
            {
                Id = Guid.NewGuid().ToString(),
                FileName = Path.GetFileNameWithoutExtension(fileName),
                Extension = Path.GetExtension(fileName),
                DownloadPath = downloadPath,
                FullPath = filePath,
                FileType = "Excel",
                ReportType = reportType,
                BranchName = branchname,
                Username = username,
                Size = BaseUtilities.GetFileSize(filePath) // Implement GetFileSize method to get the file size
            };
        }





        //public async Task<IXLWorksheet> GenerateAccountingJournal(IXLWorksheet worksheet, AccountingEntriesReport model, string printedDate, string username, string reportTitle)
        //{
        //    // Set header styles and merge cells for Bank Name
        //    int headerColumns = 8; // Define the number of columns for the header

        //    // Set Bank Name and style
        //    worksheet.Range(1, 1, 1, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetBold().Font.SetFontSize(16).Font.SetFontColor(XLColor.Black)
        //        .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
        //        .Border.SetOutsideBorderColor(XLColor.Navy);
        //    worksheet.Cell(1, 1).Value = model.Name;

        //    // Set Branch Location
        //    worksheet.Range(2, 1, 2, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(2, 1).Value = model.BranchLocation;

        //    // Set Branch Code
        //    worksheet.Range(3, 1, 3, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(3, 1).Value = $"BRANCH CODE: {model.BranchCodeX}";

        //    // Set Address Details
        //    worksheet.Range(4, 1, 4, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(4, 1).Value = $"Bp: {model.Address} Tel: {model.HeadOfficeTelePhone}";

        //    // Set Report Title
        //    worksheet.Range(5, 1, 5, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(5, 1).Value = reportTitle;

        //    // Set Printed Date and Username
        //    worksheet.Range(6, 1, 6, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(6, 1).Value = $"Printed Date: {printedDate}";

        //    worksheet.Cell(6, headerColumns - 1).Value = username;
        //    worksheet.Cell(6, headerColumns - 1).Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);

        //    // Set Transactions Header Section
        //    worksheet.Range(7, 1, 7, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetBold().Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(7, 1).Value = "PERIOD";

        //    worksheet.Range(8, 1, 8, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(8, 1).Value = $"From: {model.FromDate} TO {model.ToDate}";

        //    // Set Body Section: Transactions Header
        //    int row = 10;
        //    string[] headers = { "Date", "Reference", "Account Number", "Auxiliary", "Description", "Debit", "Credit" };

        //    for (int col = 1; col <= headers.Length; col++)
        //    {
        //        worksheet.Cell(row, col).Value = headers[col - 1];
        //        worksheet.Cell(row, col).Style
        //            .Font.SetBold().Font.SetFontSize(14)
        //            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //            .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
        //            .Border.SetOutsideBorderColor(XLColor.Navy);
        //    }

        //    // Populate Transaction Data
        //    foreach (var entry in model.AccountingEntries)
        //    {
        //        row++;
        //        worksheet.Cell(row, 1).Value = entry.EntryDate.ToString("dd-MM-yyyy hh:mm:ss");
        //        worksheet.Cell(row, 2).Value = entry.ReferenceID;
        //        worksheet.Cell(row, 3).Value = entry.AccountNumber;
        //        worksheet.Cell(row, 4).Value = entry.IsAuxilaryEntry ? "Yes" : "No";
        //        worksheet.Cell(row, 5).Value = entry.Description;
        //        worksheet.Cell(row, 6).Value = BaseUtilities.ConvertToLong(entry.DrAmount);
        //        worksheet.Cell(row, 7).Value = BaseUtilities.ConvertToLong(entry.CrAmount);
        //    }

        //    // Footer Section: Summary
        //    row += 2;
        //    worksheet.Range(row, 1, row, 2).Merge().Style
        //        .Font.SetBold().Font.SetFontSize(14)
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
        //        .Border.SetOutsideBorderColor(XLColor.Navy);
        //    worksheet.Cell(row, 1).Value = "Summary";

        //    row++;
        //    worksheet.Cell(row, 1).Value = "Total Debit";
        //    worksheet.Cell(row, 2).Value = BaseUtilities.ConvertToLong(model.AccountingEntries.Sum(e => e.DrAmount));

        //    row++;
        //    worksheet.Cell(row, 1).Value = "Total Credit";
        //    worksheet.Cell(row, 2).Value = BaseUtilities.ConvertToLong(model.AccountingEntries.Sum(e => e.CrAmount));

        //    row++;
        //    worksheet.Cell(row, 1).Value = "Total Operations";
        //    worksheet.Cell(row, 2).Value = model.AccountingEntries.Select(x => x.ReferenceID).Distinct().Count();

        //    row++;
        //    worksheet.Cell(row, 1).Value = "Debit Entries";
        //    worksheet.Cell(row, 2).Value = BaseUtilities.ConvertToLong(model.AccountingEntries.Count(e => e.DrAmount > 0));

        //    row++;
        //    worksheet.Cell(row, 1).Value = "Credit Entries";
        //    worksheet.Cell(row, 2).Value = BaseUtilities.ConvertToLong(model.AccountingEntries.Count(e => e.CrAmount > 0));

        //    // Signature Section
        //    row += 2;
        //    worksheet.Range(row, 1, row, headerColumns).Merge().Style
        //        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
        //        .Font.SetItalic().Font.SetFontSize(10).Font.SetFontColor(XLColor.Black);
        //    worksheet.Cell(row, 1).Value = "Printed by Trust Soft Credit";

        //    SetColumnWidths(worksheet, headerColumns);

        //    return worksheet;
        //}




        public async Task<IXLWorksheet> GenerateAccountingGeneralLedger(IXLWorksheet worksheet, AccountDetails model, string printedDate, string username, string reportTitle)
        {
            // Set header styles and merge cells for Bank Name
            int headerColumns = 7;
            worksheet.Range(1, 1, 1, headerColumns).Merge().Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);
            worksheet.Cell(1, 1).Value = model.Name;

            // Set Branch Name
            var branchNameRange = worksheet.Range(2, 1, 2, headerColumns);
            branchNameRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(2, 1).Value = model.BranchLocation;
            branchNameRange.Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            // Set Branch Code
            var codeRange = worksheet.Range(3, 1, 3, headerColumns);
            codeRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(3, 1).Value = $"BRANCH CODE: {model.BranchCode}";
            codeRange.Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            // Set Address Details
            var addressRange = worksheet.Range(4, 1, 4, headerColumns);
            addressRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(4, 1).Value = $"Bp: {model.Address} Tel:{model.HeadOfficeTelePhone}";
            addressRange.Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            var locationRange = worksheet.Range(5, 1, 5, headerColumns);
            locationRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(5, 1).Value = $"Location: {model.BranchLocation}";
            locationRange.Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            // Set Report Title
            var reportTitleRange = worksheet.Range(6, 1, 6, headerColumns);
            reportTitleRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(6, 1).Value = reportTitle;
            reportTitleRange.Style.Font.SetBold().Font.SetFontSize(16).Font.SetFontColor(XLColor.Black);

            // Set Printed Date
            var printedDateRange = worksheet.Range(8, 1, 1, 1);
            printedDateRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(8, 1).Value = "Printed Date:";
            printedDateRange.Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            worksheet.Cell(8, 2).Value = printedDate;
            worksheet.Range(8, 2, 8, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(8, 2, 8, 2).Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            // Set Username
            worksheet.Cell(8, 6).Value = username;
            worksheet.Range(8, 6, 8, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(8, 6, 8, 6).Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            // Set Period Section
            worksheet.Cell(9, 1).Value = "PERIOD";
            worksheet.Range(9, 1, 9, headerColumns).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(9, 1, 9, headerColumns).Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            worksheet.Cell(10, 1).Value = $"From: {model.FromDate}";
            worksheet.Range(10, 1, 10, headerColumns).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(10, 1, 10, headerColumns).Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            worksheet.Cell(10, 2).Value = $"To: {model.ToDate}";
            worksheet.Range(10, 2, 10, headerColumns).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(10, 2, 10, headerColumns).Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            // Set Transactions Header Section
            worksheet.Cell(12, 3).Value = "Comprehensive General Ledger Report: Main and Sub-Account Details";
            worksheet.Range(12, 3, 12, headerColumns).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(12, 3, 12, headerColumns).Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            worksheet.Cell(13, 3).Value = ""; // Empty row for spacing

            // Body Section: Add Ledger Entries
            int row = 14;
            worksheet.Cell(row, 1).Value = "Account Number";
            worksheet.Cell(row, 2).Value = "Description";
            worksheet.Cell(row, 3).Value = "Current Balance";
            foreach (var entry in model.LedgerAccountDetails)
            {
                row++;
                worksheet.Cell(row, 1).Value = entry.AccountNumber;
                worksheet.Cell(row, 2).Value = entry.AccountName;
                worksheet.Cell(row, 7).Value = BaseUtilities.ConvertToLong(entry.CurrentBalance);
                worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
            }

            // Footer Section: Summary
            row++;


            worksheet.Cell(row, 1).Value = "Printed by Trust Soft Credit";
            worksheet.Range(row, 1, row, headerColumns).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range(row, 1, row, headerColumns).Style.Font.SetItalic().Font.SetFontSize(10).Font.SetFontColor(XLColor.Black);
            SetColumnWidths(worksheet, headerColumns);
            return worksheet;
        }



        private void SetColumnWidths(IXLWorksheet worksheet, int headerColumns)
        {
            for (int col = 1; col <= headerColumns; col++)
            {
                worksheet.Column(col).AdjustToContents();
            }
        }




        /// </summary>
        /// <param name="workbook">The Excel workbook to save.</param>
        /// <param name="filePath">The file path where the workbook will be saved.</param>
        private void SaveWorkbook(XLWorkbook workbook, string filePath)
        {

            // Save the workbook to the specified file path
            // Save the workbook to the file
            workbook.SaveAs(filePath);
        }

        public async Task<ReportDto> ExportAccountingEntriesReportFileAsync(AccountingEntriesReport data, string file, string reportType, string? branchName,string username)
        {
            var generator = new ExcelReportGenerator();
            // Create new workbook
           
            string fileName = $"{file}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(_fileStoragePath, fileName);
            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(reportType);

                // Set headers and branch information
                worksheet = await generator.GenerateAccountingJournal (worksheet, data, BaseUtilities.UtcToLocal().ToString("dd-MM-yyyy hh:mm:ss"), username, "ACCOUNTING JOURNAL");

                // Save the workbook to the file path
                SaveWorkbook(workbook, filePath);
            }

            // Create and return FileDownloadInfoDto
            return CreateFileDownloadInfo(fileName, $"/FinancialReportExports/{fileName}", filePath, reportType, branchName, username);
        }

        public async Task<ReportDto> Export6ColumnTrialBalanceAsync(List<TrialBalance6ColumnDto> AccountModels, string file, string reportType, string username,string? branchName)
        {
           
            string fileName = $"{file}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(_fileStoragePath, fileName);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(reportType);
                worksheet = AccountLedgerReportGenerator.Generate6ColumnTrialBalanceReport(worksheet, AccountModels, filePath, BaseUtilities.UtcToLocal().ToString("dd-MM-yyyy hh:mm:ss"), username, "TRIAL BALANCE 6 COLUMN");


                SaveWorkbook(workbook, filePath);
            }
            // Create a new Excel workbook


            // Create and return FileDownloadInfoDto
            return CreateFileDownloadInfo(fileName, $"/FinancialReportExports/{fileName}", filePath, reportType, AccountModels[0].BranchName, username);

        }

        public async Task<ReportDto> Export4ColumnTrialBalanceAsync(List<TrialBalance4ColumnDto> data, string file, string reportType, string fullName, string? branchName)
        {

            string fileName = $"{file}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(_fileStoragePath, fileName);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(reportType);
                worksheet = AccountLedgerReportGenerator.Generate4ColumnTrialBalanceReport(worksheet, data, filePath, BaseUtilities.UtcToLocal().ToString("dd-MM-yyyy hh:mm:ss"), fullName, "TRIAL BALANCE 4 COLUMN");


                SaveWorkbook(workbook, filePath);
            }
            // Create a new Excel workbook


            // Create and return FileDownloadInfoDto
            return CreateFileDownloadInfo(fileName, $"/FinancialReportExports/{fileName}", filePath, reportType, data[0].BranchName, fullName);

        }
    }


}
