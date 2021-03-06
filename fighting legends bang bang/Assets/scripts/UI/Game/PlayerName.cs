﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerName : MonoBehaviour
{
    [Tooltip("Pixel offset from the player target")]
    private Vector3 ScreenOffset = new Vector3(0f, -30f, 0f);

    [Tooltip("UI Text to display Player's Name")]
    public TextMeshProUGUI PlayerNameText;

    PlayerBase _target;

    float _characterControllerHeight = 0f;

    Transform _targetTransform;

    Renderer _targetRenderer;

    Vector3 _targetPosition;



    void Awake()
    {

        this.GetComponent<Transform>().SetParent(GameObject.FindObjectOfType<Canvas>().GetComponent<Transform>());
        PlayerNameText = GetComponent<TextMeshProUGUI>();
    }


    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (_target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }


    void LateUpdate()
    {

        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (_targetRenderer != null)
        {
            this.gameObject.SetActive(_targetRenderer.isVisible);
        }

        // #Critical
        // Follow the Target GameObject on screen.
        if (_targetTransform != null)
        {
            _targetPosition = _targetTransform.position;
            _targetPosition.y += (_characterControllerHeight / 2) + 0.4f;

            this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition);
        }
    }

    public void SetTarget(PlayerBase target, Color c)
    {

        // Cache references for efficiency because we are going to reuse them.
        _target = target;
        _targetTransform = _target.GetComponent<Transform>();
        _targetRenderer = _target.GetComponent<Renderer>();

        PlayerNameText.color = c;

        CapsuleCollider _characterController = _target.GetComponent<CapsuleCollider>();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null)
        {
            _characterControllerHeight = _characterController.height;
        }

        if (PlayerNameText != null)
        {
            PlayerNameText.text = _target.photonViewer.owner.NickName.ToUpper();
        }
    }
}