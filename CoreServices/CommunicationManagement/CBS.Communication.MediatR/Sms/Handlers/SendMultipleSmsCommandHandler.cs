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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.MediatR.Sms.Handlers
{
    public class SendMultipleSmsCommandHandler : IRequestHandler<SendMultipleSmsCommand, ServiceResponse<List<NotificationDto>>>
    {

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SendSingleSmsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        /// <summary>
        /// Constructor for initializing the AddDepartmentCommandHandler.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SendMultipleSmsCommandHandler(
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


        public async Task<ServiceResponse<List<NotificationDto>>> Handle(SendMultipleSmsCommand request, CancellationToken cancellationToken)
        {
            try
            {
           
                if (request.Messages != null && request.Messages.Count != 0)
                {
                    List<SendSingleSmsSpecificationRequest> singleSmsSpecificationRequests = new List<SendSingleSmsSpecificationRequest>(); 


                    request.Messages.ForEach(e => {
                        e.Recipient = BaseUtilities.Add237Prefix(e.Recipient);
                        SendSingleSmsSpecificationRequest sendSingleSms = new SendSingleSmsSpecificationRequest();
                        sendSingleSms.Sender = request.SenderService;
                        sendSingleSms.MessageBody = e.MessageBody;
                        sendSingleSms.Recipient=e.Recipient;
                         singleSmsSpecificationRequests.Add(sendSingleSms);
                        }
                    );

                    var sendMultipleSmsResult = await SmsHelper.SendMultipleSMS(_PathHelper.SMSOtherMultiplePath, _PathHelper.SMSAPI_KEY, request.SenderService, singleSmsSpecificationRequests);

                    List<NotificationDto> notificationDtos = new List<NotificationDto>();

                    sendMultipleSmsResult.ForEach(async e => {

                        var notificationSub= request.Messages.FirstOrDefault(x=>x.Recipient==e.to);

                        var notificationEntity = _mapper.Map<Notification>(notificationSub);
                        notificationEntity.Timestamp = BaseUtilities.UtcToLocal(DateTime.Now);
                        notificationEntity.BankID = _UserInfoToken?.BankID ?? "Nan";
                        notificationEntity.BranchID = _UserInfoToken?.BranchId ?? "Nan";
                        notificationEntity.BranchName = _UserInfoToken?.BranchName ?? "Nan";
                        notificationEntity.Id = BaseUtilities.GenerateUniqueNumber(15);
                        notificationEntity.NotificationType = NotificationType.Sms.ToString();
                        notificationEntity.Status = e?.status ?? "Failed";
                        notificationEntity.Title = notificationSub.Title;
                        notificationEntity.Body = notificationSub.MessageBody;

                        // Get the MongoDB repository for Notification
                        var notificationRepository = _mongoUnitOfWork.GetRepository<Notification>();

                        // Add the Notification entity to the MongoDB repository
                        await notificationRepository.InsertAsync(notificationEntity);

                        // Map the Notification entity to AuditTrailDto and return it with a success response
                        var notificationDto = _mapper.Map<NotificationDto>(notificationEntity);

                        notificationDtos.Add(notificationDto);  

                    });


                    return ServiceResponse<List<NotificationDto>>.ReturnResultWith200(notificationDtos);

                }



                return ServiceResponse<List<NotificationDto>>.Return404("The List  Messages cannot be empty or null");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Department: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<NotificationDto>>.Return500(e);
            }
        }
    }
}
