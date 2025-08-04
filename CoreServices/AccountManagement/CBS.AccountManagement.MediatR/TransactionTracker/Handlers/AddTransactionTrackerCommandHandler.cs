using AutoMapper;
using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
 
    public class AddTransactionTrackerCommandHandler : IRequestHandler<AddTransactionTrackerCommand, ServiceResponse<bool>>
    {

        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTransactionTrackerCommandHandler> _logger; // Logger for logging handler actions and errors.
 
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

              public AddTransactionTrackerCommandHandler(IMapper mapper,
            ILogger<AddAccountCommandHandler> logger, PathHelper pathHelper, UserInfoToken userInfoToken)
        {

            _mapper = mapper;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

       
        public async Task<ServiceResponse<bool>> Handle(AddTransactionTrackerCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            try
            {
                // Get the MongoDB repository for TransactionTracker
                var _transactionRecordRepository = _mongoUnitOfWork.GetRepository<TransactionTracker>();
                //var model = await _transactionTrackerRepository.FindAsync(request.TransactionReference);

                var existingnRecord = await _transactionRecordRepository.GetByIdAsync(request.Id);
                if (existingnRecord == null)
                {
                    errorMessage = $"TransactionLog with ID {request.TransactionReference} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                var transaction = _mapper.Map<TransactionTracker>(request);
                await _transactionRecordRepository.InsertAsync(transaction);
                return ServiceResponse<bool>.ReturnResultWith201(true, "Transaction log successfully");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while saving Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommand",
                    request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<bool>.Return500(e);
            }
        }


    }
}