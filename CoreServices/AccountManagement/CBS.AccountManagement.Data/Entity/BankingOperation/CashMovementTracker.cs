 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class CashMovementTracker : BaseEntity
    {
        public string Id { get; set; }
        public string OperationType { get; set; }
        public string ReferenceId{ get; set; }
        public string Constraint { get; set; }
        public string StartTime { get; set; }
        public string ExpectedEndTime { get; set; }
        public string EndTime { get; set; }
        public string DoneBy { get; set; }

        public string Status { get; set; }//Open,Ongoing,Close,UnderTracking
        public string CashMovementTrackingConfigurationId { get; set; }
        public virtual  CashMovementTrackingConfiguration CashMovementTrackingConfiguration { get; set; }
    }


    public enum CashMovementTrackerStatus
    {
        Open,
        Ongoing,
        Close, 
        UnderTracking
    }
}
