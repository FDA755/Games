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
	internal class GameView : SurfaceView , ISurfaceHolderCallback
	{
		Thread gameThread, renderThread, birdsGeneratorThread;
		ISurfaceHolder surfaceHolder;
		bool isExited;
		bool isRunning;
		bool isPlus;
		bool isLevelChanged = false;
		int displayX, displayY;
		int score = 0;
		int plus = 0;
		int levelCount = 0;
		int levelRequiredAmount = 3;
		Paint scorePaint = new Paint();
		Paint scorePlus = new Paint();
		Paint level = new Paint();
		float rX, rY;
		Background background;
		List<Bird> birds = new List<Bird>();
		int BIRDS_MAX_COUNT = 4;
		Hero hero;
		List<Stone> stones = new List<Stone>();

		StaticObject pbLeft;
		StaticObject pbRight;
		StaticObject pbPausePlay;
		StaticObject pbExit;
		StaticObject pbBye;
		StaticObject pbFire;
		StaticObject levelNumber;

		private long plusShowTime = 0;
		private long levelShowTime = 0;
		private const int PLUS_SHOW_DURATION_MS = 500;
		private long gameStartTime = 0;

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

			///////////////////////////////////////////////////////////////////////////////////		
			pbLeft = new StaticObject(context, 12, "pbleft");
			pbLeft.X = 40;
			pbLeft.Y = pbLeft.DisplayY - pbLeft.Height - 20;
			///////////////////////////////////////////////////////////////////////////////////
			pbRight = new StaticObject(context, 12, "pbright");
			pbRight.X = pbRight.DisplayX - pbRight.Width - 40;
			pbRight.Y = pbRight.DisplayY - pbRight.Height - 20;
			///////////////////////////////////////////////////////////////////////////////////
			pbPausePlay = new StaticObject(context, 15, "pbpause");
			pbPausePlay.X = pbPausePlay.DisplayX - pbPausePlay.Width - 30;
			pbPausePlay.Y = 10;
			///////////////////////////////////////////////////////////////////////////////////
			pbExit = new StaticObject(context, 15, "blank");
			pbExit.X = pbExit.DisplayX - pbExit.Width - 30;
			pbExit.Y = 20 + pbExit.Height;
			///////////////////////////////////////////////////////////////////////////////////
			pbBye = new StaticObject(context, 2, "bye");
			pbBye.X = (pbBye.DisplayX - pbBye.Width) / 2;
			pbBye.Y = (pbBye.DisplayY - pbBye.Height) / 2;
			///////////////////////////////////////////////////////////////////////////////////
			pbFire = new StaticObject(context, 10, "fire");
			pbFire.X = pbFire.DisplayX - pbFire.Width - 30;
			pbFire.Y = pbFire.DisplayY - pbFire.Height * 2 - 30;
			///////////////////////////////////////////////////////////////////////////////////
			levelNumber = new StaticObject(context, 4, "lone");
			levelNumber.X = (levelNumber.DisplayX - levelNumber.Width) / 2;
			levelNumber.Y = (levelNumber.DisplayY - levelNumber.Height) / 2;
			///////////////////////////////////////////////////////////////////////////////////
			for (int i = 0; i < BIRDS_MAX_COUNT; i++)
			{
				birds.Add(new Bird(context, i));
			}

			hero = new Hero(context);

			scorePaint.TextSize = 80;
			scorePaint.Color = Color.Black;
			scorePaint.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.BoldItalic));

			level.TextSize = 80;
			level.Color = Color.Black;
			level.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.BoldItalic));

			scorePlus.TextSize = 110;
			scorePlus.Color = Color.Blue;
			scorePlus.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.BoldItalic));

			gameStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		}
		override
		public void Draw(Canvas canvas)
		{		
			canvas.DrawBitmap(background.Bitmap, background.X, background.Y, null);
			
			if (levelCount == 0)
			{
				long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				if (now - gameStartTime < 2000)
				{
					canvas.DrawBitmap(levelNumber.Bitmap, levelNumber.X, levelNumber.Y, null);
					isLevelChanged = true;
				}				
				else
				{
					levelCount = 1;
					isLevelChanged = false;
				}
			}
			if(levelCount == 2)
			{
				long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				if (now - levelShowTime < 2000)
				{
					canvas.DrawBitmap(levelNumber.Bitmap, levelNumber.X, levelNumber.Y, null);
					levelNumber.SetImage("ltwo");
					isLevelChanged = false;
				}
				else
				{
					isLevelChanged = false;
					isRunning = true;
				}
			}
			if (levelCount == 3)
			{
				long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				if (now - levelShowTime < 2000)
				{
					canvas.DrawBitmap(levelNumber.Bitmap, levelNumber.X, levelNumber.Y, null);
					levelNumber.SetImage("lthree");
					isLevelChanged = false;
				}
				else
				{
					isLevelChanged = false;
				}
			}
			if (levelCount == 4)
			{
				long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				if (now - levelShowTime < 2000)
				{
					canvas.DrawBitmap(levelNumber.Bitmap, levelNumber.X, levelNumber.Y, null);
					levelNumber.SetImage("lfour");
					isLevelChanged = false;
				}
				else
				{
					isLevelChanged = false;
				}
			}

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
			canvas.DrawText($"SCORE: {score.ToString()}", 10, 70, scorePaint);
			canvas.DrawText($"LEVEL: {levelCount.ToString()}", displayX - 500, 70, scorePaint);
			if (isPlus)
			{
				long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				if (now - plusShowTime < PLUS_SHOW_DURATION_MS)
				{
					canvas.DrawText($"+{plus.ToString()}", 230, 160, scorePlus);
				}
				else
				{
					isPlus = false;
				}
			}
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
				for (int i = 0; i < birds.Count; i++)
				{
					Bird bird = birds.ElementAt(i);
					if(isLevelChanged == false) 
					{ 
						bird.MoveObject(); 
					}
					if (stones.Count > 0)
					{
						for (int j = 0; j < stones.Count; j++)
						{
							Stone stone = stones.ElementAt(j);

							if (Rect.Intersects(stone.GetCollosionShape(), bird.GetCollosionShape()))
							{
								score++;
								plus = 1;
								plusShowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
								isPlus = true;
								birdsTrash.Add(bird);
								
								stone.Direction = -1;
								stone.SetImage("duckf");
								if(score >= levelRequiredAmount)
								{
									levelShowTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
									isLevelChanged = true;
									levelCount++;
									score = 0;
									ChangeLevel();
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
						if (isLevelChanged == false)
						{
							stone.MoveObject();
						}
						if(stone.Y + Height < 0 || stone.Y > displayY)
						{
							stonesTrash.Add(stone);
						}
						if(isLevelChanged)
						{
							stonesTrash.Add(stone);
						}
					}	
				}
				if (birds.Count > 0)
				{
					for(int i = 0; i < birds.Count; i++)
					{
						Bird bird = birds.ElementAt(i);
						if (isLevelChanged && levelCount > 0)
						{
							birdsTrash.Add(bird);
						}
					}
				}
				for (int i = 0; i < birdsTrash.Count; i++)
				{
					if (birds.Count > 0)
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
		public void ChangeLevel()
		{
			switch(levelCount) 
			{
				case 1:
					levelRequiredAmount = 3;
					break;
				case 2:
					levelRequiredAmount = 4;
					break;
				case 3:
					levelRequiredAmount = 5;
					break;
				case 4:
					levelRequiredAmount = 6;
					break;
				default: break;
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
					stone.SetImage("flybag");
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
			while(isRunning && !isLevelChanged)
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