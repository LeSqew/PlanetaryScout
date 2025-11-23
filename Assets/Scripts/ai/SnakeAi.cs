using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeAi : MonoBehaviour
{
    [Header("Current state")]
    public state CurrentState;
    public enum state { hidden, attacking, retreating, retreated }

    [Header("Rangeo of player's detection")]
    public float Detection_Range;

    [Header("Range for snake to perform second attack, if player didn't escape it.")]
    public float Wait_For_Second_Bite_Time;

    [Header("Is there light nearby")]
    public bool IsLightThere;

    [Header("Player collider")]
    public Collider PlayerCollider;

    [Header("Snake collider")]
    public Collider SnakeCollider;

    // to be removed when view will be added.
    public Transform SnakeTransform;

    [Header("Health Controller")]
    public HealthController healthController;

    [Header("Health Bar")]
    public HealthBarView healthBarView;

    [Header("Current distance to player")]
    public float CurrentDistanceToPlayer;

    private Vector3 PlayerClosestPoint;
    private Vector3 SnakeClosestPoint;

    private void Attack() 
    {
        CurrentState = state.attacking;
        healthController.Model.TakeDamage(20);
        // something with view here instead this
        SnakeTransform.position = new Vector3(
            SnakeTransform.position.x,
            SnakeTransform.position.y + 0.5f,
            SnakeTransform.position.z
            );
        healthBarView.PrintHp(healthController.Model.currentHealth);

        CurrentState = state.retreating;
        Invoke(nameof(CheckForAnotherAttack), Wait_For_Second_Bite_Time);
    }

    private void CheckForAnotherAttack() 
    {
        // if player still in the attack range with no light, second attack possible in time defined for it
        if (CurrentDistanceToPlayer <= Detection_Range && !IsLightThere)
        {
            CurrentState = state.attacking;
            healthController.Model.TakeDamage(20);
            SnakeTransform.position = new Vector3(
                SnakeTransform.position.x,
                SnakeTransform.position.y + 0.5f,
                SnakeTransform.position.z
            );
        }
        healthBarView.PrintHp(healthController.Model.currentHealth);
        CurrentState = state.retreated;
        SnakeTransform.GetComponent<MeshRenderer>().enabled = false;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Calculating two points of player and snake closest to each other
        PlayerClosestPoint = SnakeCollider.ClosestPoint(PlayerCollider.transform.position);
        SnakeClosestPoint = PlayerCollider.ClosestPoint(SnakeCollider.transform.position);

        // Calculating distance between player and snake
        CurrentDistanceToPlayer = Vector3.Distance(SnakeClosestPoint, PlayerClosestPoint);
        
        // If distance right, there is no light and snake hidden, then it attacks.
        if (CurrentDistanceToPlayer <= Detection_Range && !IsLightThere && CurrentState == state.hidden) 
        {
            Attack();
        }
    }
}
