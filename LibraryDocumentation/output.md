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

###IConsoleAdapter vs. IErrorAdapter
Console applications can output to two standard streams:

* StdOut - The stream for normal console output. The static Console methods such as ```Console.WriteLine``` write to this stream.
* StdErr - The stream for error information. The static Console object provides access to this through its ```Error``` property. To write to is you might use calls such as ```Console.Error.WriteLine()```.

The Toolkit's approach is to treat these two streams a little more equitably, by giving both adapters