using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

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

    public PlayerIdleState IdleState { get; private set; }
    public PlayerCrawlState CrawlState { get; private set; }
    public PlayerStretchState StretchState { get; private set; }
    public PlayerFallingState FallingState { get; private set; }
    [field: SerializeField] public HotBar Hotbar { get; private set; }
    [field: SerializeField] public PlayerMotor Motor { get; private set; }
    public CrawlerSettings CrawlerSettings { get { return m_CrawlerSettings; } }

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
        m_StateMachine.Initialize(IdleState);
    }

    private void LateUpdate()
    {
        m_StateMachine.CurrentState.LateUpdate();
    }

    private void Update()
    {
        m_StateMachine.CurrentState.Update();
        m_Crosshair.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
    }

    private void FixedUpdate()
    {
        m_StateMachine.CurrentState.FixedUpdate();
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
}
