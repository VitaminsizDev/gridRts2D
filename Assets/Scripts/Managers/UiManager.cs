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
    [SerializeField] private RectTransform unitInfoPanel;
    
    // Tweens
    private Sequence _openProductionMenuSequence;
    private Sequence _closeProductionMenuSequence;
    private Tween _openUnitInfoPanelTween;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        SelectableObject.OnSelectableSelected += UiManager_OnSelected;
    }
    
    private void OnDisable()
    {
        SelectableObject.OnSelectableSelected -= UiManager_OnSelected;
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
        
        // Open unit info panel
        _openUnitInfoPanelTween = unitInfoPanel.DOScaleX(1, globalAnimationTime / 3f).SetEase(Ease.Linear).SetAutoKill(false).Pause();
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
    
    private void UiManager_OnSelected(SelectableObject selectedObject)
    {
        if (selectedObject != null)
        {
            _openUnitInfoPanelTween.PlayForward();
            Debug.Log("Selected: " + selectedObject.PlacedObjectType.objType);
            /*if (selectedObject.PlacedObjectType.objType == PlacedObjectTypeSO.ObjectType.Building)
            {
                unitInfoPanel.Find("ProductionTitle").gameObject.SetActive(true);
                unitInfoPanel.Find("UnitButton").gameObject.SetActive(true);
            }
            else
            {
                unitInfoPanel.Find("ProductionTitle").gameObject.SetActive(false);
                unitInfoPanel.Find("UnitButton").gameObject.SetActive(false);
            }*/
        }
        else _openUnitInfoPanelTween.PlayBackwards();
    }

    public void DeselectUnit()
    {
        SelectableObject.OnSelectableSelected?.Invoke(null);
    }
}
