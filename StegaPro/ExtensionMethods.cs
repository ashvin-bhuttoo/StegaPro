using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace StegaPro
{
    /// <summary>
    /// Extension methods for Byte arrays
    /// </summary>
    public static class ByteArrayExtensionMethods
    {
        /// <summary>
        /// Extension method to append a byte array
        /// </summary>
        /// <param name="curr">current byte array</param>
        /// <param name="bytesToAppend">bytes to append</param>
        /// <returns>resulting byte array with 'bytesToAppend' appended to 'curr'</returns>
        public static byte[] Append(this byte[] curr, byte[] bytesToAppend)
        {
            if (bytesToAppend.Length > 0)
            {
                int currLen = curr.Length;
                Array.Resize(ref curr, curr.Length + bytesToAppend.Length);
                Array.Copy(bytesToAppend, 0, curr, currLen, bytesToAppend.Length);
            }
            return curr;
        }

        /// <summary>
        /// Extension method to append a byte
        /// </summary>
        /// <param name="curr">current byte array</param>
        /// <param name="byteToAppend">byte to append</param>
        /// <returns>resulting byte array with 'byteToAppend' appended to 'curr'</returns>
        public static byte[] Append(this byte[] curr, byte byteToAppend)
        {
            Array.Resize(ref curr, curr.Length + 1);
            curr[curr.Length - 1] = byteToAppend;
            return curr;
        }

        /// <summary>
        /// Extension method to append a bitarray, only 8 bit multiples are supported, use BitArray::PadRemaining() to pad missing bits if needed
        /// </summary>
        /// <param name="curr">current byte array</param>
        /// <param name="bitsToAppend">bitarray to append</param>
        /// <returns>resulting byte array with 'bitsToAppend' appended to 'curr'</returns>
        public static byte[] Append(this byte[] curr, BitArray bitsToAppend)
        {
            if (bitsToAppend.Length % 8 != 0)
                throw new Exception("Only 8 bit multiples supported");

            byte[] toAppendBytes = new byte[bitsToAppend.Length / 8];
            bitsToAppend.CopyTo(toAppendBytes, 0);
            toAppendBytes = Conversion.BinaryReverse(toAppendBytes, false);

            curr = curr.Append(toAppendBytes);
            return curr;
        }

        /// <summary>
        /// Extension method to append an integer using a specified number of bytes
        /// </summary>
        /// <param name="curr">current byte array</param>
        /// <param name="intToAppend">the integer to append</param>
        /// <param name="numBytesToAppend">number of bytes to generate from integer</param>
        /// <returns>resulting byte array with 'intToAppend' appended to 'curr'</returns>
        public static byte[] Append(this byte[] curr, int intToAppend, int numBytesToAppend = 1)
        {
            curr = curr.Append(Conversion.HexStringToBytes(intToAppend.ToString($"X{numBytesToAppend * 2}")));
            return curr;
        }

        /// <summary>
        /// Entension method to print out the contents of a byte array (for debugging purposes)
        /// </summary>
        /// <param name="curr">current byte array</param>
        public static void PrintToStdOut(this byte[] curr)
        {
            foreach (byte b in curr)
            {
                Console.Write(Conversion.IntToBinaryString(b, 8) + $"({((int)b).ToString("X2")}) ");
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Extension methods for Bit arrays
    /// </summary>
    public static class BitArrayExtensionMethods
    {
        /// <summary>
        /// Extension method to extract specified number of bits from current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="bitsOut">output parameter for bit extraction</param>
        /// <param name="numBits">number of bits to extract</param>
        /// <returns>bitarray with extracted bits removed</returns>
        public static BitArray nextBits(this BitArray current, out BitArray bitsOut, int numBits)
        {
            bool[] bools = new bool[current.Count];
            current.CopyTo(bools, 0);

            if (bools.Length < numBits)
            {
                throw new Exception("Not enough bits in bitarray");
            }

            bool[] boolsOut = new bool[numBits];
            for (int j = 0, k = numBits - 1; k >= 0; k--)
                boolsOut[j++] = bools[k];

            Array.Reverse(boolsOut);
            bitsOut = new BitArray(boolsOut);

            bools = bools.RemoveAt(0, numBits);
            current = new BitArray(bools);
            return current;
        }

        /// <summary>
        /// Extension method to scan an integer from current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="intOut">output parameter for integer extraction</param>
        /// <param name="numBits">number of bits to scan for integer</param>
        /// <returns>bitarray with scanned bits removed</returns>
        public static BitArray nextInt(this BitArray current, out int intOut, int numBits)
        {
            bool[] bools = new bool[current.Count];
            current.CopyTo(bools, 0);

            if (numBits > 32)
            {
                throw new Exception("Integer cannot be longer than 32 bits");
            }
            if (bools.Length < numBits)
            {
                throw new Exception("Not enough bits in bitarray");
            }

            BitArray scanInt = new BitArray(0);
            for (int k = numBits - 1; k >= 0; k--)
                scanInt = scanInt.Append(bools[k]);

            int[] _intOut = new int[1];
            scanInt.CopyTo(_intOut, 0);
            intOut = _intOut[0];

            bools = bools.RemoveAt(0, numBits);
            current = new BitArray(bools);
            return current;
        }

        /// <summary>
        /// Extension method to scan a byte array from current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="bytesOut">output parameter for byte array extraction</param>
        /// <param name="numBytes">number of bytes to scan from bitarray</param>
        /// <returns>bitarray with scanned bits removed</returns>
        public static BitArray nextBytes(this BitArray current, out byte[] bytesOut, int numBytes)
        {
            bool[] bools = new bool[current.Count];
            current.CopyTo(bools, 0);

            if (bools.Length < numBytes * 8)
            {
                throw new Exception("Not enough bits in bitarray");
            }

            BitArray scanBytes = new BitArray(0);
            for (int k = numBytes * 8 - 1; k >= 0; k--)
                scanBytes = scanBytes.Append(bools[k]);

            byte[] _bytesOut = new byte[numBytes];
            scanBytes.CopyTo(_bytesOut, 0);
            Array.Reverse(_bytesOut, 0, _bytesOut.Length);
            bytesOut = _bytesOut;

            bools = bools.RemoveAt(0, numBytes * 8);
            current = new BitArray(bools);
            return current;
        }

        /// <summary>
        /// Extension method to scan a byte from current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="byteOut">output parameter for byte extraction</param>
        /// <returns>bitarray with scanned bits removed</returns>
        public static BitArray nextByte(this BitArray current, out byte byteOut)
        {
            bool[] bools = new bool[current.Count];
            current.CopyTo(bools, 0);

            if (bools.Length < 8)
            {
                throw new Exception("Not enough bits in bitarray");
            }

            BitArray scanByte = new BitArray(0);
            for (int k = 8 - 1; k >= 0; k--)
                scanByte = scanByte.Append(bools[k]);

            byte[] _byteOut = new byte[1];
            scanByte.CopyTo(_byteOut, 0);
            byteOut = _byteOut[0];

            bools = bools.RemoveAt(0, 8);
            current = new BitArray(bools);
            return current;
        }

        /// <summary>
        /// Extension method to scan a bit from current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="boolOut">output parameter for bit extraction</param>
        /// <returns>bitarray with scanned bit removed</returns>
        public static BitArray nextBit(this BitArray current, out bool boolOut)
        {
            bool[] bools = new bool[current.Count];
            current.CopyTo(bools, 0);

            if (bools.Length < 1)
            {
                throw new Exception("Not enough bits in bitarray");
            }

            boolOut = bools[0];
            bools = bools.RemoveAt(0, 1);
            current = new BitArray(bools);
            return current;
        }

        /// <summary>
        /// Extension method to Prepend a bitarray to current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="before">bitarray to be prepended to current bit array</param>
        /// <returns>bitarray with current bitarray appended to bitarray representation of 'before'</returns>
        public static BitArray Prepend(this BitArray current, BitArray before)
        {
            var bools = new bool[current.Count + before.Count];
            before.CopyTo(bools, 0);
            current.CopyTo(bools, before.Count);
            return new BitArray(bools);
        }

        /// <summary>
        /// Extension method to prepend a boolean array to current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="before">boolean array to prepend </param>
        /// <returns>bitarray with current bitarray appended to bitarray representation of 'before'</returns>
        public static BitArray Prepend(this BitArray current, bool[] before)
        {
            var bools = new bool[current.Count + before.Length];
            before.CopyTo(bools, 0);
            current.CopyTo(bools, before.Length);
            return new BitArray(bools);
        }

        /// <summary>
        /// Extension method to prepend a boolean value to current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="before">boolean value to prepend</param>
        /// <returns>bitarray with current bitarray appended to bitarray representation of 'before'</returns>
        public static BitArray Prepend(this BitArray current, bool before)
        {
            var bools = new bool[current.Count + 1];
            bools[0] = before;
            current.CopyTo(bools, 1);
            return new BitArray(bools);
        }

        /// <summary>
        /// Extension method to prepend an integer to current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="before">integer value to prepend</param>
        /// <param name="maxbits">the number of bits to generate form parameter 'before'</param>
        /// <returns>bitarray with current bitarray appended to bitarray representation of 'before'</returns>
        public static BitArray Prepend(this BitArray current, int before, int maxbits = 0)
        {
            BitArray b = new BitArray(new[] { before });
            if (maxbits > 0)
            {
                bool[] subBools = new bool[maxbits];
                for (int k = 0; k < maxbits; k++)
                {
                    subBools[maxbits -1 -k] = b[k];
                }
                b = new BitArray(subBools);
            }
            var bools = new bool[current.Count + b.Length];
            b.CopyTo(bools, 0);
            current.CopyTo(bools, b.Length);
            return new BitArray(bools);
        }

        /// <summary>
        /// Extension method to prepend a byte array to current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="before">byte array to prepend</param>
        /// <returns>bitarray with current bitarray appended to bitarray representation of 'before'</returns>
        public static BitArray Prepend(this BitArray current, byte[] before)
        {
            BitArray b = new BitArray(0);
            b = before.Aggregate(b, (current1, _byte) => current1.Append(_byte, 8));
            var bools = new bool[current.Count + b.Length];
            b.CopyTo(bools, 0);
            current.CopyTo(bools, b.Length);
            return new BitArray(bools);
        }

        /// <summary>
        /// Extension method to prepend a byte to current bitarray
        /// </summary>
        /// <param name="current">current bit array</param>
        /// <param name="before">byte to prepend</param>
        /// <returns>bitarray with current bitarray appended to bitarray representation of 'before'</returns>
        public static BitArray Prepend(this BitArray current, byte before)
        {
            BitArray b = new BitArray(new[] { before });
            var bools = new bool[b.Length];
            b.CopyTo(bools, 0);
            Array.Reverse(bools);
            Array.Resize(ref bools, current.Count + b.Length);
            current.CopyTo(bools, 8);
            return new BitArray(bools);
        }

        /// <summary>
        /// Extension method to append a bitarray to current bitarray
        /// </summary>
        /// <param name="current">current bitarray</param>
        /// <param name="after">bitarray to append</param>
        /// <returns>bitarray with bitarray representation of 'after' appended to current bitarray</returns>
        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        /// <summary>
        /// Extension method to append a bitarray to current bitarray
        /// </summary>
        /// <param name="current">current bitarray</param>
        /// <param name="after">bitarray to append</param>
        /// <returns>bitarray with bitarray representation of 'after' appended to current bitarray</returns>
        public static BitArray Append(this BitArray current, bool[] after)
        {
            var bools = new bool[current.Count + after.Length];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        public static BitArray Append(this BitArray current, bool after)
        {
            var bools = new bool[current.Count + 1];
            current.CopyTo(bools, 0);
            bools[current.Count] = after;
            return new BitArray(bools);
        }

        public static BitArray Append(this BitArray current, int after, int maxbits = 0)
        {
            BitArray b = new BitArray(new[] { after });
            if (maxbits > 0)
            {
                bool[] subBools = new bool[maxbits];
                for (int k = 0; k < maxbits; k++)
                {
                    subBools[maxbits - 1 - k] = b[k];
                }
                b = new BitArray(subBools);
            }
            var bools = new bool[current.Count + b.Length];
            current.CopyTo(bools, 0);
            b.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }


        public static BitArray Append(this BitArray current, byte[] after)
        {
            BitArray b = new BitArray(0);
            b = after.Aggregate(b, (current1, _byte) => current1.Append(_byte, 8));
            var bools = new bool[current.Count + b.Length];
            current.CopyTo(bools, 0);
            b.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        public static BitArray Append(this BitArray current, byte after)
        {
            BitArray b = new BitArray(new[] { after });
            var bools = new bool[current.Count + b.Length];
            b.CopyTo(bools, 0);
            Array.Reverse(bools);
            current.CopyTo(bools, 0);
            return new BitArray(bools);
        }

        public static BitArray PadRemaining(this BitArray current, bool padBit, int len = 0)
        {
            if (len > 0)
            {
                while(current.Length != len)
                    current = current.Append(padBit);
            }
            else
                while (current.Length%8 != 0)
                {
                    current = current.Append(padBit);
                }
            return current;
        }

        public static void PrintToStdOut(this BitArray current)
        {
            byte[] bytesOut;
            if (current.Length % 8 != 0)
            {
                Console.WriteLine($"BitArray length is not an 8bit multiple, {8 - current.Length % 8} zeros were padded.");
                current = current.PadRemaining(false);
            }
            current.nextBytes(out bytesOut, current.Length / 8);
            foreach (byte b in bytesOut)
            {
                Console.Write(Conversion.IntToBinaryString(b, 8) + $"({((int)b).ToString("X2")}) ");
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Extension methods for Arrays
    /// </summary>
    public static class ArrayExtensions
    {

        /// <summary>
        /// Generic method for Removing element(s) from array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Current array for which element(s) are to be removed</param>
        /// <param name="startIndex">the starting index for element removal</param>
        /// <param name="length">the number of elements to remove from array starting from index 'startIndex'</param>
        /// <returns>An Array with elements removed base in parameters 'startIndex', 'length'</returns>
        public static T[] RemoveAt<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length < 0)
            {
                startIndex += 1 + length;
                length = -length;
            }

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            T[] newArray = new T[array.Length - length];

            Array.Copy(array, 0, newArray, 0, startIndex);
            Array.Copy(array, startIndex + length, newArray, startIndex, array.Length - startIndex - length);

            return newArray;
        }
    }


    /// <summary>
    /// Extension methods for Collections
    /// </summary>
    public static class CollectionExtensions
    {
        public static void ReplaceItem<T>(this Collection<T> col, Func<T, bool> match, T newItem)
        {
            var oldItem = col.FirstOrDefault(i => match(i));
            var oldIndex = col.IndexOf(oldItem);
            col[oldIndex] = newItem;
        }
    }
}
