using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FindClosestWeapon : Conditional
{
    public SharedGameObject closestWeapon;
    public float maxReachableDistance = 10f;
    public int minimumWeaponCount = 2;

    public override TaskStatus OnUpdate()
    {
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Pickup");
        if (weapons.Length == 0)
        {
            closestWeapon.Value = null;
            return TaskStatus.Failure;
        }

        Transform closestTransform = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        int weaponCount = 0;

        foreach (GameObject weapon in weapons)
        {
            float distance = Vector3.Distance(currentPosition, weapon.transform.position);
            if (distance <= maxReachableDistance)
            {
                if (closestTransform == null || distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTransform = weapon.transform;
                }

                weaponCount++;
                if (weaponCount >= minimumWeaponCount)
                    break;
            }
        }

        if (closestTransform != null)
        {
            closestWeapon.Value = closestTransform.gameObject;
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}