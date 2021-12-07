using System.Collections.Generic;
using RobotProject.Form2Items;

namespace RobotProject
{
    public static class Buffer
    {
        private static readonly List<List<Signal>> Buffered;

        static Buffer()
        {
            Buffered = new List<List<Signal>>(5);
            for (int i = 0; i < 5; i++)
            {
                Buffered[i] = new List<Signal>();
            }
        }

        public static void Add(Signal s, int r)
        {
            Buffered[r].Add(s);
        }

        public static Signal Pop(int r)
        {
            var res = Buffered[r][0];
            Buffered[r].RemoveAt(0);
            return res;
        }

        public static bool Empty(int r)
        {
            return Buffered[r].Count == 0;
        }
    }
}