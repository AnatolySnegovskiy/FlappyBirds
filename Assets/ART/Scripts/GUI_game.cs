using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUI_game : MonoBehaviour
{
    static public GUI_game Gui_instance;
    public float pauseTimeScale = 0.0F;
    public bool isMainMenu = false;
    public bool isPaused = false;
    static int max_value_score = 0;
    public int Max_value_score
    {
        get
        {
            return max_value_score;
        }
        set
        {
            max_value_score = value;
        }
    }

    public int value_score = 0;
    static bool Restart_scen=false;
    public string user_name;
    GameObject input_name, input_menu;
    GameObject pause_menu;
    GameObject score_ui;
    GameObject max_score_ui;
    GameObject play;
    public GameObject Table_score;
    long childScore;
    string nikname;
    public GameObject Play
    {
        get
        {
            return play;
        }
        set
        {
            play = value;
        }
    }
    // Use this for initialization
    void Start()
    {
        user_name = PlayerPrefs.GetString("Player_name");

            input_menu = GameObject.Find("Input_menu");
        input_name = GameObject.Find("Text_name");
        Table_score = GameObject.Find("Table_score");
        Table_score.transform.parent.gameObject.SetActive(false);
        Gui_instance = this;
        pause_menu = GameObject.Find("Pause_menu");
        Play = GameObject.Find("Play");
        score_ui = GameObject.Find("Score").transform.GetChild(0).gameObject;
        max_score_ui = GameObject.Find("Max_score").transform.GetChild(0).gameObject;
        if (!Restart_scen)
        {
            TogglePause();
        }
        else
        {
            pause_menu.SetActive(false);
            input_menu.SetActive(false);
        }

        if (PlayerPrefs.GetString("Player_name") == "")
        {
            pause_menu.SetActive(false);
        }
        else
        {
            input_menu.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (value_score > max_value_score)
        {

            max_value_score = value_score;
            UIHandler.FireBase.AddScore(PlayerPrefs.GetString("Player_name"), max_value_score);

        }
        max_score_ui.GetComponent<Text>().text = max_value_score.ToString();
        
    }


    public void UpScore()
    {
        value_score++;
        score_ui.GetComponent<Text>().text = value_score.ToString();
    }

    public void Restart()
    {
        Play.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Pause(false);
        value_score = 0;
        Restart_scen = true;
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Pause(false);
        }
        else
        {
            Pause(true);
        }
    }

    public void Pause(bool doPause)
    {

        if (isPaused && !doPause)
        {
            isPaused = false;
            Time.timeScale = 1.0F;
            pause_menu.SetActive(isPaused);
        }
        else if (!isPaused && doPause)
        {
            isPaused = true;
            Time.timeScale = pauseTimeScale;
            pause_menu.SetActive(isPaused);
           
        }
    }

    public void Create_user()
    {
        PlayerPrefs.SetString("Player_name", input_name.GetComponent<Text>().text);
    }

}
