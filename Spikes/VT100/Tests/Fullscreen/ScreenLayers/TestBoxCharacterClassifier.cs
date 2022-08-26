using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.FullScreen.ScreenLayers;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestBoxCharacterClassifier
    {
        [Fact]
        public void AnalysisExtractsCharacters()
        {
            //Act
            var result = BoxCharacterClassifier.Classify().ToList();
            
            //Assert
            var output = new Output();
            var rep = result.Select(n => new
            {
                n.Source, 
                Char = (char)n.Source,
                Unparsed = string.Join(" ", n.Unparsed),
                n.CornerType, 
                Left = n.Left?.ToString(),
                Right = n.Right?.ToString(),
                Up = n.Up?.ToString(),
                Down = n.Down?.ToString()
            });
            output.FormatTable(rep, ReportFormattingOptions.UnlimitedBuffer);
            output.Report.Verify();
            ExtractUsefulLists(result);
        }

        private void ExtractUsefulLists(List<BoxCharacter> boxCharacters)
        {
            var output = new Output();
            var rep = boxCharacters.Select(n => new
            {
                n.Source, 
                Char = (char)n.Source,
                Left = n.Left?.LineWeight,
                Right = n.Right?.LineWeight,
                Up = n.Up?.LineWeight,
                Down = n.Down?.LineWeight
            });
            output.FormatTable(rep, ReportFormattingOptions.UnlimitedBuffer);
            var reportPath = Path.Combine(Path.GetDirectoryName(SourcePath()), "WeightReport.txt");
            File.WriteAllText(reportPath, output.Report);
        }
        
        private static string SourcePath([CallerFilePath] string path = null)
        {
            return path;
        }
    }
}