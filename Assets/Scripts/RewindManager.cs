using System;
using System.Collections.Generic;
using UnityEngine;
using SuperuserUtils;

public class RewindManager : GenericSingletonClass<RewindManager>
{
    public List<ObjectPositionData> AListOfThePast;
    public float                    recordTime = 5f;

    public void AddObjectPosition(GameObject gObject, Vector3 position, Quaternion rotation)
    {
        AListOfThePast.Insert(0, new ObjectPositionData(gObject, position, rotation));
    }

    private void Start()
    {
        AListOfThePast = new List<ObjectPositionData>();
    }
}

public class ObjectPositionData
{
    public GameObject GameObject;
    public Vector3    Position;
    public Quaternion Rotation;

    public ObjectPositionData(GameObject gObject, Vector3 position, Quaternion rotation)
    {
        GameObject = gObject;
        Position = position;
        Rotation = rotation;
    }
}
