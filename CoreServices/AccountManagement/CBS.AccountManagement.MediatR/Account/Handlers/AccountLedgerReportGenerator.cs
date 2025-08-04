using Amazon.Auth.AccessControlPolicy;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Handlers;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

public class AccountLedgerReportGenerator
{
    private static readonly int HEADER_COLUMNS = 7;
    private static void GenerateCompanyHeader(IXLWorksheet worksheet, ReportHeader model, int HEADER_COLUMNS)
    {
        // Create and merge the entire header range
        var entireHeaderRange = worksheet.Range(1, 1, 4, HEADER_COLUMNS).Merge()
            .Style.ApplyHeaderStyle()
            .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
            .Border.SetOutsideBorderColor(XLColor.Blue);
        worksheet.Range(1, 1, 4, HEADER_COLUMNS).Merge().Style.Font.FontName = "Bahnschrift Light";

        worksheet.Range(1, 1, 4, HEADER_COLUMNS).Merge().Style.Font.Bold = false;
        // Combine all values with line breaks
        var headerText = $"{model.Name}\n" +
                        $"{model.Location}\n" +
                        $"BRANCH CODE: {model.BranchCode}\n" +
                        $"Bp: {model.Address} Tel: {model.HeadOfficeTelePhone}";

        // Set the combined text in the merged cell
        worksheet.Cell(1, 1).Value = headerText;
        worksheet.Row(4).Height = 50.0;
        // Configure text wrapping and alignment
        worksheet.Cell(1, 1).Style
            .Alignment.SetWrapText(true)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }

