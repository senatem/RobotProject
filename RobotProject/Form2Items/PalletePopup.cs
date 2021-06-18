using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    internal class PalletePopup: Form
    {
        
        public PalletePopup()
        {
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size((int) w, (int) h);
            Text = @"Ürün Ekleme";
            Geometry.Rectangle v = new Geometry.Rectangle(w * 0.1f, w * 0.9f, 0f, h);

            var helpfulText= new ModifiedLabel("warn", "Palet numarasını sayıyla girin.");
            helpfulText.Reorient(v.SliceHorizontal(1f / 8f, 3f / 8f));
            Controls.Add(helpfulText);
            
            // text can be changed here
            prodNo = new TextPair("prod no", "palet no:", v.SliceHorizontal(4f / 8f, 5f / 8f));
            
            prodNo.KeyPressed = () =>
            {
                var ss = prodNo.SelectionStart;
                if (System.Text.RegularExpressions.Regex.IsMatch(prodNo.Text, "[^0-9]"))
                {
                    // prodNo.Text = prodNo.Text.Remove(prodNo.Text.Length - 1);
                    ss = ss - 1;
                }
                else
                {
                    ProductNo = prodNo.Text;
                }

                prodNo.Text = ProductNo;
                //prodNo.CursorToEnd();
                prodNo.SelectionStart = ss;
            };
            prodNo.Implement(Controls);
            
            




            // buttons
            var buttonsRect = v.SliceHorizontal(0.75f, 0.9f);
            var conf = new ModifiedButton("onay", "onay");
            conf.Reorient(buttonsRect.SliceVertical(0.1f, 0.4f));
            conf.ClickAction = () =>
            {
                _confirmed = true;
                Close();
            };

            var exit = new ModifiedButton("çık", "çık");
            exit.Reorient(buttonsRect.SliceVertical(0.6f, 0.9f));
            exit.ClickAction = Close;

            Controls.Add(conf);
            Controls.Add(exit);
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private String ProductNo = "";

        private float w = 400f;
        private float h = 350f;

        public void Reset()
        {
            prodNo.Text = "";
            ProductNo = "";
        }

        public void Opening()
        {
            _confirmed = false;
        }

        
        private Boolean _confirmed;

        public Boolean confirmed =>
            // additional invalid conditions
            _confirmed;
        
        private readonly TextPair prodNo;

        
    }
}