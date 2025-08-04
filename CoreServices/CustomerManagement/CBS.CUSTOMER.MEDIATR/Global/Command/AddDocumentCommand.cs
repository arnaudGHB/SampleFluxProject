using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global
{
    public class AddDocumentCommand : IRequest<ServiceResponse<bool>>
    {
        public string? Id { get; set; }
        public string? UrlPath { get; set; }
        public string? DocumentName { get; set; }
        public string? Extension { get; set; }
        public string? BaseUrl { get; set; }
        public string? DocumentType { get; set; }
    }
}
