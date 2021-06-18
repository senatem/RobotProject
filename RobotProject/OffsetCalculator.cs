using System;

namespace RobotProject
{
    public class Offsets
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public readonly int Pattern;
        public readonly int Kat;

        public Offsets(int x, int y, int z, int pattern, int kat)
        {
            X = x;
            Y = y;
            Z = z;
            Pattern = pattern;
            Kat = kat;
        }
    }
    public class OffsetCalculator
    {
        private readonly ExcelReader _er = new ExcelReader("Ek-4.xlsx");

        private int GetPattern(int px, int py, int yontemKodu, int type)
        {
            string[] fields = {"Yöntem Kodu", "Tip", "Yükseklik", "Uzunluk"};
            int[] values = {yontemKodu, type, px, py};
            var pattern = (int) _er.Find(fields, values).Rows[0][7];
            return pattern;
        }
        
        public Offsets Calculate(int px, int py, int pz, int counter, int yontemKodu, int type)
        {
            var p = GetPattern(px, py, yontemKodu, type);
            
            return (p) switch
            {
                1 => PatternOne(px, py, pz, counter),
                2 => PatternTwo(px, py, pz, counter),
                3 => PatternThree(px, py, pz, counter),
                4 => PatternFour(px, py, pz, counter),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternOne(int px, int py, int pz, int counter)
        {
            return  new Offsets(px / 2, py / 2, pz * counter, 1, counter);
        }


        private static Offsets PatternTwo(int px, int py, int pz, int counter)
        {
            return (counter % 2) switch
            {
                0 => new Offsets(px / 4, py / 2, pz * ((counter + 1) / 2), 2, (counter+1)/2),
                1 => new Offsets(3 * px / 4, py / 2, pz * ((counter + 1) / 2), 2, (counter+1)/2),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternThree(int px, int py, int pz, int counter)
        {
            return (counter % 3) switch
            {
                0 => new Offsets(px / 6, py / 2, pz * ((counter + 2) / 3), 3, ((counter + 2) / 3)),
                1 => new Offsets(3 * px / 6, py / 2, pz * ((counter + 2) / 3), 3, ((counter + 2) / 3)),
                2 => new Offsets(5 * px / 6, py / 2, pz * ((counter + 2) / 3), 3, ((counter + 2) / 3)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternFour(int px, int py, int pz, int counter)
        {
            return (counter % 4) switch
            {
                0 => new Offsets(px / 4, py / 4, pz * ((counter + 3) / 4), 4, ((counter + 3) / 4)),
                1 => new Offsets(px / 4, 3 * py / 4, pz * ((counter + 3) / 4), 4, ((counter + 3) / 4)),
                2 => new Offsets(3 * px / 4, py / 4, pz * ((counter + 3) / 4), 4, ((counter + 3) / 4)),
                3 => new Offsets(3 * px / 4, 3 * py / 4, pz * ((counter + 3) / 4), 4, ((counter + 3) / 4)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}