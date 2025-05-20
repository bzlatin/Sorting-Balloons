using UnityEngine;

public class OpenLinkedIn : MonoBehaviour
{
    [SerializeField] private string linkedInURL;

    public void OpenURL()
    {
        Application.OpenURL(linkedInURL);
    }
}
