using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public enum LogAction
    {
        Create, Update, Delete, Read, Download, Upload, Login, AccountingPosting
    }
    public enum LogLevelInfo
    {
        Information, Error
    }
}
