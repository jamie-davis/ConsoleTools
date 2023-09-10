using System.Linq;
using System.Text;
using VT100.FullScreen.ControlBehaviour;

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

        public string TakeCharacterFromStack(int index, int line)
        {
            if (index >= PlateWidth || line >= PlateHeight) return " ";
            
            foreach (var plate in _plates.Reverse())
            {
                var character = plate.GetCharacter(index, line);
                if (character != null)
                    return character;
            }

            return " ";
        }
        
        public int PlateWidth => _plates.Length == 0 ? 0 : _plates[0].Width;
        public int PlateHeight => _plates.Length == 0 ? 0 : _plates[0].Height;

        public (char character, DisplayFormat displayFormat) GetCharacter(int column, int row)
        {
            foreach (var plate in _plates.Reverse())
            {
                var (character, format) = plate.GetCharacterAndFormat(column, row);
                if (character != 0)
                    return (character, format);
            }

            return (' ', DisplayFormat.NoChange);
        }

        public Plate this[int i] => _plates.Length > i ? _plates[i] : null;
    }
}