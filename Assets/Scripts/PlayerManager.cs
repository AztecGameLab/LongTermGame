﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager instance { get { return _instance; } }
    public float Health = 100;

    [HideInInspector]
    public PlayerMovement s_playerMovement;
    [HideInInspector]
    public Looking s_looking;
    [HideInInspector]
    public PlayerShooting s_playerShooting;
    [HideInInspector]
    public PlayerInteract s_playerInteract;

    private AudioManager audioManager;
    public Sound pullBow;
    public Sound shootArrow;
    public Sound music;

    Animator bowAnimator;

    public GameObject[] DevArrowSelection;
    public GameObject loreDisplay;

    private void Awake()
    {
        audioManager = AudioManager.Instance();
        bowAnimator = GetComponentInChildren<Animator>();

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogError("Multiple Players found. Destroying extra players");
        }
        else
        {
            _instance = this;
        }

        s_playerMovement = GetComponentInChildren<PlayerMovement>();
        if (!s_playerMovement)
            Debug.LogError("PlayerMovement component missing");

        s_looking = GetComponentInChildren<Looking>();
        if (!s_looking)
            Debug.LogError("Looking component missing");

        s_playerShooting = GetComponentInChildren<PlayerShooting>();
        if (!s_playerShooting)
            Debug.LogError("PlayerShooting component missing");

        s_playerInteract = GetComponentInChildren<PlayerInteract>();
        if (!s_playerInteract)
            Debug.LogError("PlayerInteract component missing");
    }

    void Start()
    {
        audioManager.PlaySound(music);
    }

    void Update()
    {
        DevArrows();
        GetInput();


    }

    void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            audioManager.PlaySound(pullBow);
            bowAnimator.SetTrigger("Draw");
        }


        if (Input.GetMouseButtonUp(0))
        {
            audioManager.StopSound(pullBow);
            audioManager.PlaySound(shootArrow);
            bowAnimator.SetTrigger("Shoot");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!loreDisplay.activeSelf)
                s_playerInteract.TryInteract();
            else
                HideLore();
        }
    }


    public void DisplayLore(string text)
    {
        loreDisplay.SetActive(true);
        loreDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText(text);

        s_playerMovement.enabled = false;
        s_looking.enabled = false;
        s_playerShooting.enabled = false;
    }

    public void HideLore()
    {
        loreDisplay.SetActive(false);

        s_playerMovement.enabled = true;
        s_looking.enabled = true;
        s_playerShooting.enabled = true;
    }

    void DevArrows()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            s_playerShooting.ArrowPrefab = DevArrowSelection[0];
        if (Input.GetKeyDown(KeyCode.Alpha1))
            s_playerShooting.ArrowPrefab = DevArrowSelection[1];
        if (Input.GetKeyDown(KeyCode.Alpha2))
            s_playerShooting.ArrowPrefab = DevArrowSelection[2];
        if (Input.GetKeyDown(KeyCode.Alpha3))
            s_playerShooting.ArrowPrefab = DevArrowSelection[3];
        if (Input.GetKeyDown(KeyCode.Alpha4))
            s_playerShooting.ArrowPrefab = DevArrowSelection[4];
        if (Input.GetKeyDown(KeyCode.Alpha5))
            s_playerShooting.ArrowPrefab = DevArrowSelection[5];
        if (Input.GetKeyDown(KeyCode.Alpha6))
            s_playerShooting.ArrowPrefab = DevArrowSelection[6];
        if (Input.GetKeyDown(KeyCode.Alpha7))
            s_playerShooting.ArrowPrefab = DevArrowSelection[7];
        if (Input.GetKeyDown(KeyCode.Alpha8))
            s_playerShooting.ArrowPrefab = DevArrowSelection[8];
        if (Input.GetKeyDown(KeyCode.Alpha9))
            s_playerShooting.ArrowPrefab = DevArrowSelection[9];

    }
}
