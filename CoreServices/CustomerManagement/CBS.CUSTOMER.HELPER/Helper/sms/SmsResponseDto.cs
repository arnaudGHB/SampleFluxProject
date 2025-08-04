using Newtonsoft.Json;


namespace BusinessServiceLayer.Objects.SmsObject
{

    public class SmsResponseDto
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusDescription")]
        public string? StatusDescription { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }   
        
        [JsonProperty("errors")]
        public object? Errors { get; set; } 
        
        [JsonProperty("data")]
        public Data? Data { get; set; }

    }

    public class Data
    {
        public string? id { get; set; }
        //public string? @from { get; set; }
        public string? to { get; set; }
        public double cost { get; set; }
        public int parts { get; set; }
        public string? status { get; set; }
    }

    /*public class OtherData
    {
        [JsonProperty("status")]
        public string? Status { get; set; }
    }*/

    /*public class Meta
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("statusDescription")]
        public string? StatusDescription { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }
    }*/

}
