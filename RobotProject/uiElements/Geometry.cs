using System;

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
                    this.x = p.x;
                    this.y = p.y;
                    this.l = this.x - w / 2;
                    this.r = this.x + w / 2;
                    this.t = this.y - h / 2;
                    this.b = this.y + h / 2;
                }
        
                public Rectangle(float w1, float w2, float h1, float h2)
                {
                    this.b = Math.Max(h1, h2);
                    this.t = Math.Min(h1, h2);
                    this.l = Math.Min(w1, w2);
                    this.r = Math.Max(w1, w2);
                    this.x = (l + r) / 2;
                    this.y = (t + b) / 2;
                    this.w = r - l;
                    this.h = b - t;
                }
        
                /** slice vertical slices the rectangle to floats ratios of 1
                 * 
                 */
                public Rectangle sliceVertical(float wStart, float wEnd)
                {
                    return new Rectangle((int )(l + w * wStart), (int)( l + w * wEnd), b, t);
                }
                
                public Rectangle sliceHorizontal(float hStart, float hEnd)
                {
                    return new Rectangle(l,r,(int )(t + h * hStart), (int)( t + h * hEnd));
                }

                /** Takes a rectangle, acts like this is unit rectangle and yields the resultingly sliced rectangle
                 */
                public Rectangle SubRectangle(Geometry.Rectangle other)
                {
                    return new Rectangle( l + w * other.l, l + w * other.r, t + h * other.b,
                        t + h * other.t);
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