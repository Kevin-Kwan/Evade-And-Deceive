using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

using System;
using System.Linq;
using System.Diagnostics;

//todo make courses as prefabs so they are easily transferrable across single and multiplayer

public class ITEM
{
    public GameObject itemObj { get; set; }
    public int startPos { get; set; }
}
public class ITEMsort
{
    public GameObject itemObj { get; set; }
    public float fintime { get; set; }
}
public class ThirdPersonCharacterController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Rigidbody theRB;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI waitingText;
    [SerializeField] private TextMeshProUGUI speed;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI posRaceText;
    [SerializeField] private Text manmenu;
    [SerializeField] private GameObject theroomimage;
    [SerializeField] private GameObject pausemenu;
    [SerializeField] private GameObject showthepositionplace;
    private bool pausemenushow=false;
    public float forwardAccel = 8f, reverseAccel = 6f, maxSpeed = 50f, turnStrength = 180, gravityForce = 10f, dragOnGround = 3f;
    // Start is called before the first frame update
    [SerializeField] private PhotonView photonView;

    private int pastplayercount;

    public GameObject PlayerCamera;
    public GameObject MapCamera;
    private float speedInput, turnInput;
    private float boostMultiplier = 10f;
    private float slowMultiplier = -10f;
    private bool grounded;
    public LayerMask whatIsGround;
    public float groundRayLength = 0.5f;
    public Transform groundRayPoint;
    public Transform leftFrontWheel, rightFrontWheel, backwheels;
    public float maxWheelTurn = 25f;

    public ParticleSystem dustTrail1;
    public ParticleSystem dustTrail2;
    public float maxEmission = 25f;
    private float emissionRate;
    GameObject cams;
    GameObject manger;
    GameManager pogu;
    bool p1, p2, p3, p4;

    bool finishedlap = false;
    bool finishrace = false;
    int lapnum = 1;
    GameObject[] checks;
    int triggercount;
    bool touchedalltrigger = false;
    List<CheckpointSingle> checklist;
    GameObject[] checkpointsTransform;
    private int nextCheckpointSingleIndex=0;
    public event EventHandler OnCorrectCheckpoint;
    public event EventHandler OnWrongCheckpoint;
    private int LastCheckpointSingleIndex = 0;
    private int checkAmount = 0;
    public int posInRace;
    

    public bool canMove=false;
    bool singleplayer = false;
    bool devMode = false;

    [SerializeField] private TrackCheckpointsUI brotherman;
    [SerializeField] private ufinish brotherman2;
    [SerializeField] private ufinish thewaitingstuff;
    [SerializeField] private ufinish thecountdown;
    [SerializeField] private ufinish goStartrace;

    [SerializeField] private GameObject theBallControl;
    private List<ITEM> items;
    private List<ITEMsort> carsort;

    private bool start = true;
    
    public Stopwatch timer;
    
    
    TimeSpan besttime;
    float sceneBestTime;
    string key;
    bool started = false;

    //For Shell projectile
    public float moveForce = 0f;
    private Rigidbody rbody;
    public GameObject shell;
    public Transform shooter;
    public float shootRate = 0f;
    public float shootForce = 0f;
    private float shootRateTimeStamp = 0f;
    public float numberOfShellsLeft = 2f;
    public GameObject shellAddedText;

    //Audio
    private AudioSource startupSound;
    private AudioSource engineRunningSound;
    
    public float engineMinAudioPitch;
    private float engineCurrentPitch;
    public static float currentCarSpeed;

    

    void Start()
    { pastplayercount = PhotonNetwork.CurrentRoom.PlayerCount;
        AudioListener.volume= PlayerPrefs.GetFloat("volume");
        if (SceneManager.GetActiveScene().name=="TrackSongle"|| SceneManager.GetActiveScene().name == "OriginalTrack")
        {
            singleplayer = true;
            
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        shellAddedText.SetActive(false);
        pausemenu.SetActive(true);
        pausemenushow = false;
        checks = GameObject.FindGameObjectsWithTag("checkpoint");
        checklist = new List<CheckpointSingle>();
        triggercount = checks.Length;
        manger = GameObject.Find("GameManager");
        //UnityEngine.Debug.Log(checks.Length);
        
        pogu = manger.GetComponent<GameManager>();
        p1 = pogu.p1;
        p2 = pogu.p2;
        p3 = pogu.p3;
        p4 = pogu.p4;
        SetName();
        checkpointsTransform = GameObject.FindGameObjectsWithTag("checkpoint");

        foreach (GameObject checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            checklist.Add(checkpointSingle);

        }
        checklist=checklist.OrderBy(go=>go.name).ToList();
        nextCheckpointSingleIndex = 0;
        if (!singleplayer)
        {
            roomNameText.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;
            theroomimage.SetActive(true);
        }
        key = SceneManager.GetActiveScene().buildIndex.ToString();
        if (PlayerPrefs.HasKey(key))
        {
            sceneBestTime = PlayerPrefs.GetFloat(key);
            besttime = TimeSpan.FromMilliseconds(PlayerPrefs.GetFloat(key));

        }
        timer = new Stopwatch(); //in the start method, we create a new stopwatch whenever a new level is loaded, so the level time always starts at 0
       
        //For Shell projectile
        rbody = GetComponent<Rigidbody>();

        //Audio
        AudioSource[] soundSources = GetComponents<AudioSource>();
        for (int i = 0; i < soundSources.Length; i++)
        {
            if(soundSources[i].priority == 128)
            {
                // UnityEngine.Debug.Log("ASSIGNED STARTUP");
                startupSound = soundSources[i];
            }
            if(soundSources[i].priority == 130)
            {
                // UnityEngine.Debug.Log("ASSIGNED RUNNING");
                engineRunningSound = soundSources[i];
                engineRunningSound.mute = true;
            }
        }
        engineRunningSound.pitch = engineMinAudioPitch;
        
        StartCoroutine(PlayEngineSounds());
    }
    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        items = new List<ITEM>();
        GameObject[] cars = GameObject.FindGameObjectsWithTag("car");
        //UnityEngine.Debug.Log(checklist.IndexOf(checkpointSingle));

        if (started==false&&checklist.IndexOf(checkpointSingle) == 0)
        { 
        timer.Start();
        started = true;
    }

        if (checklist.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            //UnityEngine.Debug.Log(this.gameObject.name + "Correct");
            checkAmount++;
            //UnityEngine.Debug.Log(checklist.IndexOf(checkpointSingle));
            //UnityEngine.Debug.Log(checkAmount);
            //todo: play a sound when u pass checkpoint
            TrackCheckpointsUI yeppers = GetComponentInChildren<TrackCheckpointsUI>();
            brotherman.Hide();
            LastCheckpointSingleIndex = checklist.IndexOf(checkpointSingle);
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checklist.Count;
            if (!finishrace)
            {
                foreach (GameObject car in cars)
                {
                    items.Add(new ITEM { itemObj = car, startPos = car.GetComponent<ThirdPersonCharacterController>().checkAmount });

                }

                items = items.OrderByDescending(w => w.startPos).ToList();

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].itemObj.GetComponent<ThirdPersonCharacterController>().posInRace = i + 1;

                    //UnityEngine.Debug.Log("ITEM" + i + " = " + items[i].startPos);
                }
            }
            
            //OnCorrectCheckpoint?.Invoke(this,EventArgs.Empty);
            if (checklist.IndexOf(checkpointSingle) == checklist.Count - 1&& !finishrace)
            {
                for(int i = 0; i < items.Count; i++)
            {
                    items[i].itemObj.GetComponent<ThirdPersonCharacterController>().posInRace = i + 1;

                    //UnityEngine.Debug.Log("ITEM" + i + " = " + items[i].startPos);
                }

                //UnityEngine.Debug.Log("finished");
                finishrace = true;
                brotherman2.Show();
                canMove = false;
                speedInput = 0f;
                if (singleplayer == false)
                {
                    brotherman2.GetComponentInChildren<TextMeshProUGUI>().text = "Congratulations!\n~" + formatters(posInRace) + " place~";
                }
                else
                {
                    brotherman2.GetComponentInChildren<TextMeshProUGUI>().text = "Congratulations!\n You finished the race!";
                }
                timer.Stop();
                checkAmount+=1;
                showthepositionplace.SetActive(false);
                float timeTaken = (float)timer.Elapsed.TotalMilliseconds;
                if (sceneBestTime == 0 || timeTaken < sceneBestTime)
                {
                    //UnityEngine.Debug.Log("YEP");
                    PlayerPrefs.SetFloat(key, timeTaken);
                    //set the key's new float being time elapsed representing the level's new best time
                    besttime = TimeSpan.FromMilliseconds(PlayerPrefs.GetFloat(key));
                }
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else if (checklist.IndexOf(checkpointSingle) == LastCheckpointSingleIndex)
        {
            //UnityEngine.Debug.Log(this.gameObject.name + "Correct");
            //todo: play a sound when u pass checkpoint

            brotherman.Hide();
        }
        else if (started == false&& checklist.IndexOf(checkpointSingle) == checklist.Count - 1)
        {
            brotherman.Hide();
        }
        else
        {
            if (!finishrace)
            {
                //UnityEngine.Debug.Log("Incorrect");
                OnCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
                brotherman.Show();

               // StartCoroutine(returnplayer());
            }
        }
        
        
        //carsort = new List<ITEMsort>();
        //if (finishrace==true)
       // {
           // foreach (GameObject car in cars)
           // {
           //     carsort.Add(new ITEMsort { itemObj = car, fintime = car.GetComponent<ThirdPersonCharacterController>().howmuchtime() });

           // }
          //  carsort = carsort.OrderByDescending(w => w.fintime).ToList();
          //  for (int i = 0; i < carsort.Count; i++)
          //  {
           //     carsort[i].itemObj.GetComponent<ThirdPersonCharacterController>().posInRace = i + 1;

                //UnityEngine.Debug.Log("ITEM" + i + " = " + items[i].startPos);
         //   }

      //  }
    }
    private void SetName()
    {if (singleplayer)
        {
            nameText.text = "";
        }
        else
        {
            nameText.text = photonView.Owner.NickName;
        }
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
            MapCamera.SetActive(true);
            dustTrail1.gameObject.SetActive(true);
            dustTrail2.gameObject.SetActive(true);
            GetComponentInChildren<Camera>().GetComponent<AudioListener>().enabled = true;
            GetComponent<AudioSource>().enabled = false;


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
        //basically checks all cameras to see if they have layers on them

        if (!p1) //if not, start assigning
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
            // Destroy(GetComponentInChildren<Camera>().gameObject);
        }

        
        
    }
    string formatters(int yo)
    {
        string number="0";
        if (yo == 1)
        {
            number= "1st";
        }
        else if (yo == 2)
        {
            number= "2nd";
        }
        else if (yo == 3)
        {
            number= "3rd";
        }
        else if (yo == 4)
        {
            number="4th";
        }
        return number;
    }
    public bool amIthemaster()
    {
        return PhotonNetwork.IsMasterClient;
    }
    public string thismasterscene()
    {
        Scene scene = SceneManager.GetActiveScene();
        return scene.name;
    }

    public void reload()
    {
        Scene scene = SceneManager.GetActiveScene();
        PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(scene.name);
        
    }
    
        // Update is called once per frame
        void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        
        if (!photonView.IsMine)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (pastplayercount != PhotonNetwork.CurrentRoom.PlayerCount)
            {
               // reload();
                pastplayercount = PhotonNetwork.CurrentRoom.PlayerCount;
            }
        }
        if (photonView.IsMine)
        {
            
            
            speed.text = "Best Time: " + string.Format("{0:00}:{1:00}:{2:00}", besttime.Minutes, besttime.Seconds, besttime.Milliseconds) + "\nYour Time: " + string.Format("{0:00}:{1:00}:{2:00}", timer.Elapsed.Minutes, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds);
            if (GameObject.Find("GameManager").GetComponent<GameManager>().raceStarted == false&& GameObject.Find("GameManager").GetComponent<GameManager>().enoughPlayers == false)
            {
                thewaitingstuff.Show();
                thecountdown.Hide();
            }
            if (GameObject.Find("GameManager").GetComponent<GameManager>().enoughPlayers == true&& GameObject.Find("GameManager").GetComponent<GameManager>().raceStarted == false)
            {
                thewaitingstuff.Hide();
                waitingText.text = ("The race will start in: " + Mathf.Round(GameObject.Find("GameManager").GetComponent<GameManager>().timeLeft));
                thecountdown.Show();
            }
            if (GameObject.Find("GameManager").GetComponent<GameManager>().raceStarted == true&&finishrace==false)
            {
                //timer.Start();
                manmenu.text = "Main Menu";
                if (pausemenushow==false)
                {
                    canMove = true;

                }
                else
                { canMove = false; }
                thewaitingstuff.Hide();
                thecountdown.Hide();
                goStartrace.Show();
                timer.Start();
                //started = true;
            }
            if (GameObject.Find("GameManager").GetComponent<GameManager>().enoughTime == true)
            {
                goStartrace.Hide();
            }
            
            { posRaceText.text = "Position: " + posInRace+"\nShells: "+numberOfShellsLeft; }

            if (canMove == true)
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



                if (Input.GetButton("Fire3"))
                {
                    
                    //UnityEngine.Debug.Log("i pressed r");
                    theBallControl.transform.position = new Vector3(checklist[LastCheckpointSingleIndex].transform.position.x, 1f, checklist[LastCheckpointSingleIndex].transform.position.z);
                    transform.rotation = checklist[LastCheckpointSingleIndex].transform.rotation;

                }
            }
            //if (GameObject.Find("GameManager").GetComponent<GameManager>().raceStarted == true || (GameObject.Find("GameManager").GetComponent<GameManager>().enoughPlayers == false && GameObject.Find("GameManager").GetComponent<GameManager>().raceStarted == false))
            {

                if (Input.GetButtonDown("Cancel"))
                {
                    UnityEngine.Debug.Log(pausemenushow);
                    
                    manmenu.text = "Main Menu";
                    if (pausemenushow == false)
                    {
                        canMove = false;
                        speedInput = 0.0f;
                        //UnityEngine.Debug.Log("bruh1");
                        //UnityEngine.Debug.Log(pausemenushow);
                        pausemenu.SetActive(true);
                        pausemenushow = true;
                        canMove = false;
                    }
                    
                    else if (pausemenushow == true)
                    {
                        //UnityEngine.Debug.Log("bruh2");
                        pausemenu.SetActive(false);
                        pausemenushow = false;
                        
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                //PlayerPrefs.SetFloat(key, 0f);
                PlayerPrefs.DeleteAll();
                besttime = TimeSpan.FromMilliseconds(PlayerPrefs.GetFloat(key));
                //else
                //{
                // SceneManager.LoadScene(sceneName);
                // }


            }
            if (devMode)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    
                        forwardAccel = 8f;
                        reverseAccel = 6f;
                   

                }
                if (Input.GetKeyDown(KeyCode.E))
                {

                    numberOfShellsLeft = 5;

                }
                if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.J))
                {
                    PhotonNetwork.AutomaticallySyncScene = true;
                    PhotonNetwork.LoadLevel(scene.name);
                }
            }
        }
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);
        leftFrontWheel.Rotate(-Vector3.forward * speedInput * Time.deltaTime);
        rightFrontWheel.Rotate(-Vector3.forward * speedInput * Time.deltaTime);
       backwheels.Rotate(Vector3.forward * speedInput * Time.deltaTime);
        transform.position = theRB.transform.position;

        //for Shell projectile
        float h = Input.GetAxisRaw("Horizontal") * moveForce;
        float v = Input.GetAxisRaw("Vertical") * moveForce;

        rbody.velocity = new Vector3(h, v, 0);

        if(Input.GetButtonDown("Fire1"))
        {
            //UnityEngine.Debug.Log("SPACE KEY PRESSED");
            if(Time.time > shootRateTimeStamp)
            {
                if(numberOfShellsLeft > 0)
                {
                    StartCoroutine(CreateShell());
                    numberOfShellsLeft = numberOfShellsLeft - 1;
                }
                // GameObject newShell = (GameObject)Instantiate(shell, shooter.position, shooter.rotation);
                
                
                //newShell.GetComponent<Rigidbody>().velocity = shootForce * (newShell.GetComponent<Rigidbody>().velocity.normalized);
                //UnityEngine.Debug.Log("SHELL SHOT");
                //shootRateTimeStamp = Time.time + shootRate;
            }
        }

        //Audio Stuff
        currentCarSpeed = Mathf.Abs((2 * theRB.velocity.magnitude) / maxSpeed);
        engineCurrentPitch = currentCarSpeed;
        if(engineCurrentPitch < engineMinAudioPitch)
        {
            engineRunningSound.pitch = engineMinAudioPitch;
        }
        else engineRunningSound.pitch = engineCurrentPitch;
    }

    // void onPhotonInstantiate(PhotonMessageInfo info)
    // {
    //     //for Shell projectile

    // }
    public void gotomain()
    { if (GameObject.Find("GameManager").GetComponent<GameManager>().raceStarted == true || (GameObject.Find("GameManager").GetComponent<GameManager>().enoughPlayers == false && GameObject.Find("GameManager").GetComponent<GameManager>().raceStarted == false))
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            manmenu.text = "You cannot leave now!\nThe race will start soon!"; 
        } }
    public void contnue()
    {
        pausemenu.SetActive(false);
        pausemenushow = false;
    }
    public float howmuchtime()
    {
        return (float)timer.Elapsed.TotalMilliseconds;
    }
    private void FixedUpdate()
    {//UnityEngine.Debug.Log(photonView.isMine);
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
                //transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation,
               75f* Time.deltaTime
             );
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
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(90, 0,0), Time.deltaTime*15f);
                //theRB.AddForce(Physics.gravity * 1f, ForceMode.Acceleration);
            }
            
                var emissionModule = dustTrail1.emission;
            var emissionModule1 = dustTrail2.emission;
            emissionModule.rateOverTime = emissionRate;
            emissionModule1.rateOverTime = emissionRate;


        }
        //fix particles (displaying wrong in multiplayer)
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "finish")
        {
            //UnityEngine.Debug.Log("touchedfinish");

        }
        if (other.gameObject.tag == "checkpoint")
        {

            PlayerThroughCheckpoint(other.GetComponent<CheckpointSingle>());

        }
        //if (other.gameObject.tag == "speedboost")
        //{
           // UnityEngine.Debug.Log("SpeedBOOST");
        //    SpeedBoost();
       // }

        if (other.gameObject.tag == "slowpad")
        {
           // UnityEngine.Debug.Log("slowDOWN");
            SlowPad();
        }


    }
    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.tag == "speedboost")
        {
            // UnityEngine.Debug.Log("SpeedBOOST");
            SpeedBoost();
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "funobject")
        {
            // UnityEngine.Debug.Log("yay");
            collision.gameObject.GetComponent<PhotonView>().RequestOwnership();

        }

        if (collision.gameObject.tag == "shell")
        {
            // UnityEngine.Debug.Log("SHELLED HAHAHAHAHA");

            StartCoroutine(ShellCollision());
            Destroy(collision.collider.gameObject);
        }

        if (collision.gameObject.tag == "die")
        {
            // UnityEngine.Debug.Log("DIE ACQUIRED");
            StartCoroutine(AddShell());
            Destroy(collision.collider.gameObject);
        }
    }
        private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "slowpad")
        {
            forwardAccel = 8f;
            reverseAccel = 6f;
        }
        
    }
    public void showBeginRace()
    { }

    IEnumerator AddShell()
    {
        numberOfShellsLeft += 1;

        shellAddedText.SetActive(true);
        yield return new WaitForSeconds(2f);
        shellAddedText.SetActive(false);    
    }
    IEnumerator returnplayer()
    {
        

        
        yield return new WaitForSeconds(3f);
        theBallControl.transform.position = new Vector3(checklist[LastCheckpointSingleIndex].transform.position.x, 1f, checklist[LastCheckpointSingleIndex].transform.position.z);
        transform.rotation = checklist[LastCheckpointSingleIndex].transform.rotation;
    }

    IEnumerator CreateShell()
    {
        GameObject newShell = (GameObject)PhotonNetwork.Instantiate(shell.name, new Vector3(shooter.transform.position.x, shooter.transform.position.y, shooter.transform.position.z), Quaternion.identity, 0);
        newShell.GetComponent<Rigidbody>().AddForce(shooter.forward * shootForce);
        yield return new WaitForSeconds(15f);
        Destroy(newShell);
    }

    private IEnumerator PlayEngineSounds()
    {
        startupSound.mute = false;
        yield return new WaitForSeconds(1.1f);
        engineRunningSound.mute = false;
    }

    IEnumerator ShellCollision()
    {
        forwardAccel = 1f;
        reverseAccel = 1f;
        yield return new WaitForSeconds(3f);
        forwardAccel = 8f;
        reverseAccel = 6f;

    }
    
    void SpeedBoost()
    {//8000f replaced speedInput
        theRB.AddForce(transform.forward *1500f * boostMultiplier);
        //forwardAccel = 15f;
    }

    void SlowPad()
    {
        // theRB.AddForce(transform.forward * speedInput * slowMultiplier);
        forwardAccel = 1f;
        reverseAccel = 1f;
    }
    void OnGUI()
    {
        if (photonView.IsMine)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperRight;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.white;
            float msec = PhotonNetwork.GetPing();
            float fps = 1.0f / Time.deltaTime;
            string text = string.Format("Ping: {0:0.0} ms \n {1:0.} fps", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}

