using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Data.Entity.LoanPurposeP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Commands;
using CBS.NLoan.Repository.LoanPurposeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanPurposeHandler : IRequestHandler<AddLoanPurposeCommand, ServiceResponse<LoanPurposeDto>>
    {
        private readonly ILoanPurposeRepository _LoanPurposeRepository; // Repository for accessing LoanPurpose data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanPurposeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanPurposeCommandHandler.
        /// </summary>
        /// <param name="LoanPurposeRepository">Repository for LoanPurpose data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanPurposeHandler(
            ILoanPurposeRepository LoanPurposeRepository,
            IMapper mapper,
            ILogger<AddLoanPurposeHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanPurposeRepository = LoanPurposeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanPurposeCommand to add a new LoanPurpose.
        /// </summary>
        /// <param name="request">The AddLoanPurposeCommand containing LoanPurpose data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanPurposeDto>> Handle(AddLoanPurposeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Check if a LoanPurpose with the same name already exists(case -insensitive)
                var existingLoanPurpose = await _LoanPurposeRepository.FindBy(c => c.PurposeName == request.PurposeName && !c.IsDeleted).FirstOrDefaultAsync();

                    //If a LoanPurpose with the same name already exists, return a conflict response
                if (existingLoanPurpose != null)
                    {
                        var errorMessage = $"{request.PurposeName} already exists.";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<LoanPurposeDto>.Return409(errorMessage);
                    }


                    // Map the AddLoanPurposeCommand to a LoanPurpose entity
                    var LoanPurposeEntity = _mapper.Map<LoanPurpose>(request);
                // Convert UTC to local time and set it in the entity
                LoanPurposeEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                LoanPurposeEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new LoanPurpose entity to the repository
                _LoanPurposeRepository.Add(LoanPurposeEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<LoanPurposeDto>.Return500();
                }

                // Map the LoanPurpose entity to LoanPurposeDto and return it with a success response
                var LoanPurposeDto = _mapper.Map<LoanPurposeDto>(LoanPurposeEntity);
                return ServiceResponse<LoanPurposeDto>.ReturnResultWith200(LoanPurposeDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanPurpose: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanPurposeDto>.Return500(e);
            }
        }
    }

}
