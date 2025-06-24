using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineRespawn : MonoBehaviour
{
    [Header("Original Spline (do not modify this)")]
    public SplineContainer originalSplineContainer;

    [Header("Ghost Travel Settings")]
    public float travelDuration = 2f;
    public float wiggleAmplitude = 0.2f;
    public float wiggleFrequency = 5f;
    public int sampledPoints = 10;

    [Header("Checkpoints")]
    public List<Transform> checkpoints;

    private SplineContainer runtimeSplineContainer;
    private float startT = 0f;
    private float endT = 1f;
    private float startTime;
    private bool isReturning = false;

    public void StartReturn(Vector3 deathPosition, Transform checkpoint)
    {
        if (originalSplineContainer == null)
        {
            Debug.LogError("Original spline container is null!");
            return;
        }

        // Destroy previous runtime spline if needed
        if (runtimeSplineContainer != null)
        {
            Destroy(runtimeSplineContainer.gameObject);
            runtimeSplineContainer = null;
        }

        // Clone container
        runtimeSplineContainer = CreateRuntimeSplineContainer();

        // Rebuild runtime spline from death to checkpoint
        RebuildSplineFromDeathToCheckpoint(runtimeSplineContainer, deathPosition, checkpoint.position);

        // Reset timing
        startT = 0f;
        endT = 1f;
        startTime = Time.time;
        isReturning = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartReturn(transform.position, checkpoints[0]);
        }

        if (!isReturning || runtimeSplineContainer == null)
            return;

        float elapsed = Time.time - startTime;
        float tNorm = Mathf.Clamp01(elapsed / travelDuration);
        float t = Mathf.Lerp(startT, endT, tNorm);
        t = Mathf.Clamp01(t);

        Vector3 splinePos = runtimeSplineContainer.EvaluatePosition(t);
        Vector3 tangent = runtimeSplineContainer.EvaluateTangent(t);
        if (tangent.sqrMagnitude < 0.0001f) tangent = Vector3.right;
        tangent.Normalize();

        Vector3 perp = new Vector3(-tangent.y, tangent.x, 0f);
        float wiggle = Mathf.Sin(Time.time * wiggleFrequency) * wiggleAmplitude;
        Vector3 offset = perp * wiggle;

        Vector3 newPos = splinePos + offset;
        if (float.IsNaN(newPos.x) || float.IsNaN(newPos.y)) return;

        Vector3 moveDir = newPos - transform.position;
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.position = newPos;

        if (tNorm >= 1f)
        {
            FinishReturn();
        }
    }

    private void FinishReturn()
    {
        isReturning = false;

        if (runtimeSplineContainer != null)
        {
            transform.position = runtimeSplineContainer.EvaluatePosition(1f);
            Destroy(runtimeSplineContainer.gameObject); // cleanup
        }

        Debug.Log("Ghost return complete.");
        // TODO: re-enable input or resume gameplay here
    }

    private SplineContainer CreateRuntimeSplineContainer()
    {
        GameObject go = new GameObject("RuntimeSplineContainer");
        var container = go.AddComponent<SplineContainer>();

        container.transform.position = originalSplineContainer.transform.position;
        container.transform.rotation = originalSplineContainer.transform.rotation;
        container.transform.localScale = originalSplineContainer.transform.localScale;

        return container;
    }

    private void RebuildSplineFromDeathToCheckpoint(SplineContainer container, Vector3 deathWorldPos, Vector3 checkpointWorldPos)
    {
        var sourceSpline = originalSplineContainer.Spline;
        var runtimeSpline = container.Spline;

        float3 localDeath = container.transform.InverseTransformPoint(deathWorldPos);
        float3 localCheckpoint = container.transform.InverseTransformPoint(checkpointWorldPos);

        // Get t values on original spline
        SplineUtility.GetNearestPoint(sourceSpline, localDeath, out _, out float deathT);
        SplineUtility.GetNearestPoint(sourceSpline, localCheckpoint, out _, out float checkpointT);

        float tStart = Mathf.Min(deathT, checkpointT);
        float tEnd = Mathf.Max(deathT, checkpointT);
        bool reverse = deathT > checkpointT;

        List<BezierKnot> pathKnots = new List<BezierKnot>();

        // Sample between tStart and tEnd
        for (int i = 0; i <= sampledPoints; i++)
        {
            float t = Mathf.Lerp(tStart, tEnd, i / (float)sampledPoints);
            float3 pos = sourceSpline.EvaluatePosition(t);
            pathKnots.Add(new BezierKnot(pos, float3.zero, float3.zero));
        }

        // Force first and last knots to be exact death and checkpoint
        if (!reverse)
        {
            pathKnots[0] = new BezierKnot(localDeath, float3.zero, float3.zero);
            pathKnots[pathKnots.Count - 1] = new BezierKnot(localCheckpoint, float3.zero, float3.zero);
        }
        else
        {
            pathKnots[0] = new BezierKnot(localCheckpoint, float3.zero, float3.zero);
            pathKnots[pathKnots.Count - 1] = new BezierKnot(localDeath, float3.zero, float3.zero);
            pathKnots.Reverse();
        }

        for (int i = 0; i < pathKnots.Count; i++)
        {
            float3 prev = (i == 0) ? pathKnots[i].Position : pathKnots[i - 1].Position;
            float3 next = (i == pathKnots.Count - 1) ? pathKnots[i].Position : pathKnots[i + 1].Position;

            float3 tangent = (next - prev) * 0.5f;
            pathKnots[i] = new BezierKnot(
                pathKnots[i].Position,
                -tangent * 0.3f,
                tangent * 0.3f,
                quaternion.identity
            );
        }

        // Clear and rebuild runtime spline
        runtimeSpline.Clear();
        foreach (var knot in pathKnots)
            runtimeSpline.Add(knot);

        container.Spline = runtimeSpline;

        
    }
}


