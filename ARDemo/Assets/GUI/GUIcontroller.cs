using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIcontroller : MonoBehaviour
{
    public Button list;
    public Button Squat;
    public Button Pushup;
    public Button Situp;

    ButtonController ls;
    ButtonController squat;
    ButtonController push;
    ButtonController sit;
    private void Start()
    {
        list.onClick.AddListener(menu);
        Squat.onClick.AddListener(Sq);
        Pushup.onClick.AddListener(Pu);
        Situp.onClick.AddListener(Si);
        ls = list.GetComponent<ButtonController>();
        squat = Squat.GetComponent<ButtonController>();
        push = Pushup.GetComponent<ButtonController>();
        sit = Situp.GetComponent<ButtonController>();
    }
    private void Sq()
    {
        if (squat.on_off)
        {
            AnimaData.q = false;
        }
        else
        {
            push.ButtonReset();
            sit.ButtonReset();
            AnimaData.q = true;
            AnimaData.p = false;
            AnimaData.s = false;
        }
    }
    private void Pu()
    {
        if (push.on_off)
        {
            AnimaData.p = false;
        }
        else
        {
            squat.ButtonReset();
            sit.ButtonReset();
            AnimaData.q = false;
            AnimaData.p = true;
            AnimaData.s = false;
        }
    }
    private void Si()
    {
        if (sit.on_off)
        {
            AnimaData.s = false;
        }
        else
        {
            squat.ButtonReset();
            push.ButtonReset();
            AnimaData.q = false;
            AnimaData.p = false;
            AnimaData.s = true;
        }
    }

    private void menu()
    {
        if (ls.on_off)
        {
            Squat.gameObject.SetActive(false);
            Pushup.gameObject.SetActive(false);
            Situp.gameObject.SetActive(false);
        }
        else
        {
            Squat.gameObject.SetActive(true);
            Pushup.gameObject.SetActive(true);
            Situp.gameObject.SetActive(true);
        }
    }

    public static bool GameIsPause = false;

    public GameObject PauseMenuUI;

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPause = false;
    }
    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPause = true;
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
