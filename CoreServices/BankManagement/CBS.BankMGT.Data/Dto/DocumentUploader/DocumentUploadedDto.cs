using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class DocumentUploadedDto
    {
        public string Id { get; set; }
        public string UrlPath { get; set; }
        public string DocumentName { get; set; }
        public string Extension { get; set; }
        public string BaseUrl { get; set; }
        public string OperationId { get; set; }
        public string FullPath { get; set; }
        public bool ResponseFromClientService { get; set; }
        public string FullLocalPath { get; set; }
        public string ServiceType { get; set; }
        public string DocumentType { get; set; }//NextofkingsPhoto,NextofkingsSignature,CustomerPhoto,CustomerSignature,CustomerOtherDocument
    }
}
