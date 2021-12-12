using System;


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

            TextChanged += KeyPressedFunction;
            //KeyPress += new KeyPressEventHandler( KeyPressedFunction);
            //this.KeyUp += KeyPressedFunction;
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
            Reorient((int)r.L, (int)r.T, (int)r.W, (int)r.H);
        }
        
        public Action KeyPressed = () => { }; // can be used to change click function with an Action

        private void KeyPressedFunction(object sender, EventArgs e)
        {
            KeyPressed();
        }

        public void CursorToEnd()
        {
            SelectionStart = Text.Length;
            SelectionLength = 0;
        }
    }
}