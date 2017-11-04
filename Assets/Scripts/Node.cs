using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
	public int ID { get; set; }
    public Node Parent { get; set; }
    public GameObject Waypoint;
    public Direction[] Directions;
    private int _directionRestrictionIndex = -1;
    /// <summary>
    /// Returns the int value of the node movement restriction.
    /// </summary>
    public int DirectionRestriction_Index
    {
        get { return _directionRestrictionIndex; }
    }

	public Node() {  }
    public Node(GameObject waypoint)
    {
        this.Waypoint = waypoint;
        initDirections();
    }
    public Node(int id, GameObject waypoint)
    {
        this.ID = id;
        this.Waypoint = waypoint;
        initDirections();
    }
    public Node(int id, GameObject waypoint, Node parent)
    {        
        this.ID = id;
        this.Waypoint = waypoint;
        this.Parent = parent;
        initDirections();
    }

    public Node(int id, GameObject waypoint, Node parent, Direction restriction)
    {        
        this.ID = id;
        this.Waypoint = waypoint;
        this.Parent = parent;
        initDirections();
        for(int i = 0; i < this.Directions.Length; i++)
        {            
            if(this.Directions[i].Name == restriction.Name)
            {
                Direction _oppositeDirection = GetOppositeDirection(this.Directions[i]);
                for(int j = 0; j < this.Directions.Length; j++)
                {
                    if(this.Directions[j].Name == _oppositeDirection.Name)
                    {
                        this.Directions[j].Value = _oppositeDirection.Value;
                        this._directionRestrictionIndex = j;
                        break;
                    }
                }                
            }
            if(this._directionRestrictionIndex != -1)
                break;
        }
    }

    public void UpdateNodeDirectionRestriction(Direction _direction, bool value = false)
    {
        for(int i = 0; i < this.Directions.Length; i++)
        {
            if (this.Directions[i].Name == _direction.Name)
                Directions[i].Value = value;
        }
    }

    /// <summary>
    /// <para>Check if the selected node is on a node list.</para>
    /// <param name="node">Node match</param>
    /// </summary>	
    /// <return>Bool. True in case the node is on the list.</return>
    private bool CheckNodeOnList(List<Node> nodes, Node node)
    {
        if (nodes.Find(p => p == node) == null)
            return false;
        else return true;
    }

    /// <summary>
    /// <para>Check if the selected node is near a node on a list.</para>
    /// <param name="node">Node match</param>
    /// </summary>	
    /// <return>Bool. True in case the node is on the list is near the new node.</return>
    private bool CheckNodeOnList(List<Node> nodes, Vector3 _waypointPosition)
    {
        // Terminar
        if (nodes.Find(p => p.Waypoint.transform.position == _waypointPosition) == null)
            return false;
        else return true;
    }

    private Direction GetOppositeDirection(Direction _direction)
    {
        Direction returnValue;
        switch(_direction.Name)
        {
            case "Forwards":
                returnValue =  new Direction("Backwards");
                break;

            case "Right":
                returnValue =  new Direction("Left");
                break;

            case "Left":
                returnValue =  new Direction("Right");
                break;

            case "Backwards":
                returnValue =  new Direction("Forwards");
                break;

            default:
                returnValue = null;
                break;
        }
        return returnValue;
    }

    private void initDirections()
    {
        List<Direction> dir = new List<Direction>();
        dir.Add(new Direction("Forwards"));
        dir.Add(new Direction("Right"));
        dir.Add(new Direction("Left"));
        dir.Add(new Direction("Backwards"));

        Directions = dir.ToArray();
    }
}

public class Direction
{
    public string Name { get; set; }
    public bool Value { get; set; }
    public float Distance { get; set; }

    public Direction() { this.Distance = -1f; }

    public Direction(string _name, bool _value = false)
    {
        this.Name = _name;
        this.Value = _value;
        this.Distance = -1f;
    }

    public Direction(string _name, float _distance)
    {
        this.Name = _name;
        this.Distance = _distance;
    }
}
