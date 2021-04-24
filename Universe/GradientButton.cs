using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chess.CustomControls
{
    class GradientButton : Button
    {
        Boolean isMouseClick;
        Boolean isMouseOver;
        Boolean isActive;

        [Description("Test text displayed in the textbox"), Category("Behavior")]
        public Boolean Active
        {
            get { return isActive; }
            set { isActive = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = new Rectangle(ClientRectangle.X+1, ClientRectangle.Y+1, ClientRectangle.Width-2, ClientRectangle.Height-2);

            if (isMouseClick)
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(230, 230, 255)); 
                e.Graphics.FillRectangle(brush, rect);
            }
            else if(isActive)
            {
                LinearGradientBrush brush = new LinearGradientBrush(rect,
                                                                        Color.FromArgb(66, 116, 144),
                                                                        Color.FromArgb(167, 200, 255),
                                                                        45F);
                e.Graphics.FillRectangle(brush, rect);
            }
             
                else if (isMouseOver)
            {
                LinearGradientBrush brush = new LinearGradientBrush(rect,
                                                                        Color.FromArgb(120, 120, 120),
                                                                        Color.FromArgb(250, 250, 255),
                                                                        45F);
                e.Graphics.FillRectangle(brush, rect);
            }
            else 
            {
                LinearGradientBrush brush = new LinearGradientBrush(rect,
                                                                        Color.FromArgb(90, 90, 90),
                                                                        Color.FromArgb(220, 220, 230),
                                                                        45F);
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw text
            Brush foreBrush = new SolidBrush(ForeColor);
            if(isMouseClick || isMouseOver)
            {
//                foreBrush = new SolidBrush(Color.Yellow);
            }
            SizeF size = e.Graphics.MeasureString(Text, Font);
            PointF pt = new PointF((Width - size.Width) / 2 + (isMouseClick ? 1 : 0), (Height - size.Height) / 2 + (isMouseClick ? 1 : 0));
            e.Graphics.DrawString(Text, Font, foreBrush, pt);
            foreBrush.Dispose();

            if (Image != null)
            {
                if (ImageAlign.Equals(ContentAlignment.BottomCenter) || ImageAlign.Equals(ContentAlignment.MiddleCenter) || ImageAlign.Equals(ContentAlignment.TopCenter))
                {
                    e.Graphics.DrawImage(Image, (Width-Image.Width) / 2, 1, Image.Width, Image.Height);
                }
                else
                {
                    e.Graphics.DrawImage(Image, 3, 1, Image.Width, Image.Height);
                }
            }

        }


        protected override void OnMouseEnter(EventArgs e)
                {
                    isMouseOver = true;
                    Invalidate();
                    base.OnMouseEnter(e);
                }

                protected override void OnMouseLeave(EventArgs e)
                {
                    isMouseOver = false;
                    Invalidate();
                    base.OnMouseLeave(e);
                }
        
                protected override void OnMouseUp(MouseEventArgs mevent)
                {
                    isMouseClick = false;
                    Invalidate();
                    base.OnMouseUp(mevent);
                }

                protected override void OnMouseDown(MouseEventArgs mevent)
                {
                    isMouseClick = true;
                    Invalidate();
                    base.OnMouseDown(mevent);
                }

            
    }
}
