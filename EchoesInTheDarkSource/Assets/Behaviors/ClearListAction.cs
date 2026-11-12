using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Clear list", story: "Clear [list]", category: "Action", id: "4e97ae476b6cfa3ab3b29fd0d1609346")]
public partial class ClearListAction : Action
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> List;

    protected override Status OnStart()
    {
        List.Value.Clear();
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

