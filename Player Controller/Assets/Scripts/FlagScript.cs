using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagScript : MonoBehaviour
{
    [SerializeField] TimerScript timerScript;
    [SerializeField] ParticleSystem confetti;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timerScript.StopTimer();
            confetti.Play();
        }
    }
}
