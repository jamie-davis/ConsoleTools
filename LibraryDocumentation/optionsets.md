---
layout: page
title: Reusing options through OptionSet
---

The ```[OptionSet]``` attribute is primarily useful when deriving from ```CommandDrivenApplication```. It allows multiple commands to share standard options without forcing you to repeat the same definition on each one. For example, if you have a program with multiple commands, and you needed to allow a database name and server to be specified on all of them using ```-d dbName``` and ```-s server```, you might put the following into every command:

	[Option("d", "database")]
	[Description("The name of the database to use.")]
	public string DatabaseName {get;set;}

	[Option("s", "server")]
	[Description("The name of the database server.")]
	public string ServerName {get;set;}

Repeating that on every command could become tedious, and you might have spelling differences or help text differences between commands, which is inefficient and looks unprofessional.

To avoid this, the toolkit supports the concept of a set of options through the ```[OptionSet]``` attribute.

Firstly, define the options you wish to reuse in a class. For example:

    public class ReusableOptions
    {
        [Option("d", "database")]
        [Description("The name of the database to use.")]
        public string DatabaseName { get; set; }

        [Option("s", "server")]
        [Description("The name of the database server.")]
        public string ServerName { get; set; }
    }

And then add a member to your command class using the ```[OptionSet]``` attribute:

    [OptionSet]
    public ReusableOptions DbOptions { get; set; }

the options defined in the option set will be added to the options in your command. As far as the command line is concerned, the fact that the options come from an ```[OptionSet]``` makes no difference.

When your command handler is called, the members of the ```[OptionSet]``` that define options will have been set. So, in the example above, the value that was specified for ```DatabaseName``` will be in ```DbOptions.DatabaseName```.
