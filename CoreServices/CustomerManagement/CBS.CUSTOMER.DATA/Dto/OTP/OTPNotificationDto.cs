namespace CBS.CUSTOMER.DATA.Dto.OTP
{

    public class TempOTPDto
    {
        public string Otp { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Url { get; set; }

        public string Id { get; set; }
        public bool IsVerify { get; set; } = true;
    }
    public class TempVerificationOTPDto
    {
        public string Id { get; set; }
        public bool IsVerify { get; set; } = true;
    }
}
