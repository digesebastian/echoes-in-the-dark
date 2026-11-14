using System.Collections.Generic;
using UnityEngine;

public class AttentionNodeManager : MonoBehaviour
{
    public static AttentionNodeManager Instance { get; private set; }

    [SerializeField] private List<AttentionNode> allNodes = new List<AttentionNode>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Automatically discover all AttentionNode components in the scene if not populated in Inspector
        if (allNodes.Count == 0)
        {
            allNodes.AddRange(FindObjectsByType<AttentionNode>(FindObjectsSortMode.None));
        }
    }

    /// <summary>
    /// Returns all nodes that are currently not claimed by any guard.
    /// </summary>
    public List<AttentionNode> GetUnclaimedNodes()
    {
        List<AttentionNode> available = new List<AttentionNode>();
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i] != null && !allNodes[i].isClaimed)
            {
                available.Add(allNodes[i]);
            }
        }
        return available;
    }

    /// <summary>
    /// Atomically claims target node for a guard and starves the rest.
    /// </summary>
    public bool TryClaimNode(AttentionNode target)
    {
        if (target == null || target.isClaimed)
        {
            return false;
        }

        target.Claim();

        // Increment starvation counter for all other unclaimed nodes
        for (int i = 0; i < allNodes.Count; i++)
        {
            if (allNodes[i] != null && allNodes[i] != target)
            {
                allNodes[i].IncrementStarvation();
            }
        }

        return true;
    }

    /// <summary>
    /// Releases a claimed node lock so other guards can target it again.
    /// </summary>
    public void ReleaseNode(AttentionNode node)
    {
        if (node != null)
        {
            node.Release();
        }
    }
}
