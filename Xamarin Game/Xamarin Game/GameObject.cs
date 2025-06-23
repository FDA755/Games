using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
	internal class GameObject
	{

		Bitmap bitmap;
		int x, y, width, height, speed;
		int displayX, displayY;
		DisplayMetrics metrics;
		public GameObject(Context context)
		{
			Metrics = context.Resources.DisplayMetrics;
			DisplayX = metrics.WidthPixels;
			DisplayY = metrics.HeightPixels;
		}
		public virtual void MoveObject() { }

		public Rect GetCollosionShape()
		{
			return new Rect(X, Y, X + Width, Y + Height);	
		}
		public Bitmap Bitmap { get => bitmap; set => bitmap = value; }
		public int X { get => x; set => x = value; }
		public int Y { get => y; set => y = value; }
		public int Width { get => width; set => width = value; }
		public int Height { get => height; set => height = value; }
		public int Speed { get => speed; set => speed = value; }
		public int DisplayX { get => displayX; set => displayX = value; }
		public int DisplayY { get => displayY; set => displayY = value; }
		public DisplayMetrics Metrics { get => metrics; set => metrics = value; }
	}
}