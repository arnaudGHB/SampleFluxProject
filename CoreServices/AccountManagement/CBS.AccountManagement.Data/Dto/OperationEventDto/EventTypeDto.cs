using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class EventTypeDto
    {
        public string Id { get; set; }
        public string OperationEventCode { get; set; }
        public string Description { get; set; }
        public string EventCode { get; set; }
        public bool HasMultipleEntries { get; set; }

        public OperationEvent ToOperationEvent()
        {
            return new OperationEvent
            {
                OperationEventName = OperationEventCode,
                Description = Description,
                Id = Id
            };
        }
    }

}
