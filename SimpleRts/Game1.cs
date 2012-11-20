using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SimpleRts
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		internal static Game1 instance;
		internal string message1 = "";

		float maxFps = 0;
		int distanceFromUpperLeft = 0;
		int distanceFromUpperRight = 0;

		#region Content
		//Fonts
		internal SpriteFont font;

		//Textures
		internal Texture2D Base;
		internal Texture2D Unit;
		internal Texture2D Resource;
		internal Texture2D Factory;
		internal Texture2D Wall;

		//Sounds
		internal SoundEffect Explosion;
		internal SoundEffect Crash;
		internal SoundEffect Placed;
		#endregion

		internal Player player1;
		internal Player player2;
		internal AI ai;
		MouseState currentMouseState;
		MouseState oldMouseState;
		KeyboardState currentKeyboardState;
		KeyboardState oldKeyboardState;

		long totalunits;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			instance = this;
			IsMouseVisible = true;
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
		}
		protected override void Initialize()
		{
			player1 = new Player(new Vector3(22.5f, (float)(graphics.PreferredBackBufferHeight / 2), 250));
			player2 = new Player(new Vector3((float)(graphics.PreferredBackBufferWidth - 22.5), (float)(graphics.PreferredBackBufferHeight / 2), 250));
			player1.enemy = player2;
			player2.enemy = player1;

			ai = new AI(player2);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//Fonts
			font = Content.Load<SpriteFont>("fonts/kootenay");

			//Textures
			Base = Content.Load<Texture2D>("buildings/base");
			Unit = Content.Load<Texture2D>("units/unit");
			Resource = Content.Load<Texture2D>("buildings/resource");
			Factory = Content.Load<Texture2D>("buildings/factory");
			Wall = Content.Load<Texture2D>("buildings/wall");

			//Sounds
			Explosion = Content.Load<SoundEffect>("sounds/explosion");
			Crash = Content.Load<SoundEffect>("sounds/collision");
			Placed = Content.Load<SoundEffect>("sounds/buildingplaced");
		}
		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			Player.TickCount++;
			AI.TickCount++;
			UpdateInput();
			ai.update();

			player1.money += GameConfig.BaseRates.goldTick * player1.ResourceBuildings.Count; //TODO multiply this by the number of resource stations this player has!
			player2.money += GameConfig.BaseRates.goldTick * player2.ResourceBuildings.Count; //TODO multiply this by the number of resource stations this player has!

			player1.Update();
			player2.Update();

			if (Player.TickCount >= GameConfig.BaseRates.unitTicks) Player.TickCount = 0;
			if (AI.TickCount >= GameConfig.BaseRates.aiUpdateWait) AI.TickCount = 0;
			base.Update(gameTime);

			totalunits = player1.Units.Count + player2.Units.Count;
		}
		void UpdateInput()
		{
			if (!this.IsActive) return;

			currentMouseState = Mouse.GetState();
			currentKeyboardState = Keyboard.GetState();

			if (currentMouseState.X < 0 || currentMouseState.Y < 0) return;

			if (currentMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
			{
				player1.Factories.Add(new entity(currentMouseState.X, currentMouseState.Y, GameConfig.Factory.hp));
				player1.money -= GameConfig.Factory.price;

				//player2.Factories.Add(new entity(graphics.PreferredBackBufferWidth - currentMouseState.X, currentMouseState.Y, GameConfig.Factory.hp));
			}
			if (currentMouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released)
			{
				player1.ResourceBuildings.Add(new entity(currentMouseState.X, currentMouseState.Y, GameConfig.Resource.hp));
				player1.money -= GameConfig.Factory.price;

				//player2.ResourceBuildings.Add(new entity(graphics.PreferredBackBufferWidth - currentMouseState.X, currentMouseState.Y, GameConfig.Resource.hp));
			}

			oldMouseState = currentMouseState;
			oldKeyboardState = currentKeyboardState;
		}


		protected override void Draw(GameTime gameTime)
		{
			var frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (frameRate > maxFps) maxFps = frameRate;

			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

			player1.Draw(spriteBatch, Color.Green);
			player2.Draw(spriteBatch, Color.Red);


			DrawInUpperLeft("Money: " + (int)player1.money, spriteBatch);
			DrawInUpperLeft("EMoney: " + (int)player2.money, spriteBatch);

			DrawInUpperRight("CFPS: " + frameRate, spriteBatch); //Current FPS
			DrawInUpperRight("MFPS: " + maxFps, spriteBatch); //Max FPS
			DrawInUpperRight("P1T: " + Player.TickCount, spriteBatch); //Max FPS
			DrawInUpperRight("CUT: " + totalunits, spriteBatch); //Current unit total
			DrawInUpperRight("CCP: NYI", spriteBatch); //Current Cursor Position
			DrawInUpperRight(message1, spriteBatch); //Game Message

			distanceFromUpperLeft = 0;
			distanceFromUpperRight = 0;

			spriteBatch.End();
			base.Draw(gameTime);
		}

		void DrawInUpperLeft(string s, SpriteBatch SB)
		{
			SB.DrawString(font, s, new Vector2(10, distanceFromUpperLeft), Color.White);
			distanceFromUpperLeft += 15;
		}
		void DrawInUpperRight(string s, SpriteBatch SB)
		{
			SB.DrawString(font, s, new Vector2((graphics.PreferredBackBufferWidth - font.MeasureString(s).X) - 10, distanceFromUpperRight), Color.White);
			distanceFromUpperRight += 15;
		}
	}
}
