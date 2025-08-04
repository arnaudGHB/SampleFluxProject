using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Queries;
using ExcelDataReader;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    public class UploadTownsCommand : IRequest<ServiceResponse<bool>>
    {
        public List<TownFile> TownFile { get; set; }

 
        public UploadTownsCommand()
        {
        }

      

        public List<TownFile> UploadTownQueryModel(IFormFile file)
        {
            try
            {
                var ListAccountModelX = new List<TownFile>();


                // Register encoding provider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using var stream = file.OpenReadStream();
                // Create ExcelReader instead of IExcelDataReader
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();
                var table = result.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {

                    string Id = table.Rows[i][0].ToString();
                     string Name = table.Rows[i][1].ToString();
                    string SubdivisonId = table.Rows[i][1].ToString();
                    string DivisionId = table.Rows[i][1].ToString();
                     

                    ListAccountModelX.Add(new TownFile(Id,Name, SubdivisonId,DivisionId));//, DateTime.Today.ToString("yyyy-MM-dd"), Convert.ToDecimal(beginningDebit), Convert.ToDecimal(beginningCredit), (Convert.ToDecimal(endCredit) - Convert.ToDecimal(endDebit)), branchCode, Convert.ToDecimal(movementCredit), Convert.ToDecimal(movementDebit), Convert.ToDecimal(endCredit), Convert.ToDecimal(endDebit)));
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