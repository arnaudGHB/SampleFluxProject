using CBS.AccountManagement.Helper;
using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UploadChartOfAccountManagementPositionCommand : IRequest<ServiceResponse<bool>>
    {
        public List<ChartOfAccountManagementPositionFile> ChartOfAccountManagementPositionFile { get; set; }

        public string BranchCode { get; set; }
        public string BranchId { get; set; }
        public List<ChartOfAccountManagementPositionFile> UploadAccountQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<ChartOfAccountManagementPositionFile>();


                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                   
                    string accountNumber = table.Rows[i][0].ToString();
                    if (accountNumber == "100000")
                    {

                    }
                    //(string part1, string part2) = GetNewAccountNumberFormat(accountNumber);
                    var newNumber = table.Rows[i][1].ToString();
                    string accountNumberNew = newNumber;// (part1 + part2).PadRight(12, '0');// GenerateNewAccountNumber(  accountNumber,   part1,   part2,   newNumber); //table.Rows[i][1].ToString();
                    string accountName = table.Rows[i][2].ToString();
                    string IsUnique = table.Rows[i][3].ToString();
                    if (accountNumberNew.Contains("740101001"))
                    {

                    }
     
                    ListAccountModelX.Add(new ChartOfAccountManagementPositionFile(accountNumber, accountNumberNew , accountName.ToUpper(),IsUnique));//, DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
                }

                return ListAccountModelX;
            }
            catch (Exception ex)
            {

                throw (ex);
            }

        }
        public string GenerateNewAccountNumber(string accountNumber, string part1, string part2, string newNumber)
{
    // Get the last character of the accountNumber and convert it to an integer
    int lastDigitOfAccountNumber = Convert.ToInt32(accountNumber.Last());

    // Define a variable to hold the final account number
    string accountNumberNew;

    // If the last digit is greater than 0
    if (lastDigitOfAccountNumber > 0)
    {
        // Extract a substring from newNumber starting at (newNumber.Length - 4) and take (newNumber.Length - 2) characters
        string substringFromNewNumber = newNumber.Substring(8);
        
        // Concatenate part1 and the extracted substring, then pad it with '0' on the right to make it 12 characters long
        accountNumberNew = (part1 + substringFromNewNumber).PadRight(12, '0');
    }
    else
    {
        // Concatenate part1 and part2, then pad it with '0' on the right to make it 12 characters long
        accountNumberNew = (part1 + part2).PadRight(12, '0');
    }

    // Return the generated account number
    return accountNumberNew;
}
        private (string part1, string part2) GetNewAccountNumberFormat(string accountNumber)
        {
            try
            {
                string part1 = "", part2 = "";
                part1 = accountNumber.Substring(0, 6);
                if (accountNumber.Length > 6)
                {
                    var idex = accountNumber.Substring(6);
                    var p2 = accountNumber.Substring(6);
                    var temp3 = p2.Substring(2);
                    if (Convert.ToInt32(temp3) > 1000)
                    {
                        part2 = RemoveLeadingZeros(temp3);
                        part2 = TrimZeros(part2);
                        part2 = ShiftLastDigitIfApplicable(part2);
                    }
                    else
                    {
                        part2 = (Convert.ToInt32(temp3)).ToString();
                        part2 = part2.PadRight(3, '0');
                    }
                }
                else
                {
                    part1 = part1.PadRight(6, '0');
                }

                return (part1, part2);
            }
            catch (Exception ex)
            {

                throw(ex);
            }
        }
        public string ShiftLastDigitIfApplicable(string input)
        {
            // Check if the input is at least 3 characters long
            if (input.Length < 3)
            {
                return input; // Return the input unchanged if it's less than 3 characters long
            }

            // Extract the last digit and the preceding two characters
            char lastDigit = input[input.Length - 1];
            char secondLastDigit = input[input.Length - 2];
            char thirdLastDigit = input[input.Length - 3];

            // Check if the last digit is non-zero and the two preceding digits are zeros
            if (lastDigit != '0' && secondLastDigit == '0' && thirdLastDigit == '0')
            {
                // Shift the last digit one step to the left
                string shiftedNumber = input.Substring(0, input.Length - 3)
                                       + lastDigit
                                       + "00"; // Place two zeros after the shifted digit
                return shiftedNumber;
            }

            // Return the input unchanged if the condition is not met
            return input;
        }

        public static string TrimZeros(string number)
        {
            // Remove leading zeros
            number = number.TrimStart('0');

            // If the number is empty after removing leading zeros, it was all zeros
            if (string.IsNullOrEmpty(number))
            {
                return "0";
            }

            // Check if there's a decimal point
            int decimalIndex = number.IndexOf('.');
            if (decimalIndex != -1)
            {
                // Split the number into integer and fractional parts
                string integerPart = number.Substring(0, decimalIndex);
                string fractionalPart = number.Substring(decimalIndex + 1);

                // Remove the last zero from the fractional part if it exists
                if (fractionalPart.EndsWith("0"))
                {
                    fractionalPart = fractionalPart.Substring(0, fractionalPart.Length - 1);
                }

                // Reconstruct the number, omitting the decimal point if fractional part is empty
                number = fractionalPart.Length > 0 ? $"{integerPart}.{fractionalPart}" : integerPart;
            }
            else
            {
                // For integers, remove the last zero if it exists and it's not the only digit
                if (number.EndsWith("0") && number.Length > 1)
                {
                    number = number.Substring(0, number.Length - 1);
                }
            }

            return number;
        }
        public static string RemoveLeadingZeros(string number)
        {
            // Check if the input is null or empty
            if (string.IsNullOrEmpty(number))
            {
                return number;
            }

            // Handle negative numbers
            bool isNegative = number.StartsWith("-");
            if (isNegative)
            {
                number = number.Substring(1);
            }

            // Split the number into integer and decimal parts
            string[] parts = number.Split('.');

            // Remove leading zeros from the integer part
            parts[0] = parts[0].TrimStart('0');

            // If integer part becomes empty, it was all zeros, so we keep one
            if (string.IsNullOrEmpty(parts[0]))
            {
                parts[0] = "0";
            }

            // Reconstruct the number
            string result = string.Join(".", parts);

            // Add the negative sign back if it was negative
            if (isNegative)
            {
                result = "-" + result;
            }

            return result;
        }
        public List<AccountChartX> UploadAccountChartQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<AccountChartX>();


                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    //string accountNumber = row["Account Number"].ToString();
                    //string accountName = row["Account name"].ToString();
                    //string beginningDebit = row["BEG.DEBIT"].ToString();
                    //string beginningCredit = row["BEG.CREDIT"].ToString();
                    //string movementDebit = row["MOV.DEBIT"].ToString();
                    //string movementCredit = row["MOV.CREDIT"].ToString();
                    //string endDebit = row["End.DEBIT"].ToString();
                    //string endCredit = row["End.CREDIT"].ToString();
                    //string branchCode = Rows["BranchCodeX"].ToString();
                    string accountNumber = table.Rows[i][0].ToString();
                    string accountName = table.Rows[i][1].ToString();
                    //string beginningDebit = table.Rows[i][2].ToString();
                    //string beginningCredit = table.Rows[i][3].ToString();
                    //string movementDebit = table.Rows[i][4].ToString();
                    //string movementCredit = table.Rows[i][5].ToString();
                    //string endDebit = table.Rows[i][6].ToString();
                    //string endCredit = table.Rows[i][7].ToString();
                    //string branchCode = table.Rows[i][8].ToString();

                    //string? col4 = table.Rows[i][3].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    ListAccountModelX.Add(new AccountChartX(accountNumber.Substring(0, 6), accountName));//, accountNumber.Substring(0, 6), DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
                }

                return ListAccountModelX;
            }
            catch (Exception ex)
            {

                throw (ex);
            }

        }
    }

    public class UploadLoanAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public List<ChartOfAccountFile> ChartOfAccountFiles { get; set; }

 
        public List<ChartOfAccountFile> UploadAccountQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<ChartOfAccountFile>();


                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    //string accountNumber = row["Account Number"].ToString();
                    //string accountName = row["Account name"].ToString();
                    //string beginningDebit = row["BEG.DEBIT"].ToString();
                    //string beginningCredit = row["BEG.CREDIT"].ToString();
                    //string movementDebit = row["MOV.DEBIT"].ToString();
                    //string movementCredit = row["MOV.CREDIT"].ToString();
                    //string endDebit = row["End.DEBIT"].ToString();
                    //string endCredit = row["End.CREDIT"].ToString();
                    //string branchCode = Rows["BranchCodeX"].ToString();
                    string accountNumber = table.Rows[i][0].ToString();

                    string accountName = table.Rows[i][1].ToString();

                    //string? col4 = table.Rows[i][3].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    ListAccountModelX.Add(new ChartOfAccountFile(accountNumber,  accountName));//, DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
                }

                return ListAccountModelX;
            }
            catch (Exception ex)
            {

                throw (ex);
            }

        }

        public List<AccountChartX> UploadAccountChartQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<AccountChartX>();


                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    //string accountNumber = row["Account Number"].ToString();
                    //string accountName = row["Account name"].ToString();
                    //string beginningDebit = row["BEG.DEBIT"].ToString();
                    //string beginningCredit = row["BEG.CREDIT"].ToString();
                    //string movementDebit = row["MOV.DEBIT"].ToString();
                    //string movementCredit = row["MOV.CREDIT"].ToString();
                    //string endDebit = row["End.DEBIT"].ToString();
                    //string endCredit = row["End.CREDIT"].ToString();
                    //string branchCode = Rows["BranchCodeX"].ToString();
                    string accountNumber = table.Rows[i][0].ToString();
                    string accountName = table.Rows[i][1].ToString();
                    //string beginningDebit = table.Rows[i][2].ToString();
                    //string beginningCredit = table.Rows[i][3].ToString();
                    //string movementDebit = table.Rows[i][4].ToString();
                    //string movementCredit = table.Rows[i][5].ToString();
                    //string endDebit = table.Rows[i][6].ToString();
                    //string endCredit = table.Rows[i][7].ToString();
                    //string branchCode = table.Rows[i][8].ToString();

                    //string? col4 = table.Rows[i][3].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    ListAccountModelX.Add(new AccountChartX(accountNumber.Substring(0, 6), accountName));//, accountNumber.Substring(0, 6), DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
                }

                return ListAccountModelX;
            }
            catch (Exception ex)
            {

                throw (ex);
            }

        }
    }


    public class UploadChartOfAccountMFICommand : IRequest<ServiceResponse<bool>>
    {
        public List<ChartOfAccountManagementPositionFile2> ChartOfAccountManagementPositionFile { get; set; }

        public string BranchCode { get; set; }
        public string BranchId { get; set; }
        public List<ChartOfAccountManagementPositionFile2> UploadAccountQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<ChartOfAccountManagementPositionFile2>();


                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 1; i < table.Rows.Count; i++)
                {

                    string accountNumber = table.Rows[i][0].ToString();
                    if (accountNumber == "100000")
                    {

                    }
                    //(string part1, string part2) = GetNewAccountNumberFormat(accountNumber);
                    //var newNumber = table.Rows[i][1].ToString();
                    //string accountNumberNew = newNumber;// (part1 + part2).PadRight(12, '0');// GenerateNewAccountNumber(  accountNumber,   part1,   part2,   newNumber); //table.Rows[i][1].ToString();
                    string accountName = table.Rows[i][1].ToString();
                    //string IsUnique = table.Rows[i][3].ToString();
                   

                    ListAccountModelX.Add(new ChartOfAccountManagementPositionFile2(accountNumber,  accountName.ToUpper()));//, DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
                }

                return ListAccountModelX;
            }
            catch (Exception ex)
            {

                throw (ex);
            }

        }
        public string GenerateNewAccountNumber(string accountNumber, string part1, string part2, string newNumber)
        {
            // Get the last character of the accountNumber and convert it to an integer
            int lastDigitOfAccountNumber = Convert.ToInt32(accountNumber.Last());

            // Define a variable to hold the final account number
            string accountNumberNew;

            // If the last digit is greater than 0
            if (lastDigitOfAccountNumber > 0)
            {
                // Extract a substring from newNumber starting at (newNumber.Length - 4) and take (newNumber.Length - 2) characters
                string substringFromNewNumber = newNumber.Substring(8);

                // Concatenate part1 and the extracted substring, then pad it with '0' on the right to make it 12 characters long
                accountNumberNew = (part1 + substringFromNewNumber).PadRight(12, '0');
            }
            else
            {
                // Concatenate part1 and part2, then pad it with '0' on the right to make it 12 characters long
                accountNumberNew = (part1 + part2).PadRight(12, '0');
            }

            // Return the generated account number
            return accountNumberNew;
        }
        private (string part1, string part2) GetNewAccountNumberFormat(string accountNumber)
        {
            try
            {
                string part1 = "", part2 = "";
                part1 = accountNumber.Substring(0, 6);
                if (accountNumber.Length > 6)
                {
                    var idex = accountNumber.Substring(6);
                    var p2 = accountNumber.Substring(6);
                    var temp3 = p2.Substring(2);
                    if (Convert.ToInt32(temp3) > 1000)
                    {
                        part2 = RemoveLeadingZeros(temp3);
                        part2 = TrimZeros(part2);
                        part2 = ShiftLastDigitIfApplicable(part2);
                    }
                    else
                    {
                        part2 = (Convert.ToInt32(temp3)).ToString();
                        part2 = part2.PadRight(3, '0');
                    }
                }
                else
                {
                    part1 = part1.PadRight(6, '0');
                }

                return (part1, part2);
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        public string ShiftLastDigitIfApplicable(string input)
        {
            // Check if the input is at least 3 characters long
            if (input.Length < 3)
            {
                return input; // Return the input unchanged if it's less than 3 characters long
            }

            // Extract the last digit and the preceding two characters
            char lastDigit = input[input.Length - 1];
            char secondLastDigit = input[input.Length - 2];
            char thirdLastDigit = input[input.Length - 3];

            // Check if the last digit is non-zero and the two preceding digits are zeros
            if (lastDigit != '0' && secondLastDigit == '0' && thirdLastDigit == '0')
            {
                // Shift the last digit one step to the left
                string shiftedNumber = input.Substring(0, input.Length - 3)
                                       + lastDigit
                                       + "00"; // Place two zeros after the shifted digit
                return shiftedNumber;
            }

            // Return the input unchanged if the condition is not met
            return input;
        }

        public static string TrimZeros(string number)
        {
            // Remove leading zeros
            number = number.TrimStart('0');

            // If the number is empty after removing leading zeros, it was all zeros
            if (string.IsNullOrEmpty(number))
            {
                return "0";
            }

            // Check if there's a decimal point
            int decimalIndex = number.IndexOf('.');
            if (decimalIndex != -1)
            {
                // Split the number into integer and fractional parts
                string integerPart = number.Substring(0, decimalIndex);
                string fractionalPart = number.Substring(decimalIndex + 1);

                // Remove the last zero from the fractional part if it exists
                if (fractionalPart.EndsWith("0"))
                {
                    fractionalPart = fractionalPart.Substring(0, fractionalPart.Length - 1);
                }

                // Reconstruct the number, omitting the decimal point if fractional part is empty
                number = fractionalPart.Length > 0 ? $"{integerPart}.{fractionalPart}" : integerPart;
            }
            else
            {
                // For integers, remove the last zero if it exists and it's not the only digit
                if (number.EndsWith("0") && number.Length > 1)
                {
                    number = number.Substring(0, number.Length - 1);
                }
            }

            return number;
        }
        public static string RemoveLeadingZeros(string number)
        {
            // Check if the input is null or empty
            if (string.IsNullOrEmpty(number))
            {
                return number;
            }

            // Handle negative numbers
            bool isNegative = number.StartsWith("-");
            if (isNegative)
            {
                number = number.Substring(1);
            }

            // Split the number into integer and decimal parts
            string[] parts = number.Split('.');

            // Remove leading zeros from the integer part
            parts[0] = parts[0].TrimStart('0');

            // If integer part becomes empty, it was all zeros, so we keep one
            if (string.IsNullOrEmpty(parts[0]))
            {
                parts[0] = "0";
            }

            // Reconstruct the number
            string result = string.Join(".", parts);

            // Add the negative sign back if it was negative
            if (isNegative)
            {
                result = "-" + result;
            }

            return result;
        }
        public List<AccountChartX> UploadChartAccountQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<AccountChartX>();


                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 1; i < table.Rows.Count; i++)
                {
               
                    string accountNumber = table.Rows[i][0].ToString();
                    string accountName = table.Rows[i][1].ToString();
     
                    ListAccountModelX.Add(new AccountChartX(accountNumber.Substring(0, 6), accountName));//, accountNumber.Substring(0, 6), DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
                }

                return ListAccountModelX;
            }
            catch (Exception ex)
            {

                throw (ex);
            }

        }
    }
}
