using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.AccountingDayOpening
{

    public class DeleteAccountingDayCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountingDay to be deleted.
        /// </summary>
        public string Id { get; set; }
        public bool DelateByDate { get; set; }
        public DateTime Date { get; set; }
    }


}
