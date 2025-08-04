using CBS.NLoan.Data.Dto.SMS;
using CBS.NLoan.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.SMS.Command
{
    public class SendSMSPICallCommand : IRequest<ServiceResponse<SMSDto>>
    {
        public string senderService { get; set; }
        public string recipient { get; set; }
        public string messageBody { get; set; }
        public string? Environment { get; set; }
        public string? Token { get; set; }
    }
}
