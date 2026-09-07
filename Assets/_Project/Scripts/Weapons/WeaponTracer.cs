using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public sealed class WeaponTracer : MonoBehaviour
{
    [SerializeField, Min(0.005f)] private float visibleDuration = 0.045f;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.enabled = false;
    }

    public void Show(Vector3 start, Vector3 end)
    {
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
            line.positionCount = 2;
        }
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.enabled = true;
        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), visibleDuration);
    }

    private void Hide()
    {
        line.enabled = false;
    }
}
