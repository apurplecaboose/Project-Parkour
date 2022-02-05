using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LevelLoader : MonoBehaviour
{
   public static LevelLoader instance;
   public GameObject LoaderCanvas;
    public Image Sliderbar;
   private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public async void LoadScene(int sceneNumb)
    {
        var scene = SceneManager.LoadSceneAsync(sceneNumb);
        scene.allowSceneActivation = false;
        LoaderCanvas.SetActive(true);
        do
        {
                await Task.Delay(100);
            Sliderbar.fillAmount = scene.progress;
        }
        while (scene.progress < 0.9f);
        scene.allowSceneActivation = false;
        LoaderCanvas.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
