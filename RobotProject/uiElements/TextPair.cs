using System;
using System.Windows.Forms;

namespace RobotProject.uiElements
{
    public class TextPair
    {
        public TextPair(String id, String text, Geometry.Rectangle r)
        {
            label = new ModifiedLabel(id + "_label", text);
            label.Reorient(r.sliceVertical(0f,0.5f));
            textBox = new ModifiedTextBox(id+"_tb","0");
            textBox.Reorient(r.sliceVertical(0.5f,1f));
        }

        public void implement(Control.ControlCollection motherControlCollection)
        {
            motherControlCollection.Add(label);
            motherControlCollection.Add(textBox);
        }
        
        private ModifiedLabel label;
        private ModifiedTextBox textBox = new ModifiedTextBox("att1T","0");
        public String text
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }
        
    }
}