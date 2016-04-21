/* Weapon.cs
 *
 * Component of FirstPersonCharacter
 * Spawns projectile from camera
 *
 * Author: Austin Nwachukwu
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof (AudioSource))]
public class Weapon : MonoBehaviour {

	public float bulletSpeed = 50;
	public Rigidbody bullet;
	private AudioSource m_AudioSource;
	public AudioClip m_FireSound;

	private void Start() {
        m_AudioSource = GetComponent<AudioSource>();
    }

	private void PlayFireSound() {
        m_AudioSource.clip = m_FireSound;
        m_AudioSource.Play();
    }

	void Fire() {
		Rigidbody bulletClone = (Rigidbody) Instantiate(bullet, transform.position, transform.rotation);
		bulletClone.velocity = transform.forward * bulletSpeed;
	}
	
	void FixedUpdate () {
        if (Input.GetMouseButtonDown(0)) {
        	Fire();
        	PlayFireSound();
        }
    }

}
