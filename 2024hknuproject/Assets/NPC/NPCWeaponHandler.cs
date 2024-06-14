using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeaponHandler : MonoBehaviour
{
    public IWeapon currentWeapon;

    private void Start()
    {
        // �⺻ ����� ProjectileWeapon�� �����մϴ�.
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
