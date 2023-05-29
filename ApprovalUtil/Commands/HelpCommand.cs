using System.Diagnostics.CodeAnalysis;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace ApprovalUtil.Commands;

[Command]
[Description("Get command help")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class HelpCommand
{
    [Positional(DefaultValue = null)]
    [Description("The command on which help is required")]
    public List<string> Topic { get; set; }
}