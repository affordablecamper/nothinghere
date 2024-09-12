// Designed by KINEMATION, 2024.

using KINEMATION.FPSAnimationFramework.Runtime.Camera;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.FPSAnimationFramework.Runtime.Playables;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.KAnimationCore.Runtime.Input;

using Demo.Scripts.Runtime.AttachmentSystem;

using System.Collections.Generic;
using Demo.Scripts.Runtime.Character;
using UnityEngine;
using FishNet.Object;
namespace Demo.Scripts.Runtime.Item
{
    public class Weapon : FPSItem
    {
        [Header("GeneralAnim")]
        [SerializeField] [Range(0f, 120f)] private float fieldOfView = 90f;
        
        [SerializeField] private FPSAnimationAsset reloadClip;
        [SerializeField] private FPSCameraAnimation cameraReloadAnimation;
        
        [SerializeField] private FPSAnimationAsset grenadeClip;
        [SerializeField] private FPSCameraAnimation cameraGrenadeAnimation;

        [Header("Raycast")]
        [SerializeField] private float raycastRange = 100f; // Range of the raycast
        [SerializeField] private LayerMask raycastLayerMask; // Layers to hit

        [Header("Recoil")]
        [SerializeField] private FPSAnimationAsset fireClip;
        [SerializeField] private RecoilAnimData recoilData;
        [SerializeField] private RecoilPatternSettings recoilPatternSettings;
        [SerializeField] private FPSCameraShake cameraShake;
        [Min(0f)] [SerializeField] private float fireRate;

        [SerializeField] private bool supportsAuto;
        [SerializeField] private bool supportsBurst;
        [SerializeField] private int burstLength;

        [Header("Attachments")] 
        
        [SerializeField]
        private AttachmentGroup<BaseAttachment> barrelAttachments = new AttachmentGroup<BaseAttachment>();
        
        [SerializeField]
        private AttachmentGroup<BaseAttachment> gripAttachments = new AttachmentGroup<BaseAttachment>();
        
        [SerializeField]
        private List<AttachmentGroup<ScopeAttachment>> scopeGroups = new List<AttachmentGroup<ScopeAttachment>>();


        [Header("Audio")]
        [SerializeField] private AudioSource AS_source;
        [SerializeField] private AudioClip[] AS_fireClips;

        [Header("Gun Settings")]

        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private int headDamage;
        [SerializeField] private int hipDamage;
        [SerializeField] private int armDamage;
        [SerializeField] private int legDamage;
        [SerializeField] private int chestDamage;
        
        [SerializeField] private float muzzleFlashDuration = 0.1f;
        [SerializeField] private float bulletForce;


        [Header("Impact Settings")]
        [SerializeField] private GameObject glassImpactEffect;
        [SerializeField] private GameObject metalImpactEffect;
        [SerializeField] private GameObject concreteImpactEffect;
        [SerializeField] private GameObject waterImpactEffect;

        //~ Controller references

        private FPSController _fpsController;
        private Animator _controllerAnimator;
        private UserInputController _userInputController;
        private IPlayablesController _playablesController;
        private FPSCameraController _fpsCameraController;
        private PlayerSetup setup;
        private FPSAnimator _fpsAnimator;
        private FPSAnimatorEntity _fpsAnimatorEntity;

        private RecoilAnimation _recoilAnimation;
        private RecoilPattern _recoilPattern;
        
        //~ Controller references
        
        private Animator _weaponAnimator;
        private int _scopeIndex;
        
        private float _lastRecoilTime;
        private int _bursts;
        private FireMode _fireMode = FireMode.Semi;
        
        private static readonly int CurveEquip = Animator.StringToHash("CurveEquip");
        private static readonly int CurveUnequip = Animator.StringToHash("CurveUnequip");
        
        private void OnActionEnded()
        {
            if (_fpsController == null) return;
            _fpsController.ResetActionState();
        }

        protected void UpdateTargetFOV(bool isAiming)
        {
            float fov = fieldOfView;
            float sensitivityMultiplier = 1f;
            
            if (isAiming && scopeGroups.Count != 0)
            {
                var scope = scopeGroups[_scopeIndex].GetActiveAttachment();
                fov *= scope.aimFovZoom;

                sensitivityMultiplier = scopeGroups[_scopeIndex].GetActiveAttachment().sensitivityMultiplier;
            }

            _userInputController.SetValue("SensitivityMultiplier", sensitivityMultiplier);
            _fpsCameraController.UpdateTargetFOV(fov);
        }

        protected void UpdateAimPoint()
        {
            if (scopeGroups.Count == 0) return;

            var scope = scopeGroups[_scopeIndex].GetActiveAttachment().aimPoint;
            _fpsAnimatorEntity.defaultAimPoint = scope;
        }
        
        protected void InitializeAttachments()
        {
            foreach (var attachmentGroup in scopeGroups)
            {
                attachmentGroup.Initialize(_fpsAnimator);
            }
            
            _scopeIndex = 0;
            if (scopeGroups.Count == 0) return;

            UpdateAimPoint();
            UpdateTargetFOV(false);
        }
        
