using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CutScene
{
    public class TweenObject : MonoBehaviour
    {
        public float duration;
        public Vector3 endValue;
        public Ease easeType;
        //public AudioClip clip;
        public TweenType tweenType;
        public AudioClip clip;
        [HideInInspector] public Vector3 startScale;
        private void Start()
        {
            //duration = 0.5f;

            //if (endValue == Vector3.zero)
            //    endValue = Vector3.one;
            //set audioclip from soundManager;
            startScale = transform.localScale;
        }
    }
}

