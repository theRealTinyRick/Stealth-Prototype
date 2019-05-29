using UnityEngine;

public class PlayerJumpAnimatorHook : MonoBehaviour
{
    public const string JumpForwardAnim = "JumpForward";
    public const string JumpStandAnim = "JumpStand";

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void JumpStand()
    {
        animator.Play(JumpStandAnim);
    }

    public void JumpForward()
    {
        animator.Play(JumpForwardAnim);
    }
}
