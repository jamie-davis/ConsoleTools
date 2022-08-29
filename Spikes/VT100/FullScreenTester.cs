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
                var layout = new Layout() { Name = "Test", NickName = "Wolf" };
                using (var fsa = new FullScreenApplication(layout, vtMode))
                    fsa.Run();

                Console.WriteLine(layout.Name);
            }
        }

        [Screen()]
        public class Layout : ILayout
        {
            #region Implementation of ILayout

            public event LayoutUpdated LayoutUpdated;

            #endregion

            [TextBox("Real Name")]
            public string Name { get; set; }

            [TextBox("Nickname")]
            public string NickName { get; set; }

            [Button("Default")]
            public void SetDefaults()
            {
                Name = "Default name";
                NickName = "Defaulty";
            }
            
            [Button("OK", ExitMode.ExitOnSuccess)]
            public bool OK()
            {
                return true;
            }
        }
    }
}