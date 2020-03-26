using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PausedMenu : MonoBehaviour
{
    private GameObject pauseMenu;
    public GameObject Player;
    private Player2DControll playerControll;
    private GameObject panelControll;
    public bool isPaused = false;
    private bool primeraVegada;
    private bool startTime = false;
    private bool started = false;

    void Awake()
    {
        
    }

    private void Start()
    {
        playerControll = Player.GetComponent<Player2DControll>();
        panelControll = GameObject.Find("panelControll");
        pauseMenu = GameObject.Find("pauseMenu");
        DesactivarMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                isPaused = false;
                CambiarPausa();
            }
            else
            {
                isPaused = true;
                ActivarMenu();
            }
        }
    }
    public void ActivarMenu() 
    {
        playerControll.enabled = false;
        Time.timeScale = 0;
        //AudioListener.pause = true;
        if (primeraVegada){
            pauseMenu.SetActive(true);
            primeraVegada = false;
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Screen.lockCursor = false;
    }
    public void DesactivarMenu()
    {
        playerControll.enabled = true;
        Time.timeScale = 1;
        AudioListener.pause = false;
        panelControll.SetActive(false);
        pauseMenu.SetActive(false);
        primeraVegada = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void CambiarPausa() 
    {
        isPaused = false;
        DesactivarMenu();
    }

}
