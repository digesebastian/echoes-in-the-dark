using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get First Waypoint", story: "Set [TargetDestination] to first point in [Waypoints]", category: "Action", id: "2c2f6b6c0293cd61e4d498a417de3107")]
public partial class GetFirstWaypointAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetDestination;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;

    protected override Status OnStart()
    {
        if (Waypoints?.Value == null || Waypoints.Value.Count == 0 || Waypoints.Value[0] == null)
                    return Status.Failure;
        
        TargetDestination.Value = Waypoints.Value[0].transform.position;
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

