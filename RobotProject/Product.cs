namespace RobotProject
{
    public class Product
    {
        private readonly int _height, _width, _type;
        private readonly int _orderSize;
        public Product(int height, int width, int type, float orderSize)
        {
            _height = height;
            _width = width;
            _type = type;
            _orderSize = (int) orderSize;
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
    }
}