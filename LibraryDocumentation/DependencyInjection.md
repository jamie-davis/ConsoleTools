---
layout: page
title: IOC and passing services into handlers 
---

Take this command:

    [Command]
    public class TestCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error, IDatabase db)
        {
            console.WrapLine("Adding a new entity.");

            db.Add(new Entity());
            db.Commit();

            console.WrapLine("Entity added.");
        }
    }

This simple command has a built in handler that takes three parameters:

* An ```IConsoleAdapter``` called ```console```
* An ```IErrorAdapter``` called ```error```
* An ```IDatabase``` instance called ```db```

```IConsoleAdapter``` and ```IErrorAdapter``` are part of the toolkit and provide enhanced access to the console window's standard output and standard error. However ```IDatabase``` is not a toolkit class, and is in fact simply invented for this example.

So how does the handler get called with the correct parameters?

### Dependency Injection for command handlers
Dependency injection is a form of inversion of control in which the provision of services is delegated to an external framework.

From the toolkit user's point of view, all you really need to know is that the parameter list for a command handler is not fixed by the framework, and it can supply you with virtually any set of parameters you wish.

### Console Adapters
The toolkit includes enhancements to the default console, in the form of adapters for standard output (```IConsoleAdapter```) and standard error (```IErrorAdapter```). These adapters are interfaces that replace the default ```Console``` object, and provide more sophisticated display and data enty features.

The only way to get access to the adapters is to get them in the handler parameter list, but this is very easy to do and simply requires that your handler method has appropriate parameters. For example:

	[CommandHandler]
	public void Handler(IConsoleAdapter console, IErrorAdapter error)

The fact that this self-handler method is defined with ```IConsoleAdapter``` and ```IErrorAdapter``` parameters will cause the toolkit to pass it the interfaces.

If you define your command handler without adapters, you can still use the default ```Console```:

	[CommandHandler]
	public void Handler()

This self-handler would still be called but it will not be possible to use the toolkit's enhanced console features.

### Injecting other types
It is equally possible to inject arbitrary types into a command handler. This is primarily useful if you are using a test driven development approach, in which you write your command handler in terms of interfaces.

For example, if you have a database abstraction called IDatabase, you can provide the toolkit with a default instance in your Initialise override:

        protected override void Initialise()
        {
            RegisterInjectionInstance<IDatabase>(new Database());
            base.Initialise();
        }

This will allow the toolkit to provide the ```Database``` instance to the command handler. The toolkit does not contain a fully fledged IOC container - it cannot create instances of your classes automatically. This mechanism is intended to allow handlers to be run by automated unit tests, through dependency on abstractions. The enhanced console is implemented through interfaces so that a unit test can supply an  implementation that captures output instead of routing it to the static ```Console```. To facilitate this, the toolkit contains a suitable implementation (see ```UnitTestConsole```).