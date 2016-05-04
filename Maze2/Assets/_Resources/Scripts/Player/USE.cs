using UnityEngine;
using System.Collections;

public class USE : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public float maxRayDistance = 2.0f;
    public LayerMask layerMask;
    public GUISkin mySkin;
    public bool showGui = false;
    public RaycastHit hit;

    void Update()
    {
        Vector3 dir = gameObject.transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, dir, out hit, maxRayDistance, layerMask))
        {
            showGui = true;
            if (Input.GetButtonDown("Use"))
            {
                GameObject target = hit.collider.gameObject;
                target.BroadcastMessage("Action");
            }
        }
        else
        {
            showGui = false;
        }
    }

    void OnGUI()
    {
        if (showGui)
        {
            GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 100), "Press key <color=#88FF6AFF> << E >> </color> to Use", mySkin.customStyles[1]);
        }
    }

}