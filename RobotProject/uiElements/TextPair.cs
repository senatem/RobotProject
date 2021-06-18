using System.Windows.Forms;
using System;

namespace RobotProject.uiElements
{
    public class TextPair
    {
        public TextPair(string id, string text, Geometry.Rectangle r)
        {
            _label = new ModifiedLabel(id + "_label", text);
            _label.Reorient(r.SliceVertical(0f,0.5f));
            _textBox = new ModifiedTextBox(id+"_tb","0");
            _textBox.Reorient(r.SliceVertical(0.5f,1f));
            _textBox.KeyPressed = KeyPressed;
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

        public int SelectionStart
        {
            get => _textBox.SelectionStart;
            set => _textBox.SelectionStart = value;
        }

        private readonly ModifiedLabel _label;
        private readonly ModifiedTextBox _textBox;
        
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