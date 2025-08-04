using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.OperationEventNameAttributes.Queries;
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
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR 
{

    public class UploadOperationEventAttributesQueryHandler : IRequestHandler<UploadOperationEventAttributesQuery, ServiceResponse<List<OperationEventAttributesDto>>>
    {
        // Dependencies
        private readonly IOperationEventAttributeRepository _OperationEventNameAttributesRepository;
        private readonly IOperationEventRepository _OperationEventRepository;
        private readonly ILogger<UploadOperationEventAttributesQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<POSContext> _unitOfWork;
        // Constructor to inject dependencies
        public UploadOperationEventAttributesQueryHandler(IUnitOfWork<POSContext> unitOfWork, IOperationEventRepository operationEventRepository,IOperationEventAttributeRepository OperationEventNameRepository, ILogger<UploadOperationEventAttributesQueryHandler> logger, IMapper mapper, UserInfoToken userInfoToken)
        {
            _OperationEventRepository = operationEventRepository;
            _OperationEventNameAttributesRepository = OperationEventNameRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _unitOfWork = unitOfWork;
        }

        // Handle method implementation
        public async Task<ServiceResponse<List<OperationEventAttributesDto>>> Handle(UploadOperationEventAttributesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there are existing OperationEvent in the repository
                var entityExists = _OperationEventNameAttributesRepository.All.ToList();
                var listOperationEvents = _OperationEventRepository.All.ToList();

                if (entityExists.Count() == 0)
                {
                    // Map DTOs to entity models
                    var listOperationEventAttributte = _mapper.Map<List<Data.OperationEventAttributes>>(request.OperationEventAttributes);

                    var listOperationEventAttributes = Data.OperationEventAttributes.SetOperationEventEntities(JsonConvert.SerializeObject(listOperationEvents) ,listOperationEventAttributte, _userInfoToken);
                   
                    // Add the new OperationEvent to the repository
                    _OperationEventNameAttributesRepository.AddRange(listOperationEventAttributes);
                    await _unitOfWork.SaveAsync();
                    
                    // Map the added OperationEvent back to DTOs
                    var OperationEventDto = _mapper.Map<List<OperationEventAttributesDto>>(request.OperationEventAttributes);
                    //SaveToJsonFile("OperationEventAttributes.json", JsonConvert.SerializeObject(listOperationEventAttributes));
                    // Return a successful response with the added OperationEvent
                    return ServiceResponse<List<OperationEventAttributesDto>>.ReturnResultWith200(OperationEventDto);
                }
                else
                {
                    // Log an error if OperationEventName list is empty
                    _logger.LogError("OperationEventAttributes is not list empty.");

                    // Return a not found response
                    return ServiceResponse<List<OperationEventAttributesDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
                var errorMessage = $"Error occurred while reading the OperationEventName configurations: {e.Message}";
                _logger.LogError(errorMessage);

                // Return a server error response
                return ServiceResponse<List<OperationEventAttributesDto>>.Return500(e);
            }
        }
        static void SaveToJsonFile(string filePath, string json)
        {
            // Serialize the data to JSON


            // Write the JSON to a file
            File.WriteAllText(filePath, json);
        }
    }
    
}
