using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalBar : MonoBehaviour
{
    public Slider slider;

    
    public void SetDistance(int distance)
    {
        slider.value = distance;
    }
}
