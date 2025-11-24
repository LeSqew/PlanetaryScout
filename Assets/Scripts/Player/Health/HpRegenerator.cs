using System.Collections;
using UnityEngine;

namespace Player.Health
{
    public class HpRegenerator : MonoBehaviour
    {
        [SerializeField] private int healAmount = 5;
        [SerializeField] private float timeOut = 5f;
        [SerializeField] private float interval = 2f;
        private HealthController  _hpController;
        private Coroutine _regenRoutine;
        private Coroutine _timeOutRoutine;

        private void Awake()
        {
            _hpController = GetComponent<HealthController>();
        }
    
        public void Timeout(int damage)
        {
            StopRegen();

            // сбрасываем предыдущий таймаут если опять получили урон
            if (_timeOutRoutine != null)
                StopCoroutine(_timeOutRoutine);

            // запускаем новый таймаут
            _timeOutRoutine = StartCoroutine(RegenTimeout());
        }

        private IEnumerator RegenTimeout()
        {
            yield return new WaitForSeconds(timeOut);
            StartRegen();
        }

        private void StartRegen()
        {
            if (_regenRoutine != null)
                return;

            _regenRoutine = StartCoroutine(RegenerationLoop());
        }

        public void StopRegen()
        {
            if (_regenRoutine == null) return;
            StopCoroutine(_regenRoutine);
            _regenRoutine = null;
        }

        private IEnumerator RegenerationLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);

                // вызываем лечение через контроллер
                _hpController.heal?.Invoke(healAmount);
            }
        }
    }
}
