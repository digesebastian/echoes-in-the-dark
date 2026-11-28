using System.Collections.Generic;
using UnityEngine;

public class AttentionNodeManager : MonoBehaviour
{
    public static AttentionNodeManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public List<AttentionNode> GetAvailableNodes(IReadOnlyList<AttentionNode> basicPoints)
        {
            List<AttentionNode> available = new List<AttentionNode>();
            if (basicPoints == null) return available;
    
            for (int i = 0; i < basicPoints.Count; i++)
            {
                AttentionNode node = basicPoints[i];
                if (node != null && !node.isClaimed)
                {
                    available.Add(node);
                }
            }
            return available;
        }

    /// <summary>
    /// Atomically claims target node for a guard and starves the rest.
    /// </summary>
    public bool TryClaimNode(AttentionNode target, IReadOnlyList<AttentionNode> basicPoints)
    {
        if (target == null || target.isClaimed)
        {
            return false;
        }

        target.Claim();

        if (basicPoints != null)
        {
            for (int i = 0; i < basicPoints.Count; i++)
            {
                AttentionNode node = basicPoints[i];
                if (node != null && node != target)
                {
                    node.IncrementStarvation();
                }
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
