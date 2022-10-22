using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private float offset;
    private float startOffset;

    private void Start()
    {
        playerTransform = GameManager.Instance.Player.transform;
        startOffset = offset;
    }

    private void LateUpdate(){
        transform.position = new Vector3 (transform.position.x,transform.position.y,playerTransform.position.z + offset);
    }
    
    public void AddOffset(float value)
    {
        var maxOffset = startOffset * 1.5f;
        offset -= value * 15;
        if (offset < maxOffset)
        {
            offset = maxOffset;
        }
    }
    
    public void RevertOffsetToNormal(){
        StartCoroutine(RevertOffset());
    }
    
    private IEnumerator RevertOffset(){
        while (offset != startOffset)
        {
            offset = Mathf.Lerp(offset, startOffset, Time.deltaTime * 2);
            yield return null;
        }
    }

}
