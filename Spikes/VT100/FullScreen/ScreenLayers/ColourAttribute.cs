using System.Collections.Generic;
using VT100.FullScreen;

internal static class ColourAttribute
{
    private static Dictionary<VtColour, string> ForegroundAttributeLookup = new Dictionary<VtColour, string>
    {
        { VtColour.Black, $"{VirtualTerminalSequences.CSI}30m" },
        { VtColour.Red, $"{VirtualTerminalSequences.CSI}31m" },
        { VtColour.Green, $"{VirtualTerminalSequences.CSI}32m" },
        { VtColour.Yellow, $"{VirtualTerminalSequences.CSI}33m" },
        { VtColour.Blue, $"{VirtualTerminalSequences.CSI}34m" },
        { VtColour.Magenta, $"{VirtualTerminalSequences.CSI}35m" },
        { VtColour.Cyan, $"{VirtualTerminalSequences.CSI}36m" },
        { VtColour.White, $"{VirtualTerminalSequences.CSI}37m" },
        { VtColour.BrightBlack, $"{VirtualTerminalSequences.CSI}1;30m" }, //(Gray)
        { VtColour.BrightRed, $"{VirtualTerminalSequences.CSI}1;31m" },
        { VtColour.BrightGreen, $"{VirtualTerminalSequences.CSI}1;32m" },
        { VtColour.BrightYellow, $"{VirtualTerminalSequences.CSI}1;33m" },
        { VtColour.BrightBlue, $"{VirtualTerminalSequences.CSI}1;34m" },
        { VtColour.BrightMagenta, $"{VirtualTerminalSequences.CSI}1;35m" },
        { VtColour.BrightCyan, $"{VirtualTerminalSequences.CSI}1;36m" },
        { VtColour.BrightWhite, $"{VirtualTerminalSequences.CSI}1;37m" }
    };
        
    public static string GetColourAttribute(VtColour colour)
    {
        if (colour != VtColour.NoColourChange && ForegroundAttributeLookup.TryGetValue(colour, out var attributeString))
            return attributeString;

        return string.Empty;
    }
}