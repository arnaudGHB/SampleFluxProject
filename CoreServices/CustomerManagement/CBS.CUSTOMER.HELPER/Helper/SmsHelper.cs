using BusinessServiceLayer.Objects.SmsObject;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.HELPER.Helper
{
    public class SmsHelper
    {
        private readonly PathHelper _PathHelper;


        public SmsHelper(PathHelper pathHelper)
        {
            _PathHelper=pathHelper;
        }

        public async Task<SmsResponseDto> SendSms(SubSmsRequestDto subSmsRequestDto)
        {


            SmsRequestDto smsRequestDto = new()
            {
                Recipient = subSmsRequestDto.Msisdn,
                MessageBody = subSmsRequestDto.Message,
                SenderService = _PathHelper.CbsSmsServiceNameUrl
            };
            var GetTokenResponse = await APICallHelper.WithAuthenthicationFromOtherServer<SmsResponseDto>(subSmsRequestDto.Token,_PathHelper.CbsSMSSmsUrl, smsRequestDto);
            return GetTokenResponse;
        }
    }

}
