using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
	public Transform tilePrefab;
	public Vector2 mapSize;
	public bool updateMap = false;
	[Range(0,0.1f)]
	public float outlinePercent;
	public List<Coord> allTileCoords;
	void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		if(updateMap)
		{
			allTileCoords = new List<Coord>();
			for(int x = 0; x < mapSize.x; x++)
			{
				for(int y = 0; y < mapSize.y; y++)
				{
					allTileCoords.Add(new Coord(x,y));
				}
			}

			string holderName = "Generated Map";
			if(transform.Find(holderName))
			{
				// Se llama a Destroy Immediate en vez de Destroy ya que 
				// se llama desde el editor.
				DestroyImmediate(transform.Find(holderName).gameObject);
			}

			Transform mapHolder = new GameObject(holderName).transform;
			mapHolder.parent = transform;

			for(int x = 0; x < mapSize.x; x++)
			{
				for(int y = 0; y < mapSize.y; y++)
				{
					// Calcular la posición en la que aparecerá el recuadro
					Vector3 tilePosition = new Vector3(-mapSize.x/2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
					Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
					newTile.localScale = Vector3.one * (1 - outlinePercent);
					newTile.parent = mapHolder;
				}
			}
		}
	}

	public struct Coord
	{
		public int x;
		public int y;
		public Coord(int _x, int _y)
		{
			x = _x;
			y = _y;
		}
	}
}
