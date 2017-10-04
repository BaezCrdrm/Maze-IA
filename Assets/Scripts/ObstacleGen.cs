using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGen : MonoBehaviour {
	public Transform ObstaclePrefab;
	public bool hasObstacle = false;
	private int loop = 0;	

	void Start()
	{
		RemoveObstacle();
		GenerateObstacle();
	}

	public void GenerateObstacle()
	{
		if (hasObstacle && loop == 0)
		{
			Transform newWall = Instantiate(ObstaclePrefab,
				gameObject.transform.position + Vector3.up * 0.5f,
				Quaternion.Euler(Vector3.right*90)) as Transform;

			
			newWall.parent = gameObject.transform;

			loop++;
		} else if(hasObstacle == false && loop > 0)
		{
			RemoveObstacle();
		}
	}

	private void RemoveObstacle()
	{
		var comp = gameObject.GetComponentsInChildren<Transform>();
		for(int i = 0; i < comp.Length; i++)
		{
			if(comp[i].gameObject.tag == "Obstacle")
				DestroyImmediate(comp[i].gameObject);
		}
		loop = 0;
	}
}
