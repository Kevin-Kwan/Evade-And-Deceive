using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string sceneName;
    public AudioSource bgm;
    public Image pointer;

    public Text option1;
    public Text option2;
    public Text option3;
    public Slider bruh;
    private bool volumeSelected = false;
    public ThirdPersonCharacterController parentcar;
    private int numberOfOptions = 3;

    private int selectedOption;
    private bool axisInUse = false;
    private bool horizaxisInUse = false;
    // Use this for initialization
    void Start()
    {
        selectedOption = 1;
        option1.color = Color.red;
        option2.color = Color.white;
        option3.color = Color.white;
        

        pointer.transform.position = new Vector3(0, option1.transform.position.y);
      
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        option1.color = Color.white; //Make sure all others will be black (or do any visual you want to use to indicate this)
        option2.color = Color.white;
        option3.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetAxisRaw("VerticalPad") > 0 && axisInUse == false)
        { //Input telling it to go up or down.
            axisInUse = true;
            selectedOption += 1;
            if (selectedOption > numberOfOptions) //If at end of list go back to top
            {
                selectedOption = 1;
            }

            option1.color = Color.white; //Make sure all others will be black (or do any visual you want to use to indicate this)
            option2.color = Color.white;
            option3.color = Color.white;

            switch (selectedOption) //Set the visual indicator for which option you are on.
            {
                case 1:
                    option1.color = Color.red;
                    pointer.transform.position = new Vector3(0, option1.transform.position.y); volumeSelected = false;
                    break;
                case 2:
                    option2.color = Color.red;
                    pointer.transform.position = new Vector3(0, option2.transform.position.y); volumeSelected = true;
                    break;
                case 3:
                    option3.color = Color.red;
                    pointer.transform.position = new Vector3(0, option3.transform.position.y); volumeSelected = false;
                    break;
                
            }


        }


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetAxisRaw("VerticalPad") < 0 && axisInUse == false)
        { //Input telling it to go up or down.
            axisInUse = true;
            selectedOption -= 1;
            if (selectedOption < 1) //If at end of list go back to top
            {
                selectedOption = numberOfOptions;
            }

            option1.color = Color.white; //Make sure all others will be black (or do any visual you want to use to indicate this)
            option2.color = Color.white;
            option3.color = Color.white;

            switch (selectedOption) //Set the visual indicator for which option you are on.
            {
                case 1:
                    option1.color = Color.red;
                    pointer.transform.position = new Vector3(0, option1.transform.position.y);
                    volumeSelected = false;
                    break;
                case 2:
                    option2.color = Color.red;
                    pointer.transform.position = new Vector3(0, option2.transform.position.y); volumeSelected = true;
                    break;
                case 3:
                    option3.color = Color.red;
                    pointer.transform.position = new Vector3(0, option3.transform.position.y); volumeSelected = false;
                    break;
               
            }
        }

        if (volumeSelected)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetAxisRaw("Horizontal") < 0 && horizaxisInUse == false)
            { //Input telling it to go up or down.
                bruh.GetComponent<Slider>().value -= 0.1f;
                horizaxisInUse = true;

            }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetAxisRaw("Horizontal") > 0 && horizaxisInUse == false)
            { //Input telling it to go up or down.
                bruh.GetComponent<Slider>().value += 0.1f;
                horizaxisInUse = true;
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
                    parentcar.contnue();
                    break;
                case 2:
                    
                    break;
                case 3:
                    parentcar.gotomain();
                    break;
            }
        }


        if (Input.GetKeyDown("escape") || Input.GetButtonDown("GoBack"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        //else
        //{
        // SceneManager.LoadScene(sceneName);
        // }

    }
}
