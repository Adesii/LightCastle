using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnterTrigger : MonoBehaviour
{

    public UnityEvent OnPlayerEnter;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponentInChildren<PlayerCharacter>() != null)
        {
            OnPlayerEnter?.Invoke();
        }
    }
}
