using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData
{

    public class TransactionTrackerAccountingDto
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
        public DateTime TransactionDate { get; set; }
        public string? DestinationUrl { get; set; }
        public string? SourceUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
