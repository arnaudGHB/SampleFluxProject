using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
 
    public class UsersNotification : BaseEntityNotification
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public string Action { get; set; }
        public string ActionId { get; set; }
        public string ActionUrl { get; set; }

        public string BranchId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSeen { get; set; }

        public string TempData { get; set; }

        
        public UsersNotification()
        {
            // Default constructor
        }
        public UsersNotification(string action,string actionUrl, string actionId,string id, string userId,string branchId,string correspondingBranch)
        {
            Id = id;
                Action = action;
            ActionUrl = actionUrl;
            ActionId = actionId;
            BranchId = branchId;
            UserId = userId;
            BankId = "1";
            TempData = correspondingBranch;
        }
        public UsersNotification(string userName, string branchName, string action, DateTime timestamp, string actionUrl)
        {
            Id = userName;
 
            Action = action;

            ActionUrl = actionUrl;
        }

        public UsersNotification SetOperationEventEntity(UsersNotification userModel, UserInfoToken userInfoToken)
        {
        
            userModel.UserId = userInfoToken.Id;
            return userModel;
        }
    }
}
