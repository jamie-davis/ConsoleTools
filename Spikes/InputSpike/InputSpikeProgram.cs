using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace InputSpike
{
    internal class InputSpikeProgram
    {
        private static void Main()
        {
            Console.WriteLine(Console.In.GetType());
            Console.WriteLine(Console.IsInputRedirected);
            Console.WriteLine(ConsoleEx.IsInputRedirected);
            Console.WriteLine("Window: Width = {0}, Height = {1}", Console.WindowWidth, Console.WindowHeight);
            Console.WriteLine("Buffer: Width = {0}, Height = {1}", Console.BufferWidth, Console.BufferHeight);
            Console.CursorTop = Console.CursorTop;
            var template = new
            {
                Str = Get<string>("Name"),
                Int = Get<int>("Age"),
                LegCount = Get<int>("How many legs"),
            };
            var input = GetInput(template);

            Console.WriteLine("Result: {0}", input);
            Console.WriteLine("Name = {0}", template.Str.Value);
        }


        private static T GetInput<T>(T template) where T: notnull
        {
            var ctor = template.GetType().GetConstructors().Single();
            var args = new List<object>();
            foreach (var property in template.GetType().GetProperties())
            {
                Console.WriteLine("{0} ({1}, {2})", property.Name, property.PropertyType, property.CanWrite);
                if (property.PropertyType == typeof (string))
                    args.Add(string.Empty);
                else
                {
                    var value = property.GetValue(template) as IInputItem;
                    object? captured = null;
                    if (value == null)
                        captured = Activator.CreateInstance(property.PropertyType);
                    else
                    {
                        captured = value.Capture();
                    }
                    args.Add(captured);
                }
            }
            var item = (T) ctor.Invoke(args.ToArray());

            return item;
        }

        private static InputItem<T> Get<T>(string prompt)
        {
            return new InputItem<T>(prompt);
        }
    }

    internal class InputItem<T> : IInputItem 
    {    
        public string? Prompt { get; set; }

        public static implicit operator T?(InputItem<T> item)
        {
            return item.Value;
        }

        private InputItem(){}

        internal InputItem(string prompt)
        {
            Prompt = prompt;
        }

        public T? Value { get; private set; }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

        object? IInputItem.Capture()
        {
            Console.Write(Prompt);
            Console.Write(" : ");
            var line = Console.ReadLine();
            var conversion = typeof (Convert).GetMethods()
                    .FirstOrDefault(m => m.ReturnType == typeof (T) 
                        && m.GetParameters().Length == 1 
                        && m.GetParameters()[0].ParameterType == typeof (string));
            if (conversion != null)
            {
                Value = (T)conversion.Invoke(null, new object[] {line});
                return this;
            }

            return null;
        }
    }

    internal interface IInputItem
    {
        object Capture();
    }


    public static class ConsoleEx
    {
        public static bool IsInputRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin)); }
        }

        public static bool IsErrorRedirected
        {
            get { return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr)); }
        }

        // P/Invoke:
        private enum FileType
        {
            Unknown,
            Disk,
            Char,
            Pipe
        };

        private enum StdHandle
        {
            Stdin = -10,
            Stdout = -11,
            Stderr = -12
        };

        [DllImport("kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hdl);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
    }
}
