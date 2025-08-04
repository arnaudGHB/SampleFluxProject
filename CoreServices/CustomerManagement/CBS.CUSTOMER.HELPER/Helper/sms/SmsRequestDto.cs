using Newtonsoft.Json;


namespace BusinessServiceLayer.Objects.SmsObject
{
    public class SmsRequestDto
    {
        [JsonProperty("senderService")]
        public string? SenderService { get; set; }

        [JsonProperty("recipient")]
        public string? Recipient { get; set; }

        [JsonProperty("messageBody")]
        public string? MessageBody { get; set; }

    }
}
