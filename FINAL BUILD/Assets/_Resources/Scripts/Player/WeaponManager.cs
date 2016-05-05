using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public GameObject[] weaponsInUse;
    public GameObject[] weaponsInGame;
    public Rigidbody[] worldModels;

    public RaycastHit hit;
    private float dis = 3.0f;
    public LayerMask layerMaskWeapon;
    public LayerMask layerMaskAmmo;

    public Transform dropPosition;

    private float switchWeaponTime = 0.25f;
    //[HideInInspector]
    public bool canSwitch = true;
    [HideInInspector]
    public bool showWepGui = false;
    [HideInInspector]
    public bool showAmmoGui = false;
    private bool equipped = false;
    [HideInInspector]
    public int weaponToSelect;
    [HideInInspector]
    public int setElement;
    [HideInInspector]
    public int weaponToDrop;

    public GUISkin mySkin;
    public AudioClip pickupSound;
    public AudioSource aSource;
    public HealthScript hs;
    private string textFromPickupScript = "";
    private string notes = "";
    private string note = "Press key <color=#88FF6AFF> << E >> </color> to pick up Ammo";
    private string wrongType = "Select appropriate weapon to pick up";
    public int selectWepSlot1 = 0;
    public int selectWepSlot2 = 0;
    
    public void Start()
    {
        for (int h = 0; h < worldModels.Length; h++)
        {
            weaponsInGame[h].SetActive(false);
        }

        weaponsInUse = new GameObject[2];
        weaponsInUse[0] = weaponsInGame[selectWepSlot1];
        weaponsInUse[1] = weaponsInGame[selectWepSlot2];

        weaponToSelect = 0;
        StartCoroutine(DeselectWeapon());
    }
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.None) return;

        if (Input.GetKeyDown("1") && weaponsInUse.Length >= 1 && canSwitch && weaponToSelect != 0)
        {
            StartCoroutine(DeselectWeapon());
            weaponToSelect = 0;

        }
        else if (Input.GetKeyDown("2") && weaponsInUse.Length >= 2 && canSwitch && weaponToSelect != 1)
        {
            StartCoroutine(DeselectWeapon());
            weaponToSelect = 1;

        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && canSwitch)
        {
            weaponToSelect++;
            if (weaponToSelect > (weaponsInUse.Length - 1))
            {
                weaponToSelect = 0;
            }
            StartCoroutine(DeselectWeapon());
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && canSwitch)
        {
            weaponToSelect--;
            if (weaponToSelect < 0)
            {
                weaponToSelect = weaponsInUse.Length - 1;
            }
            StartCoroutine(DeselectWeapon());
        }

        Vector3 pos = transform.parent.position;
        Vector3 dir = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(pos, dir, out hit, dis, layerMaskWeapon))
        {

            WeaponIndex pre = hit.transform.GetComponent<WeaponIndex>();
            setElement = pre.setWeapon;
            showWepGui = true;

            if (weaponsInUse[0] != weaponsInGame[setElement] && weaponsInUse[1] != weaponsInGame[setElement])
            {
                equipped = false;
            }
            else
            {
                equipped = true;
            }

            if (canSwitch && !equipped)
            {
                if (Input.GetKeyDown("e"))
                {
                    DropWeapon(weaponToDrop);
                    StartCoroutine(DeselectWeapon());
                    weaponsInUse[weaponToSelect] = weaponsInGame[setElement];
                    Destroy(hit.transform.gameObject);
                }
            }

        }
        else
        {
            showWepGui = false;
        }

        if (Physics.Raycast(pos, dir, out hit, dis, layerMaskAmmo))
        {
            showAmmoGui = true;
            if (hit.transform.CompareTag("Ammo"))
            {
                Pickup pickupGO = hit.transform.GetComponent<Pickup>();

                //bullets/magazines
                if (pickupGO.pickupType == PickupType.Magazines)
                {
                    WeaponScriptNEW mags = weaponsInUse[weaponToSelect].transform.GetComponent<WeaponScriptNEW>();
                    if (mags == null)
                    {
                        textFromPickupScript = "";
                        return;
                    }
                    if (mags.firstMode != fireMode.launcher)
                    {
                        notes = "";
                        textFromPickupScript = note;
                        if (Input.GetKeyDown("e"))
                        {
                            if (mags.ammoMode == Ammo.Magazines)
                                mags.magazines += pickupGO.amount;
                            else
                                mags.magazines += pickupGO.amount * mags.bulletsPerMag;

                            aSource.PlayOneShot(pickupSound, 0.3f);
                            Destroy(hit.transform.gameObject);
                        }
                    }
                    else
                    {
                        textFromPickupScript = pickupGO.AmmoInfo;
                        notes = wrongType;
                    }
                }

                //projectiles/rockets
                if (pickupGO.pickupType == PickupType.Projectiles)
                {
                    WeaponScriptNEW projectile = weaponsInUse[weaponToSelect].transform.GetComponent<WeaponScriptNEW>();
                    if (projectile == null)
                    {
                        textFromPickupScript = "";
                        return;
                    }
                    if (projectile.secondMode == fireMode.launcher || projectile.firstMode == fireMode.launcher)
                    {
                        notes = "";
                        textFromPickupScript = note;
                        if (Input.GetKeyDown("e"))
                        {
                            projectile.projectiles += pickupGO.amount;
                            aSource.PlayOneShot(pickupSound, 0.3f);
                            Destroy(hit.transform.gameObject);
                        }
                    }
                    else
                    {
                        textFromPickupScript = pickupGO.AmmoInfo;
                        notes = wrongType;
                    }
                }

                //health
                if (pickupGO.pickupType == PickupType.Health)
                {
                    textFromPickupScript = pickupGO.AmmoInfo;
                    notes = "";
                    if (Input.GetKeyDown("e"))
                    {
                        hs.Medic(pickupGO.amount);
                        aSource.PlayOneShot(pickupSound, 0.3f);
                        Destroy(hit.transform.gameObject);
                    }
                }
            }

        }
        else
        {
            showAmmoGui = false;
        }
    }

    void OnGUI()
    {
        GUI.skin = mySkin;

        if (showWepGui)
        {
            if (!equipped)
                GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 100), "Press key <color=#88FF6AFF> << E >> </color> to pickup weapon", mySkin.customStyles[1]);
            else
                GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 100), "Weapon is already equipped", mySkin.customStyles[1]);
        }

        if (showAmmoGui)
            GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 200), notes + "\n" + textFromPickupScript, mySkin.customStyles[1]);
    }

    IEnumerator DeselectWeapon()
    {
        canSwitch = false;

        for (int i = 0; i < weaponsInUse.Length; i++)
        {
            weaponsInUse[i].SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
            weaponsInUse[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(switchWeaponTime);
        SelectWeapon(weaponToSelect);
        yield return new WaitForSeconds(switchWeaponTime);
        canSwitch = true;
    }
    void SelectWeapon(int i)
    {
        weaponsInUse[i].gameObject.SetActive(true);
        weaponsInUse[i].SendMessage("DrawWeapon", SendMessageOptions.DontRequireReceiver);
        WeaponIndex temp = weaponsInUse[i].transform.GetComponent<WeaponIndex>();
        weaponToDrop = temp.setWeapon;
    }

    void DropWeapon(int index)
    {
        if (index == 0) return;

        for (int i = 0; i < worldModels.Length; i++)
        {
            if (i == index)
            {
                Rigidbody drop = Instantiate(worldModels[i], dropPosition.transform.position, dropPosition.transform.rotation) as Rigidbody;
                drop.AddRelativeForce(0, 250, Random.Range(100, 200));
                drop.AddTorque(-transform.up * 40);
            }
        }
    }

    public void EnterWater()
    {
        canSwitch = false;
        for (int i = 0; i < weaponsInUse.Length; i++)
        {
            weaponsInUse[i].SendMessage("Deselect", SendMessageOptions.DontRequireReceiver);
            weaponsInUse[i].gameObject.SetActive(false);
        }
    }

    public void ExitWater()
    {
        canSwitch = true;
        SelectWeapon(weaponToSelect);
    }



}