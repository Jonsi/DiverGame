using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerAnimator : MonoBehaviour
{
    public SkeletonAnimation SkeletonAnimation;
    public AnimationReferenceAsset IdleAnim, MoveAnim,AttackAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAnimation(AnimationReferenceAsset animation, bool loop = true)
    {
        SkeletonAnimation.state.SetAnimation(0, animation, loop);
    }
}
