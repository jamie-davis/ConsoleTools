---
layout: page
title: Future Development
---

These are some of the changes I am considering for the toolkit.

###Command Line Parsing
* Automatically support abbreviation of long option names. Only enough characters of an option name to make it unambiguous need be supplied. Currently, option names must be specified in full.
* Expand the help features. The automatic help text generation needs to be expanded, particularly for command driven applications. For example, it is common for command driven applications to have help topics that are not commands, but give information about a feature.
* Support the @file syntax to supply options and parameters in a file.
* Support parameter prompting. This would allow applications to have a switch which caused the user to be prompted for each mandatory parameter in turn, displaying help text with each parameter, and presented the options as a menu.

###Internationalisation
This is a very tricky feature to add. I made a conscious decision when I started the project not to include internationalisation at the beginning, because the best approach wasn't obvious to me. 

My vision for the toolkit has always been to allow use of ILMerge to create one-file executables, which is a common expectation with command line utilities - i.e. that you only have to copy the exe to run the command from a new location. Therefore the usual resource-DLL approach to internationalisation would not be appropriate. Some idea testing is required (and also the help of someone who can translate the library's text to a language other than English).
