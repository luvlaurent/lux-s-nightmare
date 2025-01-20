using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Rigidbody2D))]
    public class AnimationController : KinematicObject
    {
        public float maxSpeed = 7f;
        public float jumpTakeOffSpeed = 7f;
        public float kickBoardMovePower = 15f;

        public Vector2 move;
        private bool jump;
        private bool stopJump;

        private bool controlEnabled = true;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private Rigidbody2D rb;

        private bool isJumping = false;
        private bool alive = true;
        private bool isKickboard = false;

        private int direction = 1; // 1 para direita, -1 para esquerda
        private PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected override void Update()
        {
            if (controlEnabled && alive)
            {
                HandleInputs();
            }
            else
            {
                move = Vector2.zero;
            }

            base.Update();
        }

        private void HandleInputs()
        {
            Run();
            Jump();
            Attack();
            Hurt();
            Die();
            Restart();
            ToggleKickBoard();
        }

        private void Run()
        {
            Vector3 moveVelocity = Vector3.zero;
            animator.SetBool("isRun", false);

            if (!isKickboard)
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    direction = -1;
                    moveVelocity = Vector3.left;

                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);

                    if (!animator.GetBool("isJump"))
                        animator.SetBool("isRun", true);
                }
                else if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    direction = 1;
                    moveVelocity = Vector3.right;

                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);

                    if (!animator.GetBool("isJump"))
                        animator.SetBool("isRun", true);
                }

                transform.position += moveVelocity * maxSpeed * Time.deltaTime;
            }
            else
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    direction = -1;
                    moveVelocity = Vector3.left;

                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
                }
                else if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    direction = 1;
                    moveVelocity = Vector3.right;

                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
                }

                transform.position += moveVelocity * kickBoardMovePower * Time.deltaTime;
            }
        }

        private void Jump()
        {
            // Inicia o salto somente se a tecla foi pressionada e a personagem está no chão
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0) && IsGrounded)
            {
                isJumping = true;
                animator.SetBool("isJump", true);

                rb.velocity = new Vector2(rb.velocity.x, jumpTakeOffSpeed);
            }

            // Verifica se a personagem aterrissou para resetar o estado de salto
            if (IsGrounded && isJumping)
            {
                isJumping = false;
                animator.SetBool("isJump", false);
            }
        }

        private void Attack()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                animator.SetTrigger("attack");
            }
        }

        private void Hurt()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                animator.SetTrigger("hurt");
                Vector2 force = (direction == 1) ? new Vector2(-5f, 1f) : new Vector2(5f, 1f);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        private void Die()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                isKickboard = false;
                animator.SetBool("isKickBoard", false);
                animator.SetTrigger("die");
                alive = false;
            }
        }

        private void Restart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                isKickboard = false;
                animator.SetBool("isKickBoard", false);
                animator.SetTrigger("idle");
                alive = true;
            }
        }

        private void ToggleKickBoard()
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                isKickboard = !isKickboard;
                animator.SetBool("isKickBoard", isKickboard);
            }
        }

        protected override void ComputeVelocity()
        {
            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            targetVelocity = move * maxSpeed;
        }
    }
}
