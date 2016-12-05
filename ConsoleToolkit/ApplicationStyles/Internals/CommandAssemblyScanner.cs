using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    /// <summary>
    /// Scan an assembly for classes with the <see cref="CommandAttribute"/>.
    /// </summary>
    internal static class CommandAssemblyScanner
    {
        public static IEnumerable<Type> FindCommands(Assembly assembly, CommandScanType scanType)
        {
            var allCommands = FindTypesWithAttribute<CommandAttribute>(assembly).Select(t => Tuple.Create(t.Item1 as BaseCommandAttribute, t.Item2))
                .Concat(FindTypesWithAttribute<InteractiveCommandAttribute>(assembly).Select(t => Tuple.Create(t.Item1 as BaseCommandAttribute, t.Item2)))
                .Concat(FindTypesWithAttribute<NonInteractiveCommandAttribute>(assembly).Select(t => Tuple.Create(t.Item1 as BaseCommandAttribute, t.Item2)));

            switch (scanType)
            {
                case CommandScanType.InteractiveCommands:
                    allCommands = allCommands.Where(t => t.Item1.ValidInInteractiveSession);
                    break;

                case CommandScanType.NonInteractiveCommands:
                    allCommands = allCommands.Where(t => t.Item1.ValidInNonInteractiveSession);
                    break;

                case CommandScanType.AllCommands:
                    break;

                default:
                    throw new ArgumentOutOfRangeException("scanType", scanType, null);
            }
            return allCommands.Select(t => t.Item2);
        }

        private static IEnumerable<Tuple<T, Type>> FindTypesWithAttribute<T>(Assembly assembly) where T : Attribute
        {
            return assembly.GetTypes()
                .Select(t => Tuple.Create(t.GetCustomAttribute<T>(), t))
                .Where(t => t.Item1 != null);
        }

        public static IEnumerable<Type> FindCommandHandlers(Assembly assembly)
        {
            return FindTypesWithAttribute<CommandHandlerAttribute>(assembly).Select(t => t.Item2);
        }
    }
}
