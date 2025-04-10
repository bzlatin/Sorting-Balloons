using UnityEngine;

public class InstructionsUI : MonoBehaviour
{
    public GameObject instructionsPanel;

    public void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }
}
