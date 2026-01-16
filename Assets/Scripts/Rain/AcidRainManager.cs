using UnityEngine;

public class AcidRainManager : MonoBehaviour
{
    [Header("Rain Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float damageInterval = 0.1f;
    [SerializeField] private float lifetime = 10f; // Сколько живёт дождь

    private AcidRainModel _model;
    private AcidRainView _view;
    private AcidRainController _controller;
    private float _lifeTimer = 0f;

    // Метод для внешней инициализации пути
    public void SetPath(Vector3 start, Vector3 end)
    {
        _model = new AcidRainModel(start, end, speed, damagePerSecond, damageInterval, 0f);
        _model.Activate();

        _view = GetComponent<AcidRainView>();
        if (_view != null) _view.Initialize(_model);

        _controller = GetComponent<AcidRainController>();
        if (_controller != null) _controller.Initialize(_model);
    }

    private void Update()
    {
        if (_model != null)
        {
            _model.Update(Time.deltaTime);

            // Уничтожаем после завершения пути или по истечении времени
            _lifeTimer += Time.deltaTime;
            if (!_model.IsActive || _lifeTimer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}