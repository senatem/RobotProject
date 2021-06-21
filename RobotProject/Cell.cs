namespace RobotProject
{
    public class Cell
    {
        private readonly long _orderNo;
        private readonly int _orderSize;
        private int _holding;
        private readonly int _palletHeight;

        public Cell(long orderNo, int orderSize, int palletHeight)
        {
            _orderNo = orderNo;
            _orderSize = orderSize;
            _palletHeight = palletHeight;
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
    }
}