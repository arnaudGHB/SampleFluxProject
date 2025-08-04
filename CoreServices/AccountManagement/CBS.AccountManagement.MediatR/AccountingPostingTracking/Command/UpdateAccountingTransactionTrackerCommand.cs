using System;
using System.Collections.Generic;

using System.Globalization;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CBS.AccountManagement.MediatR.Command
{

    public class UpdateAccountingTransactionTrackerCommand : IRequest<ServiceResponse<bool>>
    {

        public string Id { get; set; }
        public bool HasPassed { get; set; }
        public int NumberOfRetry { get; set; }
        public DateTimeOffset DatePassed { get; set; }
        public UpdateAccountingTransactionTrackerCommand()
        {
                
        }
        public UpdateAccountingTransactionTrackerCommand(string Id, bool HasPassed, int NumberOfRetry, DateTimeOffset dateTime )
        {
           this.Id = Id;
            this.HasPassed = HasPassed;
            this.NumberOfRetry = NumberOfRetry;
            this.DatePassed = dateTime;



        }
    }





}
