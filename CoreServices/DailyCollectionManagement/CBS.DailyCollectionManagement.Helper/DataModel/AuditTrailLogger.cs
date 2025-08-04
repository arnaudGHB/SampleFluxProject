using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CBS.DailyCollectionManagement.Helper.DataModel
{
    public class AuditTrailLogger
    {
        public string? Action { get; set; } // Action performed (Create, Update, Delete, etc.)
        public string? UserName { get; set; } // User responsible for the action
        public string? MicroServiceName { get; set; }
        public string StringifyObject { get; set; }// Entity affected by the action
        public string? DetailMessage { get; set; }
        public string? Level { get; set; }
      
        public int StatusCode { get; set; }
        public AuditTrailLogger(string action, string userName, string microService, string stringifyObject, string detailMessage, string level, int statusCode)
        {
            Action = action;
            UserName = userName;
            MicroServiceName = microService;
            StringifyObject = stringifyObject;
            DetailMessage = detailMessage;
            Level = level;
            StatusCode = statusCode;
        }

       
    }
    //public enum LogAction
    //{
    //    Create, Update, Delete, Read, Download, Upload, Login
    //}
   

}
