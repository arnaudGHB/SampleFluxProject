using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global
{
    public class AddProfilePictureCommand : IRequest<ServiceResponse<AddProfilePictureDto>>
    {
        public string? CustomerId { get; set; }
        public IFormFile? picture { get; set; }
    }
}
