using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, Unity.Properties.GeneratePropertyBag]
[NodeDescription(
    name: "Select Attention Node",
    story: "Select attention node for [Agent] and store in [TargetPosition]",
    category: "Action/Navigation",
    id: "f1a2b3c4d5e67890123456789abcdef0")]
public partial class SelectAttentionNodeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<List<GameObject>> basic_points;

    [Tooltip("Exponential falloff penalty for distance (higher = nearer points favored more)")]
    [Range(1f, 4f)]
    public float distanceBias = 0.1f;

    [Tooltip("Weight multiplier added per starvation tick")]
    public float starvationWeightBonus = 2.5f;

    [Tooltip("Minimum distance a node must be from the guard to be considered (prevents camping the same node)")]
    public float minPickDistance = 1.0f;

    protected override Status OnStart()
    {
        if (Agent?.Value == null || basic_points?.Value == null || AttentionNodeManager.Instance == null)
            return Status.Failure;
        
        // extract AttentionNode components from the assigned GameObjects
        List<AttentionNode> assignedNodes = new List<AttentionNode>(basic_points.Value.Count);
        for (int i = 0; i < basic_points.Value.Count; i++)
        {
            GameObject pointObj = basic_points.Value[i];
            if (pointObj == null) continue;

            if (pointObj.TryGetComponent(out AttentionNode node))
            {
                assignedNodes.Add(node);
            }
            else
            {
                Debug.LogWarning($"[{Agent.Value.name}] '{pointObj.name}' in basic_points is missing an AttentionNode component.", pointObj);
            }
        }

        if (assignedNodes.Count == 0)
            return Status.Failure;

        // find the node in this guard's list that is currently claimed (if any)
        AttentionNode previousNode = null;
        for (int i = 0; i < assignedNodes.Count; i++)
        {
            if (assignedNodes[i] != null && assignedNodes[i].isClaimed)
            {
                previousNode = assignedNodes[i];
                break;
            }
        }

        // query unclaimed nodes (previousNode is still claimed so the guard won't pick the same spot twice in a row)
        List<AttentionNode> candidates = AttentionNodeManager.Instance.GetAvailableNodes(assignedNodes);

        // Fallback if this guard only has 1 node assigned in total
        if (candidates.Count == 0 && previousNode != null)
        {
            AttentionNodeManager.Instance.ReleaseNode(previousNode);
            previousNode = null;
            candidates = AttentionNodeManager.Instance.GetAvailableNodes(assignedNodes);
        }

        if (candidates.Count == 0)
            return Status.Failure;

        // score remaining candidates
        AttentionNode bestNode = null;
        float bestScore = float.NegativeInfinity;
        Vector2 guardPos = Agent.Value.transform.position;

        for (int i = 0; i < candidates.Count; i++)
        {
            AttentionNode candidate = candidates[i];
            float distance = Vector2.Distance(guardPos, candidate.Position);
            float score = candidate.basePriority
                        + (candidate.starvationCounter * starvationWeightBonus)
                        - (distance * distanceBias);

            if (score > bestScore)
            {
                bestScore = score;
                bestNode = candidate;
            }
        }

        // 4. Claim the new node, then release the previous one
        if (bestNode != null && AttentionNodeManager.Instance.TryClaimNode(bestNode, assignedNodes))
        {
            if (previousNode != null)
                AttentionNodeManager.Instance.ReleaseNode(previousNode);

            TargetPosition.Value = bestNode.transform.position;
            return Status.Success;
        }

        return Status.Failure;
    }
}