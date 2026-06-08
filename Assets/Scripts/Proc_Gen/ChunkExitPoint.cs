using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class ChunkExitPoint : MonoBehaviour
{
    [SerializeField] Transform snapPoint;
    [SerializeField] Transform targetPos;
    [Header("Tween Settings")]
    [SerializeField] float snapDuration = 0.2f;   // time to snap to pipe X
    [SerializeField] float pullDuration = 0.8f;   // time to rise to targetPos
    [SerializeField] Ease pullEase = Ease.InCubic; // accelerates into pipe

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
        {
            //lock input
            playerInput.Freeze?.Invoke();
            playerInput.enabled = false;
            var playerAnimation = playerInput.GetComponentInChildren<PlayerAnimation>();
            playerAnimation.ToggleSpriteOrder(-2);
            playerAnimation.ToggleTrailRenderer(false);
            playerInput.GetComponent<CreatureChain>().SpriteSortChain(-2);
            //snap pos - UFO suck effect to exit pipe
            Transform player = playerInput.transform;
            Vector3 snapTarget = new Vector3(snapPoint.position.x,
                                            player.position.y,
                                            player.position.z);

            //rising tween
            player.DOMove(snapTarget, snapDuration)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() =>
                 {
                     // Step 2 — pull up to targetPos (UFO suck effect)
                     player.DOMove(targetPos.position, pullDuration)
                           .SetEase(pullEase)
                           .OnComplete(() =>
                           {
                               DungeonManager.Instance.RemoveSpotlight();

                               //trigger exit transition- in scene transition manager
                               LevelManager.Instance.sceneTransitionManager.ReturnToMainScene();
                               //when main scene loads unfreeze player
                               SoundManager.Instance.PlayDungeonUpTransSFx();

                           });
                 });
        }
    }
}
