using Player;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Treasure : MonoBehaviour
{
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private Transform[] _exitDoorPositions;
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private TileBase _openDoorTile;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playerState.HasClaimedTreasure = true;
            foreach (Transform exitDoorPos in _exitDoorPositions)
            {
                Vector3Int doorCell = _tileMap.WorldToCell(exitDoorPos.position);
                _tileMap.SetTile(doorCell, _openDoorTile);
            }
            gameObject.SetActive(false);
        }
    }
}
