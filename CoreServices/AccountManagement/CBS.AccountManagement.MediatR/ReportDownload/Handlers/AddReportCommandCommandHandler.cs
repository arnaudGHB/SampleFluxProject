using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountCategory.
    /// </summary>
    public class AddReportCommandCommandHandler : IRequestHandler<AddReportCommand, ServiceResponse<ReportDto>>
    {
        private readonly IReportDownloadRepository _reportDownloadRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddReportCommandCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly FileProcessor _fileProcessor;

        /// <summary>
        /// Constructor for initializing the AddAccountCategoryCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddReportCommandCommandHandler(
            IReportDownloadRepository AccountCategoryRepository,
             UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AddReportCommandCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _reportDownloadRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;

        }

     
        public async Task<ServiceResponse<ReportDto>> Handle(AddReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                // Map the AddAccountCartegoryCommand to a AccountCartegory entity
                var AccountCartegoryEntity = _mapper.Map<ReportDownload>(request);
         
                // Add the new AccountCategory entity to the repository
                _reportDownloadRepository.Add(AccountCartegoryEntity);
                await _uow.SaveAsync();
                // Map the AccountCategory entity to AccountCategoryDto and return it with a success response
                var AccountCategoryDto = _mapper.Map<ReportDto>(AccountCartegoryEntity);
                return ServiceResponse<ReportDto>.ReturnResultWith200(AccountCategoryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountCategory: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ReportDto>.Return500(e);
            }
        }

        public  List<AccountCategory> SetAccountCategoriesEntities(List<AccountCategory> accountCategories, UserInfoToken _userInfoToken)
        {
            List<AccountCategory> categories = new List<AccountCategory>();
            foreach (AccountCategory entity in accountCategories)
            {
                entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.CreatedDate = BaseUtilities.UtcToDoualaTime( DateTime.Now.ToLocalTime());
                entity.ModifiedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now.ToLocalTime());
                entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CT");
                categories.Add(entity);
            }
            return categories;
        }

        public  AccountCategory SetAccountCategoryEntity(AccountCategory entity, UserInfoToken _userInfoToken)
        {


            entity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now.ToLocalTime());
            entity.ModifiedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now.ToLocalTime());
            entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CT");


            return entity;
        }

    }
}