using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data;

namespace CBS.BankMGT.MediatR.Handlers
{
 
    public class UpdateBranchLogoCommandHandler : IRequestHandler<UpdateBranchLogoCommand, ServiceResponse<BranchDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IBranchRepository _brachRepository;
        private readonly ILogger<UpdateBranchLogoCommandHandler> _logger;
        public readonly PathHelper _pathHelper;
        public UpdateBranchLogoCommandHandler(
            IMapper mapper,
            IUnitOfWork<POSContext> uow,


            ILogger<UpdateBranchLogoCommandHandler> logger,
            PathHelper pathHelper
,
            IBranchRepository bankRepository = null)
        {
            _mapper = mapper;
            _uow = uow;
            _logger = logger;
            _pathHelper = pathHelper;
            _brachRepository = bankRepository;
        }
        public async Task<ServiceResponse<BranchDto>> Handle(UpdateBranchLogoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate request
                if (request.FormFile == null || request.FormFile.Count == 0)
                {
                    return ServiceResponse<BranchDto>.Return400("No file provided for update.");
                }

                var fullPath = await SaveBranchLogoAsync(request);

                var branch = await _brachRepository.FindAsync(request.BranchID);
                branch.LogoUrl = !string.IsNullOrWhiteSpace(fullPath) ? $"{request.BaseURL}/{fullPath}" : string.Empty;

                // Update Bank
                _brachRepository.Update(branch);
                await _uow.SaveAsync();

                return ServiceResponse<BranchDto>.ReturnResultWith200(_mapper.Map<BranchDto>(branch));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Branch logo: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<BranchDto>.Return500(e, errorMessage);
            }
        }

        private async Task<string> SaveBranchLogoAsync(UpdateBranchLogoCommand request)
        {
            var filePath = $"{request.RootPath}/{_pathHelper.BranchLogoPath}";
            var profileFile = request.FormFile[0];

            // Validate file extension, size, or other relevant criteria if needed

            var fileExtension = Path.GetExtension(profileFile.FileName);
            var newProfilePhoto = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(filePath, newProfilePhoto);

            // Log information about file upload
            _logger.LogInformation($"Saving Branch logo to: {fullPath}");

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await profileFile.CopyToAsync(stream);
            }

            return $"{_pathHelper.BranchLogoPath}{newProfilePhoto}";
        }

    }

}
