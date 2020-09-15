
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f; //period is length of sin wave

    //todo remove from inspector later
    [Range(0, 1)] [SerializeField] float movementFactor; //0 for not moved, 1 for fully moved
    Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; } //Mathf.Epsilon is smallest float, cannot compare float to 0 or 0f 
        float cycles = Time.time / period; //grows continually from 0

        const float tau = Mathf.PI * 2f; //about 6.28, 2PI is length around circle and one sin wave
        float rawSinWave = Mathf.Sin(cycles * tau); 

        movementFactor = rawSinWave / 2f + 0.5f; // /2 range is now -0.5 to 0.5, +1 range is now 0 to 1 
        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
        
    }
}
