using AutoMapper;
using CBS.Communication.Common;
using CBS.Communication.Data;
using CBS.Communication.Data.Dto;
using CBS.Communication.Data.Entity;
using CBS.Communication.Domain;
using CBS.Communication.Domain.MongoDBContext.Repository.Uow;
using CBS.Communication.Helper;
using CBS.Communication.Helper.Helper;
using CBS.Communication.MediatR.Sms.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.Communication.MediatR.Sms.Handlers
{
    public class SendSingleSmsCommandHandler : IRequestHandler<SendSingleSmsCommand, ServiceResponse<NotificationDto>>
    {

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SendSingleSmsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;
        /// <summary>
        /// Constructor for initializing the AddDepartmentCommandHandler.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SendSingleSmsCommandHandler(
            IMapper mapper,
            ILogger<SendSingleSmsCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
             IMongoUnitOfWork mongoUnitOfWork,
            UserInfoToken userInfoToken,
            PathHelper pathHelper)
        {

            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mongoUnitOfWork = mongoUnitOfWork;
            _UserInfoToken = userInfoToken;
            _PathHelper = pathHelper;
        }


        public async Task<ServiceResponse<NotificationDto>> Handle(SendSingleSmsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Recipient = BaseUtilities.Add237Prefix(request.Recipient);

                SendSingleSmsSpecificationRequest sendSingleSms = new()
                {
                    Recipient = request.Recipient,
                    MessageBody = request.MessageBody,
                    Sender = request.SenderService

                };

                var sendSingleSmsResult = await SmsHelper.SendSingleSMS(_PathHelper.SMSOtherSinglePath, _PathHelper.SMSAPI_KEY, sendSingleSms);

                var notificationEntity = _mapper.Map<Notification>(request);
                notificationEntity.Timestamp = BaseUtilities.UtcToLocal(DateTime.Now);
                notificationEntity.BankID = _UserInfoToken?.BankID ?? "Nan";
                notificationEntity.BranchID = _UserInfoToken?.BranchId ?? "Nan";
                notificationEntity.BranchName = _UserInfoToken?.BranchName ?? "Nan";
                notificationEntity.Id = BaseUtilities.GenerateUniqueNumber(15);
                notificationEntity.NotificationType = NotificationType.Sms.ToString();
                notificationEntity.Title = request.Title;
                notificationEntity.Status = sendSingleSmsResult?.status ?? "Failed";
                notificationEntity.Body = request.MessageBody;

                // Get the MongoDB repository for Notification
                var notificationRepository = _mongoUnitOfWork.GetRepository<Notification>();


                // Add the Notification entity to the MongoDB repository
                await notificationRepository.InsertAsync(notificationEntity);

                // Map the Notification entity to AuditTrailDto and return it with a success response
                var notificationDto = _mapper.Map<NotificationDto>(notificationEntity);
                return ServiceResponse<NotificationDto>.ReturnResultWith200(notificationDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while sending sms : {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<NotificationDto>.Return500(e);
            }
        }
    }
}
