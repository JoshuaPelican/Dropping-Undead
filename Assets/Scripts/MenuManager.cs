using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class MenuManager : MonoBehaviour
{
    #region Singleton
    public static MenuManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject pausePanel;
    public GameObject deathPanel;
    public TextMeshProUGUI zombiesKilledText;
    public TextMeshProUGUI wavesBeatenText;
    public TextMeshProUGUI bulletsFiredText;
    public TextMeshProUGUI headshotsHitText;

    public int zombiesKilled;
    public int wavesBeaten = -1;
    public int bulletsFired;
    public int headshotsHit;

    private void Start()
    {
        AudioListener.volume = .75f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !deathPanel.activeSelf)
        {
            if (pausePanel.activeSelf)
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                pausePanel.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                pausePanel.SetActive(true);
            }
        }
    }

    public void LoadDeathStats()
    {
        pausePanel.SetActive(true);
        pausePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dead";

        deathPanel.SetActive(true);

        zombiesKilledText.text = "Undead Dropped: " + zombiesKilled;
        wavesBeatenText.text = "Waves Survived: " + wavesBeaten;
        bulletsFiredText.text = "Bullets Fired: " + bulletsFired;
        headshotsHitText.text = "Headshots Landed: " + headshotsHit;
    }

    public void UpdateMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
}
