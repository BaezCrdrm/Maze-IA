using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
	protected List<Node> Visited;
    private List<Node> _route;
    public List<Node> Route
    { get { return this._route;} }
    
    public void SetRoute()
    {
        this._route = new List<Node>();
        Node tempNode = Visited[Visited.Count - 1].Parent;
        while(true)
        {
            try
            {
                if(tempNode != null)
                {
                    this._route.Add(tempNode);
                    tempNode = tempNode.Parent;
                } else
                    break;
            }
            catch(System.Exception)
            { break; }
        }
    }
}
