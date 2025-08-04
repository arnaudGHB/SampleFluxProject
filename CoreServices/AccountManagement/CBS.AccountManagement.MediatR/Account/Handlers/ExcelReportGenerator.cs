using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class ExcelReportGenerator
    {
        private const int HEADER_COLUMNS = 7;
        private const string NUMBER_FORMAT = "#,##0";
        private readonly IXLWorksheet _worksheet;
        private int _currentRow = 10;
        private decimal _runningBalance;
        private decimal _totalDebit;
        private decimal _totalCredit;

        /// <summary>
        /// Generates an accounting journal worksheet with formatted headers, transactions, and summaries
        /// </summary>
        public async Task<IXLWorksheet> GenerateAccountingJournal(
            IXLWorksheet worksheet,
            AccountingEntriesReport model,
            string printedDate,
            string username,
            string reportTitle)
        {
            // Generate each section of the journal
            GenerateCompanyHeader(worksheet, model, HEADER_COLUMNS);
            GenerateReportMetadata(worksheet, printedDate, username, reportTitle, HEADER_COLUMNS);
            GeneratePeriodHeader(worksheet, model, "TRANSACTIONS", HEADER_COLUMNS);
            var lastRow = GenerateAccountingEntriesTransactions(worksheet, model);
            GenerateJournalEntrySummary(worksheet, model, lastRow);
            GenerateSignature(worksheet, lastRow + 7, HEADER_COLUMNS);

            SetColumnWidths(worksheet, HEADER_COLUMNS);
            return worksheet;
        }

        public async Task<IXLWorksheet> GenerateGeneralLedgerStatement(
      IXLWorksheet worksheet,
      AccountDetails model,
      string printedDate,
      string username,
      string reportTitle)
        {
            // Generate each section of the journal
            GenerateCompanyHeader(worksheet, model, 3);
            GenerateReportMetadata(worksheet, printedDate, username, reportTitle, 3);
            GeneratePeriodHeader(worksheet, model, "Comprehensive General Ledger Report: Main and Sub-Account Details", 3);
            var lastRow = GenerateGeneralLedgerStatementTransactions(worksheet, model);
            //GenerateSummary(worksheet, model, lastRow);
            GenerateSignature(worksheet, lastRow - 1, 3);

            SetColumnWidths(worksheet, 3);
            return worksheet;
        }

        public async Task<IXLWorksheet> GenerateGeneralLedgerDetailsStatement(
   IXLWorksheet worksheet,
   AccountLedgerDetails model,
   string printedDate,
   string username,
   string reportTitle)
        {
            // Generate each section of the journal
            GenerateCompanyHeader(worksheet, model, HEADER_COLUMNS);
            GenerateReportMetadata(worksheet, printedDate, username, reportTitle, HEADER_COLUMNS);
            GeneratePeriodHeader(worksheet, model, "", HEADER_COLUMNS);

            ProcessDetailAccounts(worksheet, model.LedgerDetails);

            worksheet.Columns().AdjustToContents();

            //GenerateSummary(worksheet, model, lastRow);
            GenerateSignature(worksheet, _currentRow, HEADER_COLUMNS);

            SetColumnWidths(worksheet, 3);
            return worksheet;
        }

        private void ProcessDetailAccounts(IXLWorksheet worksheet, List<LedgerDetails>? collection)
        {
            foreach (var item in collection)
            {
                AddAccountHeader(worksheet, item);
                AddTableHeaders(worksheet);
                ProcessEntries(worksheet, item.AccountingEntries);
                AddSummarySection(worksheet, item.AccountingEntries);
            }
        }



        private void SetColumnWidths(IXLWorksheet worksheet, int headerColumns)
        {
            for (int col = 1; col <= headerColumns; col++)
            {
                worksheet.Column(col).AdjustToContents();
            }
        }

     private void GenerateCompanyHeader(IXLWorksheet worksheet, ReportHeader model, int HEADER_COLUMNS)
        {
            // Create and merge the entire header range
            var entireHeaderRange = worksheet.Range(1, 1, 4, HEADER_COLUMNS).Merge()
                .Style.ApplyHeaderStyle()
                            .Font.SetFontName("Bahnschrift")
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Blue);

            // Combine all values with line breaks
            var headerText = $"{model.Name}\n" +
                            $"{model.BranchName}\n" +
                            $"BRANCH CODE: {model.BranchCode}\n" +
                            $" {model.BranchLocation} Tel: {model.BranchTelephone}";

            // Set the combined text in the merged cell
            worksheet.Cell(1, 1).Value = headerText;
            worksheet.Cell(1, 1).Style.Font.SetFontName("Bahnschrift");
            worksheet.Row(4).Height = 40;
            // Configure text wrapping and alignment
            worksheet.Cell(1, 1).Style
                .Alignment.SetWrapText(true)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }
        /// <summary>
        /// Generates the report metadata section including title and print info

        private void GenerateReportMetadata(
         IXLWorksheet worksheet,
         string printedDate,
         string username,
         string reportTitle, int HEADER_COLUMNS)
        {
            // Report title - bold and centered
            var titleRange = worksheet.Range(5, 1, 5, HEADER_COLUMNS);
            titleRange.Merge().Style.ApplyHeaderStyle().Font.SetFontSize(20).Font.SetBold();
            titleRange.Merge().Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
            titleRange.Merge().Style.Border.SetOutsideBorderColor(XLColor.Blue);
            worksheet.Cell(5, 1).Value = reportTitle;
            worksheet.Cell(5, 1).Style.Font.SetFontName("Bahnschrift");
      
            ApplyCenteredMergedStyle(worksheet, 6, $"Printed Date: {printedDate} by {username}", HEADER_COLUMNS);
     
            worksheet.Cell(6, HEADER_COLUMNS - 1).Value = username;
            worksheet.Cell(6, HEADER_COLUMNS - 1).Style.ApplyHeaderStyle();
        }


        private void GeneratePeriodHeader(IXLWorksheet worksheet, ReportHeader model, string secondTitle, int HEADER_COLUMNS)
        {
            var Value = $"FROM:        {model.FromDate:dd-MM-yyyy}        TO:          {model.ToDate:dd-MM-yyyy}";

            // Create the merged range
            var periodTagRange = worksheet.Range(7, 1, 9, HEADER_COLUMNS).Merge()
                .Style.ApplyHeaderStyle()
                .Border.SetTopBorder(XLBorderStyleValues.Thick)
                .Border.SetBottomBorder(XLBorderStyleValues.Thick)
                .Border.SetLeftBorder(XLBorderStyleValues.Thick)
                .Border.SetRightBorder(XLBorderStyleValues.Thick)
                .Border.SetTopBorderColor(XLColor.Blue)
                .Border.SetBottomBorderColor(XLColor.Blue)
                .Border.SetLeftBorderColor(XLColor.Blue)
                .Border.SetRightBorderColor(XLColor.Blue);

            // Set values and apply alignment
            worksheet.Cell(7, 1).Value = "PERIOD";
            worksheet.Cell(8, 1).Value = Value;
            worksheet.Cell(9, 1).Value = secondTitle;

            // Increase row height for row 9
            worksheet.Row(9).Height = 30; // You can adjust this value as needed

            // Add line breaks between values in the merged cell
            worksheet.Cell(7, 1).Value = $"PERIOD\n{Value}\n{secondTitle}";
            worksheet.Cell(7, 1).Style.Font.SetFontName("Bahnschrift");
            // Ensure text wrapping and vertical alignment is set
            worksheet.Cell(7, 1).Style
                .Alignment.SetWrapText(true)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        }

        /// <summary>
        /// Generates the transaction entries section with headers and data
        /// </summary>
        private int GenerateAccountingEntriesTransactions(IXLWorksheet worksheet, AccountingEntriesReport model)
        {
            var row = 10;
            GenerateAccountingEntriesTransactionHeaders(worksheet, row);
            var collections = model.AccountingEntries.OrderBy(x=>x.ValueDate).ToList();
              collections = model.AccountingEntries.OrderBy(x => x.CreatedDate).ToList();
            // Add each transaction entry
            foreach (var entry in  collections)
            {
                row++;
                AddAccountingEntriesTransactionRow(worksheet, row, entry);
            }

            return row;
        }

        /// <summary>
        /// Generates the transaction table headers
        /// </summary>
        private void GenerateAccountingEntriesTransactionHeaders(IXLWorksheet worksheet, int row)
        {
            string[] headers = { "Date", "Reference", "Account Number", "Auxiliary", "Description", "Debit", "Credit" };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(row, i + 1).Value = headers[i];
                worksheet.Cell(row, i + 1).Style
                .Font.SetBold()
                .Font.SetFontSize(14)
                .Font.SetFontName("Bahnschrift")
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
                    .Border.SetOutsideBorderColor(XLColor.Blue);
            }
        }

        /// <summary>
        /// Generates the transaction entries section with headers and data
        /// </summary>

        public int GenerateGeneralLedgerStatementTransactions(IXLWorksheet worksheet, AccountDetails model)
        {
            // Group accounts by category
            var groupedAccounts = model.LedgerAccountDetails
                .GroupBy(x => x.Cartegory)
                .OrderBy(x => x.Key);

            int currentRow = 10;

            foreach (var category in groupedAccounts)
            {
                // Category header
                worksheet.Cell(currentRow, 1).Value = category.Key;
                worksheet.Range(currentRow, 1, currentRow, 3).Merge()
                    .Style
                    .Font.SetBold()
                     .Font.SetFontSize(16.0)
                    .Font.SetFontColor(XLColor.Blue)
                    .Fill.SetBackgroundColor(XLColor.White)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                currentRow++;

                // Column headers for this category
                worksheet.Cell(currentRow, 1).Value = "Account Number";
                worksheet.Cell(currentRow, 2).Value = "Account Name";
                worksheet.Cell(currentRow, 3).Value = "Current Balance";
         
                // Style the headers
                worksheet.Range(currentRow, 1, currentRow, 3).Style
                  

                             .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                              .Border.SetTopBorder(XLBorderStyleValues.Thin)
                              .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                               .Border.SetRightBorder(XLBorderStyleValues.Thin)
                               //.Border.SetTopBorderColor(XLColor.Black)
                               .Border.SetLeftBorderColor(XLColor.Blue)
                               .Border.SetRightBorderColor(XLColor.Blue)
                             //.Border.SetBottomBorderColor(XLColor.Black)
                    .Font.SetBold()
                       .Font.SetFontSize(14.0)
                    .Fill.SetBackgroundColor(XLColor.White)
                   .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Blue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                currentRow++;

                // Data rows for this category
                foreach (var account in category)
                {
                    worksheet.Cell(currentRow, 1).Value = account.AccountNumber;
                    worksheet.Cell(currentRow, 2).Value = account.AccountName;
                    worksheet.Cell(currentRow, 3).Value = BaseUtilities.ConvertToLong(account.CurrentBalance) ;
                    worksheet.Cell(currentRow, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
                         .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                              .Border.SetTopBorder(XLBorderStyleValues.Thin)
                              .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                               .Border.SetRightBorder(XLBorderStyleValues.Thin)
                               //.Border.SetTopBorderColor(XLColor.Black)
                               .Border.SetLeftBorderColor(XLColor.Black)
                               .Border.SetRightBorderColor(XLColor.Black);
                    worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = NUMBER_FORMAT;
                    worksheet.Cell(currentRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                              .Border.SetTopBorder(XLBorderStyleValues.Thin)
                              .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                               .Border.SetRightBorder(XLBorderStyleValues.Thin)
                               //.Border.SetTopBorderColor(XLColor.Black)
                               .Border.SetLeftBorderColor(XLColor.Black)
                               .Border.SetRightBorderColor(XLColor.Black);
                    worksheet.Cell(currentRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                      .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                              .Border.SetTopBorder(XLBorderStyleValues.Thin)
                              .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                               .Border.SetRightBorder(XLBorderStyleValues.Thin)
                               //.Border.SetTopBorderColor(XLColor.Black)
                               .Border.SetLeftBorderColor(XLColor.Black)
                               .Border.SetRightBorderColor(XLColor.Black);
                    // Style the data rows
                    worksheet.Range(currentRow, 1, currentRow, 3).Style
                        .Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                    currentRow++;
                }

                // Add a blank row between categories
                currentRow++;
            }

            // Auto-fit columns
            worksheet.Columns(1, 3).AdjustToContents();
            return currentRow;
        }

      public struct CellData
        {
            public int Column { get; set; }
            public object Value { get; set; }
        }


        private void AddAccountingEntriesTransactionRow(IXLWorksheet worksheet, int row, AccountingEntryDto entry)
        {
            // Create the row range first to apply the outer border
            var rowRange = worksheet.Range(row, 1, row, HEADER_COLUMNS);
            rowRange.Style
                 .Font.SetFontName("Bahnschrift")
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Black);

            // Column 1: Entry Date
            var cell1 = worksheet.Cell(row, 1);
            cell1.Value = entry.EntryDate.ToString("dd-MM-yyyy HH:mm:ss");
            cell1.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            SetCellBorders(cell1);

            // Column 2: Reference ID
            var cell2 = worksheet.Cell(row, 2);
            cell2.Value = entry.ReferenceID;
            cell2.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            SetCellBorders(cell2);

            // Column 3: Account Number
            var cell3 = worksheet.Cell(row, 3);
            cell3.Value = entry.CrAmount>entry.DrAmount?entry.CrAccountNumber:entry.DrAccountNumber;
            cell3.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            SetCellBorders(cell3);

            // Column 4: Representative
            var cell4 = worksheet.Cell(row, 4);
            cell4.Value = entry.Representative;
            cell4.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            SetCellBorders(cell4);

            // Column 5: Naration
            var cell5 = worksheet.Cell(row, 5);
            cell5.Value = entry.Naration;
            cell5.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            SetCellBorders(cell5);

            // Column 6: Debit Amount

            var cell6 = worksheet.Cell(row, 6);
            cell6.Value = BaseUtilities.ConvertToLong(entry.DrAmount);
            cell6.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            cell6.Style.NumberFormat.Format = NUMBER_FORMAT;
            SetCellBorders(cell6);

            // Column 7: Credit Amount
            var cell7 = worksheet.Cell(row, 7);
            cell7.Value = BaseUtilities.ConvertToLong(entry.CrAmount);
            cell7.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            cell7.Style.NumberFormat.Format = NUMBER_FORMAT;
            SetCellFinalBorders(cell7);
        }


        private void SetCellBorders(IXLCell cell)
        {
            cell.Style.Border .SetLeftBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);

            cell.Style.Border.LeftBorderColor = XLColor.Black;
            cell.Style.Border.RightBorderColor = XLColor.Blue;
        }
        private void SetCellFinalBorders(IXLCell cell)
        {
      
            cell.Style.Border.SetRightBorder(XLBorderStyleValues.Thick);

        
            cell.Style.Border.RightBorderColor = XLColor.Blue;
        }
        /// <summary>
        /// Generates the summary section with totals and counts
        /// </summary>
        private void GenerateJournalEntrySummary(IXLWorksheet worksheet, AccountingEntriesReport model, int startRow)
        {
            var row = startRow + 2;

            // Summary header
            var summaryRange = worksheet.Range(row, 1, row + 5, 2);
            summaryRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick)
                .Border.SetOutsideBorderColor(XLColor.Blue);

            worksheet.Range(row, 1, row, 2).Merge()
                .Style.ApplyHeaderStyle()
                .Font.SetBold()
                .Font.SetFontName("Bahnschrift")
                .Font.SetFontSize(12);
            worksheet.Cell(row, 1).Value = "Summary";

            // Summary data
            var summaryData = new[]
            {
            ("Total Debit", BaseUtilities.ConvertToLong(model.AccountingEntries.Sum(e => e.DrAmount))),
            ("Total Credit", BaseUtilities.ConvertToLong(model.AccountingEntries.Sum(e => e.CrAmount))),
            ("Total Operations", model.AccountingEntries.Select(x => x.ReferenceID).Distinct().Count()),
            ("Debit Entries", BaseUtilities.ConvertToLong(model.AccountingEntries.Count(e => e.DrAmount > 0))),
            ("Credit Entries", BaseUtilities.ConvertToLong(model.AccountingEntries.Count(e => e.CrAmount > 0)))
        };

            foreach (var (label, value) in summaryData)
            {
                row++;
                AddSummaryRow(worksheet, row, label, value);
            }
        }

     
        /// <summary>
        /// Adds a single summary row with label and value
        /// </summary>
        private void AddSummaryRow(IXLWorksheet worksheet, int row, string label, object value)
        {
            for (int col = 1; col <= 2; col++)
            {
                if (col==2)
                {
                    var cell = worksheet.Cell(row, col);
                    cell.Value = col == 1 ? label :BaseUtilities.ConvertToLong( value);
                    cell.Style.Font.SetFontName("Bahnschrift")
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
                        .Border.SetThinBorder();
                    cell.Style.NumberFormat.Format = NUMBER_FORMAT;
                }
                else
                {
                    var cell = worksheet.Cell(row, col);
                    cell.Value = col == 1 ? label : BaseUtilities.ConvertToLong(value);
                    cell.Style.Font.SetFontName("Bahnschrift Light")
                     .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Border.SetThinBorder();
                    cell.Style.NumberFormat.Format = NUMBER_FORMAT;
                }

            }
        }
        /// <summary>
        /// Generates the Printed by  section at the bottom of the report
        /// </summary>
        private void GeneratePrintedBy(IXLWorksheet worksheet, int row, int HEADER_COLUMNS)
        {
            row = row + 3;
            worksheet.Range(row, 1, row, HEADER_COLUMNS).Merge()
                .Style.ApplyHeaderStyle()
                .Font.SetFontName("Bahnschrift Light")
                .Font.SetFontSize(10);
            worksheet.Cell(row, 1).Value = "Printed by Trust Soft Credit";
        }

        /// <summary>
        /// Generates the signature section at the bottom of the report
        /// </summary>
        private void GenerateSignature(IXLWorksheet worksheet, int row, int HEADER_COLUMNS)
        {
            row = row + 3;
            worksheet.Range(row, 1, row, HEADER_COLUMNS).Merge()
                .Style.ApplyHeaderStyle()
                .Font.SetFontName("Bahnschrift Light")
                .Font.SetBold(true)
                .Font.SetFontSize(10);
            worksheet.Cell(row, 1).Value = "Printed by Trust Soft Credit";
        }

        /// <summary>
        /// Applies centered and merged style to a row
        /// </summary>
        private void ApplyCenteredMergedStyle(IXLWorksheet worksheet, int row, string value, int HEADER_COLUMNS)
        {
            worksheet.Range(row, 1, row, HEADER_COLUMNS).Merge()
                .Style.ApplyHeaderStyle();
            worksheet.Cell(row, 1).Style.Font.SetFontName("Bahnschrift Light");
            worksheet.Cell(row, 1).Value = value;
        }
        private void AddAccountHeader(IXLWorksheet _worksheet, LedgerDetails ledger)
        {
            _worksheet.Cell(_currentRow, 1).Value = "Account N°:";
            _worksheet.Cell(_currentRow, 2).Value = ledger.AccountNumber;
            _worksheet.Cell(_currentRow, 5).Value = ledger.AccountName;
            _worksheet.Cell(_currentRow+1, 1).Value = "XAF FRANC CFA (BEAC)";
            _worksheet.Cell(_currentRow+4, 1).Value = "Beginning Balance:";
            _worksheet.Cell(_currentRow+4, 2).Value = ledger.BeginningBalance;
        }

        private void AddTableHeaders(IXLWorksheet _worksheet)
        {
            string[] headers = { "Date", "Description", "Reference", "Representative", "Debit", "Credit", "Balance" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = _worksheet.Cell(6, i + 1);
                FormatHeaderCell(cell, headers[i]);
            }
            _currentRow = 10;
        }

        private void FormatHeaderCell(IXLCell cell, string headerText)
        {
            cell.Value = headerText;
            cell.Style
                .Font.SetBold(true)
                .Font.SetFontSize(14)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Blue);
        }

        private void ProcessEntries(IXLWorksheet _worksheet, List<AccountingEntryDto> entries)
        {
            foreach (var entry in entries)
            {
                AddEntryRow(_worksheet, entry);
                UpdateTotals(entry);
                _currentRow++;
            }
        }

        private void AddEntryRow(IXLWorksheet _worksheet, AccountingEntryDto entry)
        {
            _runningBalance += entry.DrAmount - entry.CrAmount;

            var rowData = new object[]
            {
            entry.EntryDate.ToShortDateString(),
            entry.Description,
            entry.ReferenceID,
            entry.Representative,
            entry.DrAmount,
            entry.CrAmount,
                _runningBalance
            };

            var row = _worksheet.Row(_currentRow);
            for (int i = 0; i < rowData.Length; i++)
            {
                var cell = row.Cell(i + 1);
                cell.Value = rowData[i].ToString();
                ApplyDataCellBorder(cell);

                // Format numeric columns
                if (i >= 4 && i <= 7) // Debit, Credit, Balance columns
                {
                    cell.Style.NumberFormat.SetFormat("#,##0");
                }
            }
        }

        private void ApplyDataCellBorder(IXLCell cell)
        {
            cell.Style
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Black);
        }

        private void UpdateTotals(AccountingEntryDto entry)
        {
            _totalDebit += entry.DrAmount;
            _totalCredit += entry.CrAmount;
        }

        private void AddSummarySection(IXLWorksheet _worksheet, List<AccountingEntryDto> entries)
        {
            _currentRow++;
            var summaryData = new[]
            {
            ("Total Debit:", _totalDebit),
            ("Total Credit:", _totalCredit),
            ("Account Balance:", _runningBalance),
            ("Total Operation:", entries.Count),
            ("Debit:", entries.Count(x => x.DrAmount > 0)),
            ("Credit:", entries.Count(x => x.CrAmount > 0))
        };

            foreach (var (label, value) in summaryData)
            {
                var labelCell = _worksheet.Cell(_currentRow, 1);
                var valueCell = _worksheet.Cell(_currentRow, 2);

                labelCell.Value = label;
                valueCell.Value = value;

                // Format numeric values
                if (value is decimal)
                {
                    valueCell.Style.NumberFormat.SetFormat("#,##0.00");
                }

                _currentRow++;
            }
        }

    }

    /// <summary>
    /// Extension methods for XLStyle to maintain consistent styling
    /// </summary>
    /// <summary>
    /// Extension methods for XLStyle to maintain consistent styling
    /// </summary>
    public static class XLStyleExtensions
    {
        public static IXLStyle ApplyHeaderStyle(this IXLStyle style)
        {
            return style.Font.SetFontName("Bahnschrift Light")
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Font.SetFontSize(12)

                .Font.SetFontColor(XLColor.Black);
        }

        public static void SetThinBorder(this IXLBorder border)
        {
            border.OutsideBorder = XLBorderStyleValues.Thin;
            border.OutsideBorderColor = XLColor.Blue;
        }

        public static void SetNoBorder(this IXLBorder border)
        {
            border.OutsideBorder = XLBorderStyleValues.None;
            border.OutsideBorderColor = XLColor.Black;
        }

    }
}
