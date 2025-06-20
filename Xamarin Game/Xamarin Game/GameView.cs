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
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
		//Bird bird;
		List<Bird> birds = new List<Bird>();
		int BIRDS_MAX_COUNT = 4;
		public GameView(Android.Content.Context context) : base(context) 
		{
			var metrics = Resources.DisplayMetrics;
			displayX = metrics.WidthPixels; 
			displayY = metrics.HeightPixels;
			rX = displayX / 1920f;
			rY = displayY / 1080f;

			surfaceHolder = Holder;
			surfaceHolder.AddCallback(this);

			background = new Background(context);

			for (int i = 0; i < BIRDS_MAX_COUNT; i++)
			{
				birds.Add(new Bird(context, i));
			}
			//bird = new Bird(context);
		}
		override
		public void Draw(Canvas canvas)
		{
			canvas.DrawBitmap(background.Bitmap, background.X, background.Y, null);
			//canvas.DrawBitmap(bird.Bitmap, bird.X, bird.Y, null);
			for (int i = 0; i < BIRDS_MAX_COUNT; i++)
			{
				Bird bird = birds.ElementAt(i);
				canvas.DrawBitmap(bird.Bitmap, bird.X, bird.Y, null);
			}

		}
		public void Run()
		{
			Canvas canvas = null;
			while(isRunning)
			{
				if(surfaceHolder.Surface.IsValid)
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
				for (int i = 0; i < BIRDS_MAX_COUNT; i++)
				{
					Bird bird = birds.ElementAt(i);
					bird.MoveBird();
				}
				//bird.MoveBird();
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