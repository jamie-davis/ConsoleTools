using System;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    public static class Read
    {
        public static Read<bool> Boolean() { return new Read<bool>(); }
        public static Read<int> Int() { return new Read<int>(); }
        public static Read<long> Long() { return new Read<long>(); }
        public static Read<double> Double(){ return new Read<double>();}
        public static Read<decimal> Decimal(){ return new Read<decimal>();}
        public static Read<DateTime> DateTime(){ return new Read<DateTime>();}
        public static Read<string> String(){ return new Read<string>();}
    }
}
