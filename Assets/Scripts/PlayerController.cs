using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private InGameUI inGameUI;
    
    private Rigidbody2D _rb;
    private bool _isGrounded = false;

    public bool IsGrounded
    {
        get => _isGrounded;
        set => _isGrounded = value;
    }
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Green"))
        {
            if (gameManager.isGreen == false)
            {
                inGameUI.ShowGameOver();
                return;
            }
            _rb.velocity = Vector2.zero;
            IsGrounded = true;
            playerSpriteRenderer.sprite = gameManager.greenIdleSprite;
        }
        else if (collision.gameObject.CompareTag("Orange"))
        {
            if (gameManager.isGreen == true)
            {
                inGameUI.ShowGameOver();
                return;
            }
            _rb.velocity = Vector2.zero;
            IsGrounded = true;
            playerSpriteRenderer.sprite = gameManager.orangeIdleSprite;
        }
        else if (collision.gameObject.CompareTag("Carrot"))
        {
            soundManager.PlayEatSound();
            Destroy(collision.gameObject);
            
            if (gameManager.isGreen == true) gameManager.isGreen = false;
            else gameManager.isGreen = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Carrot"))
        {
            gameManager.carrotCnt += 1;
            soundManager.PlayEatSound();
            Destroy(collision.gameObject);

            if (gameManager.isGreen == true)
            {
                gameManager.isGreen = false;
                playerSpriteRenderer.sprite = gameManager.orangeJumpingSprite;
            }
            else
            {
                gameManager.isGreen = true;
                playerSpriteRenderer.sprite = gameManager.greenJumpingSprite;
            }
        }
    }
}
