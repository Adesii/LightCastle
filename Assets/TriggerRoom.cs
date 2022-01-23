using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRoom : MonoBehaviour
{
    private Room totrigger;
    // Start is called before the first frame update
    void Start()
    {
        totrigger = GetComponentInParent<Room>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Instance.Player.gameObject)
        {
            totrigger?.TriggerRoom();
            gameObject.SetActive(false);
        }
    }
}
