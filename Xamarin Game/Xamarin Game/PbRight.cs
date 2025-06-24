using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Lights;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Icu.Text.ListFormatter;
using static Android.Text.BoringLayout;

namespace Xamarin_Game
{
	internal class PbRight: GameObject
	{
		//public PbRight(Context context) : base(context)
		//{
		//	Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.pbRight);
		//	Width = Metrics.WidthPixels / 15;
		//	Height = Width * Bitmap.Height / Bitmap.Width;
		//	Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

		//	X = DisplayX - Width - 30;
		//	Y = DisplayY - Height - 10;
		//}
		private readonly Context _context;

		public PbRight(Context context) : base(context)
		{
			_context = context;
			// Можно инициализировать изображение по умолчанию здесь, если нужно
		}

		public void SetImage(string resourceName)
		{
			int resId = _context.Resources.GetIdentifier(resourceName, "drawable", _context.PackageName);
			if (resId == 0)
				throw new ArgumentException($"Drawable resource '{resourceName}' not found.", nameof(resourceName));
			Bitmap = BitmapFactory.DecodeResource(_context.Resources, resId);
			// При необходимости пересчитайте размеры
			Width = Metrics.WidthPixels / 12;
			Height = Width * Bitmap.Height / Bitmap.Width;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

			X = DisplayX - Width - 40;
			Y = DisplayY - Height - 20;
		}
	}
}