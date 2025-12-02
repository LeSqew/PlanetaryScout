using Player.Health;
using UnityEngine;

public class SnakeAiController : MonoBehaviour
{
    public SnakeAiModel model;

    private Vector3 PlayerClosestPoint;
    private Vector3 SnakeClosestPoint;

    [Header("Player collider")]
    public Collider PlayerCollider;

    [Header("Snake collider")]
    public Collider SnakeCollider;

    [Header("Health Controller")]
    public HealthController healthController;

    [Header("Snake ai view")]
    public SnakeAiView snakeAiView;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void StunPlayer() 
    {
    }

    private void Attack()
    {
        model.CurrentState = SnakeAiModel.state.attacking;
        healthController.takeDamage?.Invoke(model.SnakeDamage);
        // something with view here instead this
        snakeAiView.PlayBiteAnimation();
        StunPlayer();
        Invoke(nameof(CheckForAnotherAttack), model.WaitInSeconds_BeforeSecondBite);
    }

    private void CheckForAnotherAttack()
    {
        // if player still in the attack range with no light, second attack possible in time defined for it
        if (model.CurrentDistanceToPlayer <= model.Detection_Range && !model.IsLightThere)
        {
            model.CurrentState = SnakeAiModel.state.attacking;
            healthController.takeDamage?.Invoke(model.SnakeDamage);
            snakeAiView.PlayBiteAnimation();
            StunPlayer();
        }
        model.CurrentState = SnakeAiModel.state.retreating;
        snakeAiView.SnakeRetreat();
        Invoke(nameof(ReturnSnakeToHidden), 5f);
    }

    void ReturnSnakeToHidden() 
    {
        model.CurrentState = SnakeAiModel.state.hidden;
    }

    // Update is called once per frame
    void Update()
    {
        if (false)
        {
            model.IsLightThere = true;
        }
        else 
        {
            model.IsLightThere = false;
        }

        PlayerClosestPoint = SnakeCollider.ClosestPoint(PlayerCollider.transform.position);
        SnakeClosestPoint = PlayerCollider.ClosestPoint(SnakeCollider.transform.position);

        // Calculating distance between player and snake
        model.CurrentDistanceToPlayer = Vector3.Distance(SnakeClosestPoint, PlayerClosestPoint);

        // If distance right, there is no light and snake hidden, then it attacks.
        if (model.CurrentDistanceToPlayer <= model.Detection_Range && !model.IsLightThere && model.CurrentState == SnakeAiModel.state.hidden)
        {
            Attack();
        }
    }
}
