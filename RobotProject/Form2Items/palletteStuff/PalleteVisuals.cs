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
        
        public PalleteVisuals(Geometry.Rectangle boxRect, bool asVisual = false)
        {
            _asVisual = asVisual;
            Geometry.Rectangle r = makeMyRect(boxRect);

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
            var n = makeMyRect(boxRect);
            resize(n);
        }
        
        public void resize(Geometry.Rectangle r)
        {
            _background.Reorient(r);
            var a = new List<Geometry.Rectangle> {
                new Geometry.Rectangle(r.L + r.W/1280*177+1, r.L +r.W/1280*405, r.T + r.H/620f*288f, r.T + r.H/620f*558f),
                new Geometry.Rectangle(r.L +r.W/1280*455+1, r.L +r.W/1280*682, r.T + r.H/620f*288f, r.T + r.H/620f*558f),
                new Geometry.Rectangle(r.L +r.W/1280*732+1, r.L +r.W/1280*959, r.T + r.H/620f*288f, r.T + r.H/620f*558f)
                    
            };
            
            _pallettes[0].resize( a[0]);
            _pallettes[1].resize(a[1]);
            _pallettes[2].resize(a[2]);
            
        }

        public void resize(int x, int y, int w, int h)
        {
            resize(new Geometry.Rectangle(w, h, new Geometry.Point( x,y )));
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
            _pallettes[n].resetPallet();
        }

        public void setPallette(int n, string no, string en, string boy, int cap, int kat)
        {
            _pallettes[n].setInfo(no,en,boy,cap, kat);
        }

        public void setProdCount(int n, int? valueDefn=null, int? valueFill = null, int? pDef = null, int? pFill = null)
        {
            _pallettes[n].setCounts(valueDefn, valueFill, pDef, pFill);
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
            var left = leftMargin + 100f / 1280 * width;
            var w = 1180f / 1280f * width;
            
            
            return new Geometry.Rectangle(left, left+w, topMargin+100f/ 720f * height, topMargin+720f / 720f * height);
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