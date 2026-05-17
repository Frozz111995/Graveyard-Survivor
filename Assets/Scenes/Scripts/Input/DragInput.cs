// DragInput.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] float deadZone = 10f;

    Vector2 _startPos;
    Vector2 _delta;
    bool _pressing;

    public static DragInput Create()
    {
        // EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // Canvas
        var canvasGo = new GameObject("DragCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = -1; // под UI игры
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        // Panel
        var panelGo = new GameObject("DragPanel");
        panelGo.transform.SetParent(canvasGo.transform, false);

        var rect = panelGo.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        var image = panelGo.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0); // прозрачный

        return panelGo.AddComponent<DragInput>();
    }

    public Vector2 GetMove()
    {
        if (!_pressing) return Vector2.zero;
        if (_delta.magnitude < deadZone) return Vector2.zero;
        return _delta.normalized;
    }

    public void OnPointerDown(PointerEventData e)
    {
        _startPos = e.position;
        _pressing = true;
    }

    public void OnDrag(PointerEventData e)
    {
        _delta = e.position - _startPos;
    }

    public void OnPointerUp(PointerEventData e)
    {
        _pressing = false;
        _delta = Vector2.zero;
    }
}