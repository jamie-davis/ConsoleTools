---
layout: page
title: Framework Overview
---

The Console Toolkit supports two types of application.

##Console Applications
This is the most common style of application. It's an application that basically does a single job with options to control how it does it. For example:  

<img src="assets/images/diruserscommand.png" />  
 
The "dir" command's job is to list the contents of the file system. Its parameters and options let you customise the way it does this job, but it is essentially a one trick pony. Most Windows commands are of this type.

The Console Toolkit supports this type of application through a base class called `ConsoleApplication`. See [the Console application](consoleapps.html) documentation for details.

##Command Driven Applications
This is a less common style, but it is the style adopted by some of the more sophisticated command line applications. The command line interface of most source control systems I've used have had a command driven style. There is also a built in Windows command that uses it:  

<img src="assets/images/nethelpcommand.png" />  

The "net" command has lots of functionality, and you select what you want to do by specifying a command name as the first parameter. Each command has a different function, and its own distinct set of parameters and options. For example, "net use" will list all of the drive mappings you have set up to network resources, whereas "net config workstation" will display information about the computer on which the command is executed.  

The Console Toolkit supports this type of application through a base class called `CommandDrivenApplication`.
See [the command driven applications](commandapps.html) documentation for details.