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
using CBS.Communication.MediatR.Sms.Handlers;
using Google.Apis.Auth.OAuth2;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BaseUtilities = CBS.Communication.Helper.Helper.BaseUtilities;

namespace CBS.Communication.MediatR.PushNotifications.Handlers
{
    public class SendPushNotificationSmsCommandHandler : IRequestHandler<SendSinglePushNotificationCommand, ServiceResponse<NotificationDto>>
    {

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SendPushNotificationSmsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;
        /// <summary>
        /// Constructor for initializing the SendPushNotificationSmsCommandHandler.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SendPushNotificationSmsCommandHandler(
            IMapper mapper,
            ILogger<SendPushNotificationSmsCommandHandler> logger,
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

        static async Task<string> GetAccessTokenAsync(string serviceAccountFile)
        {
            GoogleCredential credential;
           
            using (var stream = new System.IO.FileStream(serviceAccountFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }  

        public async Task<ServiceResponse<NotificationDto>> Handle(SendSinglePushNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                // Map the AddNotificationCommand to an Notification entity
                var notificationEntity = _mapper.Map<Notification>(request);
                notificationEntity.Timestamp = BaseUtilities.UtcToLocal(DateTime.Now);
                notificationEntity.BankID = _UserInfoToken?.BankID ?? "Nan";
                notificationEntity.BranchID = _UserInfoToken?.BranchId ?? "Nan";
                notificationEntity.BranchName = _UserInfoToken?.BranchName ?? "Nan";
                notificationEntity.Id = BaseUtilities.GenerateUniqueNumber(15);
                notificationEntity.Body = request.NotificationBody;
                notificationEntity.Title = request.NotificationTitle;
                notificationEntity.NotificationType = NotificationType.PushNotification.ToString();


                var GetCMoneyMember =await GetCMoneyMembersActivationAsync(request.MemberReference);
                if (!GetCMoneyMember.Success)
                {
                    notificationEntity.Status = Status.Failed.ToString();
                    var notificationDto = await SaveNotificationAsync(notificationEntity);
                    return ServiceResponse<NotificationDto>.Return403(notificationDto, GetCMoneyMember.Message);
                }
                
                if (GetCMoneyMember.Data ==null || GetCMoneyMember.Data.NotificationToken.IsNullOrEmpty())
                {
                    notificationEntity.Status = Status.Failed.ToString();
                    var notificationDto = await SaveNotificationAsync(notificationEntity);
                    return ServiceResponse<NotificationDto>.Return403(notificationDto, "Notification token is either null or empty.");
                }

                notificationEntity.CustomerName = GetCMoneyMember.Data.Name;

                SendPushNotificationResponse sendPushNotificationResponse = new SendPushNotificationResponse();
                string serviceAccountFile = _PathHelper.ServiceAccountFilePath;
                string fcmEndpoint = _PathHelper.FcmEndpoint;

                // 1. Get an OAuth 2.0 access token
                var accessToken = await GetAccessTokenAsync(serviceAccountFile);

                // 2. Create the notification payload
                var message = new
                {
                    message = new
                    {
                        token = GetCMoneyMember.Data.NotificationToken, // Replace with the recipient's device token
                        notification = new
                        {
                            title = request.NotificationTitle,
                            body = request.NotificationBody,
                            image = request.NotificationImage
                        },

                    }
                };

                // 3. Serialize the payload to JSON
                string jsonMessage = JsonConvert.SerializeObject(message);

                // 4. Send the request
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(fcmEndpoint, content);
                    

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("Notification sent successfully!");
                        string responseBody = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("Response: " + responseBody);

                        sendPushNotificationResponse = JsonConvert.DeserializeObject<SendPushNotificationResponse>(responseBody);

                        notificationEntity.Status = Status.Successful.ToString();
                        var notificationDto = await SaveNotificationAsync(notificationEntity);
                        return ServiceResponse<NotificationDto>.ReturnResultWith200(notificationDto);
                    }
                    else
                    {
                        notificationEntity.Status = Status.Failed.ToString();
                        var notificationDto = await SaveNotificationAsync(notificationEntity);
                        _logger.LogInformation($"Error: {response.StatusCode}");
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        _logger.LogInformation("Error Response: " + errorResponse);
                        return ServiceResponse<NotificationDto>.Return403(notificationDto,"Error while sending notification.");
                    }
                }


               
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while processing notificating: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<NotificationDto>.Return500(e);
            }
        }

        public async Task<NotificationDto> SaveNotificationAsync(Notification notificationEntity)
        {
            // Get the MongoDB repository for Notification
            var notificationRepository = _mongoUnitOfWork.GetRepository<Notification>();


            // Add the Notification entity to the MongoDB repository
            await notificationRepository.InsertAsync(notificationEntity);

            // Map the Notification entity to AuditTrailDto and return it with a success response
            return _mapper.Map<NotificationDto>(notificationEntity);

           
        }

        private async Task<ServiceResponse<CMoneyMembersActivationAccountDto>> GetCMoneyMembersActivationAsync(string memberReference)
        {
            return await APICallHelper.GetRequestWithAuthentication< ServiceResponse<CMoneyMembersActivationAccountDto> >( _PathHelper.CustomerBaseUrl, _PathHelper.GetCMoneyMembersActivationByIdPath+memberReference,_UserInfoToken.Token);
            
        }
        //var GetTokenResponse = await APICallHelper.WithAuthenthicationFromOtherServer<SmsResponseDto>(subSmsRequestDto.Token,_PathHelper.CbsSMSSmsUrl, smsRequestDto);

    }
}
