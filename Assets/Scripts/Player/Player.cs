using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
    private StateMachine m_StateMachine;

    [SerializeField] private InputHandler m_InputHandler;
    [SerializeField] private CrawlerSettings m_CrawlerSettings;       
    [SerializeField] private Rigidbody2D m_RigidBody;
    [SerializeField] private Transform m_RestPosition;
    [SerializeField] private Transform m_Crosshair;
    [SerializeField] private Transform m_PlayerGraphic;
    [SerializeField] private LayerMask m_LegLayerMask;
    [SerializeField] private Transform m_CameraTarget;

    public PlayerIdleState IdleState { get; private set; }
    public PlayerCrawlState CrawlState { get; private set; }
    public PlayerStretchState StretchState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallingState FallingState { get; private set; }
    public PlayerGrappleState GrappleState { get; private set; }

    [field: SerializeField] public HotBar Hotbar { get; private set; }
    [field: SerializeField] public PlayerMotor Motor { get; private set; }
    [field: SerializeField] public PlayerAnimation Animation { get; private set; }
    [field: SerializeField] public HitBox HitBox { get; private set; }
    [field: SerializeField] public PlayerStats Stats { get; private set; }
    public CrawlerSettings CrawlerSettings { get { return m_CrawlerSettings; } }
    public InputHandler PlayerInput { get { return m_InputHandler; } }

    public Action OnUpdate = delegate { };
    public Action OnFixedUpdate = delegate { };
    public Action OnLateUpdate = delegate { };

    public Transform m_SpawnPosition;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Physics2D.queriesStartInColliders = false;
        
        m_StateMachine = new();
        IdleState = new(m_StateMachine, this, m_InputHandler);
        CrawlState = new(m_StateMachine, this, m_InputHandler);
        StretchState = new(m_StateMachine, this, m_InputHandler);
        FallingState = new(m_StateMachine, this, m_InputHandler);
        JumpState = new(m_StateMachine, this, m_InputHandler);
        GrappleState = new(m_StateMachine, this, m_InputHandler);

        m_StateMachine.Initialize(IdleState);

        OnUpdate += On_Update;
        OnFixedUpdate += On_Fixed_Update;
        OnLateUpdate += On_Late_Update;
        Stats.Died += OnPlayerDied;
        m_Crosshair.position = transform.position;
    }

    private void OnDestroy()
    {
        OnUpdate -= On_Update;
        OnFixedUpdate -= On_Fixed_Update;
        OnLateUpdate -= On_Late_Update;
    }

    private void On_Update()
    {
        m_StateMachine.CurrentState.Update();
    }

    private void On_Fixed_Update()
    {
        m_StateMachine.CurrentState.FixedUpdate();
    }

    private void On_Late_Update()
    {
        m_StateMachine.CurrentState.LateUpdate();
    }

    private void OnPlayerDied()
    {
        OnUpdate -= On_Update;
        OnFixedUpdate -= On_Fixed_Update;
        OnLateUpdate -= On_Late_Update;

        Motor.LaunchPlayer();
        StopAllCoroutines();
        StartCoroutine(DeathTimer());
    }

    private IEnumerator DeathTimer()
    {
        float time = Time.time;

        while (Time.time - time < 5f) 
        {
            yield return null;
        }

        ResetPlayer();
    }

    private void ResetPlayer()
    {
        Motor.EndLaunch();
        transform.position = m_SpawnPosition.position;    
        m_StateMachine.ChangeState(IdleState);
        Animation.ResetPlayer();
        Stats.ResetPlayer();
        OnUpdate += On_Update;
        OnFixedUpdate += On_Fixed_Update;
        OnLateUpdate += On_Late_Update;
        m_CameraTarget.position = transform.position;
        m_Crosshair.position = transform.position;
    }

    private void LateUpdate()
    {
        OnLateUpdate.Invoke();

        var cursorPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        var currentCameraPosition = transform.position;
        var direction = (m_Crosshair.position - transform.position).normalized;
        var distance = Vector3.Distance(m_Crosshair.position, transform.position);

        float offset = 1f;

        if(m_StateMachine.CurrentState == StretchState)
        {
            offset = -1f;
        }

        if (distance > m_CrawlerSettings.CrosshairCameraRange.x)
        {
            var range = m_CrawlerSettings.CrosshairCameraRange.y - m_CrawlerSettings.CrosshairCameraRange.x;
            var distanceFromLimit = Mathf.Clamp(distance - m_CrawlerSettings.CrosshairCameraRange.x, 0f, range);
            
            var t = distanceFromLimit / range;
            
            currentCameraPosition += direction * m_CrawlerSettings.MaxCameraDistance * t * offset;         
        }

        m_CameraTarget.position = Vector3.MoveTowards(m_CameraTarget.position, currentCameraPosition, Mathf.Clamp(Time.deltaTime * m_CrawlerSettings.CameraSmoothRate + Vector3.Distance(m_CameraTarget.position, currentCameraPosition), 0f, m_CrawlerSettings.MaxCameraSpeed));
    }

    private void Update()
    {
        OnUpdate.Invoke();
        var cursorPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
        m_Crosshair.position = Camera.main.ScreenToWorldPoint(cursorPosition);
    }

    private void FixedUpdate()
    {
        OnFixedUpdate.Invoke();
    }

    public void LookAtCursor()
    {
        Vector2 lookTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

        if (Vector2.Distance(lookTarget, (Vector2)transform.position) > m_CrawlerSettings.CrosshairLookThreshold)
            m_PlayerGraphic.up = lookTarget - (Vector2)transform.position;
    }

    public void LookAtStretchDirection()
    {
        Vector2 direction = ((Vector2)m_RestPosition.position - (Vector2)transform.position).normalized;
        Vector2 lookTarget2 = direction * 2f;

        m_PlayerGraphic.up = lookTarget2;
    }

    

    public void ItemPickedUp(Item item)
    {
        item.Details.AffectPlayer(this);
    }
}
