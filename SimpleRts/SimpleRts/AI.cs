using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleRts
{
	class AI
	{
		internal static int TickCount = 0;
		Random random = new Random();
		Player player; //The player instance that this AI controls
		
		internal AI(Player p)
		{
			player = p;
		}

		internal void update()
		{
			if (TickCount == GameConfig.BaseRates.aiUpdateWait)
			{
				float CurrentCost = (player.Factories.Count * GameConfig.Unit.price);
				float CurrentIncome = ((float)player.ResourceBuildings.Count * GameConfig.BaseRates.goldTick * (float)GameConfig.BaseRates.unitTicks);

				Game1.instance.message1 = player.ResourceBuildings.Count + " " + GameConfig.BaseRates.goldTick + " " + GameConfig.BaseRates.unitTicks;

				if (CurrentIncome - CurrentCost <= 5)
				{
					if (player.money < GameConfig.Resource.price)
					{
						//Well, were fucked...
						return;
					}
					else
					{
						//We have enough money to make a new resource building!
						PlaceResourceBuilding();
					}
				}
				else
				{
					if (player.money < GameConfig.Factory.price)
					{
						//We dont have a lot of money, lets save up a bit
					}
					else
					{
						//We have plenty of money, do we want walls or a factory?
						if (player.Factories.Count < 5)
						{
							//Lets have a minimum of 5 factories, before we make walls
							PlaceFactory();
						}
						else
						{
							PlaceWall();
						}
					}
				}
			}
		}

		void PlaceFactory()
		{
			float x = random.Next(100, 201);
			float y = random.Next(0, 600);

			player.Factories.Add(new entity(player.Base.Position.X - x, y, GameConfig.Factory.hp));
			player.money -= GameConfig.Factory.price;
		}
		void PlaceResourceBuilding()
		{
			float x = random.Next(25, 76);
			float y = random.Next(0, 600);

			player.ResourceBuildings.Add(new entity(player.Base.Position.X - x, y, GameConfig.Resource.hp));
			player.money -= GameConfig.Resource.price;
		}
		void PlaceWall()
		{
			float x = random.Next(200, 301);
			float y = random.Next(0, 600);

			player.Walls.Add(new entity(player.Base.Position.X - x, y, GameConfig.Wall.hp));
			player.money -= GameConfig.Wall.price;
		}
	}
}
