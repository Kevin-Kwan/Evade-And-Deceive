using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelSelectMenuScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string sceneName;
    public AudioSource bgm;
    public Image pointer;

    public Text option1;
    public Text option2;
    public Text option3;
    public Text option4;
    public Slider bruh;
    private bool volumeSelected = false;

    private int numberOfOptions = 2;

    private int selectedOption;
    private bool axisInUse = false;
    private bool horizaxisInUse = false;
    public MenuControllerSingle whatface;
    // Use this for initialization
    void Start()
    {
        selectedOption = 1;
        option1.color = Color.red;
        option2.color = Color.white;


        pointer.transform.position = new Vector3(0, option1.transform.position.y);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Application.targetFrameRate = 300;
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(sceneName);
    }
    public void levselect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    public void lobselect()
    {
        SceneManager.LoadScene("LobbySelect");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        option1.color = Color.white; //Make sure all others will be black (or do any visual you want to use to indicate this)
        option2.color = Color.white;

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetAxisRaw("VerticalPad") > 0 && axisInUse == false || Input.GetAxisRaw("Horizontal") < 0 && horizaxisInUse == false)
        { //Input telling it to go up or down.
            axisInUse = true;
            horizaxisInUse = true;
            selectedOption += 1;
            if (selectedOption > numberOfOptions) //If at end of list go back to top
            {
                selectedOption = 1;
            }

            option1.color = Color.white; //Make sure all others will be black (or do any visual you want to use to indicate this)
            option2.color = Color.white;


            switch (selectedOption) //Set the visual indicator for which option you are on.
            {
                case 1:
                    option1.color = Color.red;
                    pointer.transform.position = new Vector3(option1.transform.position.x, 0); volumeSelected = false;
                    break;
                case 2:
                    option2.color = Color.red;
                    pointer.transform.position = new Vector3(option1.transform.position.x, 0); volumeSelected = false;
                    break;
                
            }


        }


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D) || Input.GetAxisRaw("VerticalPad") < 0 && axisInUse == false || Input.GetAxisRaw("Horizontal") > 0 && horizaxisInUse == false)
        { //Input telling it to go up or down.
            axisInUse = true;
            horizaxisInUse = true;
            selectedOption -= 1;
            if (selectedOption < 1) //If at end of list go back to top
            {
                selectedOption = numberOfOptions;
            }

            option1.color = Color.white; //Make sure all others will be black (or do any visual you want to use to indicate this)
            option2.color = Color.white;


            switch (selectedOption) //Set the visual indicator for which option you are on.
            {
                case 1:
                    option1.color = Color.red;
                    pointer.transform.position = new Vector3(option1.transform.position.x, 0);
                    volumeSelected = false;
                    break;
                case 2:
                    option2.color = Color.red;
                    pointer.transform.position = new Vector3(option1.transform.position.x, 0); volumeSelected = false;
                    break;

            }
        }

       
        if (Input.GetAxisRaw("VerticalPad") == 0)
        {
            axisInUse = false;

        }
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            horizaxisInUse = false;

        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
        {
            Debug.Log("Picked: " + selectedOption); //For testing as the switch statment does nothing right now.

            switch (selectedOption) //Set the visual indicator for which option you are on.
            {
                case 1:
                    whatface.CreateSingleGameLevel1();
                    break;
                case 2:
                    whatface.CreateSingleGameLevel2();
                    break;

            }
        }


        if (Input.GetKeyDown("escape") || Input.GetButtonDown("GoBack") || Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        //else
        //{
        // SceneManager.LoadScene(sceneName);
        // }

    }
}
