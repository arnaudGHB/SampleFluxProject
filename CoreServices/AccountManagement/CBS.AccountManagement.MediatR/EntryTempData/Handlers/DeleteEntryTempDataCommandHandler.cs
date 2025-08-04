using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a AccountFeature based on DeleteAccountFeatureCommand.
    /// </summary>
    public class DeleteEntryTempDataCommandHandler : IRequestHandler<DeleteEntryTempDataCommand, ServiceResponse<bool>>
    {
        
            private readonly IEntryTempDataRepository _entryTempDataRepository; // Repository for accessing AccountFeature data.
            private readonly IMapper _mapper; // AutoMapper for object mapping.
            private readonly ILogger<DeleteEntryTempDataCommandHandler> _logger; // Logger for logging handler actions and errors.
            private readonly IUnitOfWork<POSContext> _uow;
            private readonly UserInfoToken _userInfoToken;
            /// <summary>
            /// Constructor for initializing the AddAccountFeatureCommandHandler.
            /// </summary>
            /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
            /// <param name="mapper">AutoMapper for object mapping.</param>
            /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
            /// <param name="logger">Logger for logging handler actions and errors.</param>
            public DeleteEntryTempDataCommandHandler(
                IEntryTempDataRepository AccountFeatureRepository,
                IMapper mapper,
                ILogger<DeleteEntryTempDataCommandHandler> logger,
                IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
            {
                _entryTempDataRepository = AccountFeatureRepository;
                _mapper = mapper;
                _logger = logger;
                _uow = uow;
                _userInfoToken = userInfoToken;
            }

            /// <summary>
            /// Handles the DeleteAccountFeatureCommand to delete a AccountFeature.
            /// </summary>
            /// <param name="request">The DeleteAccountFeatureCommand containing AccountFeature ID to be deleted.</param>
            /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteEntryTempDataCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountFeature entity with the specified ID exists
                var existing0  =   _entryTempDataRepository.FindBy(c=> c.Id == request.Id&&c.CreatedBy==_userInfoToken.Id); 
                if (existing0  == null)
                {
                
                      errorMessage = $"EntryTempData with {request.Id} already does not exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteEntryTempDataCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                else
                {
                   var existing= existing0.FirstOrDefault();
                    existing.IsDeleted = true;
                        _entryTempDataRepository.Remove(existing);
                   var kk=     await _uow.SaveAsync();
errorMessage = $"EntryTempData {request.Id} was successfully deleted.";
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }

 
                
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AccountFeature: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteEntryTempDataCommand",
           request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}