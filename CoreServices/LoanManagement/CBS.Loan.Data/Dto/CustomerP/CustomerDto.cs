using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.CustomerP
{
    public class CustomerDto
    {
        public string customerId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool isMemberOfACompany { get; set; }
        public bool isMemberOfAGroup { get; set; }
        public string legalForm { get; set; }
        public string formalOrInformalSector { get; set; }
        public string bankingRelationship { get; set; }
        public string placeOfBirth { get; set; }
        public string occupation { get; set; }
        public string address { get; set; }
        public string idNumber { get; set; }
        public string idNumberIssueDate { get; set; }
        public string idNumberIssueAt { get; set; }
        public string membershipApplicantDate { get; set; }
        public string membershipApplicantProposedByReferral1 { get; set; }
        public string membershipApplicantProposedByReferral2 { get; set; }
        public string membershipApprovalBy { get; set; }
        public string membershipApprovalStatus { get; set; }
        public string membershipApprovedDate { get; set; }
        public string membershipApprovedSignatureUrl { get; set; }
        public string membershipAllocatedNumber { get; set; }
        public string poBox { get; set; }
        public string fax { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string countryId { get; set; }
        public string regionId { get; set; }
        public string townId { get; set; }
        public string photoUrl { get; set; }
        public bool isUseOnLineMobileBanking { get; set; }
        public string mobileOrOnLineBankingLoginState { get; set; }
        public string customerPackageId { get; set; }
        public string customerCode { get; set; }
        public string bankName { get; set; }
        public string divisionId { get; set; }
        public string branchId { get; set; }
        public string economicActivitiesId { get; set; }
        public string signatureUrl { get; set; }
        public string bankId { get; set; }
        public string organizationId { get; set; }
        public string language { get; set; }
        public string subDivisionId { get; set; }
        public string taxIdentificationNumber { get; set; }
        public bool active { get; set; }
        public string secretQuestion { get; set; }
        public string secretAnswer { get; set; }
        public string employerName { get; set; }
        public string employerTelephone { get; set; }
        public string employerAddress { get; set; }
        public string maritalStatus { get; set; }
        public string spouseName { get; set; }
        public string spouseAddress { get; set; }
        public string spouseOccupation { get; set; }
        public string spouseContactNumber { get; set; }
        public string customerCategoryId { get; set; }
        public string workingStatus { get; set; }
        public string activeStatus { get; set; }
    }
}
