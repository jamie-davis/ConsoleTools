using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleToolkit.Annotations;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Utilities;
using DescriptionAttribute = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// This class loads command configuration from a type.
    /// </summary>
    static class CommandAttributeLoader
    {
        /// <summary>
        /// Internal class used to hold properties and fields from a type that have a specific attribute.
        /// </summary>
        /// <typeparam name="T">The attribute type sought.</typeparam>
        class AttributedMember<T> where T : Attribute
        {
            public MemberInfo MemberInfo { get; private set; }
            public Type Type { get; private set; }
            public T Attribute { get; private set; }

            public AttributedMember(MemberInfo memberInfo, Type type, T attribute)
            {
                MemberInfo = memberInfo;
                Type = type;
                Attribute = attribute;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static CommandConfig<T> Create<T>(string name) where T : class, new()
        {
            var commandConfig = new CommandConfig<T>(CommandConstructionLambdaGenerator<T>.Generate())
            {
                Name = (name ?? MakeDefaultName<T>()).ToLowerInvariant(),
                CommandType = typeof (T)
            };

            AttachPropAndFieldElements(typeof(T), commandConfig);
            AttachOptionSets(commandConfig);
            var desc = GetDescription(typeof (T));
            if (desc != null)
                (commandConfig as IContext).Description = desc;

            return commandConfig;
        }

        /// <summary>
        /// The default name for a command is the class name. If the class name ends with the word 
        /// "Command", this will be dropped automatically. e.g. HelpCommand would have the command
        /// name "help".
        /// </summary>
        /// <typeparam name="T">The command type.</typeparam>
        /// <returns>The default command name for the type.</returns>
        private static string MakeDefaultName<T>() where T : class, new()
        {
            const string suffix = "command";

            var name = typeof (T).Name.ToLowerInvariant();
            if (name.EndsWith(suffix) && name != suffix)
            {
                return name.Substring(0, name.Length - suffix.Length);
            }
            return name;
        }

        private static void AttachPropAndFieldElements<T>(Type type, CommandConfig<T> commandConfig, AttributedMember<OptionSetAttribute> optionSet = null) where T : class
        {
            var positionals = GetMembersWithAttribute<PositionalAttribute>(type)
                .OrderBy(m => m.Attribute.Index)
                .ToList();

            var options = GetMembersWithAttribute<OptionAttribute>(type)
                .ToList();

            foreach (var member in positionals)
                AttachPositional(member, commandConfig, optionSet);
            foreach (var member in options)
                AttachOption(member, commandConfig, optionSet);
        }

        private static void AttachOption<T>(AttributedMember<OptionAttribute> optionMember, CommandConfig<T> commandConfig, AttributedMember<OptionSetAttribute> optionSet) where T : class
        {
            var option = MakeOption(optionMember.Attribute, commandConfig, optionMember.MemberInfo, optionMember.Type, optionSet);
            var desc = GetDescription(optionMember.MemberInfo);
            if (desc != null)
                option.Description = desc;
        }

        private static void AttachPositional<T>(AttributedMember<PositionalAttribute> posMember, CommandConfig<T> commandConfig, AttributedMember<OptionSetAttribute> optionSet) where T : class
        {
            var name = posMember.Attribute.Name ?? posMember.MemberInfo.Name.ToLowerInvariant();
            var positional = MakePositional(commandConfig, name, posMember.MemberInfo, posMember.Type, optionSet);
            var desc = GetDescription(posMember.MemberInfo);
            if (desc != null)
                positional.Description = desc;
            if (posMember.Attribute.DefaultSpecified)
            {
                positional.IsOptional = true;
                positional.DefaultValue = posMember.Attribute.DefaultValue;
            }
        }

        private static void AttachOptionSets<T>(CommandConfig<T> commandConfig) where T : class
        {
            var type = typeof(T);
            var optionSets = GetMembersWithAttribute<OptionSetAttribute>(type)
                .ToList();

            foreach (var optionSet in optionSets)
                AttachPropAndFieldElements(optionSet.Type, commandConfig, optionSet);
        }


        private static IEnumerable<AttributedMember<T>> GetMembersWithAttribute<T>(Type type) where T : Attribute
        {
            return type.GetProperties()
                .Select(p => new AttributedMember<T>(p, p.PropertyType, p.GetCustomAttribute<T>()))
                .Concat(type.GetFields().Select(f => new AttributedMember<T>(f, f.FieldType, f.GetCustomAttribute<T>())))
                .Where(m => m.Attribute != null);
        }

        private static string GetDescription(MemberInfo member)
        {
            var attrib = member.GetCustomAttribute<DescriptionAttribute>();
            if (attrib != null)
                return attrib.Text;
            return null;
        }

        private static string GetDescription(Type type)
        {
            var attrib = type.GetCustomAttribute<DescriptionAttribute>();
            if (attrib != null)
                return attrib.Text;
            return null;
        }

        // ReSharper disable once UnusedMember.Local
        private static BasePositional CallPositional<T, TMember>(string parameterName, Expression<Func<T, TMember>> expression,
            CommandConfig<T> command) where T : class
        {
            command.Positional(parameterName, expression);
            return command.Positionals.FirstOrDefault(p => p.ParameterName == parameterName);
        }

        // ReSharper disable once UnusedMember.Local
        private static BaseOption CallOption<T, TMember>(string optionName, Expression<Func<T, TMember>> expression,
            CommandConfig<T> command) where T : class
        {
            command.Option(optionName, expression);
            return command.Options.FirstOrDefault(o => o.Name == optionName);
        }

        private static BasePositional MakePositional<T>(CommandConfig<T> commandConfig, string name, MemberInfo member, Type memberType, AttributedMember<OptionSetAttribute> optionSet) where T : class
        {
            var genericCallPositional = typeof (CommandAttributeLoader).GetMethod("CallPositional", BindingFlags.Static | BindingFlags.NonPublic);
            var callPositional = genericCallPositional.MakeGenericMethod(new[] {typeof (T), memberType});

            var parameterExpression = Expression.Variable(typeof(T));
            var accessor = MakeMemberAccessor(member, parameterExpression, optionSet);
            return callPositional.Invoke(commandConfig, new object[]
            {
                name, 
                Expression.Lambda(accessor, parameterExpression),
                commandConfig
            }) as BasePositional;
        }

        private static MemberExpression MakeMemberAccessor([NotNull] MemberInfo member, [NotNull] Expression parameterExpression,
            [CanBeNull] AttributedMember<OptionSetAttribute> optionSet)
        {
            Expression source;
            if (optionSet == null)
                source = parameterExpression;
            else
                source = Expression.MakeMemberAccess(parameterExpression, optionSet.MemberInfo);
            
            return Expression.MakeMemberAccess(source, member);
        }

        private static BaseOption MakeOption<T>(OptionAttribute optionAttribute, CommandConfig<T> commandConfig, MemberInfo member, Type memberType, AttributedMember<OptionSetAttribute> optionSet) where T : class
        {
            Type[] parameterTypes;
            var optionInitialiser = ParameterAssignmentGenerator<T>.Generate(member, memberType, out parameterTypes, optionSet == null ? null : optionSet.MemberInfo);
            
            //need to make a call to CommandConfig<T> Option<T1, T2, T3>(string optionName, Action<T, T1, T2, T3> optionInitialiser)
            var optionName = GetOptionName(optionAttribute, member);
            var option = CallOptionCreateMethod(commandConfig, optionName, memberType, optionInitialiser, parameterTypes);
            if (option != null && optionAttribute.ShortCircuit)
                option.IsShortCircuit = true;

            var alias = GetOptionAlias(optionAttribute);
            if (option != null && alias != null)
            {
                var existingNames = commandConfig.Options.SelectMany(o => new[] { o.Name }.Concat(o.Aliases));
                option.Alias(alias, existingNames);
            }

            return option;
        }

        private static BaseOption CallOptionCreateMethod<T>(CommandConfig<T> commandConfig, string optionName, Type memberType, object optionInitialiser, Type[] parameterTypes) where T : class
        {
            MethodInfo optionMethod;
            if (memberType == typeof (bool))
            {
                optionMethod = commandConfig
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(m => m.Name == "Option"
                                         && m.GetGenericArguments().Length == 0
                                         && m.GetParameters().Length == 2
                                         && m.GetParameters()[1].ParameterType.Name.Contains("Action"));
            }
            else
            {
                var genericOptionMethod = commandConfig
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(m => m.Name == "Option"
                                         && m.GetGenericArguments().Length == parameterTypes.Length
                                         && m.GetParameters().Length == 2
                                         && m.GetParameters()[1].ParameterType.Name.Contains("Action"));
                Debug.Assert(genericOptionMethod != null, "genericOptionMethod != null");
                optionMethod = genericOptionMethod.MakeGenericMethod(parameterTypes);
                
            }
            Debug.Assert(optionMethod != null);

            optionMethod.Invoke(commandConfig, new[] {optionName, optionInitialiser});
            return commandConfig.Options.FirstOrDefault(o => o.Name == optionName);
        }

        private static string GetOptionName(OptionAttribute optionAttribute, MemberInfo member)
        {
            if (optionAttribute.LongName != null)
                return optionAttribute.LongName;

            if (optionAttribute.ShortName != null)
                return optionAttribute.ShortName;

            return member.Name.ToLowerInvariant();
        }

        private static string GetOptionAlias(OptionAttribute optionAttribute)
        {
            if (optionAttribute.LongName != null && optionAttribute.ShortName != null)
                return optionAttribute.ShortName;
            return null;
        }

        public static BaseCommandConfig Load(Type commandClass)
        {
            try
            {
                var commandAttribute = commandClass.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null)
                    throw new ArgumentException("Type does not have the Command attribute.", "commandClass");

                var genericCreateMethod = typeof (CommandAttributeLoader).GetMethod("Create",
                    BindingFlags.Static | BindingFlags.NonPublic);
                var createMethod = genericCreateMethod.MakeGenericMethod(new[] {commandClass});
                return createMethod.Invoke(null, new object[]{ commandAttribute.Name }) as BaseCommandConfig;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }
    }
}
