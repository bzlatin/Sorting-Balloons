using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}
