using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sendsafely.Api.Utilities
{
    /// <summary>
    /// Utility for extracting a list of valid SendSafely package links from text input.
    /// </summary>
    public class ParseLinksUtility
    {
        private const string Regex = "(https:\\/\\/[a-zA-Z\\.-]+\\/receive\\/\\?[A-Za-z0-9&=\\-]+packageCode=[A-Za-z0-9\\-_]+#keyCode=[A-Za-z0-9\\-_]+)";

        /// <summary>
        /// Extract a list of valid SendSafely package links from text.
        /// </summary>
        /// <param name="text"> The text input that you want to check for package links.</param>
        public List<string> Parse(string text)
        {
            var links = new List<string>();

            var rgx = new Regex(Regex, RegexOptions.IgnoreCase);
            var matches = rgx.Matches(text);
            if (matches.Count > 0)
            {
                links.AddRange(from Match match in matches select match.Value);
            }

            return links;
        }
    }
}