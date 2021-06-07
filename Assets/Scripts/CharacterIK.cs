using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    private Animator anim;
    public CharacterController cont;
    public CharacterMovement move;
    public CharacterShooting shoot;
    public LayerMask groundLayers;
    [Range(0, 1)]
    public float iKWeight;
    public float groundCheckLineDist = 100f;
    public float groundOffset = .15f;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (cont.isGrounded && !move.layingDown && move.VerticalAxis == 0)
        {
            Vector3 leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            Vector3 rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot).position;

            RaycastHit leftFootHit = GetHitPoint(leftFoot + Vector3.up, leftFoot - (Vector3.up * groundCheckLineDist));
            if (leftFootHit.collider)
            {
                leftFoot = leftFootHit.point + (Vector3.up * groundOffset);
                Quaternion alignLeftFoot = Quaternion.FromToRotation(Vector3.up, leftFootHit.normal) * anim.GetBoneTransform(HumanBodyBones.LeftFoot).rotation;

                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, iKWeight);
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, alignLeftFoot);
            }

            RaycastHit rightFootHit = GetHitPoint(rightFoot + Vector3.up, rightFoot - (Vector3.up * groundCheckLineDist));
            if (rightFootHit.collider)
            {
                rightFoot = rightFootHit.point + (Vector3.up * groundOffset);
                Quaternion alignRightFoot = Quaternion.FromToRotation(Vector3.up, rightFootHit.normal) * anim.GetBoneTransform(HumanBodyBones.RightFoot).rotation;

                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, iKWeight);
                anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot);
                anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                anim.SetIKRotation(AvatarIKGoal.RightFoot, alignRightFoot);
            }
        }

        if(shoot.equippedGun != null && shoot.aiming)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, iKWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, shoot.equippedGun.handRestPoint.position);
        }
    }

    private RaycastHit GetHitPoint(Vector3 start, Vector3 end)
    {
        RaycastHit hit;
        Physics.Linecast(start, end, out hit, groundLayers);

        return hit;
    }
}
