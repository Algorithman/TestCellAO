using System;
using System.Linq;
using SmokeLounge.AOtomation.Messaging.GameData;

namespace ZoneEngine.GameObject
{
    public interface IInstancedEntity : IEntity
    {
        Identity Playfield { get; set; }
    }
}