using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace com.Sconit.Utility
{
    public class FlatFileReaderHelper
    {
          #region Private variables

        private Stream stream;
        private StreamReader reader;
        private string delimiter = ",";

        #endregion

        public FlatFileReaderHelper(Stream s) : this(s, null, null) { }

        public FlatFileReaderHelper(Stream s, Encoding enc) : this(s, enc, null) { }

        public FlatFileReaderHelper(Stream s, Encoding enc, string delimiter)
        {
            this.stream = s;
            if (!s.CanRead)
            {
                throw new Exception("Could not read the given CSV stream!");
            }
            reader = (enc != null) ? new StreamReader(s, enc) : new StreamReader(s);

            if (delimiter != null && delimiter.Length > 0)
            {
                this.delimiter = delimiter;
            }
        }

        public FlatFileReaderHelper(string filename) : this(filename, null, null) { }

        public FlatFileReaderHelper(string filename, Encoding enc) : this(filename, enc, null) { }

        public FlatFileReaderHelper(string filename, Encoding enc, string delimiter)
            : this(new FileStream(filename, FileMode.Open), enc, delimiter) { }

        public string[] ReadLine()
        {

            string data = reader.ReadLine();
            if (data == null)
            {
                return null;
            }
            if (data.Length == 0 || data.StartsWith("#"))
            {
                return new string[0];
            }
            ArrayList result = new ArrayList();

            ParseFields(result, data);

            return (string[])result.ToArray(typeof(string));
        }

        private void ParseFields(ArrayList result, string data)
        {

            int pos = -1;
            while (pos < data.Length)
                result.Add(ParseField(data, ref pos));
        }

        private string ParseField(string data, ref int startSeparatorPosition)
        {

            if (startSeparatorPosition == data.Length - 1)
            {
                startSeparatorPosition++;
                // The last field is empty
                return "";
            }

            int fromPos = startSeparatorPosition + 1;

            // Determine if this is a quoted field
            if (data[fromPos] == '"')
            {
                // If we're at the end of the string, let's consider this a field that
                // only contains the quote
                if (fromPos == data.Length - 1)
                {
                    fromPos++;
                    return "\"";
                }

                // Otherwise, return a string of appropriate length with double quotes collapsed
                // Note that FSQ returns data.Length if no single quote was found
                int nextSingleQuote = FindSingleQuote(data, fromPos + 1);
                startSeparatorPosition = nextSingleQuote + 1;
                return data.Substring(fromPos + 1, nextSingleQuote - fromPos - 1).Replace("\"\"", "\"");
            }

            // The field ends in the next seperator or EOL
            int nextSeperator = data.IndexOf(delimiter, fromPos);
            if (nextSeperator == -1)
            {
                startSeparatorPosition = data.Length;
                if (data.Substring(fromPos).Trim().Equals("?"))
                    return null;
                return data.Substring(fromPos).Trim();
            }
            else
            {
                startSeparatorPosition = nextSeperator;
                if (data.Substring(fromPos, nextSeperator - fromPos).Trim().Equals("?"))
                    return null;
                return data.Substring(fromPos, nextSeperator - fromPos).Trim();
            }
        }

        private int FindSingleQuote(string data, int startFrom)
        {
            int i = startFrom - 1;
            while (++i < data.Length)
                if (data[i] == '"')
                {
                    // If this is a double quote, bypass the chars
                    if (i < data.Length - 1 && data[i + 1] == '"')
                    {
                        i++;
                        continue;
                    }
                    else
                        return i;
                }
            // If no quote found, return the end value of i (data.Length)
            return i;
        }

        public void Dispose()
        {
            // Closing the reader closes the underlying stream, too
            if (reader != null) reader.Close();
            else if (stream != null)
                stream.Close(); // In case we failed before the reader was constructed
            GC.SuppressFinalize(this);
        }

    }
}
