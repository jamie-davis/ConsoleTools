---
layout: page
title: Interactive Overview
---

The toolkit allows your application to support a built in command line interface.

This functionality is accessed through the built in ```IInteractiveSessionService``` object that can
be injected into any command handler. You will need to add a command to your application that uses
the ```BeginSession()``` method on the service, which will then prompt the user for commands.

Any input entered by the user will be interpreted as an invocation of a command defined by your 
application, as if the command parameters had been supplied at the command line. Here is an example
command for invoking an interactive session:

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

As you can see, it is a relatively normal self-handling command definition, with the exception that 
it has an unusual command attribute - ```[NonInteractiveCommand]```, and it requests an instance of 
```IInteractiveSessionService``` be passed into the handler.

The interactive session will take place as part of the ```BeginSession()``` call. In fact, it's not so
much a call that starts a session as it is the session itself, but the name makes sense when coupled with
the ```EndSession()``` call which you will need to make when the interactive session needs to clode. For
example:

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

Here we have defined a command handler for a command the user can use to end the interactive session. Once
again, it has a specialised command attribute - ```[InteractiveCommand]``` and requires an
```IInteractiveSessionService```. 

In the example, we have apparently symmetrical commands to start and end an interactive session. However, what
actually happens is that ```BeginSession()``` call will not return control to the ```InteractiveCommand``` 
handler until the ```ExitCommand``` is executed from within the session. This does allow any set up and tear
down functionality to bracket the begin call, but the method name is somewhat misleading. (The intention was to
reflect semantically what you need to build - a command to start the session, and another one to end it.)

