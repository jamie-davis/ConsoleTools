---
layout: page
title: Output Streams And Adapters
---

The .NET static Console class provides input and output features to console applications. However, the features are quite basic and do not make it easy to format data, and using colour can be quite awkward.

The Console Toolkit provides enhanced facilities that are designed to make it easy to produce better formatted output to the console. You are not forced to use the enhanced console over the default, even if the examples in this documentation seem to encourage it.

The enhanced features are delivered through two interfaces:

* ```IConsoleAdapter```
* ```IErrorAdapter```

Both of these interfaces derive from ```IConsoleOperations``` which defines the supported output methods. This means that you can use the same formatting and colouring facilities for both standard output and error output, and in fact, if you rely on ```IConsoleOperation``` directly, you can write code that doesn't care which is being used.

### IConsoleAdapter vs. IErrorAdapter
Console applications can output to two standard streams:

* StdOut - The stream for normal console output. The static Console methods such as ```Console.WriteLine``` write to this stream.
* StdErr - The stream for error information. The static Console object provides access to this through its ```Error``` property. To write to this you might use calls such as ```Console.Error.WriteLine()```. Many console applications don't bother with the error stream and output everything to StdOut.

The Toolkit's approach is to treat these two streams a little more equitably, by giving both adapters that offer identical output facilities.

To access the streams, specify parameters on your ```[CommandHandler]``` with IConsoleAdapter and IErrorAdapter types.

### Redirection
Console output can be redirected to files. For example:

```dir > dir.txt```

Would run the built in ```dir``` command and the output will be written to a text file called "dir.txt". The ```>``` character tells the command line interpreter to attach the file as StdOut in place of the visible console window.

StdErr can also be redirected:

```dir 2> dir.txt```

Here, ```2>``` indicates that it's the error output that should be redirected. In this example, the directory information would be emitted to the console window.

If a Console Toolkit based application has its output redirected, the Toolkit will detect it and will ignore colour directives. Formatting that relies on the window size will still use the window size to perform word wrapping or format tables.

### When to write to StdErr
StdErr has a specific purpose. After all, it would be easier just to have one output stream for everything, right?

The reason for StdErr is to display diagnostic information about problems that you need to see, even if StdOut is redirected. Consider this example:

```dir | find "git" > gitfiles.txt```

This runs ```dir```, searches the output for the text "git", and the result is written to a text file called "gitfiles.txt".

If "dir" failed and wrote its error to StdOut, the error would be searched by ```find```, resulting in a silent failure and a useless "gitfiles.txt". Because the error had not been made visible, you would not be aware that something had gone wrong.

Fortunately, ```dir``` and ```find``` output their errors on StdErr:

```dir ::: | find "git" xxx > gitfiles.txt```

Gives:

	The system cannot find the path specified.
	File not found - XXX

This is still not perfect, because you can't tell which command emitted the messages. It happens to be one from each program in this case.

In the Unix world, there is a convention that output to StdErr should be prefixed with the program name. If ```dir``` and ```find``` followed that convention, the output might have looked like this:

	DIR: The system cannot find the path specified.
	FIND: File not found - XXX

The Console Toolkit embraces this convention and will automatically prefix all lines written via the IErrorAdapter with the program name. For example:

```SampleCommandApp: Unexpected argument "xxx"```

So, getting back to the question, "When to write to StdErr?", the answer is basically, whenever you are reporting information that would help diagnose why your program was unable to run correctly.

The Toolkit will automatically output command line parsing errors and unhandled exceptions to StdErr, which is sufficient in many cases, but if you intercept an unexpected condition in your code *and it is represents a failure*, you will need to report the pertinent information via IErrorAdapter.
