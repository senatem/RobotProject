using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items.palletteStuff
{
    public class PalleteVisuals
    {
        const int palleteNumber = 3;
        public PalleteVisuals(int x, int y, int w, int h, bool asVisual = false)
        {
            _asVisual = asVisual;
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point(x, y));

            if (asVisual)
            {
                _plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                _plotIndicator.PaintIndicator(Color.Fuchsia);
                Geometry.Rectangle r3 = r.SliceVertical(0f, 1f);
                _plotIndicator.Reorient(r3);
            }
            else
            {
                _background.Reorient(r);
                // indicators
                var a = r.Split(1, 3, 0.05f, 0.05f);

                a = new List<Geometry.Rectangle> {
                    new Geometry.Rectangle(177, 405, 388, 658),
                    new Geometry.Rectangle(455, 682, 388, 658),
                    new Geometry.Rectangle(732, 959, 388, 658)
                    
                };

                for (int i = 0; i < 3; i++)
                {
                    //var centreRect = a[i].SubRectangle(new Geometry.Rectangle(0f, 1f, 0.2f, 0.6f)); // centre
                    var pv = new PalletteVisual(a[i],i);
                    
                    _pallettes.Add(pv);
                }

            }    
        }
        
        public void resizeToWindowRect(Geometry.Rectangle boxRect)
        {
            // (appWidthInit/2, (appHeightInit-100)/2+100, appWidthInit, appHeightInit-100,false);
            var x = boxRect.L + boxRect.W/2;
            var y = boxRect.T + (boxRect.H - 100f/720f*boxRect.H)/2f + boxRect.H/720f*100f;
            var w = boxRect.W;
            var h = boxRect.H - 100f/720f*boxRect.H;
            resize((int)x,(int)y,(int)w,(int)h);
            //SystemControls(3*appWidthInit/8, 50, 3*appWidthInit/4, 100,false);
        }

        public void resize(int x, int y, int w, int h)
        {
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( x,y ));
            var a = r.Split(1, 4,0.05f,0.05f);
            _background.Reorient(r);
            a = new List<Geometry.Rectangle> {
                new Geometry.Rectangle(r.L + r.W/1280*177, r.L +r.W/1280*405, r.T + r.H/620f*288f, r.T + r.H/620f*558f),
                new Geometry.Rectangle(r.L +r.W/1280*455, r.L +r.W/1280*682, r.T + r.H/620f*288f, r.T + r.H/620f*558f),
                new Geometry.Rectangle(r.L +r.W/1280*732, r.L +r.W/1280*959, r.T + r.H/620f*288f, r.T + r.H/620f*558f)
                    
            };
            
            _pallettes[0].resize( a[0]);
            _pallettes[1].resize(a[1]);
            _pallettes[2].resize(a[2]);
            
        }
        

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            
            for (int i=0; i < 3; i++)
            {
                _pallettes[i].Implement(motherControlCollection);
            }
            motherControlCollection.Add(this._background);
        }
        
        public void EmptyPallette(int n)
        {
            _pallettes[n].EmptyPallette();
        }

        public void resetKat(int n)
        {
            _pallettes[n].setCounts(0);
        }

        public void setPallette(int n, string no, string en, string boy, int cap)
        {
            _pallettes[n].setInfo(no,en,boy,cap);
        }

        public void setProdCount(int n, int? valueDefn=null, int? valueFill = null)
        {
            _pallettes[n].setCounts(valueDefn, valueFill);
        }

        public void increaseProdCount(int n, int increment = 1)
        {
            _pallettes[n - 1].incementCount(0, increment);
        }

        private Geometry.Rectangle makeMyRect(Geometry.Rectangle boxRect)
        {
            return makeMyRect(boxRect.W, boxRect.H, boxRect.L, boxRect.T);
        }
        
        private Geometry.Rectangle makeMyRect(float width, float height, float leftMargin = 0f, float topMargin = 0f)
        {
            var x = leftMargin + width/2;
            var y = topMargin + (height - 100f/720f*height)/2 + height/720f*100f;
            var w = width;
            var h = height - 100f/720f*height;
            resize((int)x,(int)y,(int)w,(int)h);
            return new Geometry.Rectangle(width, height, new Geometry.Point(x, y));
        }

        public void increaseFillCount(int n, int increment = 1)
        {
            _pallettes[n - 1].incementCount(increment, 0);
        }



        private List<PalletteVisual> _pallettes = new List<PalletteVisual>();
        private readonly Indicator _plotIndicator = null!; // indicator of the empty plot for design purposes
        private readonly bool _asVisual;
        private ModifiedLabel _background = new Indicator("bg",References.ProjectPath + "Images\\r3.png");
    }
}