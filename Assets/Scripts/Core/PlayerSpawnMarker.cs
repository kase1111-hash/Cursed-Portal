using UnityEngine;

/// <summary>
/// Marker component placed on a GameObject to designate the player spawn point.
/// Used by FinaleManager to locate spawn position in the OtherDimension scene.
/// </summary>
public class PlayerSpawnMarker : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
#endif
}
