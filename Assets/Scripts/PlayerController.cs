using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {


	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 3f;

	[SerializeField]
	private float thrusterForce = 1000f;

	[SerializeField]
	private float thrusterFuelBurnSpeed = .8f;

	[SerializeField]
	private float thrusterFuelRegenSpeed = .3f;
	private float thrusterFuelAmount = 1f;


	private Animator anim;

	public float getThrusterFuelAmount(){
		return thrusterFuelAmount;
	}

	[SerializeField]
	private LayerMask enviornmentMask;


	[Header("Joint Options:")]

	[SerializeField]
	private float jointSpring = 20;
	[SerializeField]
	private float jointMaxForce = 40f;


	private ConfigurableJoint joint;
	private PlayerMotor motor;

	void Start(){
		motor = GetComponent<PlayerMotor> ();
		joint = GetComponent<ConfigurableJoint> ();
		SetJointSettings (jointSpring);
		anim = GetComponent<Animator> ();
	}

	void Update(){

		RaycastHit _hit;
		if (Physics.Raycast (transform.position, Vector3.down, out _hit, 100f, enviornmentMask)) {
			joint.targetPosition = new Vector3 (0f, -_hit.point.y, 0f);
		} else {
			joint.targetPosition = new Vector3 (0f, 0f, 0f);
		}

		float _xMov = Input.GetAxis ("Horizontal");
		float _zMov = Input.GetAxis ("Vertical");

		Vector3 movHorizontal = transform.right * _xMov;
		Vector3 movVertical = transform.forward * _zMov;

		Vector3 _velocity = (movHorizontal + movVertical) * speed;

		anim.SetFloat ("ForwardVelocity", _zMov);

		motor.Move (_velocity);

		float _yrot = Input.GetAxisRaw ("Mouse X");

		Vector3 _rotation = new Vector3 (0f, _yrot, 0) * lookSensitivity;

		motor.Rotate (_rotation);

		float _xrot = Input.GetAxisRaw ("Mouse Y");

		float _cameraRotationX = _xrot * lookSensitivity;

		motor.RotateCamera (_cameraRotationX);

		Vector3 _thrusterForce = Vector3.zero;

		if (Input.GetButton ("Jump") && thrusterFuelAmount > 0f) {

			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

			if (thrusterFuelAmount >= .01f) {
				_thrusterForce = Vector3.up * thrusterForce;
				SetJointSettings (0f);
			}
		} else {
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
			SetJointSettings (jointSpring);
		}

		thrusterFuelAmount = Mathf.Clamp (thrusterFuelAmount, 0f, 1f);

		motor.ApplyThruster (_thrusterForce);
	}
	private void SetJointSettings(float _jointSpring){
		joint.yDrive = new JointDrive { 
			positionSpring = _jointSpring, 
			maximumForce = jointMaxForce 
		};
	}
}
