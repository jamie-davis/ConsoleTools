---
layout: page
title: Command Line Parsing Conventions
---
There are three main conventions for command line parsing common on Windows, as far as I can determine. The toolikit contains support for each of them.

To select the parsing conventions you wish to use in an application, set the ```ParsingConventions``` toolkit option before passing control to the toolkit. For example

{% highlight csharp %}
static void Main(string[] args)
{
    Toolkit.Options.ParsingConventions = CommandLineParserConventions.PosixConventions;
    Toolkit.Execute<Program>(args);
}{% endhighlight %}

The example selects the POSIX conventions. The available conventions are:

+ ```MicrosoftStandard```
+ ```PosixConventions```
+ ```MsDosConventions```

Below are descriptions of each set of conventions. The descriptions contain example command lines, which assume the following options definition:

{% highlight csharp %}
[Command]
class Options
{
    [Positional]
    public string Pos1 { get; set; }

    [Positional]
    public string Pos2 { get; set; }

    [Option("one", "o")]
    public bool One { get; set; }

    [Option("two", "t")]
    public string Two { get; set; }

    [Option("help", "h", ShortCircuit = true)]
    public bool Help { get; set; }
}{% endhighlight %}

### ```MicrosoftStandard```
These conventions can be read about [here](http://technet.microsoft.com/library/ee156811.aspx#EUAA "Microsoft Command Line Standard"). 

Options are introduced using a single dash character. Option arguments can be specified as part of the same token as the name using a colon seperator, (e.g. <code>-option:arg</code>), or as the next token (e.g. <code>-option arg</code>). Multiple values within an argument should be comma seperated (e.g. <code>-c string,45</code>). Option names should not be case sensitive. Finally, the token '<code>--</code>' indicates that none of the remaining tokens are options, and should be interpreted as positional arguments.

Positional arguments are named, can also be specified as if they were options using the option syntax.

The following are all equivalent:

```p1 p2 -o -t two```

```p1 p2 -o -t:two```

```-t two -pos2 p2 p1 -o```

### ```PosixConventions```
The POSIX standard is documented <a title="here" href="http://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html">here</a>, which briefly put, says:

Options are one alphanumeric character preceded by a hyphen (e.g. -a), and can be stacked (e.g. -abc is the same as -a -b -c).
Some options require an argument which can be part of the same token or the next token (e.g. -bArg is the same as -b Arg).

The GNU project adds "long options" which consist of "--" followed by a name made of 1-3 hyphenated words (e.g. -option-name).
Long option arguments are introduced with an equals symbol (e.g. -option-name=value)

The following are all equivalent:

```p1 p2 -o --two 2```

```p1 p2 -o -t2```

### ```MsDosConventions```
The MS-DOS conventions are not formally documented, as far as I can tell. However, a reasonable set of conventions can be derived from the various built in Windows commands. Here's what I settled on:

Options are one or more alphanumeric characters preceded by a forward slash. Plus and minus are also allowed (for an example of a '+' see the robocopy /A option). Some options require an argument which must be part of the same token separated from the option by a colon (e.g. /PERSISTENT:YES).

### ```CustomConventions```
It is possible to define your own custom conventions, but it is needlessly complicated and the feature is not ready for prime time yet. If you do play with it, be aware that there is no guarantee that it will continue to work in future releases.