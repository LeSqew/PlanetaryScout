using UnityEngine;

public class ForestDogAiModel
{
    public enum state 
    { 
        patrolling, 
        checking_sound, 
        chasing_player, 
        waiting_after_bite, 
        attacking, 
        waiting_after_playing_seen,
        preparing_lunge,
        lunging
    }
}
