using System.Collections.Generic;
using RobotProject.Form2Items;

namespace RobotProject
{
    public static class Buffer
    {
        private static List<List<Signal>> _buffered = null!;

        public static void Init()
        {
            _buffered = new List<List<Signal>>(3);
            for (var i = 0; i < 3; i++)
            {
                var l = new List<Signal>(5);
                _buffered.Add(l);
            }
        }

        public static void Add(Signal s, int r)
        {
            _buffered[r].Add(s);
        }

        public static Signal Pop(int r)
        {
            var res = _buffered[r][0];
            _buffered[r].RemoveAt(0);
            return res;
        }

        public static bool Empty(int r)
        {
            return _buffered[r].Count == 0;
        }
    }
}