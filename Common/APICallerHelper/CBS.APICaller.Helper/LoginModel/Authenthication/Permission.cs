using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.APICaller.Helper.LoginModel.Authenthication
{
    public class Permission
    {
        public int menuMasterId { get; set; }
        public string id { get; set; }
        public string userID { get; set; }
        public string roleID { get; set; }
        public string roleName { get; set; }
        public string fullName { get; set; }
        public bool create { get; set; }
        public bool read { get; set; }
        public bool delete { get; set; }
        public bool update { get; set; }
        public bool download { get; set; }
        public bool upload { get; set; }
        public string menuText { get; set; }
        public int parentId { get; set; }
        public string controllerName { get; set; }
        public string actionName { get; set; }
        public string menuGroup { get; set; }
        public string iconClass { get; set; }
        public string description { get; set; }
        public bool isVisible { get; set; }
    }
}
