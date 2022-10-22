using UnityEngine;

public class MoveTileSpawner : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private float _speed;

    [SerializeField] [Range(0, 600)] private float _distanceToPlayer = 120f;

    [SerializeField] private Transform _player;

    [SerializeField] private Vector3 _mapCenter;
    [SerializeField] private Vector3 _playerPosition;
    [SerializeField] private Vector3 _spawnPoint;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _mapCenter.x = _player.transform.position.x;
        _mapCenter.y = _player.transform.position.y;
    }

    void LateUpdate()
    {
        _playerPosition = _player.transform.position;
        _spawnPoint = new Vector3(_mapCenter.x, _mapCenter.y, _playerPosition.z + _distanceToPlayer);
        gameObject.transform.position = _spawnPoint;
    }
    
}
