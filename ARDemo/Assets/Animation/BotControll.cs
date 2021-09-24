using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotControll : MonoBehaviour
{
    Animator animator;
    const int n = 3;
    string[] ls = new string[n]
    {
        "Squat",
        "Pushup",
        "Situp"
    };
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    private void Update()
    {
        if (AnimaData.q)
        {
            AnimateReset();
            animator.SetBool(ls[0], true);
        }
        if (AnimaData.p)
        {
            AnimateReset();
            animator.SetBool(ls[1], true);
        }
        if (AnimaData.s)
        {
            AnimateReset();
            animator.SetBool(ls[2], true);
        }
    }

    private void AnimateReset()
    {
        for(int i = 0; i < n; i++)
        {
            animator.SetBool(ls[i], false);
        }
    }
}
static class AnimaData
{
    public static bool q;
    public static bool p;
    public static bool s;
}