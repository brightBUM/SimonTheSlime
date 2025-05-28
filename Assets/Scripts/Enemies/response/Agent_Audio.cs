using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Magar
{
    public class Agent_Audio : OnHit
    {

        public bool isPitchVaried = true;
        [ShowIf(nameof(isPitchVaried), true)]
        public Vector2 pitchRange=new Vector2(.9f,1.25f);
        public List<AudioClip> DamageSound = new List<AudioClip>();
        public List<AudioClip> DeathSound = new List<AudioClip>();
        private AudioSource audioSource;
       // private AudioManager AudioManager;
        protected override void Start()
        {
            base.Start();
            audioSource = GetComponent<AudioSource>();
        }

        protected override void OnDamage(DamageInfo info)
        {
            if (!CanResponse(info))
                return;

            base.OnDamage(info);    
            if (DamageSound.Count <= 0) return;
            int index = Random.Range(0, DamageSound.Count);
            PlaySound(DamageSound[index]);
        }

        public void PlaySound(AudioClip c)
        {
            audioSource.clip = c;
            if (isPitchVaried)
            {
                audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            }
            audioSource.Play();
        }
        protected override void OnDeath(DamageInfo info)
        {
            base.OnDeath(info);
            if (DeathSound.Count <=0) return;
            //PlaySoundInstance(DeathSound);
            //AudioManager.PlayRandomSoundEffect(DeathSound.ToArray(),transform);
        }

    }
}
