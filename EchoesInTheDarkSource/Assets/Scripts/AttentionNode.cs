using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AttentionNode : MonoBehaviour
{
    [Header("Node Settings")]
    public int nodeID;
    [Range(1f, 10f)]
    public float basePriority = 1f;

    [Header("Runtime State")]
    public bool isClaimed = false;
    public int starvationCounter = 0;

    public Vector2 Position => transform.position;

    public void Claim()
    {
        isClaimed = true;
        // Reset starvation once a guard selects this node
        starvationCounter = 0;
    }

    public void Release()
    {
        isClaimed = false;
    }

    public void IncrementStarvation()
    {
        if (!isClaimed)
        {
            starvationCounter++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isClaimed ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.35f);

#if UNITY_EDITOR
        Handles.Label(
            transform.position + Vector3.up * 0.45f,
            $"#{nodeID} | S:{starvationCounter} | {(isClaimed ? "LOCKED" : "FREE")}"
        );
#endif
    }
}