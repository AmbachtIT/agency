using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Agency.Mathmatics
{
	public struct Rectangle
	{

		public Rectangle(Vector2 topLeft, Vector2 size)
			: this()
		{
			X = topLeft.X;
			Y = topLeft.Y;
			Width = size.X;
			Height = size.Y;
			if (Width < 0)
			{
				throw new ArgumentException();
			}
			if (Height < 0)
			{
				throw new ArgumentException();
			}
		}

		public Rectangle(float x, float y, float width, float height)
			: this()
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
			if (Width < 0)
			{
				throw new ArgumentException();
			}
			if (Height < 0)
			{
				throw new ArgumentException();
			}
		}

		public float X { get; set; }
		public float Y { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }
		public float Left { get { return X; } }
		public float Top { get { return Y; } }
		public float Right { get { return X + Width; } }
		public float Bottom { get { return Y + Height; } }
		public Vector2 TopLeft { get { return new Vector2(Left, Top); } }
		public Vector2 TopRight { get { return new Vector2(Right, Top); } }
		public Vector2 BottomLeft { get { return new Vector2(Left, Bottom); } }
		public Vector2 BottomRight { get { return new Vector2(Right, Bottom); } }
		public Vector2 Center {get {return new Vector2((Left + Right) / 2f, (Top + Bottom) / 2f);}}

		public static Rectangle operator *(Rectangle r, float n)
		{
			return new Rectangle(r.X * n, r.Y * n, r.Width * n, r.Height * n);
		}


		public bool Overlaps(Rectangle other)
		{
			if (Top >= other.Bottom || Bottom <= other.Top)
			{
				return false;
			}
			if (Left >= other.Right || Right <= other.Left)
			{
				return false;
			}
			return true;
		}

		public bool Contains(Vector2 v)
		{
			return v.X >= Left && v.X < Right && v.Y >= Top && v.Y < Bottom;
		}

		public Rectangle Expand(float amount)
		{
			return new Rectangle(this.X - amount, this.Y - amount, this.Width + amount * 2, this.Height + amount * 2);
		}

		public Rectangle ExpandLeft(float amount)
		{
			return new Rectangle(this.X - amount, this.Y, this.Width + amount, this.Height);
		}

		public Rectangle ExpandRight(float amount)
		{
			return new Rectangle(this.X, this.Y, this.Width + amount, this.Height);
		}

		public static Rectangle CreateBounding(IEnumerable<Vector2> points)
		{
			if(!points.Any())
			{
				return new Rectangle();
			}

			var min = points.First();
			var max = points.First();
			foreach(var p in points)
			{
				min = Vector2.Min(p, min);
				max = Vector2.Max(p, max);
			}
			return new Rectangle(
				min.X, 
				min.Y,
				max.X - min.X,
				max.Y - min.Y
			);
		}


		public bool IsEmpty()
		{
			return Width == 0 && Height == 0;
		}

		public Rectangle Expand(Rectangle other)
		{
			var left = System.Math.Min(Left, other.Left);
			var right = System.Math.Max(Right, other.Right);
			var top = System.Math.Min(Top, other.Top);
			var bottom = System.Math.Max(Bottom, other.Bottom);
			return new Rectangle(left,top, right - left, bottom - top);
		}

		public Vector2 Size { get { return BottomRight - TopLeft; } }
	}
}