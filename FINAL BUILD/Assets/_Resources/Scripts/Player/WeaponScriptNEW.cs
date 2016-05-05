using UnityEngine;
using System.Collections;

public enum fireMode { none, semi, auto, burst, shotgun, launcher }
public enum Ammo { Magazines, Bullets }
public enum Aim { Simple, Sniper }

public class WeaponScriptNEW : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]


    [HideInInspector]
    public fireMode currentMode = fireMode.semi;
    public fireMode firstMode = fireMode.semi;
    public fireMode secondMode = fireMode.launcher;
    public Ammo ammoMode = Ammo.Magazines;
    public Aim aimMode = Aim.Simple;

    [Header("Weapon configuration")]
    public LayerMask layerMask;
    public int damage = 50;
    public int bulletsPerMag = 50;
    public int magazines = 5;
    private float fireRate = 0.1f;
    public float fireRateFirstMode = 0.1f;
    public float fireRateSecondMode = 0.1f;
    public float range = 250.0f;
    public float force = 200.0f;

    [Header("Accuracy Settings")]
    public float baseInaccuracyAIM = 0.005f;
    public float baseInaccuracyHIP = 1.5f;
    public float inaccuracyIncreaseOverTime = 0.2f;
    public float inaccuracyDecreaseOverTime = 0.5f;
    private float maximumInaccuracy;
    public float maxInaccuracyHIP = 5.0f;
    public float maxInaccuracyAIM = 1.0f;
    private float triggerTime = 0.05f;
    private float baseInaccuracy;

    [Header("Aiming")]
    public Vector3 aimPosition;
    private bool aiming;
    private Vector3 curVect;
    private Vector3 hipPosition = Vector3.zero;
    public float aimSpeed = 0.25f;
    public float zoomSpeed = 0.5f;
    public int FOV = 40;
    public int weaponFOV = 45;

    private float scopeTime;
    private bool inScope = false;
    public Texture scopeTexture;

    [Header("Burst Settings")]
    public int shotsPerBurst = 3;
    public float burstTime = 0.07f;

    [Header("Shotgun Settings")]
    public int pelletsPerShot = 10;

    [Header("Launcher")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 30.0f;
    public float projectileGravity = 0.0f;
    public int projectiles = 20;
    public Transform launchPosition;
    public bool reloadProjectile = false;
    public AudioClip soundReloadLauncher;
    public Renderer rocket = null;

    [Header("Kickback")]
    public Transform kickGO;
    public float kickUp = 0.5f;
    public float kickSideways = 0.5f;

    [Header("Crosshair")]
    public Texture2D crosshairFirstModeHorizontal;
    public Texture2D crosshairFirstModeVertical;
    public Texture2D crosshairSecondMode;
    private float adjustMaxCroshairSize = 6.0f;

    [Header("Bulletmarks")]
    public GameObject Concrete;
    public GameObject Wood;
    public GameObject Metal;
    public GameObject Dirt;
    public GameObject Blood;
    public GameObject Water;
    public GameObject Untagged;

    [Header("Audio")]
    public AudioSource aSource;
    public AudioClip soundDraw;
    public AudioClip soundFire;
    public AudioClip soundReload;
    public AudioClip soundEmpty;
    public AudioClip switchModeSound;

    [Header("Animation Settings")]
    public Animation weaponAnim;
    public string fireAnim = "Fire";
    [Range(0.0f, 5.0f)]
    public float fireAnimSpeed = 1.0f;
    public string drawAnim = "Draw";
    [Range(0.0f, 5.0f)]
    public float drawAnimSpeed = 1.0f;
    [Range(0.0f, 5.0f)]
    public float drawTime = 1.5f;
    public string reloadAnim = "Reload";
    [Range(0.0f, 5.0f)]
    public float reloadAnimSpeed = 1.0f;
    [Range(0.0f, 5.0f)]
    public float reloadTime = 1.5f;
    public string fireEmptyAnim = "FireEmpty";
    [Range(0.0f, 5.0f)]
    public float fireEmptyAnimSpeed = 1.0f;
    public string switchAnim = "SwitchAnim";
    [Range(0.0f, 5.0f)]
    public float switchAnimSpeed = 1.0f;
    public string fireLauncherAnim = "FireLauncher";

    [Header("Other")]
    public FPSController fpscontroller;
    public WeaponManager wepManager;
    public GUISkin mySkin;
    public Renderer muzzleFlash;
    public Light muzzleLight;
    public Camera mainCamera;
    public Camera wepCamera;
    public bool withSilencer = false;

    [HideInInspector]
    public bool reloading = false;
    [HideInInspector]
    public bool selected = false;
    private bool canSwicthMode = true;
    private bool draw;
    private bool playing = false;
    private bool isFiring = false;
    private bool bursting = false;
    private int m_LastFrameShot = -10;
    private float nextFireTime = 0.0f;
    private int bulletsLeft = 0;
    private RaycastHit hit;
    private float camFOV = 60.0f;

    void Start()
    {
        //camFOV = mainCamera.fieldOfView;
        muzzleFlash.enabled = false;
        muzzleLight.enabled = false;
        bulletsLeft = bulletsPerMag;
        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        aiming = false;

        if (ammoMode == Ammo.Bullets)
        {
            magazines = magazines * bulletsPerMag;
        }
    }

    void Update()
    {
        if (selected)
        {
            if (Cursor.lockState == CursorLockMode.None) return;

            if (Input.GetButtonDown("Fire"))
            {
                if (currentMode == fireMode.semi)
                {
                    FireSemi();
                }
                else if (currentMode == fireMode.launcher)
                {
                    FireLauncher();
                }
                else if (currentMode == fireMode.burst)
                {
                   StartCoroutine(FireBurst());
                }
                else if (currentMode == fireMode.shotgun)
                {
                    FireShotgun();
                }

                if (bulletsLeft > 0)
                    isFiring = true;
            }

            if (Input.GetButton("Fire"))
            {
                if (currentMode == fireMode.auto)
                {
                    FireSemi();
                    if (bulletsLeft > 0)
                        isFiring = true;
                }
            }

            if (Input.GetButtonDown("Reload"))
            {
                StartCoroutine(Reload());
            }
        }

        if (Input.GetButton("Fire2") && !reloading && selected)
        {
            if (!aiming)
            {
                aiming = true;
                curVect = aimPosition - transform.localPosition;
                scopeTime = Time.time + aimSpeed;
            }
            if (transform.localPosition != aimPosition && aiming)
            {
                if (Mathf.Abs(Vector3.Distance(transform.localPosition, aimPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
                {
                    transform.localPosition = aimPosition;
                }
                else
                {
                    transform.localPosition += curVect / aimSpeed * Time.deltaTime;
                }
            }

            if (aimMode == Aim.Sniper)
            {
                if (Time.time >= scopeTime && !inScope)
                {
                    inScope = true;
                    Component[] gos = GetComponentsInChildren<Renderer>();
                    foreach (var go in gos)
                    {
                        Renderer a = go as Renderer;
                        a.enabled = false;
                    }
                }
            }

        }
        else
        {
            if (aiming)
            {
                aiming = false;
                inScope = false;
                curVect = hipPosition - transform.localPosition;
                if (aimMode == Aim.Sniper)
                {
                    Component[] go = GetComponentsInChildren<Renderer>();
                    foreach (var g in go)
                    {
                        Renderer b = g as Renderer;
                        if (b.name != "muzzle_flash")
                            b.enabled = true;
                    }
                }
            }

            if (Mathf.Abs(Vector3.Distance(transform.localPosition, hipPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
            {
                transform.localPosition = hipPosition;
            }
            else
            {
                transform.localPosition += curVect / aimSpeed * Time.deltaTime;
            }
        }

        if (aiming)
        {
            maximumInaccuracy = maxInaccuracyAIM;
            baseInaccuracy = baseInaccuracyAIM;
            mainCamera.fieldOfView -= FOV * Time.deltaTime / zoomSpeed;
            if (mainCamera.fieldOfView < FOV)
            {
                mainCamera.fieldOfView = FOV;
            }

        }
        else
        {
            maximumInaccuracy = maxInaccuracyHIP;
            baseInaccuracy = baseInaccuracyHIP;
            mainCamera.fieldOfView += camFOV * Time.deltaTime * 3;
            if (mainCamera.fieldOfView > camFOV)
            {
                mainCamera.fieldOfView = camFOV;
            }
        }

        if (fpscontroller.velMagnitude > 3.0f)
        {
            triggerTime += inaccuracyDecreaseOverTime;
        }

        if (isFiring)
        {
            triggerTime += inaccuracyIncreaseOverTime;
        }
        else
        {
            if (fpscontroller.velMagnitude < 3.0f)
                triggerTime -= inaccuracyDecreaseOverTime;
        }

        if (triggerTime >= maximumInaccuracy)
        {
            triggerTime = maximumInaccuracy;
        }

        if (triggerTime <= baseInaccuracy)
        {
            triggerTime = baseInaccuracy;
        }

        if (nextFireTime > Time.time)
        {
            isFiring = false;
        }

        if (Input.GetButtonDown("switchFireMode") && secondMode != fireMode.none && canSwicthMode)
        {
            if (currentMode != firstMode)
            {
               StartCoroutine(FirstFireMode());
            }
            else
            {
                StartCoroutine(SecondFireMode());
            }
        }
    }

    void LateUpdate()
    {
        if (withSilencer || inScope) return;

        if (m_LastFrameShot == Time.frameCount)
        {
            muzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            muzzleFlash.enabled = true;
            muzzleLight.enabled = true;
        }
        else
        {
            muzzleFlash.enabled = false;
            muzzleLight.enabled = false;
        }
    }

    void OnGUI()
    {
        if (selected)
        {
            GUI.skin = mySkin;
            GUIStyle style1 = mySkin.customStyles[0];

            if (scopeTexture != null && inScope)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), scopeTexture, ScaleMode.StretchToFill);

            }
            else
            {

                if (currentMode == fireMode.launcher)
                {
                    if (crosshairSecondMode == null) return;

                    float wi = crosshairSecondMode.width / 2;
                    float he = crosshairSecondMode.height / 2;
                    Rect pos = new Rect((Screen.width - wi) / 2, (Screen.height - he) / 2, wi, he);
                    if (!aiming)
                    {
                        GUI.DrawTexture(pos, crosshairSecondMode);
                    }
                }
                else
                {
                    float w = crosshairFirstModeHorizontal.width;
                    float h = crosshairFirstModeHorizontal.height;
                    Rect position1 = new Rect((Screen.width + w) / 2 + (triggerTime * adjustMaxCroshairSize), (Screen.height - h) / 2, w, h);
                    Rect position2 = new Rect((Screen.width - w) / 2, (Screen.height + h) / 2 + (triggerTime * adjustMaxCroshairSize), w, h);
                    Rect position3 = new Rect((Screen.width - w) / 2 - (triggerTime * adjustMaxCroshairSize) - w, (Screen.height - h) / 2, w, h);
                    Rect position4 = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2 - (triggerTime * adjustMaxCroshairSize) - h, w, h);
                    if (!aiming)
                    {
                        GUI.DrawTexture(position1, crosshairFirstModeHorizontal);   //Right
                        GUI.DrawTexture(position2, crosshairFirstModeVertical);     //Up
                        GUI.DrawTexture(position3, crosshairFirstModeHorizontal);   //Left
                        GUI.DrawTexture(position4, crosshairFirstModeVertical);     //Down
                    }
                }
            }

            if (firstMode != fireMode.none && firstMode != fireMode.launcher || secondMode != fireMode.none && secondMode != fireMode.launcher)
            {
                GUI.Label(new Rect(Screen.width - 200, Screen.height - 35, 200, 80), "Bullets : ");
                GUI.Label(new Rect(Screen.width - 110, Screen.height - 35, 200, 80), "" + bulletsLeft, style1);
                GUI.Label(new Rect(Screen.width - 80, Screen.height - 35, 200, 80), " |  " + magazines);
            }

            if (firstMode != fireMode.none || secondMode != fireMode.none)
            {
                GUI.Label(new Rect(Screen.width - 200, Screen.height - 65, 200, 80), "Firing Mode :");
                GUI.Label(new Rect(Screen.width - 110, Screen.height - 65, 200, 80), "" + currentMode, style1);
            }

            if (firstMode == fireMode.launcher || secondMode == fireMode.launcher)
            {
                GUI.Label(new Rect(Screen.width - 200, Screen.height - 95, 200, 80), "Projectiles : ");
                GUI.Label(new Rect(Screen.width - 110, Screen.height - 95, 200, 80), "" + projectiles, style1);
            }
        }
    }

    IEnumerator FirstFireMode()
    {

        canSwicthMode = false;
        selected = false;
        weaponAnim.Rewind(switchAnim);
        weaponAnim.Play(switchAnim);
        aSource.clip = switchModeSound;
        aSource.Play();
        yield return new WaitForSeconds(0.6f);
        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        selected = true;
        canSwicthMode = true;
    }

    IEnumerator SecondFireMode()
    {

        canSwicthMode = false;
        selected = false;
        aSource.clip = switchModeSound;
        aSource.Play();
        weaponAnim.Play(switchAnim);
        yield return new WaitForSeconds(0.6f);
        currentMode = secondMode;
        fireRate = fireRateSecondMode;
        selected = true;
        canSwicthMode = true;
    }

    void FireSemi()
    {
        if (reloading || bulletsLeft <= 0)
        {
            if (bulletsLeft == 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            return;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            FireOneBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireLauncher()
    {
        if (reloading || projectiles <= 0)
        {
            if (projectiles == 0)
            {
               StartCoroutine( OutOfAmmo());
            }
            return;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator FireBurst()
    {
        int shotCounter = 0;

        if (reloading || bursting || bulletsLeft <= 0)
        {
            if (bulletsLeft <= 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            yield break;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            while (shotCounter < shotsPerBurst)
            {
                bursting = true;
                shotCounter++;
                if (bulletsLeft > 0)
                {
                    FireOneBullet();
                }
                yield return new WaitForSeconds(burstTime);
            }
            nextFireTime = Time.time + fireRate;
        }
        bursting = false;
    }

    void FireShotgun()
    {
        if (reloading || bulletsLeft <= 0 || draw)
        {
            if (bulletsLeft == 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            return;
        }

        int pellets = 0;

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            while (pellets < pelletsPerShot)
            {
                FireOnePellet();
                pellets++;
            }
            bulletsLeft--;
            nextFireTime = Time.time + fireRate;
        }

        weaponAnim.Rewind(fireAnim);
        weaponAnim.Play(fireAnim);

        aSource.PlayOneShot(soundFire, 1.0f);

        m_LastFrameShot = Time.frameCount;
        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUp, Random.Range(-kickSideways, kickSideways), 0));
    }

    void FireOneBullet()
    {
        if (nextFireTime > Time.time || draw)
        {
            if (bulletsLeft <= 0)
            {
               StartCoroutine(OutOfAmmo());
            }
            return;
        }

        Vector3 dir = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * triggerTime, Random.Range(-0.01f, 0.01f) * triggerTime, 1));
        Vector3 pos = transform.parent.position;

        if (Physics.Raycast(pos, dir, out hit, range, layerMask))
        {

            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float rScale = Random.Range(0.5f, 1.0f);

            if (hit.rigidbody)
                hit.rigidbody.AddForceAtPosition(force * dir, hit.point);

            if (hit.collider.tag == "Concrete")
            {
                GameObject concMark = Instantiate(Concrete, contact, rot) as GameObject;
                concMark.transform.localPosition += .02f * hit.normal;
                concMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                concMark.transform.parent = hit.transform;

            }
            //ADDED
            else if (hit.collider.tag == "Zombie")
            {
                print("HIT COLLIDER");
                Instantiate(Blood, contact, rot);
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            else if (hit.collider.tag == "Enemy")
            {
                Instantiate(Blood, contact, rot);
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            else if (hit.collider.tag == "Damage")
            {
                Instantiate(Blood, contact, rot);
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            else if (hit.collider.tag == "Wood")
            {
                GameObject woodMark = Instantiate(Wood, contact, rot) as GameObject;
                woodMark.transform.localPosition += .02f * hit.normal;
                woodMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                woodMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Metal")
            {
                GameObject metalMark = Instantiate(Metal, contact, rot) as GameObject;
                metalMark.transform.localPosition += .02f * hit.normal;
                metalMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                metalMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Dirt" || hit.collider.tag == "Grass")
            {
                GameObject dirtMark = Instantiate(Dirt, contact, rot) as GameObject;
                dirtMark.transform.localPosition += .02f * hit.normal;
                dirtMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                dirtMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Water")
            {
                Instantiate(Water, contact, rot);

            }
            else if (hit.collider.tag == "Usable")
            {
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            else
            {
                GameObject def = Instantiate(Untagged, contact, rot) as GameObject;
                def.transform.localPosition += .02f * hit.normal;
                def.transform.localScale = new Vector3(rScale, rScale, rScale);
                def.transform.parent = hit.transform;
            }
        }

        aSource.PlayOneShot(soundFire);
        m_LastFrameShot = Time.frameCount;

        weaponAnim[fireAnim].speed = fireAnimSpeed;
        weaponAnim.Rewind(fireAnim);
        weaponAnim.Play(fireAnim);

        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUp, Random.Range(-kickSideways, kickSideways), 0));

        bulletsLeft--;
    }

    void FireOnePellet()
    {

        Vector3 dir = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * triggerTime, Random.Range(-0.01f, 0.01f) * triggerTime, 1));
        Vector3 pos = transform.parent.position;

        if (Physics.Raycast(pos, dir, out hit, range, layerMask))
        {

            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float rScale = Random.Range(0.5f, 1.0f);

            if (hit.rigidbody)
                hit.rigidbody.AddForceAtPosition(force * dir, hit.point);

            if (hit.collider.tag == "Concrete")
            {
                GameObject concMark = Instantiate(Concrete, contact, rot) as GameObject;
                concMark.transform.localPosition += .02f * hit.normal;
                concMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                concMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Enemy")
            {
                Instantiate(Blood, contact, rot);
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            //ADDED
            else if (hit.collider.tag == "Zombie")
            {
                print("HIT COLLIDER");
                Instantiate(Blood, contact, rot);
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            else if (hit.collider.tag == "Damage")
            {
                Instantiate(Blood, contact, rot);
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            else if (hit.collider.tag == "Wood")
            {
                GameObject woodMark = Instantiate(Wood, contact, rot) as GameObject;
                woodMark.transform.localPosition += .02f * hit.normal;
                woodMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                woodMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Metal")
            {
                GameObject metalMark = Instantiate(Metal, contact, rot) as GameObject;
                metalMark.transform.localPosition += .02f * hit.normal;
                metalMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                metalMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Dirt" || hit.collider.tag == "Grass")
            {
                GameObject dirtMark = Instantiate(Dirt, contact, rot) as GameObject;
                dirtMark.transform.localPosition += .02f * hit.normal;
                dirtMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                dirtMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Water")
            {
                Instantiate(Water, contact, rot);

            }
            else if (hit.collider.tag == "Usable")
            {
                hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);

            }
            else
            {
                GameObject def = Instantiate(Untagged, contact, rot) as GameObject;
                def.transform.localPosition += .02f * hit.normal;
                def.transform.localScale = new Vector3(rScale, rScale, rScale);
                def.transform.parent = hit.transform;
            }
        }
    }

    void FireProjectile()
    {
        if (projectiles < 1 || draw)
        {
            return;
        }

        float[] info = new float[2];
        info[0] = projectileSpeed;
        info[1] = projectileGravity;

        GameObject projectile = Instantiate(projectilePrefab, launchPosition.position, launchPosition.rotation) as GameObject;
        Projectile g = projectile.GetComponent<Projectile>();
        g.SetUp(info);

        weaponAnim[fireAnim].speed = fireAnimSpeed;
        weaponAnim.Rewind(fireAnim);
        weaponAnim.Play(fireAnim);

        projectiles--;

        if (reloadProjectile)
           StartCoroutine( ReloadLauncher());
    }

    /// <summary>
    /// When we have no ammo, call this
    /// </summary>
    IEnumerator OutOfAmmo()
    {
        if (reloading || playing) yield break;

        playing = true;
        aSource.PlayOneShot(soundEmpty, 0.3f);
        if (fireEmptyAnim != "")
        {
            weaponAnim.Rewind(fireEmptyAnim);
            weaponAnim.Play(fireEmptyAnim);
        }
        yield return new WaitForSeconds(0.2f);
        playing = false;

    }

    /// <summary>
    /// Reload RPG Launcher
    /// </summary>
    IEnumerator ReloadLauncher()
    {
        if (projectiles > 0)
        {
            wepManager.canSwitch = false;
            reloading = true;
            canSwicthMode = false;

            if (rocket != null)
               StartCoroutine(DisableProjectileRenderer());

            yield return new WaitForSeconds(0.5f);
            if (soundReloadLauncher)
                aSource.PlayOneShot(soundReloadLauncher);

            weaponAnim[reloadAnim].speed = reloadAnimSpeed;
            weaponAnim.Play(reloadAnim);

            yield return new WaitForSeconds(reloadTime);
            canSwicthMode = true;
            reloading = false;
            wepManager.canSwitch = true;
        }
        else
        {
            if (rocket != null && projectiles == 0)
            {
                rocket.enabled = false;
            }
        }
    }

    IEnumerator DisableProjectileRenderer()
    {
        rocket.enabled = false;
        yield return new WaitForSeconds(reloadTime / 1.5f);
        rocket.enabled = true;
    }

    void EnableProjectileRenderer()
    {
        if (rocket != null)
        {
            rocket.enabled = true;
        }
    }

    IEnumerator Reload()
    {
        if (reloading) yield break;

        if (ammoMode == Ammo.Magazines)
        {
            reloading = true;
            canSwicthMode = false;
            if (magazines > 0 && bulletsLeft != bulletsPerMag)
            {
                weaponAnim[reloadAnim].speed = reloadAnimSpeed;
                weaponAnim.Play(reloadAnim, PlayMode.StopAll);
                //weaponAnim.CrossFade(reloadAnim);
                aSource.PlayOneShot(soundReload);
                yield return new WaitForSeconds(reloadTime);
                magazines--;
                bulletsLeft = bulletsPerMag;
            }
            reloading = false;
            canSwicthMode = true;
            isFiring = false;
        }

        if (ammoMode == Ammo.Bullets)
        {
            if (magazines > 0 && bulletsLeft != bulletsPerMag)
            {
                if (magazines > bulletsPerMag)
                {
                    canSwicthMode = false;
                    reloading = true;
                    weaponAnim[reloadAnim].speed = reloadAnimSpeed;
                    weaponAnim.Play(reloadAnim, PlayMode.StopAll);
                    //weaponAnim.CrossFade(reloadAnim);
                    aSource.PlayOneShot(soundReload, 0.7f);
                    yield return new WaitForSeconds(reloadTime);
                    magazines -= bulletsPerMag - bulletsLeft;
                    bulletsLeft = bulletsPerMag;
                    canSwicthMode = true;
                    reloading = false;
                    yield break;
                }
                else
                {
                    canSwicthMode = false;
                    reloading = true;
                    weaponAnim[reloadAnim].speed = reloadAnimSpeed;
                    weaponAnim.Play(reloadAnim, PlayMode.StopAll);
                    //weaponAnim.CrossFade(reloadAnim);
                    aSource.PlayOneShot(soundReload);
                    yield return new WaitForSeconds(reloadTime);
                    var bullet = Mathf.Clamp(bulletsPerMag, magazines, bulletsLeft + magazines);
                    magazines -= (bullet - bulletsLeft);
                    bulletsLeft = bullet;
                    canSwicthMode = true;
                    reloading = false;
                    yield break;
                }
            }
        }
    }

    IEnumerator DrawWeapon()
    {
        draw = true;
        wepCamera.fieldOfView = weaponFOV;
        canSwicthMode = false;
        aSource.clip = soundDraw;
        aSource.Play();

        weaponAnim[drawAnim].speed = drawAnimSpeed;
        weaponAnim.Rewind(drawAnim);
        weaponAnim.Play(drawAnim, PlayMode.StopAll);
        yield return new WaitForSeconds(drawTime);

        draw = false;
        reloading = false;
        canSwicthMode = true;
        selected = true;

    }

    void Deselect()
    {
        selected = false;
        mainCamera.fieldOfView = camFOV;
        transform.localPosition = hipPosition;
    }


}