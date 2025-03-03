using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGateUnlock : MonoBehaviour
{
    [SerializeField] Transform gears;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform drawGate;
    [SerializeField] float gateUnlockDuration;
    [SerializeField] float gateXValue;
    
    public void TriggerUnlock()
    {

        //block player input as its a cutscene
        LevelManager.Instance.PlayerInputToggle(false);

        virtualCamera.gameObject.SetActive(true);
        
        DOVirtual.DelayedCall(1f, () =>
        {
            //pan camera to target

            //move draw bridge 
            SoundManager.Instance.PlayGateUnlockSFx(true);
            drawGate.DOLocalMoveX(gateXValue, gateUnlockDuration).OnUpdate(() =>
            {
                //turn gears as door opens
                foreach(Transform child in gears)
                {
                    child.Rotate(new Vector3(0, 0, 50f * Time.deltaTime), Space.Self);
                }

            }).OnComplete(() =>
            {
                // reset cam follow to player
                virtualCamera.gameObject.SetActive(false);

                //re enable input
                LevelManager.Instance.PlayerInputToggle(true);
                SoundManager.Instance.PlayGateUnlockSFx(false);

            });

        });
    }

}
