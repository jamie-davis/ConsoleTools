---
layout: page
title: Global Options
---

Global options are only available to command driven applications. They define options that are automatically added to every non-interactive command, which means they can be specified on the command line, but they are not available on commands issued in an interactive session. This includes standard commands that can be used in both modes - the command will accept global options if it is invoked from the command line, but the same command will reject global options if they are specified during an interactive session.

This is useful to control elements that are global to the application - i.e. services that are set up once per execution and can be injected into command handlers. Truly global state cannot be changed during an interactive session, because the application does not get restarted between commands, so every command will see the same set of global objects. 

For example, imagine a scenario in which the command driven application connects to one of several environments. When the application starts up, it will make a connection to the selected environment. If you start an interactive session, you need to be able to select the target environemnt, but that environment should be the target for all commands issued in the session, or else you would need to repeatedly specify the environment and keep making fresh connections to it every time a command executes. You could implement the environment support through global options like this:

    [GlobalOptions]
    public static class GlobalOptions
    {
        [Option("env", "e")]
        [Description("The name of the environment to target.")]
        public static string Environment { get; set; }
    }

As you can see, the ```[GlobalOptions]``` attribute introduces a static class that contains ```[Option(...)]``` definitions. The options defined will be added to the definition of all command line command invocations. If an option is specified, it will be validated using the global definition in the usual way, but the value will be placed in the corresponding member of the static class, and will be available in the ```PostInitialParse``` override of ```CommandDrivenApplication```, for example:

    protected override void PostInitialParse()
    {
        RegisterInjectionInstance(new Environment(GlobalOptions.Environment));

        base.PostInitialParse();
    }

This will be called before the command handler, so the ```Environment``` instance in the example would be available to the command handler.

The global options help description will appear in help text appropriately. i.e. it will only appear if help is being rendered for command line commands, and will not be included in help for interactive commands, even if the same command might appear in both places.

You may specify any number of static ```[GlobalOptions]``` classes, and all of the options specified will be supported. If the same option name is defined by multiple instances, only the first one found will be used. (Exactly which of the options this will be is undefined, as it depends on the order in which the types are returned.)