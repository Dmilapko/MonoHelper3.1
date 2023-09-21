using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MonoHelper
{
    public  static class Conversions
    {
        public static System.Drawing.PointF Plus(this System.Drawing.PointF operand1, System.Drawing.PointF operand2)
        {
            return new System.Drawing.PointF(operand1.X + operand2.X, operand1.Y + operand2.Y);
        }
        public static PointD ToPointD(this Vector2 vector)
        {
            return new PointD((double)vector.X, (double)vector.Y);
        }

        public static Vector2 FlipY(this Vector2 vector)
        {
            vector.Y = 1080 - vector.Y;
            return vector;
        }

        public static Vector2 FlipX(this Vector2 vector)
        {
            vector.Y = 1920 - vector.Y;
            return vector;
        }

        public static Vector2 FlipY(this Vector2 vector, float height)
        {
            vector.Y = height - vector.Y;
            return vector;
        }

        public static Vector2 FlipX(this Vector2 vector, float width)
        {
            vector.Y = width - vector.Y;
            return vector;
        }

        /// <summary>
        /// Converts from degrees to radians
        /// </summary>
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }

        /// <summary>
        /// Converts from degrees to radians
        /// </summary>
        public static float ToRadians(this float val)
        {
            return (float)(Math.PI / 180) * val;
        }

        /// Converts from degrees to radians
        /// </summary>
        public static float ToRadians(this int val)
        {
            return (float)(Math.PI / 180) * val;
        }


        /// <summary>
        /// Converts from radians to degrees
        /// </summary>
        public static float ToDegrees(this float val)
        {
            return val / (float)Math.PI * 180f;
        }

        /// <summary>
        /// Converts from radians to degrees
        /// </summary>
        public static float ToDegrees(this int val)
        {
            return val / (float)Math.PI * 180f;
        }

        /// <summary>
        /// Converts List of Vector2 to List of System.Drawing.PointF
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<System.Drawing.PointF> ToPointFs(this List<Vector2> points)
        {
            List<System.Drawing.PointF> res = new List<System.Drawing.PointF>();
            foreach (var point in points)
            {
                res.Add(new System.Drawing.PointF(point.X, point.Y));
            }
            return res;
        }

        public static List<System.Drawing.PointF> ToPointFs(this List<PointD> points)
        {
            List<System.Drawing.PointF> res = new List<System.Drawing.PointF>();
            foreach (var point in points)
            {
                res.Add(new System.Drawing.PointF((float)point.X, (float)point.Y));
            }
            return res;
        }

        public static Vector2 ToVector(this System.Drawing.PointF point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static System.Drawing.PointF ToPointF(this Vector2 vector)
        {
            return new System.Drawing.PointF(vector.X, vector.Y);
        }

        /// <summary>
        /// Converts Color to System.Drawing.Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Drawing.Color ToColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Vector2 Turn(this Vector2 v, float angle)
        {
            double l = Math.Sqrt(v.X * v.X + v.Y * v.Y);
            double d = angle + (double)Math.Atan2(v.X, v.Y);
            return new PointD(Math.Sin(d) * l, Math.Cos(d) * l).ToVector2();
        }
    }
}
