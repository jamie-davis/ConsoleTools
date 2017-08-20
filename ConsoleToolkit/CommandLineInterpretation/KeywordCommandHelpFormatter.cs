using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal static class KeywordCommandHelpFormatter
    {
        public static List<CommandDescription> FormatKeywordList(IEnumerable<KeywordCommand> keywordCommands)
        {
            var workList = keywordCommands.ToList();

            var result = new List<CommandDescription>();

            var index = 0;
            while (workList.Any())
            {
                var roots = workList.Where(kc => kc.Keywords.Count > index).Select(kc => new {Root = kc.Keywords[index], KeywordCommand = kc}).ToList();
                foreach (var root in roots.Select(r => r.Root).Distinct())
                {
                    var allRoot = roots.Where(r => r.Root == root).ToList();
                    var end = allRoot.Any(ar => ar.KeywordCommand.Keywords.Count == index + 1);
                    var mismatch = allRoot.Where(ar => ar.KeywordCommand.Keywords.Count > index + 1).Select(ar => ar.KeywordCommand.Keywords[index + 1]).Distinct().Count() > 1;
                    if (end || mismatch)
                    {
                        foreach (var endedRoot in allRoot)
                            workList.Remove(endedRoot.KeywordCommand);
                        var keywords = allRoot[0].KeywordCommand.Keywords.Take(index + 1).ToList();
                        var command = string.Join(" ", keywords);
                        var doc = allRoot.SelectMany(r => r.KeywordCommand.KeywordSets)
                            .FirstOrDefault(s => s.Keywords.SequenceEqual(keywords) && s.Description != null);
                        result.Add(new CommandDescription { Command = command, Text = doc == null ? string.Empty : (doc.Description ?? string.Empty) });
                    }
                }

                ++index;
            }
            return result;
        }
    }

    internal class CommandDescription
    {
        public string Command { get; set; }

        public string Text { get; set; }
    }

    internal class KeywordCommand
    {
        public List<string> Keywords { get; }
        public List<KeywordsDesc> KeywordSets { get; }
        public string CommandName { get; }

        public KeywordCommand(List<string> keywords, string commandName, List<KeywordsDesc> keywordSets)
        {
            Keywords = keywords;
            KeywordSets = keywordSets;
            CommandName = commandName;
        }
    }
}