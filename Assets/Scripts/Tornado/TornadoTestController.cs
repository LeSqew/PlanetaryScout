using Tornado;
using UnityEngine;

public class TornadoTestController : MonoBehaviour
{
    private TornadoModel _model;
    private TornadoView _view;

    private void Start()
    {
        _model = new TornadoModel(transform.position, 20f, 5f, 3f);
        _view = GetComponent<TornadoView>();
        _view.Initialize(_model);
        
        // Просто подписываемся напрямую для тестирования
        _model.OnPlayerCaught += (args) => Debug.Log("TEST: Player caught!");
        _model.OnPlayerThrown += (args) => Debug.Log("TEST: Player thrown!");
    }

    private void Update()
    {
        _model.Update(Time.deltaTime);
    }
}
