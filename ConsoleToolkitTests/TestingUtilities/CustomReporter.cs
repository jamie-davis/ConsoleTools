using System;
using System.Collections.Generic;
#if NOKLUDGE
using ApprovalTests.Core;
#endif
using ApprovalTests.Reporters;

namespace ConsoleToolkitTests.TestingUtilities
{

    /// <summary>
    /// Simple utility reporter to choose the quiet reporter on the build server, but the WinMerge reporter for development work.
    /// </summary>
#if NOKLUDGE
    public class CustomReporter : IEnvironmentAwareReporter
    {
        private static readonly IEnvironmentAwareReporter DefaultReporter = QuietReporter.INSTANCE ;
        private static readonly Dictionary<string, IEnvironmentAwareReporter> Reporters = new Dictionary
            <string, IEnvironmentAwareReporter>
        {
            {"WinMerge", WinMergeReporter.INSTANCE},
            {"BeyondCompare", BeyondCompareReporter.INSTANCE},
            {"Diff", DiffReporter.INSTANCE},
        };

        private readonly IEnvironmentAwareReporter _selectedReporter;        

        public CustomReporter()
        {
            var envVariable = Environment.GetEnvironmentVariable("ApprovalsReporter");
            if (envVariable == null || !Reporters.TryGetValue(envVariable, out _selectedReporter))
            {
                _selectedReporter = DefaultReporter;
            }
        }

        public void Report(string approved, string received)
        {
            if (_selectedReporter.IsWorkingInThisEnvironment(received))
                _selectedReporter.Report(approved, received);
            else
                DefaultReporter.Report(approved, received);
        }

        public bool IsWorkingInThisEnvironment(string forFile)
        {
            return _selectedReporter.IsWorkingInThisEnvironment(forFile) || DefaultReporter.IsWorkingInThisEnvironment(forFile);
        }
    }
#else
    public class CustomReporter
    {
        
    }
#endif
}
