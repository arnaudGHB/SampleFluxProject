namespace CBS.NLoan.Helper.Helper
{
    public class Converter
    {
        public static double toDouble(string value)
        {
            if (value == null || value.Equals(""))
            {
                return 0.0;
            }

            return Convert.ToDouble(value);
        } 
        
        public static decimal toDecimal(string value)
        {
            if (value == null || value.Equals(""))
            {
                return 0;
            }

            return Convert.ToDecimal(value);
        }
        
        public static decimal toDecimal(double value)
        {
          /*  if (value == null || value.Equals(""))
            {
                return 0;
            }*/

            return Convert.ToDecimal(value);
        }

        public static int toInt(string value)
        {
            if (value == null || value.Equals(""))
            {
                return 0;
            }

            return Convert.ToInt32(value);
        }

        public static decimal roundUp(decimal value)
        {
           return Math.Ceiling(value);

        }
    }
}
