using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkit.Properties;
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
        //this is called dynamically
        private static CommandConfig<T> Create<T>(string name, bool nonInteractiveOk, bool interactiveOk, KeywordAttribute[] keywords) where T : class, new()
        {
            var commandConfig = new CommandConfig<T>(CommandConstructionLambdaGenerator<T>.Generate())
            {
                Name = (name ?? MakeDefaultName<T>()).ToLowerInvariant(),
                CommandType = typeof(T),
                ValidInNonInteractiveContext = nonInteractiveOk,
                ValidInInteractiveContext = interactiveOk,
            };

            foreach (var keywordAttribute in keywords)
            {
                commandConfig.Keyword(keywordAttribute.Keyword, keywordAttribute.Description);
            }

            AttachPropAndFieldElements(typeof(T), commandConfig);
            AttachOptionSets(commandConfig);
            AttachValidator(commandConfig);

            var desc = GetDescription(typeof (T));
            if (desc != null)
                (commandConfig as IContext).Description = desc;

            return commandConfig;
        }

        // ReSharper disable once UnusedMember.Local
        //this is called dynamically
        private static GlobalOptionsConfig<T> CreateGlobalOptions<T>() where T : class
        {
            var config = new GlobalOptionsConfig<T>();

            if (GetMembersWithAttribute<PositionalAttribute>(typeof(T), true).Any())
                throw new PositionalsCannotBeDeclaredInGlobalOptions(typeof(T));

            var options = GetMembersWithAttribute<OptionAttribute>(typeof(T), true)
                .ToList();

            foreach (var member in options)
                AttachOption(member, config, null);

            return config;
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

        private static void AttachOption<T, TCommandConfigType>(AttributedMember<OptionAttribute> optionMember, IOptionContainer<T, TCommandConfigType> commandConfig, AttributedMember<OptionSetAttribute> optionSet) 
            where T : class 
            where TCommandConfigType : class
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

        private static void AttachValidator<T>(CommandConfig<T> commandConfig) where T : class
        {
            var func = MakeValidationFunc<T>();
            if (func != null)
                commandConfig.Validator(func);
        }

        private static Func<T, IList<string>, bool> MakeValidationFunc<T>()
        {
            var type = typeof (T);
            var validatorMethods = type.GetMethods()
                .Where(m => m.GetCustomAttribute<CommandValidatorAttribute>() != null)
                .ToList();

            var command = Expression.Parameter(typeof (T), "command");
            var errorList = Expression.Parameter(typeof (IList<string>), "errors");
            var listType = typeof (List<Tuple<object, MethodInfo>>);
            var methods = Expression.Parameter(listType, "validators");
            
            var expressions = new List<Expression>();
            expressions.Add(Expression.Assign(methods, Expression.New(listType)));
            var addMethod = typeof(List<Tuple<object, MethodInfo>>).GetMethod("Add");

            var tupleConstructor = typeof(Tuple<object, MethodInfo>).GetConstructor(new[] { typeof(object), typeof(MethodInfo) });
            Debug.Assert(tupleConstructor != null);

            var numValidators = 0;
            foreach (var method in validatorMethods)
            {
                var newTuple = Expression.New(tupleConstructor, command, Expression.Constant(method));
                expressions.Add(Expression.Call(methods, addMethod, new [] { newTuple }));
                ++numValidators;
            }

            var optionSets = GetMembersWithAttribute<OptionSetAttribute>(typeof(T))
                .ToList();
            foreach (var optionSetMember in optionSets)
            {
                var optionSetValidators = optionSetMember.Type.GetMethods()
                                            .Where(m => m.GetCustomAttribute<CommandValidatorAttribute>() != null)
                                            .ToList();
                foreach (var optionSetMethod in optionSetValidators)
                {
                    var newTuple = Expression.New(tupleConstructor, Expression.MakeMemberAccess(command, optionSetMember.MemberInfo), Expression.Constant(optionSetMethod));
                    expressions.Add(Expression.Call(methods, addMethod, new[] { newTuple }));
                    ++numValidators;
                }
            }

            if (numValidators == 0)
                return null;
            
            var runner = typeof (CommandAttributeLoader).GetMethod("RunValidation", BindingFlags.NonPublic | BindingFlags.Static);
            expressions.Add(Expression.Call(null, runner, new [] {methods, errorList}));

            var body = Expression.Block(new [] { methods }, expressions);
            var parameters = new[] {command, errorList};
            return Expression.Lambda<Func<T, IList<string>, bool>>(body, parameters).Compile();
        }

        // ReSharper disable once UnusedMember.Local
        private static bool RunValidation(IEnumerable<Tuple<object, MethodInfo>> methods, IList<string> errors)
        {
            foreach (var method in methods)
            {
                try
                {
                    var outcome = (bool) MethodInvoker.Invoke(method.Item2, method.Item1, errors);
                    if (!outcome)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<AttributedMember<T>> GetMembersWithAttribute<T>(Type type, bool useStatics = false) where T : Attribute
        {
            return (useStatics ? type.GetProperties(BindingFlags.Static | BindingFlags.Public) : type.GetProperties())
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
        private static BasePositional CallActionPositional<T, TMember>(string parameterName, Action<T, TMember> action,
            CommandConfig<T> command) where T : class
        {
            command.Positional(parameterName, action);
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
            Type itemType;
            var isListType = CollectionTypeAnalyser.TryExtractListItemType(memberType, out itemType);
            if (isListType)
                return MakeListInsertPositional(commandConfig, name, member, itemType, optionSet);

            return MakeCopyThroughPositional(commandConfig, name, member, memberType, optionSet);
        }

        private static BasePositional MakeListInsertPositional<T>(CommandConfig<T> commandConfig, string name, MemberInfo member,
            Type itemType, AttributedMember<OptionSetAttribute> optionSet) where T : class
        {

            var genericCallPositional = typeof(CommandAttributeLoader).GetMethod("CallActionPositional",
                BindingFlags.Static | BindingFlags.NonPublic);
            var callPositional = genericCallPositional.MakeGenericMethod(new[] {typeof (T), itemType});

            var commandVariable = Expression.Variable(typeof(T));
            var itemVariable = Expression.Variable(itemType);
            var listType = typeof (ICollection<>).MakeGenericType(itemType);
            var accessor = Expression.Convert(MakeMemberAccessor(member, commandVariable, optionSet), listType);
            var insertMethod = listType.GetMethod("Add");
            var insert = Expression.Call(accessor, insertMethod, new Expression[] {itemVariable});

            var positional = MethodInvoker.Invoke(callPositional, commandConfig, new object[]
            {
                name,
                Expression.Lambda(insert, new [] {commandVariable, itemVariable}).Compile(),
                commandConfig
            }) as BasePositional;

            positional.AllowMultiple = true;

            return positional;
        }

        private static BasePositional MakeCopyThroughPositional<T>(CommandConfig<T> commandConfig, string name, MemberInfo member,
            Type memberType, AttributedMember<OptionSetAttribute> optionSet) where T : class
        {

            var genericCallPositional = typeof (CommandAttributeLoader).GetMethod("CallPositional",
                BindingFlags.Static | BindingFlags.NonPublic);
            var callPositional = genericCallPositional.MakeGenericMethod(new[] {typeof (T), memberType});

            var parameterExpression = Expression.Variable(typeof (T));
            var accessor = MakeMemberAccessor(member, parameterExpression, optionSet);
            return MethodInvoker.Invoke(callPositional, commandConfig, new object[]
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

        private static BaseOption MakeOption<T,TCommandConfigType>(OptionAttribute optionAttribute, IOptionContainer<T,TCommandConfigType> optionContainer, MemberInfo member, Type memberType, AttributedMember<OptionSetAttribute> optionSet) 
            where T : class
            where TCommandConfigType : class
        {
            Type[] parameterTypes;
            var optionSetMember = optionSet == null ? null : optionSet.MemberInfo;
            var optionSetter = ParameterAssignmentGenerator<T>.Generate(member, memberType, out parameterTypes, optionSetMember);
            
            var optionName = GetOptionName(optionAttribute, member);
            var option = CallOptionCreateMethod(optionContainer, optionName, memberType, optionSetter, parameterTypes);
            if (option != null)
            {
                option.IsShortCircuit = optionAttribute.ShortCircuit;
                option.AllowMultiple = CollectionTypeAnalyser.IsCollectionType(memberType);
            }

            var alias = GetOptionAlias(optionAttribute);
            if (option != null && alias != null)
            {
                var existingNames = optionContainer.Options.SelectMany(o => new[] { o.Name }.Concat(o.Aliases));
                option.Alias(alias, existingNames);
            }

            return option;
        }

        private static BaseOption CallOptionCreateMethod<T, TCommandConfigType>(IOptionContainer<T, TCommandConfigType> container, string optionName, Type memberType, object optionInitialiser, Type[] parameterTypes) 
            where T : class
            where TCommandConfigType : class
        {
            MethodInfo optionMethod;
            if (memberType == typeof (bool))
            {
                optionMethod = container
                    .GetType()
                    .GetMethods()
                    .FirstOrDefault(m => m.Name == "Option"
                                         && m.GetGenericArguments().Length == 0
                                         && m.GetParameters().Length == 2
                                         && m.GetParameters()[1].ParameterType.Name.Contains("Action"));
            }
            else
            {
                var genericOptionMethod = container
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

            MethodInvoker.Invoke(optionMethod, container, optionName, optionInitialiser);
            return container.Options.FirstOrDefault(o => o.Name == optionName);
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
                var commandAttribute = GetCommandAttribute(commandClass);
                if (commandAttribute == null)
                    throw new ArgumentException("Type does not have the Command attribute.", nameof(commandClass));

                var keywordAttributes = commandClass.GetCustomAttributes(typeof(KeywordAttribute), true);

                var genericCreateMethod = typeof (CommandAttributeLoader).GetMethod("Create",
                    BindingFlags.Static | BindingFlags.NonPublic);
                var createMethod = genericCreateMethod.MakeGenericMethod(commandClass);
                var callParameters = new object[]
                {
                    commandAttribute.Name,
                    commandAttribute.ValidInNonInteractiveSession,
                    commandAttribute.ValidInInteractiveSession,
                    keywordAttributes == null ? null : keywordAttributes
                };
                return MethodInvoker.Invoke(createMethod, null, callParameters) as BaseCommandConfig;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        public static BaseGlobalOptionsConfig LoadGlobalOptions(Type optionsClass)
        {
            try
            {
                var commandAttribute = GetCommandAttribute(optionsClass);
                if (commandAttribute != null)
                    throw new ArgumentException("Global options type may not have the Command attribute.", nameof(optionsClass));

                if (!optionsClass.IsAbstract || !optionsClass.IsSealed)
                    throw new ArgumentException("Global options defition must be a static class.", nameof(optionsClass));

                var genericCreateMethod = typeof (CommandAttributeLoader).GetMethod("CreateGlobalOptions",
                    BindingFlags.Static | BindingFlags.NonPublic);
                var createMethod = genericCreateMethod.MakeGenericMethod(optionsClass);
                return MethodInvoker.Invoke(createMethod, null) as BaseGlobalOptionsConfig;
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        private static BaseCommandAttribute GetCommandAttribute(Type commandClass)
        {
            return commandClass.GetCustomAttribute<CommandAttribute>()
                ?? commandClass.GetCustomAttribute<InteractiveCommandAttribute>()
                ?? commandClass.GetCustomAttribute<NonInteractiveCommandAttribute>() as BaseCommandAttribute ;
        }
    }
}
