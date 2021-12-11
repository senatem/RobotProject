using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    public class ServoControls
    {
        public ServoControls(Geometry.Rectangle boxRect, bool asVisual = false)
        {
            
            _asVisual = asVisual;
            var r = makeMyRect(boxRect);
            if (asVisual)
            {
                _plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                _plotIndicator.PaintIndicator(Color.Fuchsia);
                Geometry.Rectangle r3 = r.SliceVertical(0f, 1f);
                _plotIndicator.Reorient(r3);
            }
            else
            {
                var rows = r.SliceHorizontal(0.0f,0.8f).Split(6, 1);
                for (var i = 0; i < 6; i++)
                {
                    _textPairs.Add( new TextPair($"R{i / 2 + 1}S{i % 2 + 1}", $"Robot{i / 2 + 1} Servo{i % 2 + 1} (mm):", rows[i],10f,split:0.85f,textAlign: ContentAlignment.TopLeft));
                }
                _apply.Reorient(r.SliceHorizontal(0.8f,1f));                
            }

        }
        
        

        private ModifiedLabel TextLabelGenerator(String id, String text, Geometry.Rectangle rectangle)
        {
            var staticText = new ModifiedLabel(id,emSize:12f);
            //staticText.BackColor = textColour;
            staticText.Text = text;
            staticText.Reorient(rectangle);
            staticText.TextAlign = ContentAlignment.TopLeft;
            return staticText;
        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            foreach (var textPair in _textPairs)
            {
                textPair.Implement(motherControlCollection);
                textPair.recolour(Color.White);

                textPair.KeyPressed = () =>
                {

                    if (textPair.Text == "")
                    {
                        textPair.Text = "0";
                        textPair.CursorToEnd();
                        textPair.entryValid = true;
                    } 
                    if (int.TryParse(textPair.Text, out _))
                    {
                        textPair.recolour(Color.White);
                        var n = int.Parse(textPair.Text);
                        textPair.Text = $"{n}";
                        textPair.CursorToEnd();
                        textPair.entryValid = true;
                    }
                    else
                    {
                        textPair.recolour(Color.Red);
                        textPair.entryValid = false;
                    }
                    


                };


            }
            motherControlCollection.Add(_apply);
        }
        

        
        
        

        /** Sena bu sana, yazılı değerleri liste olarak getiriyor, int değilse de null getiriyor
         */
        public List<int?> getValues()
        {
            var l = new List<int?>();
            foreach (var textPair in _textPairs)
            {
                if (textPair.entryValid)
                {
                    l.Add(int.Parse(textPair.Text));
                }
                else
                {
                    l.Add(null);                    
                }
            }
            return l;
        }

        public void resizeToWindowRect(Geometry.Rectangle boxRect)
        {
            var myRect = makeMyRect(boxRect);
            resize((int)myRect.Centre().X,(int)myRect.Centre().Y,(int)myRect.W,(int)myRect.H);
        }

        public void resize(int x, int y, int w, int h)
        {
            Geometry.Rectangle r = new Geometry.Rectangle(w, h, new Geometry.Point( x,y ));
            
            var rows = r.SliceHorizontal(0.0f,0.8f).Split(6, 1, 0f, 0f);

            for (var i = 0; i < 6; i++)
            {
                _textPairs[i].reorient( rows[i]);
            }
            _apply.Reorient(r.SliceHorizontal(0.8f,1f));
        }
        
        
        private Geometry.Rectangle makeMyRect(Geometry.Rectangle boxRect)
        {
            return makeMyRect(boxRect.W, boxRect.H, boxRect.L, boxRect.T);
        }
        
        private Geometry.Rectangle makeMyRect(float width, float height, float leftMargin = 0f, float topMargin = 0f)
        {
            //1040, 1255
            //var left = leftMargin + width / 1280f * 1039f;
            //var w = width / 1280f * 216f;
            var left = leftMargin + width / 1280f * 1058f;
            var w = width / 1280f * 198f;
            return new Geometry.Rectangle(left, left+w,
                topMargin + (height * 0.43f), topMargin + (height * 0.73f));
            //return new Geometry.Rectangle(w, h, new Geometry.Point(x, y));
        }



        private List<TextPair> _textPairs = new List<TextPair>();
        private readonly Indicator _plotIndicator = null!; // indicator of the empty plot for design purposes
        private readonly bool _asVisual;
        private ModifiedButton _apply = new ModifiedButton("apply", "Uygula",11f);
        private Action _applyPressed = () => { }; 
        public Action applyPressed  // can be used to change click function with an Action
        {
            get => _applyPressed;
            set
            {
                _applyPressed = value;
                _apply.ClickAction = value;
            }
        }
    }
}