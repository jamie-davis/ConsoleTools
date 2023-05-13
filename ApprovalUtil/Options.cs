using System.Reflection.Metadata.Ecma335;
using ApprovalUtil.Approving;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

[Command]
[Description("Utility to check the approval files and allow differences to be viewed and approved.")]
public class Options
{
    [Positional]
    [Description(@"The path to the source code. 

This will be used to recursively locate the approved and received text files. Each folder in the directory hierarchy will be checked for .cs files and related test outputs.")]
    public string PathToCode { get; set; } 
    
    [Option("help", "h", ShortCircuit = true)]
    [Description("Show command line help.")]
    public bool Help { get; set; }

    [CommandHandler]
    public void Handle(IConsoleAdapter console, IErrorAdapter error)
    {
        if (!Directory.Exists(PathToCode))
        {
            error.WrapLine($"{PathToCode.White()} path not found".Red());
            Environment.ExitCode = -100;
            return;
        }

        if (!Approver.Execute(console, error, this)) Environment.ExitCode = -200;
    }
}