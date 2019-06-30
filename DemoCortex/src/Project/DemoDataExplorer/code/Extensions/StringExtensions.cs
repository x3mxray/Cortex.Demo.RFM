using System;

namespace Demo.Project.DemoDataExplorer.Extensions
{
    public static class StringExtensions
    {
        public static string TrimStringEnd(this string input, string suffixToRemove, StringComparison comparisonType)
        {
            return input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType)
                ? input.Substring(0, input.Length - suffixToRemove.Length)
                : input;
        }

        public static string CombineUrl(this string baseUrl, string relativeUrl)
        {
            UriBuilder baseUri = new UriBuilder(baseUrl);
            if (Uri.TryCreate(baseUri.Uri, relativeUrl, out var newUri))
                return newUri.ToString();
            else
                throw new ArgumentException("Unable to combine specified url values");
        }
    }
}