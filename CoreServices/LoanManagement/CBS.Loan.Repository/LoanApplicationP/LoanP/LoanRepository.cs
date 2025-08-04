using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Dto.Resources;
using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.Repository.LoanProductP;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanP
{

    public class LoanRepository : GenericRepository<Loan, LoanContext>, ILoanRepository
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly string _fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
        private readonly UserInfoToken _userInfoToken;
        private readonly ILoanProductRepository _loanProductRepository;
        public LoanRepository(IUnitOfWork<LoanContext> unitOfWork, IPropertyMappingService propertyMappingService = null, UserInfoToken userInfoToken = null, ILoanProductRepository loanProductRepository = null) : base(unitOfWork)
        {
            _propertyMappingService = propertyMappingService;
            _userInfoToken = userInfoToken;
            _loanProductRepository = loanProductRepository;
        }

        public async Task<LoanList> GetLoansAsync(LoanResource loanResource)
        {
            // Start with a base query that selects all non-deleted loans
            IQueryable<Loan> query = All.AsNoTracking().Where(x => !x.IsDeleted);

            // Filter loans based on the provided query parameter
            query = FilterLoans(query, loanResource.SearchQuery, loanResource.IsByBranch);

            // Apply sorting based on the provided order by property and ensure it orders by DisbursementDate descending
            query = query.ApplySort(loanResource.OrderBy, _propertyMappingService.GetPropertyMapping<LoanDto, Loan>())
                         .OrderByDescending(x => x.LoanDate);

            // Apply search filter if a search query is provided
            if (!string.IsNullOrWhiteSpace(loanResource.SearchQuery) && loanResource.SearchQuery.ToLower() != "all")
            {
                query = ApplySearchFilter(query, loanResource.SearchQuery.ToLower());
            }

            // Create and return the paginated list of loans
            var loansList = new LoanList();
            return await loansList.Create(query, loanResource.Skip, loanResource.PageSize);
        }

        /// <summary>
        /// Filters the loans query based on the specified query parameter and branch context.
        /// </summary>
        /// <param name="query">The initial query.</param>
        /// <param name="queryParameter">The query parameter for filtering loans.</param>
        /// <param name="IsByBranch">Indicates if filtering should be done by branch.</param>
        /// <returns>The filtered query.</returns>
        private IQueryable<Loan> FilterLoans(IQueryable<Loan> query, string queryParameter, bool IsByBranch)
        {
            // If filtering by branch, add the branch condition to all queries
            if (IsByBranch)
            {
                query = query.Where(x => x.BranchId == _userInfoToken.BranchID);
            }

            // Apply filter based on query parameter
            switch (queryParameter)
            {
                case "Pending":
                    return query.Where(x => !x.IsLoanDisbursted);
                case "Open":
                    return query.Where(x => x.LoanStatus == LoanStatus.Open.ToString());
                case "Closed":
                    return query.Where(x => x.LoanStatus == LoanStatus.Closed.ToString());
                case "Disbursed":
                    return query.Where(x => x.LoanStatus == LoanStatus.Disbursed.ToString());
                case "Restructured":
                    return query.Where(x => x.LoanStatus == LoanStatus.Restructured.ToString());
                default:
                    return query;
            }
        }


        /// <summary>
        /// Applies a search filter to the loans query based on the search query string.
        /// </summary>
        /// <param name="query">The initial query.</param>
        /// <param name="searchQuery">The search query string.</param>
        /// <returns>The filtered query.</returns>
        private IQueryable<Loan> ApplySearchFilter(IQueryable<Loan> query, string searchQuery)
        {
            return query.Where(c =>
                (c.Id != null && c.Id.ToLower().Contains(searchQuery)) ||
                (c.LoanAmount != null && c.LoanAmount.ToString().ToLower().Contains(searchQuery)) ||
                (c.LoanStatus != null && c.LoanStatus.ToLower().Contains(searchQuery)) ||
                (c.CustomerId != null && c.CustomerId.ToLower().Contains(searchQuery))
            );
        }

        // Utility function to create the Excel file



        private FileDownloadInfoDto CreateFileDownloadInfo(string fileName, string downloadPath, string filePath)
        {
            return new FileDownloadInfoDto
            {
                Id = Guid.NewGuid().ToString(),
                FileName = Path.GetFileNameWithoutExtension(fileName),
                Extension = Path.GetExtension(fileName),
                DownloadPath = downloadPath,
                FullPath = filePath,
                FileType = "Excel",
                TransactionType = "Loan Situations",
                Size = BaseUtilities.GetFileSize(filePath) // Implement GetFileSize method to get the file size
            };
        }
        private async Task<FileDownloadInfoDto> CreateLoanExportFile(IQueryable<Loan> query, string branch, string DateFrom, string DateTo, string branchCode, bool IsOnlyUnPaidLoans, bool IsUploadedLoans)
        {
            try
            {
                var loansx = await query.AsNoTracking()
                                        .OrderBy(l => l.LoanDate)
                                        .ToListAsync();
                var loans = MapLoanToLoanDtoExport(loansx);
                if (IsOnlyUnPaidLoans)
                {
                    query.Where(x => x.Balance > 0);
                }
                // Ensure the directory exists
                if (!Directory.Exists(_fileStoragePath))
                {
                    Directory.CreateDirectory(_fileStoragePath);
                }
                string fileName = !IsUploadedLoans? $"Loan-{branchCode}-{DateFrom}-{DateTo}_{DateTime.Now.ToString("hhmmss")}.xlsx": $"Migrated_Loans_{branchCode}_{DateTime.Now.ToString("hhmmss")}.xlsx";
                var filePath = Path.Combine(_fileStoragePath, $"{fileName}");

                // Create a new workbook
                var workbook = new XLWorkbook();

                // Add a worksheet
                var worksheet = workbook.Worksheets.Add("Loans");

                // Add title and branch name
                worksheet.Cell("A1").Value = !IsUploadedLoans ? $"Loan Situations as of {DateFrom} To {DateTo}. Printed on: {DateTime.Now.ToString("dd-MMM-yyyy, hh:mm:ss")}": $"Migrated Loans. Printed on: {DateTime.Now.ToString("dd-MMM-yyyy, hh:mm:ss")}";
                worksheet.Cell("A2").Value = $"Branch: {branch}, Code: {branchCode}";

                // Merge, center, and bold the title and branch name, and set font size to 14 and text color to black
                var titleRange = worksheet.Range("A1:X1");
                titleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.FontSize = 14;
                titleRange.Style.Font.FontColor = XLColor.Black;
                // Apply border to title range
                titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                var branchRange = worksheet.Range("A2:X2");
                branchRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                branchRange.Style.Font.Bold = true;
                branchRange.Style.Font.FontSize = 14;
                branchRange.Style.Font.FontColor = XLColor.Black;
                // Apply border to branch range
                branchRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Move headers below title and branch name
                int row = 4;

                // Add headers
                var headers = new string[]
                {
                    "Acc. Number", "Name", "B.Code","L.Date","Amount","L.Balance", 
                    "Due.Int","D.Days", "I.Rate","L.Type", "Loan Purpose","L.R.Date",
                    "Due.Amount", "Due.Date","Acc.Int", "Paid","Duration.M", "Saving", 
                    "Deposit", "P-Share", "O-Share", "Salary","Coverage", "Status"
                };

                // Set header row, bold headers, add light blue background, increase font size, and wrap text
                var headerRow = worksheet.Row(4);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Font.FontSize = 12;
                headerRow.Style.Alignment.WrapText = true;

                // Calculate end column
                int endColumn = headers.Length;

                // Apply background color, font size, and wrap text to the header range
                var headerRange = worksheet.Range(4, 1, 4, endColumn);
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Font.FontSize = 12;
                headerRange.Style.Alignment.WrapText = true;

                // Populate header values
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(4, i + 1).Value = headers[i];
                }

                // Add borders to headers
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(4, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                // Increment row for the data
                row++;

                // Group loans by LoanType
                var groupedLoans = loans.GroupBy(l => l.LoanType);

                decimal grandTotalLoanAmount = 0;
                decimal grandTotalAccrualInterest = 0;
                decimal grandTotalDueInterest = 0;
                decimal grandTotalPaid = 0;
                decimal grandTotalBalance = 0;

                foreach (var loanTypeGroup in groupedLoans)
                {
                    // Group loans within each LoanType group by duration and order the groups
                    var durations = loanTypeGroup.GroupBy(l => CalculateDurationGroup(l.DisbursementDate))
                                                 .OrderBy(g => GetDurationOrder(g.Key));

                    foreach (var durationGroup in durations)
                    {
                        // Add loan type group header
                        worksheet.Cell(row, 1).Value = loanTypeGroup.Key;
                        worksheet.Range(row, 1, row, headers.Length).Merge();
                        worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range(row, 1, row, headers.Length).Style.Font.Bold = true; // Bold the title
                        row++;

                        // Add duration group header
                        worksheet.Cell(row, 1).Value = durationGroup.Key;
                        worksheet.Range(row, 1, row, headers.Length).Merge();
                        worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Range(row, 1, row, headers.Length).Style.Font.Bold = true; // Bold the title
                        row++;

                        decimal totalLoanAmount = 0;
                        decimal totalAccrualInterest = 0;
                        decimal totalDeliquentInterest = 0;
                        decimal totalPaid = 0;
                        decimal totalBalance = 0;

                        foreach (var loan in durationGroup)
                        {
                            var loanDuration = (DateTime.Now - loan.MaturityDate).Days;

                            // Calculate coverage and status
                            decimal coverage = 0; /*(loan.Saving + loan.Deposit + loan.PShare + loan.OShare + loan.Salary) / (loan.Balance + loan.AccrualInterest) * 100;*/
                            string status = coverage >= 100 ? "Insure" : "Not Assured";
                            // Add loan details
                            worksheet.Cell(row, 1).Value = loan.CustomerId;
                            worksheet.Cell(row, 2).Value = loan.CustomerName;
                            worksheet.Cell(row, 3).Value = loan.BranchCode;
                            worksheet.Cell(row, 4).Value = loan.LoanDate.ToString("yyyy-MM-dd");
                            worksheet.Cell(row, 5).Value = loan.LoanAmount;
                            worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 6).Value = loan.Balance;
                            worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 7).Value = loan.DeliquentInterest;
                            worksheet.Cell(row, 8).Value = loanDuration;
                            worksheet.Cell(row, 9).Value = loan.InterestRate;
                            worksheet.Cell(row, 10).Value = loan.LoanType;
                            worksheet.Cell(row, 11).Value = loan.LoanApplication.LoanPurpose.PurposeName;
                            worksheet.Cell(row, 12).Value = loan.LastRefundDate.ToString("yyyy-MM-dd");
                            worksheet.Cell(row, 13).Value = loan.DueAmount;
                            worksheet.Cell(row, 13).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 14).Value = loan.MaturityDate.ToString("yyyy-MM-dd");
                            worksheet.Cell(row, 15).Value = loan.AccrualInterest;
                            worksheet.Cell(row, 15).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 16).Value = loan.Paid;
                            worksheet.Cell(row, 16).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 17).Value = loan.LoanDuration;
                            worksheet.Cell(row, 18).Value = loan.Saving;
                            worksheet.Cell(row, 18).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 19).Value = loan.Deposit;
                            worksheet.Cell(row, 19).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 20).Value = loan.PShare;
                            worksheet.Cell(row, 20).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 21).Value = loan.OShare;
                            worksheet.Cell(row, 21).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 22).Value = loan.Salary;
                            worksheet.Cell(row, 22).Style.NumberFormat.Format = "#,##0.0";
                            worksheet.Cell(row, 23).Value = coverage;
                            worksheet.Cell(row, 23).Style.NumberFormat.Format = "#,##0.0 [$%]";
                            worksheet.Cell(row, 24).Value = status;

                            // Conditional formatting for Coverage
                            if (coverage > 100)
                            {
                                worksheet.Cell(row, 23).Style.Fill.BackgroundColor = XLColor.Green;
                            }

                            // Apply borders to data cells
                            for (int i = 1; i <= headers.Length; i++)
                            {
                                worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }

                            // Calculate totals for this duration group
                            totalLoanAmount += loan.LoanAmount;
                            totalAccrualInterest += loan.AccrualInterest;
                            totalDeliquentInterest += loan.DeliquentInterest;
                            totalPaid += loan.Paid;
                            totalBalance += loan.Balance;

                            row++;
                        }

                        // Adjust columns in the duration group
                        worksheet.Columns(1, headers.Length).AdjustToContents();

                        // Add totals row for the duration group
                        worksheet.Cell(row, 1).Value = "TOTAL";
                        worksheet.Cell(row, 1).Style.Font.Bold = true;
                        worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
                        worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.DarkBrown;
                        worksheet.Cell(row, 11).Value = totalLoanAmount;
                        worksheet.Cell(row, 11).Style.NumberFormat.Format = "#,##0.0";
                        worksheet.Cell(row, 12).Value = totalAccrualInterest;
                        worksheet.Cell(row, 12).Style.NumberFormat.Format = "#,##0.0";
                        worksheet.Cell(row, 13).Value = totalDeliquentInterest;
                        worksheet.Cell(row, 14).Value = totalPaid;
                        worksheet.Cell(row, 14).Style.NumberFormat.Format = "#,##0.0";
                        worksheet.Cell(row, 15).Value = totalBalance;
                        worksheet.Cell(row, 15).Style.NumberFormat.Format = "#,##0.0";

                        // Apply borders to totals row
                        for (int i = 1; i <= headers.Length; i++)
                        {
                            worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        row++;

                        // Accumulate grand totals
                        grandTotalLoanAmount += totalLoanAmount;
                        grandTotalAccrualInterest += totalAccrualInterest;
                        grandTotalDueInterest += totalDeliquentInterest;
                        grandTotalPaid += totalPaid;
                        grandTotalBalance += totalBalance;
                    }
                }

                // Add grand totals row at the end
                worksheet.Cell(row, 1).Value = "GRAND TOTAL";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
                worksheet.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.Black;
                worksheet.Cell(row, 11).Value = grandTotalLoanAmount;
                worksheet.Cell(row, 11).Style.NumberFormat.Format = "#,##0.0";
                worksheet.Cell(row, 12).Value = grandTotalAccrualInterest;
                worksheet.Cell(row, 12).Style.NumberFormat.Format = "#,##0.0";
                worksheet.Cell(row, 13).Value = grandTotalDueInterest;
                worksheet.Cell(row, 14).Value = grandTotalPaid;
                worksheet.Cell(row, 14).Style.NumberFormat.Format = "#,##0.0";
                worksheet.Cell(row, 15).Value = grandTotalBalance;
                worksheet.Cell(row, 15).Style.NumberFormat.Format = "#,##0.0";

                // Apply borders to grand totals row
                for (int i = 1; i <= headers.Length; i++)
                {
                    worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }


                // Footer calculations
                int totalLoans = loans.Count;
                decimal totalLoanVolume = loans.Sum(l => l.LoanAmount);
                decimal totalAccrualInterestVolume = loans.Sum(l => l.AccrualInterest);
                decimal totalDueInterestVolume = loans.Sum(l => l.DeliquentInterest);
                decimal totalRefundVolume = loans.Sum(l => l.Paid);
                decimal percentageRefund = Math.Round((totalRefundVolume / totalLoanVolume) * 100, 1);
                decimal totalOutstanding = loans.Sum(l => l.Balance);
                decimal totalDueAmount = loans.Sum(l => l.DueAmount);

                // Add footer summary
                worksheet.Cell(row, 1).Value = "SUMMARY";
                worksheet.Range(row, 1, row, 2).Merge();
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Range(row, 1, row, 2).Style.Font.Bold = true;
                worksheet.Range(row, 1, row, 2).Style.Font.FontSize = 13;
                worksheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.DarkBrown;
                worksheet.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                row++;

                worksheet.Cell(row, 1).Value = "1. Number of Loans";
                worksheet.Cell(row, 2).Value = totalLoans;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                row++;

                worksheet.Cell(row, 1).Value = "2. Volume of Loan";
                worksheet.Cell(row, 2).Value = totalLoanVolume;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
                row++;

                worksheet.Cell(row, 1).Value = "3. Volume of Accrual Interest";
                worksheet.Cell(row, 2).Value = totalAccrualInterestVolume;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
                row++;

                worksheet.Cell(row, 1).Value = "4. Volume of Due Interest";
                worksheet.Cell(row, 2).Value = totalDueInterestVolume;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
                row++;

                worksheet.Cell(row, 1).Value = "5. Volume of Refund";
                worksheet.Cell(row, 2).Value = totalRefundVolume;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
                row++;

                worksheet.Cell(row, 1).Value = "6. Percentage of Refund";
                worksheet.Cell(row, 2).Value = percentageRefund;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "0.0"; // Percentage format
                row++;

                worksheet.Cell(row, 1).Value = "7. Outstanding Loan";
                worksheet.Cell(row, 2).Value = totalOutstanding;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
                row++;

                worksheet.Cell(row, 1).Value = "8. Total Due Amount";
                worksheet.Cell(row, 2).Value = totalDueAmount;
                worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
                row++;

                // Add border to the footer
                worksheet.Range(row - 8, 1, row, 2).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(row - 8, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Autofit column width for all columns
                worksheet.Columns().AdjustToContents();

                // Save the workbook to the file
                workbook.SaveAs(filePath);

                // Return the download path
                var downloadPath = $"/exports/{fileName}";
                var fileDownloadInfo = CreateFileDownloadInfo(fileName, downloadPath, filePath);
                return fileDownloadInfo;


            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the loan export file.", ex);
            }
        }

        // Helper method to calculate the duration group based on the disbursement date
        private string CalculateDurationGroup(DateTime disbursementDate)
        {
            int days = (DateTime.Now - disbursementDate).Days;
            if (days <= 30) return "0-1 Month";
            if (days <= 180) return "2-6 Months";
            if (days <= 365) return "7-12 Months";
            if (days <= 730) return "13-24 Months";
            if (days <= 1095) return "25-36 Months";
            return "36+ Months";
        }

        // Helper method to get the predefined order for duration groups
        private int GetDurationOrder(string durationGroup)
        {
            switch (durationGroup)
            {
                case "0-1 Month": return 1;
                case "2-6 Months": return 2;
                case "7-12 Months": return 3;
                case "13-24 Months": return 4;
                case "25-36 Months": return 5;
                case "36+ Months": return 6;
                default: return int.MaxValue;
            }
        }

        //private async Task<FileDownloadInfoDto> CreateLoanExportFile(IQueryable<Loan> query, string branch, string DateFrom, string DateTo, string branchCode)
        //{
        //    try
        //    {
        //        var loansx = await query.AsNoTracking()
        //                               .OrderBy(l => l.LoanDate)
        //                               .ToListAsync();
        //        //var loanProducts = await _loanProductRepository.All.AsNoTracking().ToListAsync();

        //        var loans = MapLoanToLoanDtoExport(loansx);

        //        // Ensure the directory exists
        //        if (!Directory.Exists(_fileStoragePath))
        //        {
        //            Directory.CreateDirectory(_fileStoragePath);
        //        }
        //        string fileName = $"Loan-{branchCode}-{DateFrom}-{DateTo}_{DateTime.Now.ToString("hhmmss")}.xlsx";
        //        var filePath = Path.Combine(_fileStoragePath, $"{fileName}");

        //        // Create a new workbook
        //        var workbook = new XLWorkbook();

        //        // Add a worksheet
        //        var worksheet = workbook.Worksheets.Add("Loans");

        //        // Add title and branch name
        //        worksheet.Cell("A1").Value = $"Loan Situations as of {DateFrom} To {DateTo}. Printed on: {DateTime.Now.ToString("dd-MMM-yyyy, hh:mm:ss")}";
        //        worksheet.Cell("A2").Value = $"Branch: {branch}, Code: {branchCode}";

        //        // Merge, center, and bold the title and branch name, and set font size to 14 and text color to black
        //        var titleRange = worksheet.Range("A1:T1");
        //        titleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        titleRange.Style.Font.Bold = true;
        //        titleRange.Style.Font.FontSize = 14;
        //        titleRange.Style.Font.FontColor = XLColor.Black;
        //        // Apply border to title range
        //        titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //        var branchRange = worksheet.Range("A2:T2");
        //        branchRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        branchRange.Style.Font.Bold = true;
        //        branchRange.Style.Font.FontSize = 14;
        //        branchRange.Style.Font.FontColor = XLColor.Black;
        //        // Apply border to branch range
        //        branchRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //        // Move headers below title and branch name
        //        int row = 4;

        //        // Add headers
        //        var headers = new string[]
        //        {
        //    "Acc. Number", "Name", "B.Code", "L.Type", "Loan Purpose", "L.Date", "L.Age", "D.Date",
        //    "Int.Rate", "L.R.Date", "Amount", "Acc.Int", "Due.Int", "Paid", "Balance",
        //    "Due.Amount", "Duration.M", "Saving", "Deposit", "P-Share", "O-Share", "Salary",
        //    "Coverage", "Status"
        //        };

        //        // Set header row, bold headers, add light blue background, increase font size, and wrap text
        //        var headerRow = worksheet.Row(4);
        //        headerRow.Style.Font.Bold = true;
        //        headerRow.Style.Font.FontSize = 12;
        //        headerRow.Style.Alignment.WrapText = true;

        //        // Calculate end column
        //        int endColumn = headers.Length;

        //        // Apply background color, font size, and wrap text to the header range
        //        var headerRange = worksheet.Range(4, 1, 4, endColumn);
        //        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        //        headerRange.Style.Font.FontSize = 12;
        //        headerRange.Style.Alignment.WrapText = true;

        //        // Populate header values
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            worksheet.Cell(4, i + 1).Value = headers[i];
        //        }

        //        // Add borders to headers
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            worksheet.Cell(4, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        }

        //        // Increment row for the data
        //        row++;

        //        // Group loans by LoanType
        //        var groupedLoans = loans.GroupBy(l => l.LoanType);

        //        foreach (var loanTypeGroup in groupedLoans)
        //        {
        //            // Group loans within each LoanType group by duration
        //            var durations = loanTypeGroup.GroupBy(l => CalculateDurationGroup(l.DisbursementDate));

        //            foreach (var durationGroup in durations)
        //            {
        //                // Add loan type group header
        //                worksheet.Cell(row, 1).Value = loanTypeGroup.Key;
        //                worksheet.Range(row, 1, row, headers.Length).Merge();
        //                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                worksheet.Range(row, 1, row, headers.Length).Style.Font.Bold = true; // Bold the title
        //                row++;

        //                // Add duration group header
        //                worksheet.Cell(row, 1).Value = durationGroup.Key;
        //                worksheet.Range(row, 1, row, headers.Length).Merge();
        //                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                worksheet.Range(row, 1, row, headers.Length).Style.Font.Bold = true; // Bold the title
        //                row++;

        //                decimal totalLoanAmount = 0;
        //                decimal totalAccrualInterest = 0;
        //                decimal totalDueInterest = 0;
        //                decimal totalPaid = 0;
        //                decimal totalBalance = 0;
        //                //decimal totalDueAmount = 0;

        //                foreach (var loan in durationGroup)
        //                {
        //                    var loanDuration = (DateTime.Now-loan.MaturityDate).Days;

        //                    // Calculate coverage and status
        //                    decimal coverage = (loan.Saving + loan.Deposit + loan.PShare + loan.OShare + loan.Salary) / (loan.Balance + loan.AccrualInterest) * 100;
        //                    string status = coverage >= 100 ? "Insure" : "Not Assured";

        //                    // Add loan details
        //                    worksheet.Cell(row, 1).Value = loan.CustomerId;
        //                    worksheet.Cell(row, 2).Value = loan.CustomerName;
        //                    worksheet.Cell(row, 3).Value = loan.BranchCode;
        //                    worksheet.Cell(row, 4).Value = loan.LoanType;
        //                    worksheet.Cell(row, 5).Value = loan.LoanApplication.LoanPurpose.PurposeName;
        //                    worksheet.Cell(row, 6).Value = loan.LoanDate.ToString("yyyy-MM-dd");
        //                    worksheet.Cell(row, 7).Value = loanDuration;
        //                    worksheet.Cell(row, 8).Value = loan.MaturityDate.ToString("yyyy-MM-dd");
        //                    worksheet.Cell(row, 9).Value = loan.InterestRate;
        //                    worksheet.Cell(row, 10).Value = loan.LastRefundDate.ToString("yyyy-MM-dd");
        //                    worksheet.Cell(row, 11).Value = loan.LoanAmount;
        //                    worksheet.Cell(row, 11).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 12).Value = loan.AccrualInterest;
        //                    worksheet.Cell(row, 12).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 13).Value = loan.DeliquentInterest;
        //                    worksheet.Cell(row, 14).Value = loan.Paid;
        //                    worksheet.Cell(row, 14).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 15).Value = loan.Balance;
        //                    worksheet.Cell(row, 15).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 16).Value = loan.DueAmount;
        //                    worksheet.Cell(row, 16).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 17).Value = loan.LoanDuration;
        //                    worksheet.Cell(row, 18).Value = loan.Saving;
        //                    worksheet.Cell(row, 18).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 19).Value = loan.Deposit;
        //                    worksheet.Cell(row, 19).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 20).Value = loan.PShare;
        //                    worksheet.Cell(row, 20).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 21).Value = loan.OShare;
        //                    worksheet.Cell(row, 21).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 22).Value = loan.Salary;
        //                    worksheet.Cell(row, 22).Style.NumberFormat.Format = "#,##0.0";
        //                    worksheet.Cell(row, 23).Value = coverage;
        //                    worksheet.Cell(row, 23).Style.NumberFormat.Format = "#,##0.0 [$%]";
        //                    worksheet.Cell(row, 24).Value = status;

        //                    // Conditional formatting for Coverage
        //                    if (coverage > 100)
        //                    {
        //                        worksheet.Cell(row, 23).Style.Fill.BackgroundColor = XLColor.Green;
        //                    }

        //                    // Apply borders to data cells
        //                    for (int i = 1; i <= headers.Length; i++)
        //                    {
        //                        worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //                    }

        //                    // Calculate totals for this duration group
        //                    totalLoanAmount += loan.LoanAmount;
        //                    totalAccrualInterest += loan.AccrualInterest;
        //                    totalDueInterest += loan.DeliquentInterest;
        //                    totalPaid += loan.Paid;
        //                    totalBalance += loan.Balance;
        //                    //totalDueAmount += loan.DueAmount;

        //                    row++;
        //                }

        //                // Autofit column width for all columns
        //                worksheet.Columns().AdjustToContents();

        //                // Add totals row for the duration group
        //                worksheet.Cell(row, 10).Value = "TOTAL";
        //                worksheet.Cell(row, 11).Value = totalLoanAmount;
        //                worksheet.Cell(row, 11).Style.NumberFormat.Format = "#,##0.0";
        //                worksheet.Cell(row, 12).Value = totalAccrualInterest;
        //                worksheet.Cell(row, 12).Style.NumberFormat.Format = "#,##0.0";
        //                worksheet.Cell(row, 13).Value = totalDueInterest;
        //                worksheet.Cell(row, 13).Style.NumberFormat.Format = "#,##0.0";
        //                worksheet.Cell(row, 14).Value = totalPaid;
        //                worksheet.Cell(row, 14).Style.NumberFormat.Format = "#,##0.0";
        //                worksheet.Cell(row, 15).Value = totalBalance;
        //                worksheet.Cell(row, 15).Style.NumberFormat.Format = "#,##0.0";
        //                worksheet.Cell(row, 15).Value = totalBalance;
        //                worksheet.Cell(row, 15).Style.NumberFormat.Format = "#,##0.0";
        //                //worksheet.Cell(row, 16).Value = totalDueAmount;
        //                worksheet.Range(row, 1, row, headers.Length).Style.Font.SetBold();

        //                // Apply borders to total row
        //                for (int i = 1; i <= headers.Length; i++)
        //                {
        //                    worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //                }

        //                row++;
        //            }
        //        }

        //        // Footer calculations
        //        int totalLoans = loans.Count;
        //        decimal totalLoanVolume = loans.Sum(l => l.LoanAmount);
        //        decimal totalAccrualInterestVolume = loans.Sum(l => l.AccrualInterest);
        //        decimal totalDueInterestVolume = loans.Sum(l => l.DeliquentInterest);
        //        decimal totalRefundVolume = loans.Sum(l => l.Paid);
        //        decimal percentageRefund = Math.Round((totalRefundVolume / totalLoanVolume) * 100, 1);
        //        decimal totalOutstanding = loans.Sum(l => l.Balance);
        //        decimal totalDueAmount = loans.Sum(l => l.DueAmount);

        //        // Add footer summary
        //        worksheet.Cell(row, 1).Value = "SUMMARY";
        //        worksheet.Range(row, 1, row, 2).Merge();
        //        worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //        worksheet.Range(row, 1, row, 2).Style.Font.Bold = true;
        //        worksheet.Range(row, 1, row, 2).Style.Font.FontSize = 13;
        //        worksheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightBlue;
        //        worksheet.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        row++;

        //        worksheet.Cell(row, 1).Value = "1. Number of Loans";
        //        worksheet.Cell(row, 2).Value = totalLoans;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        row++;

        //        worksheet.Cell(row, 1).Value = "2. Volume of Loan";
        //        worksheet.Cell(row, 2).Value = totalLoanVolume;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        worksheet.Cell(row, 1).Value = "3. Volume of Accrual Interest";
        //        worksheet.Cell(row, 2).Value = totalAccrualInterestVolume;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        worksheet.Cell(row, 1).Value = "4. Volume of Due Interest";
        //        worksheet.Cell(row, 2).Value = totalDueInterestVolume;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        worksheet.Cell(row, 1).Value = "5. Volume of Refund";
        //        worksheet.Cell(row, 2).Value = totalRefundVolume;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        worksheet.Cell(row, 1).Value = "6. Percentage of Refund";
        //        worksheet.Cell(row, 2).Value = percentageRefund;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "0.0"; // Percentage format
        //        row++;

        //        worksheet.Cell(row, 1).Value = "7. Outstanding Loan";
        //        worksheet.Cell(row, 2).Value = totalOutstanding;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        worksheet.Cell(row, 1).Value = "8. Total Due Amount";
        //        worksheet.Cell(row, 2).Value = totalDueAmount;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        // Add border to the footer
        //        worksheet.Range(row - 8, 1, row, 2).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Range(row - 8, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //        // Autofit column width for all columns
        //        worksheet.Columns().AdjustToContents();

        //        // Save the workbook to the file
        //        workbook.SaveAs(filePath);

        //        // Return the download path
        //        var downloadPath = $"/exports/{fileName}";
        //        var fileDownloadInfo = CreateFileDownloadInfo(fileName, downloadPath, filePath);
        //        return fileDownloadInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}



        //private async Task<FileDownloadInfoDto> CreateLoanExportFile(IQueryable<Loan> query, string branch, string DateFrom, string DateTo, string branchCode)
        //{
        //    try
        //    {
        //        var loansx = await query.AsNoTracking()
        //                               .OrderBy(l => l.LoanDate)
        //                               .ToListAsync();

        //        var loans = MapLoanToLoanDtoExport(loansx);

        //        // Ensure the directory exists
        //        if (!Directory.Exists(_fileStoragePath))
        //        {
        //            Directory.CreateDirectory(_fileStoragePath);
        //        }
        //        string fileName = $"Loan-{branchCode}-{DateFrom}-{DateTo}_{DateTime.Now.ToString("hhmmss")}.xlsx";
        //        var filePath = Path.Combine(_fileStoragePath, $"{fileName}");

        //        // Create a new workbook
        //        var workbook = new XLWorkbook();

        //        // Add a worksheet
        //        var worksheet = workbook.Worksheets.Add("Loans");

        //        // Add title and branch name
        //        worksheet.Cell("A1").Value = $"Loan Situations as of {DateFrom} To {DateTo}. Printed on: {DateTime.Now.ToString("dd-MMM-yyyy, hh:mm:ss")}";
        //        worksheet.Cell("A2").Value = $"Branch: {branch}, Code: {branchCode}";

        //        // Merge, center, and bold the title and branch name, and set font size to 14 and text color to black
        //        var titleRange = worksheet.Range("A1:M1");
        //        titleRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        titleRange.Style.Font.Bold = true;
        //        titleRange.Style.Font.FontSize = 14;
        //        titleRange.Style.Font.FontColor = XLColor.Black;
        //        // Apply border to title range
        //        titleRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //        var branchRange = worksheet.Range("A2:M2");
        //        branchRange.Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        branchRange.Style.Font.Bold = true;
        //        branchRange.Style.Font.FontSize = 14;
        //        branchRange.Style.Font.FontColor = XLColor.Black;
        //        // Apply border to branch range
        //        branchRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        //        // Move headers below title and branch name
        //        int row = 4;

        //        // Add headers
        //        var headers = new string[]
        //        {
        //            "Acc. Number", "Name", "B.Code", "Loan Type", "L.Date", "Age", "D.Date",
        //            "I.Rate", "L.P Date", "Amount", "Paid", "Balance", "Duration"
        //        };

        //        // Set header row, bold headers, add light blue background, increase font size, and wrap text
        //        var headerRow = worksheet.Row(4);
        //        headerRow.Style.Font.Bold = true;
        //        headerRow.Style.Font.FontSize = 12;
        //        headerRow.Style.Alignment.WrapText = true;

        //        // Calculate end column
        //        int endColumn = headers.Length;

        //        // Apply background color, font size, and wrap text to the header range
        //        var headerRange = worksheet.Range(4, 1, 4, endColumn);
        //        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        //        headerRange.Style.Font.FontSize = 12;
        //        headerRange.Style.Alignment.WrapText = true;

        //        // Populate header values
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            worksheet.Cell(4, i + 1).Value = headers[i];
        //        }

        //        // Add borders to headers
        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            worksheet.Cell(4, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        }

        //        // Increment row for the data
        //        row++;

        //        // Group loans by LoanType
        //        var groupedLoans = loans.GroupBy(l => l.LoanType);

        //        foreach (var loanTypeGroup in groupedLoans)
        //        {
        //            // Group loans within each LoanType group by duration
        //            var durations = loanTypeGroup.GroupBy(l => CalculateDurationGroup(l.DisbursementDate));

        //            foreach (var durationGroup in durations)
        //            {
        //                // Add loan type group header
        //                worksheet.Cell(row, 1).Value = loanTypeGroup.Key;
        //                worksheet.Range(row, 1, row, headers.Length).Merge();
        //                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                worksheet.Range(row, 1, row, headers.Length).Style.Font.Bold = true; // Bold the title
        //                row++;

        //                // Add duration group header
        //                worksheet.Cell(row, 1).Value = durationGroup.Key;
        //                worksheet.Range(row, 1, row, headers.Length).Merge();
        //                worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                worksheet.Range(row, 1, row, headers.Length).Style.Font.Bold = true; // Bold the title
        //                row++;

        //                decimal totalLoanAmount = 0;
        //                decimal totalPaid = 0;
        //                decimal totalBalance = 0;

        //                foreach (var loan in durationGroup)
        //                {
        //                    var loanDuration = (DateTime.Now - loan.DisbursementDate).Days;

        //                    // Add loan details
        //                    worksheet.Cell(row, 1).Value = loan.CustomerId;
        //                    worksheet.Cell(row, 2).Value = loan.CustomerName;
        //                    worksheet.Cell(row, 3).Value = loan.BranchCode;
        //                    worksheet.Cell(row, 4).Value = loan.LoanType;
        //                    worksheet.Cell(row, 5).Value = loan.LoanDate.ToString("yyyy-MM-dd");
        //                    worksheet.Cell(row, 6).Value = loanDuration;
        //                    worksheet.Cell(row, 7).Value = loan.MaturityDate.ToString("yyyy-MM-dd");
        //                    worksheet.Cell(row, 8).Value = loan.InterestRate;
        //                    worksheet.Cell(row, 9).Value = loan.LastRefundDate.ToString("yyyy-MM-dd");
        //                    worksheet.Cell(row, 10).Value = loan.LoanAmount;
        //                    worksheet.Cell(row, 11).Value = loan.Paid;
        //                    worksheet.Cell(row, 12).Value = loan.Balance;
        //                    worksheet.Cell(row, 13).Value = loan.LoanDuration;

        //                    // Apply borders to data cells
        //                    for (int i = 1; i <= headers.Length; i++)
        //                    {
        //                        worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //                    }

        //                    // Calculate totals for this duration group
        //                    totalLoanAmount += loan.LoanAmount;
        //                    totalPaid += loan.Paid;
        //                    totalBalance += loan.Balance;

        //                    row++;
        //                }

        //                // Autofit column width for all columns
        //                worksheet.Columns().AdjustToContents();

        //                // Add totals row for the duration group
        //                worksheet.Cell(row, 9).Value = "TOTAL";
        //                worksheet.Cell(row, 10).Value = totalLoanAmount;
        //                worksheet.Cell(row, 11).Value = totalPaid;
        //                worksheet.Cell(row, 12).Value = totalBalance;
        //                worksheet.Range(row, 1, row, headers.Length).Style.Font.SetBold();

        //                // Apply borders to total row
        //                for (int i = 1; i <= headers.Length; i++)
        //                {
        //                    worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //                }

        //                row++;
        //            }
        //        }

        //        // Footer calculations
        //        int totalLoans = loans.Count;
        //        decimal totalLoanVolume = loans.Sum(l => l.LoanAmount);
        //        decimal totalRefundVolume = loans.Sum(l => l.Paid);
        //        decimal percentageRefund = Math.Round((totalRefundVolume / totalLoanVolume) * 100, 1);
        //        decimal totalOutstanding = loans.Sum(l => l.Balance);

        //        // Add footer summary
        //        worksheet.Cell(row, 1).Value = "SUMMARY";
        //        worksheet.Range(row, 1, row, 2).Merge();
        //        worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        //        worksheet.Range(row, 1, row, 2).Style.Font.Bold = true;
        //        worksheet.Range(row, 1, row, 2).Style.Font.FontSize = 13;
        //        worksheet.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightBlue;
        //        worksheet.Range(row, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        row++;

        //        worksheet.Cell(row, 1).Value = "1. Number of Loans";
        //        worksheet.Cell(row, 2).Value = totalLoans;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        row++;

        //        worksheet.Cell(row, 1).Value = "2. Volume of Loan";
        //        worksheet.Cell(row, 2).Value = totalLoanVolume;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        worksheet.Cell(row, 1).Value = "3. Volume of Refund";
        //        worksheet.Cell(row, 2).Value = totalRefundVolume;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        worksheet.Cell(row, 1).Value = "4. Percentage of Refund";
        //        worksheet.Cell(row, 2).Value = percentageRefund;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "0.0"; // Percentage format
        //        row++;

        //        worksheet.Cell(row, 1).Value = "5. Outstanding Loan";
        //        worksheet.Cell(row, 2).Value = totalOutstanding;
        //        worksheet.Cell(row, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Cell(row, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
        //        row++;

        //        // Add border to the footer
        //        worksheet.Range(row - 6, 1, row, 2).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Range(row - 6, 1, row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;



        //        // Autofit column width for all columns
        //        worksheet.Columns().AdjustToContents();

        //        // Save the workbook to the file
        //        workbook.SaveAs(filePath);

        //        // Return the download path
        //        var downloadPath = $"/exports/{fileName}";
        //        var fileDownloadInfo = CreateFileDownloadInfo(fileName, downloadPath, filePath);
        //        return fileDownloadInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        // Helper method to calculate duration group based on disbursement date
        //private string CalculateDurationGroup(DateTime disbursementDate)
        //{
        //    int daysSinceDisbursement = (DateTime.Now - disbursementDate).Days;

        //    if (daysSinceDisbursement <= 30)
        //    {
        //        return "Up to 30 days";
        //    }
        //    else if (daysSinceDisbursement <= 60)
        //    {
        //        return "31-60 days";
        //    }
        //    else if (daysSinceDisbursement <= 90)
        //    {
        //        return "61-90 days";
        //    }
        //    else if (daysSinceDisbursement <= 120)
        //    {
        //        return "91-120 days";
        //    }
        //    else
        //    {
        //        return "Above 120 days";
        //    }
        //}

        // Export loans without branch filter, with date range
        public async Task<FileDownloadInfoDto> ExportLoansAsync(DateTime startDate, DateTime endDate, string branchName, string branchCode, string queryParameter, bool IsOnlyUnpaidLoans)
        {
            if (queryParameter == "MigratedLoans")
            {
                var query = All.AsNoTracking().Where(x => x.IsDeleted == false && x.IsUpload);
                return await CreateLoanExportFile(query, branchName, string.Empty, string.Empty, branchCode, IsOnlyUnpaidLoans,true);

            }
            else
            {
                var query = All.AsNoTracking().Where(x => x.IsDeleted == false && x.LoanDate.Date >= startDate.Date && x.LoanDate.Date <= endDate.Date);
                return await CreateLoanExportFile(query, branchName, BaseUtilities.ConvertToyyyyMMdd(startDate), BaseUtilities.ConvertToyyyyMMdd(endDate), branchCode, IsOnlyUnpaidLoans,false);

            }
        }
        // Export loans with branch filter and date range
        public async Task<FileDownloadInfoDto> ExportLoansAsync(string branchId, DateTime startDate, DateTime endDate, string branchName, string branchCode, string queryParameter, bool IsOnlyUnpaidLoans)
        {

            if (queryParameter=="MigratedLoans")
            {
                // Optimize query by using AsNoTracking at the start and filtering
                var query = All.Include(x => x.LoanApplication).ThenInclude(x => x.LoanPurpose).AsNoTracking()
                               .Where(x => x.BranchId == branchId && x.IsUpload
                                           && !x.IsDeleted);


                return await CreateLoanExportFile(query, branchName, string.Empty, string.Empty, branchCode, IsOnlyUnpaidLoans,true);

            }
            else
            {
                // Optimize query by using AsNoTracking at the start and filtering
                var query = All.Include(x => x.LoanApplication).ThenInclude(x => x.LoanPurpose).AsNoTracking()
                               .Where(x => x.BranchId == branchId
                                           && !x.IsDeleted
                                           && x.LoanDate.Date >= startDate.Date
                                           && x.LoanDate.Date <= endDate.Date);


                return await CreateLoanExportFile(query, branchName, BaseUtilities.ConvertToyyyyMMdd(startDate), BaseUtilities.ConvertToyyyyMMdd(endDate), branchCode, IsOnlyUnpaidLoans,false);

            }
        }
        //public async Task<FileDownloadInfoDto> ExportLoansAsync(string branchId, DateTime startDate, DateTime endDate, string branchName, string branchCode, string queryParameter)
        //{
        //    // Parse the query parameter to match the LoanStatus enum
        //    if (!Enum.TryParse(queryParameter, true, out LoanStatus loanStatus))
        //    {
        //        throw new ArgumentException("Invalid loan status", nameof(queryParameter));
        //    }

        //    // Optimize query by using AsNoTracking at the start and filtering
        //    var query = All.AsNoTracking()
        //                   .Where(x => x.BranchId == branchId
        //                               && !x.IsDeleted
        //                               && x.LoanDate.Date >= startDate.Date
        //                               && x.LoanDate.Date <= endDate.Date
        //                               && x.LoanStatus == loanStatus.ToString());

        //    return await CreateLoanExportFile(query, branchName, BaseUtilities.ConvertToyyyyMMdd(startDate), BaseUtilities.ConvertToyyyyMMdd(endDate), branchCode);
        //}
        // Export all loans
        //public async Task<FileDownloadInfoDto> ExportLoansAsync()
        //{
        //    var query = All.AsNoTracking().Where(x => x.IsDeleted == false);
        //    return await CreateLoanExportFile(query, "All Branches");
        //}

        public List<LoanDtoExport> MapLoanToLoanDtoExport(List<Loan> loans)
        {
            var loanDtoExports = new List<LoanDtoExport>();

            foreach (var loan in loans)
            {
                var loanDto = new LoanDtoExport
                {
                    Id = loan.Id,
                    LoanApplicationId = loan.LoanApplicationId,
                    Principal = loan.Principal,
                    LoanAmount = loan.LoanAmount,
                    InterestForcasted = loan.InterestForcasted,
                    InterestRate = loan.InterestRate,
                    LastPayment = loan.LastPayment,
                    Paid = loan.Paid,
                    Balance = loan.Principal,
                    DeliquentInterest = loan.DeliquentInterest,
                    DueAmount = loan.Balance,
                    AccrualInterest = loan.AccrualInterest,
                    LastCalculatedInterest = loan.LastCalculatedInterest,
                    AccrualInterestPaid = loan.AccrualInterestPaid,
                    TotalPrincipalPaid = loan.TotalPrincipalPaid,
                    Tax = loan.Tax,
                    TaxPaid = loan.TaxPaid,
                    FeePaid = loan.FeePaid,
                    Fee = loan.Fee,
                    Penalty = loan.Penalty,
                    PenaltyPaid = loan.PenaltyPaid,
                    DisbursementDate = loan.DisbursementDate,
                    FirstInstallmentDate = loan.FirstInstallmentDate,
                    NextInstallmentDate = loan.NextInstallmentDate,
                    LoanDate = loan.LoanDate,
                    IsLoanDisbursted = loan.IsLoanDisbursted,
                    DisbursmentStatus = loan.DisbursmentStatus,
                    LastInterestCalculatedDate = loan.LastInterestCalculatedDate,
                    LastRefundDate = loan.LastRefundDate,
                    LastEventData = loan.LastEventData,
                    CustomerId = loan.CustomerId,
                    LoanManager = loan.LoanManager,
                    LoanStatus = loan.LoanStatus,
                    NewLoanId = loan.NewLoanId,
                    IsWriteOffLoan = loan.IsWriteOffLoan,
                    IsDeliquentLoan = loan.IsDeliquentLoan,
                    IsCurrentLoan = loan.IsCurrentLoan,
                    MaturityDate = loan.MaturityDate,
                    OrganizationId = loan.OrganizationId,
                    BranchId = loan.BranchId,
                    BankId = loan.BankId,
                    LoanType = loan.LoanType,
                    BranchCode = loan.BranchCode,
                    LoanId = loan.LoanId,
                    CustomerName = loan.CustomerName,
                    LoanDuration = loan.LoanDuration,
                    LoanJourneyStatus = loan.LoanJourneyStatus,
                    VatRate = loan.VatRate,
                    LoanTarget = loan.LoanTarget,
                    LoanCategory = loan.LoanCategory,
                    AccountNumber = loan.AccountNumber,
                    IsUpload = loan.IsUpload,
                    Saving = 0,
                    Deposit = 0,
                    PShare = 0,
                    OShare = 0,
                    Salary = 0,
                    LoanApplication = loan.LoanApplication
                };

                loanDtoExports.Add(loanDto);
            }

            return loanDtoExports;
        }

    }

}