        public override void OnEquip(GameObject parent)
        {
            if (parent == null) return;
            
            _fpsAnimator = parent.GetComponent<FPSAnimator>();
            _fpsAnimatorEntity = GetComponent<FPSAnimatorEntity>();
            
            _fpsController = parent.GetComponent<FPSController>();
            _weaponAnimator = GetComponentInChildren<Animator>();
            
            _controllerAnimator = parent.GetComponent<Animator>();
            _userInputController = parent.GetComponent<UserInputController>();
            _playablesController = parent.GetComponent<IPlayablesController>();
            _fpsCameraController = parent.GetComponentInChildren<FPSCameraController>();

            if (overrideController != _controllerAnimator.runtimeAnimatorController)
            {
                _playablesController.UpdateAnimatorController(overrideController);
            }
            
            InitializeAttachments();
            
            _recoilAnimation = parent.GetComponent<RecoilAnimation>();
            _recoilPattern = parent.GetComponent<RecoilPattern>();
            
            _fpsAnimator.LinkAnimatorProfile(gameObject);
            
            barrelAttachments.Initialize(_fpsAnimator);
            gripAttachments.Initialize(_fpsAnimator);
            
            _recoilAnimation.Init(recoilData, fireRate, _fireMode);

            if (_recoilPattern != null)
            {
                _recoilPattern.Init(recoilPatternSettings);
            }
            
            _fpsAnimator.LinkAnimatorLayer(equipMotion);
        }

        public override void OnUnEquip()
        {
            _fpsAnimator.LinkAnimatorLayer(unEquipMotion);
        }

        public override bool OnAimPressed()
        {
            _userInputController.SetValue(FPSANames.IsAiming, true);
            UpdateTargetFOV(true);
            _recoilAnimation.isAiming = true;
            
            return true;
        }

        public override bool OnAimReleased()
        {
            _userInputController.SetValue(FPSANames.IsAiming, false);
            UpdateTargetFOV(false);
            _recoilAnimation.isAiming = false;
            
            return true;
        }

        public override bool OnFirePressed()
        {
            // Do not allow firing faster than the allowed fire rate.
            if (Time.unscaledTime - _lastRecoilTime < 60f / fireRate)
            {
                return false;
            }
            
            _lastRecoilTime = Time.unscaledTime;
            _bursts = burstLength;
            
            OnFire();
            
            return true;
        }

        public override bool OnFireReleased()
        {
            if (_recoilAnimation != null)
            {
                _recoilAnimation.Stop();
            }
            
            if (_recoilPattern != null)
            {
                _recoilPattern.OnFireEnd();
            }
            
            CancelInvoke(nameof(OnFire));
            return true;
        }

        public override bool OnReload()
        {
            if (!FPSAnimationAsset.IsValid(reloadClip))
            {
                return false;
            }
            
            _playablesController.PlayAnimation(reloadClip, 0f);
            
            if (_weaponAnimator != null)
            {
                _weaponAnimator.Rebind();
                _weaponAnimator.Play("Reload", 0);
            }

            if (_fpsCameraController != null)
            {
                _fpsCameraController.PlayCameraAnimation(cameraReloadAnimation);
            }
            
            Invoke(nameof(OnActionEnded), reloadClip.clip.length * 0.85f);

            OnFireReleased();
            return true;
        }

        public override bool OnGrenadeThrow()
        {
            if (!FPSAnimationAsset.IsValid(grenadeClip))
            {
                return false;
            }

            _playablesController.PlayAnimation(grenadeClip, 0f);
            
            if (_fpsCameraController != null)
            {
                _fpsCameraController.PlayCameraAnimation(cameraGrenadeAnimation);
            }
            
            Invoke(nameof(OnActionEnded), grenadeClip.clip.length * 0.8f);
            return true;
        }
        
        private void OnFire()
        {
            //bullet casing
            //tracers for projectile tracer rounds
            //function for ammo
            //function for bolt/slide simulation
            //maybe possibly dual raycast projectile shooting system?

            
            PlayRandomFireClip(); // play audio for gunshot
            TriggerMuzzleFlash();
            PerformRaycast();
            if (_weaponAnimator != null)
            {
                _weaponAnimator.Play("Fire", 0, 0f);
            }
            
            _fpsCameraController.PlayCameraShake(cameraShake);
            
            if(fireClip != null) _playablesController.PlayAnimation(fireClip);

            if (_recoilAnimation != null && recoilData != null)
            {
                _recoilAnimation.Play();
            }

            if (_recoilPattern != null)
            {
                _recoilPattern.OnFireStart();
            }

            if (_recoilAnimation.fireMode == FireMode.Semi)
            {
                Invoke(nameof(OnFireReleased), 60f / fireRate);
                return;
            }
            
            if (_recoilAnimation.fireMode == FireMode.Burst)
            {
                _bursts--;
                
                if (_bursts == 0)
                {
                    OnFireReleased();
                    return;
                }
            }
            
            Invoke(nameof(OnFire), 60f / fireRate);
        }

