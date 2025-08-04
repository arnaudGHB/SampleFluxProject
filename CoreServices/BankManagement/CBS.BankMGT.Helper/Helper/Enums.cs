using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Helper
{
    public enum TrackerState
    {
        Created,
        Modified,
        Deleted,
    }
    public enum SMSTypes
    {
        Subscription,
        Saving,
        Claim,
        Cashout
    }
    public enum ServiceTypes
    {
        ClientMicroService,
        LoanMicroService,
        AccountMicroService,
        ClaimMicroService
    }
    /// <summary>
    /// Enumeration representing various HTTP status codes and their corresponding descriptions.
    /// </summary>
    public enum HttpStatusCodeEnum
    {

        /// <summary>
        /// 200 OK: The request has succeeded.
        /// </summary>
        OK = 200,

        /// <summary>
        /// 201 Created: The request has been fulfilled and a new resource has been created.
        /// </summary>
        Created = 201,

        /// <summary>
        /// 204 No Content: The server successfully processed the request and is not returning any content.
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// 400 Bad Request: The server could not understand the request due to invalid syntax.
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// 401 Unauthorized: The request requires user authentication.
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 403 Forbidden: The server understood the request, but refuses to authorize it.
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 404 Not Found: The server can not find the requested resource.
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 409 Conflict: The request could not be completed due to a conflict with the current state of the target resource.
        /// </summary>
        Conflict = 409,

        /// <summary>
        /// 500 Internal Server Error: The server has encountered a situation it doesn't know how to handle.
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// 502 Bad Gateway: The server, while acting as a gateway or proxy, received an invalid response from the upstream server.
        /// </summary>
        BadGateway = 502,

        /// <summary>
        /// 503 Service Unavailable: The server is not ready to handle the request, often due to being overloaded or down for maintenance.
        /// </summary>
        ServiceUnavailable = 503
    }
    /// <summary>
    /// Represents the various actions that can be logged in the application.
    /// </summary>
    public enum LogAction
    {
        AddBankingZoneCommand,
        UpdateBankingZoneCommand,
        AddBankZoneBranchCommand,
        UpdateBankZoneBranchCommand,
        GetBankZoneBranchQuery,
        GetAllBankZoneBranchQuery,
        GetBankingZonebyBankBranchQuery,
        DeleteBankZoneBranchCommand
    }

    public enum LogLevelInfo
    {
        Information,   // General informational messages
        Warning,       // Potential issues or important events
        Error,         // Error events that might still allow the application to continue running
        Critical,      // Serious errors that require immediate attention
        Debug,         // Detailed information for debugging purposes
        Trace         // Fine-grained informational events for tracking the flow of the application

    }

    public enum SubscriptionStatus
    {
        Awaiting_Customer_Momo_Validation,
        Unsubscrbed,
        Failed, Subscribed,ReSubcription,
        Unsubscribed,
        ReSubscription
    }
    public enum ResultStatus
    {
        Ok,
        Failed
       
    }
}
