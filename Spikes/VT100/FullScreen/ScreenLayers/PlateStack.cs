using System;
using System.Linq;
using System.Text;

namespace VT100.FullScreen.ScreenLayers
{
    internal class PlateStack
    {
        private readonly Plate[] _plates;

        public PlateStack(params Plate[] plates)
        {
            _plates = plates;
        }

        public void Render(IFullScreenConsole console)
        {
            var sb = new StringBuilder();
            using (new CursorHider())
            {
                console.SetCursorPosition(0,0);
                for (var line = 0; line < console.WindowHeight; line++)
                {
                    for (var index = 0; index < console.WindowWidth; index++)
                    {
                        sb.Append(TakeCharacterFromStack(index, line));
                    }
                }
                
                console.Write(sb.ToString());
            }
            
        }

        private string TakeCharacterFromStack(int index, int line)
        {
            foreach (var plate in _plates.Reverse())
            {
                var character = plate.GetCharacter(index, line);
                if (character != null)
                    return character;
            }

            return " ";
        }
    }
}