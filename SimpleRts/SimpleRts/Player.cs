using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace SimpleRts
{
	class Player
	{
		List<entity> RecycledEntities = new List<entity>();

		internal static int TickCount = 0;
		internal float money = 1000;
		internal Player enemy;

		Player() { }
		internal Player(Vector3 baseLocation)
		{
			Base = new entity(baseLocation);
		}

		internal entity Base;
		internal List<entity> Units = new List<entity>();
		internal List<entity> ResourceBuildings = new List<entity>();
		internal List<entity> Factories = new List<entity>();
		internal List<entity> Walls = new List<entity>();

		internal void Draw(SpriteBatch Sb, Color color)
		{
			Sb.Draw(GameConfig.Base.texture, Base.Position, null, color, 0f, GameConfig.origin, 1f, SpriteEffects.None, 0f);

			//Units (the first for loop) are unique in that they get rotated, and have no HP, so the third DataValue is the Rotation Value
			for (int i = 0; i < Units.Count; ++i)
				Sb.Draw(GameConfig.Unit.texture, Units[i].Position, null, color, Units[i].Data, GameConfig.origin, 1f, SpriteEffects.None, 0f);
			for (int i = 0; i < ResourceBuildings.Count; ++i)
				Sb.Draw(GameConfig.Resource.texture, ResourceBuildings[i].Position, null, color, 0f, GameConfig.origin, 1f, SpriteEffects.None, 0f);
			for (int i = 0; i < Factories.Count; ++i)
				Sb.Draw(GameConfig.Factory.texture, Factories[i].Position, null, color, 0f, GameConfig.origin, 1f, SpriteEffects.None, 0f);
			for (int i = 0; i < Walls.Count; ++i)
				Sb.Draw(GameConfig.Wall.texture, Walls[i].Position, null, color, 0f, GameConfig.origin, 1f, SpriteEffects.None, 0f);
		}

		internal void Update()
		{
			if (TickCount >= GameConfig.BaseRates.unitTicks)
			{
				UpdateFactories();
			}

			UpdateUnits();
		}

		//Simply Create new units if the timer is over the setting
		void UpdateFactories()
		{
			for (int i = 0; i < Factories.Count; ++i)
			{
				if (money >= GameConfig.Unit.price)
				{
					if (RecycledEntities.Count > 0)
					{
						UseRecycledEntity(Factories[i]);
					}
					else
					{
						NewEntity(Factories[i]);
					}

					money -= GameConfig.Unit.price;
				}
			}
		}

		void UseRecycledEntity(entity factory)
		{
			entity temp = RecycledEntities[0];
			RecycledEntities.Remove(temp);
			temp.Position = factory.Position;
			Units.Add(temp);
		}
		void NewEntity(entity factory)
		{
			Units.Add(new entity(factory.Position, 0));
		}

		void UpdateUnits()
		{
			for (int i = 0; i < Units.Count; ++i)
			{
				bool destroyed = false;
				entity currentUnit = Units[i];
				if (enemy == null) Console.WriteLine("Enemy is null!");
				if (enemy.Base == null) Console.WriteLine("Enemy base is null!");
				entity currentEnemyEntity = enemy.Base;

				entity target = currentEnemyEntity;
				float DistanceToTarget = Vector2.Distance(currentUnit.Position, enemy.Base.Position);

				//Holds data we dont want to recreate on each loop
				float CurrentDistance = 0;
				int DistanceForDestroying = 25;

				if (DistanceToTarget < DistanceForDestroying)
				{
					Units.Remove(currentUnit);
					RecycledEntities.Add(currentUnit);
					target.Data -= 1;
					if (target.Data == 0)
					{
						Game1.instance.Explosion.Play();
						//PLAYER WINS
						//TODO EndGame();
					}
					else
					{
						Game1.instance.Crash.Play();
					}
					continue;
				}

				//Check enemy units
				//for (int e = 0; e < enemy.Units.Count; ++e)
				//{
				//    currentEnemyEntity = enemy.Units[e];
				//    CurrentDistance = Vector2.Distance(currentUnit.Position, currentEnemyEntity.Position);

				//    if (CurrentDistance < DistanceForDestroying)
				//    {
				//        Units.Remove(currentUnit);
				//        enemy.Units.Remove(currentEnemyEntity);
				//        RecycledEntities.Add(currentUnit);
				//        RecycledEntities.Add(currentEnemyEntity);
				//        //Game1.instance.Crash.Play();
				//        destroyed = true;
				//        break;
				//    }

				//    if (CurrentDistance < DistanceToTarget)
				//    {
				//        target = currentEnemyEntity;
				//        DistanceToTarget = CurrentDistance;
				//    }
				//}

				if (enemy.Units.Count > 0)
				{
					currentEnemyEntity = enemy.Units[0];
					CurrentDistance = Vector2.Distance(currentUnit.Position, currentEnemyEntity.Position);

					if (CurrentDistance < DistanceForDestroying)
					{
						Units.Remove(currentUnit);
						enemy.Units.Remove(currentEnemyEntity);
						RecycledEntities.Add(currentUnit);
						RecycledEntities.Add(currentEnemyEntity);
						Game1.instance.Crash.Play();
						continue;
					}

					if (CurrentDistance < DistanceToTarget)
					{
						target = currentEnemyEntity;
						DistanceToTarget = CurrentDistance;
					}
				}
				else
				{

					//Check enemy walls
					for (int e = 0; e < enemy.Walls.Count; ++e)
					{
						currentEnemyEntity = enemy.Walls[e];
						CurrentDistance = Vector2.Distance(currentUnit.Position, currentEnemyEntity.Position);

						if (CurrentDistance < DistanceForDestroying)
						{
							Units.Remove(currentUnit);
							RecycledEntities.Add(currentUnit);
							currentEnemyEntity.Data -= 1;

							if (currentEnemyEntity.Data == 0)
							{
								enemy.Walls.Remove(currentEnemyEntity);
								RecycledEntities.Add(currentEnemyEntity);
								Game1.instance.Explosion.Play();
							}
							else
							{
								Game1.instance.Crash.Play();
							}
							destroyed = true;
							break;
						}

						if (CurrentDistance < DistanceToTarget)
						{
							target = currentEnemyEntity;
							DistanceToTarget = CurrentDistance;
						}
					}
					if (destroyed) continue;

					//Check enemy factories
					for (int e = 0; e < enemy.Factories.Count; ++e)
					{
						currentEnemyEntity = enemy.Factories[e];
						CurrentDistance = Vector2.Distance(currentUnit.Position, currentEnemyEntity.Position);

						if (CurrentDistance < DistanceForDestroying)
						{
							Units.Remove(currentUnit);
							RecycledEntities.Add(currentUnit);
							currentEnemyEntity.Data -= 1;

							if (currentEnemyEntity.Data == 0)
							{
								enemy.Factories.Remove(currentEnemyEntity);
								RecycledEntities.Add(currentEnemyEntity);
								Game1.instance.Explosion.Play();
							}
							else
							{
								Game1.instance.Crash.Play();
							}
							destroyed = true;
							break;
						}

						if (CurrentDistance < DistanceToTarget)
						{
							target = currentEnemyEntity;
							DistanceToTarget = CurrentDistance;
						}
					}
					if (destroyed) continue;

					//Check enemy Resource buildings
					for (int e = 0; e < enemy.ResourceBuildings.Count; ++e)
					{
						currentEnemyEntity = enemy.ResourceBuildings[e];
						CurrentDistance = Vector2.Distance(currentUnit.Position, currentEnemyEntity.Position);

						if (CurrentDistance < DistanceForDestroying)
						{
							Units.Remove(currentUnit);
							RecycledEntities.Add(currentUnit);
							currentEnemyEntity.Data -= 1;

							if (currentEnemyEntity.Data == 0)
							{
								enemy.ResourceBuildings.Remove(currentEnemyEntity);
								RecycledEntities.Add(currentEnemyEntity);
								Game1.instance.Explosion.Play();
							}
							else
							{
								Game1.instance.Crash.Play();
							}
							destroyed = true;
							break;
						}

						if (CurrentDistance < DistanceToTarget)
						{
							target = currentEnemyEntity;
							DistanceToTarget = CurrentDistance;
						}
					}
					if (destroyed) continue;
				}
				Vector2 DistanceVector = target.Position - currentUnit.Position;
				
				Units[i].Data = (float)Math.Atan2((DistanceVector.Y), (DistanceVector.X)) + MathHelper.ToRadians(90);

				DistanceVector.X = Math.Abs(DistanceVector.X);
				DistanceVector.Y = Math.Abs(DistanceVector.Y);

				float total = DistanceVector.X + DistanceVector.Y;

				float ChangeX = 1;
				float ChangeY = 1;

				if (Math.Abs(DistanceVector.X) > Math.Abs(DistanceVector.Y))
				{
					ChangeY = 1 * (DistanceVector.Y / total);
				}
				else
				{
					ChangeX = 1 * ( DistanceVector.X / total);
				}

				//Console.WriteLine(ChangeX + " " + ChangeY);

				if (DistanceVector.X > 0)
				{
					if (currentUnit.Position.X > target.Position.X) currentUnit.Position.X -= ChangeX;
					else currentUnit.Position.X += ChangeX;
				}
				if (DistanceVector.Y > 0)
				{
					if (currentUnit.Position.Y > target.Position.Y) currentUnit.Position.Y -= ChangeY;
					else currentUnit.Position.Y += ChangeY;
				}

			}
		}
	}

	class entity
	{
		internal Vector2 Position;
		internal float Data;

		internal entity(Vector3 a)
		{
			Position = new Vector2(a.X, a.Y);
			Data = a.Z;
		}
		internal entity(Vector2 a, float b)
		{
			Position = a;
			Data = b;
		}
		internal entity(float a, float b, float c)
		{
			Position = new Vector2(a, b);
			Data = c;
		}
	}
}
