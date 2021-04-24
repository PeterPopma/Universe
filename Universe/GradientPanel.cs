using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chess.CustomControls
{
    class GradientPanel : Panel
    {
        public GradientPanel()
        {
            this.ResizeRedraw = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            rect.Height = rect.Height / 2 ;
            LinearGradientBrush brush = new LinearGradientBrush(rect,
                                                                    Color.FromArgb(50, 50, 50),
                                                                    Color.FromArgb(200, 200, 200),
                                                                    90F);
            e.Graphics.FillRectangle(brush, rect);
            //rect.Height += 1;
            brush = new LinearGradientBrush(rect,
                                                                                Color.FromArgb(200, 200, 200),
                                                                                Color.FromArgb(50, 50, 50),
                                                                                90F);
            rect.Y = rect.Height;
            e.Graphics.FillRectangle(brush, rect);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();
            base.OnScroll(se);
        }
    }
}
