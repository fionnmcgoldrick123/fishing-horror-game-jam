using UnityEngine;

public class ParralaxManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public string label;
        public Transform layerTransform;

        [Range(0f, 1f)]
        [Tooltip("How much this layer moves relative to the camera.\n" +
                 "0 = fixed in world (no movement).\n" +
                 "1 = moves exactly with camera (glued to screen).\n" +
                 "Distant layers (sky, moon) should be very low (0.02–0.1).\n" +
                 "Closer layers should be higher (0.2–0.5).")]
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

    // LateUpdate ensures the camera has already moved this frame before we respond to it.
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

            Vector3 pos = layer.layerTransform.position;
            pos.x -= deltaX * layer.parallaxFactor;
            layer.layerTransform.position = pos;
        }

        _lastCameraX = cameraTransform.position.x;
    }
}
