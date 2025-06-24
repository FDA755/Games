using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
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
using System.Timers;

namespace Xamarin_Game
{
	internal class GameView : SurfaceView, ISurfaceHolderCallback
	{
		Thread gameThread, renderThread, birdsGeneratorThread;
		ISurfaceHolder surfaceHolder;
		int collisionX, collisionY = 0;
		bool isExited;
		bool isRunning;
		int displayX, displayY;
		int score = 0;
		Paint scorePaint = new Paint();
		float rX, rY;
		Background background;
		List<Bird> birds = new List<Bird>();
		int BIRDS_MAX_COUNT = 4;
		Hero hero;
		List<BirdF> birdsf = new List<BirdF>();
		List<Stone> stones = new List<Stone>();

		PbLeft pbLeft;
		PbRight pbRight;
		PbPausePlay pbPausePlay;
		PbExit pbExit;
		PbBye pbBye;
		PbFire pbFire;

		System.Timers.Timer timer = new System.Timers.Timer(1000);
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

			scorePaint.TextSize = 80;
			scorePaint.Color = Color.Black;
			scorePaint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.BoldItalic));


			pbLeft = new PbLeft(context);
			pbLeft.SetImage("pbleft");
			pbRight = new PbRight(context);
			pbRight.SetImage("pbright");
			pbPausePlay = new PbPausePlay(context);
			pbPausePlay.SetImage("pbpause");
			pbExit = new PbExit(context);
			pbExit.SetImage("blank");
			pbBye = new PbBye(context);
			pbFire = new PbFire(context);
			pbFire.SetImage("fire");
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

			if (birdsf.Count > 0)
			{

				for (int i = 0; i < birdsf.Count; i++)
				{
					BirdF birdf = birdsf.ElementAt(i);

					canvas.DrawBitmap(birdf.Bitmap, birdf.X, birdf.Y, null);
				}
			}
			canvas.DrawText($"SCORE: {score.ToString()}", 10, 70, scorePaint);

			canvas.DrawBitmap(pbLeft.Bitmap, pbLeft.X, pbLeft.Y, null);
			canvas.DrawBitmap(pbRight.Bitmap, pbRight.X, pbRight.Y, null);
			if (isRunning && !isExited)
			{
				canvas.DrawBitmap(pbPausePlay.Bitmap, pbPausePlay.X, pbPausePlay.Y, null);
				canvas.DrawBitmap(pbExit.Bitmap, pbExit.X, pbExit.Y, null);
			}
			else if(!isRunning && !isExited)
			{
				canvas.DrawBitmap(pbPausePlay.Bitmap, pbPausePlay.X, pbPausePlay.Y, null);
				canvas.DrawBitmap(pbExit.Bitmap, pbExit.X, pbExit.Y, null);
			}
			else if(isExited)
			{
				canvas.DrawBitmap(pbBye.Bitmap, pbBye.X, pbBye.Y, null);
			}
			canvas.DrawBitmap(pbFire.Bitmap, pbFire.X, pbFire.Y, null);
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
				//Thread.Sleep(1);
			}
		}
		public void Update()
		{
			while(isRunning)
			{
				List<Bird> birdsTrash = new List<Bird>();
				List<Stone> stonesTrash = new List<Stone>();
				List<BirdF> birdsfTrash = new List<BirdF>();
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

								for(int k = 0; k < birdsf.Count; k++)
								{
									BirdF birdf = new BirdF(Context);
									birdsf.Add(birdf);
									birdf = birdsf.ElementAt(k);
									birdf.MoveObject();									
									Thread.Sleep(100);
								}
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
				if (birdsf.Count > 0)
				{
					for (int i = 0; i < birdsf.Count; i++)
					{
						BirdF birdf = birdsf.ElementAt(i);
						birdf.MoveObject();
						if (birdf.Y > displayY)
						{
							birdsfTrash.Add(birdf);
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
				for (int i = 0; i < birdsfTrash.Count; i++)
				{
					if (birdsf.Count > 0)
					{
						birdsf.Remove(birdsfTrash.ElementAt(i));
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
				if(e.GetY() > pbLeft.GetCollosionShape().Top
					& e.GetY() < pbLeft.GetCollosionShape().Bottom
					& e.GetX() > pbLeft.GetCollosionShape().Left
					& e.GetX() < pbLeft.GetCollosionShape().Right
					& !isExited)
				{
					hero.IsMoveLeft = true;
					hero.IsMoveRight = false;
					pbLeft.SetImage("pbleftg");
				}
				else if (e.GetY() > pbRight.GetCollosionShape().Top
					& e.GetY() < pbRight.GetCollosionShape().Bottom
					& e.GetX() > pbRight.GetCollosionShape().Left
					& e.GetX() < pbRight.GetCollosionShape().Right
					& !isExited)
				{
					hero.IsMoveRight = true;
					hero.IsMoveLeft = false;
					pbRight.SetImage("pbrightg");
				}
				else if (e.GetY() > pbFire.GetCollosionShape().Top
					& e.GetY() < pbFire.GetCollosionShape().Bottom
					& e.GetX() > pbFire.GetCollosionShape().Left
					& e.GetX() < pbFire.GetCollosionShape().Right
					& !isExited)
				{
					pbFire.SetImage("firer");

					Stone stone = new Stone(Context, hero);
					stones.Add(stone);
					
					Thread.Sleep(100);
				}
				else if (e.GetY() > pbPausePlay.GetCollosionShape().Top
					& e.GetY() < pbPausePlay.GetCollosionShape().Bottom
					& e.GetX() > pbPausePlay.GetCollosionShape().Left 
					& e.GetX() < pbPausePlay.GetCollosionShape().Right
					& isRunning == true & !isExited)
				{
					pbPausePlay.SetImage("pbplay");
					pbExit.SetImage("pbexit");
					Pause();
				}
				else if (e.GetY() > pbPausePlay.GetCollosionShape().Top
					& e.GetY() < pbPausePlay.GetCollosionShape().Bottom
					& e.GetX() > pbPausePlay.GetCollosionShape().Left
					& e.GetX() < pbPausePlay.GetCollosionShape().Right
					& isRunning == false & !isExited)
				{
					pbPausePlay.SetImage("pbpause");
					pbExit.SetImage("blank");
					Resume();
				}
				else if (e.GetY() > pbExit.GetCollosionShape().Top
					& e.GetY() < pbExit.GetCollosionShape().Bottom
					& e.GetX() > pbExit.GetCollosionShape().Left
					& e.GetX() < pbExit.GetCollosionShape().Right
					& isRunning == false & !isExited)
				{
					isExited = true;
					Resume();
				}
				else if(e.GetY() > pbBye.GetCollosionShape().Top
					& e.GetY() < pbBye.GetCollosionShape().Bottom
					& e.GetX() > pbBye.GetCollosionShape().Left
					& e.GetX() < pbBye.GetCollosionShape().Right
					&isExited && isRunning)
					{
						Exit();
					}
			}
			else if (e.ActionMasked == MotionEventActions.Up)
			{
				hero.IsMoveRight = false;
				hero.IsMoveLeft = false;
				pbLeft.SetImage("pbleft");
				pbRight.SetImage("pbright");
				pbFire.SetImage("fire");
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
		public void Exit()
		{
			isRunning = true;
			System.Environment.Exit(0);
		}
	}
}