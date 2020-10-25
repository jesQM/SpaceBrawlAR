using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(Image))]
public class CoronaCircular : MonoBehaviour
{

    private RectTransform rectTransform;
    private CanvasRenderer canvasRenderer;
    private Canvas canvas;
    private Image image;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRenderer = GetComponent<CanvasRenderer>();
        canvas = GetComponent<Canvas>();
        image = GetComponent<Image>();

        rectTransform.localScale = Vector3.one * 0.013f;
        image.sprite = Resources.Load<Sprite>("circle");
        image.type = Image.Type.Filled;
    }

    public void SetSrpite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetFillInPercentage01(float percentage01)
    {
        image.fillAmount = percentage01;
    }

    public void SetColour(Color colour)
    {
        image.color = colour;
    }
}
