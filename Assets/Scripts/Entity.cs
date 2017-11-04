using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
    protected List<Node> Visited;
    public Rigidbody PlayerPrefab;
    private List<Node> _route;
    public List<Node> Route
    { get { return this._route; } }
    Rigidbody player;

    public void SetRoute()
    {
        this._route = new List<Node>();
        Node tempNode = Visited[Visited.Count - 1];
        do
        {
            try
            {
                if (tempNode != null)
                {
                    this._route.Add(tempNode);
                    tempNode = tempNode.Parent;
                }
                else
                    break;
            }
            catch (System.Exception)
            { break; }
        } while (true);
        this._route.Reverse();
    }

    protected void InstantiatePlayer()
    {
        player = (Rigidbody)Instantiate(PlayerPrefab, Visited[0].Waypoint.transform.position, Visited[0].Waypoint.transform.rotation);
        player.GetComponent<Player>().SetData(this._route);
    }

    protected void GoThroughSolution()
    {
        player.GetComponent<Player>();
    }
}
