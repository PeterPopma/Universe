using CustomControls;

namespace Universe.Forms
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.displayMonogame = new CustomControls.Display();
            this.SuspendLayout();
            // 
            // displayMonogame
            // 
            this.displayMonogame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayMonogame.Location = new System.Drawing.Point(0, 0);
            this.displayMonogame.Name = "displayMonogame";
            this.displayMonogame.ParentForm = null;
            this.displayMonogame.Size = new System.Drawing.Size(1048, 1048);
            this.displayMonogame.TabIndex = 0;
            this.displayMonogame.Text = "displayMonogame";
            this.displayMonogame.MouseDown += new System.Windows.Forms.MouseEventHandler(this.displayMonogame_MouseDown);
            this.displayMonogame.MouseUp += new System.Windows.Forms.MouseEventHandler(this.displayMonogame_MouseUp);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1048, 1048);
            this.Controls.Add(this.displayMonogame);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "Universe v1.0 (c) P.Popma 2018";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.SizeChanged += new System.EventHandler(this.OnTimedEventUpdateScreen);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControls.Display displayMonogame;

        public Display DisplayMonogame { get => displayMonogame; set => displayMonogame = value; }
    }
}