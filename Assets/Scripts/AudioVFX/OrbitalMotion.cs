using UnityEngine;

/// <summary>
/// Simple orbital motion for wisps around the SpiritCore in OtherDimension.
/// Orbits around parent with a gentle vertical bob.
/// </summary>
public class OrbitalMotion : MonoBehaviour
{
    [SerializeField] private float orbitSpeed = 30f;
    [SerializeField] private float bobAmount = 0.3f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startLocalPos;
    private float timeOffset;

    private void Start()
    {
        startLocalPos = transform.localPosition;
        timeOffset = Random.value * 100f;
    }

    private void Update()
    {
        if (transform.parent == null) return;

        // Orbit around parent
        transform.RotateAround(transform.parent.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // Bob up and down
        float bob = Mathf.Sin((Time.time + timeOffset) * bobSpeed) * bobAmount;
        Vector3 pos = transform.localPosition;
        pos.y = startLocalPos.y + bob;
        transform.localPosition = pos;
    }
}
