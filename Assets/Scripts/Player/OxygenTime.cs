using TMPro;
using UnityEngine;

public class OxygenTime : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public float remainingTime;
    

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0) { 
            remainingTime -= Time.deltaTime;
        }
        else
        {
            remainingTime = 0;
            Debug.Log("Game is over");
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text += string.Format("{00:00}:{1:00}", minutes, seconds);

    }
}
