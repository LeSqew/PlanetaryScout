using UnityEngine;
using System.Collections;

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
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.05f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasFallen) return;
        hasFallen = true;

        // Проигрываем звук
        if (fallSound != null)
            AudioSource.PlayClipAtPoint(fallSound, transform.position);

        // Реакция собак
        Collider[] dogs = Physics.OverlapSphere(transform.position, soundRadius, dogLayer);
        foreach (var dog in dogs)
        {
            ForestDogAiController ai = dog.GetComponent<ForestDogAiController>();
            if (ai != null)
            {
                ai.OnHearingSound(transform.position);
            }
        }

        // Если камень ударился о землю или объект, слегка уменьшить скорость и позволить катиться
        StartCoroutine(SlowDownAndStop());

        // Удаляем через 60 секунд
        Destroy(gameObject, 60f);
    }

    private IEnumerator SlowDownAndStop()
    {
        float duration = 1f; // длительность замедления
        Vector3 initialVelocity = rb.linearVelocity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rb.linearVelocity = Vector3.Lerp(initialVelocity, Vector3.zero, elapsed / duration);
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // оставляем isKinematic = false, чтобы камень больше не зависал
    }
}