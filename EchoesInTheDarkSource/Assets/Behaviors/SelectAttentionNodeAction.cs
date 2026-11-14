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

    [Tooltip("Exponential falloff penalty for distance (higher = nearer points favored more)")]
    [Range(1f, 4f)]
    public float distanceBias = 2.0f;

    [Tooltip("Weight multiplier added per starvation tick")]
    public float starvationWeightBonus = 0.5f;

    [Tooltip("Minimum distance a node must be from the guard to be considered (prevents camping the same node)")]
    public float minPickDistance = 1.0f;

    private AttentionNode _currentClaim;

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null || AttentionNodeManager.Instance == null)
            return Status.Failure;

        // Remember what we were holding so we don't immediately pick it right back
        AttentionNode lastVisitedNode = _currentClaim;

        // 1. Release the old node lock
        if (_currentClaim != null)
        {
            AttentionNodeManager.Instance.ReleaseNode(_currentClaim);
            _currentClaim = null;
        }

        // 2. Fetch globally unclaimed nodes
        List<AttentionNode> allAvailable = AttentionNodeManager.Instance.GetUnclaimedNodes();
        if (allAvailable == null || allAvailable.Count == 0)
            return Status.Failure;

        Vector2 guardPos = Agent.Value.transform.position;

        // 3. Filter out the current node (and any node too close)
        List<AttentionNode> validCandidates = new List<AttentionNode>();
        for (int i = 0; i < allAvailable.Count; i++)
        {
            AttentionNode node = allAvailable[i];

            // Never re-pick the node we just arrived at if other options exist
            if (node == lastVisitedNode && allAvailable.Count > 1)
                continue;

            // Optional: Ignore nodes within 1 meter if there are other nodes to explore
            float dist = Vector2.Distance(guardPos, node.Position);
            if (dist < minPickDistance && allAvailable.Count > 1)
                continue;

            validCandidates.Add(node);
        }

        // Fallback: If only 1 node exists in the whole map, allow picking it
        if (validCandidates.Count == 0)
        {
            validCandidates = allAvailable;
        }

        // 4. Calculate weighted probability on valid candidates only
        List<float> weights = new List<float>(validCandidates.Count);
        float totalWeight = 0f;

        for (int i = 0; i < validCandidates.Count; i++)
        {
            AttentionNode node = validCandidates[i];
            float dist = Vector2.Distance(guardPos, node.Position);

            // Proximity weight (safe from 0-distance explosion)
            float proximity = 1f / Mathf.Pow(Mathf.Max(dist, 0.5f), distanceBias);
            float starvationBonus = 1f + (node.starvationCounter * starvationWeightBonus);

            float weight = proximity * node.basePriority * starvationBonus;
            weights.Add(weight);
            totalWeight += weight;
        }

        // 5. Roulette-wheel selection
        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float runningSum = 0f;
        AttentionNode chosen = validCandidates[0];

        for (int i = 0; i < validCandidates.Count; i++)
        {
            runningSum += weights[i];
            if (roll <= runningSum)
            {
                chosen = validCandidates[i];
                break;
            }
        }

        // 6. Lock the winning node
        if (AttentionNodeManager.Instance.TryClaimNode(chosen))
        {
            _currentClaim = chosen;
            TargetPosition.Value = new Vector3(chosen.Position.x, chosen.Position.y, Agent.Value.transform.position.z);
            return Status.Success;
        }

        return Status.Failure;
    }

    protected override void OnEnd()
    {
        if (CurrentStatus == Status.Interrupted || CurrentStatus == Status.Failure)
        {
            if (_currentClaim != null && AttentionNodeManager.Instance != null)
            {
                AttentionNodeManager.Instance.ReleaseNode(_currentClaim);
                _currentClaim = null;
            }
        }
    }
}