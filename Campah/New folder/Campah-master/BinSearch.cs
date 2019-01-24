﻿using System;
using System.Text.RegularExpressions;

namespace CampahApp
{
    public class BinSearch
    {
        public static byte Wildcard = 63; //?

        /// <summary>
        /// Searches the buffer for the given hex string and returns the pointer matching the first wildcard location, or the pointer following the pattern if not using wildcards.
        /// Prefix with &lt;&lt; to always return the pointer preceding the match or &gt;&gt; to always return the pointer following (regardless of wildcards)
        /// </summary>
        /// <param name="buffer">The source binary buffer to search within</param>
        /// <param name="signature">A hex string representation of a sequence of bytes to search for</param>
        /// <returns>A pointer at the matching location</returns>
        public static IntPtr FindSignature(byte[] buffer, string signature)
        {
            return FindSignature(buffer, signature, 0);
        }

        /// <summary>
        /// Searches the buffer for the given hex string and returns the pointer matching the first wildcard location, or the pointer following the pattern if not using wildcards.
        /// Prefix with &lt;&lt; to always return the pointer preceding the match or &gt;&gt; to always return the pointer following (regardless of wildcards)
        /// </summary>
        /// <param name="buffer">The source binary buffer to search within</param>
        /// <param name="signature">A hex string representation of a sequence of bytes to search for</param>
        /// <param name="baseAddr">An offset to add to the found pointer VALUE.</param>
        /// <returns>A pointer at the matching location</returns>
        public static IntPtr FindSignature(byte[] buffer, string signature, int baseAddr)
        {
            return FindSignature(buffer, signature, baseAddr, false);
        }
        public static IntPtr FindSignature(byte[] buffer, string signature, int baseAddr, bool readLoc)
        {
            //Since this is a hex string make sure the characters are entered in pairs.
            signature = signature.Replace(" ", "").ToLower();
            if (signature.Length % 2 == 1 || signature.Replace("?", "").Length == 0 || !Regex.IsMatch(signature, @"[0-9a-f/?]*"))
                return IntPtr.Zero;

            //determine if there is a special control character and interpret its meaning
            int control = 0;
            switch (signature[0])
            {
                case '<': //force the pointer location to be read from the bytes preceding the pattern
                    control = 1;
                    signature = signature.Substring(2);
                    break;
                case '>': //force the pointer location to be read from the bytes following the pattern regardless if a wildcard is used
                    control = 2;
                    signature = signature.Substring(2);
                    break;
            }

            //convert the signature text to a binary array
            byte[] pattern = SigToByte(signature, Wildcard);
            if (pattern != null)
            {
                //Find the start index of the first wildcard. if no wildcards then the bytes following the match
                int pos;
                for (pos = 0; pos < pattern.Length; pos++)
                {
                    if (pattern[pos] == Wildcard)
                        break;
                }

                //Search for the pattern in the buffer. Convert the bytes to an int and return as a pointer
                int idx = pos == pattern.Length ? Horspool(buffer, pattern) : Bndm(buffer, pattern, Wildcard);

                //if the sig was not found then exit
                if (idx < 0)
                {
                    return IntPtr.Zero;
                }

                //Grab the 4 byte pointer at the location requested
                switch (control)
                {
                    case 1: //<<
                        //always grab the pointer in front of the sig
                        if (!readLoc)
                            return (IntPtr)(idx - 4 + baseAddr);
                        return (IntPtr)(BitConverter.ToInt32(buffer, idx - 4));
                    case 2: //>>
                        //always grab the pointer following the sig
                        if (!readLoc)
                            return (IntPtr)(idx + pattern.Length + baseAddr);
                        return (IntPtr)(BitConverter.ToInt32(buffer, idx + pattern.Length));

                    default:
                        //always pointer starting at the first wildcard. if no wildcard is being used, then the rear
                        if (!readLoc)
                            return (IntPtr)(idx + pos + baseAddr);
                        return (IntPtr)(BitConverter.ToInt32(buffer, idx + pos));
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>Backward Nondeterministic Dawg Matching search algorithm</summary>
        /// <param name="buffer">The haystack to search within</param>
        /// <param name="pattern">The needle to locate</param>
        /// <param name="wildcard">The byte to treat as a wildcard character. Note that this only matches one char for one char and does not expand.</param>
        /// <returns>The index the pattern was found at, or -1 if not found</returns>
        public static int Bndm(byte[] buffer, byte[] pattern, byte wildcard)
        {
            //This code is based on: 
            //   http://johannburkard.de/software/stringsearch/
            //   http://www-igm.univ-mlv.fr/~lecroq/string/bndm.html

            int end = pattern.Length < 32 ? pattern.Length : 32;
            var b = new int[256];

            //Pre-process
            int j = 0;
            for (int i = 0; i < end; ++i)
            {
                if (pattern[i] == wildcard)
                {
                    j |= (1 << end - i - 1);
                }
            }
            if (j != 0)
            {
                for (int i = 0; i < b.Length; i++)
                {
                    b[i] = j;
                }
            }
            j = 1;
            for (int i = end - 1; i >= 0; --i, j <<= 1)
            {
                b[pattern[i]] |= j;
            }

            //Perform search
            int pos = 0;

            while (pos <= buffer.Length - pattern.Length)
            {
                j = pattern.Length - 1;
                int last = pattern.Length;
                int d = -1;
                while (d != 0)
                {
                    d &= b[buffer[pos + j]];
                    if (d != 0)
                    {
                        if (j == 0)
                        {
                            return pos;
                        }
                        last = j;
                    }
                    --j;
                    d <<= 1;
                }
                pos += last;
            }
            return -1;
        }

        /// <summary>Boyer-Moore-Horspool search algorithm</summary>
        /// <param name="buffer">The haystack to search within</param>
        /// <param name="pattern">The needle to locate</param>
        /// <returns>The index the pattern was found at, or -1 if not found</returns>
        public static int Horspool(byte[] buffer, byte[] pattern)
        {
            //Based on: http://www-igm.univ-mlv.fr/~lecroq/string/node18.html

            var bcs = new int[256];
            int scan;

            //Build the Bad Char Skip table
            for (scan = 0; scan < 256; scan = scan + 1)
            {
                bcs[scan] = pattern.Length;
            }
            int last = pattern.Length - 1;
            for (scan = 0; scan < last; scan = scan + 1)
            {
                bcs[pattern[scan]] = last - scan;
            }

            //perform string matching
            int hidx = 0;
            int hlen = buffer.Length;
            int nlen = pattern.Length;

            while (hidx <= hlen - nlen)
            {
                for (scan = last; buffer[hidx + scan] == pattern[scan]; scan = scan - 1)
                {
                    if (scan == 0)
                        return hidx;
                }
                hidx += bcs[buffer[hidx + last]];
            }
            return -1;
        }

        /// <summary>
        /// Convert a hex string to a binary array while preserving any wildcard characters.
        /// </summary>
        /// <param name="signature">A hex string "signature"</param>
        /// <param name="wildcard">The byte to treat as the wildcard</param>
        /// <returns>The converted binary array. Null if the conversion failed.</returns>
        public static byte[] SigToByte(string signature, byte wildcard)
        {
            var pattern = new byte[signature.Length / 2];
            var hexTable = new[] { 0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,
                                   0x08,0x09,0x00,0x00,0x00,0x00,0x00,0x00,
                                   0x00,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F };

            try
            {
                for (int x = 0, i = 0; i < signature.Length; i += 2, x += 1)
                {
                    if (signature[i] == wildcard)
                    {
                        pattern[x] = wildcard;
                    }
                    else
                    {
                        pattern[x] = (byte)(hexTable[Char.ToUpper(signature[i]) - '0'] << 4 | hexTable[Char.ToUpper(signature[i + 1]) - '0']);
                    }
                }
                return pattern;
            }
            catch
            {
                return null;
            }
        }
    }
}
