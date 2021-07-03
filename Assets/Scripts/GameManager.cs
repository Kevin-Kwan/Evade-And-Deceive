using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Photon.Pun;
using TMPro;
using System.Linq;
using System.Timers;
using UnityEngine.SceneManagement;

//todo instead of ready up, add different cars to spawn in (find prefab by name ig)

public class GameManager : MonoBehaviour
{

    public GameObject PlayerPrefab;
    //public GameObject spherePrefab;
    public GameObject GameCanvas;
    public GameObject SceneCamera;
    public GameObject playerWait;
    int numPlayers;
    public bool p1 = false;
    public bool p2 = false;
    public bool p3 = false;
    public bool p4 = false;
    int intTimes = 0;
    public bool enoughPlayers = false;
    public bool raceStarted = false;
    public float timeLeft;
    public bool enoughTime = false;
    bool singleplayer;
    public bool haventrun2 = false;
    public bool haventrun3 = false;
    bool haventSynced = true;
    bool spawned = false;
    public AudioClip clipfor1;
    public AudioClip clipfor2;

    //public string itemName { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        haventSynced = true;
        // UnityEngine. Debug.Log("bruh");
        if (SceneManager.GetActiveScene().name == "TrackSongle" || SceneManager.GetActiveScene().name == "OriginalTrack")
        {
            singleplayer = true;
            SpawnPlayer();
        }
        //if (PhotonNetwork.IsMasterClient)
        //{ SpawnPlayer(); }
        AudioSource audio = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name == "TrackSongle" || SceneManager.GetActiveScene().name == "Track")
        { audio.clip = clipfor1; }
        if (SceneManager.GetActiveScene().name == "OriginalTrack" || SceneManager.GetActiveScene().name == "OriginalTrackYay")
        { audio.clip = clipfor2; }
        audio.Play();
        
    }
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        GameCanvas.SetActive(true);
        numPlayers = 0;
        numPlayers = PhotonNetwork.PlayerList.Length;


    }
    public void SpawnPlayer()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag("car");
        if (raceStarted == false)
        {
            float randomValue = Random.Range(-3.5f, 3.5f);
            GameObject bruh = (GameObject)PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(PlayerPrefab.transform.position.x + randomValue, PlayerPrefab.transform.position.y, PlayerPrefab.transform.position.z), Quaternion.identity, 0);
            intTimes++;
            if (intTimes == 4)
            { intTimes = 0; }
            Scene scene = SceneManager.GetActiveScene();

            spawned = true;
            //camera.cullingMask &= ~(1 << 14);
            GameCanvas.SetActive(false);
            SceneCamera.SetActive(false);


            //Debug.Log(PhotonNetwork.PlayerList.Length);




            //camera.cullingMask &= ~(1 << 14);
            //camera.cullingMask &= ~(1 << 14);
            //if playercount=3


        }
        else
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
    }
    IEnumerator ExampleCoroutine()
    {
        //Print the time of when the function is first called.

        //UnityEngine.Debug.Log("yo");
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        //After we have waited 5 seconds print the time again.

    }
    // Update is called once per frame
    void Update()
    {
        //UnityEngine.Debug.Log(raceStarted);

        GameObject[] cars = GameObject.FindGameObjectsWithTag("car");
        if (singleplayer == false && cars.Length >= 2)
        {
            enoughPlayers = true;


            //have a time variable in this class, call method in each car to display
        }
        if (cars.Length == 3 && haventrun2 == false)
        {
            raceStarted = false;
            enoughPlayers = false;
            timeLeft = 15f;
            StartCoroutine(ExampleCoroutine());
            enoughPlayers = true;
            haventrun2 = true;
        }
        if (cars.Length == 4 && haventrun3 == false)
        {
            raceStarted = false;
            enoughPlayers = false;
            timeLeft = 15f;
            StartCoroutine(ExampleCoroutine());
            enoughPlayers = true;
            haventrun3 = true;
        }
        if (singleplayer == true && cars.Length >= 1)
        {

            enoughPlayers = true;


            //have a time variable in this class, call method in each car to display
        }
        if (singleplayer == false && cars.Length < 2 && raceStarted == false)
        {
            enoughPlayers = false;
            timeLeft = 15f;


            //have a time variable in this class, call method in each car to display
        }

        //UnityEngine.Debug.Log(cars.Length);
        if (enoughPlayers)
        {

            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                raceStarted = true;
            }
            if (enoughPlayers && cars.Length == 4)
            { PhotonNetwork.CurrentRoom.IsOpen = false; }
            if (timeLeft < -3)
            {
                enoughTime = true;
                //PhotonNetwork.CurrentRoom.IsOpen = false;
                //PhotonNetwork.CurrentRoom.IsVisible = false;
            }

        }
        if (spawned == false && raceStarted == true)
        {
            SpawnPlayer();
        }

        //UnityEngine.Debug.Log(enoughPlayers);
        //UnityEngine.Debug.Log(timeLeft);
        if (raceStarted==true)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

        }
    }



    //todo: when there are 2 players, stick all players in a box in the beginning, wait 60 seconds before start
    //todo: position in a race: store number of correct checkpoints passed, (find all Cars, store them,then store variable) sort list of variables, https://forum.unity.com/threads/sort-a-list-of-a-class-by-a-int-in-it.224642/
    //change the itemName and itemType to stuff like gameobject ig and then sort and then rewrite variables in each of the game objects, prob use for each to add
}
