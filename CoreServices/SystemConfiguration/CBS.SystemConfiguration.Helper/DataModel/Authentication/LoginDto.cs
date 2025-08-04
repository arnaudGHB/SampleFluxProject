namespace CBS.SystemConfiguration.Helper
{
    public class LoginDto
    {
        public LoginDto(string id, string userName, string firstName, string lastName, string email, int expirationTime, string phoneNumber, string bearerToken, bool isAuthenticated, string profilePhoto, List<Claim> claims)
        {
            this.id = id;
            this.userName = userName;
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.expirationTime = expirationTime;
            this.phoneNumber = phoneNumber;
            this.bearerToken = bearerToken;
            this.isAuthenticated = isAuthenticated;
            this.profilePhoto = profilePhoto;
            this.claims = claims;
        }

        public string id { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public int expirationTime { get; set; }
        public string phoneNumber { get; set; }
        public DateTime expirationDate { get; set; }
        public string bearerToken { get; set; }
        public bool isAuthenticated { get; set; }
        public string profilePhoto { get; set; }
        public List<Claim> claims { get; set; }
    }
}