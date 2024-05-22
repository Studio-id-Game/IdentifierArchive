using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StudioIdGames.IdentifierArchiveCore
{
    public partial class Utility
    {

        public static string FixIdentifier(string identifier, int add)
        {
            var regex = FindNumRegex();
            var match = regex.Match(identifier);

            if (match.Success)
            {
                string numberString = match.Value;

                int number = int.Parse(numberString);
                number += add;

                string newNumberString = number.ToString("D5");

                return string.Concat(identifier.AsSpan(0, match.Index), newNumberString);
            }
            else
            {
                return identifier + "_00000";
            }
        }

        [GeneratedRegex(@"(\d+)$")]
        private static partial Regex FindNumRegex();
    }
}