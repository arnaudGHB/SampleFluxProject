using CBS.TransactionManagement.Data.Dto.User;
using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.User.Command
{
    public class GenerateOTPCommand : IRequest<ServiceResponse<OTPDto>>
    {
        public string Id { get; set; }
    }
}
