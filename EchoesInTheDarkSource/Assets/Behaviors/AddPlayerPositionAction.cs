using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Add Player Position", story: "clear [Points] add [Player] position to the list", category: "Action", id: "60b4c745191daf964f983489587bed5c")]
public partial class AddPlayerPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Points;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    protected override Status OnStart()
    {
        GameObject newPoint = new GameObject("PlayerPosition");
        newPoint.transform.position = Player.Value.transform.position;
        Points.Value.Clear();
        Points.Value.Add(newPoint);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

