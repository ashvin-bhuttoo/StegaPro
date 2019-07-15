using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LSC.Utils
{
    /// <summary>
    /// The Encoding static class contains functionality for encoding 
    /// data into different formats such as 7Bit, UCS2 etc.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Check if the source string is a valid hex string.
        /// </summary>
        /// <param name="hexStr">source string.</param>
        /// <returns>true:Valid, false:Invalid.</returns>
        public static bool IsHexString(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
            {
                return true;
            }
            if (hexStr.Length % 2 != 0)
            {
                return false;
            }
            return hexStr.Length <= 0 || Regex.IsMatch(hexStr, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        /// <summary>
        /// Check if the source string is a Numeric string.
        /// </summary>
        /// <param name="hexStr">source string.</param>
        /// <returns>true:Valid, false:Invalid.</returns>
        public static bool IsNumericString(string hexStr)
        {
            foreach (char c in hexStr)
            {
                if (!Char.IsDigit(c)) return false;
            }
            return true;

        }

        /// <summary>
        /// Checks if the Character is a valid Dialing pattern character  0-9, *, #
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDialingChar(char c)
        {
            switch (c)
            { 
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '*':
                case '#':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if the source string is a Valid Dialing string. 0-9, *, #
        /// </summary>
        /// <param name="hexStr">source string.</param>
        /// <returns>true:Valid, false:Invalid.</returns>
        public static bool IsDialingString(string hexStr)
        {
            foreach (char c in hexStr)
            {
                if (!IsDialingChar(c)) return false;
            }
            return true;

        }

        public static bool IsValidIpAddress(string ip)
        {
            return Regex.IsMatch(ip,
                @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }
    }


    /// <summary>
    /// The TextboxValidate static class contains static methods for validation of controls having 'Text' property (i.e. textbox)
    /// </summary>
    public static class TextboxValidate
    {
        //Example usage @ ctlcustom\ctlcellconfig.cs
        public static bool ValidateNumber(Control inputctrl, ErrorProvider epValidation, int minvalue = -1, int maxvalue = -1, bool allowBlanks = false)
        {
            if (string.IsNullOrEmpty(inputctrl.Text) && !allowBlanks)
            {
                epValidation.SetError(inputctrl, "Blank value invalid\n");
                return false;
            }
            else if (string.IsNullOrEmpty(inputctrl.Text) && allowBlanks)
            {
                epValidation.SetError(inputctrl, string.Empty);
                return true;
            }
            else
            {
                if (!inputctrl.Text.All(char.IsDigit))
                {
                    epValidation.SetError(inputctrl, "Invalid Value: " + inputctrl.Text + ", should be positive integer.\n");
                    return false;
                }

                if (allowBlanks && inputctrl.Text.Length == 0) return true;

                uint txtboxval;
                if(uint.TryParse(inputctrl.Text, NumberStyles.Integer, new CultureInfo("en-US"), out txtboxval))
                {
                    if (minvalue > 0 && txtboxval < minvalue)
                    {
                        epValidation.SetError(inputctrl, "Below Minimum value of " + minvalue + "\n");
                        return false;
                    }

                    if (maxvalue > 0 && txtboxval > maxvalue)
                    {
                        epValidation.SetError(inputctrl, "Above Maximum value of " + maxvalue + "\n");
                        return false;
                    }
                }
                else if(maxvalue > 0)
                {
                    epValidation.SetError(inputctrl, "Above Maximum value of " + maxvalue + "\n");
                    return false;
                }
            }

            epValidation.SetError(inputctrl, string.Empty);
            return true;
        }

        public static bool ValidateRegex(Control inputctrl, ErrorProvider epValidation, string regex, string regexError)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(inputctrl.Text, regex))
            {
                epValidation.SetError(inputctrl, regexError.Length > 0 ? regexError.Replace("'","") : "Regex validation failed.\n.");
                return false;
            }
            epValidation.SetError(inputctrl, string.Empty);
            return true;
        }

        public static bool ValidateText(Control inputctrl, ErrorProvider epValidation, int minLength = -1, int maxLength = -1)
        {
            if (string.IsNullOrEmpty(inputctrl.Text) && minLength > 0)
            {
                epValidation.SetError(inputctrl, "Blank value invalid\n");
                return false;
            }

            if (string.IsNullOrEmpty(inputctrl.Text) && minLength <= 0)
            {
                epValidation.SetError(inputctrl, string.Empty);
                return true;
            }

            if (minLength > 0 && inputctrl.Text.Length != minLength && maxLength <= 0) // For Fixed Lengths
            {
                epValidation.SetError(inputctrl, (inputctrl.Text.Length > minLength ? "Above" : "Below") + " Required length of " + minLength + "\n");
                return false;
            }

            if (minLength > 0 && inputctrl.Text.Length < minLength)
            {
                epValidation.SetError(inputctrl, "Below Minimum length of " + minLength + "\n");
                return false;
            }

            if (maxLength > 0 && inputctrl.Text.Length > maxLength)
            {
                epValidation.SetError(inputctrl, "Exceeded Maximum length of " + maxLength + "\n");
                return false;
            }

            epValidation.SetError(inputctrl, string.Empty);
            return true;
        }

        public static bool ValidateValuesInRange(Control inputctrl, ErrorProvider epValidation, string[] range)
        {
            if (range.Count(val => val == inputctrl.Text) <= 0)
            {
                epValidation.SetError(inputctrl, $"Error, valid values are:{Environment.NewLine}{string.Join(Environment.NewLine, range)}\n" );
                return false;
            }

            epValidation.SetError(inputctrl, string.Empty);
            return true;
        }

        public static bool ValidateHexStr(Control inputctrl, ErrorProvider epValidation, string padding, int minLengthBytes = -1, int maxLengthBytes = -1, int minValue = -1, int maxValue = -1)
        {
            if (padding.Length > 0 && maxLengthBytes > 0) // padding code
            {
                inputctrl.Text = inputctrl.Text.TrimEnd(padding.First()).PadRight(maxLengthBytes*2, padding.First());
                ((RichTextBox)inputctrl).Select(inputctrl.Text.TrimEnd(padding.First()).Length, 0);
            }

            if (string.IsNullOrEmpty(inputctrl.Text) && minLengthBytes <= 0)
            {
                epValidation.SetError(inputctrl, string.Empty);
                return true;
            }

            if (inputctrl.Text.Length%2 != 0)
            {
                epValidation.SetError(inputctrl, "Invalid hex string, odd character count.\n");
                return false;
            }

            if (inputctrl.Text.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(inputctrl.Text, @"\A\b[0-9a-fA-F]+\b\Z")) // match chars using regex
            {
                epValidation.SetError(inputctrl, "Invalid hex string.\n");
                return false;
            }

            if (minLengthBytes > maxLengthBytes && maxLengthBytes <= 0 && inputctrl.Text.Length != minLengthBytes * 2) // validate text.len == minlenbytes when maxLenbytes is not passed
            {
                epValidation.SetError(inputctrl, $"{(inputctrl.Text.Length > minLengthBytes * 2 ? "Above maximum" : "Below minimum required")} length of {minLengthBytes} bytes.\n");
                return false;
            }

            if (minLengthBytes > 0 && inputctrl.Text.Length < minLengthBytes * 2)
            {
                epValidation.SetError(inputctrl, "Below minimum length of " + minLengthBytes + (minLengthBytes <= 1 ? " byte.\n" : " bytes.\n"));
                return false;
            }

            if (maxLengthBytes > 0 && inputctrl.Text.Length > maxLengthBytes * 2)
            {
                epValidation.SetError(inputctrl, $"Exceeded maximum length of "+maxLengthBytes+ (maxLengthBytes <= 1 ? " byte.\n" : " bytes.\n"));
                return false;
            }

            if (inputctrl.Text.Length > 0)
            {
                uint txtboxval;
                if (uint.TryParse(inputctrl.Text, NumberStyles.HexNumber, new CultureInfo("en-US"), out txtboxval))
                {
                    if (minValue > 0 && txtboxval < minValue)
                    {
                        epValidation.SetError(inputctrl, $"Below minimum value of {minValue.ToString("X2")}h\n");
                        return false;
                    }

                    if (maxValue > 0 && txtboxval > maxValue)
                    {
                        epValidation.SetError(inputctrl, $"Above maximum value of {maxValue.ToString("X2")}h\n");
                        return false;
                    }
                }
                else if(maxValue > 0)
                {
                    epValidation.SetError(inputctrl, $"Above maximum value of {maxValue.ToString("X2")}h\n");
                    return false;
                }
            }

            epValidation.SetError(inputctrl, string.Empty);
            return true;
        }

        public static bool ValidateHexStr(Control inputctrl, ErrorProvider epValidation, int minLengthBytes = -1, int maxLengthBytes = -1, bool padZero = true)
        {
            if (inputctrl.Text.Length % 2 != 0 && padZero) // check if multiple of 2 chars
            {
                inputctrl.Text = "0" + inputctrl.Text;

                while (inputctrl.Text.StartsWith("00") && inputctrl.Text.Length > 3)
                    inputctrl.Text = inputctrl.Text.Remove(0, 2);
            }

            if (inputctrl.Text.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(inputctrl.Text, @"\A\b[0-9a-fA-F]+\b\Z")) // match chars using regex
            {
                epValidation.SetError(inputctrl, "Invalid hex string.\n");
                return false;
            }

            if (minLengthBytes > maxLengthBytes && maxLengthBytes <= 0 && inputctrl.Text.Length != minLengthBytes * 2) // validate text.len == minlenbytes when maxLenbytes is not passed
            {
                epValidation.SetError(inputctrl, (inputctrl.Text.Length > minLengthBytes * 2 ? "Above maximum" : "Below minimum required") + " length of " + minLengthBytes + " bytes.\n");
                return false;
            }

            if (minLengthBytes > 0 && inputctrl.Text.Length < minLengthBytes * 2)
            {
                epValidation.SetError(inputctrl, "Below minimum length of " + minLengthBytes + (minLengthBytes <= 1 ? " byte.\n" : " bytes.\n"));
                return false;
            }

            if (maxLengthBytes > 0 && inputctrl.Text.Length > maxLengthBytes * 2)
            {
                epValidation.SetError(inputctrl, "Exceeded maximum length of " + maxLengthBytes +(maxLengthBytes<=1?" byte.\n": " bytes.\n"));
                return false;
            }

            epValidation.SetError(inputctrl, string.Empty);
            return true;
        }
    }
}
