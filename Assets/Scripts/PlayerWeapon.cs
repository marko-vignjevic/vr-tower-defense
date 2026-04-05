using UnityEngine;
using System.Collections;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;

    [Header("Gun Object")]
    public GameObject gun;
    private float fireCooldown = 0f;
    private bool gunEquipped = false;

    void Start()
    {

        gun.SetActive(false);
    }

    void Update()
    {
        HandleWeaponSwap();

        if (!gunEquipped)
        {
            return;
        }

        HandleShooting();
    }

    void HandleWeaponSwap()
    {
        // press button 1 on right controller to toggle gun
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            gunEquipped = !gunEquipped;
            gun.SetActive(gunEquipped);
        }
    }
    void HandleShooting()
    {
        // Shoot when the right index trigger is held down with a cooldown
        fireCooldown -= Time.deltaTime;

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1.0f / fireRate;
        }
    }

    void Shoot()
    {
        // Instantiate a projectile and set its velocity
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * 30.0f;
        }
    }
}