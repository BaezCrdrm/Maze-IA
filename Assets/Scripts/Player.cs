using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    // Editor configuration
    public int speed = 5;
    public List<Node> Visit;
    public float minNodeDistance = 0.7f;

    // Control
    private int stage;

    // Controller
    PlayerController controller;

    void Start () {
        controller = GetComponent<PlayerController>();
        stage = 1;
	}

	void FixedUpdate () {
		if(Visit != null)
        {
            switch(stage)
            {
                case 1:
                    gameObject.transform.position = Visit[0].Waypoint.transform.position + Vector3.up * gameObject.transform.localScale.y / 2;
                    Visit.RemoveAt(0);
                    stage = 2;
                    break;

                case 2:
                    if (Visit.Count > 0)
                    {
                        if (CheckDistanceToTarget(gameObject.transform.position, Visit[0].Waypoint.transform.position))
                        {
                            // El problema está en que como el vector que se da es positivo, el gameObject se mueve
                            // de manera positiva en los ejes X & Z, y no en dirección hacia el waypoint
                            // donde uno de sus vectores es en base a la posición actual del elemento.

                            // Posible solución: Replantear la función de movimiento del controlador, de
                            // manera que no se mueva con los valores del vector, si no hacia la dirección
                            // del siguiente waypoint

                            // Posible solución 2: Modificar la dirección a la que se va a mover.
                            // Calcular un nuevo vector en base a la posición del gameobject/waypoint actual
                            // con la del objetivo, de manera que programaticamente se decidan
                            // los valores del nuevo vector.
                            controller.Move(getGameObjectVector(
                                    GetMovingVector(gameObject.transform.position, (Visit[0].Waypoint.transform.position * speed))));
                        }
                        else
                            stage = 3;
                    }
                    else
                        stage = 4;
                    break;

                case 3:
                    controller.Move(gameObject.transform.position * 0);
                    gameObject.transform.position = getGameObjectVector(Visit[0].Waypoint.transform.position);

                    Visit.RemoveAt(0);
                    stage = 2;
                    break;

                case 4:
                    controller.Move(gameObject.transform.position * 0);

                    Debug.Log("Stop");
                    break;
            }
        }
	}

    public void SetData(List<Node> _route)
    {
        this.Visit = _route;
    }

    /// <summary>
    /// <para>Check if the current distance between the given position and the
    /// final waypoint position is longer than the parameter given for the developer.</para>
    /// </summary>
    /// <param name="_currentPosition">Current position of any gameObject</param>
    /// <param name="_targetPosition">Position of the targeted waypoint</param>
    /// <returns>True if the distance is larger than the control distance. 
    /// False if the distance is shorter than the control distance</returns>
    private bool CheckDistanceToTarget(Vector3 _currentPosition, Vector3 _targetPosition)
    {
        if (Vector3.Distance(_currentPosition, _targetPosition) > minNodeDistance)
            return true;
        else return false;

        Debug.Log("Distance to obj: " + Vector3.Distance(_currentPosition, _targetPosition).ToString());
    }

    private Vector3 getGameObjectVector(Vector3 _v3)
    {
        return _v3 + Vector3.up* gameObject.transform.localScale.y / 2;
    }

    private Vector3 GetMovingVector(Vector3 _currentPosition, Vector3 _targetPosition)
    {
        Vector3 tempVector3;
        tempVector3 = _targetPosition - _currentPosition;
        return tempVector3;
    }
}
