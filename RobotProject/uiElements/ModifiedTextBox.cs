namespace RobotProject.uiElements
{
    public class ModifiedTextBox: System.Windows.Forms.TextBox, IUiElement
    {
        
        public ModifiedTextBox(string id, string text = "hey")
        {
            Name = id;
            Text = text;
            Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 162);
            Location = new System.Drawing.Point(30, 20);
            Size = new System.Drawing.Size(100, 30);
            TabIndex = 0;
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        /** Reorients by top left point, width and height 
         * 
         */ 
        public void Reorient(int? x = null, int? y = null, int? w = null, int? h = null)
        {
            Location = new System.Drawing.Point(x??Location.X, y??Location.Y);
            Size = new System.Drawing.Size(w??Size.Width, h??Size.Height);
            
        }

        public void Reorient(Geometry.Rectangle r)
        {
            Reorient((int)r.l, (int)r.t, (int)r.w, (int)r.h);
        }
    }
}