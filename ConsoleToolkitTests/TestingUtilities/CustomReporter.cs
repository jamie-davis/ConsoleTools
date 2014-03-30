using ApprovalTests.Reporters;

namespace ConsoleToolkitTests.TestingUtilities
{

    /// <summary>
    /// Simple utility reporter to choose the quiet reporter on the build server, but the WinMerge reporter for development work.
    /// </summary>
    public class CustomReporter : FirstWorkingReporter
    {
        public static readonly CustomReporter INSTANCE = new CustomReporter();

        public CustomReporter()
            : base(
                WinMergeReporter.INSTANCE,
                QuietReporter.INSTANCE
            )
        {
        }
    }

}
