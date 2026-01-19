using UnityEngine;

public class StoneImpact : MonoBehaviour
{
    [HideInInspector] public AudioClip fallSound;
    [HideInInspector] public float soundRadius = 10f;
    public LayerMask dogLayer;

    private bool hasFallen = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasFallen) return;

        hasFallen = true;

        // ����������� ����
        if (fallSound != null)
            AudioSource.PlayClipAtPoint(fallSound, transform.position);

        // ������� ���� ����� � �������
        Collider[] dogs = Physics.OverlapSphere(transform.position, soundRadius, dogLayer);
        foreach (var dog in dogs)
        {
            ForestDogAiController ai = dog.GetComponent<ForestDogAiController>();
            if (ai != null)
            {
                ai.OnHearingSound(transform.position);
            }
        }

        // �������� ������� ���������� �����
        if (rb != null)
        {
            StartCoroutine(SlowDownAndStop());
        }

        // ����� ������� ������ ����� 10 ������
        Destroy(gameObject, 60f);
    }

    private System.Collections.IEnumerator SlowDownAndStop()
    {
        float duration = 0.5f; // �� ������� ������ �����������
        Vector3 initialVelocity = rb.linearVelocity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rb.linearVelocity = Vector3.Lerp(initialVelocity, Vector3.zero, elapsed / duration);
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true; // ������������ ��������� ������
    }
}