using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventTutorial : MonoBehaviour
{
    public AudioSource source;
    public AudioClip directions2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            source.PlayOneShot(directions2);

    }
}
