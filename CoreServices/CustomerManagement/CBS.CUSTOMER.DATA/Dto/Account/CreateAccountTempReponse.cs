using BusinessServiceLayer.Objects.SmsObject;


namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateAccountTempReponse
    {
        public CreateAccountTempReponseData? Data { get; set; }
        public List<object>? Errors { get; set; }
        public int StatusCode { get; set; }
        public string? StatusDescription { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }
    }
}
