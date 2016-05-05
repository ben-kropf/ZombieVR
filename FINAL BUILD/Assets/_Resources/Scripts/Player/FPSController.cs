using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour
{

    // FPS Kit [www.armedunity.com]

    public int proneSpeed = 1;
    public int crouchSpeed = 4;
    public int walkSpeed = 6;
    public int runSpeed = 10;
    public float jumpSpeed = 8.0f;
    private float gravity = 24;
    public float baseGravity = 24;
    public float proneGravity = 15;
    private float normalFDTreshold = 8;
    private float proneFDTreshold = 4;
    private float fallingDamageThreshold;
    public float fallDamageMultiplier = 5.0f;
    private float slideSpeed = 8.0f;
    private float antiBumpFactor = .75f;
    private float antiBunnyHopFactor = 1;
    public bool airControl = false;

    [HideInInspector]
    public bool run;
    [HideInInspector]
    public bool canRun = true;

    public Transform cameraGO;
    [HideInInspector]
    Vector3 moveDirection= Vector3.zero;
[HideInInspector]
    public bool grounded = false;
    private Transform myTransform;
    [HideInInspector]
    public float speed;
    private RaycastHit hit;
    private float fallDistance;
    private bool falling = false;
    public float slideLimit = 45.0f;
    public float rayDistance;
    private Vector3 contactPoint;
    private int jumpTimer;
    private float normalHeight = 0.9f;
    private float crouchHeight = 0.2f;
    private float proneHeight = -0.4f;

    [HideInInspector]
    public int state = 0;
    // 0 = standing
    // 1 = crouching
    // 2 = prone

    private float adjustAnimSpeed = 7.0f;

    //Ladders
    [HideInInspector]
    public bool onLadder = false;
    public UseLadder useladder;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private float highestPoint;
    public HealthScript hs;
    public WeaponManager wm;
    public FootSteps footsteps;

    private float crouchProneSpeed = 3;
    private float distanceToObstacle;

    private bool sliding = false;
    [HideInInspector]
    public float velMagnitude;

    public CharacterController controller;
    public Animation walkRunAnim;
    public Animation cameraAnimations;
    private string runAnimation = "CameraRun";
    private string idleAnimation = "IdleAnimation";

    // WATER
    public bool underWater = false;
    public bool swimming = false;
    private float swimAccel;
    private float underWaterTimer = 0.0f;
    public UnityStandardAssets.ImageEffects.BlurOptimized blur;
    private float waterLevel;
    private float underwaterLevel;
    public AudioSource aSource;
    public AudioSource waterSource;
    public AudioClip enterPool;
    public AudioClip enterPoolSplash;
    public AudioClip bodyHitSound;
    public AudioClip inhale;

    public Transform fallEffect;
    public Transform fallEffectWep;

    public GameObject waterSplash;
    public ParticleEmitter waterFoam;
    private ParticleEmitter emiter;

    void Start()
    {

        myTransform = transform;
        rayDistance = controller.height / 2 + 1.1f;
        slideLimit = controller.slopeLimit - .2f;
        walkRunAnim.wrapMode = WrapMode.Loop;
        walkRunAnim.Stop();
        cameraAnimations[runAnimation].speed = 0.8f;
    }

    void Update()
    {

        velMagnitude = controller.velocity.magnitude;
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? .7071f : 1.0f;

        if (onLadder)
        {
            useladder.LadderUpdate();
            highestPoint = myTransform.position.y;
            run = false;
            fallDistance = 0.0f;
            grounded = false;
            walkRunAnim.CrossFade(idleAnimation);
            cameraAnimations.CrossFade(idleAnimation);
            return;
        }

        if (swimming)
        {
            highestPoint = myTransform.position.y;
            run = false;
            fallDistance = 0.0f;
            grounded = false;
            state = 2;
            walkRunAnim.CrossFade(idleAnimation);
            cameraAnimations.CrossFade(idleAnimation);
        }

        if (grounded)
        {
            gravity = baseGravity;

            if (Physics.Raycast(myTransform.position, -Vector3.up, out hit, rayDistance))
            {
                float hitangle = Vector3.Angle(hit.normal, Vector3.up);
                if (hitangle > slideLimit)
                {
                    sliding = true;
                }
                else
                {
                    sliding = false;
                }
            }

            if (canRun && state == 0)
            {
                if (Input.GetButton("Run") && Input.GetKey("w") && !Input.GetButton("Fire2"))
                {
                    run = true;
                }
                else
                {
                    run = false;
                }
            }

            if (falling)
            {
                if (state == 2)
                    fallingDamageThreshold = proneFDTreshold;
                else
                    fallingDamageThreshold = normalFDTreshold;

                falling = false;
                fallDistance = highestPoint - currentPosition.y;
                if (fallDistance > fallingDamageThreshold)
                {
                    ApplyFallingDamage(fallDistance);
                }

                if (fallDistance < fallingDamageThreshold && fallDistance > 0.1f)
                {
                    if (state < 2) footsteps.JumpLand();
                    else if (bodyHitSound) aSource.PlayOneShot(bodyHitSound, 0.5f);

                    StartCoroutine(FallCamera(new Vector3(7, Random.Range(-1.0f, 1.0f), 0), new Vector3(3, Random.Range(-0.5f, 0.5f), 0), 0.15f));
                }
            }

            if (sliding)
            {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize( ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
            }
            else
            {

                if (state == 0)
                {
                    if (run)
                        speed = runSpeed;
                    else
                        if (Input.GetButton("Fire2"))
                    {
                        speed = crouchSpeed;
                    }
                    else
                    {
                        speed = walkSpeed;
                    }

                }
                else if (state == 1)
                {
                    speed = crouchSpeed;
                    run = false;
                }
                else if (state == 2)
                {
                    speed = proneSpeed;
                    run = false;
                }
                if (Cursor.lockState == CursorLockMode.Locked)
                    moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
                else
                    moveDirection = new Vector3(0, -antiBumpFactor, 0);

                moveDirection = myTransform.TransformDirection(moveDirection);
                moveDirection *= speed;

                if (!Input.GetButton("Jump"))
                {
                    jumpTimer++;
                }
                else if (jumpTimer >= antiBunnyHopFactor)
                {
                    jumpTimer = 0;
                    if (state == 0)
                    {
                        moveDirection.y = jumpSpeed;
                    }

                    if (state > 0)
                    {
                        CheckDistance();
                        if (distanceToObstacle > 1.6f)
                        {
                            state = 0;
                        }
                    }
                }
            }

        }
        else
        {

            currentPosition = myTransform.position;
            if (currentPosition.y > lastPosition.y)
            {
                highestPoint = myTransform.position.y;
                falling = true;
            }

            if (!falling)
            {
                highestPoint = myTransform.position.y;
                falling = true;
            }

            if (airControl)
            {
                moveDirection.x = inputX * speed;
                moveDirection.z = inputY * speed;
                moveDirection = myTransform.TransformDirection(moveDirection);
            }

            if (swimming)
            {
                if (swimAccel > 0.0f)
                    swimAccel -= Time.deltaTime * 4.0f;

                if (Input.GetButton("Run"))
                {
                    Vector3 swimDir = cameraGO.transform.TransformDirection(Vector3.forward);

                    if (transform.position.y >= waterLevel && swimDir.y > 0.0f)
                        swimDir.y = 0.0f;

                    if (swimAccel <= 1.0f)
                    {
                        StartCoroutine(FallCamera( new Vector3(7, Random.Range(-5.0f, 5.0f), 0), new Vector3(4, 0, 0), 0.15f));
                        swimAccel = 6.0f;
                    }

                    if (swimAccel > 1.0f)
                        moveDirection = swimDir * swimAccel;

                }
                else
                {
                    moveDirection.x = inputX * 2.0f;
                    moveDirection.z = inputY * 2.0f;
                    moveDirection = myTransform.TransformDirection(moveDirection);
                }

                if (underWater)
                {
                    underWaterTimer += Time.deltaTime;

                    if (underWaterTimer > 15.0f)
                    {
                        StartCoroutine(FallCamera(new Vector3(Random.Range(-2.0f, 5.0f), Random.Range(-7.0f, 7.0f), 0),new Vector3(4, Random.Range(-2.0f, 2.0f), 0), 0.1f));
                        hs.PlayerDamage(10);
                        underWaterTimer = 12.0f;
                    }
                }
            }
        }

        if (grounded)
        {
            if (velMagnitude > crouchSpeed && !run)
            {
                walkRunAnim["Walk"].speed = velMagnitude / adjustAnimSpeed;
                walkRunAnim.CrossFade("Walk");
            }
            else
            {
                walkRunAnim.CrossFade(idleAnimation);
            }

            if (run && velMagnitude > walkSpeed)
            {
                walkRunAnim.CrossFade("Run");
                cameraAnimations.CrossFade(runAnimation);
            }
            else
            {
                cameraAnimations.CrossFade(idleAnimation);
            }

        }
        else
        {
            walkRunAnim.CrossFade(idleAnimation);
            cameraAnimations.CrossFade(idleAnimation);
            run = false;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            CheckDistance();

            if (state == 0)
            {
                state = 1;

            }
            else if (state == 1)
            {
                if (distanceToObstacle > 1.6f)
                {
                    state = 0;
                }
            }
            else if (state == 2)
            {
                if (distanceToObstacle > 1)
                {
                    state = 1;
                }
            }
        }

        if (Input.GetButtonDown("Prone"))
        {
            CheckDistance();
            if (state == 0 || state == 1)
            {
                state = 2;
            }
            else if (state == 2)
            {
                if (distanceToObstacle > 1.6f)
                {
                    state = 0;
                }
            }

            if (!grounded) gravity = proneGravity;
        }

        if (state == 0)
        { //Stand Position
            controller.height = 2.0f;
            controller.center = new Vector3(0, 0, 0);

            if (cameraGO.localPosition.y > normalHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x, normalHeight, cameraGO.localPosition.z);
            }
            else if (cameraGO.localPosition.y < normalHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x,cameraGO.localPosition.y + Time.deltaTime * crouchProneSpeed, cameraGO.localPosition.z) ;
            }

        }
        else if (state == 1)
        { //Crouch Position

            controller.height = 1.4f;
            controller.center = new Vector3(0, -0.3f, 0);
            if (cameraGO.localPosition.y != crouchHeight)
            {
                if (cameraGO.localPosition.y > crouchHeight)
                {
                    cameraGO.localPosition = new Vector3(cameraGO.localPosition.x,cameraGO.localPosition.y - Time.deltaTime * crouchProneSpeed, cameraGO.localPosition.z) ;
                }
                if (cameraGO.localPosition.y < crouchHeight)
                {
                    cameraGO.localPosition = new Vector3(cameraGO.localPosition.x, cameraGO.localPosition.y + Time.deltaTime * crouchProneSpeed, cameraGO.localPosition.z) ;
                }

            }

        }
        else if (state == 2)
        { //Prone Position

            controller.height = 0.6f;
            controller.center = new Vector3(0, -0.7f, 0);

            if (cameraGO.localPosition.y < proneHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x,proneHeight,cameraGO.localPosition.z);
            }
            else if (cameraGO.localPosition.y > proneHeight)
            {
                cameraGO.localPosition = new Vector3(cameraGO.localPosition.x, cameraGO.localPosition.y - Time.deltaTime * crouchProneSpeed, cameraGO.localPosition.z); 
            }
        }

        if (!swimming)
            moveDirection.y -= gravity * Time.deltaTime;

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;
    }

    void CheckDistance()
    {
        Vector3 pos = myTransform.position + controller.center - new Vector3(0, controller.height / 2, 0);
        RaycastHit hit;
        if (Physics.SphereCast(pos, controller.radius, myTransform.up, out hit, 10))
        {
            distanceToObstacle = hit.distance;
            Debug.DrawLine(pos, hit.point, Color.red, 2.0f);
        }
        else
        {
            distanceToObstacle = 3;
        }
    }

    void LateUpdate()
    {
        lastPosition = currentPosition;

        if (swimming)
        {
            moveDirection.y = 0.5f + (underwaterLevel - transform.position.y);

            //if(transform.position.y > waterLevel && !grounded)
            //	transform.position.y = waterLevel;

            if (emiter)
                emiter.emit = !underWater;


            if (transform.position.y > underwaterLevel)
            {

                if (underWaterTimer > 5.0f)
                {
                    GetComponent<AudioSource>().clip = inhale;
                    GetComponent<AudioSource>().Play();
                }
                underWater = false;
                underWaterTimer = 0.0f;
                blur.enabled = false;
                aSource.Stop();
            }

            if (transform.position.y > waterLevel && grounded)
            {
                swimming = false;
                if (emiter)
                {
                    emiter.emit = false;
                    emiter.transform.parent = null;
                }
                wm.ExitWater();
                state = 0;
            }
        }
    }

    IEnumerator FallCamera(Vector3 d, Vector3 dw, float ta)
    {
        Quaternion s = fallEffect.localRotation;
        Quaternion sw = fallEffectWep.localRotation;
        Quaternion e = fallEffect.localRotation * Quaternion.Euler(d);
       // Quaternion ew = fallEffectWep.localRotation * Quaternion.Euler(dw);
        float r = 1.0f / ta;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * r;
            fallEffect.localRotation = Quaternion.Slerp(s, e, t);
            fallEffectWep.localRotation = Quaternion.Slerp(sw, e, t);
            yield return null;
        }
    }


    void PlayerInWater(float s)
    {
        waterLevel = s + 0.9f;
        underwaterLevel = s + 0.4f;
        blur.enabled = true;

        if (GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();

        if (!aSource.isPlaying)
            aSource.Play();


        if (!swimming)
        {
            wm.EnterWater();

            if (grounded)
            {
                Instantiate(waterSplash, transform.position + new Vector3(0, -0.5f, 0), transform.rotation);
                waterSource.PlayOneShot(enterPool, 1.0f);
            }
            else
            {
                Instantiate(waterSplash, transform.position + new Vector3(0, 0.5f, 0.2f), transform.rotation);
                Instantiate(waterSplash, transform.position, transform.rotation);
                Instantiate(waterSplash, transform.position + new Vector3(0, -0.5f, 0.5f), transform.rotation);
                waterSource.PlayOneShot(enterPoolSplash, 1.0f);
            }

            emiter = Instantiate(waterFoam, transform.position + new Vector3(0, -0.7f, 0), transform.rotation) as ParticleEmitter;
            emiter.transform.parent = this.transform;
        }

        underWater = true;
        swimming = true;
    }

    void ApplyFallingDamage(float fallDistance)
    {
        hs.PlayerFallDamage(fallDistance * fallDamageMultiplier);
        if (state < 2) StartCoroutine(footsteps.JumpLand());
        StartCoroutine( FallCamera(new Vector3(12, Random.Range(-2.0f, 2.0f), 0), new Vector3(4, Random.Range(-1.0f, 1.0f), 0), 0.1f));
    }

    public void OnLadder()
    {
        onLadder = true;
        moveDirection = Vector3.zero;
        grounded = false;
    }

    public void OffLadder(Vector3 ladderMovement)
    {
        onLadder = false;
        Vector3 dir = gameObject.transform.forward;
        if (Input.GetAxis("Vertical") > 0)
        {
            moveDirection = dir.normalized * 5.0f;
        }
    }

}