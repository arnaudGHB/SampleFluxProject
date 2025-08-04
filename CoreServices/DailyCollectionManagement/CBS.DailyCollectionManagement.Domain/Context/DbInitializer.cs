using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using CBS.DailyCollectionManagement.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.DailyCollectionManagement.Domain
{
    public static class DbInitializer
    {
        //public static void Initialize(POSContext context, List<OperationEvent> eventsList, List<OperationEventAttributes> eventAttributesList)
        //{
        //    try
        //    {
        //        context.Database.EnsureCreated();

        //        // Check if the database already has data

        //        if (context.OperationEventAttributes.Any() && context.OperationEvent.Any())// && context.AccountCategories.Any() && context.ChartOfAccounts.Any() && context.AccountClasses.Any())
        //        {
        //            return; // Database has been seeded
        //        }
                
        //        context.OperationEventAttributes.AddRange(eventAttributesList);
        //        context.OperationEvent.AddRange(eventsList);
        //        context.SaveChanges();

             
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
 
    }
}
