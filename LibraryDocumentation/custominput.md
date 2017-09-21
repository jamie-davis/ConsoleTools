---
layout: page
title: Customising	data input
---

In the data input [overview](inputstream.html) we saw how data could be captured from the user. For example:


        var point = console.ReadInput(new {X = 0, Y = 0});
	    console.WrapLine(point.ToString());

And:

    var age = console.ReadInput(Read.Int().Prompt("Age")).Value;

However, this can be customised further, and that's what this section is about.

### The Read object
In the example above, the ```Read``` object is being used to capture an integer from the user:

	Read.Int().Prompt("Age")

This is using one of ```Read```'s static methods - ```Int()``` - to get a configuration object, and then it is specifying the ```Prompt``` option to customise the display, so that it looks like this:

	Age: 

```Read```'s static methods are as follows:

* ```Boolean```
* ```DateTime```
* ```Decimal```
* ```Double```
* ```Int```
* ```Long```
* ```String```

This basic set represents the data types you can prompt for.

### Combining Read Objects
We have previously seen how multiple data items can be collected in one instruction:

        var point = console.ReadInput(new {X = 0, Y = 0});

```Read``` can also be used in this scenario to take control of each of the individual elements:

        var point = console.ReadInput(new 
			{
				X = Read.Int().Prompt("X Position"), 
				Y = Read.Int().Prompt("Y Position")
			});

This gives:

	X Position: 10
	Y Position: 20

The downside is that the object returned looks like this if you just print it:

	{ X = ConsoleToolkit.ConsoleIO.Read`1[System.Int32], Y = ConsoleToolkit.ConsoleIO.Read`1[System.Int32] }

To use the values captured you have to take the ```Value``` property:

	point.X.Value

This is slightly cumbersome, but the mechanism does allow you to group a set of data items together as a set of related values.

Ideally, the ```Read``` objects would be dropped and only the values would be returned, but the compiler needs to be able to infer the type of the object returned by ```ReadInput```, and that can only happen if it is the same type as the parameter.

### Giving the user choices
We have seen how you can set the prompt for a ```Read``` object. However, you can also capture user selections using the ```Option``` method:

        var selection = Read.String()
            .Prompt("Drink type")
            .Option("tea", "t", "Tea")
            .Option("coffee", "c", "Coffee")
            .Option("water", "w", "Water");
        var drink = console.ReadInput(selection);
        console.WrapLine(drink.Value);

Which gives:

	Drink type [t-Tea, c-Coffee, w-Water]: x
	"x" is not a valid selection.
	Drink type [t-Tea, c-Coffee, w-Water]: t
	tea

And you can choose to have the options presented as a menu with the addition of the ```AsMenu``` method:

        var selection = Read.String()
            .Prompt("Choose one")
            .Option("tea", "t", "Tea")
            .Option("coffee", "c", "Coffee")
            .Option("water", "w", "Water")
            .AsMenu("Select your drink");
        var drink = console.ReadInput(selection);
        console.WrapLine(drink.Value);

Here the prompt remains the text that precedes the user input and we get to specify a heading for the menu.

The exmaple results in this:
	
	Select your drink
	
	t-Tea
	c-Coffee
	w-Water
	
	Choose one: x
	"x" is not a valid selection.
	Select your drink
	
	t-Tea
	c-Coffee
	w-Water
	
	Choose one: t
	tea

### Custom validation
You can also specify custom validations for input data items:

    var bufferLengthSpec = Read.Int()
        .Validate(buffer => buffer >= 1000 && buffer <= 100000, 
				  "The buffer must be between 1000 and 100000")
        .Prompt("Network buffer size");
    var bufferLength = console.ReadInput(bufferLengthSpec);


The validation will be enforced:

	Network buffer size: 999
	The buffer must be between 1000 and 100000
	Network buffer size: X
	Input string was not in a correct format.
	Network buffer size: 1000000
	The buffer must be between 1000 and 100000
	Network buffer size: 5000