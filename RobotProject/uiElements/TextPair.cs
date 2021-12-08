using System.Windows.Forms;
using System;
using System.Drawing;


namespace RobotProject.uiElements
{
    public class TextPair
    {
        public TextPair(string id, string text, Geometry.Rectangle r, float textEmSize=15.75f, float split = 0.5f, ContentAlignment  textAlign=  ContentAlignment.TopLeft)
        {
            _label = new ModifiedLabel(id + "_label", text,textEmSize);
            _label.Reorient(r.SliceVertical(0f,split));
            _label.TextAlign = textAlign;
            _textBox = new ModifiedTextBox(id+"_tb","0",textEmSize);
            _textBox.Reorient(r.SliceVertical(split,1f));
            _textBox.KeyPressed = KeyPressed;
            this._split = split;


        }

        public void reorient(Geometry.Rectangle r)
        {
            _label.Reorient(r.SliceVertical(0f,_split));
            _textBox.Reorient(r.SliceVertical(_split,1f));
        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(_label);
            motherControlCollection.Add(_textBox);
        }

        public void CursorToEnd()
        {
            _textBox.CursorToEnd();
        }

        public void recolour(Color backColour)
        {
            _label.BackColor = backColour;
        }

        public int SelectionStart
        {
            get => _textBox.SelectionStart;
            set => _textBox.SelectionStart = value;
        }

        public bool entryValid = true;

        private readonly ModifiedLabel _label;
        private readonly ModifiedTextBox _textBox;
        private float _split = 0.5f;
        
        private Action _keyPressed = () => { }; 
        public Action KeyPressed  // can be used to change click function with an Action
        {
            get => _keyPressed;
            set
            {
                _keyPressed = value;
                _textBox.KeyPressed = value;
            }
        }

        

        public string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value;
        }
        
    }
}