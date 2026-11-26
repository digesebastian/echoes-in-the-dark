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
    [SerializeReference] public BlackboardVariable<bool> DestroyObjects = new(false);

    protected override Status OnStart()
    {
        if (List == null) return Status.Failure;
        if (List.Value == null)
        {
            List.Value = new List<GameObject>();
            return Status.Success;
        }
        if (DestroyObjects != null && DestroyObjects.Value)
                {
                    for (int i = 0; i < List.Value.Count; i++)
                    {
                        if (List.Value[i] != null)
                            UnityEngine.Object.Destroy(List.Value[i]);
                    }
                }
        List.Value.Clear();
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

