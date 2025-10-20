using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layerTransform;
        [Range(0f, 5f)] public float parallaxSpeed = 1f;
        public int sortingOrder = -1;
        public bool autoCalculateWidth = true;
        public float spriteWidth = 20f;
        [HideInInspector] public Transform layerClone;
        [HideInInspector] public Vector3 startPosition;
        [HideInInspector] public Vector3 cloneStartPosition;
    }

    [SerializeField] private ParallaxLayer[] layers;
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private bool autoMove = true;
    [SerializeField] private bool autoConfigureSortingOrder = true;
    [SerializeField] private int baseSortingOrder = -10;
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private DinoRunner dinoRunner;

    private void Start()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            if (layer.layerTransform == null) continue;

            if (autoConfigureSortingOrder)
            {
                int targetOrder = layer.sortingOrder == -1 ? baseSortingOrder + i : layer.sortingOrder;
                SpriteRenderer sr = layer.layerTransform.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    if (!string.IsNullOrEmpty(sortingLayerName)) sr.sortingLayerName = sortingLayerName;
                    sr.sortingOrder = targetOrder;
                }
            }

            if (layer.autoCalculateWidth)
            {
                SpriteRenderer sr = layer.layerTransform.GetComponent<SpriteRenderer>();
                if (sr != null) layer.spriteWidth = sr.bounds.size.x;
            }

            layer.startPosition = layer.layerTransform.position;
            CreateLayerClone(layer);
        }
    }

    private void CreateLayerClone(ParallaxLayer layer)
    {
        if (layer.layerTransform == null) return;
        GameObject clone = new GameObject($"{layer.layerTransform.name}_Clone");
        layer.layerClone = clone.transform;
        SpriteRenderer origSR = layer.layerTransform.GetComponent<SpriteRenderer>();
        if (origSR != null)
        {
            SpriteRenderer cloneSR = clone.AddComponent<SpriteRenderer>();
            cloneSR.sprite = origSR.sprite;
            cloneSR.color = origSR.color;
            cloneSR.sortingLayerName = origSR.sortingLayerName;
            cloneSR.sortingOrder = origSR.sortingOrder;
            cloneSR.flipX = origSR.flipX;
            cloneSR.flipY = origSR.flipY;
        }
        Vector3 pos = layer.layerTransform.position;
        pos.x += layer.spriteWidth;
        layer.layerClone.position = pos;
        layer.layerClone.localScale = layer.layerTransform.localScale;
        layer.layerClone.rotation = layer.layerTransform.rotation;
        layer.cloneStartPosition = layer.layerClone.position;
        layer.layerClone.SetParent(transform);
    }

    private void Update()
    {
        if (!autoMove) return;
        foreach (var layer in layers)
        {
            if (layer.layerTransform == null) continue;
            float move = baseSpeed * layer.parallaxSpeed * Time.deltaTime;
            layer.layerTransform.position += Vector3.left * move;
            if (layer.layerClone != null) layer.layerClone.position += Vector3.left * move;
            if (layer.layerTransform.position.x <= layer.startPosition.x - layer.spriteWidth)
            {
                layer.layerTransform.position = new Vector3(
                    layer.layerTransform.position.x + (layer.spriteWidth * 2),
                    layer.layerTransform.position.y,
                    layer.layerTransform.position.z);
            }
            if (layer.layerClone != null && layer.layerClone.position.x <= layer.cloneStartPosition.x - layer.spriteWidth)
            {
                layer.layerClone.position = new Vector3(
                    layer.layerClone.position.x + (layer.spriteWidth * 2),
                    layer.layerClone.position.y,
                    layer.layerClone.position.z);
            }
        }
    }

    public void SetAutoMove(bool enabled) { autoMove = enabled; }
    public void SetBaseSpeed(float speed) { baseSpeed = speed; }

    private void OnDestroy()
    {
        foreach (var layer in layers)
            if (layer.layerClone != null) Destroy(layer.layerClone.gameObject);
    }
}
