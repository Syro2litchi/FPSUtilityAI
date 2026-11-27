using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZombieFPS.Director
{
    public class MyDirector : MonoBehaviour, IAIDirector
    {
        [SerializeField] private List<UtilityAction> _actions;
        public Dictionary<string, float> Context = new();
        
        [SerializeField] private float spawnCooldown = 5f;
        
        private float spawnTimer = 0f;
        private float maxTimeOutOfFight = 30f;
        public void OnGameStart(DirectorContext context, DirectorActions actions)
        {
            context.Events.PlayerDamaged += amount => { /* ... */ };
            context.Events.PlayerHealed += amount => { /* ... */ };
            context.Events.PlayerDamaged += amount =>
            {
                Debug.Log("Toto as pris " + amount + " dans la gueule");
            };
            
            //Context for player
            Context.Add("PlayerHealth", context.Player.Health);
            Context.Add("PlayerProgress", context.Player.Progress);
            Context.Add("PlayerSpeed", context.Player.Speed);
            Context.Add("PlayerReserveAmmo", context.Player.ReserveAmmo);
            Context.Add("PlayerGrenades", context.Player.Grenades);
            
            //Context for World
            Context.Add("GameTime", context.GameTime);
            
            //Stats
            Context.Add("PlayerAccuracy", context.Stats.Accuracy);
            Context.Add("HeadShots", context.Stats.TotalHeadshots);
            Context.Add("TotalKills", context.Stats.TotalKills);
            Context.Add("TotalHealing", context.Stats.TotalHealing);
            Context.Add("TimeSincePickUp", context.Stats.TimeSincePickup);
            Context.Add("TimeSinceKill", context.Stats.TimeSinceKill);
            Context.Add("TimeOutOfFight", context.Stats.TimeSinceLastCombat);
            Context.Add("PickUpsCollected", context.Stats.TotalPickupsCollected);
        }

        //read context -> score option -> call Action
        public void Tick(DirectorContext context, DirectorActions actions, float deltaTime)
        {
            spawnTimer = Mathf.Max(0f, spawnTimer - deltaTime);
            Context["PlayerHealth"] = context.Player.Health;
            Context["PlayerProgress"] = context.Player.Progress;
            Context["PlayerSpeed"] = context.Player.Speed;
            Context["GameTime"] = context.GameTime;
            float time = context.Stats.TimeSinceLastCombat;

            if (float.IsPositiveInfinity(time))
            {
                time = context.GameTime;
            }

            Context["TimeOutOfFight"] = Mathf.Clamp01(time / 30f);
            
            
            var chosenAction = ChooseAction();
            if (chosenAction.Name == "SpawnSingleEnemy" && spawnTimer <= 0f)
            {
                SpawnPointInfo spawnPoint = ChooseSpawnPoint(context, SpawnPointKind.Enemy);

                if (spawnPoint != null)
                {
                    if (actions.SpawnEnemy(EnemyType.Common, spawnPoint, false))
                    {
                        spawnTimer = spawnCooldown;
                    }
                }
            }

            if (chosenAction.Name == "SpawnHealthPickUp")
            {
                SpawnPointInfo spawnPoint = ChooseSpawnPoint(context, SpawnPointKind.Pickup);

                actions.SpawnPickup(PickupType.Health, spawnPoint);
            }
            
        }
        
        
        private UtilityAction ChooseAction() {
            // Selection
            UtilityAction bestAction = null;
            float bestScore = -1f;
            foreach (UtilityAction utilityAction in _actions) {
                float score = utilityAction.Evaluate(Context);
                if (score > bestScore) {
                    bestScore = score;
                    bestAction = utilityAction;
                }
            }
            return bestAction;
        }
        
        private SpawnPointInfo ChooseSpawnPoint(DirectorContext context, SpawnPointKind spawnPointKind)
        {
            SpawnPointInfo bestPoint = null;
            float bestDistance = float.PositiveInfinity;

            foreach (SpawnPointInfo point in context.EnemySpawnPoints)
            {
                if (point.Kind != spawnPointKind)
                    continue;

                if (point.IsVisibleToPlayer)
                    continue;

                if (!point.IsAheadOfPlayer)
                    continue;

                if (point.DistanceToPlayer < 10f)
                    continue;

                if (point.DistanceToPlayer < bestDistance)
                {
                    bestDistance = point.DistanceToPlayer;
                    bestPoint = point;
                }
            }

            return bestPoint;
        }
    }
}
