using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.MongoDBObjects
{
    public class LoanRepaymentMongo
    {
        public string Id { get; set; }
        public string BranchId { get; set; }
        public string SalaryCode { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; } // Successful, Pending (I get all by SalaryCode and Status==Pending), Failed
        public string FileUploadId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public DateTime RefundDate { get; set; }
        public string Description { get; set; }
        public string LoanId { get; set; }
        public string Error { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public string MemberReference { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public decimal TotalRepaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentChannel { get; set; }
    }

}
