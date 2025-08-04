using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.ExceptionHandler
{
    public class ChartOfAccountException : Exception
    {
        public string ExceptionType { get; set; }
        public string ExceptionCode{ get; set; }
      
        public ChartOfAccountException() : base() { }

        public ChartOfAccountException(string message) : base(message) { }

        public ChartOfAccountException(string message, Exception innerException) : base(message, innerException) { }

    }

     
}
