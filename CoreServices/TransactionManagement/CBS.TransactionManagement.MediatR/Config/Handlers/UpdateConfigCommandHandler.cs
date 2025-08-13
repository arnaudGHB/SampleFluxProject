using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a Config based on UpdateConfigCommand.
    /// </summary>
    public class UpdateConfigCommandHandler : IRequestHandler<UpdateConfigCommand, ServiceResponse<ConfigDto>>
    {
        private readonly IConfigRepository _ConfigRepository; // Repository for accessing Config data.
        private readonly ILogger<UpdateConfigCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateConfigCommandHandler.
        /// </summary>
        /// <param name="ConfigRepository">Repository for Config data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateConfigCommandHandler(
            IConfigRepository ConfigRepository,
            ILogger<UpdateConfigCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _ConfigRepository = ConfigRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateConfigCommand to update a Config.
        /// </summary>
        /// <param name="request">The UpdateConfigCommand containing updated Config data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ConfigDto>> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Config entity to be updated from the repository
                var existingConfig = await _ConfigRepository.FindAsync(request.Id);
                // Check if the Config entity exists
                if (existingConfig != null)
                {
                    // Map properties from UpdateConfigCommand to Config using AutoMapper
                    _mapper.Map(request, existingConfig);
                    // Use the repository to update the existing Config entity
                    _ConfigRepository.Update(existingConfig);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<ConfigDto>.ReturnResultWith200(_mapper.Map<ConfigDto>(existingConfig));
                    return response;
                }
                else
                {
                    // Handle case where the Config entity was not found
                    return ServiceResponse<ConfigDto>.Return404($"{request.Id} was not found to be updated.");
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                return ServiceResponse<ConfigDto>.Return500(e);
            }
        }
    }

}
