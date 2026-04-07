using UnityEngine;

public class MortarTower : MonoBehaviour
{
    [Header("Targeting")]
    public float range = 15f;

    [Header("Shooting")]
    public GameObject mortarProjectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float fireCooldown = 0f;

    private Transform currentTarget;

    void Update()
    {
        // Calls all the functions needed to find a target
        // and shoot while updating the cooldown 
        FindTarget();

        fireCooldown -= Time.deltaTime;

        if (currentTarget != null && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    void FindTarget()
    {
        // Find the enemy closest to the end of the path within range
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform bestTarget = null;
        int highestWaypointIndex = -1;

        foreach (GameObject enemyObj in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemyObj.transform.position);
            if (distance > range)
            {
                continue;
            }

            EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();

            if (movement == null)
            {
                continue;
            }

            if (movement.waypointIndex > highestWaypointIndex)
            {
                highestWaypointIndex = movement.waypointIndex;
                bestTarget = enemyObj.transform;
            }
        }

        currentTarget = bestTarget;
    }

    void Shoot()
    {

        // Shoot a mortar projectile towards the current target's last known position
        Vector3 targetPosition = currentTarget.position;

        GameObject projectile = Instantiate(mortarProjectilePrefab, firePoint.position, Quaternion.identity);

        MortarProjectile mortar = projectile.GetComponent<MortarProjectile>();
        
        if (mortar != null)
        {
            mortar.SetTarget(targetPosition);
        }
    }
}
