using System;
using VT100;
using VT100.Attributes;
using VT100.FullScreen;
using VT100.Utilities;

namespace VT100
{
    internal static class FullScreenTester
    {
        public static void Run()
        {
            using (var vtMode = new RequireVTMode())
            {
                var layout = new Layout() { Name = "Test" };
                using (var fsa = new FullScreenApplication(layout, vtMode))
                    fsa.Run();

                Console.WriteLine(layout.Name);
            }
        }

        public class Layout : ILayout
        {
            #region Implementation of ILayout

            public event LayoutUpdated LayoutUpdated;

            #endregion

            [TextBox("Name")]
            public string Name { get; set; }
        }
    }
}