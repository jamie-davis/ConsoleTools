using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    internal static class CollectionTypeAnalyser
    {
        internal static bool IsCollectionType(Type type)
        {
            return type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICollection<>));
        }

        internal static bool TryExtractListItemType(Type type, out Type itemType)
        {
            var listInterface = type.GetInterfaces()
                .FirstOrDefault(IsCollectionType);
            if (listInterface != null)
            {
                itemType = listInterface.GetGenericArguments()[0];
                return true;
            }

            itemType = type;
            return false;
        }
    }
}