using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
	internal class Bird: GameObject
	{
		int[] ducksId = { Resource.Drawable.duck0, Resource.Drawable.duck1, Resource.Drawable.duck2, Resource.Drawable.duck3 };
		public Bird(Context context, int i):base(context)
		//public Bird(Context context):base(context)
		{
			Random random = new Random();
			int index = random.Next(0, ducksId.Length);
			Bitmap = BitmapFactory.DecodeResource(context.Resources, ducksId[index]);
			var metrics = context.Resources.DisplayMetrics;
			Width = metrics.WidthPixels/16;
			Height = Width * Bitmap.Height / Bitmap.Width;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

			X = random.Next(0, DisplayX - Width);
			//X = (metrics.WidthPixels - Width) / 2;
			Y = i * Height;
			//Y = (metrics.HeightPixels - Height) / 2;

			Speed =  - (int)(10 * metrics.WidthPixels / 1920f);
		}
		public void MoveBird()
		{
			X += Speed;
			if (X + Width > DisplayX)
			{
				Speed *= -1;
				Bitmap = createFlippledBitmap(Bitmap, true, false);
			}
			else if (X < 0)
			{
				Speed *= -1;
				Bitmap = createFlippledBitmap(Bitmap, true, false);
			}
		}
		public Bitmap createFlippledBitmap(Bitmap source, bool xFlip, bool yFlip)
		{
			Matrix matrix = new Matrix();
			matrix.PostScale(xFlip ? -1:1, yFlip ? -1:1, source.Width/2, source.Height/2);
			return Bitmap.CreateBitmap(source,0,0, Width, Height, matrix, true);
		}
	}
}