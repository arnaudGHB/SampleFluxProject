
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.Customer.MEDIATR;

namespace CBS.JobTitle.MEDIATR.JobTitleMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new JobTitle.
    /// </summary>
    public class AddJobTitleCommandHandler : IRequestHandler<AddJobTitleCommand, ServiceResponse<CreateJobTitle>>
    {
        private readonly IJobTitleRepository _JobTitleRepository; // Repository for accessing JobTitle data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddJobTitleCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddJobTitleCommandHandler.
        /// </summary>
        /// <param name="JobTitleRepository">Repository for JobTitle data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddJobTitleCommandHandler(
            IJobTitleRepository JobTitleRepository,
            IMapper mapper,
            ILogger<AddJobTitleCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _JobTitleRepository = JobTitleRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddJobTitleCommand to add a new JobTitle.
        /// </summary>
        /// <param name="request">The AddJobTitleCommand containing JobTitle data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateJobTitle>> Handle(AddJobTitleCommand request, CancellationToken cancellationToken)
        {
            try
            {
              

                // Map the AddJobTitleCommand to a JobTitle entity
                var JobTitleEntity = _mapper.Map<CUSTOMER.DATA.Entity.JobTitle>(request);

               

                JobTitleEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                //JobTitleEntity.JobTitleId = BaseUtilities.GenerateUniqueNumber();
 
                // Add the new JobTitle entity to the repository
                _JobTitleRepository.Add(JobTitleEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateJobTitle>.Return500();
                }
                // Map the JobTitle entity to CreateJobTitle and return it with a success response
                var CreateJobTitle = _mapper.Map<CreateJobTitle>(JobTitleEntity);
                return ServiceResponse<CreateJobTitle>.ReturnResultWith200(CreateJobTitle);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving JobTitle: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateJobTitle>.Return500(e);
            }
        }
    }

}
