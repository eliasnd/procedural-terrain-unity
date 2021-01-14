/*
Credit:
Hans Theobald Beyer
Implementation of a Method for Hydraulic Erosion
https://www.firespark.de/resources/downloads/implementation%20of%20a%20methode%20for%20hydraulic%20erosion.pdf
*/

using UnityEngine;

public static class BeyerErosion
{
	public static HeightMap Erode(HeightMap map, int erosions, 
		float inertia = 0.3f, float gravity = -9.81f, 
		float minSlope = 0.01f, float capacity = 8, 
		float maxSteps = 500, float evaporation = 0.02f, 
		float erosion = 0.7f, float deposition = 0.2f, 
		int radius = 2, float minSedimentCapacity = 0.01f,
		float smoothFactor = 2)
	{
		HeightMap simulator = map.Clone();
		HeightMap changes = new HeightMap(map.size);

		for (int d = 0; d < erosions; d++)
		{
			Vector2 pos = new Vector2(Random.Range(0, map.size), Random.Range(0, map.size));
			Vector2 dir = grad(map, pos.x, pos.y);
			float vel = 1;
			float water = 1;
			float sediment = 0;

			for (int s = 0; s < maxSteps; s++)
			{
				int x = Mathf.FloorToInt(pos.x);
				int y = Mathf.FloorToInt(pos.y);
				float u = pos.x % 1.0f;
				float v = pos.y % 1.0f;

				float currHeight = fracHeight(simulator, pos.x, pos.y);
				Vector2 gradient = grad(simulator, pos.x, pos.y);

				dir = new Vector2(dir.x * inertia - gradient.x * (1-inertia), dir.y * inertia - gradient.y * (1-inertia));

				if (dir.x == 0 && dir.y == 0)
					dir = new Vector2(Random.Range(0, 1), Random.Range(0, 1));

				dir.Normalize();

				Vector2 newPos = pos + dir;

				//Debug.Log("currently at " + pos.x + ", " + pos.y + ", height " + currHeight);
				//Debug.Log("Direction is " + dir.x + ", " + dir.y);
				//Debug.Log("new pos is " + newPos.x + ", " + newPos.y);

				if (simulator.OutOfBounds(Mathf.FloorToInt(newPos.x), Mathf.FloorToInt(newPos.y)) || simulator.OutOfBounds(Mathf.CeilToInt(newPos.x), Mathf.CeilToInt(newPos.y)))
					break;

				float diff = fracHeight(simulator, newPos.x, newPos.y) - fracHeight(simulator, pos.x, pos.y);

				float sedimentCapacity = Mathf.Max(-diff * vel * water * capacity, minSedimentCapacity);

				//Debug.Log("Diff " + diff + ", capacity " + sedimentCapacity);

				if (diff > 0 || sediment > sedimentCapacity)
				{
					float amount = diff > 0 ? Mathf.Min(sediment, diff) : (sediment - sedimentCapacity) * deposition;

					//Debug.Log("amount " + amount);
					//Debug.Log("diff " + diff + ", sediment " + sediment + ", capacity " + sedimentCapacity);

                    changes[x, y] += amount * (1-u) * (1-v);
                    simulator[x, y] += amount * (1-u) * (1-v);
					// simulator.Set(x, y, simulator.Get(x, y) + amount * (1-u) * (1-v));
					// changes.Set(x, y, changes.Get(x, y) + amount * (1-u) * (1-v));

					if (x+1 < map.size)
					{
                        changes[x+1, y] += amount * u * (1-v);
                        simulator[x+1, y] += amount * u * (1-v);
						//simulator.Set(x+1, y, simulator.Get(x+1, y) + amount * u * (1-v));
						// changes.Set(x+1, y, changes.Get(x+1, y) + amount * u * (1-v));
					}
					if (y+1 < map.size)
					{
                        changes[x, y+1] += amount * (1-u) * v;
                        simulator[x, y+1] += amount * (1-u) * v;
						// changes.Set(x, y+1, changes.Get(x, y+1) + amount * (1-u) * v);
						// simulator.Set(x, y+1, simulator.Get(x, y+1) + amount * (1-u) * v);

						if (x+1 < map.size)
						{
                            changes[x+1, y+1] += amount * u * v;
                            simulator[x+1, y+1] += amount * u * v;
							// changes.Set(x+1, y+1, changes.Get(x+1, y+1) + amount * u * v);
							// simulator.Set(x+1, y+1, simulator.Get(x+1, y+1) + amount * u * v);
						}
					}

					sediment -= amount;
				}
				else
				{
					float amount = Mathf.Min((sedimentCapacity - sediment) * erosion, -diff);

                    changes[x, y] -= amount * (1-u) * (1-v);
                    simulator[x, y] -= amount * (1-u) * (1-v);
					// simulator.Set(x, y, simulator.Get(x, y) - amount * (1-u) * (1-v));
					// changes.Set(x, y, changes.Get(x, y) - amount * (1-u) * (1-v));

					if (x+1 < map.size)
					{
                        changes[x+1, y] -= amount * u * (1-v);
                        simulator[x+1, y] -= amount * u * (1-v);
						// simulator.Set(x+1, y, simulator.Get(x+1, y) - amount * u * (1-v));
						// changes.Set(x+1, y, changes.Get(x+1, y) - amount * u * (1-v));
					}
					if (y+1 < map.size)
					{
                        changes[x, y+1] -= amount * (1-u) * v;
						// changes.Set(x, y+1, changes.Get(x, y+1) - amount * (1-u) * v);
						simulator[x, y+1] -= amount * (1-u) * v;
                        //simulator.Set(x, y+1, simulator.Get(x, y+1) - amount * (1-u) * v);

						if (x+1 < map.size)
						{
                            changes[x+1, y+1] -= amount * u * v;
							//changes.Set(x+1, y+1, changes.Get(x+1, y+1) - amount * u * v);
							simulator[x+1, y+1] -= amount * u * v;
                            //simulator.Set(x+1, y+1, simulator.Get(x+1, y+1) - amount * u * v);
						}
					}

					sediment += amount;
				}

				vel = Mathf.Sqrt(Mathf.Max(Mathf.Pow(vel, 2) + diff * gravity, 0));

				if (vel == 0)
					break;

				water *= (1-evaporation);
				pos = newPos;
			}
		}

		for (int y = 0; y < map.size; y++)
			for (int x = 0; x < map.size; x++)
			{
				changes[x, y] /= smoothFactor;
				//Debug.Log(changes.Get(x, y));
			}

		HeightMap eroded = map + changes;

		return simulator;
	}

