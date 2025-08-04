namespace CBS.BankMGT.Data
{
    public class AuditTrail
    {
        public string Id { get; set; }
        public string? Action { get; set; } // Action performed (Create, Update, Delete, etc.)
        public DateTime Timestamp { get; set; } // Timestamp of the action
        public string? UserName { get; set; } // User responsible for the action
        public required string MicroServiceName { get; set; }
        public string? FullName { get; set; }
        public string? UserID { get; set; }
        public string Level { get; set; }
        public string IPAddress { get; set; }
        public int? StatusCode { get; set; }
        public string StringifyObject { get; set; }// Entity affected by the action
        public string? DetailMessage { get; set; } // Additional details or metadata
        public string? BranchID { get; set; }
        public string? BankID { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }
        public string? CorrolationId { get; set; }
    }

}
