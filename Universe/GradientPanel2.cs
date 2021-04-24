using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Chess.CustomControls
{
    class GradientPanel2 : Panel
    {
        public GradientPanel2()
        {
            this.ResizeRedraw = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            rect.Height = rect.Height / 2 + 1;
            LinearGradientBrush brush = new LinearGradientBrush(rect,
                                                                    Color.FromArgb(184, 210, 206),
                                                                    Color.FromArgb(125, 139, 137),
                                                                    90F);
            e.Graphics.FillRectangle(brush, rect);
            rect.Height = rect.Height - 2;
            rect.Y = rect.Height + 2;
            brush = new LinearGradientBrush(rect,
                                                                                Color.FromArgb(125, 139, 137),
                                                                                Color.FromArgb(184, 210, 206),
                                                                                90F);
            e.Graphics.FillRectangle(brush, rect);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();
            base.OnScroll(se);
        }
    }
}
