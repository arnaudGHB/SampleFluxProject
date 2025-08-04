using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddErrorRequestCommand : IRequest<ServiceResponse<bool>>
    {
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
        public string TransctionReferenceId { get; set; }

        /// <summary>
        /// Gets or sets HasPassed
        /// </summary>
        public bool HasPassed { get; set; }

    }
    
  
}
