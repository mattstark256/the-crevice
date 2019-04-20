using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLine : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private ContentSizeFitter contentSizeFitter;

    public void SetString(string s)
    {
        text.text = s;
    }

    public float GetTextHeight()
    {
        // preferredHeight doesn't change immediately so ForceUpdateCanvases is necessary
        Canvas.ForceUpdateCanvases();
        return text.preferredHeight;
    }
}
