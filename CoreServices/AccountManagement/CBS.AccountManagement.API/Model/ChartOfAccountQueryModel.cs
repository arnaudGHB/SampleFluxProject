using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using ExcelDataReader;
using System.Text;

namespace CBS.AccountManagement.API.Model
{
    public class ChartOfAccountQueryModel
    {
        private const string BankCode = "012";

        public List<Data.ChartOfAccountDto> Upload(IFormFile file, bool language)
        {
            try
            {
                var ListOperationEvent = new List<ChartOfAccountDto>();

                var ListCharts = new List<ChartOfAccountDto>();
                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (i > 0)
                    {
                        if (language)
                        {
                            string col1 = i.ToString();
                            string? col2 = table.Rows[i][0].ToString();
                            string? col3 = table.Rows[i][1].ToString();
                            string? CamCCUL = col2.PadRight(6, '0');
                            string? AccountNumberCUBO = (CamCCUL + BankCode).PadRight(12, '0');

                            ListOperationEvent.Add(new ChartOfAccountDto
                            {
                                Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "CA"),
                                AccountNumber = col2,
                                AccountNumberNetwork = CamCCUL,
                                AccountNumberCU = AccountNumberCUBO,
                                LabelEn = col3,
                                LabelFr = col3,
                                ParentAccountNumber = ((col2.Length - 1) == 0) ? "ROOT" : col2.Substring(0, col2.Length - 1),
                                IsBalanceAccount = CheckIsBalanceAccount(col2),
                                ParentAccountId = ""
                            });

                            #region MyRegion

                            //}
                            //else
                            //{
                            //    string col1 = i.ToString();
                            //    string? col2 = table.Rows[i][0].ToString();
                            //    string? col3 = table.Rows[i][1].ToString();
                            //    //string? col4 = table.Rows[i][2].ToString();
                            //    string? CamCCUL = col2.PadRight(6, '0');
                            //    string? AccountNumberCUBO = (CamCCUL+BankCode).PadRight(12,'0');
                            //    //string? col5 = table.Rows[i][4].ToString();
                            //    ListOperationEvent.Add(new ChartOfAccountDto
                            //    {
                            //        Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "CA"),
                            //        AccountNumber = col2,
                            //        AccountNumberCamCCUL= CamCCUL,
                            //        AccountNumberCUBO= AccountNumberCUBO,
                            //        LabelFr = col3,
                            //        ParentAccountNumber = ((col2.Length - 1) <= 0) ? "ROOT" : col2.Substring(0, col2.Length - 1),
                            //        IsBalanceAccount = CheckIsBalanceAccount(col2),
                            //        ParentAccountId = ""
                            //    });

                            #endregion MyRegion
                        }
                    }
                }
                foreach (var item in ListOperationEvent)
                {
                    var model = ListOperationEvent.Find(x => x.AccountNumber == item.ParentAccountNumber);
                    if (model != null)
                    {
                        item.ParentAccountId = model.Id;
                    }
                    ListCharts.Add(item);
                }

                return ListCharts;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public List<Data.ChartOfAccountDto> UploadChartOfAccountQueryModel(IFormFile file, bool language)
        {
            try
            {
                var ListOperationEvent = new List<ChartOfAccountDto>();

                var ListCharts = new List<ChartOfAccountDto>();
                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (i > 0)
                    {
                        if (language)
                        {
                            string col1 = i.ToString();
                            string? col2 = table.Rows[i][0].ToString();
                            string? col3 = table.Rows[i][1].ToString();

                            if ((col2.Length - 1) == 0)
                            {
                            }

                            ListOperationEvent.Add(new ChartOfAccountDto
                            {
                                Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "CA"),
                                AccountNumber = col2,
                                LabelEn = col3,
                                ParentAccountNumber = ((col2.Length - 1) == 0) ? "ROOT" : col2.Substring(0, col2.Length - 1),
                                IsBalanceAccount = CheckIsBalanceAccount(col2),
                                ParentAccountId = ""
                            });
                        }
                        else
                        {
                            string col1 = i.ToString();
                            string? col2 = table.Rows[i][0].ToString();
                            string? col3 = table.Rows[i][1].ToString();
                            //string? col4 = table.Rows[i][2].ToString();
                            //string? col4 = table.Rows[i][3].ToString();
                            //string? col5 = table.Rows[i][4].ToString();
                            ListOperationEvent.Add(new ChartOfAccountDto
                            {
                                Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "CA"),
                                AccountNumber = col2,
                                LabelEn = col3,
                                LabelFr = col3,
                                ParentAccountNumber = ((col2.Length - 1) == 0) ? "ROOT" : col2.Substring(0, col2.Length - 1),
                                //IsBalanceAccount = CheckIsBalanceAccount(col2),
                                //ParentAccountId = ""
                            });
                        }
                    }
                }
                foreach (var item in ListOperationEvent)
                {
                    var model = ListOperationEvent.Find(x => x.AccountNumber == item.ParentAccountNumber);
                    if (model != null)
                    {
                        item.ParentAccountId = model.Id;
                    }
                    ListCharts.Add(item);
                }

                return ListCharts;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private bool CheckIsBalanceAccount(string? col3)
        {
            if (col3.StartsWith("1") || col3.StartsWith("2") || col3.StartsWith("3") || col3.StartsWith("4") ||
                col3.StartsWith("5") || col3.StartsWith("6"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static List<ChartOfAccountDto> RemoveDuplicates(List<ChartOfAccountDto> accountCategories)
        {
            // Use LINQ to remove duplicates based on Id
            return accountCategories.GroupBy(account => account.AccountNumber).Select(group => group.First()).ToList();
        }
    }

    internal class ChartOfAccountDtoComparer : IEqualityComparer<ChartOfAccountDto>
    {
        public bool Equals(ChartOfAccountDto x, ChartOfAccountDto y)
        {
            return x.AccountNumber == y.AccountNumber;
        }

        public int GetHashCode(ChartOfAccountDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}