using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam: MonoBehaviour
{
    [Header("Beam Settings")]
    public float speed = 40f;
    public float damage = 10f;
    public float lifetime = 2f;
    public LayerMask hitLayers;

    [Header("Scaling Over Distance")]
    public bool expandOverDistance = false;
    [Tooltip("Defines which axes (X, Y, Z) the scaling will apply to. Use 1 for 'On' and 0 for 'Off'.")]
    public Vector3 expansionAxes = new Vector3(1, 0, 1); // Default to X and Z (width/height)
    [Tooltip("The maximum multiplier for the scale at the end of the curve.")]
    public float expansionMultiplier = 1.5f;
    [Tooltip("The curve evaluates the scale factor from 0 (start) to 1 (end of lifetime).")]
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1); // Changed default curve

    private float distanceTraveled;
    private Vector3 lastPosition;
    private Vector3 initialScale;

    private void Start()
    {
        lastPosition = transform.position;
        initialScale = transform.localScale;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        float move = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * move);
        distanceTraveled += move;

        if (expandOverDistance)
        {
            float timeProgress = distanceTraveled / (speed * lifetime);

            float curveValue = scaleCurve.Evaluate(timeProgress);

            // The scale factor is the initial scale * (1 + curveValue * (expansionMultiplier - 1))
            // Example: If initialScale is 1, expansionMultiplier is 2, and curveValue is 0.5:
            // newScale = 1 * (1 + 0.5 * (2 - 1)) = 1.5
            // This ensures scaling starts at the initial size (curveValue=0) and grows up to expansionMultiplier * initialScale (curveValue=1).
            float finalScale = initialScale.x * (1f + curveValue * (expansionMultiplier - 1f));

            // Apply scaling only to the specified axes
            //Vector3 newScale = new Vector3(
            //    initialScale.x + (finalScale - initialScale.x) * expansionAxes.x,
            //    initialScale.y + (finalScale - initialScale.y) * expansionAxes.y,
            //    initialScale.z + (finalScale - initialScale.z) * expansionAxes.z
            //);

            // A more readable way to apply:
             Vector3 newScale = initialScale;
             newScale.x = Mathf.Lerp(initialScale.x, finalScale, expansionAxes.x);
             newScale.y = Mathf.Lerp(initialScale.y, finalScale, expansionAxes.y);
             newScale.z = Mathf.Lerp(initialScale.z, finalScale, expansionAxes.z);

            // Simpler implementation assuming initialScale is uniform (like in your original Update)
            // If the beam's initial scale is not uniform, the axis-specific version above is better.
            // Let's use the axis-specific version for robustness.
            transform.localScale = newScale;
        }

    }
}
