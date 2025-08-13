using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;

namespace CBS.TransactionManagement.Repository.DatabaseLogging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly TransactionContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly UserInfoToken _userInfoToken;

        public DatabaseLogger(string categoryName, TransactionContext context, IHttpContextAccessor httpContextAccessor)
        {
            _categoryName = categoryName;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel)
        {
            // Enable all log levels
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Get UserInfoToken from HttpContext if needed
            var userInfoToken = _httpContextAccessor.HttpContext?.Items["UserInfoToken"] as UserInfoToken;

            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);

            // Get user information from the token or HTTP context
            var userId = string.Empty; //_userInfoToken?.Id;
            var userFullName = string.Empty;// _userInfoToken?.FullName;
            var branchId = string.Empty; //_userInfoToken?.BranchID;
            var branchCode = string.Empty; //_userInfoToken?.BranchCode;
            var branchName = string.Empty; //_userInfoToken?.BranchName;

            // Get IP Address
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            // Get URL and Route Data (Controller, Action, and URL)
            var controller = _httpContextAccessor.HttpContext?.Request?.RouteValues["controller"]?.ToString();
            var action = _httpContextAccessor.HttpContext?.Request?.RouteValues["action"]?.ToString();
            var url = _httpContextAccessor.HttpContext?.Request?.Path.ToString();
            var auditLog = new AuditLog
            {
                UserId = userId,
                UserFullName = userFullName,
                BranchId = branchId,
                BranchCode = branchCode,
                BranchName = branchName,
                IPAddress = ipAddress, 
                ActionName = action, 
                ControllerName = controller,
                Timestamp = DateTime.UtcNow,
                LogLevel = logLevel.ToString(),
                Message = message,
                StackTrace = exception?.StackTrace,
                Url = url
            };
            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }
    }

}
