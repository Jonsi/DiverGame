using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class PlayerSpine : MonoBehaviour
{
    [Header("Spine")]
    public PlayerBones Bones;
    public SkeletonAnimation Skeleton;
    public AnimationEvent AnimEventHandler;

    [Header("AnimationReferenceAsset")]
    public AnimationReferenceAsset IdleOnBoatAnim;
    public AnimationReferenceAsset JumpFromBoatAnim,
                         IdleInWaterAnim,
                         SwimMeleeAnim,
                         SwimGunAnim,
                         MeleeAttackAnim,
                         FireAnim,
                         AimAnim,
                         HitAnim,
                         DeathAnim;

    private TrackEntry _trackEntry;
    private void OnEnable()
    {
        Skeleton.state.Complete += OnAnimationComplete;
    }

    private void OnDisable()
    {
        Skeleton.state.Complete -= OnAnimationComplete;
    }
    public void SetAnimation(int trackIndex,AnimationReferenceAsset animation, PlayerSkin skin, bool loop , float speed = 1)
    {
        SetSkin(skin);
        _trackEntry = Skeleton.state.SetAnimation(trackIndex, animation.name, loop);
        _trackEntry.TimeScale = speed;
        Skeleton.state.Complete += OnAnimationComplete;
    }
    private void SetSkin(PlayerSkin skin)
    {
        if (Skeleton.initialSkinName != skin.ToString())
        {
            Skeleton.initialSkinName = skin.ToString();
            Skeleton.Initialize(true);
        }
    }
    public void SetSpeed(float speed)
    {
        _trackEntry.TimeScale = speed;
    }
    public void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == JumpFromBoatAnim.Animation.Name)
        {
            AnimEventHandler.OnJumpComplete();
        }

        if (trackEntry.Animation.Name == AimAnim.Animation.Name)
        {
            AnimEventHandler.OnAimComplete();
        }

        if (trackEntry.Animation.Name == FireAnim.Animation.Name)
        {
            Skeleton.state.SetEmptyAnimation(1, 0.2f);
            AnimEventHandler.OnFireComplete();
        }

        if (trackEntry.Animation.Name == HitAnim.Animation.Name)
        {
            AnimEventHandler.OnHitComplete();
        }

        if(trackEntry.Animation.Name == DeathAnim.Animation.Name)
        {
            AnimEventHandler.OnDeathComplete();
        }
    }
}