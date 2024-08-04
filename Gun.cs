using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class Gun : MonoBehaviour
{
    // Particle system for shooting effect
    public ParticleSystem shootParticle;
    public ParticleSystem hitParticle;
    // Gun transform (position and rotation)
    public Transform gun;
    public float fireRate = 0.5f;
    public bool canFire = true;
    // Camera transform (position and rotation)
    public Transform cam;
    // Force applied to objects hit by the bullet
    public float force = 10.0f;
    public float damage = 50;
    public float health = 100;
    public int ammo = 50;
    public int maxAmmo = 50;

    public float upgradeRate = 0.2f;
    public int upgradeAmmo = 100;

    public AudioClip hitClip;
    public AudioClip emptyClip;
    private AudioSource source;


    private GM gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GM>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if left mouse button is pressed to shoot
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (canFire && ammo > 0)
        {
            source.PlayOneShot(hitClip);
            ammo--;
            UpdateUI();
            // Play the shooting particle effect
            shootParticle.Play();
            // Apply a punch animation to the gun's position
            gun.DOPunchPosition(new Vector3(0, 0, -0.2f), 0.2f, 0);
            gun.DOPunchRotation(new Vector3(-10, 0, 0), 0.2f, 0, 0.3f);

            canFire = false;
            // Start the reload coroutine to enable firing after a delay
            StartCoroutine(Reload());

            // Perform a raycast to detect what the bullet hits
            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, 200))
            {
                Instantiate(hitParticle, hit.point + hit.normal * 0.1f, Quaternion.identity);

                Enemy e = hit.collider.GetComponent<Enemy>();
                if (e)
                {
                    // If enemy component found, apply damage to enemy
                    e.TakeDamage(damage);
                }

                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb)
                {
                    // If rigidbody found, apply force at the hit point in the opposite direction of the hit normal
                    rb.AddForceAtPosition(-hit.normal * force, hit.point);
                }
            }
        }
        else if (canFire && ammo == 0)
        {
            source.PlayOneShot(emptyClip);
            canFire = false;
            StartCoroutine(Reload());
        }
    }

    // Coroutine for reloading the gun after firing
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        Enemy e = collision.collider.GetComponent<Enemy>();
        // Check if Enemy component exists and enemy is not dead
        if (e && !e.dead)
        {
            // Decrease health of the gun by enemy's damage per second multiplied by delta time
            health = Mathf.Max(0, health - e.damage * Time.deltaTime);
            UpdateUI();

            if (health == 0)
            {
                print("dead");
                gm.Lose();
            }
        }
    }

    private void UpdateUI()
    {
        gm.UpdateStats(health, 100, ammo, maxAmmo);
    }

    public void AmmoPickUp (int size)
    {
        ammo = Mathf.Min(maxAmmo, ammo + size);
        UpdateUI();
    }

    public void Upgrade(int stage)
    {
        if (stage == 1)
        {
            fireRate = upgradeRate;
        }
        else if (stage == 2)
        {
            maxAmmo = upgradeAmmo;
        }
        health = 100;
        ammo = maxAmmo;
        UpdateUI();
    }
}
