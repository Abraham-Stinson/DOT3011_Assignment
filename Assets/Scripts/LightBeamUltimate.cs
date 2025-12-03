using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightBeamUltimate : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float laserRange = 50f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float laserDuration = 3f;

    [Header("Windup Settings")]
    [SerializeField] private float windUpTime = 1.5f;
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
        float t = Mathf.Clamp01(windupTimer / windUpTime);

        float curve = windupCurve.Evaluate(t);

        float distance = curve * laserRange;  // Extend beam from 0 to full length

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

        if (Physics.Raycast(ray, out hit, laserRange))
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, ray.origin + ray.direction * laserRange);
        }
    }

    private IEnumerator StopLaserAfterDelay()
    {
        yield return new WaitForSeconds(laserDuration);

        isLaserActive = false;
        laserLine.enabled = false;

        Destroy(gameObject); // Destroy beam object after effect ends
    }
}
