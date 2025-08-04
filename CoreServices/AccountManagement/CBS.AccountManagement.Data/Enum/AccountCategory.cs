namespace CBS.AccountManagement.Data
{
   public enum QueryParameter
    {
        all,
        bybranch
    
    }
  public  enum DashboardAccountType
    {
        CashInHand,
        CashInBank,
        CashInVault,
       // TotalShares,
        PreferenceShare,
        OrdinaryShares,
        Deposit, Savings,
        Gav,
        DailyCollections,
        MTNMobileMoney,
        OrangeMoney,
        TotalExpense,
        TotalIncome//,
        //TotalLiquidity
    }
    public enum AccountCategoryEnum
    {
        // Represents a standard checking account for daily transactions.
        Checking,

        // Represents a savings account for accumulating interest on deposited funds.
        Savings,

        // Represents a fixed-term deposit account with a specified maturity date.
        FixedDeposit,

        // Represents a money market account with higher interest rates and limited transactions.
        MoneyMarket,

        // Represents an individual retirement account for retirement savings.
        IRA,

        // Represents a business account for corporate clients.
        Business,

        // Represents a joint account held by multiple individuals.
        Joint,

        // Represents a specialized account for educational expenses.
        Education,

        // Represents a trust account managed by a trustee for the benefit of others.
        Trust,

        // Represents a custodial account for managing assets on behalf of a minor.
        Custodial,

        // Represents a foreign currency account for transactions in a specific currency.
        ForeignCurrency,

        // Represents a premium account with enhanced features and benefits.
        Premium,

        // Represents a charity or nonprofit organization account.
        NonProfit,

        // Represents a specialized account for health-related expenses.
        HealthSavings,

        // Represents an account specifically designed for senior citizens.
        SeniorCitizen,

        // Represents a specialized account for real estate transactions.
        RealEstate,

        // Represents a specialized account for government entities or agencies.
        Government,

        // Represents a specialized account for electronic transactions and payments.
        DigitalWallet
    }
}