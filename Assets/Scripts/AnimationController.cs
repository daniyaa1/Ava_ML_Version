using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator component missing from " + gameObject.name);
    }

    public void PlayWave() => animator.SetTrigger("Wave");
    public void PlayClap() => animator.SetTrigger("Clap");
    public void PlayWalk() => animator.SetTrigger("Walk");
    public void PlayTurnLeft() => animator.SetTrigger("TurnLeft");
    public void PlayTurnRight() => animator.SetTrigger("TurnRight");
    public void PlaySit() => animator.SetTrigger("Sit");
    public void PlayStandUp() => animator.SetTrigger("StandUp");
    public void PlayPoint() => animator.SetTrigger("Point");
    public void PlayRaiseHand() => animator.SetTrigger("RaiseHand");

    // FIXED: Added missing Idle and mapping HeadYes to Nod
    public void PlayIdle() => animator.SetTrigger("Idle");
    public void PlayHeadYes() => animator.SetTrigger("HeadYes"); 
    public void PlayHeadNo() => animator.SetTrigger("HeadNo");
}