using System;
using System.Linq;

namespace ZoneEngine.GameObject
{
    public interface IInstancedEntity : IEntity
    {
        int Playfield { get; set; }
    }
}