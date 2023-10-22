using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLine : MonoBehaviour
{
    [SerializeField] TimerScript timerScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timerScript.ResetTimer();
            timerScript.StartTimer();
        }
    }


}