	static float fracHeight(HeightMap map, float x, float y)
	{
		//Debug.Log(x + ", " + y);
		int unitX = Mathf.Max(0, Mathf.FloorToInt(x));
		int unitY = Mathf.Max(0, Mathf.FloorToInt(y));
		float xOffset = x % 1.0f;
		float yOffset = y % 1.0f;
		int nextX = Mathf.Min(map.size-1, unitX+1);
		int nextY = Mathf.Min(map.size-1, unitY+1);

		return Mathf.Lerp(
						Mathf.Lerp(map[unitX, unitY], map[nextX, unitY], xOffset), 
					  	Mathf.Lerp(map[unitX, nextY], map[nextX, nextY], xOffset),
					  	yOffset
				);
	}

	static Vector2 grad(HeightMap map, float x, float y)
	{
		float xOffset = x % 1.0f;
		float yOffset = y % 1.0f;
		int unitX = Mathf.Max(0, Mathf.FloorToInt(x));
		int unitY = Mathf.Max(0, Mathf.FloorToInt(y));
		int nextX = Mathf.Min(map.size-1, unitX+1);
		int nextY = Mathf.Min(map.size-1, unitY+1);

		float top = map[nextX, unitY] - map[unitX, unitY];
		float bottom = map[nextX, nextY] - map[unitX, nextY];
		float left = map[unitX, nextY] - map[unitX, unitY];
		float right = map[nextX, nextY] - map[nextX, unitY];

		float xGrad = Mathf.Lerp(top, bottom, yOffset);
		float yGrad = Mathf.Lerp(left, right, xOffset);

		return new Vector2(xGrad, yGrad);
	}
}