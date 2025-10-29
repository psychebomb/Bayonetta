using UnityEditor; // Required for AnimationUtility
using UnityEngine;

public class OverwriteAnimationCurves : MonoBehaviour
{
    public AnimationClip sourceClip;
    public AnimationClip targetClip;
    public string curvePath; // e.g., "GameObject/ChildObject"
    public string propertyName; // e.g., "localPosition.x"

    [ContextMenu("Overwrite Curve")]
    void OverwriteCurve()
    {
        if (sourceClip == null || targetClip == null)
        {
            Debug.LogError("Source or Target clip is not assigned.");
            return;
        }

        // Get the curve from the source clip
        AnimationCurve sourceCurve = AnimationUtility.GetEditorCurve(sourceClip, EditorCurveBinding.FloatCurve(curvePath, typeof(Transform), propertyName));

        if (sourceCurve == null)
        {
            Debug.LogError($"Curve for '{propertyName}' on path '{curvePath}' not found in source clip.");
            return;
        }

        // Set the curve on the target clip
        AnimationUtility.SetEditorCurve(targetClip, EditorCurveBinding.FloatCurve(curvePath, typeof(Transform), propertyName), sourceCurve);

        Debug.Log($"Curve '{propertyName}' from '{sourceClip.name}' overwritten to '{targetClip.name}'.");
    }
}