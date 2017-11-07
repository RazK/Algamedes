using UnityEngine;

namespace Infra.Gameplay {
public class FollowX : MonoBehaviour {
    [SerializeField] Transform target;
    [SerializeField] float offset;
    
    protected void FixedUpdate() {
        var position = transform.position;
        position.x = target.position.x + offset;
        transform.position = position;
    }
}
}
