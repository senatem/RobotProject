namespace RobotProject
{
    public class Pallet
    {
        private readonly int _length;
        private readonly int _height;
        private readonly int _type;
        private readonly int _max;

        public Pallet(int h, int l, int t, int m)
        {
            _length = l;
            _height = h;
            _type = t;
            _max = m;
        }

        public int GetHeight()
        {
            return _height;
        }
        public int GetLength()
        {
            return _length;
        }

        public int GetPalletType()
        {
            return _type;
        }
        public int GetMax()
        {
            return _max;
        }
    }
}