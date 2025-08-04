

namespace CBS.CUSTOMER.DATA.Dto
{
    public class GetCustomerDocument
    {
      
        public string? DocumentId { get; set; }
        public string? CustomerId { get; set; }
        public string? UrlPath { get; set; }
        public string? DocumentName { get; set; }
        public string? Extension { get; set; }
        public string? BaseUrl { get; set; }
        public string? DocumentType { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
