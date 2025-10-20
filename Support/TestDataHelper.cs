// FILE: TestDataHelper.cs
// PURPOSE: Provides decoding of test placeholders (e.g., {DQ}, {EQ:n}) into actual characters for input validation/XSS scenarios.

public static class TestDataHelper
{
    public static string NormalizeTestInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        // Replace {DQ} → double quotes
        input = input.Replace("{DQ}", "\"");

        // Replace {EQ:n} → n '=' characters
        if (input.Contains("{EQ:"))
        {
            int start = input.IndexOf("{EQ:");
            int end = input.IndexOf("}", start);

            if (start != -1 && end != -1)
            {
                string token = input.Substring(start, end - start + 1);
                string num = token.Replace("{EQ:", "").Replace("}", "");

                if (int.TryParse(num, out int count))
                    input = input.Replace(token, new string('=', count));
            }
        }

        return input.Trim();
    }
}
