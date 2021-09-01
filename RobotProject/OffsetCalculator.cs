using System;
using System.Linq;
using RobotProject.uiElements;

namespace RobotProject
{
    public class Offsets
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public readonly int Pattern;
        public readonly int Kat;
        public readonly int Rotation;

        public Offsets(int x, int y, int z, int pattern, int kat, int rotation)
        {
            X = x;
            Y = y;
            Z = z;
            Pattern = pattern;
            Kat = kat;
            Rotation = rotation;
        }
    }

    public class OffsetCalculator
    {
        private readonly ExcelReader _er = new ExcelReader(References.ProjectPath + "Ek-4.xlsx");

        private int GetPattern(int px, int py, int yontemKodu, int type)
        {
            string[] fields = {"Yontem_Kodu", "Tip", "Yukseklik", "Uzunluk"};
            int[] values = {yontemKodu, type, px-px%100, py-py%100};
            var pattern = (int) (double) _er.Find(fields, values).Rows[0]["Paletleme_Sekli"];
            return pattern;
        }

        private bool GetRotation(int px, int py, int yontemKodu, int type)
        {
            string[] fields = {"Yontem_Kodu", "Tip", "Yukseklik", "Uzunluk"};
            int[] values = {yontemKodu, type, px-px%100, py-py%100};
            var name = (long) (double) _er.Find(fields, values).Rows[0]["Name"];
            long[] toRotate =
            {
                1521111160040,
                1521111170060,
                1521112270060,
                1522422290040,
                1535422290040,
                1522113370050,
                1522113390050,
                1521111170040,
                1521111170060,
                1521112270040,
                1521112270060,
                1521112290040,
                1521112290060,
                1532412150040,
                1532412160050,
                1532412250040,
                1532412260040,
                1532412260050,
                1522111160040,
                1522111160050,
                1522112260040,
                1522112260050,
                1542112260040,
                1542112260050,
                1522112290040,
                1522112290050,
                1522112290060,
                1542112290040,
                1542112290050,
                1542112290060,
                1522111160040,
                1522111160050,
                1522112260040,
                1522112260050,
                1542112260040,
                1542112260050,
                1522112290040,
                1522112290050,
                1522112290060,
                1542112290040,
                1542112290050,
                1542112290060,
                1521111160040,
                1521111160050,
                1521112160040,
                1521112160050,
                1521112260040,
                1521112260050,
                1521112260040,
                1521112290040,
                1521112290050,
                1521113390040,
                1521122260040,
                1521122290040,
                1521123390040,
                1541112260040,
                1541112290040,
                1541113390040,
                1531112260040,
                1531112290040,
                1541112260040,
                1541112290040,
                1541112260040,
                1541112260050,
                1521111160040,
                1521111160050,
                1521112160040,
                1521112160050,
                1521112260040,
                1521112260050,
                1521111160040,
                1521111160050,
                1521111170040,
                1521111170060,
                1521112160050,
                1521112270040,
                1521112270060,
            };
            if (toRotate.Contains(name))
            {
                return true;
            }

            int[][] rotateNameless =
            {
                new [] {1, 21, 700, 400},
                new [] {1, 21, 700,	500},
                new [] {1, 21, 700,	600},
                new [] {1, 21, 800,	400},
                new [] {1, 21,	800,	500},
                new [] {1, 21,	800,	600},
                new [] {1, 21,	900,	400},
                new [] {1, 21,	900,	500},
                new [] {1, 21,	900,	600},
                new [] {1, 22,	700,	400},
                new [] {1, 22,	700	, 500},
                new [] {1, 22,	700,	600},
                new [] {1, 22,	800,	400},
                new [] {1, 22,	800,	500},
                new [] {1, 22,	800,	600},
                new [] {1, 22,	900,	400},
                new [] {1, 22,	900,	500},
                new [] {1, 22,	900,	600},
                new [] {1, 33,	700,	400},
                new [] {1, 33,	700,	500},
                new [] {1, 33,	700,	600},
                new [] {1, 33,	800,	400},
                new [] {1, 33,	800,	500},
                new [] {1, 33,	800,	600},
                new [] {1, 33,	900,	400},
                new [] {1, 33,	900,	500},
                new [] {1, 33,	900,	600}
            };

            return rotateNameless.Contains(values);
        }

        public Offsets Calculate(int px, int py, int pz, int counter, int yontemKodu, int type, int palletHeight, int palletWidth)
        {
            var p = GetPattern(px, py, yontemKodu, type);
            var r = GetRotation(px, py, yontemKodu, type);

            return p switch
            {
                1 => PatternOne(pz, counter, palletHeight, palletWidth),
                2 => PatternTwo(px, py, pz, counter, palletHeight, palletWidth, r),
                3 => PatternThree(px, pz, counter, palletHeight, palletWidth),
                4 => PatternFour(px, py, pz, counter, palletHeight, palletWidth, r),
                5 => PatternFive(px, py, pz, counter, palletHeight, palletWidth, r),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternOne(int pz, int counter, int palletHeight, int palletWidth)
        {
            return new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (counter-1) + palletHeight, 1, counter, 0);
        }


        private static Offsets PatternTwo(int px, int py, int pz, int counter, int palletHeight, int palletWidth, bool toRotate)
        {
            if (toRotate)
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(-py - 30, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2)-1) + palletHeight, 2, (counter + 1) / 2, 90),
                    1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2)-1) + palletHeight, 2, (counter + 1) / 2, 270),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return (counter % 2) switch
            {
                0 => new Offsets(-px - 30, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2)-1) + palletHeight, 2, (counter + 1) / 2, 0),
                1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2)-1) + palletHeight, 2, (counter + 1) / 2, 0),
                _ => throw new ArgumentOutOfRangeException()
            };

        }

        private static Offsets PatternThree(int px, int pz, int counter, int palletHeight, int palletWidth)
        {
            return (counter % 3) switch
            {
                0 => new Offsets(-2*px - 70, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 2) / 3) - 1) + palletHeight, 3, (counter + 2) / 3, 0),
                1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 2) / 3) - 1) + palletHeight, 3, (counter + 2) / 3, 180),
                2 => new Offsets(-px - 30, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 2) / 3) - 1)+ palletHeight, 3, (counter + 2) / 3, 180),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternFour(int px, int py, int pz, int counter, int palletHeight, int palletWidth, bool toRotate)
        {
            if (toRotate)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(-py - 30, -px/2 - ((palletWidth * 10) % 200) / 2 - 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 270),
                    1 => new Offsets(0,  px/2 - ((palletWidth * 10) % 200) / 2 + 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 90),
                    2 => new Offsets(0, -px/2 - ((palletWidth * 10) % 200) / 2 - 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 270),
                    3 => new Offsets(-py - 30,  px/2 - ((palletWidth * 10) % 200) / 2 + 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 90),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            return (counter % 4) switch
            {
                0 => new Offsets(-px - 30, -py/2 - ((palletWidth * 10) % 200) / 2 - 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 0),
                1 => new Offsets(0,  py/2 - ((palletWidth * 10) % 200) / 2 + 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 180),
                2 => new Offsets(0, -py/2 - ((palletWidth * 10) % 200) / 2 - 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 180),
                3 => new Offsets(-px - 30,  py/2 - ((palletWidth * 10) % 200) / 2 + 10, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private static Offsets PatternFive(int px, int py, int pz, int counter, int palletHeight, int palletWidth, bool toRotate)
        {
            if (toRotate)
            {           
                return (counter % 2) switch
                {
                    0 => new Offsets(0 - 30, px/2 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 270),
                    1 => new Offsets(0, px/2 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 270),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
            }

            return (counter % 2) switch
            {
                0 => new Offsets(0 - 30, py / 2 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 0),
                1 => new Offsets(0, py / 2 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 180),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}