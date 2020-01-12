using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SLMPLauncher
{
    public class FuncParser
    {
        static List<string> cacheFile = new List<string>();
        static int startIndex = -1;
        static int enbIndex = -1;
        static int lineIndex = -1;
        static bool blockClearKE = false;
        static bool blockClearSR = false;
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static bool keyExists(string path, string section, string key)
        {
            startIndex = -1;
            enbIndex = -1;
            lineIndex = -1;
            bool findSection = false;
            bool findKey = false;
            if (File.Exists(path))
            {
                cacheFile.AddRange(File.ReadAllLines(path));
                for (int i = 0; i < cacheFile.Count; i++)
                {
                    if (!findSection && cacheFile[i].Equals("[" + section + "]", StringComparison.OrdinalIgnoreCase))
                    {
                        findSection = true;
                        startIndex = i;
                        enbIndex = i;
                    }
                    else if (findSection && cacheFile[i].StartsWith("[") && cacheFile[i].EndsWith("]"))
                    {
                        break;
                    }
                    else if (findSection && cacheFile[i].Length != 0)
                    {
                        if (cacheFile[i].StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                        {
                            findKey = true;
                            lineIndex = i;
                            break;
                        }
                        else
                        {
                            enbIndex = i;
                        }
                    }
                }
            }
            if (blockClearKE)
            {
                blockClearKE = false;
            }
            else
            {
                cacheFile.Clear();
            }
            return findKey;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static string stringRead(string path, string section, string key)
        {
            string outString = null;
            blockClearKE = true;
            if (keyExists(path, section, key))
            {
                outString = cacheFile[lineIndex].Remove(0, (key + "=").Length);
                if (outString.Length == 0)
                {
                    outString = null;
                }
            }
            if (blockClearSR)
            {
                blockClearSR = false;
            }
            else
            {
                cacheFile.Clear();
            }
            return outString;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static void iniWrite(string path, string section, string key, string value)
        {
            bool readyToWrite = false;
            blockClearSR = true;
            string line = stringRead(path, section, key);
            if (lineIndex != -1)
            {
                if (value != null && value.Length == 0)
                {
                    value = null;
                }
                if (!string.Equals(line, value, StringComparison.OrdinalIgnoreCase))
                {
                    cacheFile[lineIndex] = key + "=" + value;
                    readyToWrite = true;
                }
            }
            else
            {
                if (startIndex != -1 && enbIndex != -1)
                {
                    cacheFile[enbIndex] += Environment.NewLine + key + "=" + value;
                    readyToWrite = true;
                }
                else if (File.Exists(path))
                {
                    FuncMisc.appendToFile(path, Environment.NewLine + "[" + section + "]" + Environment.NewLine + key + "=" + value);
                }
            }
            if (readyToWrite)
            {
                FuncMisc.writeToFile(path, cacheFile);
            }
            cacheFile.Clear();
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static int intRead(string path, string section, string key)
        {
            int value = -1;
            string line = stringRead(path, section, key);
            if (!string.IsNullOrEmpty(line))
            {
                value = stringToInt(line);
            }
            return value;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static double doubleRead(string path, string section, string key)
        {
            double value = -1.0;
            string line = stringRead(path, section, key);
            if (!string.IsNullOrEmpty(line))
            {
                value = stringToDouble(line);
            }
            return value;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static bool readAsBool(string path, string section, string key)
        {
            string line = stringRead(path, section, key);
            return line != null && (line == "1" || string.Equals(line, "true", StringComparison.OrdinalIgnoreCase));
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static int stringToInt(string input)
        {
            int value = -1;
            if (!string.IsNullOrEmpty(input))
            {
                if (input.Contains("."))
                {
                    Int32.TryParse(input.Remove(input.IndexOf('.')), out value);
                }
                else if (input.Contains(","))
                {
                    Int32.TryParse(input.Remove(input.IndexOf(',')), out value);
                }
                else
                {
                    Int32.TryParse(input, out value);
                }
            }
            return value;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static double stringToDouble(string input)
        {
            double value = -1;
            if (!string.IsNullOrEmpty(input))
            {
                Double.TryParse(input.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value);
            }
            return value;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static string convertFileSize(double input)
        {
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            while (input >= mod)
            {
                input /= mod;
                i++;
            }
            return Math.Round(input, 2) + units[i];
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static List<string> parserESPESM(string file)
        {
            List<string> outString = new List<string>();
            if (File.Exists(file) && new FileInfo(file).Length > 50)
            {
                try
                {
                    FileStream fs = File.OpenRead(file);
                    byte[] bytesFile = new byte[8];
                    fs.Read(bytesFile, 0, 8);
                    if (equalsByteArrays(new byte[] { bytesFile[0], bytesFile[1], bytesFile[2], bytesFile[3] }, new byte[] { 84, 69, 83, 52 }))
                    {
                        int length = BitConverter.ToInt32(bytesFile, 4);
                        bytesFile = new byte[length];
                        fs.Seek(28, SeekOrigin.Begin);
                        fs.Read(bytesFile, 0, length);
                        fs.Close();
                        int jump = 2 + BitConverter.ToInt16(bytesFile, 0);
                        if (equalsByteArrays(new byte[] { bytesFile[jump], bytesFile[jump + 1], bytesFile[jump + 2], bytesFile[jump + 3] }, new byte[] { 79, 70, 83, 84 }))
                        {
                            jump += 6 + BitConverter.ToInt16(bytesFile, jump + 4);
                        }
                        if (equalsByteArrays(new byte[] { bytesFile[jump], bytesFile[jump + 1], bytesFile[jump + 2], bytesFile[jump + 3] }, new byte[] { 68, 69, 76, 69 }))
                        {
                            jump += 6 + BitConverter.ToInt16(bytesFile, jump + 4);
                        }
                        if (equalsByteArrays(new byte[] { bytesFile[jump], bytesFile[jump + 1], bytesFile[jump + 2], bytesFile[jump + 3] }, new byte[] { 67, 78, 65, 77 }))
                        {
                            jump += 6 + BitConverter.ToInt16(bytesFile, jump + 4);
                        }
                        if (equalsByteArrays(new byte[] { bytesFile[jump], bytesFile[jump + 1], bytesFile[jump + 2], bytesFile[jump + 3] }, new byte[] { 83, 78, 65, 77 }))
                        {
                            jump += 6 + BitConverter.ToInt16(bytesFile, jump + 4);
                        }
                        for (int i = jump; i < bytesFile.Length; i++)
                        {
                            if (equalsByteArrays(new byte[] { bytesFile[i], bytesFile[i + 1], bytesFile[i + 2], bytesFile[i + 3] }, new byte[] { 77, 65, 83, 84 }))
                            {
                                jump = ((bytesFile[i + 5] << 8) | bytesFile[i + 4]) - 1;
                                byte[] bytesText = new byte[jump];
                                Buffer.BlockCopy(bytesFile, i + 6, bytesText, 0, jump);
                                outString.Add(Encoding.UTF8.GetString(bytesText));
                                i += jump + 12 + ((bytesFile[i + jump + 12] << 8) | bytesFile[i + jump + 11]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        fs.Close();
                        outString.Clear();
                        outString.Add("ERROR FILE READ");
                    }
                    bytesFile = null;
                }
                catch
                {
                    outString.Clear();
                    outString.Add("FILE READ ERROR");
                }
            }
            return outString;
        }
        private static bool equalsByteArrays(byte[] ba1, byte[] ba2)
        {
            for (int i = 0; i < ba1.Length; i++)
            {
                if (ba1[i] != ba2[i])
                {
                    return false;
                }
            }
            return true;
        }
        // ------------------------------------------------ BORDER OF FUNCTION ---------------------------------------------------------- //
        public static bool checkESM(string file)
        {
            if (File.Exists(file) && new FileInfo(file).Length > 50)
            {
                try
                {
                    FileStream fs = File.OpenRead(file);
                    fs.Seek(8, SeekOrigin.Begin);
                    int read = fs.ReadByte();
                    fs.Close();
                    return read == 1 || read == 129;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}