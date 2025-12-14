using System.Text;

namespace GDScriptInterface.SourceGenerator;

/// <summary>
/// Provides extension methods for string manipulation, including converting to snake_case and PascalCase.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Converts the input string to snake_case format.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The converted string in snake_case format.</returns>
    public static string ToSnakeCase(this string input)
    {
        StringBuilder sb = new();
        char[] chars = input.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ' ')
            {
                sb.Append('_'); // Replace spaces with underscores
            }
            else if (char.IsUpper(chars[i]))
            {
                // Handle uppercase letters that need an underscore prefix
                if (i + 1 < chars.Length && char.IsLower(chars[i + 1]) &&
                    i > 0 && char.IsUpper(chars[i - 1]))
                {
                    sb.Append('_'); // Add underscore before a single uppercase letter surrounded by uppercase letters
                }
                sb.Append(char.ToLower(chars[i])); // Convert uppercase letter to lowercase
            }
            else if (char.IsLower(chars[i]))
            {
                // Handle lowercase letters that may need an underscore prefix
                if (i > 0 && char.IsDigit(chars[i - 1]) && i + 1 < chars.Length && char.IsLower(chars[i + 1]))
                {
                    sb.Append('_'); // Add underscore between digits and lowercase letters
                }
                sb.Append(chars[i]); // Append the lowercase letter

                if (i + 1 < chars.Length && char.IsUpper(chars[i + 1]))
                {
                    sb.Append('_'); // Add underscore before an uppercase letter following a lowercase letter
                }
            }
            else if (char.IsDigit(chars[i]))
            {
                // Handle digits that may need an underscore prefix
                if (i > 0 && (char.IsUpper(chars[i - 1]) || char.IsLower(chars[i - 1])))
                {
                    sb.Append('_'); // Add underscore between letters and digits
                }
                sb.Append(chars[i]); // Append the digit
            }
            else
            {
                sb.Append(chars[i]); // Append other characters as-is
            }
        }

        return sb.ToString(); // Return the final snake_case string
    }

    /// <summary>
    /// Converts the input string to PascalCase format.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The converted string in PascalCase format.</returns>
    public static string ToPascalCase(this string input)
    {
        StringBuilder sb = new();
        char[] chars = input.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == '_' || chars[i] == ' ')
            {
                continue; // Skip underscores and spaces
            }
            else if (char.IsLower(chars[i]))
            {
                // Capitalize lowercase letters at the start or after underscores/spaces
                if ((i == 0) ||
                    (i > 0 && (chars[i - 1] == '_' || chars[i - 1] == ' ')))
                {
                    sb.Append(char.ToUpper(chars[i])); // Convert to uppercase
                }
                else
                {
                    sb.Append(chars[i]); // Append lowercase letter as-is
                }
            }
            else
            {
                sb.Append(chars[i]); // Append other characters as-is
            }
        }

        return sb.ToString(); // Return the final PascalCase string
    }

}