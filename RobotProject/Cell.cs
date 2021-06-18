namespace RobotProject
{
    public class Cell
    {
        private readonly int _orderNo;
        private readonly int _orderSize;
        private int _holding;

        public Cell(int orderNo, int orderSize)
        {
            _orderNo = orderNo;
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
            return _orderNo;
        }
    }
}