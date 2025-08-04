using CBS.AccountManagement.Data;
using ExcelDataReader;
using System.Text;

namespace CBS.AccountManagement.API.Model
{
    public class AccountCartegoryModel
    {
        public List<AccountCartegoryDto> Upload(IFormFile file)
        {
            var ListOperationEvent = new List<AccountCartegoryDto>();
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
                    string? col2 = table.Rows[i][1].ToString();
                    string? col3 = table.Rows[i][2].ToString();

                    ListOperationEvent.Add(new AccountCartegoryDto { Id = col1, Name = col2, Description = col3 });
                }
            }
            return ListOperationEvent;
        }
    }
}