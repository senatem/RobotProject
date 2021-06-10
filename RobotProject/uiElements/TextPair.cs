using System.Windows.Forms;

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
        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(_label);
            motherControlCollection.Add(_textBox);
        }
        
        private readonly ModifiedLabel _label;
        private readonly ModifiedTextBox _textBox;
        public string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value;
        }
        
    }
}