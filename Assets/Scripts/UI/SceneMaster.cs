using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMaster : MonoBehaviour
{
    public GameObject text;
    public float sec = 5f;


    private void Start()
    {
        StartCoroutine(ShowAndHide(5.0f));
    }
    IEnumerator ShowAndHide(float delay)
    {
        text.SetActive(true);
        yield return new WaitForSeconds(delay);
        text.SetActive(false);
    }
}
