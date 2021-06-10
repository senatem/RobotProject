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

        public bool Full()
        {
            return _holding == _orderSize;
        }

        public int GetCellType()
        {
            return _type;
        }
    }
}