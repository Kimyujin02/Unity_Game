using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;
    public AudioClip fireSfx;
    private new AudioSource audio;
    // MuzzleFlash
    public MeshRenderer muzzleFlash;
    private RaycastHit hit;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
    }

    void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * 100.0f, Color.green);
        if (Input.GetMouseButtonDown(0))
        {
            Fire();

            if (Physics.Raycast(firePos.position, firePos.forward, out hit, 100.0f))
            {
                // Log the hit object name for debugging purposes.
                Debug.Log($"Hit = {hit.transform.name}");

                // Safely get the MonsterCtrl component and call OnDamage if it exists.
                var monsterCtrl = hit.transform.GetComponent<MonsterCtrl>();
                if (monsterCtrl != null)
                {
                    monsterCtrl.OnDamage(hit.point, hit.normal);
                }
            }
        }
    }

    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
        // Play fire sound effect
        audio.PlayOneShot(fireSfx, 1.0f);
        // Show muzzle flash effect
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        // Set random offset for muzzle flash texture
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;

        // Set random rotation for muzzle flash
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        // Set random scale for muzzle flash
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        // Enable muzzle flash
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.2f);
        // Disable muzzle flash
        muzzleFlash.enabled = false;
    }
}
