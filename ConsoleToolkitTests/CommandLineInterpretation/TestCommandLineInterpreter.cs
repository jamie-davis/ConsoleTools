using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    //[SetCulture("en-gb")]
    [UseReporter(typeof(CustomReporter))]
    public class TestCommandLineInterpreter
    {
        private CommandLineInterpreter _interpreter;
        private CommandLineInterpreterConfiguration _config;
        private static string _failureMessage;

        #region Types for test
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable NotAccessedField.Local

        public class PositionalTest
        {
            public string Name { get; set; }
            public string Param1 { get; set; }
            public bool Option { get; set; }
            public string Option2 { get; set; }
            public int OptionInt { get; set; }

            public PositionalTest(string name)
            {
                Name = name;
            }

            public override string ToString()
            {
                return string.Format("Command {0} {1} Option: {2} Option2: {3} OptionInt: {4}", Name, Param1, Option, Option2, OptionInt);
            }

        }

        public class GenericCommand<T>
        {
            public string Name { get; set; }
            public T Option { get; set; }
            public bool Validated { get; set; }

            public override string ToString()
            {
                var formatString = typeof(T) == typeof(DateTime) ? "{1:dd/MM/yyyy HH:mm:ss}" : "{1}";
                return string.Format("CMD[{0}] Option[" + formatString + "]{2}", Name, Option, Validated ? " VALIDATED" : string.Empty);
            }

            public static GenericCommand<T> Make(string name)
            {
                return new GenericCommand<T> {Name = name};
            }

            public static void AddCommand(CommandLineInterpreterConfiguration config)
            {
                config.Command(typeof(T).Name, Make)
                    .Option<T>("opt", (c, x) => c.Option = x);
                config.Command("P" + typeof(T).Name, Make)
                    .Positional<T>("parm", (c, x) => c.Option = x)
                .Validator((t, m) =>
                {
                    t.Validated = true;
                    if (_failureMessage != null)
                    {
                        m.Add(_failureMessage);
                        return false;
                    }

                    return true;
                });
            }
        }

        public class CustomParamType
        {
            public string Value { get; private set; }
            public CustomParamType(char c, string name)
            {
                Value = c + "-" + name;
            }

            public override string ToString()
            {
                return Value;
            }
        }

        public class DefaultCommandType
        {
            public string Param1 { get; set; }
            public bool Option { get; set; }
            public string Option2 { get; set; }
            public int OptionInt { get; set; }

            public override string ToString()
            {
                return string.Format("CMD[default] Param1[{0}] Option[{1}] Option2[{2}] OptionInt[{3}]", Param1, Option, Option2, OptionInt);
            }
        }

        public class RepeatPosCommandType
        {
            public List<string> Param1 { get; set; }

            public RepeatPosCommandType()
            {
                Param1 = new List<string>();
            }

            public override string ToString()
            {
                return string.Format("Param1[{0}]", string.Join(", ", Param1));
            }
        }

        class MockParser : ICommandLineParser
        {
            public string[] Args;
            public List<IOption> Options;
            public List<IPositionalArgument> Positionals;

            public void Parse(string[] args, IEnumerable<IOption> options, IEnumerable<IPositionalArgument> positionalArguments, IParserResult result)
            {
                Args = args;
                Options = options.ToList();
                Positionals = positionalArguments.ToList();
            }
        }

        static class GlobalOptions
        {
            public static bool Opt { get; set; }
        }
        // ReSharper restore NotAccessedField.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        #endregion

        public TestCommandLineInterpreter()
        {
            _failureMessage = null;
            MakeConfig();
            _interpreter = new CommandLineInterpreter(_config);
        }

        private void MakeConfig()
        {
            _config = new CommandLineInterpreterConfiguration();
            _config.Command("positionalTest", word => new PositionalTest(word))
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Option("opt", (c, b) => c.Option = b)
                .Option<string>("opt2", (c, s) => c.Option2 = s)
                .Option<int>("optInt", (c, i) => c.OptionInt = i);

            GenericCommand<bool>.AddCommand(_config);
            GenericCommand<byte>.AddCommand(_config);
            GenericCommand<char>.AddCommand(_config);
            GenericCommand<short>.AddCommand(_config);
            GenericCommand<ushort>.AddCommand(_config);
            GenericCommand<int>.AddCommand(_config);
            GenericCommand<uint>.AddCommand(_config);
            GenericCommand<long>.AddCommand(_config);
            GenericCommand<ulong>.AddCommand(_config);
            GenericCommand<double>.AddCommand(_config);
            GenericCommand<float>.AddCommand(_config);
            GenericCommand<decimal>.AddCommand(_config);
            GenericCommand<DateTime>.AddCommand(_config);

            CommandLineInterpreterConfiguration.AddCustomConverter(s => s.Length > 1 ? new CustomParamType(s.First(), s.Substring(1)) : null);
        }

        [Fact]
        public void CommandWithPositionalParameterIsExtracted()
        {
            var args = CommandLineTokeniser.Tokenise("positionalTest parameter");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void MissingPositionalParameterIsAnError()
        {
            var args = CommandLineTokeniser.Tokenise("positionalTest");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void ExtraPositionalParameterIsAnError()
        {
            var args = CommandLineTokeniser.Tokenise("positionalTest one two");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void CommandWithOptionIsExtracted()
        {
            var args = CommandLineTokeniser.Tokenise("positionalTest parameter -opt");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void CommandWithParameterisedOptionIsExtracted()
        {
            var args = CommandLineTokeniser.Tokenise("positionalTest parameter -opt2 fish");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void CommandWithOptionWithMissingParameterIsAnError()
        {
            var args = CommandLineTokeniser.Tokenise("positionalTest parameter -opt2 -opt");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void CommandWithOptionWithMissingParameterLastIsAnError()
        {
            var args = CommandLineTokeniser.Tokenise("positionalTest parameter -opt2");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void InvalidCommandIsAnError()
        {
            var args = CommandLineTokeniser.Tokenise("nothing parameter -opt2");
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void DefaultCommandIsUsedWhenNoOtherCommandsAreDefined()
        {
            var args = CommandLineTokeniser.Tokenise("parameter -opt2:value");
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Option("opt", (c, b) => c.Option = b)
                .Option<string>("opt2", (c, s) => c.Option2 = s)
                .Option<int>("optInt", (c, i) => c.OptionInt = i);

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var command = interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void CommandLineInterpreterSupportsDefaultCommand()
        {
            _config = new CommandLineInterpreterConfiguration();
            _config.Parameters(() => new PositionalTest(null))
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Option("opt", (c, b) => c.Option = b)
                .Option<string>("opt2", (c, s) => c.Option2 = s)
                .Option<int>("optInt", (c, i) => c.OptionInt = i);
        }

        public static Tuple<Type, string, string>[] ParameterTypeCases =
        {
            new Tuple<Type, string, string>(typeof(bool),"true", "X"),
            new Tuple<Type, string, string>(typeof(byte),"10", "X"),
            new Tuple<Type, string, string>(typeof(char),"A", "fred"),
            new Tuple<Type, string, string>(typeof(short),"10", "X"),
            new Tuple<Type, string, string>(typeof(ushort),"10", "X"),
            new Tuple<Type, string, string>(typeof(int),"10", "X"),
            new Tuple<Type, string, string>(typeof(uint),"10", "X"),
            new Tuple<Type, string, string>(typeof(long),"10", "X"),
            new Tuple<Type, string, string>(typeof(ulong),"10", "X"),
            new Tuple<Type, string, string>(typeof(double),"10.567", "X"),
            new Tuple<Type, string, string>(typeof(float),"10.567", "X"),
            new Tuple<Type, string, string>(typeof(decimal),"10.567", "X"),
            new Tuple<Type, string, string>(typeof(DateTime),"2014-02-05", "X")
        };

        [Fact]
        public void OptionParametersAreConverted()
        {
            var sb = new StringBuilder();

            foreach (var testCase in ParameterTypeCases)
            {
                var type = testCase.Item1;
                var validValue = testCase.Item2;
                var commandLine = string.Format("{0} -opt:{1}", type.Name, validValue);
                var args = CommandLineTokeniser.Tokenise(commandLine);
                string[] errors;
                var command = _interpreter.Interpret(args, out errors, false);

                sb.AppendLine(string.Format("{0} : {1}", commandLine, command));
            }

            Approvals.Verify(sb);
        }

        [Fact]
        public void CommandParametersAreConverted()
        {
            var sb = new StringBuilder();

            foreach (var testCase in ParameterTypeCases)
            {
                var type = testCase.Item1;
                var validValue = testCase.Item2;
                var commandLine = string.Format("P{0} {1}", type.Name, validValue);
                var args = CommandLineTokeniser.Tokenise(commandLine);
                string[] errors;
                var command = _interpreter.Interpret(args, out errors, false);

                sb.AppendLine(string.Format("{0} : {1}", commandLine, command));
            }

            Approvals.Verify(sb);
        }

        [Fact]
        public void OptionParametersThatCannotBeConvertedGenerateErrors()
        {
            var sb = new StringBuilder();

            foreach (var testCase in ParameterTypeCases)
            {
                var type = testCase.Item1;
                var invalidValue = testCase.Item3;
                var commandLine = string.Format("{0} -opt:{1}", type.Name, invalidValue);
                var args = CommandLineTokeniser.Tokenise(commandLine);
                string[] errors;
                var command = _interpreter.Interpret(args, out errors, false);

                sb.AppendLine(string.Format("{0} : {1}", commandLine, command == null ? "Command is null" : command.ToString()));

                var errorList = errors.ToList();
                if (errorList.Any())
                    sb.AppendLine(errorList.Aggregate((t, i) => t + ";" + i));
                else
                    sb.AppendLine("No errors.");
            }

            Approvals.Verify(sb.Replace("\0", ""));
        }

        [Fact]
        public void PositionalParametersThatCannotBeConvertedGenerateErrors()
        {
            var sb = new StringBuilder();

            foreach (var testCase in ParameterTypeCases)
            {
                var type = testCase.Item1;
                var invalidValue = testCase.Item3;
                var commandLine = string.Format("P{0} {1}", type.Name, invalidValue);
                var args = CommandLineTokeniser.Tokenise(commandLine);
                string[] errors;
                var command = _interpreter.Interpret(args, out errors, false);

                sb.AppendLine(string.Format("{0} : {1}", commandLine, command == null ? "Command is null" : command.ToString()));

                var errorList = errors.ToList();
                if (errorList.Any())
                    sb.AppendLine(errorList.Aggregate((t, i) => t + ";" + i));
                else
                    sb.AppendLine("No errors.");
            }

            Approvals.Verify(sb.Replace("\0", ""));
        }

        [Fact]
        public void CustomParameterTypesCanBeExtracted()
        {
            GenericCommand<CustomParamType>.AddCommand(_config);
            var commandLine = "PCustomParamType AOne";
            var args = CommandLineTokeniser.Tokenise(commandLine);
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(string.Format("{0} : {1}", commandLine, command));
        }

        [Fact]
        public void CustomParameterTypeConversionFailureIsDetected()
        {
            GenericCommand<CustomParamType>.AddCommand(_config);
            var commandLine = "PCustomParamType A";
            var args = CommandLineTokeniser.Tokenise(commandLine);
            string[] errors;
            _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(string.Format("{0} : {1}", commandLine, errors.Aggregate((t, i) => t + ", " + i)));
        }

        [Fact]
        public void ValidationsAreExecuted()
        {
            GenericCommand<CustomParamType>.AddCommand(_config);
            var commandLine = "PCustomParamType AOne";
            var args = CommandLineTokeniser.Tokenise(commandLine);
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(string.Format("{0} : {1}", commandLine, command));
        }

        [Fact]
        public void ValidationFailuresAreDetected()
        {
            FailValidation("Validation Failed.");

            GenericCommand<CustomParamType>.AddCommand(_config);
            var commandLine = "PCustomParamType AOne";
            var args = CommandLineTokeniser.Tokenise(commandLine);
            string[] errors;
            var command = _interpreter.Interpret(args, out errors, false);
            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void OptionNamesAreSuppliedToParser()
        {
            var customParser = new MockParser();
            var config = new CommandLineInterpreterConfiguration(customParser);
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Option("opt", (c, b) => c.Option = b)
                .Option<string>("opt2", (c, s) => c.Option2 = s)
                .Option<int>("optInt", (c, i) => c.OptionInt = i);

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            interpreter.Interpret(new [] { "x" }, out errors, false);

            Assert.Equal(new[] { "opt", "opt2", "optInt" }, customParser.Options.Select(o => o.Name));
        }

        [Fact]
        public void OptionAliasNamesAreSuppliedToParser()
        {
            var customParser = new MockParser();
            var config = new CommandLineInterpreterConfiguration(customParser);
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Option("opt", (c, b) => c.Option = b)
                .Option<string>("opt2", (c, s) => c.Option2 = s)
                .Alias("opt2Alias")
                .Option<int>("optInt", (c, i) => c.OptionInt = i)
                .Alias("optintalias");

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            interpreter.Interpret(new [] { "x" }, out errors, false);

            Assert.Equal(new[] { "opt", "opt2", "opt2Alias", "optInt", "optintalias" }, customParser.Options.Select(o => o.Name));
        }

        [Fact]
        public void PositionalNamesAreSuppliedToParser()
        {
            var customParser = new MockParser();
            var config = new CommandLineInterpreterConfiguration(customParser);
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Positional<string>("secondParam", (p, s) => p.Param1 = s)
                .Option("opt", (c, b) => c.Option = b)
                .Option<string>("opt2", (c, s) => c.Option2 = s)
                .Option<int>("optInt", (c, i) => c.OptionInt = i);

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            interpreter.Interpret(new [] { "x", "y" }, out errors, false);

            Assert.Equal(new[] { "firstParam", "secondParam" }, customParser.Positionals.Select(o => o.ParameterName));
        }

        [Fact]
        public void ShortCircuitOptionsHaltParsing()
        {
            var config = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MicrosoftStandard);
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Positional<string>("secondParam", (p, s) => p.Param1 = s)
                .Option("h", (c, b) => c.Option = b)
                .ShortCircuitOption();
 
            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new [] { "x", "-h" };
            var command = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, command, errors));
        }

        [Fact]
        public void EmptyCommandStringGeneratesMissingParametersError()
        {
            var customParser = new MockParser();
            var config = new CommandLineInterpreterConfiguration(customParser);
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Positional<string>("secondParam", (p, s) => p.Param1 = s)
                .Option("opt", (c, b) => c.Option = b)
                .Option<string>("opt2", (c, s) => c.Option2 = s)
                .Option<int>("optInt", (c, i) => c.OptionInt = i);

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new string[] { };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors));
        }

        [Fact]
        public void RepeatablePositionalsAreAccepted()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => { })
                .Positional<string>("secondParam", (p, s) => p.Param1 += s)
                .AllowMultiple();

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new [] { "1", "2a", "2b", "2c", "2d" };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors));
        }

        [Fact]
        public void RepeatablePositionalsMustBeSpecifiedAtLeastOnce()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => { })
                .Positional<string>("secondParam", (p, s) => p.Param1 += s)
                .AllowMultiple();

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new [] { "1" };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors));
        }

        [Fact]
        public void OptionalRepeatablePositionalsGetDefaultValue()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new RepeatPosCommandType())
                .Positional<string>("param1", (p, s) => p.Param1.Add(s))
                .AllowMultiple()
                .DefaultValue("defaultvalue");
            
            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new string[]{};
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors));
        }

        [Fact]
        public void OptionalRepeatablePositionalsGetSpecifiedValues()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new RepeatPosCommandType())
                .Positional<string>("param1", (p, s) => p.Param1.Add(s))
                .AllowMultiple()
                .DefaultValue("defaultvalue");
            
            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new[]{ "1", "2"};
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors));
        }

        [Fact]
        public void RepeatableOptionsMayBeSpecifiedMultipleTimes()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Parameters(() => new DefaultCommandType())
                .Positional<string>("firstParam", (p, s) => p.Param1 = s)
                .Option<string>("opt2", (c, s) => c.Option2 += s)
                .AllowMultiple();

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new[] { "1", "-opt2", "1", "-opt2", "2","-opt2", "3", };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors));
        }

        [Fact]
        public void CommandsWithKeywordsAreRecognised()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.Command("add", s => new GenericCommand<string> {Name = s})
                .Keyword("key");

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new[] { "key", "add" };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors));
        }

        [Fact]
        public void GlobalOptionsAreValid()
        {
            GlobalOptions.Opt = false;

            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option("gopt", () => GlobalOptions.Opt);
            config.Command("add", s => new GenericCommand<string> {Name = s})
                .Keyword("key");

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new[] { "key", "add", "-gopt" };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors, new {gopt = GlobalOptions.Opt}));
        }

        [Fact]
        public void GlobalOptionsAreIgnoredIfTheyClashWithCommandOptionNames()
        {
            GlobalOptions.Opt = false;

            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option("opt", () => GlobalOptions.Opt);
            config.Command("add", s => new GenericCommand<bool> {Name = s})
                .Option("opt", c => c.Option);

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new[] { "add", "-opt" };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors, new {gopt = GlobalOptions.Opt}));
        }

        [Fact]
        public void GlobalOptionsAreInvalidInInteractiveMode()
        {
            GlobalOptions.Opt = false;

            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option("gopt", () => GlobalOptions.Opt);
            config.Command("add", s => new GenericCommand<string> {Name = s})
                .Keyword("key");

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new[] { "key", "add", "-gopt" };
            var result = interpreter.Interpret(args, out errors, false, isInteractive: true);

            Approvals.Verify(Describe(args, result, errors, new {gopt = GlobalOptions.Opt}));
        }

        [Fact]
        public void GlobalOptionsDefaultIfNotSet()
        {
            GlobalOptions.Opt = false; //initialise the global option - this value should be carried through.

            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option("gopt", () => GlobalOptions.Opt);
            config.Command("add", s => new GenericCommand<string> {Name = s})
                .Keyword("key");

            var interpreter = new CommandLineInterpreter(config);
            string[] errors;
            var args = new[] { "key", "add" };
            var result = interpreter.Interpret(args, out errors, false);

            Approvals.Verify(Describe(args, result, errors, new {gopt = GlobalOptions.Opt}));
        }

        private void FailValidation(string failureMessage)
        {
            _failureMessage = failureMessage;
        }

        private static string Describe(IEnumerable<string> args, object command, IEnumerable<string> errors, params object[] globalObjects)
        {
            var argsList = new StringBuilder();
            var ix = 0;
            argsList.AppendLine("Argument list:");
            foreach (var s in args)
            {
                argsList.AppendLine(string.Format("{0,3}: {1}", ix++, s));
            }
            argsList.AppendLine();

            var errorReport = new StringBuilder();
            foreach (var message in errors)
            {
                errorReport.AppendLine(message);
            }

            var sb = new StringBuilder();
            if (command == null)
            {
                sb.Append(argsList);
                sb.AppendLine("Command is null.");

                if (errorReport.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(errorReport.ToString());
                }
                return sb.ToString();
            }

            sb.AppendLine(command.GetType().Name);
            sb.AppendLine();
            sb.AppendLine(argsList.ToString());
            sb.AppendLine();

            sb.AppendLine(string.Format("Command type: {0}", command.GetType().Name));
            sb.AppendLine();
            sb.AppendLine(command.ToString());

            if (errorReport.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine(errorReport.ToString());
            }

            if (globalObjects.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Global Objects:");
                foreach (var globalObject in globalObjects)
                {
                    foreach (var propertyInfo in globalObject.GetType().GetProperties())
                    {
                        sb.AppendLine($"{propertyInfo.Name} = {propertyInfo.GetValue(globalObject, null).ToString()}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
