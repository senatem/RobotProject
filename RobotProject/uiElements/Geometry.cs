using System;

namespace RobotProject.uiElements
{
    public static class Geometry
    {
        public class Rectangle
            {
                public Rectangle(float w, float h, Point p)
                {
                    float x1;
                    float x;
                    W = w;
                    H = h;
                    _x = p.X;
                    _y = p.Y;
                    L = _x - w / 2;
                    _r = _x + w / 2;
                    T = _y - h / 2;
                    _b = _y + h / 2;
                }
        
                public Rectangle(float w1, float w2, float h1, float h2)
                {
                    float x1;
                    float x;
                    _b = Math.Max(h1, h2);
                    T = Math.Min(h1, h2);
                    L = Math.Min(w1, w2);
                    _r = Math.Max(w1, w2);
                    _x = (L + _r) / 2;
                    _y = (T + _b) / 2;
                    W = _r - L;
                    H = _b - T;
                }
        
                /** slice vertical slices the rectangle to floats ratios of 1
                 * 
                 */
                public Rectangle SliceVertical(float wStart, float wEnd)
                {
                    return new Rectangle((int )(L + W * wStart), (int)( L + W * wEnd), _b, T);
                }
                
                public Rectangle SliceHorizontal(float hStart, float hEnd)
                {
                    return new Rectangle(L,_r,(int )(T + H * hStart), (int)( T + H * hEnd));
                }

                /** Takes a rectangle, acts like this is unit rectangle and yields the resultingly sliced rectangle
                 */
                public Rectangle SubRectangle(Rectangle other)
                {
                    return new Rectangle( L + W * other.L, L + W * other._r, T + H * other._b,
                        T + H * other.T);
                }

                private readonly float _x;
                private readonly float _y;
                public readonly float W;
                public readonly float H;
                public readonly float L;
                private readonly float _r;
                public readonly float T;
                private readonly float _b;
            }
        
            public class Point
            {
                public Point(float x, float y)
                {
                    X = x;
                    Y = y;
                }
                public readonly float X;
                public readonly float Y;
            }
    }
}