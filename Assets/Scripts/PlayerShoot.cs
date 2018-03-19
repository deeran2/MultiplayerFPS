using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	private WeaponManager weaponManager;

	private PlayerWeapon currentWeapon;


	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	void Start(){
		if(cam ==null){
			Debug.Log("No camera");
			this.enabled = false;
		}
		weaponManager = GetComponent<WeaponManager> ();
	}

	void Update(){
		currentWeapon = weaponManager.GetCurrentWeapon ();

		if (currentWeapon.fireRate <= 0) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
			}
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				InvokeRepeating ("Shoot", 0f, 1f/currentWeapon.fireRate);
			} else if(Input.GetButtonUp("Fire1")){
				CancelInvoke ("Shoot");
			}
		}
	}

	[Command]
	void CmdOnShoot(){
		RpcDoShootEffect ();
	}

	[ClientRpc]
	void RpcDoShootEffect(){
		weaponManager.GetCurrentGraphics ().muzzleFlash.Play ();
	}

	[Command]
	void CmdOnHit(Vector3 pos, Vector3 normal){
		RpcDoHitEffect (pos, normal);
	}
	[ClientRpc]
	void RpcDoHitEffect(Vector3 pos, Vector3 normal){
		GameObject hitEffect = (GameObject)Instantiate (weaponManager.GetCurrentGraphics ().hitEffectPrefab, pos, Quaternion.LookRotation (normal));
		Destroy (hitEffect, 2f);
	} 

	[Client]
	void Shoot(){

		if (!isLocalPlayer) {
			return;
		}

		CmdOnShoot ();

		RaycastHit _hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)) {
			if (_hit.collider.tag == PLAYER_TAG) {
				CmdPlayerShot (_hit.collider.name, currentWeapon.damage);
			}
			CmdOnHit (_hit.point, _hit.normal);
		}
	}

	[Command]
	void CmdPlayerShot(string _playerID, int _damage){
		Debug.Log (_playerID + " has been shot");
		Player _player = GameManager.GetPlayer (_playerID);
		_player.RpcTakeDamage (_damage);
	}
}
