using UnityEngine;

public class HelicopterHealth : MonoBehaviour
{
    // FPS KIT [www.armedunity.com]

    public float maximumHitPoints = 100.0f;
    public float hitPoints = 100.0f;
    public Rigidbody deadReplacement;
    public float detonationDelay = 0.0f;
    public Transform explosion;
    public GameObject particle;
    public HelicopterController controllScript;
    public GUISkin mySkin;
    private bool destroyed = false;

    private void Update()
    {
        if (hitPoints <= maximumHitPoints / 2)
        {
            particle.GetComponent<ParticleEmitter>().emit = true;
            particle.GetComponent<AudioSource>().enabled = true;
            controllScript.disabled = true;
            hitPoints -= Time.deltaTime * 20;

            if (hitPoints <= 0.0f)
                Invoke("Detonate", detonationDelay);
        }
    }

    private void ApplyDamage(float damage)
    {
        if (hitPoints <= 0.0f)
            return;

        // Apply damage
        hitPoints -= damage;

        // Are we dead?
        if (hitPoints <= 0.0f)
            Invoke("Detonate", detonationDelay);
    }

    private void Detonate()
    {
        if (destroyed) return;

        destroyed = true;

        Instantiate(explosion, transform.position, transform.rotation);
        gameObject.SetActive(false);

        // If we have a dead barrel then replace ourselves with it!
        if (deadReplacement)
        {
            Instantiate(deadReplacement, transform.position, transform.rotation);
        }
        // Destroy ourselves
        Destroy(transform.parent.gameObject);
    }

    private void OnGUI()
    {
        GUI.skin = mySkin;
        GUIStyle style1 = mySkin.customStyles[0];

        if (controllScript.controlsEnabled)
        {
            GUI.Label(new Rect(40, Screen.height - 50, 150, 80), " Helicopter Health: ");
            GUI.Label(new Rect(170, Screen.height - 50, 150, 80), "" + hitPoints, style1);
        }
    }
}