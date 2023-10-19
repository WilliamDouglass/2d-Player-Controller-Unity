using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlUIText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI JumpsText;
    [SerializeField] private PlayerMovement playerMovementScript;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int numJumps = playerMovementScript.jumpCounter;
        JumpsText.text = $"Jumps: {numJumps}";
    }
}
