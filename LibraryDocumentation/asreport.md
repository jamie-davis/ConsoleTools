---
layout: page
title: More control through Report<T>
---

Automatic tabular data formatting is discussed in the [basic output](output.html) section. Here we will be covering a more comprehensive mechanism that provides far more control of tabular output, called ```Report<T>```.

This is an example of the automatic table formatting:

    var data = Enumerable.Range(0, 10)
        .Select(i => new { Value = i, Squared = i * i, String = new string('I', i)});
    console.FormatTable(data);

Which emits:	

	      Value     Squared String
	----------- ----------- ---------
	          0           0
	          1           1 I
	          2           4 II
	          3           9 III
	          4          16 IIII
	          5          25 IIIII
	          6          36 IIIIII
	          7          49 IIIIIII
	          8          64 IIIIIIII
	          9          81 IIIIIIIII

```FormatTable``` can do this automatically with any ```IEnumerable```. It has built in support for certain types, and will use the ```ToString()``` output for the rest.

The properties of the enumerated type are used to determine the data columns. Attributes of the column, such as left or right alignment, are determined by the datatype of the property. The heading is determined from the property name. This is all very strightforward and convenient when the defaults are adequate.

However, the Toolkit also offers a way to customise the output. ```Report<T>``` allows you to select what columns are shown, and to control most aspects of column formatting.

Take this example:

            var files = Directory.EnumerateFiles("C:\\Windows")
                .Select(f => new FileInfo(f))
                .Take(5);

This is the output (truncated for brevity):

	                                                     Is
	                             Directory               Read
	Name                  Length Name       Directory    Only  Exists
	------------ --------------- ---------- ------------ ----- ------
	bfsvc.exe              56832 C:\Windows C:\Windows   False True   Full Name:  C:\Windows\bfsvc.exe
	                                                                  Extension:                  .exe
	                                                                  Creation Time:        22/08/2013
	                                                                  12:21:53
	                                                                  Creation Time Utc:    22/08/2013
	                                                                  11:21:53
	                                                                  Last Access Time:     22/08/2013
	                                                                  12:21:53
	                                                                  Last Access Time Utc: 22/08/2013
	                                                                  11:21:53
	                                                                  Last Write Time:      22/08/2013
	                                                                  12:21:47
	                                                                  Last Write Time Utc:  22/08/2013
	                                                                  11:21:47
	                                                                  Attributes:              Archive
	bootstat.dat           67584 C:\Windows C:\Windows   False True   Full Name:  C:\Windows\bootstat.
	                                                                  dat
	                                                                  Extension:                  .dat
	                                                                  Creation Time:        22/08/2013
	                                                                  15:46:23
	                                                                  Creation Time Utc:    22/08/2013
	                                                                  14:46:23
	                                                                  Last Access Time:     22/08/2013
	                                                                  15:46:23
	                                                                  Last Access Time Utc: 22/08/2013
	                                                                  14:46:23
	                                                                  Last Write Time:      01/05/2015
	                                                                  07:58:35
	                                                                  Last Write Time Utc:  01/05/2015
	                                                                  06:58:35
	                                                                  Attributes:      System, Archive


The default handling does a decent job, but we don't need all of that data. Lets use ```Report<T>``` to refine it.

To use ```Report<T>```, there is an extension method for ```IEnumerable``` called ```AsReport```. This is defined in the ```ConsoleToolkit.ConsoleIO``` namespace. Lets get a report from our file data example:


    var report = files.AsReport(rep => rep
        .AddColumn(f => f.Name, cc => cc.Heading("File Name"))
        .AddColumn(f => f.Directory, cc => cc.Heading("Location"))
        .AddColumn(f => string.Format("{0:0.0}", f.Length/1024.0), cc => cc
            .Heading("Length (KiB)")
            .RightAlign())
        );

    console.FormatTable(report);

You might notice that we are creating a new variable called ```report``` but we just pass it into ```FormatTable``` as we did before.

