---
layout: page
title: Options
---

Commands can be configured in various ways, and one of the most common elements is the option or switch.

Consider this command line:

```MyProgram -i myfile.txt```

The text ```MyProgram``` is the name of the executable, the text ```-i myfile.txt``` is an option passed to the executable with the parametere "myfile.txt". The executable understands the meaning of the parameter because it is preceded by an option name, which differentiates it from a [positional parameter](positionalParameters.html).

You can define an option on a ```[Command]``` using a special attribute called ```[Option]```:

	[Command]
	class Options
	{
		[Option]
		public string Category {get;set;}
	{

This defines a commmand with no parameters and an option called "category". Option names are quite importatnt because the user is required to enter them. For this reason, the toolkit provides a way for the options to be named explicitely, rather than relying on the property name. For example:

	[Option("c")]
	public string Category {get;set;}

Defines an option called ```-c```. The option value will still be available in the Category property of the command class.

You can also define alias names for options:

	[Option("category", "c")]
	public string Category {get;set;}

In this case the option can be called either ```-c``` or ```-category```.

The options defined are reflected in the help text. For example, given the following:

    [Command]
    [Description("Process a file.")]
    internal class Options
    {
        [Positional]
        [Description("The file to process.")]
        public string FileName { get; set; }

        [Option("category", "c")]
        [Description("The category for the results.")]
        public string Category { get; set; }
    }

The help text looks like this:

~~~
Process a file.

Usage: SampleCommandApp <filename> [options]

Parameters:

filename  The file to process.

Options:

-category, -c  The category for the results.
~~~

This example assumes a console application.

#### Help Text
Options can have help text associated with them using the ```[Description]``` attribute:

        [Option("category", "c")]
        [Description("The category for the results.")]
        public string Category { get; set; }

Which can be seen in the help output:

~~~
Process a file.

Usage: SampleCommandApp <filename> [options]

Parameters:

filename  The file to process.

Options:

-category, -c  The category for the results.
~~~

#### Repeating Options
Sometimes, an option must be repeatable - for example, Microsoft's C++ compiler accepts any number of ```-D``` parameters:
```cl a.cpp /D WIN32 /D NDEBUG /D _CONSOLE /D _MBCS```

Which would compile ```a.cpp``` and define several pre-processor symbols - ```WIN32```, ```NDEBUG```. ```_CONSOLE``` and ```_MBCS```.

You can achieve the same behaviour by specifying an ```[Option]``` as a collection type:

        [Option("define", "d")]
        public List<string> Definitions { get; set; } 

The fact that ```List<string>``` implements ```ICollection<string>``` allows the toolkit to accept multiple values for the option.

You can have repeating options of types other than strings:

        [Option("number", "n")]
        public List<int> Numbers { get; set; } 

All of the options supplied for ```Numbers``` would be validated as integers.

The fact that an option can be repeated is not automatically reported by the help text, so you should mention it in your ```[Description]``` text.

#### Options with multiple parameters
It is possible to define an option that takes multiple parameters by defining a simple type with multiple properties:

    [Command]
    internal class Options
    {

        public class MyType
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }

        [Option("complex", "c")]
        public MyType ComplexOption { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine("Name: {0}", ComplexOption.Name);
            console.WrapLine("Number: {0}", ComplexOption.Number);
        }
    }

Using the default options, passing in a command line parameter of:

```MyProgram -c myname,10```

Would print:

	Name: myname
	Number: 10

It is also valid for this type of option to be repeatable:

        [Option("complex", "c")]
        public List<MyType> ComplexOption { get; set; }

#### Validation
The type of the property defining the option determines the validation that will be carried out on the input.

Enumerations are not supported for options at ?the time of writing. If an unsupported type is used for an option, an ```InvalidParameterType``` exception will be thrown by the toolkit.

All of the type validation is carried out automatically, and command handlers will only be called when the options have acceptable values.

Options that are not specified on the command line will have the default value for their property type. The only exception to this is that repeatable options will be set to an empty collection.

#### Short Circuit Options
The toolkit will validate the command line and only call handlers where the user hsa specified all of the required information. However, sometimes you need to be able to side-step the validation. For example, this is common with the help option in a standard console application. The command line might usually have mandatory parameters, but the user needs to be able to access the help information because they don't know what's valid and what isn't:

```MyProgram -h```

The executable could have several mandatory parameters, and ordinarily you would want them to be validated, but getting help is an exception - you want the above command to work.

To facilitate this, the ```[Options]``` can have a "sort circuit" characteristic, which halts command line validation when the option is used. Here's how you specify it:

	[Option("help", "h", ShortCircuit = true)]
	public bool Help {get;set;}

Assume the above option definition is part of a command configuration, and the user specifies it on a command line. The toolkit will start validating the command line as usual, but as soon as the "-h" option is evaluated, the toolkit would stop processing the rest of the command line.

This feature is useful when you have a requirement that is not the main function of the program, such as administrative operations (e.g. clearing cached data), or, as above, giving command line help.