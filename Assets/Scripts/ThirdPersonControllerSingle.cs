using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ThirdPersonControllerSingle : MonoBehaviour
{
    [SerializeField] Rigidbody theRB;
    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 180, gravityForce = 10f, dragOnGround = 3f;
    // Start is called before the first frame update
   

    public GameObject PlayerCamera;
    private float speedInput, turnInput;
    private bool grounded;
    public LayerMask whatIsGround;
    public float groundRayLength = 0.5f;
    public Transform groundRayPoint;
    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f;
    private float emissionRate;

    void Start()
    {


    }
    private void Awake()
    {

        PlayerCamera.SetActive(true);




    }

    // Update is called once per frame
    void Update()
    {
       
            speedInput = 0f;

            if (Input.GetAxis("Vertical") > 0)
            {
                speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
            }

            turnInput = Input.GetAxis("Horizontal");
            if (grounded)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
            }

            if (Input.GetKeyDown("escape"))
            {
                
                SceneManager.LoadScene("MainMenu");
            }
            //else
            //{
            // SceneManager.LoadScene(sceneName);
            // }


        
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);
        transform.position = theRB.transform.position;

        //  }
    }
    private void FixedUpdate()
    {//Debug.Log(photonView.isMine);
        
            grounded = false;
            RaycastHit hit;
            if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
            {
                grounded = true;
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }
            emissionRate = 0f;
            if (grounded)
            {
                theRB.drag = dragOnGround;
                if (Mathf.Abs(speedInput) > 0)
                {
                    theRB.AddForce(transform.forward * speedInput);
                    emissionRate = maxEmission;
                }
            }
            else
            {
                theRB.drag = 0.01f;
                theRB.AddForce(Vector3.up * -gravityForce * 100f);
            }
            foreach (ParticleSystem part in dustTrail)
            {
                var emissionModule = part.emission;
                emissionModule.rateOverTime = emissionRate;
            }
        
        //fix particles (displaying wrong in multiplayer)
    }
}
