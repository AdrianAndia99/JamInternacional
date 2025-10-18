using UnityEngine;

/// <summary>
/// Efecto adicional para el rayo láser de Godzilla
/// Agrega distorsión y animación al LineRenderer
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LaserBeamEffect : MonoBehaviour
{
    [Header("Configuración de Ondulación")]
    [Tooltip("Amplitud de la ondulación del rayo")]
    [SerializeField] private float waveAmplitude = 0.1f;
    
    [Tooltip("Frecuencia de la ondulación")]
    [SerializeField] private float waveFrequency = 10f;
    
    [Tooltip("Velocidad de la animación")]
    [SerializeField] private float waveSpeed = 5f;

    [Header("Segmentación del Rayo")]
    [Tooltip("Número de segmentos del rayo (más = suave)")]
    [SerializeField] private int segmentCount = 30;

    [Header("Efectos Visuales")]
    [Tooltip("Textura del rayo (scroll)")]
    [SerializeField] private bool scrollTexture = true;
    
    [Tooltip("Velocidad de scroll de la textura")]
    [SerializeField] private float textureScrollSpeed = 2f;

    [Tooltip("Intensidad del parpadeo")]
    [SerializeField] private float flickerIntensity = 0.1f;

    private LineRenderer lineRenderer;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float timeOffset;
    private Material laserMaterial;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        
        // Crear copia del material para no afectar otros objetos
        if (lineRenderer.material != null)
        {
            laserMaterial = new Material(lineRenderer.material);
            lineRenderer.material = laserMaterial;
        }
    }

    private void OnEnable()
    {
        timeOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (!lineRenderer.enabled) return;

        UpdateLaserWave();
        UpdateLaserEffects();
    }

    /// <summary>
    /// Actualiza la ondulación del rayo
    /// </summary>
    private void UpdateLaserWave()
    {
        if (lineRenderer.positionCount < 2) return;

        // Obtener puntos inicial y final
        startPoint = lineRenderer.GetPosition(0);
        endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        Vector3 direction = (endPoint - startPoint).normalized;
        float distance = Vector3.Distance(startPoint, endPoint);
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        // Si el rayo es vertical, usar otro vector perpendicular
        if (perpendicular.magnitude < 0.1f)
        {
            perpendicular = Vector3.Cross(direction, Vector3.right).normalized;
        }

        // Actualizar cada segmento
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 basePosition = Vector3.Lerp(startPoint, endPoint, t);

            // Calcular ondulación
            float waveOffset = Mathf.Sin((t * waveFrequency) + (Time.time * waveSpeed) + timeOffset) * waveAmplitude;
            
            // Aplicar ondulación perpendicular a la dirección del rayo
            Vector3 wavePosition = basePosition + (perpendicular * waveOffset);

            lineRenderer.SetPosition(i, wavePosition);
        }
    }

    /// <summary>
    /// Actualiza efectos visuales adicionales
    /// </summary>
    private void UpdateLaserEffects()
    {
        // Scroll de textura
        if (scrollTexture && laserMaterial != null)
        {
            float offset = Time.time * textureScrollSpeed;
            laserMaterial.mainTextureOffset = new Vector2(offset, 0);
        }

        // Parpadeo
        if (flickerIntensity > 0)
        {
            float flicker = 1f + (Mathf.PerlinNoise(Time.time * 20f, timeOffset) - 0.5f) * flickerIntensity;
            
            if (laserMaterial != null)
            {
                laserMaterial.SetFloat("_Brightness", flicker);
            }

            // Variar ligeramente el ancho
            float widthMultiplier = 1f + (Mathf.PerlinNoise(Time.time * 15f, timeOffset + 50f) - 0.5f) * (flickerIntensity * 0.5f);
            lineRenderer.widthMultiplier = widthMultiplier;
        }
    }

    /// <summary>
    /// Establece los puntos inicial y final del rayo
    /// </summary>
    public void SetLaserPoints(Vector3 start, Vector3 end)
    {
        startPoint = start;
        endPoint = end;

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, end);
        }
    }

    private void OnDestroy()
    {
        // Limpiar material duplicado
        if (laserMaterial != null)
        {
            Destroy(laserMaterial);
        }
    }
}
