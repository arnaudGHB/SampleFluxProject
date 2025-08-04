using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{
   
    public class ConfigRepository : GenericRepository<Config, TransactionContext>, IConfigRepository
    {
        public ConfigRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }
        // Method to retrieve system configuration
        public async Task<Config> GetConfigAsync(string source)
        {
            var config = await All.FirstOrDefaultAsync();
           
            // Check if system config exist
            if (config == null)
            {
                var errorMessage = $"Sorry, System configuration not found.";
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            // Verify if system year is opened.
            if (!config.IsYearOpen)
            {
                var errorMessage = $"Sorry, system year is not opened. Contact your system adminstrator.";
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            //// Verify if system year is opened.
            //if (!config.IsDayOpen)
            //{
            //    var errorMessage = $"Sorry, system day is not opened. Contact your system adminstrator.";
            //    throw new InvalidOperationException(errorMessage); // Throw exception.
            //}
            if (source != OperationSourceType.TTP.ToString())
            {
                // Verify if system day is opened.
                if (!config.IsDayOpen)
                {
                    var errorMessage = $"Sorry, system day is not opened. Contact your system adminstrator.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
            }
           

            return config; // Return config.
        }
        public async Task CheckIfSystemIsOpen()
        {
            var config = await All.FirstOrDefaultAsync();

            // Check if system config exist
            if (config == null)
            {
                var errorMessage = $"Sorry, System configuration not found.";
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Verify if system year is opened.
            if (!config.IsYearOpen)
            {
                var errorMessage = $"Sorry, system year is not opened. Contact your system adminstrator.";
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Verify if system year is opened.
            if (!config.IsDayOpen)
            {
                var errorMessage = $"Sorry, system day is not opened. Contact your system adminstrator.";
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

        }
        public async Task CloseDay(Config config)
        {
            config.IsDayOpen = false;
            Update(config);
        }
        public async Task OpenDay(Config config)
        {
            config.IsDayOpen = true;
            Update(config);
        }
        public async Task SetAutomAticChargingOFF(Config config)
        {
            config.UseAutomaticChargingSystem = false;
            Update(config);
        }
        public async Task SetAutomAticChargingON(Config config)
        {
            config.UseAutomaticChargingSystem = true;
            Update(config);
        }
    }
}
