using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    bool setthevolumeyet = false;
    Slider volumeSlider;
    int firstRun = 0;
    int actualfirstrun;
    private void Start()
    {
        firstRun = PlayerPrefs.GetInt("savedFirstRun");
        actualfirstrun = PlayerPrefs.GetInt("actualfirstRun");
        UnityEngine.Debug.Log(PlayerPrefs.GetInt("savedFirstRun"));
        UnityEngine.Debug.Log(PlayerPrefs.GetInt("actualfirstRun"));
        if (PlayerPrefs.GetInt("savedFirstRun") == 0&&actualfirstrun==0) 
        {
            GetComponent<Slider>().value = 1f;
            PlayerPrefs.SetFloat("volume", 1f);
            PlayerPrefs.SetInt("savedFirstRun", 1);
            PlayerPrefs.SetInt("actualfirstRun", 1);
            //UnityEngine.Debug.Log(PlayerPrefs.GetInt("savedFirstRun"));
        }
       // changeVolume(PlayerPrefs.GetInt("savedFirstRun"));
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("volume");

    }
    // Start is called before the first frame update
    public void changeVolume(float newVolume)
     {
         PlayerPrefs.SetFloat("volume", newVolume);
         AudioListener.volume = PlayerPrefs.GetFloat("volume");
     }

    // Update is called once per frame
    void Update()
    {
        changeVolume(GetComponent<Slider>().value);
        GetComponent<Slider>().value= PlayerPrefs.GetFloat("volume");
    }
}