    /// <summary>
    /// Generates the report metadata section including title and print info
    /// </summary>
    public static IXLWorksheet GenerateLedgerReport(IXLWorksheet worksheet, AccountLedgerDetails model, string filePath, string printedDate, string username, string reportTitle)
    {
        const int HEADER_COLUMNS = 7;

        // Define some styles
        var headerStyle = worksheet.Style;
        headerStyle.Font.Bold = true;
        headerStyle.Font.FontSize = 11;
        headerStyle.Border.OutsideBorder = XLBorderStyleValues.Thick;
        headerStyle.Border.SetOutsideBorderColor(XLColor.Blue);
        headerStyle.Border.SetTopBorder(XLBorderStyleValues.Thick);
        headerStyle.Border.SetBottomBorder(XLBorderStyleValues.Thick);
        headerStyle.Border.SetLeftBorder(XLBorderStyleValues.Thick);
        headerStyle.Border.SetRightBorder(XLBorderStyleValues.Thick);
        headerStyle.Border.SetBottomBorderColor(XLColor.Blue);
        headerStyle.Border.SetTopBorderColor(XLColor.Blue);
        headerStyle.Border.SetLeftBorderColor(XLColor.Blue);
        headerStyle.Border.SetRightBorderColor(XLColor.Blue);
        var borderStyle = worksheet.Style;
        borderStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
        borderStyle.Border.SetBottomBorderColor(XLColor.Black);
        borderStyle.Border.SetTopBorderColor(XLColor.Blue);
        borderStyle.Border.SetLeftBorderColor(XLColor.Blue);
        borderStyle.Border.SetRightBorderColor(XLColor.Blue);
        GenerateCompanyHeader(worksheet, model, HEADER_COLUMNS);
        GenerateReportMetadata(worksheet, printedDate, username, reportTitle, HEADER_COLUMNS);
        GeneratePeriodHeader(worksheet, model, "", HEADER_COLUMNS);

        int currentRow = 11;

        foreach (var ledger in model.LedgerDetails)
        {
            // Header section
            worksheet.Cell(currentRow, 1).Style.Font.FontName = "Bahnschrift Light";
            worksheet.Cell(currentRow, 1).Value = "Account N°:";
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 2).Value = ledger.AccountNumber;
            worksheet.Cell(currentRow, 2).Style.Font.Bold = false;
            worksheet.Cell(currentRow, 2).Style.Font.FontName = "Bahnschrift Light";
            //worksheet.Cell(currentRow, 4).Value = "";
            //worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
            //worksheet.Cell(currentRow, 5).Value = ledger.AccountName;
            //worksheet.Cell(currentRow, 5).Style.Font.Bold = false;
            //worksheet.Cell(currentRow, 5).Style.Font.FontName = "Bahnschrift Light";
            worksheet.Cell(currentRow + 2, 1).Value = "Beginning Balance:";
            worksheet.Cell(currentRow + 2, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow + 2, 2).Value = ConvertToLong(ledger.BeginningBalance);
            worksheet.Cell(currentRow + 2, 2).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(currentRow + 2, 2).Style.Font.FontName = "Bahnschrift Light";
            worksheet.Cell(currentRow + 1, 1).Value = "Account Name:";
            worksheet.Cell(currentRow + 1, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow + 1, 2).Value = ledger.AccountName;
            worksheet.Cell(currentRow + 1, 2).Style.Font.Bold = false;
            worksheet.Cell(currentRow + 3, 2).Style.Font.FontName = "Bahnschrift Light";
            worksheet.Cell(currentRow + 3, 1).Value = "Currency:";
            worksheet.Cell(currentRow + 3, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow + 3, 2).Value = "XAF FRANC CFA (BEAC)";
            worksheet.Cell(currentRow + 3, 2).Style.Font.Bold = false;
            worksheet.Cell(currentRow + 3, 2).Style.Font.FontName = "Bahnschrift Light";
            worksheet.Range(currentRow, 1, currentRow + 3, 2)
           .Style
           .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
           .Border.SetOutsideBorderColor(XLColor.Blue);
            // Summary Table Headers
            int headerRow = currentRow + 4;
            worksheet.Cell(headerRow, 1).Value = "Date";
            worksheet.Cell(headerRow, 2).Value = "Reference";
            worksheet.Cell(headerRow, 3).Value = "Representative";
            worksheet.Cell(headerRow, 4).Value = "Description";

            worksheet.Cell(headerRow, 5).Value = "Debit";
            worksheet.Cell(headerRow, 6).Value = "Credit";
            worksheet.Cell(headerRow, 7).Value = "Balance";

            worksheet.Range(headerRow, 1, headerRow, 7).Style = headerStyle;

            // Populate table data
            int dataStartRow = headerRow + 1;
            decimal runningBalance = 0;

            if (ledger.AccountingEntries != null)
            {
                foreach (var entry in ledger.AccountingEntries)
                {
                    worksheet.Cell(dataStartRow, 1).Value = entry.EntryDatetime;
                    worksheet.Cell(dataStartRow, 4).Value = entry.Description == null ? "N/A" : entry.Description;
                    worksheet.Row(4).Height = 20;
                    // Configure text wrapping and alignment
                    worksheet.Cell(dataStartRow, 4).Style

                        .Alignment.SetWrapText(true)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                    worksheet.Cell(dataStartRow, 2).Value = entry.ReferenceID;
                    worksheet.Cell(dataStartRow, 3).Value = entry.Representative == null ? "N/A" : entry.Representative;
                    worksheet.Cell(dataStartRow, 5).Value = entry.DrAmount;
                    worksheet.Cell(dataStartRow, 5).Value = ConvertToLong(entry.DrAmount);
                    worksheet.Cell(dataStartRow, 5).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(dataStartRow, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Cell(dataStartRow, 6).Value = entry.CrAmount;
                    worksheet.Cell(dataStartRow, 6).Value = ConvertToLong(entry.CrAmount);
                    worksheet.Cell(dataStartRow, 6).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(dataStartRow, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    runningBalance = entry.CurrentBalance;
                    worksheet.Cell(dataStartRow, 7).Value = runningBalance;
                    worksheet.Cell(dataStartRow, 7).Value = ConvertToLong(runningBalance);
                    worksheet.Cell(dataStartRow, 7).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(dataStartRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    worksheet.Range(dataStartRow, 1, dataStartRow, 7).Style


                         .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                          .Border.SetTopBorder(XLBorderStyleValues.Thin)
                          .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                           .Border.SetRightBorder(XLBorderStyleValues.Thin)
                           .Border.SetTopBorderColor(XLColor.Black)
                           .Border.SetLeftBorderColor(XLColor.Black)
                           .Border.SetRightBorderColor(XLColor.Black)
                         .Border.SetBottomBorderColor(XLColor.Black);
                    worksheet.Range(dataStartRow, 1, dataStartRow, 7).Style.Font.FontName = "Bahnschrift Light";
                    worksheet.Range(dataStartRow, 1, dataStartRow, 7).Style.Font.Bold = false;
                    //worksheet.Range(dataStartRow, 1, dataStartRow, 7).Style = borderStyle;
                    worksheet.Range(dataStartRow, 1, dataStartRow, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Range(dataStartRow, 1, dataStartRow, 7).Style.Border.SetOutsideBorderColor(XLColor.Blue);
                    dataStartRow++;
                }
            }
            // Summary Section
            //dataStartRow += 2;

            // Add "Summary" header
            worksheet.Cell(dataStartRow, 1).Value = "Summary";
            worksheet.Cell(dataStartRow, 1).Style.Font.FontName = "Bahnschrift Light";
            worksheet.Cell(dataStartRow, 1).Style.Font.SetBold();
            worksheet.Cell(dataStartRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(dataStartRow, 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Range(dataStartRow, 1, dataStartRow, 2).Merge();
            worksheet.Range(dataStartRow, 1, dataStartRow, 2).Style
                .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
                .Border.SetOutsideBorderColor(XLColor.Blue);

            dataStartRow += 1;

            // Add Total Debit
            worksheet.Cell(dataStartRow, 1).Value = "Total Debit:";
            worksheet.Cell(dataStartRow, 1).Style.Font.SetBold();
            worksheet.Cell(dataStartRow, 2).Value = ConvertToLong(ledger.AccountingEntries?.Sum(e => e.DrAmount) ?? 0);
            worksheet.Cell(dataStartRow, 2).Style.Font.Bold = false;
            worksheet.Cell(dataStartRow, 2).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(dataStartRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            // Add Total Credit
            dataStartRow += 1;
            worksheet.Cell(dataStartRow, 1).Value = "Total Credit:";
            worksheet.Cell(dataStartRow, 1).Style.Font.SetBold();
            worksheet.Cell(dataStartRow, 2).Value = ConvertToLong(ledger.AccountingEntries?.Sum(e => e.CrAmount) ?? 0);
            worksheet.Cell(dataStartRow, 2).Style.Font.Bold = false;
            worksheet.Cell(dataStartRow, 2).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(dataStartRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            // Add Account Balance
            dataStartRow += 1;
            worksheet.Cell(dataStartRow, 1).Value = "Account Balance:";
            worksheet.Cell(dataStartRow, 1).Style.Font.SetBold();
            worksheet.Cell(dataStartRow, 2).Value = ConvertToLong(runningBalance);
            worksheet.Cell(dataStartRow, 2).Style.Font.Bold = false;
            worksheet.Cell(dataStartRow, 2).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(dataStartRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            // Add Total Operation
            dataStartRow += 1;
            worksheet.Cell(dataStartRow, 1).Value = "Total Operation:";
            worksheet.Cell(dataStartRow, 1).Style.Font.SetBold();
            worksheet.Cell(dataStartRow, 2).Value = ConvertToLong(ledger.AccountingEntries?.Count ?? 0);
            worksheet.Cell(dataStartRow, 2).Style.Font.Bold = false;
            worksheet.Cell(dataStartRow, 2).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(dataStartRow, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            // Apply thick outer border for the entire summary section
            worksheet.Range(dataStartRow - 3, 1, dataStartRow, 2).Style
                .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
                .Border.SetOutsideBorderColor(XLColor.Blue);

            // Apply consistent font and alignment
            worksheet.Range(dataStartRow - 3, 1, dataStartRow, 2).Style.Font.FontName = "Bahnschrift Light";
            worksheet.Range(dataStartRow - 3, 1, dataStartRow, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            currentRow = dataStartRow + 4;
        }



        worksheet.Columns().AdjustToContents();
        worksheet.Columns().AdjustToContents(4, 25, 80);
        return worksheet;
    }

    private static void GeneratePeriodHeader(IXLWorksheet worksheet, ReportHeader model, string secondTitle, int HEADER_COLUMNS)
    {
        var Value = $"FROM:        {model.FromDate:dd-MM-yyyy}        TO:          {model.ToDate:dd-MM-yyyy}";

        // Create the merged range
        var periodTagRange = worksheet.Range(7, 1, 9, HEADER_COLUMNS).Merge()
            .Style.ApplyHeaderStyle()
         .Border.SetOutsideBorder(XLBorderStyleValues.Thick)
            .Border.SetOutsideBorderColor(XLColor.Blue);
        worksheet.Range(7, 1, 9, HEADER_COLUMNS).Merge().Style.Font.FontName = "Bahnschrift Light";

        worksheet.Range(7, 1, 9, HEADER_COLUMNS).Merge().Style.Font.Bold = false;
        // Set values and apply alignment
        worksheet.Cell(7, 1).Value = "PERIOD";
        worksheet.Cell(8, 1).Value = Value;
        worksheet.Cell(9, 1).Value = secondTitle;

        // Increase row height for row 9
        worksheet.Row(9).Height = 30; // You can adjust this value as needed

        // Add line breaks between values in the merged cell
        worksheet.Cell(7, 1).Value = $"PERIOD\n{Value}\n{secondTitle}";

        // Ensure text wrapping and vertical alignment is set
        worksheet.Cell(7, 1).Style
            .Alignment.SetWrapText(true)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
    }

    private static void GenerateReportMetadata(
     IXLWorksheet worksheet,
     string printedDate,
     string username,
     string reportTitle, int HEADER_COLUMNS)
    {
        // Report title - bold and centered
        var titleRange = worksheet.Range(5, 1, 5, HEADER_COLUMNS);
        titleRange.Merge().Style.ApplyHeaderStyle().Font.SetFontSize(24).Font.SetBold();
        worksheet.Cell(5, 1).Value = reportTitle;

        // Apply Thin blue border to the title cell
        //worksheet.Range(5, 1, 5, HEADER_COLUMNS).Style
        //    .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        //    //.Border.SetBottomBorder(XLBorderStyleValues.Thick)
        //    //.Border.SetLeftBorder(XLBorderStyleValues.Thick)
        //    //.Border.SetRightBorder(XLBorderStyleValues.Thick)
        //    .Border.SetOutsideBorderColor(XLColor.Blue);
         //.Border.SetTopBorderColor(XLColor.Blue)
         // .Border.SetLeftBorderColor(XLColor.Blue)
         //    .Border.SetRightBorderColor(XLColor.Blue);

        // Print details
        ApplyCenteredMergedStyle(worksheet, 6, $"Printed Date: {printedDate} by {username}", HEADER_COLUMNS);
        worksheet.Cell(6, HEADER_COLUMNS - 1).Value = username;
        worksheet.Cell(6, HEADER_COLUMNS - 1).Style.ApplyHeaderStyle();

    }

    private static void ApplyCenteredMergedStyle(IXLWorksheet worksheet, int row, string value, int HEADER_COLUMNS)
    {
        worksheet.Range(row, 1, row, HEADER_COLUMNS).Merge()
            .Style.ApplyHeaderStyle();
        worksheet.Cell(row, 1).Value = value;
        worksheet.Cell(row, 1).Style.Font.FontName = "Bahnschrift Light";

        worksheet.Cell(row, 1).Style.Font.Bold = false;
    }



    private static void AddSummaryRow(IXLWorksheet worksheet, int row, string label, object value)
    {
        for (int col = 1; col <= 2; col++)
        {
            var cell = worksheet.Cell(row, col);
            cell.Value = col == 1 ? label : value.ToString();
            cell.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                .Border.SetThinBorder();
        }
    }
    public static IXLWorksheet Generate4ColumnTrialBalanceReport(IXLWorksheet worksheet, List<TrialBalance4ColumnDto> accountModels, string filePath, string printedDate, string username, string reportTitle)
    {



        var headerStyle = worksheet.Style;
        headerStyle.Font.Bold = true;
        headerStyle.Font.FontSize = 11;
        headerStyle.Border.OutsideBorder = XLBorderStyleValues.Thick;
        headerStyle.Border.SetOutsideBorderColor(XLColor.Blue);

        var borderStyle = worksheet.Style;
        borderStyle.Border.OutsideBorder = XLBorderStyleValues.Thick;
        borderStyle.Border.SetBottomBorderColor(XLColor.Blue);
        borderStyle.Border.SetTopBorderColor(XLColor.Blue);
        borderStyle.Border.SetLeftBorderColor(XLColor.Blue);
        borderStyle.Border.SetRightBorderColor(XLColor.Blue);
        GenerateCompanyHeader(worksheet, accountModels[0].ConvertToHeaderReport(), HEADER_COLUMNS + 1);
        GenerateReportMetadata(worksheet, printedDate, username, reportTitle, HEADER_COLUMNS + 1);
        GeneratePeriodHeader(worksheet, accountModels[0].ConvertToHeaderReport(), "", HEADER_COLUMNS + 1);
        // Print account details

        // Print balance sheet header
        worksheet.Cell(10, 1).Value = "Account Number";
        worksheet.Cell(10, 2).Value = "Account Name";
        worksheet.Cell(10, 4).Value = "Begining Bal DR";
        worksheet.Cell(10, 3).Value = "BBD";
        worksheet.Cell(10, 5).Value = "Mov't Debit";
        worksheet.Cell(10, 6).Value = "Mov't Credit";
        worksheet.Cell(10, 7).Value = "Ending Balance DR";
        worksheet.Cell(10, 8).Value = "EBD";

        // Apply header style
        var headerRange0 = worksheet.Range(10, 1, 10, 8);
        headerRange0.Style.Font.FontSize = 14;
        headerRange0.Style.Font.Bold = true;
        headerRange0.Style.Font.FontName = "Bahnschrift Light";
        headerRange0.Style.Alignment.WrapText = false;
        headerRange0.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        headerRange0.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        headerRange0.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        headerRange0.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        headerRange0.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        headerRange0.Style.Border.SetOutsideBorderColor(XLColor.Blue);
        var headerRange10 = worksheet.Range(11, 1, accountModels.Count() + 11, 8);
        //headerRange10.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //headerRange10.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        //headerRange10.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //headerRange10.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        headerRange10.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        headerRange10.Style.Border.SetOutsideBorderColor(XLColor.Blue);
        headerRange10.Style.Font.FontSize = 11;
        headerRange10.Style.Font.Bold = false;
        headerRange10.Style.Font.FontName = "Bahnschrift Light";
        headerRange10.Style.Alignment.WrapText = true;
        int row = 11;
        foreach (var account in accountModels)
        {
            worksheet.Cell(row, 1).Value = account.AccountNumber;
            worksheet.Cell(row, 2).Value = account.AccountName;
            worksheet.Cell(row, 3).Value = account.BeginningBookingDirection;
            worksheet.Cell(row, 4).Value = ConvertToLong(account.BeginningBalance);
            worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";

            worksheet.Cell(row, 5).Value = ConvertToLong(account.DebitBalance);
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 6).Value = ConvertToLong(account.CreditBalance);
            worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 7).Value = ConvertToLong(account.EndingBalance);
            worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 8).Value = account.EndingBookingDirection;

            row++;
        }

        // Autofit columns
        worksheet.Columns().AdjustToContents();


        var footerRange = worksheet.Range(accountModels.Count() + 11, 2, accountModels.Count() + 11, 8);
        footerRange.Style.Font.FontSize = 12;
        footerRange.Style.Font.Bold = true;
        footerRange.Style.Alignment.WrapText = false;
        footerRange.Style.Alignment.JustifyLastLine = false;
        footerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //footerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        //footerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        //footerRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        //footerRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        footerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
        footerRange.Style.Border.SetOutsideBorderColor(XLColor.Blue);
        // Print totals
        worksheet.Cell(row, 2).Value = "Totals";
        worksheet.Cell(row, 3).Value = accountModels[0].totalBeginningBookingDirection.ToString();
        worksheet.Cell(row, 4).Value = ConvertToLong(accountModels[0].totalBeginningBalance.ToString());
        worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 5).Value = ConvertToLong(accountModels[0].totalDebitBalance.ToString());
        worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 6).Value = ConvertToLong(accountModels[0].totalCreditBalance.ToString());
        worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 7).Value = ConvertToLong(accountModels[0].totalEndingBalance.ToString());
        worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 8).Value = accountModels[0].totalEndingBookingDirection.ToString();




        return worksheet;


    }

    //public static IXLWorksheet Generate6ColumnTrialBalanceReport(IXLWorksheet worksheet, List<TrialBalance6ColumnDto> accountModels, string filePath, string printedDate, string username, string reportTitle)
    //{



    //    var headerStyle = worksheet.Style;
    //    headerStyle.Font.Bold = true;
    //    headerStyle.Font.FontSize = 11;
    //    headerStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
    //    headerStyle.Border.SetOutsideBorderColor(XLColor.Blue);

    //    var borderStyle = worksheet.Style;
    //    borderStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
    //    borderStyle.Border.SetBottomBorderColor(XLColor.Blue);
    //    borderStyle.Border.SetTopBorderColor(XLColor.Blue);
    //    borderStyle.Border.SetLeftBorderColor(XLColor.Blue);
    //    borderStyle.Border.SetRightBorderColor(XLColor.Blue);
    //    borderStyle.Border.SetBottomBorder( XLBorderStyleValues.Thin);
    //    borderStyle.Border.SetLeftBorder(XLBorderStyleValues.Thin);
    //    borderStyle.Border.SetRightBorder( XLBorderStyleValues.Thin);

    //    borderStyle.Font.Bold = false;
    //    borderStyle.Font.FontName = "Bahnschrift Light";
    //    GenerateCompanyHeader(worksheet, accountModels[0].ConvertToHeaderReport(), HEADER_COLUMNS + 1);
    //    GenerateReportMetadata(worksheet, printedDate, username, reportTitle, HEADER_COLUMNS + 1);
    //    GeneratePeriodHeader(worksheet, accountModels[0].ConvertToHeaderReport(), "", HEADER_COLUMNS + 1);
    //    // Print account details

    //    // Print balance sheet header
    //    worksheet.Cell(10, 1).Value = "Account Number";
    //    worksheet.Cell(10, 2).Value = "Account Name";
    //    worksheet.Cell(10, 3).Value = "Begining Bal DR";
    //    worksheet.Cell(10, 4).Value = "Begining Bal CR";
    //    worksheet.Cell(10, 5).Value = "Mov't Debit";
    //    worksheet.Cell(10, 6).Value = "Mov't Credit";
    //    worksheet.Cell(10, 7).Value = "Ending Balance DR";
    //    worksheet.Cell(10, 8).Value = "Ending Balance CR";

    //    // Apply header style
    //    var headerRange0 = worksheet.Range(10, 1, 10, 8);
    //    headerRange0.Style.Font.FontSize = 14;
    //    headerRange0.Style.Font.Bold = true;
    //    headerRange0.Style.Font.FontName = "Bahnschrift Light";
    //    headerRange0.Style.Alignment.WrapText = false;
    //    headerRange0.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
    //    headerRange0.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
    //    headerRange0.Style.Border.RightBorder = XLBorderStyleValues.Thick;
    //    headerRange0.Style.Border.TopBorder = XLBorderStyleValues.Thick;
    //    headerRange0.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
    //    headerRange0.Style.Border.SetOutsideBorderColor(XLColor.Blue);
    //    var headerRange10 = worksheet.Range(11, 1, accountModels.Count() + 11, 8);
    //    headerRange10.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
    //    headerRange10.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
    //    headerRange10.Style.Border.RightBorder = XLBorderStyleValues.Thin;
    //    headerRange10.Style.Border.TopBorder = XLBorderStyleValues.Thin;
    //    headerRange10.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
    //    headerRange10.Style.Border.SetOutsideBorderColor(XLColor.Blue);
    //    headerRange10.Style.Font.FontSize = 11;
    //    headerRange10.Style.Font.Bold = false;
    //    headerRange10.Style.Font.FontName = "Bahnschrift Light";
    //    headerRange10.Style.Alignment.WrapText = true;
    //    int row = 11;
    //    foreach (var account in accountModels)
    //    {
    //        worksheet.Cell(row, 1).Value = account.AccountNumber;
    //        worksheet.Cell(row, 2).Value = account.AccountName;
    //        worksheet.Cell(row, 3).Value = ConvertToLong(account.BeginningDebitBalance);
    //        worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
    //        worksheet.Cell(row, 4).Value = ConvertToLong(account.BeginningCreditBalance);
    //        worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
    //        worksheet.Cell(row, 5).Value = ConvertToLong(account.DebitBalance);
    //        worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
    //        worksheet.Cell(row, 6).Value = ConvertToLong(account.CreditBalance);
    //        worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
    //        worksheet.Cell(row, 7).Value = ConvertToLong(account.EndDebitBalance);
    //        worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
    //        worksheet.Cell(row, 8).Value = ConvertToLong(account.EndCreditBalance);
    //        worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0";

    //        row++;
    //    }

    //    // Autofit columns
    //    worksheet.Columns().AdjustToContents();


    //    var footerRange = worksheet.Range(accountModels.Count() + 11, 2, accountModels.Count() + 11, 8);
    //    footerRange.Style.Font.FontSize = 12;
    //    footerRange.Style.Font.Bold = true;
    //    footerRange.Style.Alignment.WrapText = false;
    //    footerRange.Style.Alignment.JustifyLastLine = false;
    //    footerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    //    footerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
    //    footerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
    //    footerRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
    //    footerRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
    //    footerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
    //    footerRange.Style.Border.SetOutsideBorderColor(XLColor.Blue);
    //    // Print totals
    //    worksheet.Cell(row, 2).Value = "Totals";
    //    worksheet.Cell(row, 3).Value = ConvertToLong(accountModels[0].TotalBeginningDebitBalance.ToString());
    //    worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
    //    worksheet.Cell(row, 4).Value = ConvertToLong(accountModels[0].TotalBeginningCreditBalance.ToString());
    //    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
    //    worksheet.Cell(row, 5).Value = ConvertToLong(accountModels[0].TotalDebitBalance.ToString());
    //    worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
    //    worksheet.Cell(row, 6).Value = ConvertToLong(accountModels[0].TotalCreditBalance.ToString());
    //    worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
    //    worksheet.Cell(row, 7).Value = ConvertToLong(accountModels[0].TotalEndDebitBalance.ToString());
    //    worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
    //    worksheet.Cell(row, 8).Value = ConvertToLong(accountModels[0].TotalEndCreditBalance.ToString());
    //    worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0";




    //    return worksheet;


    //}
    public static IXLWorksheet Generate6ColumnTrialBalanceReport(IXLWorksheet worksheet, List<TrialBalance6ColumnDto> accountModels, string filePath, string printedDate, string username, string reportTitle)
    {
        var headerStyle = worksheet.Style;
        headerStyle.Font.Bold = true;
        headerStyle.Font.FontSize = 11;
        headerStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerStyle.Border.SetOutsideBorderColor(XLColor.Blue);

        // Apply border style to all cells
        var borderStyle = worksheet.Style;
        borderStyle.Border.OutsideBorder = XLBorderStyleValues.Thin;
        borderStyle.Border.SetOutsideBorderColor(XLColor.Blue);

        // Apply specific thick border to rightmost cells
        var rightBorderStyle = worksheet.Style;
        rightBorderStyle.Border.RightBorder = XLBorderStyleValues.Thick;
        rightBorderStyle.Border.SetRightBorderColor(XLColor.Blue);

        // Set font and other styles
        borderStyle.Font.Bold = false;
        borderStyle.Font.FontName = "Bahnschrift Light";

        // Generate headers and metadata
        GenerateCompanyHeader(worksheet, accountModels[0].ConvertToHeaderReport(), HEADER_COLUMNS + 1);
        GenerateReportMetadata(worksheet, printedDate, username, reportTitle, HEADER_COLUMNS + 1);
        GeneratePeriodHeader(worksheet, accountModels[0].ConvertToHeaderReport(), "", HEADER_COLUMNS + 1);

        // Print account details headers
        worksheet.Cell(10, 1).Value = "Account Number";
        worksheet.Cell(10, 2).Value = "Account Name";
        worksheet.Cell(10, 3).Value = "Begining Bal DR";
        worksheet.Cell(10, 4).Value = "Begining Bal CR";
        worksheet.Cell(10, 5).Value = "Mov't Debit";
        worksheet.Cell(10, 6).Value = "Mov't Credit";
        worksheet.Cell(10, 7).Value = "Ending Balance DR";
        worksheet.Cell(10, 8).Value = "Ending Balance CR";

        // Apply header style to the header range
        var headerRange0 = worksheet.Range(10, 1, 10, 8);
        headerRange0.Style.Font.FontSize = 14;
        headerRange0.Style.Font.Bold = true;
        headerRange0.Style.Font.FontName = "Bahnschrift Light";
        headerRange0.Style.Alignment.WrapText = false;
        headerRange0.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        headerRange0.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
        headerRange0.Style.Border.RightBorder = XLBorderStyleValues.Thick;
        headerRange0.Style.Border.TopBorder = XLBorderStyleValues.Thick;
        headerRange0.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
        headerRange0.Style.Border.SetOutsideBorderColor(XLColor.Blue);

        // Print account details
        int row = 11;
        foreach (var account in accountModels)
        {
            worksheet.Cell(row, 1).Value = account.AccountNumber;
            worksheet.Cell(row, 2).Value = account.AccountName;
            worksheet.Cell(row, 3).Value = ConvertToLong(account.BeginningDebitBalance);
            worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 4).Value = ConvertToLong(account.BeginningCreditBalance);
            worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 5).Value = ConvertToLong(account.DebitBalance);
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 6).Value = ConvertToLong(account.CreditBalance);
            worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 7).Value = ConvertToLong(account.EndDebitBalance);
            worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
            worksheet.Cell(row, 8).Value = ConvertToLong(account.EndCreditBalance);
            worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0";

            // Apply thin borders to the detail range
            var detailRange = worksheet.Range(row, 1, row, 8);
            detailRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            detailRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            detailRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            detailRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            detailRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            detailRange.Style.Border.SetOutsideBorderColor(XLColor.Black);

            row++;
        }

