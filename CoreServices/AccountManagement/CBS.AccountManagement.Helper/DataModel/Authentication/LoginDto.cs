namespace CBS.AccountManagement.Helper
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public object ProfilePhoto { get; set; }
        public object Provider { get; set; }
        public object BankID { get; set; }
        public string BranchID { get; set; }
        public bool IsActive { get; set; }
        public string GoogleAuthenticatorSecretKey { get; set; }
        public string BarcodeImageUrl { get; set; }
        public bool IsGoogleAuthenticatorEnabled { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<object> UserAllowedIPs { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<object> UserClaims { get; set; }
    }

 

    public class UserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public object UserName { get; set; }
        public object FirstName { get; set; }
        public object LastName { get; set; }
        public object RoleName { get; set; }
        public bool IsTeller { get; set; }
        public object BranchId { get; set; }
        public object BankId { get; set; }
    }



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