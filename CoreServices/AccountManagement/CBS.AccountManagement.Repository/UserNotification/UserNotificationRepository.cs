using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
  
    public class UserNotificationRepository : GenericRepository<UsersNotification, POSContext>, IUserNotificationRepository
    {
        public UserNotificationRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}