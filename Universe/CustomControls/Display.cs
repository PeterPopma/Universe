using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Universe.Forms;
using System.Threading;
using System.Diagnostics;
using Universe;
using Universe.Space;

namespace CustomControls
{
    public class Display : WinFormsGraphicsDevice.GraphicsDeviceControl
    {
        ContentManager contentManager;

        FormMain parentForm;

        const int ICON_SIZE = 70;
        const int ZOOM_IN_LEFT = 10;
        const int ZOOM_IN_TOP = 70;
        const int ZOOM_OUT_LEFT = ZOOM_IN_LEFT + ICON_SIZE;
        const int ZOOM_OUT_TOP = 70;
        const int HOME_LEFT = 10;
        const int HOME_TOP = 150;

        const int NAVIGATOR_LEFT = 170;
        const int NAVIGATOR_TOP = 50;
        const int NAVIGATOR_SIZE = 200;
        const int MOVE_SPEED_PIXELS = 10;

        Texture2D textureMilkyway;
        Texture2D textureArrowWidth;
        Texture2D textureHome;
        Texture2D textureNavigator;
        Texture2D textureZoomIn;
        Texture2D textureZoomOut;
        Texture2D[] textureDot = new Texture2D[12];
        SpriteBatch spriteBatch;
        SpriteFont fontNormal, fontNormal2, fontNormal3, fontNormal4, fontNormal5, fontNormal6, fontNormal7;
        SpriteFont fontSmall;
        SpaceCalculation SpaceCalculation = new SpaceCalculation();
        bool isMouseDown;
        int mouseX;
        int mouseY;

        public FormMain ParentForm { get => parentForm; set => parentForm = value; }
        public bool IsMouseDown { get => isMouseDown; set => isMouseDown = value; }
        public int MouseX { get => mouseX; set => mouseX = value; }
        public int MouseY { get => mouseY; set => mouseY = value; }

        protected override void Initialize()
        {
            ParentForm = (this.Parent as FormMain);
            contentManager = new ResourceContentManager(Services, Resources.ResourceManager);
            // To add new textures, use the Monogame pipeline tool to compile to .xnb and import them in Resourses.resx
            textureMilkyway = contentManager.Load<Texture2D>("milkyway");
            textureArrowWidth = contentManager.Load<Texture2D>("arrow_width");
            textureHome = contentManager.Load<Texture2D>("home");
            textureNavigator = contentManager.Load<Texture2D>("navigator");
            textureZoomIn = contentManager.Load<Texture2D>("zoom_in");
            textureZoomOut = contentManager.Load<Texture2D>("zoom_out");
            fontNormal = contentManager.Load<SpriteFont>("font_segoeuimono");
            fontNormal2 = contentManager.Load<SpriteFont>("font_miramonte");
            fontNormal3 = contentManager.Load<SpriteFont>("font_lindsey");
            fontNormal4 = contentManager.Load<SpriteFont>("font_kootenay");
            fontNormal5 = contentManager.Load<SpriteFont>("font_pescadero");
            fontNormal6 = contentManager.Load<SpriteFont>("font_pericles");
            fontNormal7 = contentManager.Load<SpriteFont>("font_segoeuimono");
            fontSmall = contentManager.Load<SpriteFont>("font_small");
            textureDot[0] = new Texture2D(GraphicsDevice, 1, 1);
            Color myColor = new Color(255, 255, 255, 255);
            Color[] az = Enumerable.Range(0, 1).Select(i => myColor).ToArray();
            textureDot[0].SetData(az);
            for (int k = 3; k < 13; k++)
            {
                textureDot[k - 2] = contentManager.Load<Texture2D>("smalldot" + k);
            }

            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpaceCalculation.Initialize(ParentForm.DisplayMonogame.Width, ParentForm.DisplayMonogame.Height);
        }

        public void UpdateScreen()
        {
            Invalidate();
        }

        override protected void Draw()
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            try
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                SpaceCalculation.DrawStars(spriteBatch, textureDot);

