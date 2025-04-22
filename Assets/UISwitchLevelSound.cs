using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UISwitchLevelSound : MonoBehaviour
{
    void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                if (UIAudioManager.Instance != null)
                    UIAudioManager.Instance.SwitchLevelButtonClick();
            });
        }
    }
}
