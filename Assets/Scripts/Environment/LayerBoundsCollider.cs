using UnityEngine;


[RequireComponent(typeof(Collider))]
public class LayerBoundsCollider : MonoBehaviour
{
    
    [SerializeField] private float _pushBackForce = 1f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerState player))
        {
            var collisionPoint = collision.contacts[0].point;
            player.BuffHandler.PushBack(collisionPoint, _pushBackForce);
        }
    }
}
