using Runtime;
using UnityEngine;

namespace ___my_tests
{
    public class GameManager : Singleton<GameManager>
    {
        // some serialize fields
        // awake method with a level loading logic 

        protected override void Awake()
        {
            // levelManager => prepare asking level 
        } 
    }
}
