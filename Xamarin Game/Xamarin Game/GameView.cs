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
using System.Threading;

namespace Xamarin_Game
{
	internal class GameView : SurfaceView, ISurfaceHolderCallback
	{
		Thread gameThread, renderThread;
		ISurfaceHolder surfaceHolder;
		bool isRunning;
		int displayX, displayY;
		float rX, rY;
		Background background;
		Bird bird;
		public GameView(Context context) : base(context) 
		{
			var metrics = Resources.DisplayMetrics;
			displayX = metrics.WidthPixels; 
			displayY = metrics.HeightPixels;
			rX = displayX / 1920f;
			rY = displayY / 1080f;

			surfaceHolder = Holder;
			surfaceHolder.AddCallback(this);

			background = new Background(context);
			bird = new Bird(context);
		}
		override
		public void Draw(Canvas canvas)
		{
			canvas.DrawBitmap(bird.Bitmap, bird.X, bird.Y, null);
			canvas.DrawBitmap(background.Bitmap, background.X, background.Y, null);
		}
		public void Run()
		{
			Canvas canvas = null;
			while(isRunning)
			{
				if(surfaceHolder.Surface != null)
				{
					canvas = surfaceHolder.LockCanvas();
					Draw(canvas);
					surfaceHolder.UnlockCanvasAndPost(canvas);
				}
				Thread.Sleep(17);
			}

		}
		public void Update()
		{
			while(isRunning)
			{
				bird.X += bird.Speed;
				if(bird.X + bird.Width > displayX)
				{
					bird.Speed *= -1;
				}
				else if (bird.X < 0)
				{
					bird.Speed *= -1;
				}
				Thread.Sleep(17);
			}
		}
		public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
		{
		
		}
		public void SurfaceCreated(ISurfaceHolder holder)
		{
			Resume();
		}
		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			Pause();
		}

		public void Resume()
		{
			isRunning = true;
			gameThread = new Thread(new ThreadStart(Update));
			renderThread = new Thread(new ThreadStart(Run));

			gameThread.Start();
			renderThread.Start();
		}
		public void Pause()
		{
			bool retry = true;
			while(retry) 
			{
				try
				{
					isRunning = false;
					gameThread.Join();
					renderThread.Join();
					retry = false;
				}
				catch( Exception e )
				{
                    Console.WriteLine(e.Message);
                }
				
			}
		}
	}
}