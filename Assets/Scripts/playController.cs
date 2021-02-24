using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class playController : MonoBehaviour {

    public VideoPlayer videoPlayer;
    

    //public VideoClip videoClip;

    
    public Button play;           

    public Sprite playicon;
    public Sprite pauseicon;


    public Button fornt;          
    public Button back;

    private void Start()
    {
        
    }
    /// <summary>
    /// PlayOrPause
    /// </summary>
    public void PlayOrPause()
    {
        if (videoPlayer.isPlaying==true)
        {
            videoPlayer.Pause();
            
            play.GetComponent<Image>().sprite = playicon;
        }
        else
        {
            videoPlayer.Play();
            play.GetComponent<Image>().sprite = pauseicon;
        }
    }

    /// <summary>
    /// fornt
    /// </summary>
    public void Fornt()
    {
        videoPlayer.time += 10f;
    }
    /// <summary>
    /// back
    /// </summary>
    public void Back()
    {
        videoPlayer.time -= 10f;
    }


}
