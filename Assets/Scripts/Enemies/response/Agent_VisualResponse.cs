using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magar
{
    public class Agent_VisualResponse : OnHit
    {
        [Header("Visual Response Settings")]
        [SerializeField] public Renderer[] _renderers;
        [SerializeField] private Material _hitMaterial;
        [SerializeField] private float _hitFlashDuration = 0.1f;

        private Dictionary<Renderer, Material[]> _originalMaterials = new Dictionary<Renderer, Material[]>();
        private Coroutine _flashRoutine;
        private bool _isFlashing = false;

        protected override void Start()
        {
            base.Start();
            InitializeRenderers();

        }
        private void InitializeRenderers()
        {
            foreach (var renderer in _renderers)
            {
                _originalMaterials[renderer] = renderer.materials;
            }
        }

        protected override void OnDamage(DamageInfo info)
        {
            if (!CanResponse(info))
                return;
            base.OnDamage(info);

            if (_hitMaterial == null)
            {
                Debug.LogWarning("Hit material not assigned in Agent_VisualResponse", this);
                return;
            }

            if (_isFlashing)
            {
                if (_flashRoutine != null)
                {
                    StopCoroutine(_flashRoutine);
                }
                ResetMaterials();
            }

            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _isFlashing = true;
            ApplyHitMaterial();

            yield return new WaitForSeconds(_hitFlashDuration);

            ResetMaterials();
            _isFlashing = false;
        }

        private void ApplyHitMaterial()
        {
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;

                Material[] hitMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < hitMaterials.Length; i++)
                {
                    hitMaterials[i] = _hitMaterial;
                }
                renderer.materials = hitMaterials;
            }
        }

        private void ResetMaterials()
        {
            foreach (var renderer in _renderers)
            {
                if (renderer == null || !_originalMaterials.ContainsKey(renderer)) continue;

                renderer.materials = _originalMaterials[renderer];
            }
        }

        private void OnDestroy()
        {
            if (_isFlashing && _flashRoutine != null)
            {
                StopCoroutine(_flashRoutine);
                ResetMaterials();
            }
        }
    }
}