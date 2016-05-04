using UnityEngine;
using System.Collections;


public enum PickupType { Health, Magazines, Projectiles }

public class Pickup : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]
    public PickupType pickupType = PickupType.Health;
    public int amount = 3;
    public string AmmoInfo = "";

}