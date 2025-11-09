using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpController : MonoBehaviour
{
    [SerializeField] private bool isInputEnabled = true;
    [SerializeField] private float jumpForceMin = 5f;
    [SerializeField] private float jumpForceMax = 10f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float horizontalJumpSpeed = 5f; // 오른쪽으로 이동하는 수평 속도
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SoundManager soundManager;

    private Rigidbody2D _rb;
    private PlayerController _playerController;
    private float _jumpChargeTime;
    private float _touchDelay = 0.1f;
    private float _lastTouchTime = 0f;
    public bool IsInputEnabled
    {
        get => isInputEnabled;
        set => isInputEnabled = value;
    }
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody2D>();
        _jumpChargeTime = 0f;
    }
    void Update()
    {
        if (!IsInputEnabled) { return; }
        if (_playerController.IsGrounded && Input.touchCount > 0) // TODO: 공중에서 이단 점프 추가 예정
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) { return; }
            if (Time.time - _lastTouchTime < _touchDelay) return;
            
            if (touch.phase == TouchPhase.Began)// 점프 충전 시작
            {
                _jumpChargeTime = 0f;
                
                if(gameManager.isGreen == true) playerSpriteRenderer.sprite = gameManager.greenJumpPreparationSprite;
                else playerSpriteRenderer.sprite = gameManager.orangeJumpPreparationSprite;
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) // 점프 충전 - 최대 충전 시간은 못넘음
            {
                
                _jumpChargeTime = Mathf.Min(_jumpChargeTime + Time.deltaTime, maxChargeTime);
            }
            else if (touch.phase == TouchPhase.Ended) // 점프 시작
            {
                
                float jumpForce = Mathf.Lerp(jumpForceMin, jumpForceMax, _jumpChargeTime / maxChargeTime);
                Vector2 jumpVector = new Vector2(horizontalJumpSpeed, jumpForce);
                _rb.AddForce(jumpVector, ForceMode2D.Impulse);
                
                soundManager.PlayJumpSound();
                _playerController.IsGrounded = false;
                _lastTouchTime = Time.time;
                
                if(gameManager.isGreen == true) playerSpriteRenderer.sprite = gameManager.greenJumpingSprite;
                else playerSpriteRenderer.sprite = gameManager.orangeJumpingSprite;
            }
        }
    }

    public void EnableInput(bool enable)
    {
        IsInputEnabled = enable;
    }
}
