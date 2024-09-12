using UnityEngine;
using System.Collections;
using System;

public class TracerScript : MonoBehaviour {

    public int count = 0;
   
    public float despawnTime;
	
	void Start () {
		StartCoroutine (Despawn());
	}
	
	IEnumerator Despawn() {
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}

    private void OnCollisionEnter(Collision collision)
    {
        count++;

    }


    private void Update()
    {
        if (count > 1)
        {
            Destroy(gameObject);
            
        }


    }

}