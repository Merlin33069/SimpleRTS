using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimpleRts
{
	static class GameConfig
	{
		internal static Vector2 origin = new Vector2(12.5f, 12.5f);

		internal static class Base
		{
			internal static byte hp = 250;
			internal static byte price = 0;
			internal static Texture2D texture = Game1.instance.Base;
		}
		internal static class Resource
		{
			internal static byte hp = 10;
			internal static byte price = 100;
			internal static Texture2D texture = Game1.instance.Resource;
		}
		internal static class Factory
		{
			internal static byte hp = 10;
			internal static byte price = 100;
			internal static Texture2D texture = Game1.instance.Factory;
		}
		internal static class Wall
		{
			internal static byte hp = 5;
			internal static byte price = 25;
			internal static Texture2D texture = Game1.instance.Wall;
		}
		internal static class Unit
		{
			internal static byte hp = 1;
			internal static byte price = 1;
			internal static Texture2D texture = Game1.instance.Unit;
		}
		internal static class BaseRates
		{
			internal static float goldTick = .025f; //Gold per tick (per resource building)
			internal static byte unitTicks = 120; //Ticks per unit (per factory)
			internal static byte aiUpdateWait = 60;
		}
	}
}
