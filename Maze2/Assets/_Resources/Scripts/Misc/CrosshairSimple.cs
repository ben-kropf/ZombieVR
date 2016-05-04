using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CrosshairSimple : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    Texture2D crosshair = null;

    void OnGUI()
    {
        float w = crosshair.width / 2;
        float h = crosshair.height / 2;
        Rect pos = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2, w, h);

        if (!Input.GetButton("Fire2"))
        {
            GUI.DrawTexture(pos, crosshair);
        }
    }
}