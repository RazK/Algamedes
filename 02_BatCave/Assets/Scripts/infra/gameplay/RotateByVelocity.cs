using UnityEngine;
using Infra.Utils;

namespace Infra.Gameplay {
public class RotateByVelocity : MonoBehaviour {
    // NOTE: We don't require using RotateByVelocity on the same game object as
    // the rigidbodyToTrack, to allow disabling the RotateByVelocity. We can
    // disable the RotateByVelocity's game object and have the rigidbodyToTrack
    // rotate freely.
    [SerializeField] Rigidbody2D rigidbodyToTrack;

    [Range(0,1)]
    [SerializeField] float lerpAmount;
    [SerializeField] float minAngle;
    [SerializeField] float maxAngle;
    [SerializeField] float offsetAngle;
    [Tooltip("The abslute X velocity that below that, only the Y factor will be used")]
    [SerializeField] float minXVelocity;
    [Tooltip("The abslute Y velocity that below that the angle will be set to 0")]
    [SerializeField] float minYVelocityToZeroAngle;

    [Header("Debugging")]
    [SerializeField] float targetAngle;

    protected void FixedUpdate() {
        var velocity = rigidbodyToTrack.velocity;

        if (-minXVelocity < velocity.x && velocity.x < minXVelocity) {
            if (-minYVelocityToZeroAngle < velocity.y && velocity.y < minYVelocityToZeroAngle) {
                targetAngle = 0f;
            } else {
                // Use only the Y factor.
                targetAngle = velocity.y > 0f ? maxAngle : minAngle;
            }
        } else {
            targetAngle = velocity.GetAngle();
        }
        targetAngle += offsetAngle;
        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        Quaternion currentRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle));
        transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, lerpAmount);
    }
}
}
