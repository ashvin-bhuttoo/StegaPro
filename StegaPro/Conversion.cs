using System;
using System.Collections;
using System.Linq;
using System.Text;
using LSC.Utils;

namespace StegaPro
{
    /// <summary>
    /// The Conversion static class contains functionality for converting 
    /// to and from various data formats, such as from hexadecimal to binary.
    /// </summary>
    public static class Conversion
    {
        /// <summary>
        /// Method to return Hex String from Bit Array
        /// </summary>
        /// <param name="ba">BitArray to be converted</param>
        /// <returns>string containing hex representation of parameter 'ba'</returns>
        public static string BitArrayToHexString(BitArray ba)
        {
            byte[] bytes;
            ba.nextBytes(out bytes, ba.Length/8);
            return BytesToHexString(bytes);
        }

        /// <summary>
        /// Method to reverse (mirror) either a whole byte array's bits or by each byte individually.
        /// </summary>
        /// <param name="x">byte array to reverse</param>
        /// <param name="whole">specifies whether to reverse te entire bit stream or each byte individually</param>
        /// <returns>byte array containing the reverse of parameter 'x'</returns>
        public static byte[] BinaryReverse(byte[] x, bool whole = true)
        {
            int length = x.Length % 2 == 0 ? x.Length : x.Length + 1;
            int mid = (length / 2);
            byte[] retVal = new byte[length];

            Array.Copy(x, 0, retVal, whole ? (length - x.Length) : 0, x.Length);

            if (whole)
            {
                for (int i = 0; i < mid; i++)
                {
                    int rev0 = (retVal[i] >> 4) | ((retVal[i] & 0xf) << 4);
                    rev0 = ((rev0 & 0xcc) >> 2) | ((rev0 & 0x33) << 2);
                    rev0 = ((rev0 & 0xaa) >> 1) | ((rev0 & 0x55) << 1);

                    int rev1 = (retVal[length - i - 1] >> 4) | ((retVal[length - i - 1] & 0xf) << 4);
                    rev1 = ((rev1 & 0xcc) >> 2) | ((rev1 & 0x33) << 2);
                    rev1 = ((rev1 & 0xaa) >> 1) | ((rev1 & 0x55) << 1);

                    retVal[length - i - 1] = (byte)rev0;
                    retVal[i] = (byte)rev1;
                }
            }
            else
            {
                for (int i = 0; i < x.Length; i++)
                {
                    int rev = (retVal[i] >> 4) | ((retVal[i] & 0xf) << 4);
                    rev = ((rev & 0xcc) >> 2) | ((rev & 0x33) << 2);
                    rev = ((rev & 0xaa) >> 1) | ((rev & 0x55) << 1);
                    retVal[i] = (byte)rev;
                }
            }

            if (x.Length != length)
                Array.Resize(ref retVal, x.Length);

            return retVal;
        }

        /// <summary>
        /// Converts a Hex String to a Binary String.
        /// </summary>
        /// <param name="hexstring">hex string to be converted</param>
        /// <param name="reverse">if true, the output is reversed</param>
        /// <returns>binary string representation of parameter 'hexstring'</returns>
        public static string HexStringToBinaryString(string hexstring, bool reverse = false)
        {
           return reverse ? ReverseStr(string.Join(string.Empty,
              hexstring.Select(
                c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
              )
            )) : string.Join(string.Empty,
              hexstring.Select(
                c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
              )
            );
        }

