using System;
using System.Drawing;
using System.Transactions;
using System.Windows.Forms;

namespace RobotProject.uiElements
{
    public class ModifiedLabel: System.Windows.Forms.Label, IUiElement
    {
        public ModifiedLabel(string id, string text = "hey",float emSize = 15.75f)
        {
            Name = id;
            Text = text;
            Font = new System.Drawing.Font("Microsoft Sans Serif", emSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 162);
            Location = new System.Drawing.Point(30, 20);
            Size = new System.Drawing.Size(100, 30);
            TabIndex = 0;
            Click += ClickFunction;
            
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public void Reorient(int? x=null, int? y = null, int? w=null, int? h=null)
        {
            Location = new System.Drawing.Point(x??Location.X, y??Location.Y);
            Size = new System.Drawing.Size(w??Size.Width, h??Size.Height);
        }
        
        public void Reorient(Geometry.Rectangle r)
        {
            Reorient((int)r.L, (int)r.T, (int)r.W, (int)r.H);
        }

        public Action ClickAction = () => { }; // can be used to change click function with an Action

        private void ClickFunction(object sender, EventArgs e)
        {
            ClickAction();
        }
        
        

    }
}