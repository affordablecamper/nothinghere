using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System;
//script by Daniel Bryant


public class GrenadeLauncher : MonoBehaviour

{
    private Vector3 boltPos;

    //[SerializeField]
    //variables!
    [Header("CameraShake")]
    public float Magnitude = 2f;
    public float Roughness = 10f;
    public float FadeOutTime = 5f;

    [SerializeField]


    [Header("Misc")]

    public Camera FPSCamera;                //Camera
    
    public LayerMask enviormentMask;
    public float defaultFov;
    public LayerMask enemyMask;               //enemy mask
   
    [Space]

    [Header("Integers/Values")]
    public int damage = 20;                 //Damage
    public int magAmmo = 30;            //Total mag ammo.
    public int reserveAmmo = 90;            //Ammo that is left or reserved.
    public int maxMagazineSize = 30;    //The total Size of the mag.

    [Space]
    [Header("Floats/Values")]
    public float boltForce = .3f;           //The force of the bolt.
    public float boltSpeed = .1f;        //How much time the bolt has.
    public float fireRate = 1f;             //FireRate.
    public float weaponRange = 100f;    //Range of the weapon.
    public float bulletPower;               //The thrust of the tracer.
    public float AimFOV;
    public float fovSpeed = 5f;

    public float muzzleflashtime = 0.1f;//How fast the muzzle flash goes off for.
    public float recoilAmount = 2f;         //Physical recoil stuff.
   
    public float aimSpeed = 2f;
    public bool akimbo;
    public float fxrecoilRecoveryRate;
    public float raycastForce;


    //privates
    private float fireCountdown = 0f;   //Something to do with firerate.
    private float muzzleFlashTimerStart;    //When the muzzle flash should start again.

    [Header("GameObjects")]
    [Space]
    public GameObject boltObject;       //The bolt and or slide of the gun.
    public GameObject weaponObject;         //The weapon its self


    public GameObject muzzleFlash;          //The light

    public GameObject bullet;            //The tracer

    private GameObject playerCharacter; //The player character.


    [Space]
    [Header("Booleans")]
    public bool canShoot;              //If the weapon can shoot or not.
    public bool muzzleFlashEnabled = false;//If the muzzle flash is enabled.
    public bool magEmpty;              //If the mag is empty.
    public bool isAiming = false;        //If the player is aiming.
    public bool isReloading;           //If the player is reloading.
    public bool isshiftAiming;          //If the player is aiming and pressing shift.
   

    [Space]
    [Header("Transforms")]
   
    public Transform fwd;             //The forward working direction that the "tracer" gameobject comes out from

    [Space]
    [Header("Vectors")]
    public Vector3 normalPosition;          //The normal position of the view model.
    public Vector3 AimPos;              //The postion of the view model while aiming. 
                                        //public Vector3 shiftaimPos;

    
    public bool isReloadingInProgress;
    public Animator anim;


    [Space]
    [Header("Audio")]

    private AudioSource Audio;         //The audio source

    public AudioClip gunshotClip;      //Audio stuff
    [Space]
    [Header("Impacts")]
    public GameObject concreteImpactEffect;
    public GameObject waterImpactEffect;
    //public GameObject playerImpactEffect;

    //animation stuff




    //public Animation Reload;
    //public Text ammoText;






    public void CancelReload()
    {
        // Reset the reload animation flags
        anim.Rebind();
        //anim.ResetTrigger("FullReload");
        anim.ResetTrigger("Reload");




        // Enable shooting after canceling reload
        canShoot = true;

        isReloading = false;
        isReloadingInProgress = false;


    }



    private void Start()
    {
        

        canShoot = true;
        boltPos = boltObject.transform.localPosition;
        FPSCamera = Camera.main.GetComponent<Camera>();
        isReloading = false;
        Audio = GetComponent<AudioSource>(); // finding the audio source
        muzzleFlashTimerStart = muzzleflashtime;

        if (FPSCamera == null)
        {

            Debug.LogError(Color.cyan + "No FPS Cam is attached");
            this.enabled = false;
        }

    }



    // Update is called once per frame
    //[Client]


    public void Reload()
    {
        if (magAmmo == maxMagazineSize || reserveAmmo == 0)
            return;

        // Check if the reload animation is already playing
        if (isReloadingInProgress)
            return;

        // Set isReloadingInProgress to true before starting the reload animation
        isReloadingInProgress = true;

        if (magEmpty && magAmmo == 0)
        {
            // Play the full empty reload animation or perform other actions specific to full empty reload
            // ...
            anim.SetTrigger("Reload");
        }
        else
        {
            // Play the regular reload animation or perform other actions specific to reloading
            // ...
            anim.SetTrigger("Reload");
        }

        

        // Disable shooting audio temporarily
      
    }

    private void FinishReloadAnimation()
    {
        // Replenish magAmmo
        int ammoNeeded = maxMagazineSize - magAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);
        magAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        // Reset the reload animation flags
        anim.ResetTrigger("Reload");

        // Enable shooting after reload
        canShoot = true;

        // Enable shooting audio
        // ...

