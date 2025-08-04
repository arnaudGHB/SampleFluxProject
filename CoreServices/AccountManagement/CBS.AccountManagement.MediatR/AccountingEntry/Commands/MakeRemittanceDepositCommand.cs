using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{


    public class MakeRemittanceCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string? MemberReference { get; set; }
        public string ProductId { get; set; }
        public string OperationType { get; set; }
        public string HeadOfficeBranchCode { get; set; }
        public string HeadOfficeBranchId { get; set; }
        public string Source { get; set; }//Members_Account, Physical_Teller,Vitual_Teller
        public List<CollectionAmount>? AmountCollection { get; set; }
        public DateTime TransactionDate { get; set; }
        public CollectionAmount GetPrincipalAmount(string amountType,string levelOfexecution)
        {

            return this.AmountCollection.Where(x => x.AmountType == amountType && x.LevelOfExecution==levelOfexecution).FirstOrDefault();
        }



    }

    public class CollectionAmount
    {

        public string? EventAttributeName { get; set; }
        public string LevelOfExecution { get; set; }
        public decimal Amount { get; set; }
        public string AmountType { get; set; }
        public string? Naration { get; set; }

        public string GetOperationEventCode(string operation)
        {
            return operation + "@" + this.EventAttributeName;
        }
       
    }
}
