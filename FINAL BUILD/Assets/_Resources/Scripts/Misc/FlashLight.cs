using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour
{

    // FPS KIT [www.armedunity.com]

    public Light linkedLight;
    private float timeLeft;
    private float recharge;
    private bool stop = true;
    public const float BatteryLife = 30f;
    public const float RechargeTime = 10f;

    private float minutes;
    private float seconds;
     
    public GUISkin mySkin;

    void Start() {
        stop = false;
        timeLeft = BatteryLife;
        recharge = 0;
        Update();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (linkedLight.enabled) linkedLight.enabled = false;
            else if (timeLeft > 0) linkedLight.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            LightOff();
        }

        if (!linkedLight.enabled) {
            if (stop) {
                recharge += Time.deltaTime;
                if (recharge > RechargeTime) {
                    print("Flashlight Recharged!");
                    Start();
                }
            }
            return;
        }

        timeLeft -= Time.deltaTime;
         
        minutes = Mathf.Floor(timeLeft / 60);
        seconds = timeLeft % 60;
        if (seconds > 59) seconds = 59;
        if (minutes < 0) {
            stop = true;
            LightOff();
            minutes = 0;
            seconds = 0;
        }
    }

    void LightOn()
    {
        linkedLight.enabled = true;
    }

    void LightOff()
    {
        linkedLight.enabled = false;
    }

    public void AddTime() {
        timeLeft += Time.deltaTime;
    }


    void OnGUI()
    {
        GUI.skin = mySkin;
        GUI.depth = 2;

        GUI.Label(new Rect(40, Screen.height - 110, 100, 60), " Battery: ");
        GUI.Label(new Rect(100, Screen.height - 110, 160, 60), "" + minutes + ":" + seconds + "(" + recharge + ")", mySkin.customStyles[0]);
    }

}