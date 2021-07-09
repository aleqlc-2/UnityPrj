using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private WeaponManager weapon_Manager;

    public float fireRate = 15f;
    private float nextTimeToFire;
    public float damage = 20f;

    private Animator zoomCameraAnim;
    private bool zoomed;
    private Camera mainCam;
    private GameObject crosshair;
    private bool is_Aiming;

    [SerializeField] private GameObject arrow_Prefab, spear_Prefab;
    [SerializeField] private Transform arrow_Bow_StartPosition;

    void Awake()
    {
        // 이 스크립트가 부착되어 있는 오브젝트에서 WeaponManager스크립트를 불러옴
        weapon_Manager = GetComponent<WeaponManager>();

        zoomCameraAnim = transform.Find(Tags.LOOK_ROOT)
                        .transform.Find(Tags.ZOOM_CAMERA).GetComponent<Animator>();

        crosshair = GameObject.FindWithTag(Tags.CROSSHAIR);

        mainCam = Camera.main;
    }

    void Update()
    {
        WeaponShoot();
        ZoomInAndOut();
    }

    void WeaponShoot()
    {
        // 4 - Assault Rifle
        if (weapon_Manager.GetCurrentSelectedWeapon().fireType == WeaponFireType.MULTIPLE)
        {
            if (Input.GetMouseButton(0) && Time.time > nextTimeToFire) // 연사하므로 GetMouseButton
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();
                BulletFired();
            }
        }
        else // 4 - Assault Rifle 이외의 무기
        {
            if (Input.GetMouseButtonDown(0)) // 단발이므로 GetMouseButtonDown
            {
                // Axe
                if (weapon_Manager.GetCurrentSelectedWeapon().tag == Tags.AXE_TAG)
                {
                    weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();
                }

                // Revolver, Shotgun
                if (weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.BULLET)
                {
                    weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();
                    BulletFired();
                }
                else // Arrow, Spear // Axe도 포함되지만 SELF_AIM불가이기때문에 상관없음
                {
                    if (is_Aiming) // SELF_AIM이 활성화되었다면(즉, aim먼저 하지 않으면 발사불가)
                    {
                        weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();

                        if (weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.ARROW)
                        {
                            ThrowArrowOrSpear(true); // 화살 발사
                        }
                        else if (weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.SPEAR)
                        {
                            ThrowArrowOrSpear(false); // 창 발사
                        }
                    }
                }
            }
        }
    }

    void ZoomInAndOut()
    {
        // AIM이 가능한 무기일때
        if (weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.AIM)
        {
            if (Input.GetMouseButtonDown(1)) // 마우스 우클릭하면
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_IN_ANIM); // 줌인
                crosshair.SetActive(false);
            }

            if (Input.GetMouseButtonUp(1)) // 마우스 우클릭 뗐을 때
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM); // 줌아웃
                crosshair.SetActive(true);
            }
        }
        
        // SELF_AIM이 가능한 무기일때(Spear, Bow)
        if (weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.SELF_AIM)
        {
            if (Input.GetMouseButtonDown(1))
            {
                weapon_Manager.GetCurrentSelectedWeapon().Aim(true); // 미리 만들어놓은 애니메이션인듯
                is_Aiming = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                weapon_Manager.GetCurrentSelectedWeapon().Aim(false);
                is_Aiming = false;
            }
        }
    }

    void ThrowArrowOrSpear(bool throwArrow)
    {
        if (throwArrow)
        {
            GameObject arrow = Instantiate(arrow_Prefab);
            arrow.transform.position = arrow_Bow_StartPosition.position;
            arrow.GetComponent<ArrowBowScript>().Launch(mainCam);
        }
        else
        {
            GameObject spear = Instantiate(spear_Prefab);
            spear.transform.position = arrow_Bow_StartPosition.position;
            spear.GetComponent<ArrowBowScript>().Launch(mainCam);
        }
    }

    void BulletFired()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
        {
            // print(hit.transform.gameObject.tag); // 영점대로 사격이 정확히 되는지 확인
            
            if (hit.transform.tag == Tags.ENEMY_TAG)
            {
                hit.transform.GetComponent<HealthScript>().ApplyDamage(damage);
            }
        }
    }
}
