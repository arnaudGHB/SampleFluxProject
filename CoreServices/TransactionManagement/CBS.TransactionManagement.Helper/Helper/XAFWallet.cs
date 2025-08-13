using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper
{
    public class XAFWallet
    {
        private List<XAFCurrencyUnit> CurrencyUnits { get; }

        public XAFWallet()
        {
            CurrencyUnits = new List<XAFCurrencyUnit>();
        }

        public void AddCurrency(XAFDenomination denomination, int quantity)
        {
            var existingUnit = CurrencyUnits.Find(unit => unit.Denomination == denomination);

            if (existingUnit != null)
            {
                existingUnit.Quantity += quantity;
            }
            else
            {
                CurrencyUnits.Add(new XAFCurrencyUnit { Denomination = denomination, Quantity = quantity });
            }
        }

        public void CollectDenominations(Dictionary<XAFDenomination, int> denominations)
        {
            foreach (var denomination in denominations)
            {
                AddCurrency(denomination.Key, denomination.Value);
            }
        }

        public decimal CalculateTotalValue()
        {
            decimal totalValue = 0;

            foreach (var unit in CurrencyUnits)
            {
                decimal denominationValue = GetDenominationValue(unit.Denomination);
                totalValue += denominationValue * unit.Quantity;
            }

            return totalValue;
        }

        private decimal GetDenominationValue(XAFDenomination denomination)
        {
            switch (denomination)
            {
                case XAFDenomination.Note10000: return 10000;
                case XAFDenomination.Note5000: return 5000;
                case XAFDenomination.Note2000: return 2000;
                case XAFDenomination.Note1000: return 1000;
                case XAFDenomination.Note500: return 500;
                case XAFDenomination.Coin500: return 500;
                case XAFDenomination.Coin100: return 100;
                case XAFDenomination.Coin50: return 50;
                case XAFDenomination.Coin25: return 25;
                case XAFDenomination.Coin10: return 10;
                case XAFDenomination.Coin5: return 5;
                case XAFDenomination.Coin1: return 1;
                default: return 0;
            }
        }







        public static decimal CalculateCommission(decimal rate, decimal Fee)
        {
            rate = Math.Max(0, Math.Min(100, rate)); // Ensure rate is within the valid range [0, 100]
            return (rate / 100) * Fee; // Calculate the tax charges based on the percentage
        }

        private static decimal CalculateFlatCharges(decimal flat, decimal amount)
        {
            return flat; // Calculate charges based on flat
        }

        /// <summary>
        /// Calculates the total charges for a customer.
        /// </summary>
        /// <param name="rate">The percentage rate for tax charges.</param>
        /// <param name="chargeRange">The charge range.</param>
        /// <param name="formCharge">The form charge.</param>
        /// <param name="amount">The transaction amount.</param>
        /// <returns>The total charges.</returns>
        public static decimal CalculateCustomerCharges(decimal rate, decimal chargeRange, decimal formCharge, decimal amount, decimal MembershipActivationAmount=0)
        {
            // Ensure rate is within the valid range [0, 100]
            rate = Math.Max(0, Math.Min(100, rate));

            // Calculate the tax charges based on the percentage
            var taxCharges = (rate / 100) * amount;

            // Calculate the total charges
            var totalCharges = chargeRange + taxCharges + formCharge+ MembershipActivationAmount;

            return totalCharges;
        }

        public static decimal ConvertPercentageToCharge(decimal rate, decimal amount)
        {
            // Ensure rate is within the valid range [0, 100]
            rate = Math.Max(0, Math.Min(100, rate));

            // Calculate the tax charges based on the percentage
            var charge = (rate / 100) * amount;

            // Calculate the total charges

            return charge;
        }
        /// <summary>
        /// Calculates the total charges for a customer.
        /// </summary>
        /// <param name="rate">The percentage rate for tax charges.</param>
        /// <param name="chargeRange">The charge range.</param>
        /// <param name="formCharge">The form charge.</param>
        /// <param name="amount">The transaction amount.</param>
        /// <returns>The total charges.</returns>
        public static decimal CalculateCustomerWithdrawalCharges(decimal ServiceCharge, decimal WithrawalFormCharge,bool isMinorAccount)
        {
            var totalCharges = !isMinorAccount? ServiceCharge + WithrawalFormCharge: WithrawalFormCharge;
            return totalCharges;
        }

        public static decimal CollectAndCalculateTotalValue(Dictionary<XAFDenomination, int> mappedDictionary)
        {
            var wallet = new XAFWallet(); // Create a new instance of XAFWallet

            wallet.CollectDenominations(mappedDictionary); // Collect denominations

            return wallet.CalculateTotalValue(); // Calculate total value
        }

    }

}
