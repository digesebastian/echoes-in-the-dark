using System.Collections.Generic;
using UnityEngine;

public class GridPathfinder2D : MonoBehaviour
{
    public static GridPathfinder2D Instance { get; private set; }

    [Header("Grid Area Settings")]
    [Tooltip("Center of your playable level in world coordinates")]
    public Vector2 gridCenter = Vector2.zero;
    [Tooltip("Total width and height of your playable level in world units")]
    public Vector2 gridWorldSize = new Vector2(40f, 40f);
    [Tooltip("Half-size of each pathfinding cell (0.25 = 0.5x0.5 unit cells)")]
    public float nodeRadius = 0.25f;

    [Header("Obstacle Detection")]
    [Tooltip("Select ONLY the Layer(s) your solid 2D walls/obstacles/locked doors are on")]
    public LayerMask unwalkableMask;
    [Tooltip("Extra clearance buffer around walls (set to at least your guard's collider radius, e.g., 0.3)")]
    public float agentClearance = 0.3f;

    [Header("Debug")]
    public bool drawGridGizmos = true;

    private Node[,] _grid;
    private float _nodeDiameter;
    private int _gridSizeX, _gridSizeY;
    private readonly Collider2D[] _hitBuffer = new Collider2D[1];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _nodeDiameter = nodeRadius * 2f;
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / _nodeDiameter);
        BuildGrid();
    }

    public void BuildGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        Vector2 worldBottomLeft = gridCenter - Vector2.right * (gridWorldSize.x / 2f) - Vector2.up * (gridWorldSize.y / 2f);

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(unwalkableMask);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        float checkRadius = Mathf.Max(0.01f, nodeRadius + agentClearance);

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft
                    + Vector2.right * (x * _nodeDiameter + nodeRadius)
                    + Vector2.up * (y * _nodeDiameter + nodeRadius);

                int hits = Physics2D.OverlapCircle(worldPoint, checkRadius, filter, _hitBuffer);
                bool walkable = (hits == 0);
                _grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public void UpdateBounds(Bounds bounds)
    {
        if (_grid == null) return;

        float pad = nodeRadius + agentClearance;
        Node minNode = NodeFromWorldPoint(new Vector2(bounds.min.x - pad, bounds.min.y - pad));
        Node maxNode = NodeFromWorldPoint(new Vector2(bounds.max.x + pad, bounds.max.y + pad));

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(unwalkableMask);
        filter.useLayerMask = true;
        filter.useTriggers = false;

        float checkRadius = Mathf.Max(0.01f, nodeRadius + agentClearance);

        for (int x = minNode.gridX; x <= maxNode.gridX; x++)
        {
            for (int y = minNode.gridY; y <= maxNode.gridY; y++)
            {
                int hits = Physics2D.OverlapCircle(_grid[x, y].worldPosition, checkRadius, filter, _hitBuffer);
                _grid[x, y].walkable = (hits == 0);
            }
        }
    }

    public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        if (_grid == null) return null;

        Node startNode = GetClosestWalkableNode(NodeFromWorldPoint(startPos));
        Node targetNode = GetClosestWalkableNode(NodeFromWorldPoint(targetPos));

        if (startNode == null || targetNode == null)
            return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                   (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                if (neighbor.gridX != currentNode.gridX && neighbor.gridY != currentNode.gridY)
                {
                    if (!_grid[currentNode.gridX, neighbor.gridY].walkable ||
                        !_grid[neighbor.gridX, currentNode.gridY].walkable)
                    {
                        continue;
                    }
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    private List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            waypoints.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        waypoints.Reverse();
        return waypoints;
    }

    private Node GetClosestWalkableNode(Node origin)
    {
        if (origin.walkable) return origin;

        Node bestNode = null;
        float bestDistSq = float.MaxValue;

        for (int radius = 1; radius <= 8; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int checkX = origin.gridX + x;
                    int checkY = origin.gridY + y;
                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                    {
                        Node candidate = _grid[checkX, checkY];
                        if (candidate.walkable)
                        {
                            float dSq = (candidate.worldPosition - origin.worldPosition).sqrMagnitude;
                            if (dSq < bestDistSq)
                            {
                                bestDistSq = dSq;
                                bestNode = candidate;
                            }
                        }
                    }
                }
            }
            if (bestNode != null) return bestNode;
        }
        return null;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>(8);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbors.Add(_grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = Mathf.Clamp01((worldPosition.x - gridCenter.x + gridWorldSize.x / 2f) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.y - gridCenter.y + gridWorldSize.y / 2f) / gridWorldSize.y);

        int x = Mathf.Clamp(Mathf.FloorToInt(_gridSizeX * percentX), 0, _gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(_gridSizeY * percentY), 0, _gridSizeY - 1);
        return _grid[x, y];
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGridGizmos) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(gridCenter, new Vector3(gridWorldSize.x, gridWorldSize.y, 0.1f));

        if (_grid != null)
        {
            foreach (Node n in _grid)
            {
                Gizmos.color = n.walkable ? new Color(0f, 1f, 0f, 0.15f) : new Color(1f, 0f, 0f, 0.35f);
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (_nodeDiameter - 0.05f));
            }
        }
    }

    public class Node
    {
        public bool walkable;
        public Vector2 worldPosition;
        public int gridX;
        public int gridY;
        public int gCost;
        public int hCost;
        public Node parent;

        public int FCost => gCost + hCost;

        public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
        {
            this.walkable = walkable;
            this.worldPosition = worldPos;
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}