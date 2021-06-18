using System;
using System.Collections.Generic;

namespace RobotProject.uiElements
{
    public static class Geometry
    {
        public class Rectangle
            {
                public Rectangle(float w, float h, Point p)
                {
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

                public List<Rectangle> Split(int rows, int cols, float wPad=0f, float hPad=0f)
                {
                    var a = new List<Rectangle>();
                    for(float i=0;i<cols;i++)
                    {
                        for(float j=0;j<rows;j++)
                        {
                            a.Add(new Rectangle(L + W / cols * i, L + W / cols * (i + 1), T + H / rows * j,
                                T + H / rows * (j + 1)).SubRectangle(new Rectangle(wPad,1-wPad,hPad,1-hPad))
                            
                            );
                        }
                    }
                    return a;
                }

                public Rectangle FittingSquare(float n = 1f)
                {
                    return new Rectangle(ShortSide()*n, ShortSide()*n, new Point(_x, _y));
                }

                private float ShortSide()
                {
                    return Math.Min(W, H);
                }
                
                public float LongSide()
                {
                    return Math.Max(W, H);
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