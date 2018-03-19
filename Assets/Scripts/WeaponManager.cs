using UnityEngine.Networking;
using UnityEngine;

public class WeaponManager : NetworkBehaviour {
	
	[SerializeField]
	private string weaponLayerName = "Weapon";

	[SerializeField]
	private Transform weaponHolder;

	[SerializeField]
	private PlayerWeapon primaryWeapon;

	private PlayerWeapon currentWeapon;
	private WeaponGraphics currentGraphics;

	void Start(){

		EquipWeapon (primaryWeapon);
	}

	public PlayerWeapon GetCurrentWeapon(){
		return currentWeapon;
	}

	public WeaponGraphics GetCurrentGraphics(){
		return currentGraphics;
	}

	void EquipWeapon(PlayerWeapon newWeapon){
		currentWeapon = newWeapon;

		GameObject weaponIns = (GameObject)Instantiate (newWeapon.weaponGFX, weaponHolder.position, weaponHolder.rotation);
		weaponIns.transform.SetParent (weaponHolder);

		currentGraphics = weaponIns.GetComponent<WeaponGraphics> ();
		if(currentGraphics == null){
			Debug.Log ("No weapon graphics");
		}

		if (isLocalPlayer) {
			Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponLayerName));
		}

	}

}
