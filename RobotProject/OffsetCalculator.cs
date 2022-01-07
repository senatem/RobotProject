using System;
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
        public readonly int NextKat;

        public Offsets(int x, int y, int z, int pattern, int kat, int rotation, int nextRotation, int nextKat)
        {
            X = x;
            Y = y;
            Z = z;
            Pattern = pattern;
            Kat = kat;
            Rotation = rotation;
            NextRotation = nextRotation;
            NextKat = nextKat;
        }
    }

    public class OffsetCalculator
    {
       //public readonly ExcelReader Er = new ExcelReader("C:\\Users\\deneme.robot8\\Desktop\\Paletleme.xlsx");
        public readonly ExcelReader Er = new ExcelReader(References.ProjectPath + "Paletleme.xlsx");
        
        private int GetPattern(int px, int py, int yontemKodu, int type)
        {
            try
            {
                string[] fields = {"YontemKodu", "Tip", "Yukseklik", "Uzunluk"};
                int[] values = {yontemKodu, type, px - px % 100, py - py % 100};
                var pattern = (int) (double) Er.Find(fields, values).Rows[0]["PaletlemeSekli"];
                return pattern;
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Ürün paletleme listesinde bulunamadı.mn");
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
                1 => PatternOne(pz + 2, counter, palletWidth, palletZ),
                2 => PatternTwo(px, pz + 2, counter, palletWidth, r, palletZ),
                3 => PatternThree(px, pz + 2, counter, palletWidth, palletZ),
                4 => PatternFour(px, py, pz + 2, counter, palletWidth, r, pri, palletZ),
                5 => PatternFive(py, pz + 2, counter, palletWidth, r, palletZ),
                6 => PatternSix(px, py, pz + 2, counter, palletWidth, palletZ),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternOne(int pz, int counter, int palletWidth, int palletZ)
        {
            return new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (counter - 1) + palletZ, 1, counter, 0, 0,
                counter + 1);
        }


        private static Offsets PatternTwo(int px, int pz, int counter, int palletWidth,
            string rotation, int palletZ)
        {
            if (rotation == "90-270")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 270, 90,
                        (counter + 2) / 2),
                    1 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 90, 270,
                        (counter + 2) / 2),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "0-180")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(-px - 20, 0 + ((palletWidth * 10) % 200) / 2,
                        pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 0, 180, (counter + 2) / 2),
                    1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2) - 1) + palletZ,
                        2, (counter + 1) / 2, 180, 0, (counter + 2) / 2),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "90-90")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(-px - 20, 0 + ((palletWidth * 10) % 200) / 2,
                        pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 0, 0, (counter + 2) / 2),
                    1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 1) / 2) - 1) + palletZ,
                        2, (counter + 1) / 2, 0, 0, (counter + 2) / 2),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return new Offsets(0, 0, 0, 0, 0, 0, 0, 0);
        }

        private static Offsets PatternThree(int px, int pz, int counter, int palletWidth, int palletZ)
        {
            return (counter % 3) switch
            {
                0 => new Offsets(-2 * px - 50, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 2) / 3) - 1) + palletZ, 3, (counter + 2) / 3, 0, 180, (counter + 3) / 3),
                1 => new Offsets(0, 0 + ((palletWidth * 10) % 200) / 2, pz * (((counter + 2) / 3) - 1) + palletZ,
                    3, (counter + 2) / 3, 180, 0, (counter + 3) / 3),
                2 => new Offsets(-px - 20, 0 + ((palletWidth * 10) % 200) / 2,
                    pz * (((counter + 2) / 3) - 1) + palletZ, 3, (counter + 2) / 3, 0, 0, (counter + 3) / 3),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Offsets PatternFour(int px, int py, int pz, int counter, int palletWidth,
            string rotation, bool priority, int palletZ)
        {
            if (rotation == "90-270" && !priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 90,
                        (counter + 4) / 4),
                    1 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 90,
                        (counter + 4) / 4),
                    2 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 270,
                        (counter + 4) / 4),
                    3 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 270,
                        (counter + 4) / 4),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "90-270" && priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 270,
                        (counter + 4) / 4),
                    1 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 90,
                        (counter + 4) / 4),
                    2 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 90, 270,
                        (counter + 4) / 4),
                    3 => new Offsets(0, 0, pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 270, 90,
                        (counter + 4) / 4),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "0-180" && !priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180, (counter + 4) / 4),
                    1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 0, (counter + 4) / 4),
                    2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180, (counter + 4) / 4),
                    3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 0, (counter + 4) / 4),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "0-180" && priority)
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180, (counter + 4) / 4),
                    1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 180, (counter + 4) / 4),
                    2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 180, 0, (counter + 4) / 4),
                    3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 180, (counter + 4) / 4),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "90-90")
            {
                return (counter % 4) switch
                {
                    0 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 0, (counter + 4) / 4),
                    1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 0, (counter + 4) / 4),
                    2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 0, (counter + 4) / 4),
                    3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                        pz * (((counter + 3) / 4) - 1) + palletZ, 4, (counter + 3) / 4, 0, 0, (counter + 4) / 4),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return new Offsets(0, 0, 0, 0, 0, 0, 0, 0);
        }

        private static Offsets PatternFive(int py, int pz, int counter, int palletWidth,
            string rotation, int palletZ)
        {
            if (rotation == "90-270")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 90, 270,
                        (counter + 2) / 2),
                    1 => new Offsets(0, 0, pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 270, 90,
                        (counter + 2) / 2),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "0-180")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0 - 20, py / 2 + ((palletWidth * 10) % 200) / 2,
                        pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 0, 180, (counter + 2) / 2),
                    1 => new Offsets(0, py / 2 + ((palletWidth * 10) % 200) / 2,
                        pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 180, 0, (counter + 2) / 2),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (rotation == "90-90")
            {
                return (counter % 2) switch
                {
                    0 => new Offsets(0 - 20, py / 2 + ((palletWidth * 10) % 200) / 2,
                        pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 0, 0, (counter + 2) / 2),
                    1 => new Offsets(0, py / 2 + ((palletWidth * 10) % 200) / 2,
                        pz * (((counter + 1) / 2) - 1) + palletZ, 2, (counter + 1) / 2, 0, 0, (counter + 2) / 2),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return new Offsets(0, 0, 0, 0, 0, 0, 0, 0);
        }

        private static Offsets PatternSix(int px, int py, int pz, int counter, int palletWidth,
            int palletZ)
        {
            return (counter % 6) switch
            {
                0 => new Offsets(-2 * px - 50, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 0, 180, (counter + 6) / 6),
                1 => new Offsets(0, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 180, 0, (counter + 6) / 6),
                2 => new Offsets(0, -py / 2 - ((palletWidth * 10) % 200) / 2 - 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 0, 180, (counter + 6) / 6),
                3 => new Offsets(-px - 20, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 180, 0, (counter + 6) / 6),
                4 => new Offsets(-px - 20, -py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 0, 180, (counter + 6) / 6),
                5 => new Offsets(-2 * px - 50, py / 2 - ((palletWidth * 10) % 200) / 2 + 10,
                    pz * (((counter + 5) / 6) - 1) + palletZ, 6, (counter + 5) / 6, 180, 0, (counter + 6) / 6),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}