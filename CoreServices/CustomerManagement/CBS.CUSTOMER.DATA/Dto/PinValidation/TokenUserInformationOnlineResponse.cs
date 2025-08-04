using CBS.APICaller.Helper.LoginModel.Authenthication;

namespace CBS.CUSTOMER.DATA.Dto.PinValidation
{
    public class TokenUserInformationOnlineResponse
    {
        public LoginData data { get; set; }
        public List<object> errors { get; set; }
        public int statusCode { get; set; }
        public string statusDescription { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }
}
