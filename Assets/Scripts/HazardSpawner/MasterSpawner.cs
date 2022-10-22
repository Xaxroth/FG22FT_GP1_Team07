using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size = 3;
    }

    public static int playerLevel = 1;

    public static MasterSpawner Instance;
    
    [Header("Object Pools")]

    public List<Pool> LevelOneChunks;
    public List<Pool> LevelTwoChunks;
    public List<Pool> LevelThreeChunks;

    public List<Pool> Projectiles;
    public List<Pool> Bubbles;

    public Dictionary<string, Queue<GameObject>> LevelOneDictionary;
    public Dictionary<string, Queue<GameObject>> LevelTwoDictionary;
    public Dictionary<string, Queue<GameObject>> LevelThreeDictionary;

    public Dictionary<string, Queue<GameObject>> ProjectileDictionary;
    public Dictionary<string, Queue<GameObject>> BubbleDictionary;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _projectileAudioClip;
    
    //PoolContainers
    private GameObject _levelOneContainer;
    private GameObject _levelTwoContainer;
    private GameObject _levelThreeContainer;
    private GameObject _projectileContainer;


    public void Awake()
    {
        // HAZARDS //

        playerLevel = 1;

        LevelOneDictionary = new Dictionary<string, Queue<GameObject>>();
        _levelOneContainer = new GameObject("LevelOneContainer");

        foreach (Pool pool in LevelOneChunks)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject spawnedObject = Instantiate(pool.Prefab, _levelOneContainer.transform);
                spawnedObject.SetActive(false);
                objectPool.Enqueue(spawnedObject);
            }

            LevelOneDictionary.Add(pool.Tag, objectPool);
             
        }

        LevelTwoDictionary = new Dictionary<string, Queue<GameObject>>();
        _levelTwoContainer = new GameObject("LevelTwoContainer");

        foreach (Pool pool in LevelTwoChunks)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject spawnedObject = Instantiate(pool.Prefab, _levelTwoContainer.transform);
                spawnedObject.SetActive(false);
                objectPool.Enqueue(spawnedObject);
            }

            LevelTwoDictionary.Add(pool.Tag, objectPool);

        }

        LevelThreeDictionary = new Dictionary<string, Queue<GameObject>>();
        _levelThreeContainer = new GameObject("LevelThreeContainer");

        foreach (Pool pool in LevelThreeChunks)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject spawnedObject = Instantiate(pool.Prefab, _levelThreeContainer.transform);
                spawnedObject.SetActive(false);
                objectPool.Enqueue(spawnedObject);
            }

            LevelThreeDictionary.Add(pool.Tag, objectPool);

        }

        // PROJECTILES // 

        ProjectileDictionary = new Dictionary<string, Queue<GameObject>>();
        _projectileContainer = new GameObject("ProjectileContainer");

        foreach (Pool pool in Projectiles)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject spawnedObject = Instantiate(pool.Prefab, _projectileContainer.transform);
                spawnedObject.SetActive(false);
                objectPool.Enqueue(spawnedObject);
            }

            ProjectileDictionary.Add(pool.Tag, objectPool);

        }

        // BUBBLES //

        BubbleDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in Bubbles)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                GameObject spawnedObject = Instantiate(pool.Prefab);
                spawnedObject.SetActive(false);
                objectPool.Enqueue(spawnedObject);
            }

            BubbleDictionary.Add(pool.Tag, objectPool);

        }
    }



    public void SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!LevelOneDictionary.ContainsKey(tag))
        {
            return;
        }

        GameObject newObject;

        switch (playerLevel)
        {
            case 1:
                newObject = LevelOneDictionary[tag].Peek();
                newObject.transform.parent = null;
                newObject.SetActive(true);
                newObject.transform.position = position;
                newObject.transform.rotation = rotation;
                break;
            case 2:
                newObject = LevelTwoDictionary[tag].Peek();
                newObject.transform.parent = null;
                newObject.SetActive(true);
                newObject.transform.position = position;
                newObject.transform.rotation = rotation;
                break;
            case 3:
                newObject = LevelThreeDictionary[tag].Peek();
                newObject.transform.parent = null;
                newObject.SetActive(true);
                newObject.transform.position = position;
                newObject.transform.rotation = rotation;
                break;
        }
    }

    public void SpawnProjectile(string tag, Vector3 position, Quaternion rotation)
    {
        if (!ProjectileDictionary.ContainsKey(tag))
        {
            return;
        }

        GameObject newObject = ProjectileDictionary[tag].Dequeue();

        newObject.SetActive(true);

        newObject.transform.position = position;
        newObject.transform.rotation = rotation;

        ProjectileDictionary[tag].Enqueue(newObject);

        if (newObject.tag == "Projectile")
        {
            _audioSource.PlayOneShot(_projectileAudioClip);
        }
        else if (newObject.tag == "Pickup")
        {
            _audioSource.PlayOneShot(_projectileAudioClip);
        }
    }

    public void SpawnBubbles(string tag, Vector3 position, Quaternion rotation)
    {
        if (!ProjectileDictionary.ContainsKey(tag))
        {
            return;
        }

        GameObject newObject = BubbleDictionary[tag].Dequeue();

        newObject.SetActive(true);

        newObject.transform.position = position;
        newObject.transform.rotation = rotation;

        BubbleDictionary[tag].Enqueue(newObject);
    }
    
    public void PoolObject(GameObject projectile, float timer)
    {
        if (timer <= 0)
        {
            projectile.SetActive(false);
        }
        else
        {
            StartCoroutine(Coroutine(projectile, timer));
        }
    }

    private IEnumerator Coroutine(GameObject projectile, float timer)
    {
        projectile.SetActive(false);
        yield return new WaitForSeconds(timer);
        projectile.SetActive(true);
    }
}
