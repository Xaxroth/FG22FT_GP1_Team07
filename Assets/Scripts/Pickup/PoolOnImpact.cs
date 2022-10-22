using UnityEngine;


public class PoolOnImpact : MonoBehaviour
{
    private float _respawnTimer;
    public void Init(float timer)
    {
        _respawnTimer = timer;
    }

    public void Pool()
    {
        GameManager.Instance.Spawner.PoolObject(this.gameObject, _respawnTimer);
    }
}
