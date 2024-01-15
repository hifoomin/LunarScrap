using LethalLib.Modules;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using Vector3 = UnityEngine.Vector3;

namespace LunarScrap.Scrap
{
    public static class Utils
    {
        public static Item CreateScrap(string name, int rarity, bool hasIdleSound = true, GameObject idleSoundObject = null)
        {
            var prefab = Main.assetBundle.LoadAsset<Item>(name + ".asset");
            if (!prefab)
            {
                Main.LSLogger.LogError("scrap " + name + " is null");
                return null;
            }
            Utilities.FixMixerGroups(prefab.spawnPrefab);
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(prefab.spawnPrefab);
            Items.RegisterScrap(prefab, rarity, Levels.LevelTypes.All);
            if (hasIdleSound)
            {
                var idleSound = prefab.spawnPrefab.GetComponent<IdleSound>() ? prefab.spawnPrefab.GetComponent<IdleSound>() : prefab.spawnPrefab.AddComponent<IdleSound>();
                idleSound.prefab = idleSoundObject;
            }
            return prefab;
        }

        public static void SpawnEnemyIgnoreCap(int index, bool inside = true)
        {
            if (Server())
            {
                Vector3 position = Vector3.zero;
                float yRotation = 0f;
                if (inside)
                {
                    var randomVent = Random.Range(0, RoundManager.Instance.allEnemyVents.Length);
                    var floorNode = RoundManager.Instance.allEnemyVents[randomVent].floorNode;
                    position = floorNode.position;
                    yRotation = floorNode.eulerAngles.y;
                }
                else
                {
                    int what = 0;
                    var what2 = false;
                    var spawnPoints = GameObject.FindGameObjectsWithTag("OutsideAINode");
                    position = spawnPoints[RoundManager.Instance.AnomalyRandom.Next(0, spawnPoints.Length)].transform.position;
                    position = RoundManager.Instance.GetRandomNavMeshPositionInRadius(position, 4f, default);
                    Main.LSLogger.LogError("trying to spawn outside");
                    for (int i = 0; i < spawnPoints.Length - 1; i++)
                    {
                        Main.LSLogger.LogError("iterating through every spawnpoint");
                        for (int k = 0; k < RoundManager.Instance.spawnDenialPoints.Length; k++)
                        {
                            Main.LSLogger.LogError("iteratring through every spawn denial point");
                            what2 = true;
                            if (Vector3.Distance(position, RoundManager.Instance.spawnDenialPoints[k].transform.position) < 16f)
                            {
                                Main.LSLogger.LogError("checking 16m distance");
                                what = (what + 1) % spawnPoints.Length;
                                position = spawnPoints[what].transform.position;
                                position = RoundManager.Instance.GetRandomNavMeshPositionInRadius(position, 4f, default);
                                what2 = false;
                                break;
                            }
                        }
                        if (what2)
                        {
                            Main.LSLogger.LogError("what2 break");
                            break;
                        }
                    }
                }
                Main.LSLogger.LogError("position is " + position);
                Main.LSLogger.LogError("y rotation is " + yRotation);
                Main.LSLogger.LogError("index is " + index);
                RoundManager.Instance.SpawnEnemyOnServer(position, yRotation, index);
                Main.LSLogger.LogError("spawning enemy");
                // RoundManager.Instance.SpawnedEnemies.Add() guh
            }
        }

        public static void SpawnEffect(GameObject prefab, float lifetime, Vector3 position, UnityEngine.Quaternion rotation)
        {
            if (Server())
            {
                var scaleVFX = prefab.GetComponent<ScaleVFX>() ? prefab.GetComponent<ScaleVFX>() : prefab.AddComponent<ScaleVFX>();
                scaleVFX.lifetime = lifetime;

                var destroyOnTimerNetworked = prefab.GetComponent<DestroyOnTimerNetworked>() ? prefab.GetComponent<DestroyOnTimerNetworked>() : prefab.AddComponent<DestroyOnTimerNetworked>();
                destroyOnTimerNetworked.lifetime = lifetime + 0.1f;

                // Main.LSLogger.LogError("spawned vfx");
                var instantiated = Object.Instantiate(prefab);
                var trans = instantiated.transform;
                trans.position = position;
                trans.rotation = rotation;

                instantiated.GetComponent<NetworkObject>().Spawn();
            }
        }

