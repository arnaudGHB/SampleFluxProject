using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.MediatR.ChartOfAccount.Commands;

namespace CBS.AccountManagement.MediatR 
{

    public class UpdateListOfChartOfAccountCommandHandler : IRequestHandler<UpdateListOfChartOfAccountCommand, ServiceResponse<List<ChartOfAccountDto>>>
    {
        // Dependencies
        private readonly IChartOfAccountRepository _ChartOfAccountRepository;
        private readonly IAccountCategoryRepository _AccountCategoryRepository;
        private readonly ILogger<UpdateListOfChartOfAccountCommandHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;

        // Constructor to inject dependencies
        public UpdateListOfChartOfAccountCommandHandler(IChartOfAccountRepository ChartOfAccountRepository,
            IAccountCategoryRepository AccountCategoryRepository,
            ILogger<UpdateListOfChartOfAccountCommandHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper)
        {
            _ChartOfAccountRepository = ChartOfAccountRepository;
            _AccountCategoryRepository = AccountCategoryRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
        }

     
        public async Task<ServiceResponse<List<ChartOfAccountDto>>> Handle(UpdateListOfChartOfAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there are existing AccountCategorys in the repository
                var entityExists = _ChartOfAccountRepository.All.Count() == 0;

                if (entityExists)
                {
                    // Map DTOs to entity models
                    var listAccountCategorys = _mapper.Map<List<Data.ChartOfAccount>>(request.ChartOfAccounts);
                    //// Add the new AccountCategories to the repository
                    _ChartOfAccountRepository.AddRange(listAccountCategorys);

                    // Map the added AccountCategories back to DTOs
                    var ChartOfAccount = _mapper.Map<List<ChartOfAccountDto>>(request.ChartOfAccounts);

                    // Return a successful response with the added AccountCategories
                    return ServiceResponse<List<ChartOfAccountDto>>.ReturnResultWith200(request.ChartOfAccounts);
                }
                else
                {
                    var messageError = "ChartOfAccount has already been configured.";
                    // Log an error if AccountCategory list is empty
                    _logger.LogError(messageError);

                    // Return a not found response
                    return ServiceResponse<List<ChartOfAccountDto>>.Return404(messageError);
                }
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
                var errorMessage = $"Error occurred while reading the AccountCategory configurations: {e.Message}";
                _logger.LogError(errorMessage);

                // Return a server error response
                return ServiceResponse<List<ChartOfAccountDto>>.Return500(errorMessage);
            }
        }

        private List<Data.CashMovementTracker> AssignAccountCartegoryId(List<Data.CashMovementTracker> listAccountCategorys)
        {
            List<Data.CashMovementTracker> subAccounts = new List<Data.CashMovementTracker>();
            foreach (var accountCategory in listAccountCategorys)
            {
                var entity = _AccountCategoryRepository.FindBy(n => n.Account.Equals(accountCategory.Id)).FirstOrDefault();
                accountCategory.Id = entity.Id;
                subAccounts.Add(accountCategory);
            }
            return subAccounts;
        }
    }
}
