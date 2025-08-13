using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper
{
    public static class FileProcessor
    {

        /// <summary>
        /// Validates the structure and data types of the uploaded text file.
        /// </summary>
        /// <param name="file">Uploaded file (Text file)</param>
        /// <param name="logger">Logger instance for logging</param>
        public static void ValidateFileAsyncSalaryModelTextFile(IFormFile file, ILogger logger)
        {
            string tempFilePath = null;

            try
            {
                if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Unsupported file format. Only .txt is allowed.");
                }

                // Save the file temporarily
                tempFilePath = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Validate the text file
                foreach (var line in File.ReadLines(tempFilePath))
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

                    if (line.Substring(line.Length - 9).Trim().Length != 9)
                    {
                        throw new InvalidOperationException("Invalid NetSalary length in file.");
                    }
                }

                logger.LogInformation("File validation successful for text file: {FileName}.", file.FileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during text file validation: {FileName}", file.FileName);
                throw new InvalidOperationException($"The uploaded text file is invalid. Please verify its structure and data types. Error: {ex.Message}");
            }
            finally
            {
                if (tempFilePath != null && File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }


        /// <summary>
        /// Converts a CSV file to Excel format.
        /// </summary>
        /// <param name="csvFile">Uploaded CSV file</param>
        /// <param name="logger">Logger instance for logging</param>
        /// <returns>Path to the converted Excel file</returns>
        public static string ConvertCsvToExcel(IFormFile csvFile, ILogger logger)
        {
            var tempExcelFilePath = Path.Combine(Path.GetTempPath(), $"{BaseUtilities.UtcNowToDoualaTime().ToString("yyyyMMddhhmmss")}_{csvFile.FileName}.xlsx");

            try
            {
                using (var reader = new StreamReader(csvFile.OpenReadStream()))
                {
                    var dataTable = new DataTable();
                    var headers = reader.ReadLine()?.Split(',');

                    if (headers == null)
                    {
                        throw new InvalidOperationException("CSV file is empty or invalid.");
                    }

                    foreach (var header in headers)
                    {
                        dataTable.Columns.Add(header.Trim());
                    }

                    while (!reader.EndOfStream)
                    {
                        var row = reader.ReadLine()?.Split(',');
                        if (row != null)
                        {
                            dataTable.Rows.Add(row);
                        }
                    }

                    using (var workbook = new XLWorkbook())
                    {
                        workbook.Worksheets.Add(dataTable, "SalaryData");
                        workbook.SaveAs(tempExcelFilePath);
                    }
                }

                logger.LogInformation("CSV file successfully converted to Excel.");
                return tempExcelFilePath;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error converting CSV to Excel.");
                throw;
            }
        }
        public static string ConvertCsvToExcel(string csvFilePath, string outputExcelFilePath)
        {
            var dataTable = new DataTable();
            using (var reader = new StreamReader(csvFilePath))
            {
                var headers = reader.ReadLine()?.Split(',');
                if (headers == null) throw new Exception("Invalid CSV File");

                foreach (var header in headers)
                    dataTable.Columns.Add(header.Trim());

                while (!reader.EndOfStream)
                {
                    var rows = reader.ReadLine()?.Split(',');
                    if (rows != null)
                        dataTable.Rows.Add(rows);
                }
            }

            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable, "SalaryData");
                workbook.SaveAs(outputExcelFilePath);
            }

            return outputExcelFilePath;
        }
        /// <summary>
        /// Saves an uploaded Excel file locally for validation.
        /// </summary>
        /// <param name="file">Uploaded Excel file</param>
        /// <param name="logger">Logger instance for logging</param>
        /// <returns>Path to the saved Excel file</returns>
        public static string SaveExcelFile(IFormFile file, ILogger logger)
        {
            var tempExcelFilePath = Path.Combine(Path.GetTempPath(), $"{BaseUtilities.UtcNowToDoualaTime().ToString("yyyyMMddhhmmss")}_{file.FileName}");

            try
            {
                using (var stream = new FileStream(tempExcelFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                logger.LogInformation("Excel file successfully saved.");
                return tempExcelFilePath;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving Excel file.");
                throw;
            }
        }

        private static string[][] ExpectedListHeaders()
        {
            return new []{
                  new []{"Salary Code", "Matricule", "Member Reference",  "Member Name", "Cross Salary" },
                 new[]{ "Salary Code", "Matricule", "Names",  "Account Number", "Cross Salary"}
            };
        }

        /// <summary>
        /// Validates the headers of the Excel file to ensure they match the expected structure.
        /// Logs errors and throws an exception if headers are incorrect.
        /// </summary>
        /// <param name="worksheet">Excel worksheet to validate.</param>
        /// <param name="logger">Logger instance for logging validation status.</param>
        /// <param name="fileName">Name of the file being processed (for better logging).</param>
        /// <exception cref="InvalidOperationException">Thrown if headers do not match the expected structure.</exception>
        public static async Task<int> ValidateHeadersAsync(IXLWorksheet worksheet, ILogger logger, string fileName)
        {
            var expectedListHeaders = ExpectedListHeaders();

            bool found = false;
            int headerIndex = 0;

            foreach (var expectedHeaders in expectedListHeaders)
            {
                headerIndex++;
                for (int i = 1; i <= expectedHeaders.Length; i++)
                {
                    var cellValue = worksheet.Cell(1, i).GetString().Trim(); // Trim to remove extra spaces

                    if (!string.Equals(cellValue, expectedHeaders[i - 1], StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    else if (i == expectedHeaders.Length && string.Equals(cellValue, expectedHeaders[i - 1], StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                    }
                }

                if (found) break;

            }

            if (!found)
            {
                string headerError = $"[ERROR] Invalid header in column for file '{fileName}.";
                logger.LogError(headerError);
                await BaseUtilities.LogAndAuditAsync(headerError, fileName, HttpStatusCodeEnum.BadRequest, LogAction.SalaryFileProcessing, LogLevelInfo.Error);

                throw new InvalidOperationException(headerError);
            }


            string validationSuccessMessage = $"[SUCCESS] Excel file '{fileName}' headers are valid.";
            logger.LogInformation(validationSuccessMessage);
            await BaseUtilities.LogAndAuditAsync(validationSuccessMessage, fileName, HttpStatusCodeEnum.OK, LogAction.SalaryFileProcessing, LogLevelInfo.Information);

            return headerIndex-1;
        }


        /// <summary>
        /// Validates the data types of the Excel file.
        /// </summary>
        /// <param name="worksheet">Excel worksheet</param>
        /// <param name="logger">Logger instance for logging</param>
        private static void ValidateDataTypes(IXLWorksheet worksheet, ILogger logger)
        {
            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var rowNumber = row.RowNumber();

                // Validate Salary Code (Column 1)
                if (string.IsNullOrWhiteSpace(row.Cell(1).GetString()))
                {
                    logger.LogError("Invalid data in row {Row}, column 1 (Salary Code): Value is required.", rowNumber);
                    throw new InvalidOperationException($"Invalid data in row {rowNumber}, column 1 (Salary Code): Value is required.");
                }

                // Validate Matricules (Column 2)
                if (string.IsNullOrWhiteSpace(row.Cell(2).GetString()))
                {
                    logger.LogError("Invalid data in row {Row}, column 2 (Matricules): Value is required.", rowNumber);
                    throw new InvalidOperationException($"Invalid data in row {rowNumber}, column 2 (Matricules): Value is required.");
                }

                // Validate NetSalary (Column 6)
                if (!decimal.TryParse(row.Cell(6).GetString(), out var netSalary) || netSalary < 0)
                {
                    logger.LogError("Invalid data in row {Row}, column 6 (NetSalary): Expected a positive decimal value.", rowNumber);
                    throw new InvalidOperationException($"Invalid data in row {rowNumber}, column 6 (NetSalary): Expected a positive decimal value.");
                }
            }

            logger.LogInformation("Excel file data types are valid.");
        }
    }
}