        public static void SpawnEffect(GameObject prefab, float lifetime, GameObject parent)
        {
            if (Server())
            {
                var scaleVFX = prefab.GetComponent<ScaleVFX>() ? prefab.GetComponent<ScaleVFX>() : prefab.AddComponent<ScaleVFX>();
                scaleVFX.lifetime = lifetime;

                var destroyOnTimerNetworked = prefab.GetComponent<DestroyOnTimerNetworked>() ? prefab.GetComponent<DestroyOnTimerNetworked>() : prefab.AddComponent<DestroyOnTimerNetworked>();
                destroyOnTimerNetworked.lifetime = lifetime + 0.1f;

                // Main.LSLogger.LogError("spawned vfx");
                var instantiated = Object.Instantiate(prefab);
                instantiated.transform.parent = parent.transform.parent;

                instantiated.GetComponent<NetworkObject>().Spawn();
            }
        }

        public static float PlaySound(GameObject prefab, GameObject source, float volume = 1f, float randomizationDeviation = 0.1f)
        {
            float soundDuration = float.NegativeInfinity;

            if (Server())
            {
                var destroyOnTimerNetworked = prefab.GetComponent<DestroyOnTimerNetworked>() ? prefab.GetComponent<DestroyOnTimerNetworked>() : prefab.AddComponent<DestroyOnTimerNetworked>();

                // Main.LSLogger.LogError("spawned standard sfx");

                var instantiated = Object.Instantiate(prefab);
                var trans = instantiated.transform;
                trans.position = source.transform.position;
                trans.rotation = source.transform.rotation;

                var audioSource = instantiated.GetComponent<AudioSource>();
                audioSource.volume = volume;
                var randomAdd = Random.Range(0f, 1f) > 0.5f ? -randomizationDeviation : randomizationDeviation;
                audioSource.pitch = 1f * (1f + randomAdd);
                audioSource.PlayOneShot(audioSource.clip);
                soundDuration = audioSource.clip.length;

                destroyOnTimerNetworked.lifetime = soundDuration + 0.1f;

                instantiated.GetComponent<NetworkObject>().Spawn();
            }
            return soundDuration;
        }

        public static void Shake(ScreenShakeType shakeType)
        {
            HUDManager.Instance.ShakeCamera(shakeType);
        }

        public static bool Server()
        {
            return NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost || (((NetworkBehaviour)RoundManager.Instance).IsHost);
        }
    }

    public class DestroyOnTimerNetworked : MonoBehaviour
    {
        public float lifetime = 3f;
        public float timer = 0f;
        public NetworkObject networkObject;

        public void Start()
        {
            networkObject = GetComponent<NetworkObject>();
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= lifetime)
            {
                networkObject.Despawn();
            }
        }
    }

    public class DestroyOnTimer : MonoBehaviour
    {
        public float lifetime = 3f;
        public float timer = 0f;

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= lifetime)
            {
                Destroy(this);
            }
        }
    }

    public class ScaleVFX : MonoBehaviour
    {
        public float time;
        public float lifetime = 2f;
        public Vector3 baseScale;
        public AnimationCurve scaleCurve;

        public void Start()
        {
            baseScale = transform.localScale;
            scaleCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(0.8f, 1f), new Keyframe(1f, 0f));
            Reset();
        }

        public void OnEnable()
        {
            Reset();
        }

        public void Update()
        {
            time += Time.deltaTime;
            UpdateScale(time);
        }

        public void Reset()
        {
            time = 0f;
            UpdateScale(0f);
        }

        public void UpdateScale(float time)
        {
            var percentLifetime = Mathf.Clamp01(time / lifetime);
            var multiplier = 1f;
            if (scaleCurve != null)
            {
                multiplier = scaleCurve.Evaluate(percentLifetime);
            }

            Vector3 finalScale;
            finalScale = baseScale * multiplier;

            transform.localScale = finalScale * multiplier;
        }
    }

    public class IdleSound : MonoBehaviour
    {
        public GameObject prefab;
        public float timer = 0f;
        public float minInterval = 7f;
        public float maxInterval = 15f;

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= Random.Range(minInterval, maxInterval))
            {
                if (!prefab)
                {
                    return;
                }

                Utils.PlaySound(prefab, gameObject);
                timer = 0f;
            }
        }
    }
}