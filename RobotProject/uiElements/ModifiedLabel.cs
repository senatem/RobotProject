using System;

namespace RobotProject.uiElements
{
    public class ModifiedLabel: System.Windows.Forms.Label, UiElement
    {
        public ModifiedLabel(string id, string text = "hey")
        {
            Name = id;
            Text = text;
            Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (162)));
            Location = new System.Drawing.Point(30, 20);
            Size = new System.Drawing.Size(100, 30);
            TabIndex = 0;
            Click += new System.EventHandler(this.ClickFunction);
        }

        public void Reorient(int? x=null, int? y = null, int? w=null, int? h=null)
        {
            Location = new System.Drawing.Point(x??Location.X, y??Location.Y);
            Size = new System.Drawing.Size(w??Size.Width, h??Size.Height);
        }

        public Action clickAction = () => { }; // can be used to change click function with an Action

        public void ClickFunction(object sender, EventArgs e)
        {
            clickAction();
        }
    }
}