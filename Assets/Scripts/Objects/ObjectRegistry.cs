using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : MonoBehaviour
{
    public static ObjectRegistry Instance { get; private set; }

    private Dictionary<DataCategory, List<ScannableObject>> _objectsByCategory = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    void Initialize()
    {
        // Получаем все ScannableObject на сцене
        var allObjects = FindObjectsByType<ScannableObject>(FindObjectsSortMode.None);

        foreach (var obj in allObjects)
        {
            if (obj == null) continue;

            var category = obj.category;
            if (!_objectsByCategory.ContainsKey(category))
            {
                _objectsByCategory[category] = new List<ScannableObject>();
            }
            _objectsByCategory[category].Add(obj);
        }

        Debug.Log($"[ObjectRegistry] Инициализировано: {allObjects.Length} объектов");
    }

    public void RegisterObject(ScannableObject obj)
    {
        if (obj == null) return;
    
        if (!_objectsByCategory.ContainsKey(obj.category))
        {
            _objectsByCategory[obj.category] = new List<ScannableObject>();
        }
        _objectsByCategory[obj.category].Add(obj);
    }

    public void UnregisterObject(ScannableObject obj)
    {
        if (obj == null) return;
        Debug.Log($"[-] UnregisterObject вызван для: {obj.name}");
        if (Instance == null)
        {
            Debug.LogError("ObjectRegistry.Instance is null!");
            return;
        }
    
        if (_objectsByCategory.ContainsKey(obj.category))
        {
            _objectsByCategory[obj.category].Remove(obj);
        }
    }
    // ObjectRegistry.cs
    public LayerMask GetScannableLayerMask()
    {
        var layers = new HashSet<int>();
        foreach (var list in _objectsByCategory.Values)
        {
            foreach (var obj in list)
            {
                if (obj != null)
                {
                    layers.Add(obj.gameObject.layer);
                }
            }
        }

        LayerMask mask = 0;
        foreach (int layer in layers)
        {
            mask |= (1 << layer);
        }
        return mask;
    }
    public int GetRemainingCount(DataCategory category)
    {
        if (_objectsByCategory.TryGetValue(category, out var list))
        {
            // Считаем только активные и включённые объекты
            int count = 0;
            foreach (var obj in list)
            {
                if (obj != null && obj.gameObject.activeInHierarchy && obj.enabled)
                {
                    count++;
                }
            }
            return count;
        }
        return 0;
    }

    // Опционально: получить все объекты категории
    public List<ScannableObject> GetObjects(DataCategory category)
    {
        return _objectsByCategory.GetValueOrDefault(category, new List<ScannableObject>());
    }
}