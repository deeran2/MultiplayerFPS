using UnityEngine;

public class PlayerUI : MonoBehaviour {

	[SerializeField]
	private RectTransform thrusterFuelFill;

	private PlayerController controller;

	void SetFuelAmout(float amount){
		thrusterFuelFill.localScale = new Vector3 (1f, amount, 1f);
	}

	public void SetController(PlayerController _controller){
		controller = _controller;
	}

	void Update(){
		SetFuelAmout (controller.getThrusterFuelAmount ());
	}
}
