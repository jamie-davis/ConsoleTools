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
        public static IEnumerable<Type> FindCommands(Assembly assembly)
        {
            return FindTypesWithAttribute<CommandAttribute>(assembly);
        }

        private static IEnumerable<Type> FindTypesWithAttribute<T>(Assembly assembly) where T : Attribute
        {
            return assembly.GetTypes().Where(t => t.GetCustomAttribute<T>() != null);
        }

        public static IEnumerable<Type> FindCommandHandlers(Assembly assembly)
        {
            return FindTypesWithAttribute<CommandHandlerAttribute>(assembly);
        }
    }
}
