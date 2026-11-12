using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = Unity.Mathematics.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Generate Investigation Points", story: "Create Investigation Points", category: "Action", id: "ee794e8e0efd52d4fb93792f0a7fa345")]
public partial class GenerateInvestigationPointsAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> HighPoints;
    [SerializeReference] public BlackboardVariable<List<GameObject>> MidPoints;

    private float _searchRadius;
    private int _pointsCount;
    
    protected override Status OnStart()
    {
        if (HighPoints.Value == null || HighPoints.Value.Count == 0 || MidPoints.Value == null) 
            return Status.Failure;

        _searchRadius = 5f;
        _pointsCount = 4;
        
        Vector2 center = HighPoints.Value[0].transform.position;

        // generate random points around the center
        for (int i = 0; i < _pointsCount; i++)
        {
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 spawnPos = center + (randomDir * UnityEngine.Random.Range(1f, _searchRadius));
            
            GameObject newPoint = new GameObject($"MidPoint_{i}");
            newPoint.transform.position = spawnPos;
            MidPoints.Value.Add(newPoint);
        }

        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

