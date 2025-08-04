using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Repository
{
    public interface IConfigRepository : IGenericRepository<Config>
    {
        Task<Config> GetConfigAsync(string source);
        Task CheckIfSystemIsOpen();
        Task OpenDay(Config config);
        Task CloseDay(Config config);
        Task SetAutomAticChargingOFF(Config config);
        Task SetAutomAticChargingON(Config config);

    }
}
