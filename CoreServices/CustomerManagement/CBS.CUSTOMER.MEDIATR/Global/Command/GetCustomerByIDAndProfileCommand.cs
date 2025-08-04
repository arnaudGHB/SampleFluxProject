using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global
{
    public class GetCustomerByIDAndProfileCommand : IRequest<ServiceResponse<GetByCustomerByIdWithProfileTypeDto>>
    {
        public string? Id { get; set; }
    }
}
