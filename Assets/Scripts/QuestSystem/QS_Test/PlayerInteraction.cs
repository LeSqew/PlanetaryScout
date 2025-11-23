using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    public SpectrometerController spectrometer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
            {
                var scannable = hit.collider.GetComponent<ScannableObject>();
                if (scannable != null)
                {
                    Debug.Log("Найден объект: " + scannable.name);
                    if (scannable.category == DataCategory.Flora)
                    {
                        spectrometer.StartAnalysis(scannable);
                    }
                }
                else
                {
                    Debug.Log("Объект не имеет ScannableObject");
                }
            }
            else
            {
                Debug.Log("Raycast не попал в объект");
            }
        }
    }
}