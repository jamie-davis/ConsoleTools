using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkit.ApplicationStyles.Internals
{
    /// <summary>
    /// This is the base class for all Console Application framework classes. It is an internal class 
    /// and should not be used directly by console applications.
    /// </summary>
    public abstract class ConsoleApplicationBase
    {
        private enum InitPhase
        {
            PreInit,
            PosInit,
        }

        private InitPhase _initPhase = InitPhase.PreInit;
        private Func<Type, bool> _typeFilter;

        internal Lazy<MethodParameterInjector> Injector { get; set; }

        /// <summary>
        /// The command configuration for the application.<para/>
        /// You can supply a value for this in <see cref="Initialise"/>, or allow
        /// the default behaviour to locate classes with the <see cref="CommandAttribute"/>.
        /// <para/>
        /// It is an error to set this property outside of the <see cref="Initialise"/> method.
        /// </summary>
        protected CommandLineInterpreterConfiguration Config { get; set; }

        /// <summary>
        /// The available command handlers
        /// </summary>
        internal Dictionary<Type, ICommandHandler> Handlers { get; private set; }

        /// <summary>
        /// This is the error code returned by the console application framework when the command line is invalid.
        /// <para/>
        /// By default, the value is 1, but you can override this virtual property to return a different value.
        /// </summary>
        protected virtual int CommandLineErrorExitCode { get { return 1; } }

        /// <summary>
        /// This is the error code returned by the console application framework when no command handler can be located.
        /// <para/>
        /// By default, the value is 1000, but you can override this virtual property to return a different value.
        /// </summary>
        protected virtual int MissingCommandHandlerExitCode { get { return 1000; } }

        /// <summary>
        /// This is the console adapter for the application.
        /// </summary>
        protected IConsoleAdapter Console { get; set; }

        internal ConsoleApplicationBase()
        {
            Injector = new Lazy<MethodParameterInjector>(() => new MethodParameterInjector(new object[] { this, Console }));
        }

        /// <summary>
        /// This is an optional extension point. Override this method to carry out any initialisation 
        /// you wish to perform before the command line parameters are interpreted.<para/>
        /// 
        /// This method is the last opportunity you have to provide a command line configuration 
        /// before the default behaviour scans the assembly for classes decorated with the 
        /// <see cref="CommandAttribute"/>.<para/>
        /// 
        /// You can also customise the automatic configuration detection. See <see cref="SetConfigTypeFilter"/>.<para/>
        /// 
        /// It is also the last opportunity you have to select the interpreter that will be used to
        /// parse the command arguments. By default, the console application will support the
        /// current Microsoft standard for command line interpretation, but the older MS Dos style, 
        /// and the GNU Unix style are also available.<para/>
        /// 
        /// It is also the last place available to you to attach command handlers if you do not want 
        /// the default behaviour of scanning the assembly for classes decorated with the 
        /// <see cref="CommandHandlerAttribute"/>.<para/>
        /// 
        /// If it makes sense in your application to set the above in your constructor, it is fine to
        /// do that and you do not have to override this method.
        /// </summary>
        protected virtual void Initialise()
        {
        }

        private void LoadHandlersFromCommands(Type[] commandTypesArray, MethodParameterInjector injector)
        {
            foreach (var handler in commandTypesArray.SelectMany(t => CommandHandlerLoader.LoadHandlerMethods(t, commandTypesArray, injector)))
            {
                Handlers[handler.CommandType] = handler;
            }
        }

        private void LoadCommandHandlersFromAssembly(Type[] commandTypes, MethodParameterInjector injector)
        {
            var commandHandlerTypes = CommandAssemblyScanner.FindCommandHandlers(GetType().Assembly);
            if (_typeFilter != null)
                commandHandlerTypes = commandHandlerTypes.Where(_typeFilter);

            foreach (var handler in commandHandlerTypes.Select(t => CommandHandlerLoader.Load(t, commandTypes, injector)))
            {
                Handlers[handler.CommandType] = handler;
            }
        }

        private void LoadHandlersFromClass(Type[] commandTypes, MethodParameterInjector injector)
        {
            var handlerMethods = CommandHandlerLoader.LoadHandlerMethods(GetType(), commandTypes, injector);
            var duplicateHandler = handlerMethods.FirstOrDefault(m => Handlers.ContainsKey(m.CommandType));
            if (duplicateHandler != null)
            {
                throw new MultipleHandlersForCommand(duplicateHandler.CommandType);
            }

            foreach (var commandHandler in handlerMethods)
            {
                Handlers[commandHandler.CommandType] = commandHandler;
            }
        }

        internal abstract void LoadConfigFromAssembly();

        /// <summary>
        /// Register an instance of a type or interface that should be passed to command handlers that require
        /// the specified type.<para/>
        /// <remarks>Handler methods may specify the specified type in their parameter list and the instance will be
        /// provided.</remarks>
        /// </summary>
        /// <param name="instance">The instance to provide.</param>
        protected void RegisterInjectionInstance<T>(T instance)
        {
            Injector.Value.AddInstance(instance);
        }

        /// <summary>
        /// This method carries out funtionality that should follow <see cref="Initialise"/>.
        /// </summary>
        protected virtual void PostInitialise()
        {
            if (_initPhase != InitPhase.PreInit)
                throw new CallOrderViolationException();
            _initPhase = InitPhase.PosInit;

            if (Config == null)
                LoadConfigFromAssembly();
            Debug.Assert(Config != null);

            if (Handlers == null)
                Handlers = new Dictionary<Type, ICommandHandler>();

            if (!Handlers.Any())
            {
                var commandTypes = Config.Commands.Select(c => c.CommandType);
                if (Config.DefaultCommand != null)
                {
                    commandTypes = commandTypes.Concat(new[] {Config.DefaultCommand.CommandType});
                }

                var commandTypesArray = commandTypes.ToArray();
                LoadHandlersFromCommands(commandTypesArray, Injector.Value);
                LoadCommandHandlersFromAssembly(commandTypesArray, Injector.Value);
                LoadHandlersFromClass(commandTypesArray, Injector.Value);
            }
        }

        /// <summary>
        /// Sets a filter that can limit the types that will be searched for configuration. Supply a predicate filter 
        /// that excludes any types you don't want considered when assemblies are searched for configuration.
        /// </summary>
        protected void SetConfigTypeFilter(Func<Type, bool> typeFilter)
        {
            _typeFilter = typeFilter;
        }

        protected IEnumerable<Type> GetCommandTypes()
        {
            var types = CommandAssemblyScanner.FindCommands(GetType().Assembly);
            if (_typeFilter != null)
                types = types.Where(_typeFilter);
            return types;
        }
    }
}