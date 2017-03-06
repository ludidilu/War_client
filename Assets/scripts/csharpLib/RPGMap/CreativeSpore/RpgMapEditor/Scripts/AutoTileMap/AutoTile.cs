/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;
using System;


namespace CreativeSpore.RpgMapEditor
{

    public enum eLayerType
    {
        Ground, // tiles with predefined collisions
        Overlay, // tiles no collision
        Objects, // objects like triggers and actors
        FogOfWar, // used for fog of war
        Event,
    };

    /// <summary>
    /// An auto-tile has 4 parts that change according to neighbors. These are the different types for each part.
    /// </summary>
    public enum eTilePartType
    {
        INT_CORNER,
        EXT_CORNER,
        INTERIOR,
        H_SIDE, // horizontal sides
        V_SIDE // vertical sides
    }

    [Obsolete("This has been deprecated after adding multiple layer support!")]
    /// <summary>
    /// Each type of tile layer in the map
    /// </summary>
    public enum eTileLayer
    {
        /// <summary>
        /// mostly for tiles with no alpha
        /// </summary>
        GROUND,
        /// <summary>
        /// mostly for tiles with alpha
        /// </summary>
        GROUND_OVERLAY,
        /// <summary>
        /// for tiles that should be drawn over everything else
        /// </summary>
        OVERLAY
    }

    /// <summary>
    /// Each type of tile of the map
    /// </summary>
    public enum eTileType
    {
        /// <summary>
        /// Animated auto-tiles with 3 frames of animation, usually named with _A1 suffix in the texture
        /// </summary>
        ANIMATED,
        /// <summary>
        /// Ground auto-Tiles, usually named with _A2 suffix in the texture
        /// </summary>
        GROUND,
        /// <summary>
        /// Building auto-Tiles, usually named with _A3 suffix in the texture
        /// </summary>
        BUILDINGS,
        /// <summary>
        /// Wall auto-Tiles, usually named with _A4 suffix in the texture
        /// </summary>
        WALLS,
        /// <summary>
        /// Normal tiles, usually named with _A5 suffix in the texture. Same as Objects tiles, but included as part of an auto-tileset
        /// </summary>
        NORMAL,
        /// <summary>
        /// Normal tiles, usually named with _B, _C, _D and _E suffix in the texture
        /// </summary>
        OBJECTS
    };

    /// <summary>
    /// Type map collision according to tile on certain map position
    /// </summary>
    public enum eTileCollisionType
    {
        /// <summary>
        /// Used to indicate the empty tile with no type
        /// </summary>
        EMPTY = -1,
        /// <summary>
        /// A PASSABLE tile over a BLOC, WALL, or FENCE allow walking over it.
        /// </summary>
        PASSABLE,
        /// <summary>
        /// Not passable
        /// </summary>
        BLOCK,
        /// <summary>
        /// Partially not passable, depending of autotiling
        /// </summary>
        WALL,
        /// <summary>
        /// Partially not passable, depending of autotiling
        /// </summary>
        FENCE,
        /// <summary>
        /// A passable tile. Used to check when a tile should be placed in overlay layer
        /// </summary>
        OVERLAY,
        /// <summary>
        /// The size of this enum
        /// </summary>
        _SIZE
    }

    /// <summary>
    /// Define a tile of the map
    /// </summary>        
    public class AutoTile
    {
        public int TilesetIdx;
        public int Id = -1;
        public int MappedIdx;
        public eTileType Type;
        public int TileX;
        public int TileY;
        public int Layer;
        public int[] TilePartsIdx;
        public eTilePartType[] TilePartsType;
        public int TilePartsLength;

        public bool IsWaterTile()
        {
            return Id != -1 && Type == eTileType.ANIMATED; // TODO: temporary fix: if it's an animated tileset, it's considered as water
        }
    }
}
