// Ignore Spelling: Sms

using Newtonsoft.Json;


namespace CBS.Communication.Helper.Helper
{
    public  class SmsMessages
    {
        public string? Title { get; set; }
        public string? Recipient { get; set; }

        public string? CustomerName { get; set; }
        public string? MessageBody { get; set; }
        public string? MemberReference { get; set; }
    }
}
