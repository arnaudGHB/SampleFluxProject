using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Branch.
    /// </summary>
    public class AddBranchCommandHandler : IRequestHandler<AddBranchCommand, ServiceResponse<BranchDto>>
    {
        private readonly IBranchRepository _BranchRepository; // Repository for accessing Branch data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBranchCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddBranchCommandHandler.
        /// </summary>
        /// <param name="BranchRepository">Repository for Branch data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBranchCommandHandler(
            IBranchRepository BranchRepository,
            IMapper mapper,
            ILogger<AddBranchCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _BranchRepository = BranchRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddBranchCommand to add a new Branch.
        /// </summary>
        /// <param name="request">The AddBranchCommand containing Branch data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BranchDto>> Handle(AddBranchCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingBranch = await _BranchRepository.FindBy(c => c.Name == request.Name || c.BranchCode == request.BranchCode).FirstOrDefaultAsync();

                if (existingBranch != null)
                {
                    var errorMessage = existingBranch.Name == request.Name
                        ? $"Branch {request.Name} already exists."
                        : $"Branch with code {request.BranchCode} already exists.";

                    _logger.LogError(errorMessage);
                    return ServiceResponse<BranchDto>.Return409(errorMessage);
                }

                var branchEntity = _mapper.Map<Branch>(request);
                branchEntity.Id = BaseUtilities.GenerateUniqueNumber();

                _BranchRepository.Add(branchEntity);
                await _uow.SaveAsync();

                var branchDto = _mapper.Map<BranchDto>(branchEntity);
                return ServiceResponse<BranchDto>.ReturnResultWith200(branchDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving Branch: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<BranchDto>.Return500(e, errorMessage);
            }
        }
    }

}
