namespace RobotProject
{
    public class Cell
    {
        private readonly long _orderNo;
        private readonly int _robotNo;
        private readonly int _orderSize;
        private int _holding;
        private readonly int _palletHeight;
        private int _palletWidth;

        public Cell(long orderNo, int robotNo, int orderSize, int palletHeight, int palletWidth)
        {
            _orderNo = orderNo;
            _robotNo = robotNo;
            _orderSize = orderSize;
            _palletHeight = palletHeight;
            _palletWidth = palletWidth;
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

        public long GetCellType()
        {
            return _orderNo;
        }

        public int GetPalletHeight()
        {
            return _palletHeight;
        }

        public int GetRobotNo()
        {
            return _robotNo;
        }

        public int GetPalletWidth()
        {
            return _palletWidth;
        }
    }
}