using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.Types;
using RobotProject.Form2Items.palletteStuff;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    public class ErrorBox
    {
        //8 distinct errors
        public ErrorBox(Geometry.Rectangle boxRect, bool asVisual = false)
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
                _errorHeader = new Indicator("header",text: "Hatalar");
                _errorHeader.Reorient(new Geometry.Rectangle(r.L, r.L + r.W, r.T, r.T + 30f));
                _errorHeader.BackColor = Color.LightGray;
                _errorHeader.TextAlign = ContentAlignment.TopCenter;
                
                
                _errorList =  new Indicator("header",emSize: 9f);
                _errorList.Reorient(new Geometry.Rectangle(r.L, r.L + r.W, r.T+30f, r.T + r.H-80f));
                _errorList.BackColor = Color.LightGray;
                
                _fixButton  = new ModifiedButton("fix", "Hataları Düzelt",12f);
                _fixButton.Reorient(new Geometry.Rectangle(r.L, r.L + r.W, r.T + r.H-80f, r.T + r.H));



            }

            

        }

        

        public void setErrorText(String s)
        {
            _errorList.Text = s;
            if (s != "")
            {
                _errorHeader.BackColor = Color.Red;
            }
            else
            {
                _errorHeader.BackColor = Color.LightGray;
            }
        }
        
        /** Input as a list of strings
         * 
         */
        public void setErrorText(List<String> lines)
        {
            var s = "";
            foreach (var line in lines)
            {
                s += "\n- " + line+"\n";
            }
            setErrorText(s);
        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(_errorHeader);
            motherControlCollection.Add(_errorList);
            motherControlCollection.Add(_fixButton);
        }
        
        public void resize(Geometry.Rectangle r)
        {
            _errorHeader.Reorient(new Geometry.Rectangle(r.L, r.L + r.W, r.T, r.T + 30f));
            _errorList.Reorient(new Geometry.Rectangle(r.L, r.L + r.W, r.T+30f, r.T + r.H-80f));
            _fixButton.Reorient(new Geometry.Rectangle(r.L+10f, r.L + r.W-20f, r.T + r.H-80f+10f, r.T + r.H-14f));
        }
        
        public void resize(int x, int y, int w, int h)
        {
            resize(new Geometry.Rectangle(w, h, new Geometry.Point( x,y )));
            
            
        }
        
        
        private Geometry.Rectangle makeMyRect(Geometry.Rectangle boxRect)
        {
            return makeMyRect(boxRect.W, boxRect.H, boxRect.L, boxRect.T);
        }
        
        private Geometry.Rectangle makeMyRect(float width, float height, float leftMargin = 0f, float topMargin = 0f)
        {
            //1040, 1255
            var left = leftMargin + 10f / 1280f * width;
            var w = 220f / 1280f * width;
            var top = topMargin + 290f / 720f * height;
            var h = 370f / 720f * height;
            return new Geometry.Rectangle(left,left+w,top,top+h);
        }
        
        private readonly Indicator _plotIndicator;
        private readonly Indicator _errorHeader;
        private readonly Indicator _errorList;
        public ModifiedButton _fixButton;
        private readonly bool _asVisual;

        public void resizeToWindowRect(Geometry.Rectangle boxRect)
        {
            var n = makeMyRect(boxRect);
            resize(n);
        }
    }
}