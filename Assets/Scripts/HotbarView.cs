using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class HotbarView : MonoBehaviour
{
    [SerializeField] private List<HotbarViewSlot> m_Slots;
    [SerializeField] private EmptyEventChannel m_HotbarSelectionEventChannel;
    [SerializeField] private HotBar m_Hotbar;
    [SerializeField] private RectTransform m_SelectionIndicator;

    private void OnEnable()
    {
        m_HotbarSelectionEventChannel.EventRaised += OnHotBarSelectionChanged;
    }

    private void OnDisable()
    {
        m_HotbarSelectionEventChannel.EventRaised -= OnHotBarSelectionChanged;
    }

    private void OnHotBarSelectionChanged(Empty empty)
    {
        Debug.Log("Hotbar view selection changed");
        m_SelectionIndicator.SetParent(m_Slots[m_Hotbar.CurrentHotbarIndex].transform, false);
    }
}

