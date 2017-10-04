using UnityEditor;

[CustomEditor (typeof(ObstacleGen))]
public class ObsEditor : Editor {
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		ObstacleGen obstacle = target as ObstacleGen;
		obstacle.GenerateObstacle();
	}
}
