using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
	internal class Button: GameObject
	{
		private readonly Context _context;
		public Button(Context context, int scale, string resourceName):base(context) 
		{
			_context = context;
			Scale = scale;
			SetImage(resourceName);
		}
		public void SetImage(string resourceName1)
		{
			int resId = _context.Resources.GetIdentifier(resourceName1, "drawable", _context.PackageName);
			if (resId == 0)
				throw new ArgumentException($"Drawable resource '{resourceName1}' not found.", nameof(resourceName1));
			Bitmap = BitmapFactory.DecodeResource(_context.Resources, resId);

			Width = Metrics.WidthPixels / Scale;
			Height = Width * Bitmap.Height / Bitmap.Width;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);
		}
	}
}