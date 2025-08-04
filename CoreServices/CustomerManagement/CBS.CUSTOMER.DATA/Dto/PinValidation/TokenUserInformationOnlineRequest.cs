namespace CBS.CUSTOMER.DATA.Dto.PinValidation
{
    public class TokenUserInformationOnlineRequest
    {
        public string? Msisdn { get; set; }
        public string? Pin { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Channel { get; set; }
        public string? IpAddress { get; set; }
        public string? BranchId { get; set; }
    }
}
