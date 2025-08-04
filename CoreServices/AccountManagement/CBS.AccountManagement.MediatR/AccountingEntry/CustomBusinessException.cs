using CBS.AccountManagement.Helper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.AccountingEntry
{
    // Custom exception class
    public class CustomBusinessException : Exception
    {
        public string ErrorCode { get; }

        public CustomBusinessException(string message) : base(message)
        {
        }

        public CustomBusinessException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public CustomBusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ExceptionLogger
    {
        public static async void LogException(Exception ex,  string Command, LogAction action)
            {
            string message = "";
            // In a real application, you might log to a file, database, or logging service
            message =$"[{DateTime.Now}] [{action.ToString()}] ERROR: {ex.Message}\n";
            message = message + $"Stack Trace: {ex.StackTrace}";

            if (ex.InnerException != null)
            {
                message = message +  $"Inner Exception: {ex.InnerException.Message}";
               
            }
            await BaseUtilities.LogAndAuditAsync(message, Command, HttpStatusCodeEnum.OK, action, LogLevelInfo.Critical);
        }
    }

    public class ExceptionManager
    {
        // Centralized exception handler
        //public static void HandleException(Exception ex)
        //{
        //    // Log the exception
        //    ExceptionLogger.LogException(ex);

        //    // Handle specific exception types
        //    switch (ex)
        //    {
        //        case CustomBusinessException businessEx:
        //            Console.WriteLine($"Business error occurred. Code: {businessEx.ErrorCode}");
        //            // Business-specific recovery logic
        //            break;

        //        case SqlException sqlEx:
        //            Console.WriteLine($"Database error occurred. Error Number: {sqlEx.Number}");
        //            // Database-specific recovery logic
        //            break;

        //        case FileNotFoundException fileEx:
        //            Console.WriteLine($"File not found: {fileEx.FileName}");
        //            // File-related recovery logic
        //            break;

        //        case ArgumentException argEx:
        //            Console.WriteLine("Invalid argument provided.");
        //            // Argument-related recovery logic
        //            break;

        //        default:
        //            Console.WriteLine("An unexpected error occurred. Please try again later.");
        //            break;
        //    }
        //}
    }

}
