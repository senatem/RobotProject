using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RobotProject.uiElements;

namespace RobotProject.Form2Items
{
    
    /** Stuff for displaying boxes on belt and pallets, a bit messy unfortunately,
     * functions for outside access are given first and are explained when relevant
     */
    public class BoxVisuals
    {
        private static int beltRow = 2;
        private static int beltCol = 12;
        private static int robotRow = 4;
        private static int robotCol = 4;
        
        /** A box is added to the belt (or not belt depending on parameters)
         */
        public bool AddToBoxes(SingleBox s)
        {
            boxes.Add(s);
            refresh();
            return true;
        }

        /** Robot operated on the next item
         */
        public bool RobotOperation(int n)
        {
            var b = boxes.FindIndex(e => (e.RobotNo == n)&&(e.Belt));
            if (b == -1)
            {
                return false;
            }
            else
            {
                boxes[b] =  boxes[b].UnBelt();
                refresh();
                return true;
            }
        }

        /** Clears the given pallete
         */
        public bool EmptyPallete(int n)
        {
            boxes.RemoveAll(e => (e.RobotNo == n) && (e.Belt == false));
            refresh();
            return true;
        }

        public bool RelevantPallete(int n, string text)
        {

            return (n < palleteIndicators.Count);
        }
        public BoxVisuals(int x, int y, int w, int h, bool asVisual = false)
        {
            this.plotIndicator = plotIndicator;
            this.beltIndicator = beltIndicator;
            this.asVisual = asVisual;
            realSize = new Geometry.Rectangle(w, h, new Geometry.Point( (float)x,(float)y ));
            if (asVisual)
            {
                this.plotIndicator = new Indicator("plc", References.ProjectPath + "Images\\placeholder.jpg");
                plotIndicator.PaintIndicator(Color.Chartreuse);
                Geometry.Rectangle r3 = realSize.SliceVertical(0f, 1f);
                this.plotIndicator.Reorient(r3 );
            }
            else
            {
                // examples
                /*
                var sb = new SingleBox("111", "elma", "15", "152", "154",true,0);
                boxes.Add(sb);
                var sb2 = new SingleBox("111", "elma", "25", "252", "254",true,1);
                boxes.Add(sb2);
                var sb3 = new SingleBox("111", "elma3", "125", "152", "154",false,0);
                boxes.Add(sb3);
                var sb4 = new SingleBox("111", "elma1", "225", "252", "254",false,1);
                boxes.Add(sb4);
                */
                
                
                beltIndicator = new Indicator("beltIndicator");
                beltIndicator.BackColor = Color.Black;
                beltIndicator.Reorient(realSize.SubRectangle(beltRect));
                for (int i = 0; i < 3; i++)
                {
                    palleteIndicators[i].BackColor = Color.Black;
                    palleteIndicators[i].Reorient(realSize.SubRectangle(palleteRects[i]));
                    robotIndicators[i].TextAlign = ContentAlignment.MiddleCenter;
                    robotIndicators[i].Reorient(realSize.SubRectangle(palleteRects[i]).SubRectangle(new Geometry.Rectangle(0.3f,0.7f,-0.15f,-0.05f)));
                    var j = i;
                    robotIndicators[i].ClickAction = () =>
                    {
                        
                        this.RobotOperation(j);
                    };
                }

                
                for (int j = 0; j < beltCol; j++)
                {
                    for (int i = 0; i < beltRow; i++)
                    {
                        var ml = new ModifiedLabel("aa", String.Format("{0}, {0}",i,j),8f);
                        ml.ForeColor = Color.Black;
                        ml.BackColor = Color.Goldenrod;
                        ml.TextAlign = ContentAlignment.TopCenter;
                        var otb = new Geometry.Rectangle((float) j / beltCol,
                            (float) (j + 1) / beltCol, (float) i / beltRow, (float)
                            (i + 1) / beltRow);
                        var tb = realSize.SubRectangle(beltRect.SubRectangle(otb)).SubRectangle(new Geometry.Rectangle(0.05f,0.95f,0.05f,0.95f));
                        ml.Reorient(tb);
                        beltBoxLabels.Add(ml);
                    }
                }

                for (var i = 0; i < robotRow; i++)
                {
                    for (var j = 0; j < robotCol; j++)
                    {
                        var index = 0;
                        foreach (var e in palleteRects)
                        {
                            var ml = new ModifiedLabel("aa", $"{i}, {j}", 8f)
                            {
                                ForeColor = Color.Black,
                                BackColor = Color.Goldenrod,
                                TextAlign = ContentAlignment.TopCenter
                            };
                            var otb = new Geometry.Rectangle((float) j / robotCol,
                                (float) (j + 1) / robotCol, (float) i / robotRow, (float)
                                (i + 1) / robotRow);
                            var tb = realSize.SubRectangle(e.SubRectangle(otb)).SubRectangle(new Geometry.Rectangle(0.05f,0.95f,0.05f,0.95f));
                            ml.Reorient(tb);
                            robotLabels[index].Add(ml);
                            index++;
                        }
                    }
                }



            }
            refresh();
        }

