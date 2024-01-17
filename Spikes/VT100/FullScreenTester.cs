﻿using System;
using VT100.Attributes;
using VT100.FullScreen;
using VT100.FullScreen.ControlBehaviour;
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

        [Screen]
        [Border(BorderType.Normal)]
        [Background(VtColour.Blue)]
        [InputBackground(VtColour.Yellow)]
        [CaptionPosition(CaptionPosition.Left)]
        public class Layout : ILayout
        {
            #region Implementation of ILayout

            public event LayoutUpdated LayoutUpdated;

            #endregion

            [TextBox("Real Name")]
            [Border(BorderType.Normal)]
            [InputForeground(VtColour.Black)]
            public string Name { get; set; }

            [TextBox("Nickname")]
            [InputBackground(VtColour.Red)]
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