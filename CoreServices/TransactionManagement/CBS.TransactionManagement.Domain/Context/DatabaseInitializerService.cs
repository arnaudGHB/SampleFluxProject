using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DatabaseInitializerService
{
    public static void Initialize(TransactionContext dbContext)
    {
        //AddConfigRecord(dbContext, SystemConfigs.tax.ToString(), "0.5", "System tax rate");
        //AddConfigRecord(dbContext, SystemConfigs.StartOfDayAlert.ToString(), "6", "Start of day alert");
        //AddConfigRecord(dbContext, SystemConfigs.EndOFDayAlert.ToString(), "17", "End of day alert");
        //AddConfigRecord(dbContext, SystemConfigs.IsDayOpen.ToString(), "False", "Is the day open for transactions");
        //AddConfigRecord(dbContext, SystemConfigs.IsYearOpen.ToString(), "True", "Is the year open for transactions");
        //AddSavingProductRecord(dbContext, DefaultProducts.DAILYOPERATIONS.ToString(), "To be used by tellers for daily transactions");
    }

    // Add a record to the Config table
    private static void AddConfigRecord(TransactionContext dbContext, string name, string value, string description)
    {
        // Check if the record already exists
        if (!dbContext.Configs.Any(c => c.Name == name))
        {
            // Add the record if it doesn't exist
            dbContext.Configs.Add(new Config
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                Name = name,
                Value = value,
                CreatedBy = "SYSTEM",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = "SYSTEM",
                Description = description
            });
            dbContext.SaveChanges();
        }
    }

    // Add a record to the SavingProduct table
    private static void AddSavingProductRecord(TransactionContext dbContext, string name, string description)
    {
        // Check if the record already exists
        if (!dbContext.SavingProducts.Any(c => c.Name == name))
        {
            // Add the record if it doesn't exist
            dbContext.SavingProducts.Add(new SavingProduct
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                Name = name, 
                Description = description,
                CreatedBy = "SYSTEM",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = "SYSTEM"
            });
            dbContext.SaveChanges();
        }
    }
}
