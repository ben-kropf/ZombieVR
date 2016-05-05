/* WeaponManager.cs
 *
 * Used to destory prefab copies after spawn.
 *
 * Author: Austin Nwachukwu
 */

using UnityEngine;
using System.Collections;

public class OldWeaponManager : MonoBehaviour {
	private void Awake() {
        Destroy(this.gameObject, 3F);
    }
}
