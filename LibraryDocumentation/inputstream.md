---
layout: page
title: Input Overview
---

The input stream in a Console Application is typically treated in one of three ways:

* Ignored Completely

Many console applications simply don't use the input stream.

* The main input for the application

Sometimes applications support piping data directly in. This is a very specific use of StdInput, and the Toolkit does not offer any features for this type of application. You can still build them, and use ```Console.In``` as normal and the toolkit will not get in the way.

* Prompting the user for information

It is reasonably rare to find a console application that asks you to enter information at the command line. However, this may be due to the fact that it is not quick and easy to do. You will typically have data conversions and validation to do, and usually have to put a loop around every prompt. One of the goals of the Toolkit is to make this easier, and that's what we are going to talk about in this section.

### IConsoleAdapter
As with output, the console input operations are accessed through ```IConsoleAdapter```. This offers the following input commands:

* ```ReadLine``` which reads a string from the input stream.
* ```ReadInput``` which reads and validates input. This can return data of a specified type, and will block invalid input.
* ```Confirm``` which is a very simple way to get a yes or no decision from the user.

#### Confirming operations
```IConsoleAdapter``` allows you to prompt the user for confirmation in a single statement. For example:

    if (console.Confirm("Really delete?"))
        Delete();

This will give the user the opportunity to respond to a confirmation request:

	Really delete? [Y-Yes, N-No]: Y
	Deleted.

The user is required to give valid input:
	
	Really delete? [Y-Yes, N-No]: x
	"x" is not a valid selection.
	Really delete? [Y-Yes, N-No]: go ahead
	"go ahead" is not a valid selection.
	Really delete? [Y-Yes, N-No]: N

The method returns a boolean indicating the user's decision.

### Reading validated data
The Toolkit also offers a simple facility to read a number of data types from the user. For example, you can read an integer like this:

    var age = console.ReadInput(Read.Int().Prompt("Age")).Value;

Let'e examine that in more detail:

	var age = console.ReadInput(

```age``` here is going to be an integer. We can tell because of the parameter:

	Read.Int().Prompt("Age")

This is a fluent definition. ```Read.``` is referencing a static object that exposes a method of each supported data type. (```bool```, ```int```, ```long```, ```double```, ```Decimal```, ```DateTime``` and ```string``` are supported.) In this case ```Int()``` is being selected, and it is returning a configuration object.

With ```Prompt("Age")``` we are asking the toolkit to show the text "Age".

This is the outcome:

	Age: old
	Input string was not in a correct format.
	Age: 30

As you can see, the toolkit will only accept a valid integer.

There are other things you can do with the configuration object, which is described in more detail in the  [Customising	data input](custominput.html) section.

### Filling a data structure
The toolkit is able to make gathering multiple input items relatively trivial:

        var point = console.ReadInput(new {X = 0, Y = 0});
	    console.WrapLine(point.ToString());
	
Which gives:

	X: 10
	Y: 20
	{ X = 10, Y = 20 }

Although ```point``` is anonymous, and therefore immutable, the toolkit generates a new instance of the type and initialises it. However, ot is not neccessary to use anonymous types, and named, mutable types are just as good.
