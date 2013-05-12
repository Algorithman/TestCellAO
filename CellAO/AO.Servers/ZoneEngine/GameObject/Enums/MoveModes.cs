using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.GameObject.Enums
{
    /// <summary>
    /// Enumeration of Move modes
    /// </summary>
    public enum MoveModes
    {
        None,

        Rooted,

        Walk,

        Run,

        Swim,

        Crawl,

        Sneak,

        Fly,

        Sit,

        SocialTemp, // NV: What is this again exactly?
        Nothing,

        Sleep,

        Lounge
    }

    /// <summary>
    /// Enumeration of Spin or Strafe directions
    /// </summary>
    public enum SpinOrStrafeDirections
    {
        None,

        Left,

        Right
    }

    /// <summary>
    /// Enumeration of Move directions
    /// </summary>
    public enum MoveDirections
    {
        None,

        Forwards,

        Backwards
    }

}
