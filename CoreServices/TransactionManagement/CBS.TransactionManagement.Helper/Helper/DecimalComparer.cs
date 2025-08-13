using System;

namespace CBS.TransactionManagement.Helper.Helper
{
    public class DecimalComparer
    {
        // Tolerance value for decimal comparisons
        private const decimal Tolerance = 0.0000001m;

        public static bool IsGreaterThan(decimal a, decimal b)
        {
            return (a - b) > Tolerance;
        }

        public static bool IsGreaterThanOrEqual(decimal a, decimal b)
        {
            return (a - b) >= -Tolerance;
        }

        public static bool IsLessThan(decimal a, decimal b)
        {
            return (b - a) > Tolerance;
        }

        public static bool IsLessThanOrEqual(decimal a, decimal b)
        {
            return (b - a) >= -Tolerance;
        }
    }
}
