---
layout: page
title: Command Overview
---

The definition of commands is central to the functionality of a console application.

The Console Toolkit defines two types of application:

* Console Application
* Command Driven Application

From a command definition point of view, the only difference between the two types is the number of ```[Command]``` classes you need to define within your project.

# Console Applications
Console applications only have one ```[Command]``` that defines the options for the entire program. For this reason it is customary (but not mandatory) to name the command class ```Options```. For example:

	[Command]
	public class Options
	{
		[Positional]
		public string File {get; set;}
	}

Which would define a very simple, single command line parameter. (It also defines no help text or help option whatsoever, so it's not a very good example.)

The ```ConsoleApplication``` derived program could then be invoked from the command line and would expect the single parameter to be provided:

```MyProgram myfile.txt```

Attempting to define more than one ```[Command]``` class within the program will result in an exception at runtime, because the toolkit will be unable to determine which ```[Command]``` should define the program's parameters.

# Command Driven Applications
Command driven applications will contain more than one ```[Command]``` class - one for each command that they support.

For example, a program that defines "add", "delete" and "edit" commands would have three command classes:

	[Command]
	class AddCommand
	{
		[Positional]
		public string FileToAdd {get;set;}
	}

	[Command]
	class DeleteCommand
	{
		[Positional]
		public string FileToDelete {get;set;}
	}

	[Command]
	class EditCommand
	{
		[Positional]
		public string FileToEdit {get;set;}
	}

Once again, this example contains no help text and no help command, which is not good practice.

# Defining Commands
Commands are classes that have the ```[Command]``` attribute. The toolkit searches the assembly for classes with this attribute in order to build its command configuration.

### Command naming
By default, the class name will be used as the command name in a ```CommandDrivenApplication``` (in a ```ConsoleApplication```, there is only one command and its name is not important). By convention, if the command class name ends with "Command", the word "Command" will be dropped. For example:

* ```class AddCommand``` would configure a command called "add"
* ```class DeleteCommand``` would configure a command called "delete"
* ```class GetHelp``` would configure a command called "gethelp"

If you want to override the command name, you can specify a parameter to the ```[Command]``` attribute:

	[Command("doit")]
	class MyCommand
	{
	} 

which would define a command called "doit". 

### Command Help
The toolkit provides automated help facilities, and you can provide help text for a command class by adding the ```[Description]``` attribute (this example assumes a ```CommandDrivenApplication```):

	[Command]
	[Description("Add a file to the archive.")]
	class AddCommand
	{
		[Positional]
		public string Filename {get;set;}
	}

	[Command]
	[Description("Delete a file from the archive.")]
	class DeleteCommand
	{
		[Positional]
		public string Filename {get;set;}
	}

These commands define help text using the ```[Description]``` attribute. The resulting help text looks like this:
~~~
Available commands

add     Add a file to the archive.
delete  Delete a file from the archive.
~~~