                // Draw icons
                spriteBatch.Draw(textureArrowWidth, new Rectangle(0, 2, ParentForm.Width - 20, textureArrowWidth.Height), Color.White);
                spriteBatch.Draw(textureHome, new Rectangle(HOME_LEFT, HOME_TOP, ICON_SIZE, ICON_SIZE), Color.White);
                spriteBatch.Draw(textureNavigator, new Rectangle(NAVIGATOR_LEFT, NAVIGATOR_TOP, NAVIGATOR_SIZE, NAVIGATOR_SIZE), Color.White);
                spriteBatch.Draw(textureZoomIn, new Rectangle(ZOOM_IN_LEFT, ZOOM_IN_TOP, ICON_SIZE, ICON_SIZE), Color.White);
                spriteBatch.Draw(textureZoomOut, new Rectangle(ZOOM_OUT_LEFT, ZOOM_OUT_TOP, ICON_SIZE, ICON_SIZE), Color.White);

                // print text
                double stars_in_screen = SpaceCalculation.STARS_PER_CUBIC_LIGHTYEAR * Math.PI * 4 / 3 * Math.Pow(SpaceCalculation.ScreenWidth / 2 * SpaceCalculation.CurrentScale * SpaceCalculation.LIGHTYEARS_PER_PIXEL_SCALE_1, 3);
                string text = string.Format("{0:0.00}", stars_in_screen);
                if (stars_in_screen>100000)
                { 
                    text = string.Format("{0:#,0}", stars_in_screen);
                }
                spriteBatch.DrawString(fontNormal2, "Avg. stars in screen area:  " + text, new Vector2(10, 280), Color.Yellow);
                spriteBatch.DrawString(fontNormal2, "X-Distance from sun (ly):  " + string.Format("{0:0.0}", SpaceCalculation.OffsetX), new Vector2(10, 310), Color.Yellow);
                spriteBatch.DrawString(fontNormal2, "Y-Distance from sun (ly):  " + string.Format("{0:0.0}", SpaceCalculation.OffsetY), new Vector2(10, 340), Color.Yellow);
                spriteBatch.DrawString(fontNormal2, "Layer:  " + SpaceCalculation.CurrentTopLayer, new Vector2(10, 370), Color.Yellow);
                spriteBatch.DrawString(fontNormal2, "Scale:  1:" + string.Format("{0:0}", SpaceCalculation.CurrentScale), new Vector2(10, 400), Color.Yellow);

