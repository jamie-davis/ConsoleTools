using System.Collections.Generic;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;

internal static class ColourAttribute
{
    private static Dictionary<VtColour, string> ForegroundAttributeLookup = new()
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

    private static Dictionary<VtColour, string> BackgroundAttributeLookup = new()
    {
        { VtColour.Black, $"{VirtualTerminalSequences.CSI}40m" },
        { VtColour.Red, $"{VirtualTerminalSequences.CSI}41m" },
        { VtColour.Green, $"{VirtualTerminalSequences.CSI}42m" },
        { VtColour.Yellow, $"{VirtualTerminalSequences.CSI}43m" },
        { VtColour.Blue, $"{VirtualTerminalSequences.CSI}44m" },
        { VtColour.Magenta, $"{VirtualTerminalSequences.CSI}45m" },
        { VtColour.Cyan, $"{VirtualTerminalSequences.CSI}46m" },
        { VtColour.White, $"{VirtualTerminalSequences.CSI}47m" },
        { VtColour.BrightBlack, $"{VirtualTerminalSequences.CSI}100m" }, //(Gray)
        { VtColour.BrightRed, $"{VirtualTerminalSequences.CSI}101m" },
        { VtColour.BrightGreen, $"{VirtualTerminalSequences.CSI}102m" },
        { VtColour.BrightYellow, $"{VirtualTerminalSequences.CSI}103m" },
        { VtColour.BrightBlue, $"{VirtualTerminalSequences.CSI}104m" },
        { VtColour.BrightMagenta, $"{VirtualTerminalSequences.CSI}105m" },
        { VtColour.BrightCyan, $"{VirtualTerminalSequences.CSI}106m" },
        { VtColour.BrightWhite, $"{VirtualTerminalSequences.CSI}107m" }
    };
        
    public static string GetForegroundAttribute(VtColour colour)
    {
        if (colour != VtColour.NoColourChange && ForegroundAttributeLookup.TryGetValue(colour, out var attributeString))
            return attributeString;

        return GetDefaultForegroundAttribute();
    }

    public static string GetBackgroundAttribute(VtColour colour)
    {
        if (colour != VtColour.NoColourChange && BackgroundAttributeLookup.TryGetValue(colour, out var attributeString))
            return attributeString;

        return GetDefaultBackgroundAttribute();
    }

    public static string GetColourAttribute(DisplayFormat format)
    {
        return $"{GetBackgroundAttribute(format.Background)}{GetForegroundAttribute(format.Foreground)}";
    }

    public static string GetDefaultForegroundAttribute()
    {
        return $"{VirtualTerminalSequences.CSI}39m";
    }

    public static string GetDefaultBackgroundAttribute()
    {
        return $"{VirtualTerminalSequences.CSI}49m";
    }
}