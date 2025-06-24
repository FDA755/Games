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
	internal class PbBye : GameObject
	{
		public PbBye(Context context) : base(context)
		{
			Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.bye);
			Width = Metrics.WidthPixels / 2;
			Height = Width * Bitmap.Height / Bitmap.Width;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

			X = (DisplayX - Width) / 2;
			Y = (DisplayY - Height) / 2;
			//X = DisplayX - Width - 30;
			//Y = +20 + Height * 2;
		}
	}
}