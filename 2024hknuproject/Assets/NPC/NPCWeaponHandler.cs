using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeaponHandler : MonoBehaviour
{
    public IWeapon currentWeapon;

    private void Start()
    {
        // 기본 무기로 ProjectileWeapon을 설정합니다.
        currentWeapon = GetComponent<ProjectileWeapon>();
    }

    public void Attack(Vector2 targetPosition)
    {
        if (currentWeapon != null)
        {
            currentWeapon.Attack(targetPosition);
        }
    }
}
