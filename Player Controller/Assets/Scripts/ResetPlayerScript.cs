using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPlayerScript : MonoBehaviour
{
    [SerializeField] private Transform StartPoint;
    [SerializeField] private TimerScript timerScript;
    public void ResetPlayer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            gameObject.transform.position = StartPoint.position;
            timerScript.StopTimer();
            timerScript.ResetTimer();
        }
    }
}
