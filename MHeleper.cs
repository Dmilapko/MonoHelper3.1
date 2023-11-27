using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MonoHelper
{
    public static class MHeleper
    {
        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the binary file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the binary file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the binary file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static T CreateDeepCopy<T>(this T objectToCopy)
        {
            using (var ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, objectToCopy);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }

        public static T CreateDeepCopyXML<T>(this T objectToCopy)
        {
            using (var ms = new MemoryStream())
            {
                var xml = new XmlSerializer(objectToCopy.GetType());
                xml.Serialize(ms, objectToCopy);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)xml.Deserialize(ms);
            }
        }

        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }

        public static int GetSign(this int number)
        {
            if (number == 0) return 0;
            if (number < 0) return -1;
            else return 1;
        }
        public static int GetSign(this double number)
        {
            if (number == 0) return 0;
            if (number < 0) return -1;
            else return 1;
        }
        public static int GetSign(this float number)
        {
            if (number == 0) return 0;
            if (number < 0) return -1;
            else return 1;
        }

        public static string PutZeros(string StrAfterZeros, int SizeOfOutputStr)
        {
            while (StrAfterZeros.Length < SizeOfOutputStr) StrAfterZeros = "0" + StrAfterZeros;
            return StrAfterZeros;
        }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static double RandomDouble()
        {
            lock (syncLock)
            { // synchronize
                return random.NextDouble();
            }
        }

        public static int RandomInt(int left, int right)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(left, right);
            }
        }

        /// <summary>
        /// Return random index depending on probability of each
        /// </summary>
        /// <param name="probabilities"></param>
        /// <returns></returns>
        public static int RandomElement(List<double> probabilities)
        {
            lock (syncLock)
            { // synchronize
                for (int i = 1; i < probabilities.Count; i++)
                {
                    probabilities[i] += probabilities[i - 1];
                }
                double r = random.NextDouble() * probabilities.Last();
                for (int i = 0; i < probabilities.Count; i++)
                {
                    if (r < probabilities[i]) return i;
                }
                return -1;
            }
        }

        public static int RandomRound(double d)
        {
            int init_d = (int)d;
            if (RandomDouble() < (d - init_d)) return init_d + 1;
            else return init_d;
        }


        public static Texture2D Clone(this Texture2D texture, GraphicsDevice graphics)
        {
            Texture2D res = new Texture2D(graphics, texture.Width, texture.Height);
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            res.SetData(cd);
            return res;
        }

        /// <summary>
        /// Creates filled circle with smooth borders
        /// </summary>
        public static Texture2D CreateCircle(GraphicsDevice graphicsDevice, int radius, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius * 2, radius * 2);
            Color[] colorData = new Color[radius * radius * 4];

            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
                    int index = x * radius * 2 + y;
                    double ltc = Math.Sqrt(Math.Abs(x - radius + 0.5f) * Math.Abs(x - radius + 0.5f) + Math.Abs(y - radius + 0.5f) * Math.Abs(y - radius + 0.5f)) + 0.5f;
                    if (ltc <= radius)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        if (ltc <= radius + 1)
                        {
                            colorData[index] = new Color(color, radius + 1 - (float)ltc);
                            //colorData[index] = MixTwoColors(new Color(color, radius + 1 - (float)ltc), colorData[index]);
                        }
                        else colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        /// <summary>
        /// Creates filled circle with smooth borders
        /// </summary>
        public static Color[] CreateCircle(this Color[] colorData, int radius, Color color)
        {
            int initsize = (int)Math.Sqrt(colorData.Length) / 2;
            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
                    int index = (x + (initsize - radius)) * initsize * 2 + (y + (initsize - radius));
                    double ltc = Math.Sqrt(Math.Abs(x - radius + 0.5f) * Math.Abs(x - radius + 0.5f) + Math.Abs(y - radius + 0.5f) * Math.Abs(y - radius + 0.5f)) + 0.5f;
                    if (ltc <= radius)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        if (ltc <= radius + 1)
                        {
                            if (colorData[index] == Color.Transparent) colorData[index] = new Color(color, radius + 1 - (float)ltc);
                            else colorData[index] = MixTwoColors(new Color(color, radius + 1 - (float)ltc), colorData[index]);
                        }
                    }
                }
            }

            return colorData;
        }

        public static Texture2D DrawCircle(this Texture2D texture, Vector2 center, double radius, double width, Color color, double border_size = 1d)
        {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    double dist = Math.Abs(Math.Sqrt(Math.Pow(center.X - x, 2) + Math.Pow(center.Y - y, 2)) - radius);
                    if (dist <= width)
                    {
                        float A = (float)Math.Min(border_size, width - dist);
                        if (A < 1)
                        {
                            if (A > 0) colorData[x + y * texture.Width] = MixTwoColorsNA(colorData[x + y * texture.Width], new Color(color, A));
                        }
                        else colorData[x + y * texture.Width] = new Color(color, 1f);
                    }
                }
            }
            texture.SetData(colorData);
            return texture;
        }
        /// <summary>
        /// Creates circle with outline
        /// </summary>
        public static Texture2D CreateCircle(GraphicsDevice graphicsDevice, int radius, Color color, int outlinewidth, Color outlinecolor)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius * 2, radius * 2);
            Color[] colorData = new Color[radius * radius * 4];

            colorData.CreateCircle(radius, outlinecolor);
            colorData.CreateCircle(radius - outlinewidth, color);

            texture.SetData(colorData);
            return texture;
        }

        /// <summary>
        /// Converts TIME in MS to string min:sec:ms
        /// </summary>
        public static string GetTimeMinSecMs(this int time)
        {
            int min = 0, sec = 0, ms = 0;
            min = time / 60000;
            time = time % 60000;
            sec = time / 1000;
            ms = time % 1000;
            if (min != 0) return min.ToString() + ":" + PutZeros(sec.ToString(), 2) + "." + PutZeros(ms.ToString(), 3);
            else return sec.ToString() + "." + PutZeros(ms.ToString(), 3);
        }

        /// <summary>
        /// Returns height in pixels of FONT
        /// </summary>
        public static int GetFontHeight(SpriteFont font)
        {
            return (int)Math.Round(font.MeasureString("Gg").Y * 0.8);
        }

        /// <summary>
        /// Returns width in pixels of TEXT with FONT;
        /// </summary>
        /// <param name="font"></param>
        /// <param name="_fontinitsize">initial size of FONT</param>
        /// <param name="_fontsize">scaled size of FONT</param>
        /// <returns></returns>
        public static int GetFontHeight(SpriteFont font, int _fontinitsize, int _fontsize)
        {
            return (int)Math.Round(font.MeasureString("Wg").Y / (float)_fontinitsize * (float)_fontsize);
        }

        /// <summary>
        /// Returns width in pixels of TEXT with FONT;
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="_fontinitsize">initial size of FONT</param>
        /// <param name="_fontsize">scaled size of FONT</param>
        /// <returns></returns>
        public static int GetTextWidth(string text, SpriteFont font, int _fontinitsize, int _fontsize)
        {
            return (int)Math.Round(font.MeasureString(text).X / (float)_fontinitsize * (float)_fontsize);
        }

        /// <summary>
        /// Changes COLOR to Color(0, 0, 0, 0)
        /// </summary>
        public static Texture2D MakeTransparent(this Texture2D texture, Color color)
        {
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            for (int i = 0; i < cd.Length; i++)
                if (cd[i] == color)
                    cd[i] = new Color(0, 0, 0, 0);
            texture.SetData(cd);
            return texture;
        }

        /// <summary>
        /// Changes COLOR to TRANSPARENTCOLOR
        /// </summary>
        public static Texture2D MakeTransparent(this Texture2D texture, Color color, Color transparentcolor)
        {
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            for (int i = 0; i < cd.Length; i++)
                if (cd[i] == color)
                    cd[i] = transparentcolor;
            texture.SetData(cd);
            return texture;
        }

        /// <summary>
        /// Returns brightness [0; MAXBRIGHTNESS]
        /// </summary>
        public static float Brightness(this Color color, float maxbrightness)
        {
            return (color.R + color.G + color.B) / 255f / 3f * maxbrightness;
        }

        /// <summary>
        /// Flips texture
        /// </summary>
        public static Texture2D SaveAsFlippedTexture2D(this Texture2D input, bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(input.GraphicsDevice, input.Width, input.Height);
            Color[] data = new Color[input.Width * input.Height];
            Color[] flipped_data = new Color[data.Length];

            input.GetData<Color>(data);

            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    int index = 0;
                    if (horizontal && vertical)
                        index = input.Width - 1 - x + (input.Height - 1 - y) * input.Width;
                    else if (horizontal && !vertical)
                        index = input.Width - 1 - x + y * input.Width;
                    else if (!horizontal && vertical)
                        index = x + (input.Height - 1 - y) * input.Width;
                    else if (!horizontal && !vertical)
                        index = x + y * input.Width;

                    flipped_data[x + y * input.Width] = data[index];
                }
            }

            flipped.SetData<Color>(flipped_data);

            return flipped;
        }

        /// <summary>
        /// Make borders smooth
        /// Requires MakeTransparent!!!
        /// </summary>
        public static Texture2D SmoothBorders(this Texture2D texture, Color BackgroundColor)
        {
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            texture.SmoothRight(cd, 1, BackgroundColor);
            texture.SmoothRight(cd, -1, BackgroundColor);
            texture.SmoothDown(cd, 1, BackgroundColor);
            texture.SmoothDown(cd, -1, BackgroundColor);
            return texture;
        }

        public static Color MixTwoColors(Color color, Color blend)
        {
            if (blend.A != 0) return new Color(
                 (byte)MathHelper.Clamp((color.R * color.A + blend.R * blend.A) / 2f, 0, 255),
                 (byte)MathHelper.Clamp((color.G * color.A + blend.G * blend.A) / 2f, 0, 255),
                 (byte)MathHelper.Clamp((color.B * color.A + blend.B * blend.A) / 2f, 0, 255),
                 (byte)MathHelper.Clamp((color.A + blend.A) / 2, 0, 255));
            else return color;
        }

        /*    public static Color MixTwoColorsA(Color color, Color blend)
            {
                return new Color(
                     (color.R + blend.R) / 2f,
                     (color.G + blend.G) / 2f,
                     (color.B + blend.B) / 2f,
                     (color.A + blend.A) / 2f);
            }*/

        public static Color MixTwoColorsNA(Color color, Color blend)
        {
            return new Color(
                 (byte)MathHelper.Clamp((color.R * color.A + blend.R * blend.A) / (blend.A + color.A), 0, 255),
                 (byte)MathHelper.Clamp((color.G * color.A + blend.G * blend.A) / (blend.A + color.A), 0, 255),
                 (byte)MathHelper.Clamp((color.B * color.A + blend.B * blend.A) / (blend.A + color.A), 0, 255));
        }

        public static Texture2D SmoothDown(this Texture2D texture, Color[] cd, int stepdir, Color TransColor)
        {
            Color[] cdnow = new Color[texture.Width * texture.Height];
            texture.GetData(cdnow);
            for (int y = 1; y < texture.Height - 1 + stepdir; y++)
            {
                for (int x = 1; x < texture.Width; x++)
                {
                    if (cd[y * texture.Width + x] == TransColor)
                    {
                        if (cd[(y - stepdir) * texture.Width + x] != TransColor)
                        {
                            int bytop = 0, bybottom = 0;
                            if (x != 0)
                            {
                                int ny = y;
                                while ((cd[ny * texture.Width + x - 1] != TransColor) && (cd[ny * texture.Width + x] == TransColor))
                                {
                                    ny += stepdir;
                                    bytop++;
                                }
                            }
                            if (x != texture.Width)
                            {
                                int ny = y;
                                while ((cd[ny * texture.Width + x + 1] != TransColor) && (cd[ny * texture.Width + x] == TransColor))
                                {
                                    ny += stepdir;
                                    bybottom++;
                                }
                            }
                            //int max = (int)(Math.Max(bytop, bybottom) * cd[y * texture.Width + x - 1].Brightness(5))+1;
                            int max = Math.Max(bytop, bybottom);
                            //if (max < 2) max = 2;
                            for (int ny = 0; Math.Abs(ny) < max - 1; ny += stepdir)
                            {
                                Color insertcolor = new Color(cd[(y - stepdir) * texture.Width + x], (max - Math.Abs(ny) - 1) / (float)max);
                                Color backcolor = cdnow[(ny + y) * texture.Width + x];
                                if (backcolor.A != 0) cdnow[(ny + y) * texture.Width + x] = MixTwoColors(insertcolor, backcolor);
                                else cdnow[(ny + y) * texture.Width + x] = insertcolor;
                            }
                        }

                    }
                }
            }
            texture.SetData(cdnow);
            return texture;
        }
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        public static ProcessThread CurrentThread()
        {
            int id = GetCurrentThreadId();
            return
                (from ProcessThread th in Process.GetCurrentProcess().Threads
                    where th.Id == id
                    select th).Single();
        }

        public static Texture2D SmoothRight(this Texture2D texture, Color[] cd, int stepdir, Color TransColor)
        {
            Color[] cdnow = new Color[texture.Width * texture.Height];
            texture.GetData(cdnow);
            for (int y = 1; y < texture.Height; y++)
            {
                for (int x = 1; x < texture.Width - 1 + stepdir - 1; x++)
                {
                    if (cd[y * texture.Width + x] == TransColor)
                    {
                        if (cd[y * texture.Width + x - stepdir] != TransColor)
                        {
                            int bytop = 0, bybottom = 0;
                            if (y != 0)
                            {
                                int nx = x;
                                while ((cd[(y - 1) * texture.Width + nx] != TransColor) && (cd[(y) * texture.Width + nx] == TransColor))
                                {
                                    nx += stepdir;
                                    bytop++;
                                }
                            }
                            if (y != texture.Height)
                            {
                                int nx = x;
                                while ((cd[(y + 1) * texture.Width + nx] != TransColor) && (cd[(y) * texture.Width + nx] == TransColor))
                                {
                                    nx += stepdir;
                                    bybottom++;
                                }
                            }
                            //int max = (int)(Math.Max(bytop, bybottom) * cd[y * texture.Width + x - 1].Brightness(5))+1;
                            int max = Math.Max(bytop, bybottom);
                            //if (max < 2) max = 2;
                            for (int nx = 0; Math.Abs(nx) < max - 1; nx += stepdir)
                            {
                                Color insertcolor = new Color(cd[y * texture.Width + x - stepdir], (max - Math.Abs(nx) - 1) / (float)max);
                                Color backcolor = cdnow[y * texture.Width + x + nx];
                                if (backcolor.A != 0) cdnow[y * texture.Width + x + nx] = MixTwoColors(insertcolor, backcolor);
                                else cdnow[y * texture.Width + x + nx] = new Color(cd[y * texture.Width + x - stepdir], (max - Math.Abs(nx) - 1) / (float)max);
                            }

                        }
                    }
                }
            }
            texture.SetData(cdnow);
            return texture;
        }

        static public Texture2D CreateCurve(GraphicsDevice graphics, int width, int height, List<Vector2> points, Color color)
        {
            System.Drawing.Image image = new System.Drawing.Bitmap(width, height);
            Texture2D res = new Texture2D(graphics, width, height);
            Color[] data = new Color[width * height];
            res.GetData(data);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                // draw in bmp using g
                g.DrawCurve(new System.Drawing.Pen(color.ToColor()), points.ToPointFs().ToArray());
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image))
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            System.Drawing.Color clr = bmp.GetPixel(x, y); // Get the color of pixel at position 5,5
                            int red = clr.R;
                            int green = clr.G;
                            int blue = clr.B;
                            int alpha = clr.A;
                            data[y * width + x] = new Color(red, green, blue, alpha);
                        }
                    }
                }
            }
            res.SetData(data);
            return res;
        }

        public static List<int> CreateCurve(int width, int height, List<Vector2> points)
        {
            List<int> res = new List<int>();
            System.Drawing.Image image = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                // draw in bmp using g
                g.DrawCurve(new System.Drawing.Pen(System.Drawing.Color.Red), points.ToPointFs().ToArray());
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image))
                {
                    for (int x = 0; x < width; x++)
                    {
                        int y;
                        for (y = 0; y < height; y++)
                        {
                            if (y == 376)
                            {

                            }
                            if (bmp.GetPixel(x, y).R == 255) break;
                        }
                        res.Add(y);
                    }
                }
            }
            return res;
        }

        public static bool InRect(this Vector2 pos, int width, int height)
        {
            return ((pos.X >= 0) && (pos.Y >= 0) && (pos.X < width) && (pos.Y < height));
        }

        public static bool InRect(this Vector2 pos, Vector2 BiggerThan, Vector2 SmallerThan)
        {
            return ((pos.X >= BiggerThan.X) && (pos.Y >= BiggerThan.Y) && (pos.X < SmallerThan.X) && (pos.Y < SmallerThan.Y));
        }

        public static bool InRect(this Vector2 pos, Rectangle rectangle)
        {
            return ((pos.X >= rectangle.Left) && (pos.Y >= rectangle.Bottom) && (pos.X < rectangle.Right) && (pos.Y < rectangle.Top));
        }

        public static bool InRect(this int pos, int width, int height)
        {
            int x = pos % width, y = pos / width;
            return ((x >= 0) && (y >= 0) && (x < width) && (y < height));
        }

        public static bool RectangleIntersects(PointD l1, PointD r1, PointD l2, PointD r2)
        {
            //if (l1.X < l2.X && r2.X < r1.X && l1.Y < l2.Y && r2.Y < r1.Y) return true;
            // if rectangle has area 0, no overlap
            if (l1.X == r1.X || l1.Y == r1.Y || r2.X == l2.X || l2.Y == r2.Y)
                return false;

            // If one rectangle is on left side of other
            if (l1.X > r2.X || l2.X > r1.X)
                return false;

            // If one rectangle is above other
            if (r1.Y > l2.Y || r2.Y > l1.Y)
                return false;

            return true;
        }

        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
        public static void Fill(this Texture2D texture, Vector2 position, Color color_from, Color color_to)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            List<int> target = new List<int>();
            target.Add((int)(position.Y * texture.Width + position.X));
            while (target.Count != 0)
            {
                List<int> next_target = new List<int>();
                foreach (int pos in target)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 vnpos = new Vector2((-1 + i) % 2 + (pos % texture.Width), (i % 2) + (pos / texture.Width));
                        if (i == 3) vnpos.Y -= 2;
                        int npos = (int)(vnpos.Y * texture.Width + vnpos.X);
                        if (vnpos.InRect(texture.Width, texture.Height) && (data[npos] == color_from))
                        {
                            next_target.Add(npos);
                            data[npos] = color_to;
                        }
                    }
                }
                target = next_target;
            }
            texture.SetData(data);
        }

        public static void Fill(this Texture2D texture, Color color_to)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (int i = 0; i < texture.Width * texture.Height; i++)
            {
                data[i] = color_to;
            }
            texture.SetData(data);
        }

        public static void FillNot(this Texture2D texture, Vector2 position, Color color_not_to_fill, Color color_to)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            List<int> target = new List<int>();
            target.Add((int)(position.Y * texture.Width + position.X));
            while (target.Count != 0)
            {
                List<int> next_target = new List<int>();
                foreach (int pos in target)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 vnpos = new Vector2((-1 + i) % 2 + (pos % texture.Width), (i % 2) + (pos / texture.Width));
                        if (i == 3) vnpos.Y -= 2;
                        int npos = (int)(vnpos.Y * texture.Width + vnpos.X);
                        if (vnpos.InRect(texture.Width, texture.Height) && (data[npos] != color_not_to_fill) && (data[npos] != color_to))
                        {
                            next_target.Add(npos);
                            data[npos] = color_to;
                        }
                    }
                }
                target = next_target;
            }
            texture.SetData(data);
        }

        public static List<List<bool>> ToBoolMatrix(this Texture2D texture, List<Color> true_data)
        {
            Color[] cd = new Color[(uint)(texture.Width * texture.Height)];
            texture.GetData(cd);
            List<List<bool>> res = new List<List<bool>>();
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (y == 0) res.Add(new List<bool>());
                    if (true_data.Contains(cd[y * texture.Width + x]))
                    {
                        res[x].Add(true);
                    }
                    else res[x].Add(false);
                }
            }
            return res;
        }

        public static Texture2D CreateRectangle(GraphicsDevice graphics, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(graphics, width, height);
            Color[] cd = new Color[width * height];
            for (int i = 0; i < width * height; i++) cd[i] = color;
            texture.SetData(cd);
            return texture;
        }

        static double section(double h, double r = 1) // returns the positive root of intersection of line y = h with circle centered at the origin and radius r
        {
            //assert(r >= 0); // assume r is positive, leads to some simplifications in the formula below (can factor out r from the square root)
            return (h < r) ? Math.Sqrt(r * r - h * h) : 0; // http://www.wolframalpha.com/input/?i=r+*+sin%28acos%28x+%2F+r%29%29+%3D+h
        }

        static double g(double x, double h, double r = 1) // indefinite integral of circle segment
        {
            return .5f * (Math.Sqrt(1 - x * x / (r * r)) * x * r + r * r * Math.Asin(x / r) - 2 * h * x); // http://www.wolframalpha.com/input/?i=r+*+sin%28acos%28x+%2F+r%29%29+-+h
        }

        static double area(double x0, double x1, double h, double r) // area of intersection of an infinitely tall box with left edge at x0, right edge at x1, bottom edge at h and top edge at infinity, with circle centered at the origin with radius r
        {
            if (x0 > x1)
                MHeleper.Swap(ref x0, ref x1); // this must be sorted otherwise we get negative area
            double s = section(h, r);
            return g(Math.Max(-s, Math.Min(s, x1)), h, r) - g(Math.Max(-s, Math.Min(s, x0)), h, r); // integrate the area
        }

        static double area(double x0, double x1, double y0, double y1, double r) // area of the intersection of a finite box with a circle centered at the origin with radius r
        {
            if (y0 > y1)
                MHeleper.Swap(ref y0, ref y1); // this will simplify the reasoning
            if (y0 < 0)
            {
                if (y1 < 0)
                    return area(x0, x1, -y0, -y1, r); // the box is completely under, just flip it above and try again
                else
                    return area(x0, x1, 0, -y0, r) + area(x0, x1, 0, y1, r); // the box is both above and below, divide it to two boxes and go again
            }
            else
            {
                //    assert(y1 >= 0); // y0 >= 0, which means that y1 >= 0 also (y1 >= y0) because of the swap at the beginning
                return area(x0, x1, y0, r) - area(x0, x1, y1, r); // area of the lower box minus area of the higher box
            }
        }

        public static double Area_of_intersection_between_circle_and_rectangle(double x0, double x1, double y0, double y1, double cx, double cy, double r) // area of the intersection of a general box with a general circle
        {
            x0 -= cx; x1 -= cx;
            y0 -= cy; y1 -= cy;
            // get rid of the circle center

            return area(x0, x1, y0, y1, r);
        }

        public static double Area_of_intersection_between_circle_and_rectangle(Rectangle rect, PointD circle_center, double r) // area of the intersection of a general box with a general circle
        {
            return Area_of_intersection_between_circle_and_rectangle(rect.Left, rect.Right, rect.Bottom, rect.Top, circle_center.X, circle_center.Y, r);
        }

        public static double BezierLength(PointD p0, PointD p1, PointD p2, double posd = 1)
        {
            return get_l_analytic(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y, posd);
        }

        private static double get_l_analytic(double x0, double y0, double x1, double y1, double x2, double y2, double t) // get arclength from parameter t=<0,1>
        {
            double ax, ay, bx, by, A, B, C, b, c, u, k, L;
            ax = x0 - x1 - x1 + x2;
            ay = y0 - y1 - y1 + y2;
            bx = x1 + x1 - x0 - x0;
            by = y1 + y1 - y0 - y0;
            A = 4.0 * ((ax * ax) + (ay * ay));
            B = 4.0 * ((ax * bx) + (ay * by));
            C = (bx * bx) + (by * by);
            b = B / (2.0 * A);
            c = C / A;
            u = t + b;
            k = c - (b * b);
            L = 0.5 * Math.Sqrt(A) *
                (
                 (u * Math.Sqrt((u * u) + k))
                - (b * Math.Sqrt((b * b) + k))
                + (k * Math.Log(Math.Abs((u + Math.Sqrt((u * u) + k)) / (b + Math.Sqrt((b * b) + k)))))
                );
            return L;
        }

        public static double Normalize(this double value, double mod)
        {
            return (value % mod + mod) % mod;
        }
    }
}
