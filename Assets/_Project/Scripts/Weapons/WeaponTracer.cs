using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class WeaponTracer : MonoBehaviour
{
    [SerializeField, Min(0.005f)] private float visibleDuration = 0.045f;
    [SerializeField] private LineRenderer[] pelletLines;

    private LineRenderer line;

    private void Awake()
    {
        if (pelletLines == null || pelletLines.Length == 0)
        {
            line = GetComponent<LineRenderer>();
            pelletLines = new[] { line };
        }
        else
        {
            line = pelletLines[0];
        }

        foreach (LineRenderer pelletLine in pelletLines)
        {
            pelletLine.positionCount = 2;
            pelletLine.enabled = false;
        }
    }

    public void Show(Vector3 start, Vector3 end)
    {
        ShowPellets(start, new[] { end }, 1);
    }

    public void ShowPellets(Vector3 start, Vector3[] endpoints, int count)
    {
        if (pelletLines == null || pelletLines.Length == 0)
        {
            line = GetComponent<LineRenderer>();
            pelletLines = new[] { line };
        }

        int visibleCount = Mathf.Min(Mathf.Min(count, endpoints.Length), pelletLines.Length);
        for (int i = 0; i < pelletLines.Length; i++)
        {
            bool visible = i < visibleCount;
            pelletLines[i].enabled = visible;
            if (visible)
            {
                pelletLines[i].SetPosition(0, start);
                pelletLines[i].SetPosition(1, endpoints[i]);
            }
        }

        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), visibleDuration);
    }

    private void Hide()
    {
        if (pelletLines == null)
            return;
        foreach (LineRenderer pelletLine in pelletLines)
            pelletLine.enabled = false;
    }
}