        isReloadingInProgress = false;
        isReloading = false;
        magEmpty = false;
        anim.enabled = false; // Disable the animator component
    }


    void Update()
    {
       



        if (Input.GetButtonDown("Fire1") && magEmpty == false && isReloading == false)
        {
            Shoot();
        }

        fireCountdown -= Time.deltaTime;

        if (muzzleFlashEnabled == true)
        {
            muzzleflashtime -= Time.deltaTime;
            muzzleFlash.SetActive(true);
        }

        if (muzzleflashtime <= 0)
        {
            muzzleFlashEnabled = false;
            muzzleflashtime = muzzleFlashTimerStart;
            muzzleFlash.SetActive(false);
        }

       



        if (magAmmo <= 0)
        {

            magEmpty = true;


        }
        else magEmpty = false;




        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading)
            {
                anim.enabled = true; // Move this line inside the if condition
                Reload();
            }
        }



        if (Input.GetKey(KeyCode.Mouse1) && isReloading == false && akimbo == false)
        {

            isAiming = true;

        }
        else if (!Input.GetKey(KeyCode.Mouse1) && isReloading == false && akimbo == false)
        {


            isAiming = false;

        }



      


    }




    //[Client]
    public void Shoot()
    {

        {




            //ammoText.text = ("Ammo:" + magAmmo + "/" + reserveAmmo); // set the ammo text to the desired ammounts set and done by the vars


           
                
                GameObject newProjectile = Instantiate(bullet, fwd.transform.position, fwd.transform.rotation) as GameObject;
                newProjectile.transform.rotation = FPSCamera.transform.rotation;
                newProjectile.GetComponent<Rigidbody>().AddForce(fwd.transform.forward.normalized * bulletPower);

                newProjectile.GetComponent<Rigidbody>().rotation = Quaternion.LookRotation(newProjectile.GetComponent<Rigidbody>().velocity);
                

                //Vector3 Bolt = boltObject.transform.localPosition;
                //Bolt.z = Bolt.z - boltForce;

                //boltObject.transform.localPosition = Bolt;


                //boltObject.transform.Rotate(Vector3.forward * boltForce);
                StartCoroutine(RotateMe(Vector3.forward * boltForce, boltSpeed));

                muzzleFlashEnabled = true;

                magAmmo -= 1;

                //recoil effect kinda... might change this later on doesent actully effect the bullet or raycast <<<TODO
               
                Audio.PlayOneShot(gunshotClip); //playing audio from audio source

                Vector3 weaponObjectLocalPosition = weaponObject.transform.localPosition;
                weaponObjectLocalPosition.z = weaponObjectLocalPosition.z - recoilAmount;
                weaponObject.transform.localPosition = weaponObjectLocalPosition;
                //migration Name = gameObject.AddComponent<migration>();









            



        }




    }



    IEnumerator RotateMe(Vector3 byAngles, float inTime)
    {
        var fromAngle = boltObject.transform.rotation;
        var toAngle = Quaternion.Euler(boltObject.transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            boltObject.transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
    }



    private void FixedUpdate()
    {
        float targetFieldOfView = isAiming ? defaultFov - AimFOV : defaultFov;
        FPSCamera.fieldOfView = Mathf.Lerp(FPSCamera.fieldOfView, targetFieldOfView, Time.deltaTime * fovSpeed);

        boltObject.transform.localPosition = Vector3.Lerp(boltObject.transform.localPosition, boltPos, Time.deltaTime * boltSpeed);
        weaponObject.transform.localPosition = Vector3.Lerp(weaponObject.transform.localPosition, isAiming ? AimPos : normalPosition, Time.deltaTime * fxrecoilRecoveryRate);
    }





    //[Command]
    // void CmdOnPlayerMaskShot(Vector3 _pos, Vector3 _normal)
    //{
    // RpcOnPlayerMaskShot(_pos, _normal);
    //}


    //[ClientRpc]
    //void RpcOnPlayerMaskShot(Vector3 _pos, Vector3 _normal)
    //{
    //Instantiate(playerimpactEffect, _pos, Quaternion.LookRotation(_normal));

    //}

    //[Command]
    //void CmdOnShootEffect()
    //{
    // RpcOnShootEffect();
    // }


    //[ClientRpc]
    //void RpcOnShootEffect()
    //{
    //thirdpersonMuzzleFlashObject.SetActive(true);

    //}

    //[Command]
    //void CmdOnTracerShoot(Vector3 _hit)
    //{
    //RpcOnTracerShoot(_hit);

    //}

    //[ClientRpc]
    //void RpcOnTracerShoot(Vector3 _hit)
    //{
    //GameObject newProjectile = Instantiate(tracer, fwd.transform.position, fwd.transform.rotation) as GameObject;
    //newProjectile.GetComponent<Rigidbody>().AddForce(fwd.transform.forward.normalized * tracerPower);
    //newProjectile.GetComponent<Rigidbody>().velocity = (_hit - transform.position).normalized * tracerPower;
    //newProjectile.GetComponent<Rigidbody>().rotation = Quaternion.LookRotation(newProjectile.GetComponent<Rigidbody>().velocity);

    //}

    //[Command]
    //public void CmdPlayerShot(string _playerID, int _damage)
    //{

    //Debug.Log(_playerID + "has been shot");
    // Player _player = GameManager1.GetPlayer(_playerID);
    //_player.RpcTakeDamage(_damage);

    //}


    //private void doneShooting()
    //{
    //StartCoroutine(boltShootingTimer());


    //}

    //[Command]
    //void CmdNoShootEffect()
    //{
    //RpcNoShootEffect();
    //}



    //[ClientRpc]
    //void RpcNoShootEffect()
    //{
    //thirdpersonMuzzleFlashObject.SetActive(false);
    //}

    //private IEnumerator boltShootingTimer()
    //{
    //yield return new WaitForSeconds(boltTime);
    //Vector3 Bolt = boltObject.transform.localPosition;
    //Bolt.x = Bolt.x + boltForce;

    //boltObject.transform.localPosition = Bolt;
    //}

}
