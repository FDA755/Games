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
	internal class Stone : GameObject
	{
		int direction = 1;
		public bool IsFrozen { get; set; }
		public long FreezeEndTime { get; set; }
		public int FrozenY { get; set; }
		private readonly Context _context;
		public Stone(Context context, Hero hero) : base(context)
		{
			_context = context;
			// Можно инициализировать изображение по умолчанию здесь, если нужно
			X = (hero.X + ((hero.Width - Width) / 2));
			Y = hero.Y - Height;
		}
		public void SetImage(string resourceName)
		{
			int resId = _context.Resources.GetIdentifier(resourceName, "drawable", _context.PackageName);
			if (resId == 0)
				throw new ArgumentException($"Drawable resource '{resourceName}' not found.", nameof(resourceName));
			Bitmap = BitmapFactory.DecodeResource(_context.Resources, resId);
			// При необходимости пересчитайте размеры
			Width = Metrics.WidthPixels / 15;
			Height = Width * Bitmap.Height / Bitmap.Width;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

			Speed = (int)(6 * Metrics.WidthPixels / 1920f);
		}
		public int Direction { get => direction; set => direction = value; }
		public override void MoveObject()
		{
			if (IsFrozen)
			{
				Direction = -1;
				Y = FrozenY;
				if (DateTimeOffset.Now.ToUnixTimeMilliseconds() >= FreezeEndTime)
				{
					IsFrozen = false;
					SetImage("duckf");
				}
				return;
			}
			else
				Y -= Speed * Direction;			
		}
	}
}