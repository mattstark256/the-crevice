using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField]
    private TextLine textPrefab;
    [SerializeField]
    private Transform textParent;
    [SerializeField]
    private float lineSpacing = 50;
    [SerializeField]
    private float scrollSpeed = 500;
    [SerializeField]
    private float minHeight = -300;
    [SerializeField]
    private float maxHeight = 300;

    Vector3 currentSpawnPoint = Vector3.zero;
    Vector3 initialTextParentPosition;
    Vector3 textParentTargetPosition;
    float textMoveSpeed = 7;
    bool hasScrolled = false;

    private void Awake()
    {
        initialTextParentPosition = textParent.transform.localPosition;
        textParentTargetPosition = initialTextParentPosition;
    }

    void Update()
    {
        // Move text up
        if (!hasScrolled)
        {
            textParent.transform.localPosition = Vector3.Lerp(textParent.transform.localPosition, textParentTargetPosition, Time.deltaTime * textMoveSpeed);
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            hasScrolled = true;
            textParent.transform.localPosition += Vector3.down * Input.GetAxisRaw("Mouse ScrollWheel") * scrollSpeed;
            if (textParent.transform.localPosition.y < minHeight)
            {
                textParent.transform.localPosition = Vector3.up * minHeight;
            }
            if (textParent.transform.localPosition.y > textParentTargetPosition.y + maxHeight)
            {
                textParent.transform.localPosition = textParentTargetPosition + Vector3.up * maxHeight;
            }
        }
    }

    public TextLine CreateText(string message)
    {
        TextLine textLine = Instantiate(textPrefab, textParent);
        textLine.SetString(message);
        textLine.transform.localPosition = currentSpawnPoint;
        currentSpawnPoint += Vector3.down * (lineSpacing + textLine.GetTextHeight());
        textParentTargetPosition = initialTextParentPosition - currentSpawnPoint;
        hasScrolled = false;
        return textLine;
    }

    public void SkipToTarget()
    {
        textParent.transform.localPosition = textParentTargetPosition;
    }
}
