using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountType.
    /// </summary>
    public class CreateLoanProductDefaultCommandHandler : IRequestHandler<CreateLoanProductDefaultCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<CreateLoanProductDefaultCommandHandler> _logger; // Logger for logging handler actions and errors.
    
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;

        public CreateLoanProductDefaultCommandHandler(
            IAccountTypeRepository AccountTypeRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<CreateLoanProductDefaultCommandHandler> logger
,
            IMediator? mediator)
        {
      
            _logger = logger;
            _mediator = mediator;
                _userInfoToken = userInfoToken;
        }


        public async Task<ServiceResponse<bool>> Handle(CreateLoanProductDefaultCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var collection = request.GetProductList();
                foreach (var item in collection)
                {
                    var model = new AddProductAccountingBookCommand
                    {
                        ProductAccountBookId = item.ProductId,
                        ProductAccountBookName = item.ProductName,
                        ProductAccountBookDetails = Rubrique.ConvertRubriqueToProductAccountBookDetail(Rubrique.GetAllRubrique()),
                        ProductAccountBookType= "Loan_Product"

                    };
                  
                    var result = await _mediator.Send(model);
                    if (!result.Success)
                    {
                        throw new Exception("Configuration failed");
                    }


                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountType: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountTypeCommand",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}
