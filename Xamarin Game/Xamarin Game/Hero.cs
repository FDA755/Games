using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin_Game
{
	internal class Hero : GameObject
	{
		bool isMoveLeft;
		bool isMoveRight;
		public Hero(Context context) : base(context)
		{
			//Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.slingshot2);
			Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.standBag);
			Width = Metrics.WidthPixels / 10;
			Height = Width * Bitmap.Height / Bitmap.Width;
			Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

			X = (DisplayX - Width) / 2;
			Y = DisplayY - Height;

			Speed = (int)(6 * Metrics.WidthPixels / 1920f);
		}
		public override void MoveObject()
		{
			if(isMoveLeft & !isMoveRight) 
			{
				X -= Speed;
				if(X <= 0)
				{
					X = 0;
				}
			}else if(!isMoveLeft & isMoveRight)
			{
				X += Speed;
				if ((X + Width) > DisplayX)
				{
					X = DisplayX - Width;
				}
			}
		}

		public bool IsMoveLeft { get => isMoveLeft; set => isMoveLeft = value; }
		public bool IsMoveRight { get => isMoveRight; set => isMoveRight = value; }
	}
}