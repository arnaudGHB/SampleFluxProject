using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.MediatR 
{

    //public class UploadOperationEventQueryHandler : IRequestHandler<UploadOperationEventQuery, ServiceResponse<List<OperationEventDto>>>
    //{
    //    // Dependencies
    //    private readonly IOperationEventRepository _OperationEventNameRepository;
    //    private readonly ILogger<UploadOperationEventQueryHandler> _logger;
    //    private readonly IMapper _mapper;
    //    private readonly UserInfoToken _userInfoToken;
    //    private readonly IUnitOfWork<POSContext> _unitOfWork;
    //    // Constructor to inject dependencies
    //    public UploadOperationEventQueryHandler(IUnitOfWork<POSContext> uow,IOperationEventRepository OperationEventNameRepository, ILogger<UploadOperationEventQueryHandler> logger, IMapper mapper, UserInfoToken userInfoToken)
    //    {
    //        _OperationEventNameRepository = OperationEventNameRepository;
    //        _logger = logger;
    //        _mapper = mapper;
    //        _userInfoToken = userInfoToken;
    //        _unitOfWork = uow;
    //    }

    //    // Handle method implementation
    //    public async Task<ServiceResponse<List<OperationEventDto>>> Handle(UploadOperationEventQuery request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            // Check if there are existing OperationEvent in the repository
    //            var entityExists = _OperationEventNameRepository.All.ToList();

    //            if (entityExists.Count() == 0)
    //            {
    //                // Map DTOs to entity models
    //                var listOperationEvent = _mapper.Map<List<OperationEvent>>(request.OperationEvent);

    //                listOperationEvent = OperationEvent.SetOperationEventEntities(listOperationEvent, _userInfoToken);
                   
    //                // Add the new OperationEvent to the repository
    //                _OperationEventNameRepository.AddRange(listOperationEvent);

    //               await _unitOfWork.SaveAsync();
    //                // Map the added OperationEvent back to DTOs
    //                var OperationEventDto = _mapper.Map<List<OperationEventDto>>(request.OperationEvent);

    //                // Return a successful response with the added OperationEvent
    //                return ServiceResponse<List<OperationEventDto>>.ReturnResultWith200(OperationEventDto);
    //            }
    //            else
    //            {
    //                // Log an error if OperationEventName list is empty
    //                _logger.LogError("OperationEventName list empty.");

    //                // Return a not found response
    //                return ServiceResponse<List<OperationEventDto>>.Return404();
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            // Log an error if an exception occurs during processing
    //            var errorMessage = $"Error occurred while reading the OperationEventName configurations: {e.Message}";
    //            _logger.LogError(errorMessage);

    //            // Return a server error response
    //            return ServiceResponse<List<OperationEventDto>>.Return500(e);
    //        }
    //    }
    //}
}
