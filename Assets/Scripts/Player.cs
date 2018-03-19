using UnityEngine.Networking;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {
	[SyncVar]
	private bool _isDead = false;
	public bool isDead {
		get{return _isDead;}
		protected set{_isDead = value;}
	}

	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathEffect;

	[SerializeField]
	private GameObject spawnEffect;

	private bool firstSetup = true;

	public void SetUp(){

		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraActive (false);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (true);
		}

		CmdBroadcastNewPlayer ();

	}
	[Command]
	private void CmdBroadcastNewPlayer(){

		RpcSetupPlayerOnAllClients ();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients(){

		if (firstSetup) {
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++) {

				wasEnabled [i] = disableOnDeath [i].enabled;
			}
			firstSetup = false;
		}
		SetDefaults ();
	}

//	void Update(){
//		if (!isLocalPlayer) {
//			return;
//		}
//		if(Input.GetKeyDown(KeyCode.K)){
//			RpcTakeDamage(999);
//		}
//	}
	[ClientRpc]
	public void RpcTakeDamage(int amount){

		if (isDead) {
			return;
		}

		currentHealth -= amount;

		if (currentHealth <= 0) {
			Die ();
		}
	}

	private void Die(){
		isDead = true;

		for (int i = 0; i < disableOnDeath.Length; i++) {

			disableOnDeath [i].enabled = false;
		}

		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {

			disableGameObjectsOnDeath [i].SetActive (false);
		}

		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = false;
		}

		GameObject GFXins = (GameObject)Instantiate (deathEffect, transform.position, Quaternion.identity);
		Destroy (GFXins, 3f);

		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraActive (true);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (false);
		}

		StartCoroutine (Respawn ());
	}
	private IEnumerator Respawn(){

		yield return new WaitForSeconds (GameManager.instance.matchSettings.respawnDelay);

		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		yield return new WaitForSeconds (.1f);

		SetUp ();
	}

	public void SetDefaults(){

		isDead = false;

		for (int i = 0; i < disableOnDeath.Length; i++) {

			disableOnDeath [i].enabled = wasEnabled [i];
		}

		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {

			disableGameObjectsOnDeath [i].SetActive (true);
		}

		currentHealth = maxHealth;

		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = true;
		}



		GameObject GFXins = (GameObject)Instantiate (spawnEffect, transform.position, Quaternion.identity);
		Destroy (GFXins, 3f);

	}
}
