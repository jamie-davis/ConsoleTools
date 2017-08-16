using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal static class CommandSelector
    {
        public static SelectionResult Select(string[] args, List<ICommandKeys> keys)
        {
            var failures = new List<Tuple<ICommandKeys, int>>();
            if (args.Length > 0)
            {
                foreach (var command in keys)
                {
                    int partialLength;
                    if (MatchWithKeyword(command, args, out partialLength) || string.Compare(args[0], command.Name, StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        return new SelectionResult
                        {
                            Matched = true,
                            Command = command,
                            UsedArgumentCount = command.Keywords.Count + 1
                        };
                    }
                    else if (partialLength > 0)
                        failures.Add(Tuple.Create(command, partialLength));
                }
            }

            return new SelectionResult
            {
                Matched = false,
                Error = BuildMatchError(failures)
            };
        }

        private static string BuildMatchError(List<Tuple<ICommandKeys, int>> failures)
        {
            if (!failures.Any())
                return "Command not recognised.";

            var longest = failures.Max(f => f.Item2);
            var continuations = failures.Where(f => f.Item2 == longest)
                .Select(f => new {LastWord = f.Item1.Keywords[longest - 1], Continuation = longest < f.Item1.Keywords.Count ? f.Item1.Keywords[longest] : f.Item1.Name} )
                .ToList();
            if (continuations.Count == 1)
            {
                var continuation = continuations.First();
                return $"{continuation.LastWord} must be followed by {continuation.Continuation}.";
            }

            var listBarOne = string.Join(", ", continuations.Take(continuations.Count - 1).Select(c => c.Continuation)) + " or " +
                             continuations.Last().Continuation;
            return $"{continuations.First().LastWord} must be followed by {listBarOne}.";
        }

        private static bool MatchWithKeyword(ICommandKeys command, string[] args, out int partialMatchLength)
        {
            if (command.Keywords.Any() && args.Length >= command.Keywords.Count)
            {
                var matches = command.Keywords.Zip(args, (a, k) => string.Compare(a,k, StringComparison.InvariantCultureIgnoreCase) == 0).ToList();
                var keywordsMatch = matches.All(m => m);
                partialMatchLength = keywordsMatch ? command.Keywords.Count : matches.FindIndex(m => !m);
                return keywordsMatch && args.Length > command.Keywords.Count && command.Name == args[command.Keywords.Count];
            }

            partialMatchLength = 0;
            return false;
        }

        internal class SelectionResult
        {
            public bool Matched { get; set; }

            public string Error { get; set; }

            public ICommandKeys Command { get; set; }

            public int UsedArgumentCount { get; set; }
        }

    }
}
 