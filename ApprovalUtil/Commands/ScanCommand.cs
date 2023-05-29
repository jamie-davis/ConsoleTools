using System.Diagnostics.CodeAnalysis;
using ApprovalUtil.Approving;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ApprovalUtil.Commands;

[Command]
[Description("Check approval files and allow differences to be viewed and approved.")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ScanCommand : IApproverParams
{
    [Positional]
    [Description(@"The path to the source code. 

This will be used to recursively locate the approved and received text files. Each folder in the directory hierarchy will be checked for .cs files and related test outputs.")]
    public string? PathToCode { get; set; }

    [Option("interactive", "i")]
    [Description("If specified, the approval functions will be offered as a menu")]
    public bool Interactive { get; set; }

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