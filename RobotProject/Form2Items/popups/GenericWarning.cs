using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    /** This is a generic warning that can be used for various purposes, usage is really simple, hence the point
     * two inputs are optional, but modifying the first one is severely reccomended
     * to modify, please inherit instead of modifying this
     */
    public class GenericWarning : Form
    {
        public GenericWarning(string warningText = "ben bir uyarıyım", string buttonText = "onay")
        {
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size((int) W, (int) H);
            StartPosition = FormStartPosition.CenterParent;
            Text = @"Uyarı";
            
            Geometry.Rectangle v = new Geometry.Rectangle(W * 0.1f, W * 0.9f, 0f, H);

            var a = v.Split(2, 1);
            
            ModifiedLabel textLabel = new ModifiedLabel("warning text",warningText,12f);
            textLabel.Reorient(a[0]);
            textLabel.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(textLabel);
            
            ModifiedButton confirm = new ModifiedButton("confirm", buttonText);
            confirm.Reorient(a[1].SubRectangle(new Geometry.Rectangle(0.25f,0.75f,0.2f,0.7f)));
            confirm.ClickAction = Close;
            Controls.Add(confirm);
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private const float W = 300f;
        private const float H = 200f;
    }
}