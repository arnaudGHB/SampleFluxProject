using CBS.AccountManagement.Data;
using ExcelDataReader;
using Newtonsoft.Json;
using System.Text;

namespace CBS.AccountManagement.API.Model
{
    public class OperationEventModel
    {
        public List<OperationEventDto> Upload(IFormFile file)
        {
            var ListOperationEvent = new List<OperationEventDto>();
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
                    //string col4 = table.Rows[i][3].ToString();
                    //string col5 = table.Rows[i][4].ToString();
                    ListOperationEvent.Add(new OperationEventDto { Id = Guid.NewGuid().ToString(), OperationEventName = col2.Trim(), Description = col3 });
                }
            }
            // Save data to a JSON file
            SaveToJsonFile("OperationEvent.json", JsonConvert.SerializeObject(ListOperationEvent));
            return ListOperationEvent;
        }

        private static void SaveToJsonFile(string filePath, string json)
        {
            // Serialize the data to JSON

            // Write the JSON to a file
            File.WriteAllText(filePath, json);
        }
    }
}