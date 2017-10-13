﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Tester : Entity {
	// Editor configuration
	//public float speed = 1;
	public float movementDistance = 1f;
	public float height = 0.25f;

	public GameObject StartingPoint;
	public GameObject EndingPoint;
	public float minObjectiveDistance = 1.2f;

	public bool training = true;
    public int delayTime = 20;

	// Local
	List<Node> open;
	Node _targetNode;

	// 	Temporal/Control
	private int _id = 1;
	private int stage;
	Direction movementDirection;
    float finishNodeDistance = 0f;
    int delay;
    bool OpenChildStage = true;
    Direction[] recordedDistance;
    bool drasticDistanceChange = false;

	// Controller
	PlayerController controller;
	List<Camera> cameras;
	Direction[] CanMove;

#region Unity
    void Start () {
		controller = GetComponent<PlayerController>();
		Init();        
	}
	
	void Update () {
        finishNodeDistance = Vector3.Distance(gameObject.transform.position,
                _targetNode.Waypoint.transform.position);                

        if (delay >= delayTime)
        {
            if (training)
            {
                Node tempNode = Visited[Visited.Count - 1];
                /// Stages
                /// 1. Scan
                /// 2. SetMovement
                /// 3. Move
                ///  Repeat
                /// 4. Open nodes
                /// 5. Visit nodes
                /// 6. Stop trainer
                /// 7. Instantiate player
                /// 8. End
                switch (stage)
                {
                    case 1:
                        CanMove = ScanFromPosition(tempNode);
                        Visited[Visited.Count - 1].Directions = CanMove;
                        movementDirection = null;
                        stage = 2;
                        break;

                    case 2:
                        bool _outsideLimits = false;
                        int index = 0;
                        for (index = 0; index < CanMove.Length; index++)
                        {
                            try
                            {
                                if (CanMove[index].Value == true)
                                {
                                    movementDirection = CanMove[index];
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                Debug.LogError("Outside the limits...?");
                                movementDirection = null;
                                _outsideLimits = true;
                                break;
                            }
                        }

                        if (movementDirection != null)
                        {
                            // "Forward movement"
                            Camera _tempCam = GetCamera(movementDirection);
                            float _collisionDistance = GetDistance(_tempCam);

                            // Collateral changes
                            //drasticDistanceChange = FindNewCollateralWay(movementDirection, CanMove);

                            if (_collisionDistance > 0.51f)
                            {
                                SelfMovement(movementDirection);
                                OpenChildStage = true;
                            }
                            else
                            {
                                stage = 3;
                                CanMove[index].Value = false;
                            }
                        }
                        else
                        {
                            if (_outsideLimits == true)
                                stage = 6;
                            else
                                stage = 5;
                        }
                        break;

                    case 3:
                        bool nearbyNodes = ScanForNearbyNodes(
                                new Vector3(transform.position.x, 0, transform.position.z),
                                0.51f);

                        if (finishNodeDistance > minObjectiveDistance / 2)
                        {
                            if (OpenChildStage == true)
                            {
                                if (nearbyNodes == false)
                                {
                                    OpenChild(Visited[Visited.Count - 1], movementDirection);
                                    MoveGameObjectTo(Visited[Visited.Count - 1].Waypoint.transform.position, height);
                                    drasticDistanceChange = false;
                                }

                                stage = 4;
                                delay = 0;
                                OpenChildStage = false;
                            }
                        }
                        else
                        {                            
                            stage = 6;
                            delay = 0;
                        }
                        break;

                    case 4:
                        for (int j = 0; j < Visited[Visited.Count - 1].Directions.Length; j++)
                        {
                            if (Visited[Visited.Count - 1].Directions[j].Value == true)
                            {
                                stage = 2;
                                break;
                            }
                            else
                                stage = 5;
                        }
                        OpenChildStage = false;
                        delay = 0;
                        break;

                    case 5:
                        Debug.Log("Visit new node");
                        OpenChildStage = true;
                        if (open.Count > 0)
                        {
                            VisitNewNode();
                            stage = 1;
                        }
                        //else
                        //    stage = 6;
                        delay = 0;
                        CanMove = null;
                        movementDirection = null;
                        break;

                    case 6:
                        _targetNode.Parent = Visited[Visited.Count - 1];
                        Visited.Add(_targetNode);
                        MoveGameObjectTo(EndingPoint.transform.position, height);
                        Debug.Log("Numero de nodos: " + Visited.Count.ToString());
                        Debug.Log("Finito");
                        //training = false;
                        stage = 7;
                        SetRoute();
                        Debug.Log("Ruta: " + Route.Count.ToString());                        
                        break;

                    case 7:
                        InstantiatePlayer();
                        stage = 8;
                        //training = false;
                        break;

                    case 8:
                        gameObject.SetActive(false);
                        training = false;
                        break;
                }
            }
        }
        else
            delay++;
	}    
    #endregion

    #region Init functions
    private void Init()
	{
		stage = 1;
		Visited = new List<Node>();
		open = new List<Node>();

		Visited.Add(new Node(_id, StartingPoint));
		UpdateNewNodeId();
		_targetNode = new Node(_id, EndingPoint);
		UpdateNewNodeId();
		
		MoveGameObjectTo(Visited[0].Waypoint.transform.position, height);
		GetCameras();
		InitDirections();
        delay = delayTime;
    }

	private void InitDirections()
    {
        Node _tempNode = new Node(new GameObject("temp"));
        CanMove = _tempNode.Directions;
        _tempNode.Waypoint.SetActive(false);
        _tempNode = null;
    }
#endregion

#region Stage functions
    private Direction[] ScanFromPosition(Node _node)
	{
        Direction[] _directions = _node.Directions;
		foreach (Camera cam in cameras)
		{
			float _distance = GetDistance(cam);
			if(_distance > minObjectiveDistance && _distance != 0)
			{
				switch(cam.name)
				{
					case "FrontCamera":
                        _directions[0].Value = SetMovementDirection(_node.DirectionRestriction_Index, 0);
                        break;

					case "RightCamera":
                        _directions[1].Value = SetMovementDirection(_node.DirectionRestriction_Index, 1);
                        break;

					case "LeftCamera":
                        _directions[2].Value = SetMovementDirection(_node.DirectionRestriction_Index, 2);
                        break;

					case "BackCamera":
                        _directions[3].Value = SetMovementDirection(_node.DirectionRestriction_Index, 3);
                        break;
				}
			}
		}
        return _directions;
	}

    private GameObject CreateWaypoint()
    {
        Debug.Log("Set a waypoint");
        GameObject gobj = new GameObject
        {
            name = "Waypoint",
            tag = "Waypoint"
        };
        gobj.transform.position = gameObject.transform.position + Vector3.down * gameObject.transform.position.y;
        StopMovement(gobj);
        gameObject.transform.position = gobj.transform.position;
        return gobj;
    }

    private void OpenChild(Node _parent, Direction _direction)
    {
        Node _child = new Node(_id, CreateWaypoint(), _parent, _direction);
        UpdateNewNodeId();
        open.Add(_child);

        _parent.UpdateNodeDirectionRestriction(_direction);
        Debug.Log("Child open");
    }

    private void VisitNewNode()
    {
        Visited.Add(open[0]);
        open.RemoveAt(0);
        MoveGameObjectTo(Visited[Visited.Count - 1].Waypoint.transform.position, height);
    }
#endregion

#region Movement functions
    void MoveGameObjectTo(Vector3 position, float _height)
	{
		gameObject.transform.position = position + Vector3.up * _height;
	}
    
	private void SelfMovement(Direction _direction)
	{
		float x = 0, z = 0;
		if(_direction.Name == "Forwards" && _direction.Value == true)
			x = movementDistance;
		else if(_direction.Name == "Left" && _direction.Value == true)
			z = movementDistance;
		else if(_direction.Name == "Right" && _direction.Value == true)
			z = -movementDistance;
		else if(_direction.Name == "Backwards" && _direction.Value == true)
			x = -movementDistance;

		Vector3 moveInput = new Vector3(x, 0f, z);
        //gameObject.transform.position += moveInput * speed;
        gameObject.transform.position += moveInput;
    }

	private void StopMovement(GameObject _gameObject)
	{
		controller.Move(_gameObject.transform.position * 0);
	}
#endregion

#region Helper functions
	private void UpdateNewNodeId()
    {
        do { _id++; } while (Visited.Find(p => p.ID == _id) != null || 
            open.Find(p => p.ID == _id) != null);
    }

	private float GetDistance(Camera _camera)
	{
		RaycastHit hit;
		Vector3 direction = new Vector3();
		switch(_camera.name)
		{
			case "FrontCamera":
				direction = Vector3.right;
				break;

			case "RightCamera":
				direction = Vector3.back;
				break;

			case "LeftCamera":
				direction = Vector3.forward;
				break;

			case "BackCamera":
				direction = Vector3.left;
				break;
		}
		Ray fwdRay = new Ray(_camera.transform.position, direction);
		Debug.DrawRay(_camera.transform.position, direction, Color.red);
		if(Physics.Raycast(fwdRay, out hit))
		{
            /// Show camera distance in Log
			//Debug.Log(_camera.name.ToString() + ": " + hit.distance.ToString());
		}
		return hit.distance;
	}

	private void GetCameras()
	{
		cameras = new List<Camera>();
		var cam = gameObject.GetComponentsInChildren<Camera>();
		foreach (Camera item in cam)
		{
			cameras.Add(item);
		}
	}

	private Camera GetCamera(Direction _direction)
	{
		string cameraName = "";
		switch(_direction.Name)
		{
			case "Forwards":
				cameraName += "FrontCamera";
				break;

			case "Right":
				cameraName += "RightCamera";
				break;

			case "Left":
				cameraName += "LeftCamera";
				break;

			case "Backwards":
				cameraName += "BackCamera";
				break;
		}
		return cameras.Find(p => p.name == cameraName);
	}

    private int GetWaysCount(Direction[] _directions)
    {
        int j = 0;
        for (int i = 0; i < _directions.Length; i++)
        {
            if (_directions[i].Value == true)
                j++;
        }
        return j;
    }

    private bool FindNewCollateralWay(Direction movementDirection, Direction[] canMove)
    {

        return false;
    }

    /// <summary> Scan for nearby waypoints</summary>
    /// <param name="_currentPosition">Current gameObject position</param>
    /// <param name="_minDistance"></param>
    /// <returns>True if it finds a nearby waypoint</returns>
    private bool ScanForNearbyNodes(Vector3 _currentPosition, float _minDistance)
    {
        GameObject[] _waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject wp in _waypoints)
        {
            if(Vector3.Distance(wp.transform.position, _currentPosition) <= _minDistance)
            {
                _waypoints = null;
                return true;
            }
        }
        return false;
    }

    private bool SetMovementDirection(int _restriction, int _direction)
    {
        if (_restriction == _direction)
            return false;
        else return true;
    }
    #endregion
}
