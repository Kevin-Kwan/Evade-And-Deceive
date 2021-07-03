using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;

public class TurdPersonCharacterController : MonoBehaviourPun
{
    [SerializeField] private Rigidbody theRB;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 180, gravityForce = 10f, dragOnGround = 3f;
    // Start is called before the first frame update
    [SerializeField] private PhotonView photonView;


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
    GameObject cams;
    GameObject manger;
    GameManager pogu ;
    bool p1,p2,p3,p4;

    bool finishedlap = false;
    bool finishrace = false;
    int lapnum = 1;
    GameObject[] checks;
    int triggercount;
    bool touchedalltrigger=false;
    List<CheckpointSingle> checklist;
    GameObject[] checkpointsTransform;
    private int nextCheckpointSingleIndex;
    public event EventHandler OnCorrectCheckpoint;
    public event EventHandler OnWrongCheckpoint;
    private int LastCheckpointSingleIndex=0;
    public int checkAmount=0;
    public int posInRace;

    [SerializeField] private TrackCheckpointsUI brotherman;
    [SerializeField] private ufinish brotherman2;
    

    public Transform SpawnPoint;

    [SerializeField] private GameObject theBallControl;

    Room room;
    private List<ITEM> items;

    void Start()
    {checks = GameObject.FindGameObjectsWithTag("checkpoint");
        checklist = new List<CheckpointSingle>();
        triggercount = checks.Length;
        //Debug.Log(checks.Length);
        manger = GameObject.Find("GameManager");
       pogu = manger.GetComponent<GameManager>();
        p1 = pogu.p1;
        p2 = pogu.p2;
        p3 = pogu.p3;
        p4 = pogu.p4;
        SetName();
        checkpointsTransform = FindObsWithTag("checkpoint");
        Debug.Log("car created");
        foreach (GameObject checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            
            checklist.Add(checkpointSingle);

        }
        nextCheckpointSingleIndex = 0;
        roomNameText.text = "Room Code: "+PhotonNetwork.CurrentRoom.Name;
       
    }
    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        Debug.Log(checklist);
        Debug.Log(checklist.IndexOf(checkpointSingle));
        if (checklist.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            Debug.Log(this.gameObject.name + "Correct");
            //todo: play a sound when u pass checkpoint

            brotherman.Hide();
            LastCheckpointSingleIndex = checklist.IndexOf(checkpointSingle);
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checklist.Count;
            checkAmount++;


            //OnCorrectCheckpoint?.Invoke(this,EventArgs.Empty);
            if (checklist.IndexOf(checkpointSingle) == checklist.Count - 1)
            {

                Debug.Log("finished");
                brotherman2.Show();
                finishrace = true;
                //todo: reset all the starting numbers, use int to count laps
            }
        }
        else if (checklist.IndexOf(checkpointSingle) == LastCheckpointSingleIndex)
        {
            Debug.Log(this.gameObject.name + "Correct");
            //todo: play a sound when u pass checkpoint

            brotherman.Hide();
        }
        else
        {
            Debug.Log("Incorrect");
            OnCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
            brotherman.Show();
        }
    }

        private void SetName()
        {
            nameText.text = photonView.Owner.NickName;
        }
        GameObject[] FindObsWithTag(string tag)
        {
            GameObject[] foundObs = GameObject.FindGameObjectsWithTag(tag);
            Array.Sort(foundObs, CompareObNames);
            return foundObs;
        }


        int CompareObNames(GameObject x, GameObject y)
        {
            return x.name.CompareTo(y.name);
        }
        private void Awake()
        {

            if (photonView.IsMine)
            {
                PlayerCamera.SetActive(true);
                GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = true;
                GetComponent<AudioSource>().enabled = true;



            }
            GameObject[] cams = GameObject.FindGameObjectsWithTag("camcam");
            GameObject cameraview = this.transform.Find("CM vcam1").gameObject;
            for (int i = 0; i < cams.Length; i++)
            {
                if (cams[i].layer == 12)
                {
                    p1 = true;
                }
                if (cams[i].layer == 13)
                {
                    p2 = true;
                }
                if (cams[i].layer == 14)
                {
                    p3 = true;
                }
                if (cams[i].layer == 15)
                {
                    p4 = true;
                }
            }

            if (!p1)
            {
                cameraview.layer = LayerMask.NameToLayer("P1");
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 13);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 14);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 15);
            }
            else if (!p2)
            {
                cameraview.layer = LayerMask.NameToLayer("P2");
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 12);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 14);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 15);
            }
            else if (!p3)
            {
                cameraview.layer = LayerMask.NameToLayer("P3");
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 12);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 13);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 15);
            }
            else if (!p4)
            {
                cameraview.layer = LayerMask.NameToLayer("P4");
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 12);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 13);
                PlayerCamera.GetComponent<Camera>().cullingMask &= ~(1 << 14);
            }
            //make sure that players are ordered idea: https://forum.photonengine.com/discussion/14763/playerlist-actornumber-problem get player number, compare, then assign




            if (!photonView.IsMine)
            {
                Destroy(theRB);
                GetComponent<AudioSource>().enabled = false;
                GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = false;
                // Destroy(GetComponentInChildren<Camera>().gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (photonView.IsMine)
            {
                if (finishrace)
                {
                    brotherman2.Show();
                }

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
                    PhotonNetwork.LeaveRoom();
                    SceneManager.LoadScene("MainMenu");
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Debug.Log("i pressed r");
                    theBallControl.transform.position = new Vector3(checklist[LastCheckpointSingleIndex].transform.position.x, 1f, checklist[LastCheckpointSingleIndex].transform.position.z);
                    transform.rotation = checklist[LastCheckpointSingleIndex].transform.rotation;

                }

                //else
                //{
                // SceneManager.LoadScene(sceneName);
                // }


            }
            leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
            rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);
            transform.position = theRB.transform.position;

            //  }
        }
        private void FixedUpdate()
        {//Debug.Log(photonView.isMine);
            if (!photonView.IsMine)
            {
                return;
            }
            else
            {
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
            }
            //fix particles (displaying wrong in multiplayer)
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "finish")
            {
                Debug.Log("touchedfinish");

            }
            if (other.gameObject.tag == "checkpoint")
            {

                PlayerThroughCheckpoint(other.GetComponent<CheckpointSingle>());

            }

            //boost pad (WIP)
            if (other.gameObject.tag == "speedboost")
            {
                float initForwardAccel = forwardAccel;
                forwardAccel *= 2;
            }

        }

    }
             
        
    

