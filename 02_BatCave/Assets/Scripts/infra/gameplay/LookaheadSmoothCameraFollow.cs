using UnityEngine;
using System;

namespace Infra.Gameplay {
/// <summary>
/// Place this on a camera to follow an object moving horizontally while
/// "enticipating" its movements up and down.
/// The lookahead target should be a child of the target and placed at an offset.
/// As the target turns, its lookahead child moves up or down. The camera follows
/// the target's X position and the lookahead's Y position.
/// </summary>
[RequireComponent(typeof(Camera))]
public class LookaheadSmoothCameraFollow : MonoBehaviour {
    [Serializable]
    public class ClampRange {
        public float min;
        public bool clampMin;
        public float max;
        public bool clampMax;
        public float clampSoftness = 50f;

        /// <summary>
        /// Soft clamp the value.
        /// </summary>
        public float Clamp(float value) {
            if (clampMin && value < min) {
                var diff = value - min;
                return min + diff / (clampSoftness - diff);
            } else if (clampMax && value > max) {
                var diff = value - max;
                return max + diff / (clampSoftness + diff);
            }
            return value;
        }
    }

    [Tooltip("The target that is moving horizontally.")]
    [SerializeField] Transform target;
    [Tooltip("The lookahead target that is a child of the target object.")]
    [SerializeField] Transform targetLookahead;
    [Tooltip("How soft does the camera react to the changing position.\n" +
        "0 means the camera will always be at the target's X and the lookahead's " +
        "Y (not a very comfortable experience).")]
    [SerializeField] float snapLooseness = 0.15f;
    [Tooltip("The offset of the camera from the target (at X) and the lookahead (at Y).\n" +
        "Z will be automatically set to be the initial distance of the target " +
        "from the camera.")]
    [SerializeField] Vector3 offset;
    [Tooltip("How to clamp the camera's position on the X axis.")]
    [SerializeField] ClampRange clampX;
    [Tooltip("How to clamp the camera's position on the Y axis.")]
    [SerializeField] ClampRange clampY;

    // Only used to pass to SmoothDamp.
    private Vector3 velocity = Vector3.zero;

    protected void Awake() {
        var cameraComponent = GetComponent<Camera>();
        offset.z = cameraComponent.transform.position.z - target.position.z;
    }

    protected void FixedUpdate() {
        if (target && targetLookahead && target.gameObject.activeInHierarchy) {
            // Calculate position.
            var targetPosition = new Vector3(target.position.x, targetLookahead.position.y, 0.0f) + offset;

            // Soft clamp the position.
            targetPosition.x = clampX.Clamp(targetPosition.x);
            targetPosition.y = clampY.Clamp(targetPosition.y);

            // Damp and set position.
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                snapLooseness,
                Mathf.Infinity,
                Time.fixedDeltaTime);
        }
    }
}
}
