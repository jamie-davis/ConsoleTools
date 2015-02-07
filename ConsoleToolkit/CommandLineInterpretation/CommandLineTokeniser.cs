using System;
using System.Collections.Generic;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// Emulate the console application command line splitting behaviour.
    /// This is an example:
    /// 
    /// ShowArgs.exe a b c "d with space" "embed \"speech marks\"" "or ""force"" them"
    /// 
    ///   0: a
    ///   1: b
    ///   2: c
    ///   3: d with space
    ///   4: embed "speech marks"
    ///   5: or "force" them
    /// </summary>
    public static class CommandLineTokeniser
    {
        public static string[] Tokenise(string testCommandLine)
        {
            var output = new List<string>();
            var newItem = String.Empty;
            var hasData = false;
            var delimited = false;
            var pos = 0;
            
            Action commit = () =>
            {
                output.Add(newItem);
                newItem = string.Empty;
                delimited = false;
                hasData = false;
            };

            while (pos < testCommandLine.Length)
            {
                var next = testCommandLine[pos++];

                var noData = !delimited && !hasData;

                var isSpace = next == ' ';
                if (isSpace && noData)
                    continue;

                if (isSpace && !delimited)
                {
                    commit();
                    continue;
                }

                var isEscapedSpeechMark = next == '\\' &&  pos < testCommandLine.Length && testCommandLine[pos] == '\"';
                var isSpeechMark = (next == '\"') || isEscapedSpeechMark;
                if (isSpeechMark && !delimited)
                {
                    delimited = true;
                    hasData = true;
                }
                else if (isSpeechMark && delimited)
                {
                    if (pos < testCommandLine.Length && testCommandLine[pos] == '\"')
                    {
                        //forced speech mark
                        newItem += '\"';
                        ++pos;
                    }
                    else
                    {
                        delimited = false;
                        ++pos;
                    }
                }
                else
                {
                    hasData = true;
                    newItem += next;
                }
            }

            if (hasData)
                commit();

            return output.ToArray();
        }
    }
}