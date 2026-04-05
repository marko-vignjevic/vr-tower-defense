using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bonus Option #2: Fast Travel
/// Implements brief constant-velocity motion (under 100ms) to hotspots.
/// No easing - just pure constant velocity movement.
/// Reuses existing hotspot infrastructure from Part 3.
/// </summary>
public class FastTravel : MonoBehaviour
{
    [SerializeField] private Transform OVRCameraRig;     // OVRCameraRig transform (root)
    [SerializeField] private Transform centerEyeAnchor;  // TrackingSpace/CenterEyeAnchor

    [Header("Hotspots (TargetPose transforms)")]
    [SerializeField] private List<Transform> hotspots = new List<Transform>();

    [Header("Input")]
    [SerializeField] private OVRInput.Button fastTravelButton = OVRInput.Button.Two; // B button on right controller
    [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.RTouch;

    [Header("Fast Travel Settings")]
    [SerializeField] private float travelDuration = 0.08f; // 80ms - under 100ms as required
    [SerializeField] private bool allowInput = true; // Can disable during travel

    [Header("Debounce")]
    [SerializeField] private float cooldownSeconds = 0.25f;

    private int _index = -1; // hotspot list index
    private float _nextAllowedTime = 0f;
    private bool _isTraveling = false;

    private void Reset()
    {
        // Auto-find common references
        if (OVRCameraRig == null)
        {
            var rig = FindObjectOfType<OVRCameraRig>();
            if (rig) OVRCameraRig = rig.transform;
        }
        if (centerEyeAnchor == null && OVRCameraRig != null)
        {
            var t = OVRCameraRig.Find("TrackingSpace/CenterEyeAnchor");
            if (t) centerEyeAnchor = t;
        }
    }

    private void Update()
    {
        // Don't accept input during travel or cooldown
        if (_isTraveling || Time.time < _nextAllowedTime) return;

        if (OVRInput.GetDown(fastTravelButton, controller))
        {
            _nextAllowedTime = Time.time + cooldownSeconds;
            StartFastTravel();
        }
    }

    public void StartFastTravel()
    {
        if (OVRCameraRig == null || centerEyeAnchor == null || hotspots == null || hotspots.Count == 0)
        {
            Debug.LogWarning("FastTravel: Missing rig/head/hotspots reference.");
            return;
        }

        if (_isTraveling)
        {
            Debug.LogWarning("FastTravel: Already traveling!");
            return;
        }

        // Update index to next hotspot
        _index = (_index + 1) % hotspots.Count;
        Transform targetHotspot = hotspots[_index];

        // Start the fast travel coroutine
        StartCoroutine(FastTravelToHotspot(targetHotspot));
    }

    private IEnumerator FastTravelToHotspot(Transform target)
    {
        _isTraveling = true;

        // Calculate start and end positions/rotations
        Vector3 startRigPos = OVRCameraRig.position;
        Quaternion startRigRot = OVRCameraRig.rotation;

        // Compute where we want to end up (same logic as HotspotNext)
        Vector3 headPos = centerEyeAnchor.position;
        Vector3 headToRigOffset = startRigPos - headPos;
        
        // Desired head position at hotspot XZ, keeping current Y
        Vector3 desiredHeadPosition = new Vector3(target.position.x, target.position.y, target.position.z);
        Vector3 endRigPos = desiredHeadPosition + headToRigOffset;
        
        // Target rotation from hotspot
        float targetYRotation = target.eulerAngles.y;
        Quaternion endRigRot = Quaternion.Euler(0f, targetYRotation, 0f);

        // Calculate constant velocity (distance / time)
        float distance = Vector3.Distance(startRigPos, endRigPos);
        float velocity = distance / travelDuration;

        float elapsed = 0f;

        // Move with constant velocity - NO easing
        while (elapsed < travelDuration)
        {
            elapsed += Time.deltaTime;
            
            // Linear interpolation factor (0 to 1)
            float t = elapsed / travelDuration;
            
            // Clamp to prevent overshooting
            t = Mathf.Clamp01(t);

            // Constant-velocity position interpolation (linear)
            OVRCameraRig.position = Vector3.Lerp(startRigPos, endRigPos, t);
            
            // Constant-velocity rotation interpolation (linear)
            OVRCameraRig.rotation = Quaternion.Slerp(startRigRot, endRigRot, t);

            yield return null;
        }

        // Ensure we end exactly at target
        OVRCameraRig.position = endRigPos;
        OVRCameraRig.rotation = endRigRot;

        _isTraveling = false;
    }

    /// <summary>
    /// Public method to trigger fast travel to a specific hotspot index
    /// </summary>
    public void FastTravelToIndex(int index)
    {
        if (hotspots == null || index < 0 || index >= hotspots.Count)
        {
            Debug.LogWarning($"FastTravel: Invalid hotspot index {index}");
            return;
        }

        _index = index - 1; // Set to one before target, so StartFastTravel increments to correct index
        StartFastTravel();
    }

    /// <summary>
    /// Get current hotspot index
    /// </summary>
    public int GetCurrentHotspotIndex()
    {
        return _index;
    }

    /// <summary>
    /// Check if currently traveling
    /// </summary>
    public bool IsTraveling()
    {
        return _isTraveling;
    }
}
