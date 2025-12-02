using UnityEngine;

[System.Serializable]
public class SnakeAiModel
{
    [Header("Current state")]
    public state CurrentState;
    public enum state { hidden, attacking, retreating}

    [Header("Range of player's detection")]
    public float Detection_Range;

    [Header("Range for snake to perform second attack, if player didn't escape it.")]
    public float WaitInSeconds_BeforeSecondBite;

    [Header("Is there light nearby")]
    public bool IsLightThere;

    [Header("Current distance to player")]
    public float CurrentDistanceToPlayer;

    [Header("Damage of snake attack for player")]
    public int SnakeDamage;
}
