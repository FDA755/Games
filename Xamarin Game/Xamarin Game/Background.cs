using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
	internal class Background
	{
		Bitmap bitmap;
		int x, y, width, height;

		public Background(Context context) 
		{
			Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.background);
			var metrics = context.Resources.DisplayMetrics;
			width = metrics.WidthPixels;
			Height = metrics.HeightPixels;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, width, Height, true);

			X = 0; 
			Y = 0;
		}

		public int Width { get => width; set => width = value; }
		public int Height { get => height; set => height = value; }
		public int X { get => x; set => x = value; }
		public int Y { get => y; set => y = value; }
		public Bitmap Bitmap { get => bitmap; set => bitmap = value; }
	}
}