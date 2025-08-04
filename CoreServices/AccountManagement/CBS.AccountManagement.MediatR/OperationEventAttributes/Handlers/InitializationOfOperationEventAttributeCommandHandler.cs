using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.OperationEventAttributesDto;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class InitializationOfOperationEventAttributeCommandHandler : IRequestHandler<InitializationOfOperationEventAttributeCommand, ServiceResponse<List<OperationEventAttributesDto>>>
    {
        private readonly IOperationEventRepository _OperationEventRepository;
        private readonly IOperationEventAttributeRepository _OperationEventAttributesRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<InitializationOfOperationEventAttributeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the InitializationOfOperationEventAttributeCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for OperationEventAttributes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public InitializationOfOperationEventAttributeCommandHandler(
            IOperationEventAttributeRepository OperationEventAttributesRepository,
              IOperationEventRepository OperationEventRepository,
            IMapper mapper,
            ILogger<InitializationOfOperationEventAttributeCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _OperationEventRepository = OperationEventRepository;
            _OperationEventAttributesRepository = OperationEventAttributesRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        public Task<ServiceResponse<List<OperationEventAttributesDto>>> Handle(InitializationOfOperationEventAttributeCommand request, CancellationToken cancellationToken)
        {
            try
            {
             
                var OperationEvents = _OperationEventRepository.All.ToList();

                OperationEvents= OperationEvents.Where(x => x.AccountTypeId== request.ServiceOperationTypeId).ToList();

                // If a AccountCategory with the same name already exists, return a conflict response
                if (OperationEvents.Count == 0)
                {
                    var errorMessage = $"No OperationEvent Was initially Created.";
                    _logger.LogError(errorMessage);
                    return Task.FromResult (ServiceResponse<List<OperationEventAttributesDto>>.Return409(errorMessage));
                }
                if (request.ServiceOperationType.ToUpper().Equals("TELLER"))
                {
                    var errorMessage = $"No OperationEvent for teller account type.";
                    _logger.LogError(errorMessage);
                    return Task.FromResult(ServiceResponse<List<OperationEventAttributesDto>>.Return409(errorMessage));
                }

                var listOperationEventAttributes = OperationEventAttributes.SetOperationEventAttributesEntities(_userInfoToken, OperationEvents, request.ServiceOperationType, request.ServiceOperationTypeId);
                List<OperationEventAttributesDto> EventAttributesDtos = _mapper.Map<List<OperationEventAttributesDto>>(listOperationEventAttributes);
                // Add the new OperationEventAttributes entity to the repository
                _OperationEventAttributesRepository.AddRange(listOperationEventAttributes);
                _uow.SaveAsync();
                return Task.FromResult(ServiceResponse<List<OperationEventAttributesDto>>.ReturnResultWith200(EventAttributesDtos));
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving OperationEventAttributes: {e.Message}";
                _logger.LogError(errorMessage);
                return Task.FromResult(ServiceResponse<List<OperationEventAttributesDto>>.Return500(e));
            }
        }
    }
}