                spriteBatch.DrawString(fontSmall, string.Format("{0:0.0}", SpaceCalculation.CurrentScale * SpaceCalculation.ScreenWidth * SpaceCalculation.LIGHTYEARS_PER_PIXEL_SCALE_1) + "  Lightyears", new Vector2(SpaceCalculation.ScreenWidth / 2 - 100, 40), Color.White);
                spriteBatch.DrawString(fontSmall, "Sun", new Vector2(SpaceCalculation.XLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer), SpaceCalculation.YLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer)), Color.White);
                if (SpaceCalculation.CurrentTopLayer < SpaceCalculation.VISIBLE_LAYERS)
                {
                    spriteBatch.DrawString(fontSmall, "Alpha centauri", new Vector2(SpaceCalculation.XLightyearsToScreen(4.2, SpaceCalculation.CurrentTopLayer), SpaceCalculation.YLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer)), Color.White);
                }
                if (SpaceCalculation.CurrentTopLayer >= 110 && SpaceCalculation.CurrentTopLayer < 142)
                {
                    spriteBatch.DrawString(fontSmall, "Center of Milkyway", new Vector2(SpaceCalculation.XLightyearsToScreen(25000, SpaceCalculation.CurrentTopLayer), SpaceCalculation.YLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer)), Color.White);
                }
                if (SpaceCalculation.CurrentTopLayer >= 160 && SpaceCalculation.CurrentTopLayer<210)
                {
                    spriteBatch.DrawString(fontSmall, "Andromeda Galaxy", new Vector2(SpaceCalculation.XLightyearsToScreen(2537000, SpaceCalculation.CurrentTopLayer), SpaceCalculation.YLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer)), Color.White);
                }
                if (SpaceCalculation.CurrentTopLayer >= 210 && SpaceCalculation.CurrentTopLayer < 260)
                {
                    spriteBatch.DrawString(fontSmall, "Virgo cluster center", new Vector2(SpaceCalculation.XLightyearsToScreen(5000000, SpaceCalculation.CurrentTopLayer), SpaceCalculation.YLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer)), Color.White);
                }
                if (SpaceCalculation.CurrentTopLayer >= 260 && SpaceCalculation.CurrentTopLayer < 280)
                {
                    spriteBatch.DrawString(fontSmall, "Virgo supercluster center", new Vector2(SpaceCalculation.XLightyearsToScreen(50000000, SpaceCalculation.CurrentTopLayer), SpaceCalculation.YLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer)), Color.White);
                }
                if (SpaceCalculation.CurrentTopLayer >= 280 && SpaceCalculation.CurrentTopLayer < 300)
                {
                    spriteBatch.DrawString(fontSmall, "Laniakea supercluster center", new Vector2(SpaceCalculation.XLightyearsToScreen(125000000, SpaceCalculation.CurrentTopLayer), SpaceCalculation.YLightyearsToScreen(0, SpaceCalculation.CurrentTopLayer)), Color.White);
                }

                spriteBatch.DrawString(fontSmall, "NumStars: " + SpaceCalculation.NumStars, new Vector2(10, 760), Color.White);

                spriteBatch.End();

            }
            catch (System.NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Equals("Begin cannot be called again until End had been succesfully called."))
                {
                    spriteBatch.End();
                }
            }
        }

        public void UpdateFrame()
        {
            if (isMouseDown)
            {
                CheckButtons(MouseX, MouseY);
            }
        }
        public void OnMouseDown(int X, int Y)
        {
            isMouseDown = true;
            MouseX = X;
            MouseY = Y;
        }
        public void CheckButtons(int X, int Y)
        {

            // zoom in	  
            if (X >= ZOOM_IN_LEFT && X < ZOOM_IN_LEFT + ICON_SIZE && Y >= ZOOM_IN_TOP && Y < ZOOM_IN_TOP + ICON_SIZE)
            {
                Thread.Sleep(80);
                SpaceCalculation.ZoomIn();
            }

            // zoom out	  
            if (X >= ZOOM_OUT_LEFT && X < ZOOM_OUT_LEFT + ICON_SIZE && Y >= ZOOM_OUT_TOP && Y < ZOOM_OUT_TOP + ICON_SIZE)
            {
                Thread.Sleep(80);
                SpaceCalculation.ZoomOut();
            }

            // go home
            if (X >= HOME_LEFT && X < HOME_LEFT + ICON_SIZE && Y >= HOME_TOP && Y < HOME_TOP + ICON_SIZE)
            {
                SpaceCalculation.SetPosition(0, 0);
            }

            // move left	  
            if (X >= NAVIGATOR_LEFT && X < NAVIGATOR_LEFT + NAVIGATOR_SIZE / 3 && Y >= NAVIGATOR_TOP && Y < NAVIGATOR_TOP + NAVIGATOR_SIZE)
            {
                SpaceCalculation.ChangePosition(-MOVE_SPEED_PIXELS, 0);
            }

            // move right	  
            if (X >= NAVIGATOR_LEFT + NAVIGATOR_SIZE * 2 / 3 && X < NAVIGATOR_LEFT + NAVIGATOR_SIZE && Y >= NAVIGATOR_TOP && Y < NAVIGATOR_TOP + NAVIGATOR_SIZE)
            {
                SpaceCalculation.ChangePosition(MOVE_SPEED_PIXELS, 0);
            }

            // move up	  
            if (X >= NAVIGATOR_LEFT && X < NAVIGATOR_LEFT + NAVIGATOR_SIZE && Y >= NAVIGATOR_TOP && Y < NAVIGATOR_TOP + NAVIGATOR_SIZE / 2)
            {
                SpaceCalculation.ChangePosition(0, -MOVE_SPEED_PIXELS);
            }

            // move down	  
            if (X >= NAVIGATOR_LEFT && X < NAVIGATOR_LEFT + NAVIGATOR_SIZE && Y >= NAVIGATOR_TOP + NAVIGATOR_SIZE * 2 / 3 && Y < NAVIGATOR_TOP + NAVIGATOR_SIZE)
            {
                SpaceCalculation.ChangePosition(0, MOVE_SPEED_PIXELS);
            }

            parentForm.Invalidate();
        }
 
    }
}
