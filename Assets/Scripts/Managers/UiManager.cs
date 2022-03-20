using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    // Singleton
    public static UiManager Instance { get; private set; }
    
    [Header("UI Settings")]
    public float globalAnimationTime = 0.5f;
    
    [Header("Object References")]
    [SerializeField] private RectTransform productionMenu;
    [SerializeField] private RectTransform openProductionButton;
    
    // Tweens
    private Sequence _openProductionMenuSequence;
    private Sequence _closeProductionMenuSequence;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Setup tweens
        
        // Open production menu
        _openProductionMenuSequence = DOTween.Sequence();
        _openProductionMenuSequence.Append(openProductionButton.DOAnchorPosX(-openProductionButton.rect.width, globalAnimationTime).SetEase(Ease.OutBounce));
        _openProductionMenuSequence.Append(productionMenu.DOAnchorPosX(0, globalAnimationTime).SetEase(Ease.OutBounce));
        _openProductionMenuSequence.SetAutoKill(false).Pause();
        
        // Close production menu
        _closeProductionMenuSequence = DOTween.Sequence();
        _closeProductionMenuSequence.Append(productionMenu.DOAnchorPosX(-productionMenu.rect.width, globalAnimationTime).SetEase(Ease.OutBounce));
        _closeProductionMenuSequence.Append(openProductionButton.DOAnchorPosX(0, globalAnimationTime).SetEase(Ease.OutBounce));
        _closeProductionMenuSequence.SetAutoKill(false).Pause();
    }

    public void ToggleProductionMenu(bool toggle)
    {
        if (toggle)
        {
            _openProductionMenuSequence.Restart();
        }
        else
        {
            _closeProductionMenuSequence.Restart();
        }
    }
}