        // Muzzle flash functionality
        private void TriggerMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(true); // Show the muzzle flash
                Invoke(nameof(HideMuzzleFlash), muzzleFlashDuration); // Schedule to hide after duration
            }
        }

        private void HideMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(false); // Hide the muzzle flash
            }
        }


        private void PerformRaycast()
        {
            if (_fpsAnimatorEntity == null || _fpsAnimatorEntity.defaultAimPoint == null)
            {
                Debug.LogWarning("Aim point not set!");
                return;
            }

            Vector3 rayOrigin = _fpsAnimatorEntity.defaultAimPoint.position;
            Vector3 rayDirection = _fpsAnimatorEntity.defaultAimPoint.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, raycastRange, raycastLayerMask))
            {
                Debug.Log($"Raycast hit: {hitInfo.collider.gameObject.name} at distance {hitInfo.distance}");
                Rigidbody rb = hitInfo.transform.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(bulletForce, transform.position, 3.5f);
                }

                #region enviornmental impacts

                if (hitInfo.collider.tag == "Concrete")
                {
                    Instantiate(concreteImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

                if (hitInfo.collider.tag == "Water")
                {
                    Instantiate(waterImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

                if (hitInfo.collider.tag == "Metal")
                {
                    Instantiate(metalImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

                if (hitInfo.collider.tag == "Glass")
                {
                    Instantiate(glassImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

                if (hitInfo.collider.tag == "Vent")
                {
                    Instantiate(metalImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }

                #endregion

                #region NonPlayerEntitiy Impacts

                if (hitInfo.collider.tag == "Head")
                {
                    //Instantiate(enemyImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Limbs _limb = hitInfo.transform.GetComponent<Limbs>();
                    SendDamage(_limb, headDamage);
                }
                if (hitInfo.collider.tag == "Hips")
                {
                    //Instantiate(enemyImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Limbs _limb = hitInfo.transform.GetComponent<Limbs>();
                    SendDamage(_limb, hipDamage);
                }

                if (hitInfo.collider.tag == "Arms")
                {
                    //Instantiate(enemyImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Limbs _limb = hitInfo.transform.GetComponent<Limbs>();
                    SendDamage(_limb, armDamage);
                }

                if (hitInfo.collider.tag == "Legs")
                {
                    //Instantiate(enemyImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Limbs _limb = hitInfo.transform.GetComponent<Limbs>();
                    SendDamage(_limb, legDamage);
                }

                if (hitInfo.collider.tag == "Chest")
                {
                    //Instantiate(enemyImpactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Limbs _limb = hitInfo.transform.GetComponent<Limbs>();
                    SendDamage(_limb, chestDamage);
                }



                #endregion


            }
            else
            {
                Debug.Log("Raycast did not hit anything");
            }

            // Optionally, you can visualize the ray in the scene view
            Debug.DrawRay(rayOrigin, rayDirection * raycastRange, Color.green, 1f);
        }

        public void SendDamage(Limbs _limb, int _damage)
        {
            GameObject playerGameObject = _fpsController.gameObject;
            setup = playerGameObject.GetComponent<PlayerSetup>();
            _limb.SendHitToServer(setup.playerID, _damage);
        }


        //audio
        public void PlayRandomFireClip()
        {
            if (AS_fireClips.Length == 0)
            {
                Debug.LogWarning("No audio clips assigned!");
                return;
            }

            int randomIndex = Random.Range(0, AS_fireClips.Length);
            AudioClip randomClip = AS_fireClips[randomIndex];

            AS_source.PlayOneShot(randomClip);
        }


        public override void OnCycleScope()
        {
            if (scopeGroups.Count == 0) return;
            
            _scopeIndex++;
            _scopeIndex = _scopeIndex > scopeGroups.Count - 1 ? 0 : _scopeIndex;
            
            UpdateAimPoint();
            UpdateTargetFOV(true);
        }

        private void CycleFireMode()
        {
            if (_fireMode == FireMode.Semi && supportsBurst)
            {
                _fireMode = FireMode.Burst;
                _bursts = burstLength;
                return;
            }

            if (_fireMode != FireMode.Auto && supportsAuto)
            {
                _fireMode = FireMode.Auto;
                return;
            }

            _fireMode = FireMode.Semi;
        }
        
        public override void OnChangeFireMode()
        {
            CycleFireMode();
            _recoilAnimation.fireMode = _fireMode;
        }

        public override void OnAttachmentChanged(int attachmentTypeIndex)
        {
            if (attachmentTypeIndex == 1)
            {
                barrelAttachments.CycleAttachments(_fpsAnimator);
                return;
            }

            if (attachmentTypeIndex == 2)
            {
                gripAttachments.CycleAttachments(_fpsAnimator);
                return;
            }

            if (scopeGroups.Count == 0) return;
            scopeGroups[_scopeIndex].CycleAttachments(_fpsAnimator);
            UpdateAimPoint();
        }
    }

   


}