using UnityEngine;
using System.Collections;

public class SnakeAiView : MonoBehaviour
{
    [Header("Health Bar")]
    public HealthBarView healthBarView;

    public Rigidbody SnakeRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void UpdateHP(int hpvalue) 
    {
        healthBarView.PrintHp(hpvalue);
    }

    public void PlayBiteAnimation() 
    {
    }

    public void SnakeRetreat() 
    {
        Vector3 movement;
        if (Random.Range(0, 2) > 0) 
        {
            if (Random.Range(0, 2) > 0)
            {
                movement = new Vector3(4f, 0, 4f);
            }
            else 
            {
                movement = new Vector3(4f, 0, -4f);
            }
        }
        else
        {
            if (Random.Range(0, 2) > 0)
            {
                movement = new Vector3(-4f, 0, 4f);
            }
            else
            {
                movement = new Vector3(-4f, 0, -4f);
            }
        }

        Vector3 NeededPosition = SnakeRigidbody.position + movement;
        StartCoroutine(MoveSmoothly(NeededPosition, 1f));
    }

    private IEnumerator MoveSmoothly(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = SnakeRigidbody.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Вычисляем прогресс от 0 до 1
            float progress = elapsedTime / duration;

            // Плавно интерполируем между начальной и конечной позицией
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            SnakeRigidbody.MovePosition(newPosition);

            // Увеличиваем время и ждем следующий кадр
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Убеждаемся, что достигли точной конечной позиции
        SnakeRigidbody.MovePosition(targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
