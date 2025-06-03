using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Magar
{

    public class Agent_Fx : OnHit
    {
        public ParticleSystem SpawnFx;
        public bool DirectionalFx;
        public List<ParticleSystem> DamageFx= new List<ParticleSystem>();
        public ParticleSystem DeathFx;

        protected override void Start()
        {
            base.Start();
            if (SpawnFx != null)
            {
                SpawnFx.Play();
            }
        }

        protected override void OnDamage(DamageInfo info)
        {
            base.OnDamage(info);
            if (DamageFx.Count <= 0) return;
            int index = Random.Range(0,DamageFx.Count);
            if(DirectionalFx)
            {
                DamageFx[index].transform.forward = info.dir.normalized;
            }
            DamageFx[index].Play();
        }

        public void PlayFx(ParticleSystem p, DamageInfo info, bool isDirectional = true)
        {
            if (isDirectional)
            {
                p.transform.forward = info.dir.normalized;
            }
            p.Play();
        }
        
        public void PlayFxInstance(ParticleSystem p, DamageInfo info, bool isDirectional = true,bool isInstance=false)
        {
            //Vector3 dir = Vector3.ProjectOnPlane(info.dir, onHitResponseTransform.up).normalized;

            if (isDirectional)
            {
                p.transform.forward = info.dir.normalized;
            }
            if (!isInstance)
            {
            p.transform.parent = null;
            p.gameObject.SetActive(true);
            p.Play();

            }
            else
            {
                ParticleSystem spawnedFx = Instantiate(p, p.transform.position, p.transform.rotation);
                spawnedFx.gameObject.SetActive(true);
                spawnedFx.Play();
            }
        }
        public void PlayFxInstance(ParticleSystem p)
        {
            p.transform.parent = null;
            p.gameObject.SetActive(true);
            p.Play();

        }
        protected override void OnDeath(DamageInfo info)
        {
            base.OnDeath(info);
            if (DeathFx == null) return;
            DeathFx.transform.parent = null;
            DeathFx.gameObject.SetActive(true);
            DeathFx.Play();
        }

        protected override void OnTeleport()
        {
            base.OnTeleport();
            SpawnFx?.Play();
        }


    }
}
