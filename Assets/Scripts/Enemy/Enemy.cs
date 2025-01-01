using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    private StateMachine m_StateMachine;

    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyPursueState PursueState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    [field: SerializeField] public Equipment CurrentEquipment { get; private set; }
    [field: SerializeField] public EnemySettings Settings { get; private set; }
    [field: SerializeField] public EnemyMotor Motor { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public EnemyAnimation Animation { get; private set; }
    [field: SerializeField] public HitBox HitBox { get; private set; }
    [field: SerializeField] public EnemyStats Stats { get; private set; }

   

    private void CreateStates()
    {
        m_StateMachine = new StateMachine();
        IdleState = new(m_StateMachine, this);
        PatrolState = new(m_StateMachine, this);
        PursueState = new(m_StateMachine, this);
        AttackState = new(m_StateMachine, this);

        m_StateMachine.Initialize(IdleState);
        
    }

    public void Initialize(Player player, Slider healthbar, SpawnZone zone)
    {
        Player = player;
        Animation.Initialize(healthbar);
        Motor.Initialize(zone);
        Stats.Died += OnEnemyDied;
        CreateStates();
    }

    private void OnDestroy()
    {
        Stats.Died -= OnEnemyDied;
    }

    private void OnEnemyDied()
    {
        DropItems();
        Destroy(gameObject);
    }
    
    private void DropItems()
    {
        foreach(var item in Settings.GuaranteedDrops)
        {
            var prefab = Instantiate(item.ItemPrefab);
            prefab.transform.position = transform.position;
        }
    }

    private void Update()
    {
        m_StateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        m_StateMachine.CurrentState.FixedUpdate();
    }

    private void LateUpdate()
    {
        m_StateMachine.CurrentState.LateUpdate();
    }
}
