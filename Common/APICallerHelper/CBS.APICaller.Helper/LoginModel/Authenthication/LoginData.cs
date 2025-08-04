using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.APICaller.Helper.LoginModel.Authenthication
{
    public class LoginData
    {
        public string? id { get; set; }
        public string? userName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
        public object bankId { get; set; }
        public string? branchId { get; set; }
        public int expirationTime { get; set; }
        public DateTime expirationDate { get; set; }
        public string? phoneNumber { get; set; }
        public string? bearerToken { get; set; }
        public string? refreshToken { get; set; }
        public bool isAuthenticated { get; set; }
        public object profilePhoto { get; set; }
        public List<Role> roles { get; set; }
        public List<Claim> claims { get; set; }
        public List<Permission> permissions { get; set; }
    }
}
