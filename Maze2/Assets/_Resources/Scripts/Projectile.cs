using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    private float projectileSpeed = 30;
    private float projectileGravity = 0.0f;
    private float destroyAfter = 5.0f;

    private Vector3 newPos = Vector3.zero;
    private Vector3 oldPos = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;
   

    public LayerMask layerMask;

    private float timer;
    private Transform myTransform;

    public GameObject smoke;
    public GameObject explosion;

    public void SetUp(float[] info)
    {
        myTransform = transform;
        timer = destroyAfter;

        projectileSpeed = info[0];
        projectileGravity = info[1];
        moveDir = myTransform.TransformDirection(new Vector3(0, 0, 1));

        newPos = myTransform.position;
        oldPos = newPos;
    }

    void Update()
    {

        newPos += moveDir * (Time.deltaTime * projectileSpeed);
		RaycastHit hit;
        Vector3 dir = newPos - oldPos;
        float dist = dir.magnitude;
        dir /= dist;
        if (dist > 0)
        {
            if (Physics.Raycast(oldPos, dir, out hit, dist, layerMask))
            {
                if (explosion != null)
                    Instantiate(explosion, transform.position, Quaternion.identity);

                DestroyProjectile();
            }
        }

        oldPos = myTransform.position;
        myTransform.position = newPos;
        moveDir.y -= projectileGravity * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0.0f)
            DestroyProjectile();
    }

    void DestroyProjectile()
    {
        smoke.GetComponent<Destroyer>().DestroyNow();
        smoke.transform.parent = null;

        Destroy(gameObject);
    }
}