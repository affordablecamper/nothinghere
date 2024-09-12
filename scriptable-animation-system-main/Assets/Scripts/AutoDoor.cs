using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoDoor : MonoBehaviour
{
    public Animator anim;
    public Collider col;
    public AudioSource source;
    public AudioClip clip;
    public Behaviour obstacle;
    public float doorStayOpenTime;
    public bool doorIsLocked;
    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" &&!doorIsLocked)
        {
            anim.SetBool("Open", true);
            source.PlayOneShot(clip);
            col.enabled = false;
            obstacle.enabled = false;
        }

       

    }


    


    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Player" &&!doorIsLocked)
        {
            StartCoroutine("Wait", doorStayOpenTime);
        }

        
    }


    IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        anim.SetBool("Open", false);
        source.PlayOneShot(clip);
        col.enabled = true;
        obstacle.enabled = true;
    }

}
