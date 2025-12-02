using UnityEngine;
using System.Collections;

public class SnakeAiView : MonoBehaviour
{
    public Rigidbody SnakeRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            // ��������� �������� �� 0 �� 1
            float progress = elapsedTime / duration;

            // ������ ������������� ����� ��������� � �������� ��������
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            SnakeRigidbody.MovePosition(newPosition);

            // ����������� ����� � ���� ��������� ����
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ����������, ��� �������� ������ �������� �������
        SnakeRigidbody.MovePosition(targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
