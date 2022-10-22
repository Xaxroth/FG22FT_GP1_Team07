using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [Header("Logistics")]
    [SerializeField] private Transform _player;
    [SerializeField] private AudioSource _projectileAudioSource;
    [SerializeField] private AudioClip _projectileAudioClip;

    [Header("Custom Controls")]
    [SerializeField] [Range(0, 100)] private float _speed;
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = _player.transform.position + new Vector3(0, 0, 120);
        
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, -_speed  * Time.fixedDeltaTime);
    }
}
