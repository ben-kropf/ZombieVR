using UnityEngine;
using System.Collections;

public class LandMineDamage : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public GameObject explosion;
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
            Explosion();
    }

    IEnumerator ApplyDamage()
    {
        yield return new WaitForSeconds(.2f);
        Explosion();
    }

    void Explosion()
    {
        if (activated) return;
        activated = true;

        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }


}