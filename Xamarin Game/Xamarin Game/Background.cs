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
	internal class Background : GameObject
	{
		public Background(Context context) : base(context)
		{
			Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.forest);
			var metrics = context.Resources.DisplayMetrics;
			Width = metrics.WidthPixels;
			Height = metrics.HeightPixels;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

			X = 0; 
			Y = 0;
		}
	}
}