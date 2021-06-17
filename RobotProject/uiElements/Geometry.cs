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
                    this.w = w;
                    this.h = h;
                    x = p.x;
                    y = p.y;
                    l = x - w / 2;
                    r = x + w / 2;
                    t = y - h / 2;
                    b = y + h / 2;
                }
        
                public Rectangle(float w1, float w2, float h1, float h2)
                {
                    b = Math.Max(h1, h2);
                    t = Math.Min(h1, h2);
                    l = Math.Min(w1, w2);
                    r = Math.Max(w1, w2);
                    x = (l + r) / 2;
                    y = (t + b) / 2;
                    w = r - l;
                    h = b - t;
                }
        
                /** slice vertical slices the rectangle to floats ratios of 1
                 * 
                 */
                public Rectangle SliceVertical(float wStart, float wEnd)
                {
                    return new Rectangle((int )(l + w * wStart), (int)( l + w * wEnd), b, t);
                }
                
                public Rectangle SliceHorizontal(float hStart, float hEnd)
                {
                    return new Rectangle(l,r,(int )(t + h * hStart), (int)( t + h * hEnd));
                }

                /** Takes a rectangle, acts like this is unit rectangle and yields the resultingly sliced rectangle
                 */
                public Rectangle SubRectangle(Rectangle other)
                {
                    return new Rectangle( l + w * other.l, l + w * other.r, t + h * other.b,
                        t + h * other.t);
                }

                public List<Rectangle> Split(int rows, int cols, float wPad=0f, float hPad=0f)
                {
                    var a = new List<Rectangle>();
                    for(float i=0;i<cols;i++)
                    {
                        for(float j=0;j<rows;j++)
                        {
                            a.Add(new Rectangle(l + w / cols * i, l + w / cols * (i + 1), t + h / rows * j,
                                t + h / rows * (j + 1)).SubRectangle(new Rectangle(wPad,1-wPad,hPad,1-hPad))
                            
                            );
                        }
                    }
                    return a;
                }

                public Rectangle FittingSquare(float n = 1f)
                {
                    return new Rectangle(ShortSide()*n, ShortSide()*n, new Point(x, y));
                }

                public float ShortSide()
                {
                    return Math.Min(w, h);
                }
                
                public float LongSide()
                {
                    return Math.Max(w, h);
                }


                public float x;
                public float y;
                public float w;
                public float h;
                public float l;
                public float r;
                public float t;
                public float b;
            }
        
            public class Point
            {
                public Point(float x, float y)
                {
                    this.x = x;
                    this.y = y;
                }
                public float x;
                public float y;
            }
    }
}