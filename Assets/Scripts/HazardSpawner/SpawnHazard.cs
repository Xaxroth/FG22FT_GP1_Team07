using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHazard : MonoBehaviour
{
    [Header("Logistics")]
    [SerializeField] private MasterSpawner _masterSpawnerScript;
    [SerializeField] private EnemyScript _enemy;

    [SerializeField] private Transform spawnLocation;

    [SerializeField] private GameObject tutorialLevel;

    [SerializeField] private AudioSource _objectAudioSource;
    [SerializeField] private AudioClip _projectileAudioClip;

    [Header("Custom Controls")]
    [SerializeField] private bool _randomizeRotation;

    [SerializeField] [Range(0, 10)] private float _spawnRate = 8;

    [SerializeField] private int _minValue = 0;
    [SerializeField] private int _maxValue = 0;

    [SerializeField] private string _objectString;

    private int _spawnIndex;

    void Awake()
    {
        _masterSpawnerScript = GameObject.FindGameObjectWithTag("HazardSpawner").GetComponent<MasterSpawner>();

    }

    private void Start()
    {
        _enemy = GameManager.Instance.Enemy.GetComponent<EnemyScript>();

        GameObject Tutorial = Instantiate(tutorialLevel, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1400), Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z));
    }

    private void Update()
    {
        _spawnRate = _enemy.DistanceToPlayer / 10f;

        if (_spawnRate >= 5)
        {
            _spawnRate = 5;
        }

        if (_spawnRate <= 1)
        {
            _spawnRate = 1;
        }
    }

    public void SpawnObjects()
    {
        switch (MasterSpawner.playerLevel)
        {
            case 1:
                _spawnIndex = Random.Range(_minValue, _masterSpawnerScript.LevelOneChunks.Count);
                break;
            case 2:
                _spawnIndex = Random.Range(_minValue, _masterSpawnerScript.LevelTwoChunks.Count);
                break;
            case 3:
                _spawnIndex = Random.Range(_minValue, _masterSpawnerScript.LevelThreeChunks.Count);
                break;
        }

        _objectString = _spawnIndex.ToString();
        _masterSpawnerScript.SpawnFromPool(_objectString, spawnLocation.transform.position, transform.rotation);
    }

    public IEnumerator SpawnBubbles()
    {
        _spawnIndex = Random.Range(_minValue, _masterSpawnerScript.Bubbles.Count);

        _objectString = _spawnIndex.ToString();

        if (_randomizeRotation)
        {
            transform.rotation = Quaternion.Euler(Random.Range(0, 180), 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        _masterSpawnerScript.SpawnBubbles(_objectString, spawnLocation.transform.position, transform.rotation);

        yield return new WaitForSeconds(_spawnRate);

        StartCoroutine(SpawnBubbles());
    }
}
