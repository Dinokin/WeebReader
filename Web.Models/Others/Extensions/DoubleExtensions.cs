using System;

namespace WeebReader.Web.Models.Others.Extensions
{
    public static class DoubleExtensions
    {
        public static bool Between(this double? value, double minimum, double maximum)
        {
            value ??= 0;

            return value.Value.Between(minimum, maximum);
        }

        public static bool Between(this double value, double minimum, double maximum)
        {
            if (minimum > maximum)
                throw new ArgumentException($"The argument {nameof(minimum)} must not be greater than the argument {nameof(maximum)}.");

            if (maximum < minimum)
                throw new ArgumentException($"The argument {nameof(maximum)} must not be less than the argument {nameof(minimum)}.");
            
            return value >= minimum && value <= maximum;
        }
    }
}