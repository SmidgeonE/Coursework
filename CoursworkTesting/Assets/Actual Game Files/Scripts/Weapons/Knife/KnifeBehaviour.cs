﻿using System.Collections;
using System.Collections.Generic;
using Actual_Game_Files.Scripts;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

public class KnifeBehaviour : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float knifeDistance;
    [SerializeField] private float knifeDamage;
    [SerializeField] private float knifeAttackSpeed;
    [Space]
    
    [Header("Game Objects")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource thisAudioSource;

    private static readonly int IsStabbing = Animator.StringToHash("IsStabbing");
    private bool _canStab = true;
    
    private void Update()
    {
        CheckAttackButtonPress();
    }

    private void CheckAttackButtonPress()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0) || !_canStab) return;
        animator.SetTrigger(IsStabbing);
        StartCoroutine(Wait(knifeAttackSpeed));
    }

    private void Stab()
    {
        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out var hit, knifeDistance))
        {
            if (hit.transform.CompareTag("Enemies"))
            {
                hit.transform.GetComponent<EnemyScript>().TakeDamage(knifeDamage);
                audioManager.Play("KnifeStab", thisAudioSource);
            }
            else if (hit.transform.CompareTag("Environment"))
                audioManager.Play("KnifeWallHit", thisAudioSource);
        }
        else
            audioManager.Play("KnifeSwoosh", thisAudioSource);
    }

    private void DisableKnife()
    {
        gameObject.SetActive(false);
    }

    private void PlayPullOutSound()
    {
        audioManager.Play("KnifeUnsheathing", thisAudioSource);
    }
    
    private IEnumerator Wait(float time)
    {
        _canStab = false;
        yield return new WaitForSeconds(time);
        _canStab = true;
    }
}
