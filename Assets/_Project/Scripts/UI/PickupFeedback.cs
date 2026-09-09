using UnityEngine;
using UnityEngine.UI;

public sealed class PickupFeedback : MonoBehaviour
{
    private const float Lifetime = 0.85f;
    private Text label;
    private float age;

    public static void Show(Vector3 worldPosition, string message, Color color)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        Camera camera = Camera.main;
        if (canvas == null || camera == null) return;

        GameObject instance = new GameObject("PickupFeedback", typeof(RectTransform), typeof(CanvasGroup), typeof(Text), typeof(PickupFeedback));
        instance.transform.SetParent(canvas.transform, false);
        RectTransform rect = (RectTransform)instance.transform;
        rect.anchorMin = rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220f, 56f);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, camera.WorldToScreenPoint(worldPosition + Vector3.up), canvas.worldCamera, out Vector2 localPoint);
        rect.anchoredPosition = localPoint;

        Text text = instance.GetComponent<Text>();
        text.text = message;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 26;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.raycastTarget = false;
    }

    private void Awake() => label = GetComponent<Text>();

    private void Update()
    {
        age += Time.deltaTime;
        transform.localPosition += Vector3.up * (48f * Time.deltaTime);
        if (label != null)
        {
            Color color = label.color;
            color.a = 1f - Mathf.Clamp01(age / Lifetime);
            label.color = color;
        }
        if (age >= Lifetime) Destroy(gameObject);
    }
}
