---
layout: page
title: Positional Parameters
---

Commands can be configured in various ways, and one of the most common elements is the positional parameter.

Consider this command line:

```MyProgram myfile.txt```

The text ```MyProgram``` is the name of the executable, the text ```myfile.txt``` is a parameter passed to the executable.

In toolkit parlance, ```myfile.txt``` is a positional parameter because it does not need an option name to identify what the value being passed in means - the meaning is implied by the fact that it is the first parameter i.e. its position on the command line.

You can define a positional parameter on a ```[Command]``` using a special attribute called ```[Positional]```:

	[Command]
	class Options
	{
		[Positional]
		public string FileToAdd {get;set;}

		[Positional]
		public string Category {get;set;}
	{

This defines a commmand with two parameters called "filetoadd" and "category". The parameter names are generally not important, but they do appear in the help text. For example:

~~~

Usage: SampleCommandApp <filetoadd> <category>

Parameters:

filetoadd
category

~~~

This example assumes a console application.

#### Help Text
Positional parameters can have help text associated with them using the ```[Description]``` attribute:

        [Positional]
	    [Description("The path to the file to be added to the archive")]
        public string FileToAdd { get; set; }

Which would have the following effect on the help output:

~~~

Usage: SampleCommandApp <filetoadd> <category>

Parameters:

filetoadd  The path to the file to be added to the archive
category

~~~

#### Default value
If a positional parameter is optional, it must be given a default value:

        [Positional(DefaultValue = "DefaultCategory")]
        public string Category { get; set; }

If no value is specified for the parameter, rather than giving an error message, the parameter will be initialised with the default value.

This only works if all of the optional positional parameters are at the end of the parameter list. If an optional parameter is followed by a non-optional parameter, the default value will be ignored and the user will be required to provide a valid value.

#### Parameter Order
Usually, you will define the parameter order by the order in which the parameters are specified in the class. If you wish to override the order, you can specify a parameter index:

        [Positional(1)]
	    [Description("The path to the file to be added to the archive")]
        public string FileToAdd { get; set; }

This will reposition the parameter:

~~~

Usage: SampleCommandApp [<category>] <filetoadd>

Parameters:

category
filetoadd  The path to the file to be added to the archive

~~~

#### Repeating parameters
Sometimes, a parameter must be repeatable - for example, compilers commonly support listing any number of source files on the command line:

```cl a.cpp b.cpp```

Which would compile both ```a.cpp``` and ```b.cpp```.

You can achieve the same behaviour by specifying a positional parameter as a collection type:

        [Positional]
        public List<string> Repeating { get; set; } 

The fact that ```List<string>``` implements ```ICollection<string>``` allows the toolkit to accept multiple values for the parameter.

There is only one repeating parameter possible for a command, and it must be the last parameter. The command line parser has no way to detect the end of the repetitions for a repeating parameter, so any subsequent parameters will never get values.

You can have repeating parameters of types other than strings:

        [Positional]
        public List<int> Repeating { get; set; } 

All of the parameters supplied for ```Repeating``` would be validated as integers.

At least one value is expected in a repeating positional. Supplying zero instances will give an error message. However, you can still supply a default value:

        [Positional(DefaultValue = "default")]
        public List<int> Repeating { get; set; } 

In this case, if no value is provided on the command line, it will not be an error and the handler will receive a collection containing a single string set to ```"default"```.

#### Validation
The type of the property defining the positional parameter determines the validation that will be carried out on the command line input.

Only simple types are supported for positional parameters, which excludes enumerations at this time. If an unsupported type is used for a positional parameter, a ```InvalidParameterType``` exception will be thrown by the toolkit.

The validation of a parameter is essentially if .Net can parse the string as the datatype, it is valid.

All of the type validation is carried out automatically, and command handlers will only be called when the parameters have acceptable values.