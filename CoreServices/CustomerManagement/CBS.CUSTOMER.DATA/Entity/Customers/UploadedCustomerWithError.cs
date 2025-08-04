

// Ignore Spelling: Kycs Online

using System.ComponentModel.DataAnnotations;
using CBS.CUSTOMER.DATA.Entity.Document;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class UploadedCustomerWithError : BaseEntity
    {
        /*
           var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", data.MemberNumber, data.MembersName, data.CNI, data.DateCNI, data.LieuCNI, data.Telephone, data.AdhesionDate, data.Type, data.Genre, data.Balance, data.BranchCode, Errors != null ? Errors[0] : null);
         */
        [Key]
        public string Id { get; set; }
        public string? MemberNumber { get; set; }
        public string? MemberName { get; set; }
        public string? MemberSurName { get; set; }
        public string? Cni { get; set; }
        public string? CniDeliveranceDate { get; set; }
        public string? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? CniLocation { get; set; }
        public string? Telephone { get; set; }
        public string? CreationDate { get; set; }
        public string? Type { get; set; }
        public string? Town { get; set; }
        public string? Quater { get; set; }
        public string? Genre { get; set; }
        public string? BranchCode { get; set; }
        public string? BankCode { get; set; }
        public string? InitialError { get; set; }
        public bool? resolved { get; set; }
   

    }
}
