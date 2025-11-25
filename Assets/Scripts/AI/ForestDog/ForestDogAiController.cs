using Player.Health;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ForestDogAiController : MonoBehaviour
{
    private int currentWaypointIndex = 0;

    public Collider player;

    public Collider dog;

    [Header("Waypoints of patrolling")]
    public Transform[] patrol_waypoints;

    [Header("Patrolling agent (dog)")]
    public NavMeshAgent agent;

    public PlayerStealthController stealthController;

    public float MaxHearingDistance;

    public ForestDogAiModel.state state;

    public HealthController healthController;

    private float attackCooldownTimer = 0f;

    public float attackCooldown = 6f;

    [SerializeField]
    private float DogAttackDistance = 1f;

    [SerializeField]
    private int DogAttackDamage = 20;

    [SerializeField]
    private float dogAlarmSpeed = 8f;

    [SerializeField]
    private float dogCalmSpeed = 2f;

    public ForestDogAiView view;

    Vector3 PlayerPoint;
    Vector3 DogPoint;

    public void CheckForPlayer()
    {
        if (!stealthController.isCrouching)
        {
            if (Vector3.Distance(PlayerPoint, DogPoint) <= MaxHearingDistance)
            {
                state = ForestDogAiModel.state.chasing_player;
                agent.speed = dogAlarmSpeed;
                agent.SetDestination(player.transform.position);
                view.PlayDogRunAnimation();
            }
            else
            {
                agent.speed = dogCalmSpeed;
            }
        }
        else 
        {
            if (Vector3.Distance(PlayerPoint, DogPoint) <= DogAttackDistance) 
            {
                Attack();
            }
        }
    }

    public void OnHearingSound(Vector3 soundPosition)
    {
        // ���������� ������� �������������� � ��� � �����
        state = ForestDogAiModel.state.checking_sound;
        agent.SetDestination(soundPosition);
        agent.speed = dogAlarmSpeed;
        view.PlayDogRunAnimation();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent.SetDestination(patrol_waypoints[currentWaypointIndex].position);
    }

    void Attack() 
    {
        state = ForestDogAiModel.state.waiting_after_bite;

        healthController.takeDamage?.Invoke(DogAttackDamage);

        view.PlayDogAttackAnimation();

        agent.isStopped = true;

        attackCooldownTimer = attackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPoint = dog.ClosestPoint(player.transform.position);
        DogPoint = player.ClosestPoint(dog.transform.position);



        if (state != ForestDogAiModel.state.waiting_after_bite) { CheckForPlayer(); }

        if (state == ForestDogAiModel.state.waiting_after_bite) 
        {
            attackCooldownTimer -= Time.deltaTime;

            if (attackCooldownTimer <= 0f)
            {
                agent.isStopped = false;

                state = ForestDogAiModel.state.patrolling;
                CheckForPlayer();
                if (state != ForestDogAiModel.state.chasing_player) 
                { 
                    state = ForestDogAiModel.state.patrolling; 
                    agent.SetDestination(patrol_waypoints[currentWaypointIndex].position); 
                }
            }
        }

        if (state == ForestDogAiModel.state.chasing_player) {
            Vector3 PlayerPointNearest = dog.ClosestPoint(player.transform.position);
            Vector3 DogPointNearest = player.ClosestPoint(dog.transform.position);

            if (Vector3.Distance(PlayerPointNearest, DogPointNearest) <= DogAttackDistance) 
            {
                Attack();
            }
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.speed = dogCalmSpeed;
            state = ForestDogAiModel.state.patrolling;
            agent.SetDestination(patrol_waypoints[currentWaypointIndex].position);
        }

        if (state == ForestDogAiModel.state.patrolling && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) 
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrol_waypoints.Length;
            agent.SetDestination(patrol_waypoints[currentWaypointIndex].position);
        }
    }
}
