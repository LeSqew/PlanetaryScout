using System;
using UnityEngine;
using UnityEngine.UI;

public class AnimalScanner : MonoBehaviour, IMinigameController
{
    [Header("Scan Settings")]
    [SerializeField] private float scanTime = 2f;
    [SerializeField] private float maxAngleDegrees = 5f; // угол для "в центре"
    [SerializeField] private Image scanProgressCircle;

    private Camera _mainCamera;
    private ScannableObject _target;
    private float _progress = 0f;
    private bool _isCompleted = false;
    private Action<bool, ScannableObject> _onFinishedCallback;

    public bool RequiresInputBlocking => false;

    void Awake()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Камера не найдена!");
        }
    }

    void Start()
    {
        if (scanProgressCircle != null)
        {
            scanProgressCircle.fillAmount = 0f;
            scanProgressCircle.transform.position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }
    }

    void Update()
    {
        if (_isCompleted || _target == null || _mainCamera == null) return;

        // Проверяем, в центре ли сейчас цель
        bool isInCenter = IsTargetInCenter();

        if (Input.GetMouseButton(0))
        {
            if (isInCenter)
            {
                _progress += Time.deltaTime / scanTime;
                _progress = Mathf.Clamp01(_progress);

                if (scanProgressCircle != null)
                    scanProgressCircle.fillAmount = _progress;

                if (_progress >= 1f)
                {
                    OnSuccess();
                }
            }
            else
            {
                ResetProgress();
            }
        }
        else
        {
            ResetProgress();
        }
    }

    private bool IsTargetInCenter()
    {
        Vector3 directionToTarget = (_target.transform.position - _mainCamera.transform.position).normalized;
        float angle = Vector3.Angle(_mainCamera.transform.forward, directionToTarget);
        return angle <= maxAngleDegrees;
    }

    private void ResetProgress()
    {
        _progress = 0f;
        if (scanProgressCircle != null)
            scanProgressCircle.fillAmount = 0f;
    }

    // === IMinigameController ===
    public void StartAnalysis(ScannableObject target, Action<bool, ScannableObject> onFinishedCallback)
    {
        _target = target;
        _onFinishedCallback = onFinishedCallback;
        _progress = 0f;
        _isCompleted = false;

        if (scanProgressCircle != null)
            scanProgressCircle.fillAmount = 0f;

        gameObject.SetActive(true);
    }

    public void Cleanup()
    {
        _isCompleted = true;
        _target = null;
        _onFinishedCallback = null;
        Destroy(gameObject);
    }

    private void OnSuccess()
    {
        if (_isCompleted) return;
        _isCompleted = true;
        _onFinishedCallback?.Invoke(true, _target);
    }
}