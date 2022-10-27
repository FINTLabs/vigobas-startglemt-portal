using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace VigoBAS_Start
{
    public class PhoneNumber : IEquatable<PhoneNumber>
    {
        public const string DefaultCountryCode = "47";

        static string[] TwoDigitCountryCodes = {
            "20", "27",
            "30", "31", "32", "33", "34", "36", "39",
            "40", "41", "43", "44", "45", "46", "47", "48", "49",
            "51", "52", "53", "54", "55", "56", "57", "58",
            "60", "61", "62", "63", "64", "65", "66",
            "81", "82", "83", "84", "86", "89",
            "90", "91", "92", "93", "94", "95"
        };

        public string CountryCode { get; set; }
        
        public string LocalNumber { get; set; }

        public static PhoneNumber Parse(string number)
        {
            number = Regex.Replace(number, @"[^\+\d]", "");

            int localNumberStartIndex = 0;

            if (number.StartsWith("+") || number.StartsWith("00"))
            {
                number = number.TrimStart('0', '+');

                if (number[0] == '1' || number[0] == '7')
                    localNumberStartIndex = 1;
                else if (TwoDigitCountryCodes.Any(cc => number.StartsWith(cc)))
                    localNumberStartIndex = 2;
                else
                    localNumberStartIndex = 3;

                return new PhoneNumber
                {
                    CountryCode = number.Substring(0, localNumberStartIndex),
                    LocalNumber = number.Substring(localNumberStartIndex)
                };
            }

            return new PhoneNumber { LocalNumber = number };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PhoneNumber);
        }

        public bool Equals(PhoneNumber other)
        {
            var cc1 = CountryCode ?? DefaultCountryCode;
            var cc2 = other.CountryCode ?? DefaultCountryCode;

            return other != null &&
                   cc1 == cc2 &&
                   LocalNumber == other.LocalNumber;
        }

        public override int GetHashCode()
        {
            return LocalNumber.GetHashCode();
        }

        public static bool operator ==(PhoneNumber left, PhoneNumber right)
        {
            return EqualityComparer<PhoneNumber>.Default.Equals(left, right);
        }

        public static bool operator !=(PhoneNumber left, PhoneNumber right)
        {
            return !(left == right);
        }
    }
}