using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Rendering.Universal;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SetSpriteColor", story: "Set [Self] sprite [Color]", category: "Action", id: "6900748c24f45542fa58330c2c1466e4")]
public partial class SetSpriteColorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Color> Color;

    protected override Status OnStart()
    {
        Debug.Log($"[{Self.Value.name}] SetSpriteColor -> {Color.Value}");
        
        SpriteRenderer spriteRenderer = Self.Value.GetComponent<SpriteRenderer>();
        Light2D light2D = Self.Value.GetComponentInChildren<Light2D>();
        if (!spriteRenderer || !light2D)  return Status.Failure;
        Color targetColor = Color.Value;
        targetColor.a = 1f;
        spriteRenderer.color = targetColor;
        light2D.color = targetColor;
        return Status.Success;
    }
}

