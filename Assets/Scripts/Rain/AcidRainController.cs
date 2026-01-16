using Player.Health;
using UnityEngine;

public class AcidRainController : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private AcidRainModel _model;
    private HealthController _playerHealth;

    public void Initialize(AcidRainModel model)
    {
        _model = model;
        _model.OnPlayerHit += DealDamageToPlayer;
    }

    private void DealDamageToPlayer(int damage)
    {
        if (_playerHealth != null)
        {
            Debug.Log("Acid rain damage");
            _playerHealth.takeDamage?.Invoke(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerHealth = other.GetComponent<HealthController>();
            if (_playerHealth == null)
            {
                _playerHealth = other.GetComponentInParent<HealthController>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerHealth = null;
        }
    }

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.OnPlayerHit -= DealDamageToPlayer;
        }
    }
}