using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class MenuControllerSingle : MonoBehaviourPunCallbacks
{
    [SerializeField] private string VersionName = "0.1";
    string level = "TrackSongle";
    [SerializeField] private GameObject errorcool;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private GameObject error1cool;
    [SerializeField] private TextMeshProUGUI error1Message;
    [SerializeField] private TextMeshProUGUI loadMessage;
    [SerializeField] private GameObject loadcool;
    [SerializeField] private GameObject StartButton;
    bool areuconnected;
    public LevelSelectMenuScript bruh;
    // Start is called before the first frame update

    void Start()
    {
        UnityEngine.Debug.Log(PhotonNetwork.IsConnected);
        
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connected");
            PhotonNetwork.ConnectUsingSettings();
            areuconnected = true;
        }

    }

    private void Awake()
    {


    }
    public override void OnConnectedToMaster()
     {
        PhotonNetwork.JoinLobby();
        Debug.Log("Connected");
    areuconnected = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape") || Input.GetButtonDown("GoBack") || Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (errorcool.activeSelf || loadcool.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                bruh.enabled = false;
                errorDismiss();
                error1Dismiss();
                loadmessageDismiss();
                bruh.enabled = true;
            }
        }
        //else
        //{
        // SceneManager.LoadScene(sceneName);
        // }

    }
    public void ChangeUserNameInput()
    {
        

    }
    public void SetUserName()
    {
        
    }
    public void CreateGame()
    {
        
           
        
    }
    public void CreateSingleGameLevel1()
    {
        areuconnected = PhotonNetwork.IsConnected;
        if (areuconnected == false)
        {
            errorMessage.text = "Error: Please ensure that you are connected to the Internet and no firewalls are blocking your connection!";
            errorcool.SetActive(true);
            loadmessageDismiss();
        }
        else
        {
            level = "TrackSongle";
            loadcool.SetActive(true);
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 1 }, null);
        }
        
    }
    public void CreateSingleGameLevel2()
    {
        areuconnected = PhotonNetwork.IsConnected;
        if (areuconnected == false)
        {
            errorMessage.text = "Error: Please ensure that you are connected to the Internet and no firewalls are blocking your connection!";
            errorcool.SetActive(true);
            loadmessageDismiss();
        }
        else
        {
            level = "OriginalTrack";
            loadcool.SetActive(true);
            // = 15000;
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 1 }, null);
        }
    }
    public void errorDismiss()

    {
        errorcool.SetActive(false);
    }
    public void error1Dismiss()

    {
        error1cool.SetActive(false);
    }
    public void loadmessageDismiss()

    {
        loadcool.SetActive(false);
    }
    public void JoinGame()
    {
       


    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorMessage.text = "Error: Please ensure that you are connected to the Internet and no firewalls are blocking your connection!";
        errorcool.SetActive(true);
        loadmessageDismiss();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        errorMessage.text = "Error: Please ensure that you are connected to the Internet and no firewalls are blocking your connection!";
        areuconnected = false;
        errorcool.SetActive(true);
        loadmessageDismiss();
    }
    public override void OnJoinedRoom()
    {
        // = 0;
        PhotonNetwork.LoadLevel(level);
    }
}
