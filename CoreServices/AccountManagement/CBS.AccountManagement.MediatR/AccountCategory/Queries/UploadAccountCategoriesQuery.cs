using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR 
{
    public class UploadAccountCategoriesQuery : IRequest<ServiceResponse<List<AccountCartegoryDto>>>
    {
        public List<AccountCartegoryDto> AccountCategories { get; set; }
    }
}
