---
layout: page
title: The Interactive Session Service
---

The ```IInteractiveSessionService``` is available for injection into any command handler. For example:

    [NonInteractiveCommand]
    class InteractiveCommand
    {
        [CommandHandler]
        public void Handle(IInteractiveSessionService interactiveSessionService)
        {
            interactiveSessionService.SetPrompt("-->");
            interactiveSessionService.BeginSession();
        }

    }

The example command is requesting an interactive session after setting the prompt to be displayed to the user.

### BeginSession()

The ```BeginSession()``` call runs the interactive session. The user will be prompted for a command string and 
this will be interpreted by the interactive session service and if it is valid, the appropriate command will be
invoked. From the point of view of the command handlers, there is no difference between being invoked from the 
command line directly and via the interactive session service.

### EndSession()

The ```EndSession()``` call should be executed from a command that wishes to terminate the interactive session.
This will generally be some sort of "exit" or "quit" command (whatever makes sense in your application). For example:

    [InteractiveCommand]
    [Description("End the interactive session.")]
    public class ExitCommand
    {
        [CommandHandler]
        public void Handle(IInteractiveSessionService service)
        {
            service.EndSession();
        }
    }

## Special command types

### HidingcCommands from the interactive session

It often makes sense to have commands that are available from the command line but not via an interactive session.
The command that starts the session is a good example - you should not allow that command to be invoked from within
a session, so it will normally be declared as a ```[NonInteractiveCommand]``` instead of a ```[Command]```. This will make the command invalid 
in the interactive session, which will not acknowledge its existence in help or during command parsing.

### Commands that are only valid within an interactive session

In that same way as commands may not make sense in an interactive session, it is also likely that you will define
commands that only make sense inside an interactive session. The "exit" command is a good example of this, in that 
it makes no sense to ask to exit an interactive session from the command line parameters. To declare a command that
is not valid outside of an interactive session, use ```[InteractiveCommand]``` in place of ```[Command]```.
 
### Commands valid in both interactive and non-interactive contexts

Any command defined with the usual ```[Command]``` attribute is valid in both contexts. No special consideration is required. 

### Effect on help text

The command help formatting handlers are aware of the difference between interactive an non-interactive commands, and are
sensitive to the context in which help is being invoked. Therefore, a ```CommandDrivenApplication``` will only need one 
help command, defined as a standard ```[Command]```. If it is invoked from the command line, it will show only ```[Command]```
and ```[NonInteractiveCommand]``` declared handlers. If it is invoked from an interactive session, it will show only
```[Command]``` and ```[InteractiveCommand]``` declared handlers.

One slightly unusual situation is a ```ConsoleApplication``` that starts an interactive session. The application is only allowed
one command that is available from the command line, and defines the application's command line options. These options define a 
switch that the user can specify to get command line help. For example:

{% highlight csharp %}
[NonInteractiveCommand]
[Description("Extract files from archive.")]
class Options
{
    [Positional]
    [Description("The name of the archive file.")]
    public string File { get; set; }

    [Positional]
    [Description("Optional list of files to extract. If no files are specifed, all files will be extracted.")]
    public List<string> ExtractFiles { get; set; }   
    
    [Option("output", "o")]
    [Description("Output to folder. If this option is not specified, output to the current directory.")]
    public string Output { get; set; }
    
    [Option("list", "l")]
    [Description("List the matching files instead of extracting them.")]
    public bool List { get; set; }

    [Option("interactive", "i")]
    [Description("Start an interactive session.")]
    public bool Interactive { get; set; }

    [Option("help", "h", ShortCircuit = true)]
    [Description("Display help text.")]
    public bool Help { get; set; }

    [CommandHandler]
    public void Handle(IConsoleAdapter console, IErrorAdapter error, IInteractiveSessionService service)
    {
        if (Interactive)
        {
            service.BeginSession();
        }
        else
        {
            ...
        }
    }
}
{% endhighlight %}

In the example, the "h" option invokes command line help. This is notified to the console application during the override to ```Initialise()```:

    protected override void Initialise()
    {
        base.HelpOption<Options>(o => o.Help);
        base.Initialise();
    }

This does not allow command level help to be invoked, and so ```ConsoleApplication``` allows a help command to be defined in addition to the program options:


    [InteractiveCommand]
    [Description("Display command help")]
    public class HelpCommand
    {
        [Positional(DefaultValue = null)]
        [Description("The command for which to display help")]
        public string Command { get; set; }
    }

This is defined as an ```[InteractiveCommand]``` which will be acceptable to a ```ConsoleApplication```. To get identify the command as a help invoker, add a line
to the initialise override:

        protected override void Initialise()
        {
            HelpOption<Options>(o => o.Help);
            InteractiveHelpCommand<HelpCommand>(h => h.Command);
            base.Initialise();
        }

Here ```InteractiveHelpCommand<HelpCommand>(h => h.Command);``` is installing the interactive help handler.