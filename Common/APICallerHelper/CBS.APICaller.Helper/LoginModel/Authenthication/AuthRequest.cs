using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.APICaller.Helper.LoginModel.Authenthication

{
    public class AuthRequest
    {
        public AuthRequest(string username, string password)
        {
            UserName = username;
            Password = password;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
    
}