using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayWeapon : MonoBehaviour
{
    GameObject player;
    public bool isFiring = false;
    public ParticleSystem flashTir, hitEffect;
    public TrailRenderer tracerEffect;
    public Transform rayOrigin;
    public Transform raycastAimTarget;

    //Attributs d'arme
    public float weaponDamage = 12 ;

    //Raycast of Weapon
    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        player = transform.parent.gameObject;
    }

    public void StartFiring()
    {
        isFiring = true;
        flashTir.Emit(1);

        ray.origin = rayOrigin.position;
        ray.direction = (raycastAimTarget.position - rayOrigin.position).normalized;

        TrailRenderer tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1.0f);
            if (hit.transform.gameObject.layer == 3)
            { 
                HealthBar bar = hit.transform.GetComponent<HealthBar>();
                if (player.activeSelf && bar)
                    bar.TakeDamage(player, weaponDamage);
            }


            hitEffect.transform.position = hit.point;
            hitEffect.transform.forward = hit.normal;
            hitEffect.Emit(1);

            tracer.transform.position = hit.point;
        } 
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
