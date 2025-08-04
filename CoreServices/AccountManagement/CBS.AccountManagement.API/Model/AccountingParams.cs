using CBS.AccountManagement.Data;
using ExcelDataReader;
using System.Data;
using System.Text;

namespace CBS.AccountManagement.API.Model
{
    public class AccountingFinancialParams
    {
        public FinancialReportConfigurations ReadingExcel(IFormFile formFile, DocumentRef data)
        {
            FinancialReportConfigurations configurations = new FinancialReportConfigurations();
            configurations.BalanceSheetAssets = new List<BalanceSheetAssetDto>();
            configurations.BalanceSheetLiabilities = new List<BalanceSheetAssetLiabilitiesDto>();
            configurations.Incomes = new List<IncomeDto>();
            configurations.Expenses = new List<ExpenseDto>();

            try
            {
                using (var stream = formFile.OpenReadStream())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Get list of worksheets
                        var worksheets = reader.AsDataSet().Tables;

                        foreach (DataTable worksheet in worksheets)
                        {
                            // Get worksheet name
                            string name = worksheet.TableName;

                            if (name == "Asset")
                            {
                                var result = reader.AsDataSet();
                                var table = result.Tables["Asset"];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    if (i > 1)
                                    {
                                        string col01 = i.ToString();
                                        string col0 = table.Rows[i][0].ToString();
                                        string col1 = table.Rows[i][1].ToString();
                                        string col2 = table.Rows[i][2].ToString();
                                        string col4 = table.Rows[i][4].ToString();

                                        configurations.BalanceSheetAssets.Add(new BalanceSheetAssetDto
                                        {
                                            DocumentId = data.BalanceSheetId,
                                            DocumentTypeId = data.AssetId,
                                            Reference = col0,

                                            HeadingFR = col1,
                                            HeadingEN = col1,
                                            ListGrossDr = col2,
                                            ListProvCr = col4,
                                        });
                                    }
                                }
                            }
                            else if (name == "Liability and Equity")
                            {
                                var result = reader.AsDataSet();
                                var table = result.Tables["Liability and Equity"];
                                for (int h = 0; h < table.Rows.Count; h++)
                                {
                                    if (h > 1)
                                    {
                                        string col01 = h.ToString();
                                        string col0 = table.Rows[h][0].ToString();
                                        string col1 = table.Rows[h][1].ToString();
                                        string col2 = table.Rows[h][2].ToString();
                                        var (acceptedList, ExceptionList) = ProcessInputString(col2);
                                        try
                                        {
                                            
                                            if (col0 == "RP28")
                                            {
                                            }
                                            configurations.BalanceSheetLiabilities.Add(new BalanceSheetAssetLiabilitiesDto
                                            {
                                                DocumentId = data.BalanceSheetId,
                                                DocumentTypeId = data.LiabilityId,
                                                Reference = col0,
                                                HeadingEn = col1,
                                                HeadingFr = col1,
                                                selectedList = acceptedList,
                                                selectedException = ExceptionList
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            throw;
                                        }
                                    }
                                }
                            }
                            else if (name == "Expense")
                            {
                                var result = reader.AsDataSet();
                                var table = result.Tables["Expense"];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    if (i > 1)
                                    {
                                        string col01 = i.ToString();
                                        string col0 = table.Rows[i][0].ToString();
                                        string col1 = table.Rows[i][1].ToString();
                                        string col2 = table.Rows[i][2].ToString();

                                        var (acceptedList, ExceptionList) = ProcessInputString(col2);
                                        configurations.Expenses.Add(new ExpenseDto
                                        {
                                            DocumentId = data.ProfitAndLossId,
                                            DocumentTypeId = data.ExpenseId,
                                            Reference = col0,

                                            HeadingFR = col1,
                                            HeadingEN = col1,
                                            AccountList = acceptedList,
                                            AccountExceptionList = ExceptionList
                                        });
                                    }
                                }
                            }
                            else if (name == "Income")
                            {
                                var result = reader.AsDataSet();
                                var table = result.Tables["Income"];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    if (i > 1)
                                    {
                                        string col01 = i.ToString();
                                        string col0 = table.Rows[i][0].ToString();
                                        string col1 = table.Rows[i][1].ToString();
                                        string col2 = table.Rows[i][2].ToString();
                                        var (acceptedList, ExceptionList) = ProcessInputString(col2);
                                        configurations.Incomes.Add(new IncomeDto
                                        {
                                            DocumentId = data.ProfitAndLossId,
                                            DocumentTypeId = data.IncomeId,
                                            Reference = col0,
                                            HeadingFR = col1,
                                            HeadingEN = col1,
                                            AccountList = acceptedList,
                                            AccountExceptionList = ExceptionList
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //RP15

                throw;
            }
            return configurations;
        }

        public AccountSerializer ProcessColumn(string col2)
        {
            AccountSerializer model = new AccountSerializer { selectedList = "", selectedExceptionList = "" };

            if (col2.Contains("sauf"))
            {
                string[] parts = col2.Split("/");
                model.selectedList = parts[0];
                model.selectedExceptionList = parts[1];
            }
            else
            {
                model.selectedList = col2;
            }

            return model;
        }

        public (string, string) ProcessInputString(string input)
        {
            // Split the input string by '/'
            try
            {
                var sections = input.Split('/');

                var includedNumbers = new List<string>();
                var excludedNumbers = new List<string>();

                foreach (var section in sections)
                {
                    var parts = section.Trim().Split("sauf");

                    // Add the number before 'sauf' to includedNumbers
                    includedNumbers.Add(parts[0].Trim());

                    // If there's a part after 'sauf', add those numbers to excludedNumbers
                    if (parts.Length > 1)
                    {
                        var excluded = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(n => n.Trim());
                        excludedNumbers.AddRange(excluded);
                    }
                }

                // Join the lists into strings
                string result1 = string.Join(",", includedNumbers);
                string result2 = string.Join(",", excludedNumbers);

                return (result1, result2);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public AccountSerializer ProcessIncomeColumn(string col2)
        {
            AccountSerializer model = new AccountSerializer { selectedList = "", selectedExceptionList = "" };

            if (col2.Contains("sauf"))
            {
                string[] parts = col2.Split("/");
                model.selectedList = (parts[0]);
                model.selectedExceptionList = (parts[1]);
            }
            else
            {
                model.selectedList = (col2);
            }

            return model;
        }
    }

    public class AccountSerializer
    {
        public string selectedList { get; set; }
        public string selectedExceptionList { get; set; } = null;
    }
}