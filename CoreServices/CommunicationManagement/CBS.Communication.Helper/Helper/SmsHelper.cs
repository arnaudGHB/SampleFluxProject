// Ignore Spelling: Sms Api

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.Helper.Helper
{
    public class SmsHelper
    {


        public static async Task<SendSingleSmsResponse> SendSingleSMS(string SmsUri,string ApiKey, SendSingleSmsSpecificationRequest sendSmsSpecification)
        {
            try
            {
                APICallHelper.ClearQueryStringParams();
                APICallHelper.AddQueryStringParam("api_key", ApiKey);
                APICallHelper.AddQueryStringParam("sender", sendSmsSpecification.Sender);
                APICallHelper.AddQueryStringParam("recipient", sendSmsSpecification.Recipient);
                APICallHelper.AddQueryStringParam("text", sendSmsSpecification.MessageBody);

                var finalUrl = APICallHelper.MakeFinalUrlWithOutResourceUrl(SmsUri);

                var response= await APICallHelper.GetRequestWithoutAuthentication<SendSingleSmsResponse>(finalUrl);

                return response;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }   
        
        
        public static async Task<List<SendSingleSmsResponse>> SendMultipleSMS(string MultipleSmsUri,string ApiKey,string sender, List<SendSingleSmsSpecificationRequest> smsMessages)
        {
            try
            {

                SendMultipleSmsRequest sendMultipleSmsRequest = new SendMultipleSmsRequest()
                {
                    BodyMessages = smsMessages,
                    Sender = sender

                };
                APICallHelper.ClearQueryStringParams();
                APICallHelper.AddQueryStringParam("api_key", ApiKey);
                var finalUrl = APICallHelper.MakeFinalUrlWithOutResourceUrl(MultipleSmsUri);



                return await APICallHelper.WithOutAuthenthicationFromOtherServer<List<SendSingleSmsResponse>>(finalUrl, sendMultipleSmsRequest);


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        
    }
}
