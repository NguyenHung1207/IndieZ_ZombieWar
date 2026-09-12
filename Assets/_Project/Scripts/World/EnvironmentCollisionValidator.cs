using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentCollisionValidator
{
    public readonly struct Result
    {
        public readonly int VisualCount;
        public readonly int RealColliderCount;
        public readonly int DisabledColliderCount;
        public readonly string[] MissingNames;
        public readonly string[] PairReports;

        public Result(int visualCount, int realColliderCount, int disabledColliderCount,
            List<string> missingNames, List<string> pairReports)
        {
            VisualCount = visualCount;
            RealColliderCount = realColliderCount;
            DisabledColliderCount = disabledColliderCount;
            MissingNames = missingNames.ToArray();
            PairReports = pairReports.ToArray();
        }
    }

    public static Result ValidateLoadedLevel1()
    {
        List<string> missing = new List<string>();
        List<string> reports = new List<string>();
        int visuals = 0;
        int realColliders = 0;
        int disabledColliders = 0;
        WorldChunk[] chunks = Object.FindObjectsByType<WorldChunk>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
        {
            Transform visualRoot = chunks[chunkIndex].transform.Find("Visuals");
            Transform collisionRoot = chunks[chunkIndex].transform.Find("Collision");
            CountProxyPairs(visualRoot, collisionRoot, chunks[chunkIndex].name, ref visuals,
                ref realColliders, ref disabledColliders, missing, reports);
        }
        return new Result(visuals, realColliders, disabledColliders, missing, reports);
    }

    public static Result ValidateLoadedLevel2()
    {
        List<string> missing = new List<string>();
        List<string> reports = new List<string>();
        int visuals = 0;
        int realColliders = 0;
        int disabledColliders = 0;
        Level2Environment environment = Object.FindFirstObjectByType<Level2Environment>();
        if (environment != null)
        {
            Transform geometryRoot = environment.transform.Find("GameplayGeometry");
            Transform visualRoot = environment.transform.Find("WastelandArt");
            Transform collisionRoot = environment.transform.Find("ExplicitCollisionProxies");
            CountSelfColliders(geometryRoot, environment.name, ref visuals, ref realColliders,
                ref disabledColliders, missing, reports);
            CountProxyPairs(visualRoot, collisionRoot, environment.name, ref visuals,
                ref realColliders, ref disabledColliders, missing, reports);
        }
        return new Result(visuals, realColliders, disabledColliders, missing, reports);
    }

    private static void CountSelfColliders(Transform visualRoot, string ownerName, ref int visualCount,
        ref int realColliderCount, ref int disabledColliderCount, List<string> missing, List<string> reports)
    {
        if (visualRoot == null) return;
        for (int index = 0; index < visualRoot.childCount; index++)
        {
            Transform visual = visualRoot.GetChild(index);
            if (!visual.gameObject.activeInHierarchy) continue;
            visualCount++;
            CountRealColliders(visual, visual, ownerName, ref realColliderCount,
                ref disabledColliderCount, missing, reports);
        }
    }

    private static void CountProxyPairs(Transform visualRoot, Transform collisionRoot, string ownerName,
        ref int visualCount, ref int realColliderCount, ref int disabledColliderCount,
        List<string> missing, List<string> reports)
    {
        if (visualRoot == null) return;
        for (int index = 0; index < visualRoot.childCount; index++)
        {
            Transform visual = visualRoot.GetChild(index);
            if (!visual.gameObject.activeInHierarchy) continue;
            visualCount++;
            string expectedName = visual.name.EndsWith("_Visual")
                ? visual.name.Substring(0, visual.name.Length - "_Visual".Length) + "_Collision"
                : visual.name + "_Collision";
            Transform proxy = collisionRoot != null ? FindChild(collisionRoot, expectedName) : null;
            if (proxy == null)
            {
                missing.Add(ownerName + "/" + visual.name);
                reports.Add(visual.name + " -> " + expectedName + " -> MISSING");
                continue;
            }
            CountRealColliders(visual, proxy, ownerName, ref realColliderCount,
                ref disabledColliderCount, missing, reports);
        }
    }

    private static void CountRealColliders(Transform visual, Transform colliderOwner, string ownerName,
        ref int realColliderCount, ref int disabledColliderCount, List<string> missing, List<string> reports)
    {
        Collider[] colliders = colliderOwner.GetComponentsInChildren<Collider>(true);
        if (colliders.Length == 0)
        {
            missing.Add(ownerName + "/" + visual.name);
            reports.Add(visual.name + " -> " + colliderOwner.name + " -> NO COLLIDER COMPONENT");
            return;
        }

        int enabledCount = 0;
        int invalidCount = 0;
        int inactiveCount = 0;
        string componentName = colliders[0].GetType().Name;
        for (int index = 0; index < colliders.Length; index++)
        {
            Collider collider = colliders[index];
            if (!collider.gameObject.activeInHierarchy)
            {
                inactiveCount++;
            }
            else if (collider.enabled && !collider.isTrigger)
            {
                realColliderCount++;
                enabledCount++;
            }
            else
            {
                disabledColliderCount++;
                invalidCount++;
            }
        }
        if (enabledCount == 0 && invalidCount == 0 && inactiveCount > 0)
        {
            disabledColliderCount++;
            invalidCount = inactiveCount;
        }
        string state = invalidCount == 0 ? "ENABLED" : "INVALID " + invalidCount;
        reports.Add(visual.name + " -> " + colliderOwner.name + " -> " + componentName +
            (colliders.Length > 1 ? " x" + colliders.Length : string.Empty) + " " + state);
    }

    private static Transform FindChild(Transform parent, string name)
    {
        for (int index = 0; index < parent.childCount; index++)
        {
            Transform child = parent.GetChild(index);
            if (child.name == name)
                return child;
        }
        return null;
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Validation/Level 2 Real Environment Colliders")]
    private static void ValidateFromEditor()
    {
        Result level2 = ValidateLoadedLevel2();
        string report = $"LEVEL2_VISUAL_COUNT = {level2.VisualCount}\n" +
            $"LEVEL2_REAL_COLLIDER_COUNT = {level2.RealColliderCount}\n" +
            $"LEVEL2_MISSING_COLLIDER_COUNT = {level2.MissingNames.Length}\n" +
            $"LEVEL2_DISABLED_COLLIDER_COUNT = {level2.DisabledColliderCount}\n" +
            string.Join("\n", level2.PairReports);
        if (level2.MissingNames.Length == 0 && level2.DisabledColliderCount == 0)
            Debug.Log(report);
        else
            Debug.LogError(report);
    }
#endif
}
