using AutoMapper;
using CBS.Communication.Common;
using CBS.Communication.Data;
using CBS.Communication.Data.Dto;
using CBS.Communication.Data.Entity;
using CBS.Communication.Domain;
using CBS.Communication.Domain.MongoDBContext.Repository.Uow;
using CBS.Communication.Helper;
using CBS.Communication.Helper.Helper;
using CBS.Communication.MediatR.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.Communication.MediatR.GeneralNotification.Handlers
{
    public class GetAllNotificationsByNotificationTypeQueryHandler : IRequestHandler<GetAllNotificationsByNotificationTypeQuery, ServiceResponse<List<NotificationDto>>>
    {

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllNotificationsByNotificationTypeQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly PathHelper _PathHelper;
        /// <summary>
        /// Constructor for initializing the GetAllNotificationsByNotificationTypeQuery.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllNotificationsByNotificationTypeQueryHandler(
            IMapper mapper,
            ILogger<GetAllNotificationsByNotificationTypeQueryHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IMongoUnitOfWork mongoUnitOfWork,
            PathHelper pathHelper)
        {

            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mongoUnitOfWork = mongoUnitOfWork;
            _UserInfoToken = userInfoToken;
            _PathHelper = pathHelper;
        }


        public async Task<ServiceResponse<List<NotificationDto>>> Handle(GetAllNotificationsByNotificationTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Get the MongoDB repository for Notification
                var notificationRepository = _mongoUnitOfWork.GetRepository<Notification>();

                Dictionary<string , object> keyValuePairs = new Dictionary<string , object>();
                keyValuePairs.Add("NotificationType", request.NotificationType);
                keyValuePairs.Add("Status", Status.Successful);

                // Retrieve all non-deleted Notification entities
                var entities = await notificationRepository.GetWithGenericFilterAsync(keyValuePairs, "Timestamp",true);

              
                // Map the entities to NotificationDto and return a success response
                var notificationDtos = _mapper.Map<List<NotificationDto>>(entities);
                return ServiceResponse<List<NotificationDto>>.ReturnResultWith200(notificationDtos);
         
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while get notification of type {request.NotificationType}  : {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<NotificationDto>>.Return500(e);
            }
        }
    }
}
