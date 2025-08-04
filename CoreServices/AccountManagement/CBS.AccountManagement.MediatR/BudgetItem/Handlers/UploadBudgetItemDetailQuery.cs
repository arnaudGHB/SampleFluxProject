using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace CBS.BudgetItemDetailManagement.MediatR 
{

    public class UploadBudgetItemDetailQueryHandler : IRequestHandler<UploadBudgetItemDetailQuery, ServiceResponse<List<BudgetItemDetailDto>>>
    {
        // Dependencies
        private readonly IBudgetItemDetailRepository _budgetItemDetailRepository;
        private readonly ILogger<UploadBudgetItemDetailQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        // Constructor to inject dependencies
        public UploadBudgetItemDetailQueryHandler(IBudgetItemDetailRepository budgetItemDetailRepository, ILogger<UploadBudgetItemDetailQueryHandler> logger, IMapper mapper, UserInfoToken userInfoToken)
        {
            _budgetItemDetailRepository = budgetItemDetailRepository; 
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
        }

        // Handle method implementation
        public async Task<ServiceResponse<List<BudgetItemDetailDto>>> Handle(UploadBudgetItemDetailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List < BudgetItemDetailDto > Modelist = new List<BudgetItemDetailDto>();
              
                foreach (var item in request.BudgetItemDetails)
                {
                    var existingBudgetItemDetail =   _budgetItemDetailRepository.FindBy(c => c.BudgetId == item.BudgetId &&c.IsDeleted==false);
                    if (existingBudgetItemDetail.Any())
                    {
                        var model = existingBudgetItemDetail.FirstOrDefault();
                        //Modelist.Add(new BudgetItemDetailDto(model.BudgetItemDetailNumber, model.BudgetItemDetailName,model.CurrentBalance.ToString()));
                    }
                    else
                    {
                        // Log an error if OperationEventName list is empty
                        //Modelist.Add(new BudgetItemDetailUploadDto(item.BudgetItemDetailNumber, item.BudgetItemDetailHolder, 0.ToString()));
                    }
                }
                string errorMessage = $" Acccount upload successful. Success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UploadBudgetItemDetailQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<BudgetItemDetailDto>>.ReturnResultWith200(Modelist);
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
          
                string errorMessage = $"An error occured: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UploadBudgetItemDetailQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<BudgetItemDetailDto>>.Return500(e);
            }
        }
    }
}
