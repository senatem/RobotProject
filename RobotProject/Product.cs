namespace RobotProject
{
    public class Product
    {
        private readonly int _height, _width, _type;
        private readonly int _orderSize, _yontemKodu;

        public Product(int height, int width, int type, float orderSize, string yontemKodu)
        {
            _height = height;
            _width = width;
            _type = type;
            _orderSize = (int) orderSize;
            _yontemKodu = int.Parse(yontemKodu);
        }
        
        public int GetHeight()
        {
            return _height;
        }
        
        public int GetWidth()
        {
            return _width;
        }
        public int GetProductType()
        {
            return _type;
        }

        public int GetOrderSize()
        {
            return _orderSize;
        }

        public int GetYontem()
        {
            return _yontemKodu;
        }
    }
}