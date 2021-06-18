using System.Windows.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RobotProject.uiElements
{
    public class ModifiedRadioButton: RadioButton
    {
        public ModifiedRadioButton(string id, string text = "modbutton")
        {
            Name = id;
            Text = text;
            Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            Location = new Point(100, 50);
            Size = new Size(200, 200);
            TabIndex = 0;
            Click += ClickFunction;
            TextAlign = ContentAlignment.MiddleLeft;
            Anchor = AnchorStyles.None;
            //BackgroundImage = System.Drawing.Image.FromFile("E:\\supreme_command\\cs_projects\\RobotProject\\RobotProject\\Images\\placeholder.jpg");
            //BackgroundImageLayout = ImageLayout.Stretch;

        }

        public Action ClickAction = () => { }; // can be used to change click function with an Action
        
        public void Reorient(int? x=null, int? y = null, int? w=null, int? h=null)
        {
            Location = new Point(x??Location.X, y??Location.Y);
            Size = new Size(w??Size.Width, h??Size.Height);
        }
        
        public void Reorient(Geometry.Rectangle r)
        {
            Reorient((int)r.L, (int)r.T, (int)r.W, (int)r.H);
        }

        private void ClickFunction(object sender, EventArgs e)
        {
            ClickAction();
        }
    }
}