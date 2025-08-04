using CBS.AccountManagement.Data;
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
    public class UploadAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public List<AccountModelX> AccountModelList { get; set; }

        public string BranchCode { get; set; }
        public string BranchId { get; set; }

        public List<AccountModelX> UploadAccountQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<AccountModelX>();

               
                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
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
                    string beginningDebit = table.Rows[i][2].ToString();
                    string beginningCredit = table.Rows[i][3].ToString();
                    string movementDebit = table.Rows[i][4].ToString();
                    string movementCredit = table.Rows[i][5].ToString();
                    string endDebit = table.Rows[i][6].ToString();
                    string endCredit = table.Rows[i][7].ToString();
                    string branchCode = table.Rows[i][8].ToString();

                    //string? col4 = table.Rows[i][3].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    if (accountNumber!= "AccountNumber")
                    {
                        ListAccountModelX.Add(new AccountModelX(accountNumber, accountName, accountNumber.Substring(0, 6), DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));

                    }
                }
 
               
                return ListAccountModelX;
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
    }

    public class UploadAccountResult
    {
        public int Account_Present { get; set; }
        public int Total_Account { get; set; }
        public string file_path { get; set; }

        public bool UploadStatus { get; set; }

        public string BranchName { get; set; }

    }

 




    public class UploadAccount : IRequest<ServiceResponse<UploadAccountResult>>
    {
        public List<AccountModelX> AccountModelList { get; set; }

        public string BranchId { get; set; }
        public bool IsHarmonizationActivated { get;   set; }

        public List<AccountModelX> UploadAccountQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<AccountModelX>();


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
                    string beginningDebit = table.Rows[i][2].ToString();
                    string beginningCredit = table.Rows[i][3].ToString();
                    string movementDebit = table.Rows[i][4].ToString();
                    string movementCredit = table.Rows[i][5].ToString();
                    string endDebit = table.Rows[i][6].ToString();
                    string endCredit = table.Rows[i][7].ToString();
                    string branchCode = table.Rows[i][8].ToString();

                    //string? col4 = table.Rows[i][3].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    //string? col5 = table.Rows[i][4].ToString();
                    ListAccountModelX.Add(new AccountModelX(accountNumber, accountName, accountNumber.Substring(0, 6), DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
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
    //Dictionary<string, List<AccountModelX>> models
    public class AccountModelX  
    {
        public string AccountNumberNetwork { get; set; } = "";

        public string AccountNumberCU { get; set; } = "";
        public string ChartofAccount { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }

        public string BookingDirection { get; set; }
        public decimal BeginningDebitBalance { get; set; }

        public decimal BeginningCreditBalance { get; set; }
        public decimal MovementDebitBalance { get; set; }
        public decimal MovementCreditBalance { get; set; }
        public decimal EndBalanceDebit { get; set; }
        public decimal EndBalanceCredit { get; set; }

        public string CreatedDate { get; set; }
        public AccountModelX()
        {
            
        }
        public AccountModelX(string AccNumber, string AccName, string chartofaccount, string DateCreated, decimal beginningBalanceDr , decimal beginningBalanceCr, decimal currentBalance, string branchCode, decimal movementCreditBalance, decimal movementDebitBalance, decimal endingBalanceCr, decimal endingBalanceDr)
        {

            AccountNumber = AccNumber;
            AccountName = AccName;
            BeginningDebitBalance = beginningBalanceDr;
            BeginningCreditBalance = beginningBalanceCr;
            CreatedDate = DateCreated;
            ChartofAccount = chartofaccount;
            //BranchCodeX = branchCode;
            BookingDirection = beginningBalanceCr == 0 ? "D" : "C";
            EndBalanceCredit = endingBalanceCr;
            EndBalanceDebit = endingBalanceDr;
            MovementDebitBalance = movementDebitBalance;
            MovementCreditBalance = movementCreditBalance;
        }

        public static AccountModelX ConvertToAccountModelX( AccountDto account)
        {
            return new AccountModelX
            {
                AccountName = account.AccountName,
                AccountNumber= account.AccountNumberCU
            };
        }

    }


    public class AccountChartX
    {
       

        public AccountChartX(string accountNumber, string? accountName)
        {
            this.AccountNumber = accountNumber;
            AccountName = accountName;
        }

        public string AccountNumber { get; set; }
        public string AccountName { get; set; }


    }
    public class ImportResult
    {
        public List<AccountModelX> ModelList1 { get; set; }
        public List<AccountModelX> ModelList2 { get; set; }
        public List<AccountModelX> ModelList3 { get; set; }
        public List<AccountModelX> ModelList4 { get; set; }
        public List<AccountModelX> ModelList5 { get; set; }
        public List<AccountModelX> ModelList6 { get; set; }
        public List<AccountModelX> ModelList7 { get; set; }
        public List<AccountModelX> ModelList8 { get; set; }
        public List<AccountModelX> ModelList9 { get; set; }
        public List<AccountModelX> ModelList10 { get; set; }
        public List<AccountModelX> ModelList11 { get; set; }
        public List<AccountModelX> ModelList12 { get; set; }
        public List<AccountModelX> ModelList13 { get; set; }
        public List<AccountModelX> ModelList14 { get; set; }


    }

}
