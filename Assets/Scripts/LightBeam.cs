using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam: MonoBehaviour
{
    public FlashlightStatsBase statsRuntime;
    public enum BeamType {Narrow,Wide}
    public BeamType beamType;

    [Header("Beam Stats")]
    private float _speed;
    private float _damage;
    private float _lifetime;
    private float _cooldown;
    public LayerMask hitLayers;

    [Header("Wide Beam Settings")]
    [Tooltip("Defines which axes (X, Y, Z) the scaling will apply to. Use 1 for 'On' and 0 for 'Off'.")]
    public Vector3 expansionAxes = new Vector3(1, 0, 1); // Default to X and Z (width/height)
    [Tooltip("The maximum multiplier for the scale at the end of the curve.")]
    public float expansionMultiplier = 1.5f;
    [Tooltip("The curve evaluates the scale factor from 0 (start) to 1 (end of lifetime).")]
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1); // Changed default curve

    private Vector3 fireDirection;
    private float distanceTraveled;
    private Vector3 initialScale;

    private float skinOffset = 0.05f; // to avoid clipping

    [SerializeField] private GameObject hitEffectPrefab;


    public void InitializeDirection(Camera cam)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        fireDirection = ray.direction.normalized;
    }
    private void Start()
    {
        initialScale = transform.localScale;

        statsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        if (beamType == BeamType.Narrow)
        {
            _speed = statsRuntime.narrowSpeed;
            _damage = statsRuntime.narrowDamage;
            _lifetime = statsRuntime.narrowLifetime;
            _cooldown = statsRuntime.narrowCooldown;
        }
        else if (beamType==BeamType.Wide)
        {
            _speed= statsRuntime.wideSpeed;
            _damage= statsRuntime.wideDamage;
            _lifetime = statsRuntime.wideLifetime;
            _cooldown = statsRuntime.wideCooldown;
        }

        Destroy(gameObject, _lifetime);

        if (fireDirection == Vector3.zero)
            fireDirection = transform.forward;
    }

    private void Update()
    {
        float moveDistance = _speed * Time.deltaTime;

        // RAYCAST FOR COLLISION
        if (beamType==BeamType.Narrow)  // only destroy on hit if narrow
        {
            if (Physics.Raycast(transform.position, fireDirection, out RaycastHit hit, moveDistance + skinOffset, hitLayers))
            {
                // Düþmana damage verme olayý buraya yazýlacak

                Destroy(gameObject);
                GameObject hitVFX = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

                Destroy(hitVFX, 2f);
                return;
            }
        }

        transform.position += fireDirection * moveDistance;

        // Apply expansion over distance
        distanceTraveled += moveDistance;

        if (beamType==BeamType.Wide)
            ApplyScaleExpansion();

    }

    private void ApplyScaleExpansion()
    {
        float timeProgress = distanceTraveled / (_speed * _lifetime);
        float curveValue = scaleCurve.Evaluate(timeProgress);

        float finalScale = initialScale.x * (1f + curveValue * (expansionMultiplier - 1f));

        Vector3 newScale = initialScale;
        newScale.x = Mathf.Lerp(initialScale.x, finalScale, expansionAxes.x);
        newScale.y = Mathf.Lerp(initialScale.y, finalScale, expansionAxes.y);
        newScale.z = Mathf.Lerp(initialScale.z, finalScale, expansionAxes.z);

        transform.localScale = newScale;
    }
}
