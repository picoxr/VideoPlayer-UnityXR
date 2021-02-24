using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SwitchScenes : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void LoadScence1()
    {
        SceneManager.LoadScene("3D-side-by-side");
    }
    public void LoadScence2()
    {
        SceneManager.LoadScene("360player");
    }
    public void LoadScence3()
    {
        SceneManager.LoadScene("3D-over-under");
    }
    public void QuitScence()
    {
        SceneManager.LoadScene("home");
    }
}