The report definition is a bit funky, so let's go through it one line at a time:

    var report = files.AsReport(rep => rep

Here we are creating the report using the ```AsReport``` extension method. This can be applied to any ```IEnumerable<T>```, and you configure it using a lambda function. The example breaks the line in the middle of the lambda, just for layout purposes.

The lambda's definition is ```Action<ReportParameters<T>>```, which is slightly hideous (especially if you work out what ```T``` is in the example), but you should never have to actually look at that because, in normal usage, the compiler will infer the type for you.

```ReportParameters``` is a class used exclusively for configuring reports. The next line defines a column:

        .AddColumn(f => f.Name, cc => cc.Heading("File Name"))

Here we have a call to ```ReportParameter```'s ```AddColumn``` method. The first parameter is this lambda:

	f => f.Name

This is selecting the ```Name``` property from the ```T``` that we are reporting on.

The second parameter is this lambda:

	cc => cc.Heading("File Name")

This is how the column's settings are defined. ```cc``` is a column definition with the type ```ColumnConfig```. This defines various ways to configure a column, and we will get into those shortly. However, in this case we are just using it to define the column heading - ```"File Name"```, to be specific.

        .AddColumn(f => f.Directory, cc => cc.Heading("Location"))

The next line defines the "Location" column.

        .AddColumn(f => string.Format("{0:0.0}", f.Length/1024.0), cc => cc
            .Heading("Length (KiB)")
            .RightAlign())

The final line adds a column to report the file size in kibibytes. The first lambda formats the number:

	f => string.Format("{0:0.0}", f.Length/1024.0)

The second one configures the column:

	cc => cc
            .Heading("Length (KiB)")
            .RightAlign()

In this case, we are setting the column heading, as before - ```.Heading("Length (KiB)")``` - and choosing to right align the column values with ```.RightAlign()```. The result looks like this:

	                                      Length
	File Name                Location      (KiB)
	------------------------ ------------ ------
	bfsvc.exe                C:\Windows     55.5
	bootstat.dat             C:\Windows     66.0
	comsetup.log             C:\Windows      6.3
	diagerr.xml              C:\Windows     20.5
	diagwrn.xml              C:\Windows     20.5
	DirectX.log              C:\Windows      9.8
	DtcInstall.log           C:\Windows      5.1
	explorer.exe             C:\Windows   2442.7
	HelpPane.exe             C:\Windows    978.0
	hh.exe                   C:\Windows     17.0
	mib.bin                  C:\Windows     42.1
	msxml4-KB2758694-enu.LOG C:\Windows    251.8
	notepad.exe              C:\Windows    216.0
	PFRO.log                 C:\Windows     21.6
	Professional.xml         C:\Windows     35.4

###ColumnConfig
This is the object that allows column formatting parameters to be specified. You can see in the example above that columns are configured using a fluent interface:

	cc.Heading("Length (KiB)")
      .RightAlign()

Here ```cc``` is a ```ColumnConfig``` instance.

The following configuration options are available:

|```.Heading(heading)```|Used to set the column heading text.|
|```.RightAlign()```|Requests that the column content is right aligned.|
|```.LeftAlign()```|Requests that the column content is left aligned.|
|```.DecimalPlaces(int n)```|Requests that the column value is shown with n decimal places. This only has an effect on ```double``` and ```decimal``` values.| 
|```.Width(int n)```|Sets a fixed width for the column. This will be used if space permits.|
|```.MinWidth(int n)```|Specifies the minimum width for the column. The column width will be at least this big if space permits.|
|```.MaxWidth(int n)```|Specifies the maximum width for the column.|
|```.ProportionalWidth(double ```|Specifies that the column should share the spare space on the line with any other ```ProportionalWidth``` columns. The other columns will be made as compact as possible.|

There are no colour configuration options available, but the colour extension methods are supported, so you can have coloured column contents by using them in the data. This will correctly apply colour to multi-line column values. (Colours will not bleed into adjacent columns.)

###Nested Reports
It is possible to nest reports. For example:

    var dirs = Directory.EnumerateDirectories("C:\\Dev")
        .Where(d => Directory.EnumerateFiles(d).Count() > 2)
        .Take(2);

    var report = dirs.AsReport(rep => rep
        .AddColumn(d => d, cc => cc.Heading("Directory"))
        .AddChild(d => Directory.EnumerateFiles(d).Take(5)
                    .Select(f => new FileInfo(f)),
                   nested => nested
                    .AddColumn(fi => fi.Name, cc => cc.Heading("File Name"))
                    .AddColumn(fi => (fi.Length/1024).ToString("0.00"),
                        cc => cc.Heading("Length (KiB)")))
        );

    console.FormatTable(report);

Let's look a bit closer at that:

    var dirs = Directory.EnumerateDirectories("C:\\Dev")
        .Where(d => Directory.EnumerateFiles(d).Count() > 2)
        .Take(2);

Here's the source data - the names of some directories that are not empty.

    var report = dirs.AsReport(rep => rep
        .AddColumn(d => d, cc => cc.Heading("Directory"))

This is simply defining a column and setting the heading, as we've seen before.

        .AddChild(d => Directory.EnumerateFiles(d).Take(5)
                    .Select(f => new FileInfo(f)),
                   nested => nested
                    .AddColumn(fi => fi.Name, cc => cc.Heading("File Name"))
                    .AddColumn(fi => (fi.Length/1024).ToString("0.00"),
                        cc => cc.Heading("Length (KiB)")))

And this is the child report definition. We should look at this in more detail:

         .AddChild(d => Directory.EnumerateFiles(d).Take(5)
                    .Select(f => new FileInfo(f)),

Here we have the source data for the nested report. Embedded in the code is a lambda that extracts some files:

	d => Directory.EnumerateFiles(d).Take(5)
                  .Select(f => new FileInfo(f))

This takes the row from the main report (which is just a directory name in this example) as a parameter. The report in the example is based on ```FileInfo``` objects from the directory.

	   nested => nested
	    .AddColumn(fi => fi.Name, cc => cc.Heading("File Name"))
	    .AddColumn(fi => (fi.Length/1024).ToString("0.00"),
	        cc => cc.Heading("Length (KiB)")))

The rest of the statement is just a report definition.

This is the result:
	
	Directory
	------------------------
	C:\Dev\ConsoleTools
	
	                                        Length
	    File Name                           (KiB)
	    ----------------------------------- ------
	    .gitignore                          2.00
	    ConsoleToolkit.gpState              0.00
	    ConsoleToolkit.sln                  8.00
	    ConsoleToolkit.sln.DotSettings      0.00
	    ConsoleToolkit.sln.DotSettings.user 325.00
	
	C:\Dev\ConsoleToolsPages
	
	                 Length
	    File Name    (KiB)
	    ------------ ------
	    .gitignore   0.00
	    404.html     0.00
	    atom.xml     0.00
	    changelog.md 3.00
	    Gemfile      0.00

As you can see, each nested report is individually formatted, so their column widths do not match. This can add a substantial runtime overhead in some cases.