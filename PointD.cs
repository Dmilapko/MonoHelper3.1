using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoHelper;

namespace MonoHelper
{
    
    public struct PointD
    {
        public double X, Y;

        public PointD(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
        }

        public static PointD operator +(PointD point1, PointD point2)
        {
            return new PointD(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static PointD operator *(PointD point1, PointD point2)
        {
            return new PointD(point1.X * point2.X, point1.Y * point2.Y);
        }

        public static PointD operator -(PointD point1, PointD point2)
        {
            return new PointD(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static PointD operator *(PointD point, double multiplier)
        {
            return new PointD(point.X * multiplier, point.Y * multiplier);
        }

        public static PointD operator *(double multiplier, PointD point)
        {
            return new PointD(point.X * multiplier, point.Y * multiplier);
        }

        public static PointD operator /(PointD point, double divider)
        {
            return new PointD(point.X / divider, point.Y / divider);
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float)X, (float)Y);
        }

        public double Angle()
        {
            return Math.Atan2(X, Y);
        }

        public void Round()
        {
            X = Math.Round(X);
            Y = Math.Round(Y);
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public PointD Turn(double angle)
        {
            double l = Math.Sqrt(X * X + Y * Y);
            double d = angle + (double)Math.Atan2(X, Y);
            return new PointD(Math.Sin(d) * l, Math.Cos(d) * l);
        }

        public void Rotate(double angle)
        {
            this = Turn(angle);
        }

        public bool InRect(Rectangle rect)
        {
            return (X < rect.Right) && (X >= rect.Left) && (Y >= rect.Bottom) && (Y < rect.Top);
        }
    }
}
