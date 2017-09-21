---
layout: page
title: Defining command handlers
---

Command handlers are routines that are called when the command line has been parsed successfully. This is where you code the functionality of your application.

There are three types of command handler:

* Self Handlers
* Handler Methods
* Handler Classes

Generally speaking, Self Handlers are the preferred choice in most cases.

### Self Handlers
A self handler is a ```[CommandHandler]``` defined within a ```[Command]``` class. Most of the examples within the documentation are of this type.

To define a self handler, add the ```[CommandHandler]``` attribute to a method of your ```[CommandClass]```. For example:

    [Command]
    [Description("Process a file.")]
    internal class Options
    {
        [Option("help", "h", ShortCircuit = true)]
        [Description("Display help text.")]
        public bool Help { get; set; }

        [Positional]
        public string Filename
        {
            get; set;
            
        }

        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            //do something
        }
    }

In the example, the ```Handle``` method will be called to process the command. The toolkit will create an instance of the ```Options``` class, set the ```Filename``` property and call ```Handle``` with the specified parameters. 

### Handler Methods
Handler methods are methods defined in the ```Program``` class, that have the ```[CommandHandler]``` attribute and take the command class as a parameter. For example:

    internal class Program : ConsoleApplication
    {
        private static void Main(string[] args)
        {
            Toolkit.Execute<Program>(args);
        }

        protected override void Initialise()
        {
            RegisterInjectionInstance<IDatabase>(new Database());
            HelpOption<Options>(o => o.Help);
            base.Initialise();
        }

        [CommandHandler]
        public void Handle(Options options, IConsoleAdapter console, IErrorAdapter error)
        {
            //do something
        }
    }


    [Command]
    [Description("Process a file.")]
    internal class Options
    {
        [Option("help", "h", ShortCircuit = true)]
        [Description("Display help text.")]
        public bool Help { get; set; }

        [Positional]
        public string Filename
        {
            get; set;
            
        }
    }

Quite a big example, because it needs to show the whole ```Program``` class to be clear. The command handler is the last method in ```Program```:

        [CommandHandler]
        public void Handle(Options options, IConsoleAdapter console, IErrorAdapter error)
        {
            //do something
        }

The toolkit will create an instance of ```Options``` and pass it to ```Handle``` as a parameter.

Handler methods can be used when your program derives from ```CommandDrivenApplication```. Just define a handler method for each ```[Command]```. The toolkit looks for a ```[CommandHandler]``` that takes the ```[Command]``` type as a parameter.

This feature is useful for simple ```ConsoleApplications```, but is not recommended for ```CommandDrivenApplications```.

### Handler Classes
A Handler Class contains the a ```[HandlerMethod]``` for a ```[Command]```, but is not the same class.

For example:

    [CommandHandler]
    internal class OptionHandler
    {
        public void Handle(Options options, IConsoleAdapter console, IErrorAdapter error)
        {
            //do something
        }
    }

The toolkit would instantiate an instance of ```OptionHandler``` and pass it the ```Options``` instance to handle.

This type of command handler is useful if the the command has a lot of parameters and options, and you want to separate the code concerning command handling from the ````[Command]``` configuration information.
