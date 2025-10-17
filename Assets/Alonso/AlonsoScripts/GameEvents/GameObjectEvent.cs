using System;
using UnityEngine;

namespace Assets.Scripts.GameEvents
{
    [CreateAssetMenu(fileName = "GameObject Event", menuName = "GameEvents/GameObjectEvent", order = 3)]
    public class GameObjectEvent : GameEvent <GameObject>
    {

    }
}