        /** Refreshes the information displayed on the visuals
         */
        public void refresh()
        {
            var index = 0;

            var beltBoxes = boxes.FindAll(e => e.Belt == true);
            foreach (var item in beltBoxLabels)
            {
                if (index < beltBoxes.Count)
                {
                    item.Text = beltBoxes[index].FullText;
                    item.Visible = true;
                }
                else
                {
                    item.Visible = false;
                }
                index++;
            }
            
            for(int i=0;i<3;i++)
            {
                var palleteBoxes = boxes.FindAll(e => (e.Belt == false) && (e.RobotNo == i));
                index = 0;
                foreach (var item in robotLabels[i])
                {
                    
                    if (index < palleteBoxes.Count)
                    {
                        item.Text = palleteBoxes[index].FullText;
                        item.Visible = true;
                    }
                    else
                    {
                        item.Visible = false;
                    }
                    index++;
                }
            }


        }

        public void Implement(Control.ControlCollection motherControlCollection)
        {
            if (asVisual)
            {
                motherControlCollection.Add(plotIndicator);
            }
            else
            {                
                foreach (var bb in beltBoxLabels)
                {
                    motherControlCollection.Add(bb);
                }
                foreach (var b in robotLabels)
                {
                    foreach (var bb in b)
                    {
                        motherControlCollection.Add(bb);                        
                    }
                }
                motherControlCollection.Add(beltIndicator);
                foreach (var pi in palleteIndicators)
                {
                    motherControlCollection.Add(pi);
                }
                foreach (var pi in robotIndicators)
                {
                    motherControlCollection.Add(pi);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                var e = new ModifiedButton(String.Format("c{0}",i), String.Format("{0}. robotu temizle ",i+1));
                var j = i;
                e.ClickAction = () =>
                {
                    this.EmptyPallete(j);
                };
                e.Reorient(realSize.SubRectangle(palleteRects[i]).SubRectangle(new Geometry.Rectangle(0f,1f,1.05f,1.15f)));
                motherControlCollection.Add(e);
                
                palleteTypes[i].Reorient(realSize.SubRectangle(palleteRects[i]).SubRectangle(new Geometry.Rectangle(0f,1f,1.18f,1.25f)));
                palleteTypes[i].TextAlign = ContentAlignment.TopCenter;
                motherControlCollection.Add(palleteTypes[i]);
            }
            
            

            //var exit = new ModifiedButton("exit", "exit");
            
        }

        

        private Indicator plotIndicator;
        private List<ModifiedLabel> beltBoxLabels = new List<ModifiedLabel>();
        private List<List<ModifiedLabel>> robotLabels = new List<List<ModifiedLabel>>()
        {
            new List<ModifiedLabel>(),new List<ModifiedLabel>(),new List<ModifiedLabel>()   
        };
        private List<SingleBox> boxes = new List<SingleBox>();
        private Indicator beltIndicator;
        private List<Indicator> palleteIndicators = new List<Indicator>() // the bg
        {
            new Indicator("r1"), new Indicator("r2"), new Indicator("r3")
        };
        private List<ModifiedLabel> robotIndicators = new List<ModifiedLabel>() // the bg
        {
            new ModifiedLabel("r1","Robot 1"), new ModifiedLabel("r2","Robot 2"), new ModifiedLabel("r3","Robot 3")
        };
        private List<ModifiedLabel> palleteTypes = new List<ModifiedLabel>() // the bg
        {
            new ModifiedLabel("p1","1. palet",12), new ModifiedLabel("r2","2. palet",12), new ModifiedLabel("r3","3. palet",12)
        };
        private bool asVisual = false;
        private Geometry.Rectangle realSize;
        private Geometry.Rectangle beltRect = new Geometry.Rectangle(0f, 1f, 0f, 0.2f);
        private List<Geometry.Rectangle> palleteRects = new List<Geometry.Rectangle>()
        {
            new Geometry.Rectangle(0.01f, 0.33f, 0.3f, 0.8f),
            new Geometry.Rectangle(0.34f, 0.66f, 0.3f, 0.8f),
            new Geometry.Rectangle(0.67f, 0.99f, 0.3f, 0.8f)
        };
    }
}