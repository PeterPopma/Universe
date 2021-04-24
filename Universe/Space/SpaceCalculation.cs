using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Space
{
    class SpaceCalculation
    {
        // star data is stored for scales: [CurrentTopLayer-VISIBLE_SCALES] until [CurrentTopLayer]
        public const int VISIBLE_LAYERS = 30;
        const int BUFFER_LAYERS = 0;

        const double SCALE_MULTIPLICATION_FACTOR = 1.08;
        public const double LIGHTYEARS_PER_PIXEL_SCALE_1 = 0.01;

        const long DIAMETER_OBSERVABLE_UNIVERSE = 93000000000L;
        const long RADIUS_OBSERVABLE_UNIVERSE = DIAMETER_OBSERVABLE_UNIVERSE / 2L;
        const double NUM_STARS_IN_UNIVERSE = 1000000000000000000000000d;
        public const double STARS_PER_CUBIC_LIGHTYEAR = NUM_STARS_IN_UNIVERSE / (4 / 3 * Math.PI * RADIUS_OBSERVABLE_UNIVERSE * RADIUS_OBSERVABLE_UNIVERSE * RADIUS_OBSERVABLE_UNIVERSE);

        // it is hard to calculate this value, because we are looking at only 1 layer of this area of the universe
        const int STARS_PER_LAYER = 800;

        private int screenWidth;
        private int screenHeight;

        double ScreenCenterX;
        double ScreenCenterY;
        double ScreenWidthLightyearsScale1;
        double ScreenHeightLightyearsScale1;
        double ScreenCubicLightyearsScale1;
        int maxLayers;
        int numStars;
 
        // CurrentTopLayer represents a of stars at a certain scale
        // at any time, we are only watching the stars from current layer and the 10(=VISIBLE_LAYERS) layers below it.
        private int currentTopLayer;
        // the real distance of 1 pixel on the screen. [1 pixel] = [scale*LIGHTYEARS_PER_PIXEL_SCALE_1] lightyears
        private double currentScale;
        // offset from our sun in real distance, + is to the right
        // this is layer-independent, for example [OffsetX]=10 means the current centerX is 10 lightyears to the right from the sun
        private double offsetX = 0;
        private double offsetY = 0;
        //var starsList = [];
        // we store star information in regions, indexed by layer and offset
        private List<Region>[] regions;
        Random random = new Random();

        public double OffsetX { get => offsetX; set => offsetX = value; }
        public double OffsetY { get => offsetY; set => offsetY = value; }
        public int ScreenWidth { get => screenWidth; set => screenWidth = value; }
        public int ScreenHeight { get => screenHeight; set => screenHeight = value; }
        public int CurrentTopLayer { get => currentTopLayer; set => currentTopLayer = value; }
        public double CurrentScale { get => currentScale; set => currentScale = value; }
        public int MaxLayers { get => maxLayers; set => maxLayers = value; }
        public int NumStars { get => numStars; set => numStars = value; }
        internal List<Region>[] Regions { get => regions; set => regions = value; }

        public void Initialize(int width, int height)
        {
            CurrentTopLayer = 1;
            ScreenHeight = height;
            ScreenWidth = width;
            ScreenCenterX = ScreenWidth / 2;
            ScreenCenterY = ScreenHeight / 2;
            ScreenWidthLightyearsScale1 = ScreenWidth * LIGHTYEARS_PER_PIXEL_SCALE_1;
            ScreenHeightLightyearsScale1 = ScreenHeight * LIGHTYEARS_PER_PIXEL_SCALE_1;
            // 10^24 stars in observable universe. radius universe 46.5 billion. #stars / (4/3 * PI * 46500000000^3) = 0.00000000079146420354094182443310004573847
            ScreenCubicLightyearsScale1 = ScreenHeight /* (this is considered as "depth") */  * ScreenWidth * ScreenHeight * LIGHTYEARS_PER_PIXEL_SCALE_1 * LIGHTYEARS_PER_PIXEL_SCALE_1 * LIGHTYEARS_PER_PIXEL_SCALE_1;
            // Math.pow(SCALE_MULTIPLICATION_FACTOR, layer) * ScreenWidth * LIGHTYEARS_PER_PIXEL_SCALE_1 = 93.000.000.000 ly (diameter observable universe)
            // ->  Math.pow(SCALE_MULTIPLICATION_FACTOR, layer) =  93000000000 / (ScreenWidth * LIGHTYEARS_PER_PIXEL_SCALE_1)
            // ->  layer =  ln(93000000000 / (ScreenWidth * LIGHTYEARS_PER_PIXEL_SCALE_1))/ln(SCALE_MULTIPLICATION_FACTOR)
            MaxLayers = (int)(1 + Math.Floor(Math.Log(DIAMETER_OBSERVABLE_UNIVERSE / (ScreenWidth * LIGHTYEARS_PER_PIXEL_SCALE_1)) / Math.Log(SCALE_MULTIPLICATION_FACTOR)));
            Regions = new List<Region>[MaxLayers+1];
            CurrentScale = Math.Pow(SCALE_MULTIPLICATION_FACTOR, CurrentTopLayer - 1);

            for (int layer = 1; layer <= 1 + BUFFER_LAYERS; layer++)
            {
                CreateNewLayer(layer);
            }
        }

        public void ZoomOut()
        {
            if (CurrentTopLayer < MaxLayers)
            {
                CurrentTopLayer++;
                Console.Write("zoom out. currenttoplayer: " + CurrentTopLayer);
                CurrentScale = LayerToScale(CurrentTopLayer);

                var new_layer = CurrentTopLayer + BUFFER_LAYERS;
                if (new_layer < MaxLayers + BUFFER_LAYERS)
                {
                    var invalid_layer = CurrentTopLayer - VISIBLE_LAYERS - BUFFER_LAYERS;
                    if (invalid_layer > 0)
                    {
                        RemoveInvalidLayer(invalid_layer);
                    }
                    CreateNewLayer(new_layer);
                }
            }
        }

        public void ZoomIn()
        {
            if (CurrentTopLayer > 1)
            {
                CurrentTopLayer--;
                Console.Write("zoom in. currenttoplayer: " + CurrentTopLayer);
                CurrentScale = LayerToScale(CurrentTopLayer);

                var new_layer = CurrentTopLayer - VISIBLE_LAYERS - BUFFER_LAYERS;
                if (new_layer > 0)
                {
                    var invalid_layer = CurrentTopLayer + BUFFER_LAYERS + 1;
                    if (invalid_layer > 0)
                    {
                        RemoveInvalidLayer(invalid_layer);
                    }
                    CreateNewLayer(new_layer);
                }
            }
        }

        // converts x (in lightyears from sun) to a point on the screen
        public int XLightyearsToScreen(double x, int layer)
        {
            return (int)(ScreenCenterX + (x - OffsetX) / (LIGHTYEARS_PER_PIXEL_SCALE_1 * LayerToScale(layer)));
        }

        // converts y (in lightyears from sun) to a point on the screen
        public int YLightyearsToScreen(double y, int layer)
        {
            return (int)(ScreenCenterY + (y - OffsetY) / (LIGHTYEARS_PER_PIXEL_SCALE_1 * LayerToScale(layer)));
        }

        // converts x (in pixels on screen) to lightyears from sun
        private double XScreenToLightyears(double x, int layer)
        {
            return OffsetX + (x - ScreenCenterX) * LIGHTYEARS_PER_PIXEL_SCALE_1 * LayerToScale(layer);
        }

        // converts y (in pixels on screen) to lightyears from sun
        private double YScreenToLightyears(double y, int layer)
        {
            return OffsetY + (y - ScreenCenterY) * LIGHTYEARS_PER_PIXEL_SCALE_1 * LayerToScale(layer);
        }

        // converts x (in pixels on screen) to lightyears from sun
        private double XScreenToLightyears(double x, int layer, double offsetX)
        {
            return offsetX + (x - ScreenCenterX) * LIGHTYEARS_PER_PIXEL_SCALE_1 * LayerToScale(layer);
        }

        // converts y (in pixels on screen) to lightyears from sun
        private double YScreenToLightyears(double y, int layer, double offsetY)
        {
            return offsetY + (y - ScreenCenterY) * LIGHTYEARS_PER_PIXEL_SCALE_1 * LayerToScale(layer);
        }

        private double ScaleUsedToCreateLayer(int layer)
        {
            return LayerToScale(layer + (VISIBLE_LAYERS - 1) + BUFFER_LAYERS);
        }

        private double LayerToScale(int layer)
        {
            // The last layer should be exactly the scale what we want as maximum
            if (layer == MaxLayers)
            {
                return DIAMETER_OBSERVABLE_UNIVERSE / (ScreenWidth * LIGHTYEARS_PER_PIXEL_SCALE_1);
            }
            return Math.Pow(SCALE_MULTIPLICATION_FACTOR, layer - 1);
        }
        public void ChangePosition(double XPixels, double YPixels)
        {
            OffsetX = OffsetX + CurrentScale * XPixels * LIGHTYEARS_PER_PIXEL_SCALE_1;
            OffsetY = OffsetY + CurrentScale * YPixels * LIGHTYEARS_PER_PIXEL_SCALE_1;

            UpdateRegions();
        }


        public void SetPosition(double newOffsetX, double newOffsetY)
        {
            OffsetX = newOffsetX;
            OffsetY = newOffsetY;

            UpdateRegions();
        }

        private void UpdateRegions()
        {
            for (int current_layer = CurrentTopLayer; current_layer > CurrentTopLayer - VISIBLE_LAYERS; current_layer--)
            {
                if (current_layer > 0)
                {
                    RemoveInvalidRegions(current_layer);
                    CreateNewRegions(current_layer);
                }
            }
        }

        private void RemoveInvalidRegions(int layer)
        {
            double valid_area_left, valid_area_top, valid_area_right, valid_area_bottom;
            var remove_count = 0;
            double used_scale = ScaleUsedToCreateLayer(layer);
            valid_area_left = (OffsetX - OffsetX % (ScreenWidthLightyearsScale1 * used_scale)) - ScreenWidthLightyearsScale1 * used_scale;
            valid_area_top = (OffsetY - OffsetY % (ScreenHeightLightyearsScale1 * used_scale)) - ScreenHeightLightyearsScale1 * used_scale;
            valid_area_right = valid_area_left + 2 * ScreenWidthLightyearsScale1 * used_scale;
            valid_area_bottom = valid_area_top + 2 * ScreenHeightLightyearsScale1 * used_scale;

            if (layer == 13)
            {
                Console.WriteLine("valid_area_left="+valid_area_left);
            }
            for (var index = Regions[layer].Count - 1; index >= 0; index--)
            {
                if (Regions[layer][index].OffsetX < valid_area_left || Regions[layer][index].OffsetY < valid_area_top || Regions[layer][index].OffsetX > valid_area_right || Regions[layer][index].OffsetY > valid_area_bottom)
                {
                    Console.Write("removed region. OffsetX:" + Regions[layer][index].OffsetX + ", OffsetY:" + Regions[layer][index].OffsetY + ", Layer:" + layer);
                    Regions[layer].RemoveAt(index);
                    remove_count++;
                }
            }
        }

        private void CreateNewRegions(int layer)
        {
            double used_scale = ScaleUsedToCreateLayer(layer);
            var create_count = 0;
            var used_offset_x = (OffsetX - OffsetX % (ScreenWidthLightyearsScale1 * used_scale)) - ScreenWidthLightyearsScale1 * used_scale;
            // note: we're using 2.8 times instead of 3 times to prevent rounding errors causing the creation of an extra region
            if (layer == 13)
            {
                Console.WriteLine("used_offset_x=" + used_offset_x);
            }
            for (double x = used_offset_x; x < used_offset_x + 2.8 * ScreenWidthLightyearsScale1 * used_scale; x += ScreenWidthLightyearsScale1 * used_scale)
            {
                var used_offset_y = (OffsetY - OffsetY % (ScreenHeightLightyearsScale1 * used_scale)) - ScreenHeightLightyearsScale1 * used_scale;
                // note: we're using 2.8 times instead of 3 times to prevent rounding errors causing the creation of an extra region
                for (double y = used_offset_y; y < used_offset_y + 2.8 * ScreenHeightLightyearsScale1 * used_scale; y += ScreenHeightLightyearsScale1 * used_scale)
                {
                    if (!Regions[layer].Exists(o => o.OffsetX==x && o.OffsetY == y))
                    {
                        AddRegion(x, y, layer, used_scale);
                        create_count++;
                    }
                }
            }
            if (create_count > 0)
            {
                //    console.log("created " + create_count + " regions on layer: " + layer);
            }

        }

        // At layer 1, no star must be closer than 6ly from 0,0 (because there is only Alpha Centauri and the sun)
        private Star CreateLayer1Star(double x_real_minimum, double y_real_minimum, double x_real_maximum, double y_real_maximum)
        {
            double distance = 0;
            Star new_star = new Star();
            while (distance < 6)
            {
                new_star.X = random.NextDouble() * (x_real_maximum - x_real_minimum) + x_real_minimum;
                new_star.Y = random.NextDouble() * (y_real_maximum - y_real_minimum) + y_real_minimum;
                distance = Math.Sqrt(Math.Pow(new_star.X, 2) + Math.Pow(new_star.Y, 2));
            }
            return new_star;
        }

        private void GenerateStars(List<Star> starsList, double x_real_minimum, double y_real_minimum, double x_real_maximum, double y_real_maximum, int layer, int num_stars)
        {
            //  console.log("generating " + num_stars + " stars on layer: " + layer);
            if (layer < 0)
            {
                GenerateStarsRandom(starsList, x_real_minimum, y_real_minimum, x_real_maximum, y_real_maximum, layer, num_stars);
            }
            else if(layer<0)
            {
                GenerateStarsMilkyWay(starsList, x_real_minimum, y_real_minimum, x_real_maximum, y_real_maximum, layer, num_stars);
            }
            else if (layer > 0 && layer < 220)
            {
                GenerateGalaxies(starsList, x_real_minimum, y_real_minimum, x_real_maximum, y_real_maximum, layer, num_stars);
            }
            else if (layer < 0)
            {
                // generate cosmic web
            }
            else
            {
                // generate random
            }
        }

        private void GenerateStarsRandom(List<Star> starsList, double x_real_minimum, double y_real_minimum, double x_real_maximum, double y_real_maximum, int layer, int num_stars)
        {
            for (int i = 0; i < num_stars; i++)
            {
                var new_star = new Star();
                if (layer == 1)
                {
                    new_star = CreateLayer1Star(x_real_minimum, y_real_minimum, x_real_maximum, y_real_maximum);
                }
                else
                {
                    new_star.X = random.NextDouble() * (x_real_maximum - x_real_minimum) + x_real_minimum;
                    new_star.Y = random.NextDouble() * (y_real_maximum - y_real_minimum) + y_real_minimum;
                }
                starsList.Add(new_star);
            }
        }

        private void GenerateStarsMilkyWay(List<Star> starsList, double x_real_minimum, double y_real_minimum, double x_real_maximum, double y_real_maximum, int layer, int num_stars)
        {
            List<Box> boxes = new List<Box>();
            // Divide visible space into boxes, random offset and size, 10.000.000 lightyears apart, 80.000-120.000 ly big
            double current_y = y_real_minimum - y_real_minimum % 10000000;
            if (y_real_minimum < 0)        // for negative numbers, we must subtract one grid
            {
                current_y -= 10000000;
            }
            while (current_y <= y_real_maximum)
            {
                double current_x = x_real_minimum - x_real_minimum % 10000000;
                if (x_real_minimum < 0)        // for negative numbers, we must subtract one grid
                {
                    current_x -= 10000000;
                }
                while (current_x <= x_real_maximum)
                {
                    // It could be that this box is outside the visible space, but that does not really matter..
                    double x = current_x /*+ (random.Next(4000000)-2000000)*/;
                    double y = current_y /*+ (random.Next(4000000) - 2000000)*/;
                    boxes.Add(new Box(x, y, x + 80000/* + random.Next(40000)*/, y + 80000 /*+ random.Next(40000)*/));
                    current_x += 10000000;
                }
                current_y += 10000000;
            }
            int stars_created = 0;

            // loop through all boxes to create stars there
            while (stars_created < num_stars)
            {
                foreach (Box current_box in boxes)
                {
                    var new_star = new Star();
                    new_star.X = random.NextDouble() * (current_box.Right - current_box.Left) + current_box.Left;
                    new_star.Y = random.NextDouble() * (current_box.Bottom - current_box.Top) + current_box.Top;
                    if (new_star.X >= x_real_minimum && new_star.X < x_real_maximum && new_star.Y >= y_real_minimum && new_star.Y < y_real_maximum)
                    {
                        starsList.Add(new_star);
                    }
                    // Even if the star is not really added, it is still counted, so we don't get many 
                    stars_created++;
                }
            }

        }

        private void GenerateGalaxies(List<Star> starsList, double x_real_minimum, double y_real_minimum, double x_real_maximum, double y_real_maximum, int layer, int num_stars)
        {
            const double AVERAGE_DISTANCE_GALAXIES = 10000000;
            const double RADIUS_NO_STARS_FROM_MILKYWAY = 6000000;

            List<SpacePoint> galaxies = new List<SpacePoint>();
            // Divide visible space into points, random offset and size, 10.000.000 lightyears apart
            double current_y = y_real_minimum - y_real_minimum % AVERAGE_DISTANCE_GALAXIES;
            if(y_real_minimum<0)        // for negative numbers, we must subtract one grid
            {
                current_y -= AVERAGE_DISTANCE_GALAXIES;
            }
            while (current_y <= y_real_maximum)
            {
                double current_x = x_real_minimum - x_real_minimum % AVERAGE_DISTANCE_GALAXIES;
                if (x_real_minimum < 0)        // for negative numbers, we must subtract one grid
                {
                    current_x -= AVERAGE_DISTANCE_GALAXIES;
                }
                while (current_x <= x_real_maximum)
                {
                    // It could be that this box is outside the visible space.
                    double x = current_x;// + (random.Next(4000000)-2000000);
                    double y = current_y;// + (random.Next(4000000) - 2000000);
                    if (x < -RADIUS_NO_STARS_FROM_MILKYWAY || x > RADIUS_NO_STARS_FROM_MILKYWAY || y < -RADIUS_NO_STARS_FROM_MILKYWAY || y > RADIUS_NO_STARS_FROM_MILKYWAY)     // skip milkyway area
                    {
                        galaxies.Add(new SpacePoint(x, y));
                    }
                    current_x += AVERAGE_DISTANCE_GALAXIES;
                }
                current_y += AVERAGE_DISTANCE_GALAXIES;
            }
            // add milky way
            double x_milkyway = 25000;
            if (x_real_minimum - x_milkyway > -RADIUS_NO_STARS_FROM_MILKYWAY && x_real_maximum - x_milkyway < RADIUS_NO_STARS_FROM_MILKYWAY && y_real_minimum > -RADIUS_NO_STARS_FROM_MILKYWAY && y_real_maximum < RADIUS_NO_STARS_FROM_MILKYWAY)
            {
                galaxies.Add(new SpacePoint(x_milkyway, 0));
            }
            double x_andromeda = 2537000;
            // add andromeda
            if (x_real_minimum - x_andromeda > -RADIUS_NO_STARS_FROM_MILKYWAY && x_real_maximum - x_andromeda < RADIUS_NO_STARS_FROM_MILKYWAY && y_real_minimum > -RADIUS_NO_STARS_FROM_MILKYWAY && y_real_maximum < RADIUS_NO_STARS_FROM_MILKYWAY)
            {
                galaxies.Add(new SpacePoint(x_andromeda, 0));
            }

            foreach (SpacePoint galaxy in galaxies)
            {
                int max_length = 20000 + random.Next(40000);     // 40.000-120.000 ly diameter
                for (int k=0; k<1; k++)
                {
                    var new_star = new Star();
                    double direction = random.NextDouble() * 2 * Math.PI;
                    double length = random.Next(max_length);
                    new_star.X = galaxy.X + length * Math.Cos(direction);
                    new_star.Y = galaxy.Y + length * Math.Sin(direction);
                    if (new_star.X >= x_real_minimum && new_star.X < x_real_maximum && new_star.Y >= y_real_minimum && new_star.Y < y_real_maximum)
                    {
                        starsList.Add(new_star);
                    }
                }
            }

        }

        private void AddRegion(double x, double y, int layer, double scale)
        {
            var x_real_minimum = x;
            var y_real_minimum = y;
            var x_real_maximum = x + ScreenWidthLightyearsScale1 * scale;
            var y_real_maximum = y + ScreenHeightLightyearsScale1 * scale;
            Region new_region = new Region(x, y, layer, new List<Star>());
            if (Regions[layer] == null)
            {
                Regions[layer] = new List<Region>();
            }
            Regions[layer].Add(new_region);
            if(Regions[layer].Count>9)
            {
                int error = 1;
            }

            GenerateStars(new_region.Stars, x_real_minimum, y_real_minimum, x_real_maximum, y_real_maximum, layer, STARS_PER_LAYER);
        }

        // Creates a new layer with 9 regions of stars around the offset
        // Used when zooming in- and out and at beginning
        private void CreateNewLayer(int layer)
        {
            Console.Write("adding layer: " + layer);
            var regions = 0;
            // determine the area to generate stars for
            // the scale is based on the largest scale the stars are visible in, 
            // so that we don't need to generate stars for every layer when zooming in- and out
            double used_scale = ScaleUsedToCreateLayer(layer);
            var used_offset_x = (OffsetX - OffsetX % (ScreenWidthLightyearsScale1 * used_scale)) - ScreenWidthLightyearsScale1 * used_scale;
            // note: we're using 2.8 times instead of 3 times to prevent rounding errors causing the creation of an extra region
            for (double x = used_offset_x; x < used_offset_x + 2.8 * ScreenWidthLightyearsScale1 * used_scale; x += ScreenWidthLightyearsScale1 * used_scale)
            {
                var used_offset_y = (OffsetY - OffsetY % (ScreenHeightLightyearsScale1 * used_scale)) - ScreenHeightLightyearsScale1 * used_scale;
                // note: we're using 2.8 times instead of 3 times to prevent rounding errors causing the creation of an extra region
                for (double y = used_offset_y; y < used_offset_y + 2.8 * ScreenHeightLightyearsScale1 * used_scale; y += ScreenHeightLightyearsScale1 * used_scale)
                {
                    //      console.log("addregion. x:" + x + " y:" + y + " begin x:" + used_offset_x + " end x: " + (used_offset_x+3*ScreenWidthLightyearsScale1*used_scale) + " step x:" + (ScreenWidthLightyearsScale1*used_scale));
                    AddRegion(x, y, layer, used_scale);
                    regions++;
                }
            }
            Console.Write("added: " + regions + " regions");

        }

        private void RemoveInvalidLayer(int layer)
        {
            Console.Write("removing layer: " + layer);
            var remove_count = 0;
            for (var index = Regions[layer].Count - 1; index >= 0; index--)
            {
                Regions[layer].RemoveAt(index);
                remove_count++;
            }
            if (remove_count == 0)
            {
                Console.Write("Tried to remove layer with no regions!!! ");
            }
        }

        public void DrawStars(SpriteBatch spriteBatch, Texture2D[] textureDot)
        {
            numStars = 0;

            for (int layer = CurrentTopLayer; layer>CurrentTopLayer-VISIBLE_LAYERS && layer>0; layer--)
            {
                foreach (Region region in Regions[layer])
                {
                    numStars += region.Stars.Count;
                    foreach (Star current_star in region.Stars)
                    {
                        var screen_x = XLightyearsToScreen(current_star.X, CurrentTopLayer);
                        if (screen_x >= 0 && screen_x < ScreenWidth)
                        {
                            var screen_y = YLightyearsToScreen(current_star.Y, CurrentTopLayer);
                            if (screen_y >= 0 && screen_y < ScreenHeight)
                            {
                                int depth = 5 + CurrentTopLayer - region.Layer;   // depth 5 .. 34 with VISIBLE_LAYERS=30
                                int star_size = 4 - (depth / 10);
                                int colorvalue = 280 - depth * 5;
                                var color = new Color(colorvalue, colorvalue, colorvalue);

                                spriteBatch.Draw(textureDot[star_size], new Rectangle(screen_x, screen_y, textureDot[star_size].Width, textureDot[star_size].Height), color);
                            }
                        }
                    }
                }
            }
            // draw Sun and Alpha centauri
            if (CurrentTopLayer < VISIBLE_LAYERS)
            {
                int depth = CurrentTopLayer;
                int star_size = 4 - (depth / 10);
                var color = 255 - depth * 5;

                int screen_x = XLightyearsToScreen(0, depth);
                int screen_y = YLightyearsToScreen(0, depth);
                spriteBatch.Draw(textureDot[star_size], new Rectangle(screen_x, screen_y, textureDot[star_size].Width, textureDot[star_size].Height), Color.White);

                screen_x = XLightyearsToScreen(4.2, depth);
                screen_y = YLightyearsToScreen(0, depth);
                spriteBatch.Draw(textureDot[star_size], new Rectangle(screen_x, screen_y, textureDot[star_size].Width, textureDot[star_size].Height), Color.White);
            }
        }


    }
}
