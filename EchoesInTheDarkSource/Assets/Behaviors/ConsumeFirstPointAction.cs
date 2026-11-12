using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Consume First Point", story: "Remove first element from [list]", category: "Action", id: "5db32223141ec5142f9220b1ab9ff88f")]
public partial class ConsumeFirstPointAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;

    protected override Status OnStart()
    {
        if (List.Value != null && List.Value.Count > 0)
        {
            GameObject pointToDestroy = List.Value[0];
            List.Value.RemoveAt(0);
            if (pointToDestroy != null) UnityEngine.Object.Destroy(pointToDestroy);
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

