using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    // 만약 각각의 무기에 WeaponHandler스크립트를 부착 안했다면
    // GameObject[] 자료형으로 썼겠지.
    [SerializeField] private WeaponHandler[] weapons;

    private int current_Weapon_Index;

    void Start()
    {
        // 게임시작시 디폴트무기는 Axe
        current_Weapon_Index = 0;
        weapons[current_Weapon_Index].gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1
        {
            TurnOnSelectedWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2
        {
            TurnOnSelectedWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // 3
        {
            TurnOnSelectedWeapon(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) // 4
        {
            TurnOnSelectedWeapon(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5)) // 5
        {
            TurnOnSelectedWeapon(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6)) // 6
        {
            TurnOnSelectedWeapon(5);
        }
    }

    void TurnOnSelectedWeapon(int weaponIndex)
    {
        if (current_Weapon_Index == weaponIndex) return; // 이미 들고있는무기면 return

        weapons[current_Weapon_Index].gameObject.SetActive(false); // 현재 들고있는 무기 비활성화
        weapons[weaponIndex].gameObject.SetActive(true); // 새로운 무기 활성화
        current_Weapon_Index = weaponIndex; // 현재 무기의 인덱스로 갱신
    }

    public WeaponHandler GetCurrentSelectedWeapon()
    {
        return weapons[current_Weapon_Index];
    }
}
