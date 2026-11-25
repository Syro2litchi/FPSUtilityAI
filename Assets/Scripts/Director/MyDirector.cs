using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZombieFPS.Director
{
    public class MyDirector : MonoBehaviour, IAIDirector
    {
        public Dictionary<string, float> Context = new();
        
        public void OnGameStart(DirectorContext context, DirectorActions actions)
        {
            context.Events.PlayerDamaged += amount => { /* ... */ };
            context.Events.PlayerHealed += amount => { /* ... */ };
            context.Events.PlayerDamaged += amount =>
            {
                Debug.Log("Toto as pris " + amount + " dans la gueule");
            };
            // Context.Add("Damages", DirectorContext.);
        }

        //read context -> score option -> call Action
        public void Tick(DirectorContext context, DirectorActions actions, float deltaTime)
        {
            
            
        }
    }
}