        /// <summary>
        /// Method to reverse a string 
        /// </summary>
        /// <param name="s">string to reverse</param>
        /// <returns>Reverse of parameter 's'</returns>
        public static string ReverseStr(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        /// <summary>
        /// Converts an Integer to a Binary String, Zero padded if specified with optional parameter pad
        /// </summary>
        /// <param name="_in">integer to be converted to binary string</param>
        /// <param name="pad">if greater than zero, output is left padded with Zeros if pad value greater than output length</param>
        /// <param name="reverse">if true, the output is reversed</param>
        /// <returns>binary string representation of parameter '_in' </returns>
        public static string IntToBinaryString(int _in, int pad = 0, bool reverse = false)
        {
            string bin = Convert.ToString(_in, 2);
            
            if (pad > 0)
            {
                if (bin.Length > pad)
                {
                    throw new Exception("Error: Binary String Length Exceeds Padding Length");
                }
                bin = bin.PadLeft(pad, '0');
            }

            return reverse ? ReverseStr(bin) : bin;
        }

        /// <summary>
        /// Converts a Binary string to hex string
        /// </summary>
        /// <param name="binary">binary string to be converted</param>
        /// <returns>hex string representation of parameter 'binary'</returns>
        public static string BinaryStringToHexString(string binary)
        {
            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);
            
            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }


        /// <summary>
        /// Converts an Ascii string to Hex
        /// </summary>
        /// <param name="Data">ascii string to be converted</param>
        /// <returns>hex string representation of parameter 'Data'</returns>
        public static string StrToHexString(string Data)
        {
            return BytesToHexString(AsciiStrToByteArray(Data));
        }

