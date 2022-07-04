using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticCloseMenu : MonoBehaviour
{
    [SerializeField] private float delayedCloseTimer;
    private float elapsedTime;

    // Start is called before the first frame update
    void OnEnable()
    {
        elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= delayedCloseTimer)
        {
            gameObject.SetActive(false);
        }
    }
}