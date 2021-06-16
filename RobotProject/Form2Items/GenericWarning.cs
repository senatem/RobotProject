using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RobotProject.uiElements;


namespace RobotProject
{
    /** This is a generic warning that can be used for various purposes, usage is really simple, hence the point
     * two inputs are optional, but modifying the first one is severely reccomended
     * to modify, please inherit instead of modifying this
     */
    public class GenericWarning : Form
    {
        private IContainer components = null;

        public GenericWarning(string warningText = "ben bir uyarıyım", string buttonText = "onay")
        {
            
            
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size((int) w, (int) h);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Uyarı";
            
            Geometry.Rectangle v = new Geometry.Rectangle(w * 0.1f, w * 0.9f, 0f, h);

            var a = v.Split(2, 1);
            
            textLabel = new ModifiedLabel("warning text",warningText,12f);
            textLabel.Reorient(a[0]);
            textLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(textLabel);
            
            confirm = new ModifiedButton("confirm", buttonText);
            confirm.Reorient(a[1].SubRectangle(new Geometry.Rectangle(0.25f,0.75f,0.2f,0.7f)));
            confirm.clickAction = () =>
            {
                this.Close();
            };
            this.Controls.Add(confirm);
        }

        private float w = 300f;
        private float h = 200f;
        private ModifiedLabel textLabel;
        private ModifiedButton confirm;

    }
}