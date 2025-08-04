using CBS.BankMGT.Data.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using CBS.BankMGT.Helper;

namespace CBS.BankMGT.MediatR
{
    public class UpdateBankLogoCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string UrlPath { get; set; }
    }
}
