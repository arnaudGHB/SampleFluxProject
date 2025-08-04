using CBS.AccountManagement.Data;
using ExcelDataReader;
using System.Text;

namespace CBS.AccountManagement.API.Model
{
    public class OperationEventAttributesModel
    {
        public List<OperationEventAttributesDto> Upload(IFormFile file)
        {
            var ListOperationEvent = new List<OperationEventAttributesDto>();
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
                    string col1 = table.Rows[i][0].ToString();
                    string col2 = table.Rows[i][1].ToString();
                    string col3 = table.Rows[i][2].ToString();

                    ListOperationEvent.Add(new OperationEventAttributesDto { Id = col1, /*OperationEventCode = col2,*/Name = col3 });
                }
            }
            return ListOperationEvent;
        }
    }
}