        // Autofit columns
        worksheet.Columns().AdjustToContents();

        // Print totals in footer
        var footerRange = worksheet.Range(row, 2, row, 8);
        footerRange.Style.Font.FontSize = 12;
        footerRange.Style.Font.Bold = true;
        footerRange.Style.Alignment.WrapText = false;
        footerRange.Style.Alignment.JustifyLastLine = false;
        footerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        footerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        footerRange.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
        footerRange.Style.Border.RightBorder = XLBorderStyleValues.Thick;
        footerRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
        footerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thick);
        footerRange.Style.Border.SetOutsideBorderColor(XLColor.Blue);

        // Print totals labels and values
        worksheet.Cell(row, 2).Value = "Totals";
        worksheet.Cell(row, 3).Value = ConvertToLong(accountModels[0].TotalBeginningDebitBalance.ToString());
        worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 4).Value = ConvertToLong(accountModels[0].TotalBeginningCreditBalance.ToString());
        worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 5).Value = ConvertToLong(accountModels[0].TotalDebitBalance.ToString());
        worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 6).Value = ConvertToLong(accountModels[0].TotalCreditBalance.ToString());
        worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 7).Value = ConvertToLong(accountModels[0].TotalEndDebitBalance.ToString());
        worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
        worksheet.Cell(row, 8).Value = ConvertToLong(accountModels[0].TotalEndCreditBalance.ToString());
        worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0";

        return worksheet;
    }

    public static long ConvertToLong(object value)
    {
        try
        {
            if (value == null)
            {
                // Handle null value by returning 0 or a default value.
                return 0;
            }

            // If the value is already a numeric type, convert directly.
            if (value is int || value is long || value is short || value is byte)
            {
                return Convert.ToInt64(value);
            }

            if (value is decimal || value is double || value is float)
            {
                // Round the value before converting to avoid truncation errors.
                return Convert.ToInt64(Math.Round(Convert.ToDecimal(value)));
            }

            if (value is string)
            {
                // Remove potential formatting characters like commas or currency symbols.
                string cleanedValue = value.ToString().Replace(",", "").Replace("$", "").Trim();

                if (decimal.TryParse(cleanedValue, out decimal parsedDecimal))
                {
                    return Convert.ToInt64(Math.Round(parsedDecimal));
                }
                else
                {
                    throw new FormatException("The string value cannot be parsed as a numeric value.");
                }
            }

            // Attempt to convert any other object type if possible.
            if (value is IConvertible)
            {
                return Convert.ToInt64(value);
            }

            // If none of the above conditions are met, throw an exception.
            throw new InvalidCastException("The provided value is not convertible to a long.");
        }
        catch (OverflowException)
        {
            // Handle values that are out of range for Int64.
            throw new OverflowException("The value is too large or too small to be converted to a long.");
        }
        catch (FormatException ex)
        {
            // Handle invalid formats.
            throw new FormatException($"Invalid format: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Catch any other unexpected exceptions.
            throw new Exception($"An error occurred during conversion: {ex.Message}");
        }
    }

}
