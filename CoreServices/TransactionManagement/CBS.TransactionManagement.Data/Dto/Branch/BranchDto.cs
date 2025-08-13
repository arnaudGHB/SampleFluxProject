using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class BranchDto
    {
        public string id { get; set; }
        public string branchCode { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string bankId { get; set; }
        public string capital { get; set; }
        public string registrationNumber { get; set; }
        public string logoUrl { get; set; }
        public string immatriculationNumber { get; set; }
        public string taxPayerNUmber { get; set; }
        public string pBox { get; set; }
        public string webSite { get; set; }
        public string dateOfCreation { get; set; }
        public string bankInitial { get; set; }
        public string motto { get; set; }
        public bool activeStatus { get; set; }
        public bool isHeadOffice { get; set; }
        public string headOfficeTelehoneNumber { get; set; }
        public string headOfficeAddress { get; set; }
        public Bank Bank { get; set; }
    }
}
