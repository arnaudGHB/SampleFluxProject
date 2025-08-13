using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP
{
    public class SalaryUploadModel:BaseEntity
    {
        public string Id { get; set; }
        public string SalaryCode { get; set; }
        public string Matricule { get; set; }
        public string? CustomerId { get; set; }
        public string Surname { get; set; }
        public string? MemberReference { get; set; }
        public string? Phonumber { get; set; }
        public string Name { get; set; }
        public string LaisonBankAccountNumber { get; set; }
        public decimal NetSalary { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string FileUploadId { get; set; }
        public DateTime Date { get; set; }
        public string UploadedBy { get; set; }
        public string SalaryType { get; set; }
        public virtual FileUpload FileUpload { get; set; }
    }

  
}
