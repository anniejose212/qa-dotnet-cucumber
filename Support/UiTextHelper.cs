// FILE: UiTextHelper.cs
// PURPOSE: Utility for normalizing UI text and performing case-insensitive equality checks.

using System;
using System.Net;

namespace qa_dotnet_cucumber.Support
{
    public static class UiTextHelper
    {
        // Normalizes UI text (HTML decode + trim)
        public static string Normalize(string s)
        {
            if (s == null)
                return "";

            return WebUtility.HtmlDecode(s).Trim();
        }

        // Compares two strings after normalization (case-insensitive)
        public static bool EqNorm(string a, string b)
        {
            return string.Equals(Normalize(a), Normalize(b), StringComparison.OrdinalIgnoreCase);
        }
    }
}
