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
		internal class BirdF : GameObject
		{
			public BirdF(Context context) : base(context)
			{
				Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.stone);
				Width = Metrics.WidthPixels / 15;
				Height = Width * Bitmap.Height / Bitmap.Width;
				Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

				X = DisplayX/2;
				Y = DisplayY/2;

				Speed = (int)(6 * Metrics.WidthPixels / 1920f);
			}
			public override void MoveObject()
			{
				Y += Speed;
			}
		}
	}