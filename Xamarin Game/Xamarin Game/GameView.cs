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
		Thread gameThread, renderThread, birdsGeneratorThread;
		ISurfaceHolder surfaceHolder;
		bool isRunning;
		int displayX, displayY;
		int score = 0;
		Paint scorePaint = new Paint();
		float rX, rY;
		Background background;
		List<Bird> birds = new List<Bird>();
		int BIRDS_MAX_COUNT = 4;
		Hero hero;
		private List<Stone> stones = new List<Stone>();
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
			hero = new Hero(context);
			scorePaint.TextSize = 50;
			scorePaint.Color = Color.Black;
		}
		override
		public void Draw(Canvas canvas)
		{
			canvas.DrawBitmap(background.Bitmap, background.X, background.Y, null);
			for (int i = 0; i < birds.Count; i++)
			{
				Bird bird = birds.ElementAt(i);
				canvas.DrawBitmap(bird.Bitmap, bird.X, bird.Y, null);
			}
			canvas.DrawBitmap(hero.Bitmap, hero.X, hero.Y, null);

			if(stones.Count > 0)
			{
				for(int i = 0; i < stones.Count; i++)
				{
					Stone stone = stones.ElementAt(i);
					canvas.DrawBitmap(stone.Bitmap, stone.X, stone.Y, null);
				}
			}
			canvas.DrawText($"SCORE: {score.ToString()}", 5, 45, scorePaint);
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
				List<Bird> birdsTrash = new List<Bird>();
				List<Stone> stonesTrash = new List<Stone>();
				for (int i = 0; i < birds.Count; i++)
				{
					Bird bird = birds.ElementAt(i);
					bird.MoveObject();
					if (stones.Count > 0)
					{
						for (int j = 0; j < stones.Count; j++)
						{
							Stone stone = stones.ElementAt(j);
							if (Rect.Intersects(stone.GetCollosionShape(), bird.GetCollosionShape()))
							{
								score++;
								birdsTrash.Add(bird);
								stonesTrash.Add(stone);
							}
						}
					}
				}
				hero.MoveObject();
				if(stones.Count > 0)
				{
					for(int i = 0; i < stones.Count;  i++)
					{
						Stone stone = stones.ElementAt(i);
						stone.MoveObject();
						if(stone.Y + Height < 0)
						{
							stonesTrash.Add(stone);
						}
					}	
				}
				for (int i = 0; i < birdsTrash.Count; i++)
				{
					if(birds.Count > 0)
					{
						birds.Remove(birdsTrash.ElementAt(i));
					}
				}
				for (int i = 0; i < stonesTrash.Count; i++)
				{
					if(stones.Count > 0)
					{
						stones.Remove(stonesTrash.ElementAt(i));
					}
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
		public override bool OnTouchEvent(MotionEvent e)
		{
			if(e.ActionMasked == MotionEventActions.Down) 
			{
				if(e.GetX() > 0 & e.GetX() < displayX / 3)
				{
					hero.IsMoveLeft = true;
					hero.IsMoveRight = false;
				}
				else if (e.GetX() > (displayX / 3) * 2 & e.GetX() < displayX)
				{
					hero.IsMoveRight = true;
					hero.IsMoveLeft = false;
				}
				else
				{
					Stone stone = new Stone(Context, hero);
					stones.Add(stone);
				}
			}else if (e.ActionMasked == MotionEventActions.Up)
			{
				hero.IsMoveRight = false;
				hero.IsMoveLeft = false;
			}

			return true;
		}
		private void GenerateBirds()
		{
			while(isRunning)
			{
				if(birds.Count < BIRDS_MAX_COUNT)
				{
					birds.Add(new Bird(Context, new Random().Next(0, 4)));
				}
				Thread.Sleep(2500);
			}
		}
		public void Resume()
		{
			isRunning = true;

			gameThread = new Thread(new ThreadStart(Update));
			renderThread = new Thread(new ThreadStart(Run));
			birdsGeneratorThread = new Thread(new ThreadStart(GenerateBirds));

			gameThread.Start();
			renderThread.Start();
			birdsGeneratorThread.Start();
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