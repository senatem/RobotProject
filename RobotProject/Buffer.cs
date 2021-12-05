using System.Collections.Generic;
using RobotProject.Form2Items;

namespace RobotProject
{
    public class Buffer
    {
        private static readonly List<List<Signal>> _buffer = new List<List<Signal>>(5);

        public void Add(Signal s, int r)
        {
            _buffer[r].Add(s);
        }

        public Signal Pop(int r)
        {
            var res = _buffer[r][0];
            _buffer[r].RemoveAt(0);
            return res;
        }

        public bool Empty(int r)
        {
            return _buffer[r].Count == 0;
        }
    }
}