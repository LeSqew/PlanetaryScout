using UnityEngine;

public class AcidRainView : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainParticles;
    [SerializeField] private BoxCollider rainCollider;

    private AcidRainModel _model;

    public void Initialize(AcidRainModel model)
    {
        _model = model;
        _model.OnPositionChanged += UpdatePosition;
        _model.OnRainActiveChanged += ToggleRainVisuals;
    }

    private void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void ToggleRainVisuals(bool isActive)
    {
        if (rainCollider != null) rainCollider.enabled = isActive;
        if (rainParticles != null)
        {
            if (isActive) rainParticles.Play();
            else rainParticles.Stop();
        }
    }

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.OnPositionChanged -= UpdatePosition;
            _model.OnRainActiveChanged -= ToggleRainVisuals;
        }
    }
}