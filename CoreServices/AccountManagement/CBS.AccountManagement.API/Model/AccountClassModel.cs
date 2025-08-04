using CBS.AccountManagement.Data;
using ExcelDataReader;
using System.Text;

namespace CBS.AccountManagement.API.Model
{
    public class AccountClassModel
    {
        public List<AccountClassDto> Upload(IFormFile file)
        {
            var cartegories = new List<AccountClassDto>();
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
                    string col1 = i.ToString();
                    string col2 = table.Rows[i][1].ToString();
                    string col3 = table.Rows[i][2].ToString();
                    string col4 = table.Rows[i][3].ToString();
                    string col5 = table.Rows[i][4].ToString();
                    cartegories.Add(new AccountClassDto { Id = col1, AccountNumber = col4, AccountCategoryId = col5 });
                }
            }
            return cartegories;
        }
    }
}