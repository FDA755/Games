using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
	internal class Bird
	{
		Bitmap bitmap;
		int x, y, width, height, speed;

		public Bird(Context context)
		{
			Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.bird);
			var metrics = context.Resources.DisplayMetrics;
			width = metrics.WidthPixels/8;
			Height = width * bitmap.Height / bitmap.Width;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, width, Height, true);

			X = (metrics.WidthPixels - Width) / 2;
			Y = (metrics.HeightPixels - Height) / 2;

			Speed = (int)(10 * metrics.WidthPixels / 1920f);
		}

		public int Width { get => width; set => width = value; }
		public int Height { get => height; set => height = value; }
		public int X { get => x; set => x = value; }
		public int Y { get => y; set => y = value; }
		public Bitmap Bitmap { get => bitmap; set => bitmap = value; }
		public int Speed { get => speed; set => speed = value; }
	}
}