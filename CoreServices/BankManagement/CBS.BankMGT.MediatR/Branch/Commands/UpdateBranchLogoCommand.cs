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

    public class UpdateBranchLogoCommand : IRequest<ServiceResponse<BranchDto>>
    {
        public IFormFileCollection FormFile { get; set; }
        public string RootPath { get; set; }
        public string BranchID { get; set; }
        public string BaseURL { get; set; }
    }
}
