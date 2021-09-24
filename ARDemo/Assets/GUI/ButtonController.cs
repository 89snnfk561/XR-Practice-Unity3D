using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    Button button;
    Image image;
    public bool on_off { get; set; }
    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }
    void Start()
    {
        on_off = false;
        image.color = Color.gray;
        button.onClick.AddListener(Click);
    }

    public void ButtonReset()
    {
        on_off = false;
        image.color = Color.gray;
    }
    
    

    private void Click()
    {
        if (on_off == true)
        {
            on_off = false;
            image.color = Color.gray;
        }
        else if(on_off == false)
        {
            on_off = true;
            image.color = Color.white;
        }
    }
}


