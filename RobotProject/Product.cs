namespace RobotProject
{
    public class Product
    {
        public readonly int Height, Width, Type;
        public readonly int OrderSize, YontemKodu;

        public Product(int height, int width, int type, float orderSize, string yontemKodu)
        {
            Height = height;
            Width = width;
            Type = type;
            OrderSize = (int) orderSize;
            YontemKodu = int.Parse(yontemKodu);
        }
        
        public int GetHeight()
        {
            return Height;
        }
        
        public int GetWidth()
        {
            return Width;
        }
        public int GetProductType()
        {
            return Type;
        }

        public int GetOrderSize()
        {
            return OrderSize;
        }

        public int GetYontem()
        {
            return YontemKodu;
        }
    }
}