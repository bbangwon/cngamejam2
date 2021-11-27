using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

namespace cngamejam{

    public class EffectManager : MonoSingleton<EffectManager>
    {
        [SerializeField]
        GameObject[] attackEffectPrefabs;

        List<(LeanGameObjectPool pool, float animTime)> attackEffectPools = new List<(LeanGameObjectPool pool, float animTime)>();

        private void Awake()
        {
            //공격 이펙트
            foreach (var effectPrefab in attackEffectPrefabs)
            {
                GameObject newEffectFolder = new GameObject(effectPrefab.name);
                newEffectFolder.transform.SetParent(transform, false);

                LeanGameObjectPool pool = newEffectFolder.AddComponent<LeanGameObjectPool>();
                pool.Prefab = effectPrefab;

                float animTime = effectPrefab.GetComponent<Animator>().runtimeAnimatorController.animationClips.FirstOrDefault().length;

                attackEffectPools.Add((pool, animTime));
            }
        }

        public async UniTask SpawnAttackEffect(Vector3 position)
        {
            var pool = attackEffectPools.OrderBy(order => UnityEngine.Random.value).FirstOrDefault();

            GameObject spawnEffect = pool.pool.Spawn(position, Quaternion.identity, transform);

            await UniTask.Delay(TimeSpan.FromSeconds(pool.animTime).Milliseconds);

            pool.pool.Despawn(spawnEffect);
            
        }
    }
}