        /// <summary>
        /// Converts a Hexadecimal String into AscII String format
        /// </summary>
        /// <param name="Data">hex string to be converted</param>
        /// <returns>ascii string representation of parameter 'Data'</returns>
        public static string HexStringToAsc(ref string Data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= Data.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(int.Parse(Data.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// An overload of static string HexStringToAsc(ref string Data)
        /// </summary>
        /// <param name="Data">hex string to be converted</param>
        /// <returns>ascii string representation of parameter 'Data'</returns>
        public static string HexStringToAsc(string Data)
        {
            return HexStringToAsc(ref Data);
        }

        /// <summary>
        /// Converts a byte array into AscII String format..
        /// </summary>
        /// <param name="Data">byte array to be converted</param>
        /// <param name="offset">offset at which ascii chars are to be decoded from parameter 'Data'</param>
        /// <param name="length">length of data to convert from 'Data' starting at 'offset'</param>
        /// <returns></returns>
        public static string ByteArrayToAsc(byte[] Data,int offset,int length)
        {
            StringBuilder sb = new StringBuilder();
            int end = length + offset;
            for (int i= offset;i<end;i++)
            {
                sb.Append((char) Data[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Convert hex string to byte array.
        /// </summary>
        /// <param name="hexStr">Source hex string.</param>
        /// <returns>return byte array.</returns>
        public static byte[] HexStringToBytes(string hexStr)
        {
            hexStr = hexStr.Replace(" ", "").Replace("\r", "").Replace("\n", "").ToUpper((new System.Globalization.CultureInfo("en-US", false)));
            if (hexStr.Length%2 == 1)
                hexStr = "0" + hexStr;

            int i;
            
            if (!Validation.IsHexString(hexStr))
            {
                return null;
            }
        
            int bc = hexStr.Length / 2;
            byte[] retval = new byte[bc];
            for (i = 0; i < bc; i++)
            {
                retval[i] = Convert.ToByte(hexStr.Substring(i*2,2),16);
            }
            return retval;
        }

        /// <summary>
        /// Function to obtain a Hexadecimal string from a Byte array
        /// </summary>
        /// <param name="inBytes">byte array to be converted to hex string</param>
        /// <returns>hex string representation of parameter 'inBytes'</returns>
        public static string BytesToHexString(byte[] inBytes)
        {
            if (inBytes == null) return string.Empty;
            if (inBytes.Length == 0) return string.Empty;
  
            int j;
            string tmpHexOut = "";

            for (j = 0; j < inBytes.Length; j++)
                tmpHexOut = tmpHexOut + inBytes[j].ToString("X2");

            return tmpHexOut;
        }

        /// <summary>
        /// Function to obtain a Hexadecimal string from a Bytes array
        /// </summary>
        /// <param name="inBytes">byte array to be converted to hex string</param>
        /// <param name="offSet">offset at which bytes are to be decoded from parameter 'Data'</param>
        /// <param name="Length">length of data to convert from 'inBytes' starting at 'offset'</param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] inBytes, int offSet, int Length)
        {
            if (inBytes == null) return string.Empty;
            if (inBytes.Length < offSet + Length) return string.Empty;

            int j;
            string tmpHexOut = "";

            for (j = offSet; j < offSet + Length; j++)
                tmpHexOut = tmpHexOut + inBytes[j].ToString("X2");

            return tmpHexOut;
        }

        /// <summary>
        /// Converts a Hexadecimal string value to Byte
        /// </summary>
        /// <param name="HexValue">hex string with value expected between 0x00-0xFF inclusive excluding the '0x'</param>
        /// <returns>byte representation of parameter 'HexValue'</returns>
        public static byte HexToByte(string HexValue)
        {
            return byte.Parse(HexValue, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Converts a Hexadecimal Value to integer
        /// </summary>
        /// <param name="HexValue">hex string with value expected between 0x00-0xFFFFFFFF inclusive exluding the '0x'</param>
        /// <returns>integer representation of parameter 'HexValue'</returns>
        public static int HexToInt(string HexValue)
        {
            return int.Parse(HexValue, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Convert byte array to a <see cref="long"/> integer.
        /// </summary>
        /// <param name="bytes">byte array to be converted to long value, expected length between 1 and 8 bytes</param>
        /// <returns>A <see cref="long"/> integer representation of parameter 'bytes'</returns>
        public static long BytesToLong(byte[] bytes)
        {
            long tempInt = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                tempInt = tempInt << 8 | bytes[i];
            }
            return tempInt;
        }

        /// <summary>
        /// Convert byte array to a <see cref="long"/> integer, overload of 'long BytesToLong(byte[] bytes)'
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long BytesToLong(byte[] bytes,int offset,int length)
        {
            long tempInt = 0;
            for (int i = offset; i < length; i++)
            {
                tempInt = tempInt << 8 | bytes[i];
            }
            return tempInt;
        }
        
        /// <summary>
        /// Convert a ASCII byte array to string, also fielter out the null characters.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            string retval = "";
            if (bytes == null || bytes.Length < 1) return retval;
            char[] cretval = new char[bytes.Length];
            for (int i = 0, j = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != '\0')
                {
                    cretval[j++] = (char)bytes[i];
                }
            }
            retval = new string(cretval);
            retval = retval.TrimEnd('\0');
            return retval;
        }

        /// <summary>
        /// Converts an ASCII string to a byte array
        /// </summary>
        /// <param name="str">Ascii string to be converted</param>
        /// <returns>byte array representation of Ascii string 'str'</returns>
        public static byte[] AsciiStrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Swap the position of each 2 Nibbles in a Hexadecimal String Eg "A0B1" becomes "0A1B"
        /// </summary>
        /// <param name="hexCodedString">Hexx Coded String Value</param>
        /// <param name="stripPadding">If true, removes trailing "F"s in the resulting value</param>
        /// <returns></returns>
        public static string NibbleSwap(string hexCodedString, bool stripPadding)
        {
            int actingLength;
            string retVal = "";

            if ((hexCodedString.Length % 2) != 0) actingLength = hexCodedString.Length - 1;
            else actingLength = hexCodedString.Length;

            for (int i = 0; i < actingLength; i += 2)
            {
                retVal = retVal + hexCodedString.Substring(i + 1, 1) + hexCodedString.Substring(i, 1);
            }

            if (actingLength < hexCodedString.Length) retVal = retVal + "F" + hexCodedString.Substring(actingLength, 1);

            if (stripPadding)
            {
                actingLength = retVal.Length;
                for (int i = retVal.Length - 1; i >= 0; i--)
                {
                    if (retVal.Substring(i, 1) == "F") actingLength--;
                    else break;
                }
                retVal = retVal.Substring(0, actingLength);
            }
            return retVal;
        }
    }
}
