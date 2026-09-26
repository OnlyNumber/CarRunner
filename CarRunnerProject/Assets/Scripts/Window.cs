using UnityEngine;
using UnityEngine.UI;

public class Window : MonoBehaviour
{
    public RectTransform window;

    public Button Button;

    public void ShowWindow()
    {
        gameObject.SetActive(true);
    }
    
    public void HideWindow()
    {
        gameObject.SetActive(false);
    }
}
