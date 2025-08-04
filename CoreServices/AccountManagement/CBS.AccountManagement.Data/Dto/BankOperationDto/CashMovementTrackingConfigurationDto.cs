using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class CashMovementTrackingConfigurationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MovementType { get; set; }
        public string Duration { get; set; }

        public string AlertTime { get; set; }
        public string MessageBeforeAlertTime { get; set; }
        public string MessageAfterAlertTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
