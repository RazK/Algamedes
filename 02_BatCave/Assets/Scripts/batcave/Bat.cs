using UnityEngine;
using UnityEngine.EventSystems;
using Infra.Gameplay.UI;

namespace BatCave
{
    /// <summary>
    /// The Bat controller. Responsible for playing bat animations, handling collision
    /// with the cave walls and responding to player input.
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    public class Bat : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] float flyYSpeed;
        [SerializeField] float xSpeed;

        [Header("Animation")] [SerializeField] string flyUpBoolAnimParamName;
        [SerializeField] string isAliveBoolAnimParamName;

        [Header("State")] [SerializeField] bool isAlive;

        [Header("Testing")] [SerializeField] bool isInvulnerable;

        private int flyUpBoolAnimParamId;
        private int isAliveBoolAnimParamId;

        private bool flyUp;
        private Animator animator;
        private Rigidbody2D body;

        protected void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            flyUpBoolAnimParamId = Animator.StringToHash(flyUpBoolAnimParamName);
            isAliveBoolAnimParamId = Animator.StringToHash(isAliveBoolAnimParamName);

            GameInputCapture.OnTouchDown += OnTouchDown;
            GameInputCapture.OnTouchUp += OnTouchUp;

            isAlive = true;
        }

        protected void OnDestroy()
        {
            GameInputCapture.OnTouchDown -= OnTouchDown;
            GameInputCapture.OnTouchUp -= OnTouchUp;
        }

        protected void Update()
        {
            if (!isAlive) return;

            // Handle keyboard input.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                flyUp = true;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                flyUp = false;
            }
            animator.SetBool(flyUpBoolAnimParamId, flyUp);
            animator.SetBool(isAliveBoolAnimParamId, isAlive);
        }

        protected void FixedUpdate()
        {
            if (!isAlive)
            {
                return;
            }
            if (flyUp)
            {
                body.velocity = new Vector2(xSpeed, flyYSpeed);
            }
            animator.SetBool(flyUpBoolAnimParamId, flyUp);
            // EXERCISE: Set the bat's body.velocity so it will always move at
            //           xSpeed to the right and at flyYSpeed if it should fly up.
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (isInvulnerable) return;
            isAlive = false;
        }

        private void OnTouchDown(PointerEventData e)
        {
            if (!isAlive) return;
            flyUp = true;
        }

        private void OnTouchUp(PointerEventData e)
        {
            flyUp = false;
        }
    }
}