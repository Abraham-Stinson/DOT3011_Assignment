using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightBeamUltimate : MonoBehaviour
{
    public FlashlightStatsBase statsRuntime;

    private float _laserRange;
    private float _laserCooldown;
    private float _laserDuration;
    private float _laserDamage;

    [Header("Windup Settings")]
    private float _windUpTime;
    [SerializeField] private AnimationCurve windupCurve;  

    [Header("Line Renderer")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private AnimationCurve widthCurve;

    [Header("Material Flash Settings")]
    [SerializeField] private Material beamMaterial;       // optional assign in inspector
    [SerializeField] private Color baseColor=Color.red;
    [SerializeField] private float baseIntensity = 1f;    // the steady/base emission intensity
    [SerializeField] private float emissionPulse = 1f;    // pulse amplitude (how much it varies)
    [SerializeField] private float pulseSpeed = 4f;       // pulse speed
    private Material materialInstance;
    private static readonly int emissionID = Shader.PropertyToID("_EmissionColor");

    [Header("Camera")]
    [SerializeField] private Camera playerCam;

    [SerializeField] private GameObject hitEffectPrefab;
    private GameObject hitVFX;

    // Internal
    private Transform firePoint;
    private bool isLaserActive = false;
    private bool isWindingUp = false;
    private float windupTimer = 0f;

    private void Awake()
    {
        if (playerCam == null)
            playerCam = Camera.main;

        if (laserLine == null)
            laserLine = GetComponent<LineRenderer>();

        if (widthCurve != null)
            laserLine.widthCurve = widthCurve;

        if (beamMaterial == null)
        {
            // try to take the material from LineRenderer (this creates an instance)
            if (laserLine != null)
            {
                materialInstance = laserLine.material; // this returns instance - safe
                baseColor=materialInstance.color;
            }
            else
            {
                Debug.LogWarning("No beamMaterial or laserLine found.");
            }
        }
        else
        {
            // instantiate to avoid changing the original asset
            materialInstance = Instantiate(beamMaterial);
            if (laserLine != null)
                laserLine.material = materialInstance;
        }

        if (materialInstance != null)
        {
            materialInstance.EnableKeyword("_EMISSION");
            // initialize emission to baseColor * baseIntensity
            materialInstance.SetColor(emissionID, baseColor * baseIntensity);
        }
    }

    private void Start()
    {
        statsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;

        _laserDuration = statsRuntime.ultimateDuration;
        _laserRange = statsRuntime.ultimateRange;
        _laserDamage = statsRuntime.ultimateDamage;
        _laserCooldown = statsRuntime.ultimateCooldown;
    }

    void Update()
    {
        if (isWindingUp) WindupLaser();
        else if (isLaserActive) UpdateLaser();
    }

    private void LateUpdate()
    {
        if (materialInstance == null || laserLine == null || !laserLine.enabled) return;

        // produce a sine from -1..1, then scale by pulse amplitude
        float sine = Mathf.Sin(Time.time * pulseSpeed); // -1 .. 1
        float intensity = baseIntensity + sine * emissionPulse; // base ± pulse

        // prevent negative intensity
        intensity = Mathf.Max(0f, intensity);

        // set HDR emission color = baseColor * intensity
        Color emission = baseColor * intensity;
        materialInstance.SetColor(emissionID, emission);
    }

    public void InitiateLaser(Transform firePoint)
    {
        this.firePoint = firePoint;

        // Begin windup
        isWindingUp = true;
        windupTimer = 0f;

        // Prepare laser line
        laserLine.enabled = true;
        laserLine.positionCount = 2;

    }

    void WindupLaser()
    {
        windupTimer += Time.deltaTime;
        float t = Mathf.Clamp01(windupTimer / _windUpTime);

        float curve = windupCurve.Evaluate(t);

        float distance = curve * _laserRange;  // Extend beam from 0 to full length

        Vector3 start = firePoint.position;
        Vector3 end = start + playerCam.transform.forward * distance;

        laserLine.SetPosition(0, start);
        laserLine.SetPosition(1, end);

        if (t >= 1f)
        {
            isWindingUp = false;
            isLaserActive = true;
            StartCoroutine(StopLaserAfterDelay());
        }
    }

    void UpdateLaser()
    {
        laserLine.SetPosition(0, firePoint.position);

        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (hitVFX != null)
            hitVFX.transform.position = laserLine.GetPosition(1);

        if (Physics.Raycast(ray, out hit, _laserRange))
        {
            laserLine.SetPosition(1, hit.point);

            //spawning hit VFX
            
            if (hitVFX == null)
                hitVFX = Instantiate(hitEffectPrefab, laserLine.GetPosition(1),Quaternion.identity);
        }
        else
        {
            laserLine.SetPosition(1, ray.origin + ray.direction * _laserRange);

            if(hitVFX!=null)
                Destroy(hitVFX);
        }
    }

    private IEnumerator StopLaserAfterDelay()
    {
        yield return new WaitForSeconds(_laserDuration);

        isLaserActive = false;
        laserLine.enabled = false;

        Destroy(gameObject); // Destroy beam object after effect ends

        if (hitVFX != null)
            Destroy(hitVFX);
    }
}
