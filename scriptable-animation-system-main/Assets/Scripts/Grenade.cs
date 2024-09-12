using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grenade : MonoBehaviour
{
    public GameObject Explosion;
    [SerializeField]
    private int bounces;
    public int damage;
    public AudioSource Audio;         //The audio source
    public AudioClip explosion;
    public float range;
    [Header("CameraShake")]
    public float Magnitude = 2f;
    public float Roughness = 10f;
    public float FadeOutTime = 5f;
    public float explosionForce = 100f;

 

    private void OnCollisionEnter(Collision collision)
    {

        bounces++;

        if (bounces >= 2) {

            Audio.PlayOneShot(explosion);
            //CameraShaker.Instance.ShakeOnce(Magnitude, Roughness, 0, FadeOutTime);
            Instantiate(Explosion, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, range);


            foreach (Collider nearbyObject in colliders)
            {


                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                //if (rb != null)
                //rb.AddExplosionForce(explosionForce, transform.position, range);
                //Debug.Log("teeest");

                if (nearbyObject.gameObject.layer == 17 || nearbyObject.gameObject.layer == 18 || nearbyObject.gameObject.layer == 19)
                {

                  
                    Limbs _limb = nearbyObject.transform.GetComponent<Limbs>();

                    _limb.SendHitToServer(6969, damage); // attacker id doesnt work with grenades dont really need to fix maybe?
                }

                



            }

            Destroy(this.gameObject);





        }





    }



    
}
