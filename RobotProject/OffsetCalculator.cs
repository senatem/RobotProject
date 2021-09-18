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
        private readonly ExcelReader _er = new ExcelReader(References.ProjectPath + "Paletleme.xlsx");

        private int GetPattern(int px, int py, int yontemKodu, int type, int pH, int pW)
        {
            string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk", "PaletH", "PaletL"};
            int[] values = {yontemKodu, type, px - px % 100, py - py % 100, pH, pW};
            var pattern = (int) (double) _er.Find(fields, values).Rows[0]["PaletlemeSekli"];
            return pattern;
        }

        private bool GetPriority(int px, int py, int yontemKodu, int type, int pH, int pW)
        {
            string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk", "PaletH", "PaletL"};
            int[] values = {yontemKodu, type, px - px % 100, py - py % 100, pH, pW};

            var db1 = _er.Find(fields, values).Rows[0];
           // var db2 = (string) db1["KlavuzEtiket"];

            return db1.IsNull("KlavuzEtiket");
            
            //return string.IsNullOrEmpty(db2);
            
           // return (string) _er.Find(fields, values).Rows[0]["KlavuzEtiket"] == "";
            // klavuz-etiket -> false, üst kapak -> true
        }

        private string GetRotation(int px, int py, int yontemKodu, int type, int pH, int pW)
        {
            string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk", "PaletH", "PaletL"};
            int[] values = {yontemKodu, type, px - px % 100, py - py % 100, pH, pW};
            var rotation = _er.Find(fields, values).Rows[0]["Rotation"];
            return (string) rotation;
        }

        public Offsets Calculate(int px, int py, int pz, int counter, int yontemKodu, int type, int palletHeight,
            int palletWidth)
        {
            var p = GetPattern(px, py, yontemKodu, type, palletHeight, palletWidth);
            var r = GetRotation(px, py, yontemKodu, type, palletHeight, palletWidth);
            var pri = GetPriority(px, py, yontemKodu, type, palletHeight, palletWidth);

            return p switch
            {
                1 => PatternOne(pz, counter, palletHeight, palletWidth),
                2 => PatternTwo(px, py, pz, counter, palletHeight, palletWidth, r),
                3 => PatternThree(px, pz, counter, palletHeight, palletWidth),
                4 => PatternFour(px, py, pz, counter, palletHeight, palletWidth, r, pri),
                5 => PatternFive(px, py, pz, counter, palletHeight, palletWidth, r, pri),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternOne(int pz, int counter, int palletHeight, int palletWidth)
        {
            return new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (counter - 1) + palletHeight, 1, counter, 0);
        }


        private static Offsets PatternTwo(int px, int py, int pz, int counter, int palletHeight, int palletWidth,
            string rotation)
        {
            if (rotation == "90-270")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 270),
                    1 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 90),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return (counter % 2) switch
            {
                0 => new Offsets(-px - 20, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 0),
                1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2) - 1) + palletHeight,
                    2, (counter + 1) / 2, 180),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternThree(int px, int pz, int counter, int palletHeight, int palletWidth)
        {
            return (counter % 3) switch
            {
                0 => new Offsets(-2 * px - 50, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 2) / 3) - 1) + palletHeight, 3, (counter + 2) / 3, 0),
                1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 2) / 3) - 1) + palletHeight,
                    3, (counter + 2) / 3, 180),
                2 => new Offsets(-px - 20, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 2) / 3) - 1) + palletHeight, 3, (counter + 2) / 3, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternFour(int px, int py, int pz, int counter, int palletHeight, int palletWidth,
            string rotation, bool priority)
        {
            if (rotation == "90-270" && !priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 270),
                    1 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 90),
                    2 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 90),
                    3 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 270),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "90-270" && priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 90),
                    1 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 270),
                    2 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 90),
                    3 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 270),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "0-180" && !priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 0),
                    1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 180),
                    2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 0),
                    3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 180),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return (counter % 4) switch
            {
                0 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 0),
                1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 180),
                2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 180),
                3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 3) / 4) - 1) + palletHeight, 4, (counter + 3) / 4, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternFive(int px, int py, int pz, int counter, int palletHeight, int palletWidth,
            string rotation, bool priority)
        {
            if (rotation == "90-270")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 90),
                    1 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 270),
                };
            }

            return (counter % 2) switch
            {
                0 => new Offsets(0 - 20, py / 2 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 0),
                1 => new Offsets(0, py / 2 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletHeight, 2, (counter + 1) / 2, 180),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternSix(int px, int py, int pz, int counter, int palletHeight, int palletWidth)
        {
            return (counter % 6) switch
            {
                0 => new Offsets(-2 * px - 50, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 5) / 6) - 1) + palletHeight, 6, (counter + 5) / 6, 0),
                1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletHeight, 6, (counter + 5) / 6, 180),
                2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 5) / 6) - 1) + palletHeight, 6, (counter + 5) / 6, 0),
                3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletHeight, 6, (counter + 5) / 6, 180),
                4 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletHeight, 6, (counter + 5) / 6, 0),
                5 => new Offsets(-2 * px - 50, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletHeight, 6, (counter + 5) / 6, 180),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}