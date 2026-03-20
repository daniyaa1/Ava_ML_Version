using UnityEngine;

public class BoneControlTest : MonoBehaviour
{
    private Transform leftUpperArm;
    private Transform leftLowerArm;
    private Transform leftHand;

    void Start()
    {
        // Get Animator component
        Animator animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found!");
            return;
        }

        // Get humanoid bones
        leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);

        Debug.Log("Bones assigned successfully.");
    }

    void Update()
    {
        if (leftUpperArm != null)
        {
            leftUpperArm.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 30f);
        }

        if (leftLowerArm != null)
        {
            leftLowerArm.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 20f);
        }

        if (leftHand != null)
        {
            leftHand.localRotation = Quaternion.Euler(Mathf.Sin(Time.time) * 40f, 0, 0);
        }
    }
}