using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;



public class MenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] private string VersionName = "0.1";
    [SerializeField] private GameObject usernameMenu;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private GameObject ConnectPanel;
    [SerializeField] private InputField Username;
    [SerializeField] private InputField CreateGameInput;
    [SerializeField] private InputField JoinGameInput;
    [SerializeField] private GameObject errorcool;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private GameObject error1cool;
    [SerializeField] private TextMeshProUGUI loadMessage;
    [SerializeField] private GameObject loadcool;
    [SerializeField] private TextMeshProUGUI error1Message;
    [SerializeField] private GameObject prefillname;
    [SerializeField] private GameObject clearusername;
    private string prevname;
    bool areuconnected;
    string levelname="Track";

    public LobbySelectMenuScriptMore bruh;
    public LobbySelectMenuScript bruh1;
    [SerializeField] private GameObject StartButton;
    // Start is called before the first frame update

    void Start()
    {
        UnityEngine.Debug.Log(PhotonNetwork.IsConnected);
        PhotonNetwork.AutomaticallySyncScene = true;
        Text yuh=prefillname.GetComponent<Text>();
        string name = PlayerPrefs.GetString("previoususername");
        
            Username.text= name;
        
        
        usernameMenu.SetActive(true);
        lobbyMenu.SetActive(false);
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connected");
            PhotonNetwork.ConnectUsingSettings();
            areuconnected = true;
        }
        
        
        errorcool.SetActive(false);
        error1cool.SetActive(false);

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
    public void setLevel1()
    {
        areuconnected = PhotonNetwork.IsConnected;
        levelname = "Track";
        errorMessage.text = "Selected Map 1! (For room creation only)";
        errorcool.SetActive(true);
    }
    public void setLevel2()
    {
        areuconnected = PhotonNetwork.IsConnected;
        levelname = "OriginalTrackYay" ;

        errorMessage.text = "Selected Map 2! (For room creation only)";
        errorcool.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {



        if (Input.GetKeyDown("escape") || Input.GetButtonDown("GoBack") || Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        if (errorcool.activeSelf || error1cool.activeSelf || loadcool.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                bruh.enabled = false;
                bruh1.enabled = false;
                errorDismiss();
                error1Dismiss();
                loadmessageDismiss();
                bruh.enabled = true;
                bruh1.enabled = true;
            }
        }

    }
    public void ChangeUserNameInput()
    {
        if (Username.text.Length >= 1)
        {
            StartButton.SetActive(true);
            clearusername.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
            clearusername.SetActive(false);
        }

    }
    public void SetUserName()
    {
        areuconnected = PhotonNetwork.IsConnected;
        usernameMenu.SetActive(false);
        lobbyMenu.SetActive(true);
        PhotonNetwork.NickName = Username.text;
        PlayerPrefs.SetString("previoususername", Username.text);

    }

    public void ClearUserName()
    {
        areuconnected = PhotonNetwork.IsConnected;
        Username.text="";
        

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        errorMessage.text = "Error: Please ensure that you are connected to the Internet and no firewalls are blocking your connection!";
        errorcool.SetActive(true);
        areuconnected = false;
        loadmessageDismiss();
    }
    
    public void SetAsPastUserName()
    {if (PhotonNetwork.NickName != "")
        {
            usernameMenu.SetActive(false);
            PhotonNetwork.NickName = PhotonNetwork.NickName;
        }
    else
        {
            error1Message.text = "This is your first time playing! You never came up with a name! Please set a username first.";
            error1cool.SetActive(true);
        }
    }
    public void CreateGame()
    {
        areuconnected = PhotonNetwork.IsConnected;
        
        if (areuconnected==false)
        {
            errorMessage.text = "Error: Please ensure that you are connected to the Internet and no firewalls are blocking your connection!";
            errorcool.SetActive(true);
            loadmessageDismiss();
        }
        else if (CreateGameInput.text == "")
        {
            errorMessage.text = "Failed to create a room! Did you forget to choose the lobby name?";
            errorcool.SetActive(true);
        }
        else
        {
            loadcool.SetActive(true);
            // = 15000;
            PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
        }
    }
    
    IEnumerator waiting()
    {
        
        yield return new WaitForSeconds(3);
        
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        
        
        errorMessage.text = "Failed to join a room! No rooms exist! Create one instead!";
        errorcool.SetActive(true);
        loadmessageDismiss();


    }
    public void errorDismiss()
        
    {
        errorcool.SetActive(false) ;
    }
    public void loadmessageDismiss()

    {
        loadcool.SetActive(false);
    }
    public void error1Dismiss()

    {
        error1cool.SetActive(false);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorMessage.text = "Failed to create room! There's already one that exists!";
        errorcool.SetActive(true);
        loadmessageDismiss();


    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorMessage.text = "Failed to join room! The race is already in progress!";
        errorcool.SetActive(true);
        loadmessageDismiss();




    }
    public void JoinGame()
    {
        areuconnected = PhotonNetwork.IsConnected;
        if (areuconnected == false)
        {
            errorMessage.text = "Error: Please ensure that you are connected to the Internet and no firewalls are blocking your connection!";
            errorcool.SetActive(true);
            loadmessageDismiss();
        }
        else if (CreateGameInput.text == "")
        {
            errorMessage.text = "Failed to create or join a room! Did you forget to input the lobby name?";
            errorcool.SetActive(true);
        }
        else
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;

            loadcool.SetActive(true);
            // = 15000;
            PhotonNetwork.JoinOrCreateRoom(CreateGameInput.text, roomOptions, TypedLobby.Default);
        }

    }
    public void JoinARandomGame()
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
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            loadcool.SetActive(true);
            // = 15000;
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnJoinedRoom()
    {
        // = 0;

        PhotonNetwork.AutomaticallySyncScene = true;
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Active scene is '" + scene.name + "'.");
        PhotonNetwork.LoadLevel(levelname);
    }
}
