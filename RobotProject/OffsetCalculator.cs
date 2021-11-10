using System;
using System.Linq;
using System.Windows.Forms;
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
        public readonly int NextRotation;

        public Offsets(int x, int y, int z, int pattern, int kat, int rotation, int nextRotation)
        {
            X = x;
            Y = y;
            Z = z;
            Pattern = pattern;
            Kat = kat;
            Rotation = rotation;
            NextRotation = nextRotation;
        }
    }

    public class OffsetCalculator
    {
        public readonly ExcelReader Er = new ExcelReader(References.ProjectPath + "Paletleme.xlsx");

        private int GetPattern(int px, int py, int yontemKodu, int type)
        {
            try
            {
                string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk"};
                int[] values = {yontemKodu, type, px - px % 100, py - py % 100};
                var t = Er.Find(fields, values).Rows[0];
                var pattern = (int) (double) Er.Find(fields, values).Rows[0]["PaletlemeSekli"];
                return pattern;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }

        }

        private bool GetPriority(int px, int py, int yontemKodu, int type)
        {
            string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk"};
            int[] values = {yontemKodu, type, px - px % 100, py - py % 100};

            var db1 = Er.Find(fields, values).Rows[0];

            return db1.IsNull("KlavuzEtiket");
        }

        private string GetRotation(int px, int py, int yontemKodu, int type, int pH, int pW)
        {
            string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk"};
            int[] values = {yontemKodu, type, px - px % 100, py - py % 100, pH, pW};
            var rotation = Er.Find(fields, values).Rows[0]["Rotation"];
            return (string) rotation;
        }
        
        public Offsets Calculate(int px, int py, int pz, int counter, int yontemKodu, int type, int palletHeight,
            int palletWidth, int palletZ)
        {
            var p = GetPattern(px, py, yontemKodu, type);
            
            var r = "";
            var pri = false;
            
            if (p != 1)
            { 
                r = GetRotation(px, py, yontemKodu, type, palletHeight, palletWidth);
                
                pri = GetPriority(px, py, yontemKodu, type);
                
                
            }

            return p switch
            {
                1 => PatternOne(pz, counter, palletHeight, palletWidth, palletZ),
                2 => PatternTwo(px, py, pz, counter, palletHeight, palletWidth, r, palletZ),
                3 => PatternThree(px, pz, counter, palletHeight, palletWidth, palletZ),
                4 => PatternFour(px, py, pz, counter, palletHeight, palletWidth, r, pri, palletZ),
                5 => PatternFive(px, py, pz, counter, palletHeight, palletWidth, r, palletZ),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternOne(int pz, int counter, int palletHeight, int palletWidth, int palletZ)
        {
            return new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (counter - 1) + palletZ, 1, counter, 0, 0);
        }


        private static Offsets PatternTwo(int px, int py, int pz, int counter, int palletHeight, int palletWidth,
            string rotation, int palletZ)
        {
            if (rotation == "90-270")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 270, 90),
                    1 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 90, 270),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return (counter % 2) switch
            {
                0 => new Offsets(-px - 20, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 0, 180),
                1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2) - 1) + palletZ,
                    2, (counter + 1) / 2, 180, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternThree(int px, int pz, int counter, int palletHeight, int palletWidth, int palletZ)
        {
            return (counter % 3) switch
            {
                0 => new Offsets(-2 * px - 50, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 2) / 3) - 1) + palletZ, 3, (counter + 2) / 3, 0, 180),
                1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 2) / 3) - 1) + palletZ,
                    3, (counter + 2) / 3, 180, 0),
                2 => new Offsets(-px - 20, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 2) / 3) - 1) + palletZ, 3, (counter + 2) / 3, 0, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternFour(int px, int py, int pz, int counter, int palletHeight, int palletWidth,
            string rotation, bool priority, int palletZ)
        {
            if (rotation == "90-270" && !priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 90),
                    1 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 90),
                    2 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 270),
                    3 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 270),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "90-270" && priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 270),
                    1 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 90),
                    2 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 270),
                    3 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 90),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "0-180" && !priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180),
                    1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 0),
                    2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180),
                    3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 0),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return (counter % 4) switch
            {
                0 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180),
                1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 180),
                2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 0),
                3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternFive(int px, int py, int pz, int counter, int palletHeight, int palletWidth,
            string rotation, int palletZ)
        {
            if (rotation == "90-270")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 90, 270),
                    1 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 270, 90),
                };
            }

            return (counter % 2) switch
            {
                0 => new Offsets(0 - 20, py / 2 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 0, 180),
                1 => new Offsets(0, py / 2 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 180, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternSix(int px, int py, int pz, int counter, int palletHeight, int palletWidth, int palletZ)
        {
            return (counter % 6) switch
            {
                0 => new Offsets(-2 * px - 50, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 0, 180),
                1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 180, 0),
                2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 0, 180),
                3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 180, 0),
                4 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 0, 180),
                5 => new Offsets(-2 * px - 50, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 180, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}