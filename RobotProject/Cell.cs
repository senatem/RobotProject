namespace RobotProject
{
    public class Cell
    {
        private readonly int _type;
        private readonly int _orderSize;
        private int _holding;

        public Cell(int type, int orderSize)
        {
            _type = type;
            _orderSize = orderSize;
        }
        
        public void AddProduct()
        {
            _holding++;
        }

        public int GetCounter()
        {
            return _holding;
        }
        public int Full()
        {
            return _holding == _orderSize ? 1 : 0;
        }

        public int GetCellType()
        {
            return _type;
        }
    }
}