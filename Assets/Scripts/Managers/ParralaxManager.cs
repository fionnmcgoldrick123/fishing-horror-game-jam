using UnityEngine;

public class ParralaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public string label;
        public Transform layerTransform;

        [Range(0f, 1f)]
        [Tooltip("Per-layer speed. Cubed internally and scaled down, so the full\\n" +
                 "0-1 range is very gentle. 0 = no movement, 1 = subtle drift.")]
        public float parallaxFactor = 0.05f;
    }

    [Header("References")]
    [Tooltip("Leave empty to auto-use Camera.main.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Layers — order does not matter")]
    [SerializeField] private ParallaxLayer[] layers = new ParallaxLayer[]
    {
        new ParallaxLayer { label = "Sky / Stars",        parallaxFactor = 0.02f },
        new ParallaxLayer { label = "Moon",               parallaxFactor = 0.05f },
        new ParallaxLayer { label = "Clouds",             parallaxFactor = 0.08f },
        new ParallaxLayer { label = "Far Background",     parallaxFactor = 0.12f },
        new ParallaxLayer { label = "Mid Background",     parallaxFactor = 0.25f },
        new ParallaxLayer { label = "Near Background",    parallaxFactor = 0.45f },
    };

    private float _lastCameraX;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = UnityEngine.Camera.main.transform;

        _lastCameraX = cameraTransform.position.x;
    }

    private void LateUpdate()
    {
        float deltaX = cameraTransform.position.x - _lastCameraX;

        if (Mathf.Approximately(deltaX, 0f))
        {
            _lastCameraX = cameraTransform.position.x;
            return;
        }

        foreach (ParallaxLayer layer in layers)
        {
            if (layer.layerTransform == null) continue;

            float effective = layer.parallaxFactor * layer.parallaxFactor * layer.parallaxFactor * 0.1f;

            Vector3 pos = layer.layerTransform.position;
            pos.x -= deltaX * effective;
            layer.layerTransform.position = pos;
        }

        _lastCameraX = cameraTransform.position.x;
    }
}
