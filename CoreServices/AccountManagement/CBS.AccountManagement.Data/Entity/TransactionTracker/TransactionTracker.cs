using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    
    public class TransactionTracker : BaseEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the command data type.
        /// </summary>
        public string CommandDataType { get; set; }

        /// <summary>
        /// Gets or sets the command JSON object.
        /// </summary>
        public string CommandJsonObject { get; set; }
        /// <summary>
        /// Gets or sets transaction id
        /// </summary>
        public string TransactionReferenceId { get; set; }
        public bool HasPassed { get; set; }
        public int NumberOfRetry { get; set; }
        public DateTime DatePassed { get; set; }
    }

    public class TransactionTrackerResponse 
    {
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the command data type.
        /// </summary>
        public string CommandDataType { get; set; }

        public string TransactionTrackerId { get; set; }
       
        public string TransactionReferenceId { get; set; }
        public bool HasPassed { get; set; }
        public int NumberOfRetry { get; set; }
        public DateTime DatePassed { get; set; }
    }
}
