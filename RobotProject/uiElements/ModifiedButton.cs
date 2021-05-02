using System;
using System.Windows.Forms;

namespace RobotProject.uiElements
{
    public class ModifiedButton: System.Windows.Forms.Button, UiElement
    {
        public ModifiedButton(string id, string text = "modbutton")
        {
            Name = id;
            Text = text;
            Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
            Location = new System.Drawing.Point(100, 50);
            Size = new System.Drawing.Size(200, 200);
            TabIndex = 0;
            Click += new System.EventHandler(this.ClickFunction);
            //BackgroundImage = System.Drawing.Image.FromFile("E:\\supreme_command\\cs_projects\\RobotProject\\RobotProject\\Images\\placeholder.jpg");
            //BackgroundImageLayout = ImageLayout.Stretch;

        }
        

        public Action clickAction = () => { }; // can be used to change click function with an Action
        
        public void Reorient(int? x=null, int? y = null, int? w=null, int? h=null)
        {
            Location = new System.Drawing.Point(x??Location.X, y??Location.Y);
            Size = new System.Drawing.Size(w??Size.Width, h??Size.Height);
        }
        
        public void Reorient(Geometry.Rectangle r)
        {
            Reorient((int)r.l, (int)r.t, (int)r.w, (int)r.h);
        }
        
        public void ClickFunction(object sender, EventArgs e)
        {
            clickAction();
        }
    }
}