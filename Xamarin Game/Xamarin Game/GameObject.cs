using Android.Content;
using Android.Graphics;
using Android.Util;
using System;

namespace Xamarin_Game
{
	internal class GameObject
	{
		Bitmap bitmap;
		int x, y, width, height, speed, scale;
		int displayX, displayY;
		DisplayMetrics metrics;
		private Context context;
		public GameObject(Context context)
		{
			this.context = context;
			Metrics = context.Resources.DisplayMetrics;
			DisplayX = metrics.WidthPixels;
			DisplayY = metrics.HeightPixels;
		}
		public virtual void MoveObject() { }
		public Rect GetCollosionShape()
		{
			return new Rect(X, Y, X + Width, Y + Height);
		}
		public Rect GetCollosionShapeHit()
		{
			return new Rect(X, Y, X + Width, Y + Height/2);
		}
		public virtual void Hide() { }
		public Bitmap Bitmap { get => bitmap; set => bitmap = value; }
		public int X { get => x; set => x = value; }
		public int Y { get => y; set => y = value; }
		public int Width { get => width; set => width = value; }
		public int Height { get => height; set => height = value; }
		public int Speed { get => speed; set => speed = value; }
		public int DisplayX { get => displayX; set => displayX = value; }
		public int DisplayY { get => displayY; set => displayY = value; }
		public int Scale { get => scale; set => scale = value; }
		public DisplayMetrics Metrics { get => metrics; set => metrics = value; }
	}
}