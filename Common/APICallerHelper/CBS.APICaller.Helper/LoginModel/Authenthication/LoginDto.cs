

using Microsoft.AspNetCore.Http;

namespace CBS.APICaller.Helper.LoginModel.Authenthication

{

    public class CustomerMapper
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? SignatureUrl { get; set; }
        public string? Occupation { get; set; }
        public string? Address { get; set; }
        public string? IDNumber { get; set; }
        public string? IDNumberIssueDate { get; set; }
        public string? IDNumberIssueAt { get; set; }
        public string Language { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? MembershipApprovalStatus { get; set; }
        public string BranchId { get; set; }
        public bool Active { get; set; }
        public string BankId { get; set; }
        public string LegalForm { get; set; }
        public string BranchCode { get; set; }
        public string Matricule { get; set; }
        public string AgeCategoryStatus { get; set; }
        
    }


    public class LoginDto
    {
        public string id { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string bankId { get; set; }
        public string branchId { get; set; }
        public int expirationTime { get; set; }
        public DateTime expirationDate { get; set; }
        public string phoneNumber { get; set; }
        public string bearerToken { get; set; }
        public string refreshToken { get; set; }
        public bool isAuthenticated { get; set; }
        public object profilePhoto { get; set; }
        public object password { get; set; }
        public bool isMFA { get; set; }
        public object googleAuthenticatorSecretKey { get; set; }
        public bool isVerified { get; set; }
        public bool isBlocked { get; set; }
        public int loginAttempts { get; set; }
        public bool changePasswordOnFirstLogin { get; set; }
        public Branch branch { get; set; }
        public List<Role> roles { get; set; }
        public List<Claim> claims { get; set; }
        public object permissions { get; set; }


    }

    public class Bank
    {
        public string id { get; set; }
        public string bankCode { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string organizationId { get; set; }
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
        public string customerServiceContact { get; set; }
       
    }
 
    public class Branch
    {
        public string id { get; set; }
        public string branchCode { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string bankId { get; set; }
        public string registrationNumber { get; set; }
        public string logoUrl { get; set; }
        public string immatriculationNumber { get; set; }
        public string taxPayerNUmber { get; set; }
        public string pBox { get; set; }
        public bool activeStatus { get; set; }
        public bool isHeadOffice { get; set; }
        public string webSite { get; set; }
        public string dateOfCreation { get; set; }
        public string bankInitial { get; set; }
        public string motto { get; set; }
        public string headOfficeTelehoneNumber { get; set; }
        public string headOfficeAddress { get; set; }
        public Bank bank { get; set; }
     
    }

    public class BranchRoot
    {
        public List<Branch> data { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public List<object> errors { get; set; }
        public bool success { get; set; }
    }


    public class Role
    {
        public string userId { get; set; }
        public string roleId { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string roleName { get; set; }
        public bool isTeller { get; set; }
        public string branchId { get; set; }
        public string bankId { get; set; }
    }

    public class Claim
    {
        public string claimType { get; set; }
        public string claimValue { get; set; }
    }

    public class CallBackRespose
    {
        public string Id { get; set; }
        public string UrlPath { get; set; }
        public string FullPath { get; set; }
        public string DocumentName { get; set; }
        public string Extension { get; set; }
        public string BaseUrl { get; set; }
        public string DocumentType { get; set; }
        public string DocumentId { get; set; }
    }

    public class AddDocumentUploadedCommand
    {
        public IFormFileCollection FormFiles { get; set; }
        public string OperationID { get; set; }
        public string? DocumentId { get; set; }
        public string DocumentType { get; set; }
        public string ServiceType { get; set; }
        public string CallBackBaseUrl { get; set; }
        public string CallBackEndPoint { get; set; }
        public string RemoteFilePath { get; set; }
        public bool IsSynchronus { get; set; } = true;


        public   AddDocumentUploadedCommand(
        IFormFileCollection formFiles = null,
    string operationID = null,
    string documentId = null,
    string documentType = "Default",
    string serviceType = "Default",
    string callBackBaseUrl = "http://default.com",
    string callBackEndPoint = "/default",
    string remoteFilePath = "/default/path",
    bool isSynchronus = true)
        {
            FormFiles = formFiles ?? new FormFileCollection();
            OperationID = operationID ?? Guid.NewGuid().ToString();
            DocumentId = documentId;
            DocumentType = documentType;
            ServiceType = serviceType;
            CallBackBaseUrl = callBackBaseUrl;
            CallBackEndPoint = callBackEndPoint;
            RemoteFilePath = remoteFilePath;
            IsSynchronus = isSynchronus;
        }

        public async Task<double> GetFormFileCollectionSizeInMegabitsAsync()
        {
            long totalBytes = 0;
            foreach (var file in this.FormFiles)
            {
                using var stream = file.OpenReadStream();
                var buffer = new byte[81920]; // 80KB buffer
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
                {
                    totalBytes += bytesRead;
                }
            }

            return Math.Round((totalBytes * 8.0) / 1_000_000, 2);
        }
        public long GetFilesizeInKilobits( )
        {
            long totalBytes = 0;

            foreach (var file in this.FormFiles)
            {
                totalBytes += file.Length; // Gets size in bytes
            }

            // Convert bytes to kilobits (1 byte = 8 bits, 1 kilobit = 1024 bits)
            long kilobits = (totalBytes * 8) / 1024;

            return kilobits;
        }
    }
}