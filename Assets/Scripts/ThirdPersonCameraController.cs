using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target;
    public CharacterMovement move;
    public CharacterShooting shoot;

    public float behindOffset;
    public float heightOffset;
    public float targetHeightOffset;

    public Transform player;
    public Transform aimTarget;

    public float followDelay;
    public float targetSwapSmoothing;
    private Vector3 velocity;

    private float xzOffset;
    private float yOffset;
    private float yTargetOffset;

    private float camFollowDelay;
    private Vector3 transPos;

    private Vector3 targetPos;

    void Update()
    {
        if(target != null)
        {
            camFollowDelay = followDelay;

            if (shoot.aiming)
            {
                target = aimTarget;
                camFollowDelay = .05f;

                if (move.HorizontalAxis != 0)
                {
                    targetSwapSmoothing = 1f;
                }
                else
                {
                    targetSwapSmoothing = .05f;
                }

                xzOffset = Mathf.Lerp(xzOffset, -behindOffset - .5f , .2f);
                yOffset = Mathf.Lerp(yOffset, -(aimTarget.localPosition.y - 1.5f) - 2, .25f);
                yTargetOffset = Mathf.Lerp(yTargetOffset, 0, .1f);
            }
            else if(move.layingDown)
            {
                xzOffset = Mathf.Lerp(xzOffset, -behindOffset / 1.5f, .3f);
                yOffset = Mathf.Lerp(yOffset, heightOffset / 5f, .3f);
                yTargetOffset = Mathf.Lerp(yTargetOffset, .5f, .1f);
            }
            else
            {
                target = player;

                xzOffset = Mathf.Lerp(xzOffset, -behindOffset, .1f);
                yOffset = Mathf.Lerp(yOffset, heightOffset, .1f);
                yTargetOffset = Mathf.Lerp(yTargetOffset, targetHeightOffset, .05f);
            }

            targetPos = Vector3.Lerp(targetPos, (target.forward * xzOffset) + new Vector3(target.position.x, target.position.y + yOffset, target.position.z), targetSwapSmoothing);

            transPos = Vector3.Lerp(transPos, target.position, targetSwapSmoothing);
            transform.LookAt(transPos + (Vector3.up * yTargetOffset));

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, camFollowDelay);
        }
    }
}
