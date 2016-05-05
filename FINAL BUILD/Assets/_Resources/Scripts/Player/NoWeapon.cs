using UnityEngine;
using System.Collections;

public class NoWeapon : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public float cSize = 32.0f;
    public Texture2D crossTexture;

    void Start()
    {
    }

    void OnGUI()
    {
        Rect pos = new Rect((Screen.width - cSize) / 2, (Screen.height - cSize) / 2, cSize, cSize);
        GUI.DrawTexture(pos, crossTexture);
    }

    //Empty fuctions, cuz we don't have weapon

    void DrawWeapon()
    {
    }

    void Deselect()
    {
    }
}