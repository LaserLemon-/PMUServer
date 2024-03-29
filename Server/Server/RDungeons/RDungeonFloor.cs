﻿using System;
using System.Collections.Generic;
using System.Text;
using Server.Maps;

namespace Server.RDungeons
{
    public class RDungeonFloor
    {
        GeneratorOptions options;
			
        public GeneratorOptions Options { get {return options;} }
			
        
        public int Darkness { get; set; }

        

        public Enums.RFloorGoalType GoalType { get; set; }

        public int GoalMap { get; set; }
        public int GoalX { get; set; }
        public int GoalY { get; set; }
        
        public string Music { get; set; }
        
        
        
        #region Terrain Variables

        public int StairsX { get; set; }
        public int StairsSheet { get; set; }

        public int mGroundX { get; set; }
        public int mGroundSheet { get; set; }

        public int mTopLeftX { get; set; }
        public int mTopLeftSheet { get; set; }
        public int mTopCenterX { get; set; }
        public int mTopCenterSheet { get; set; }
        public int mTopRightX { get; set; }
        public int mTopRightSheet { get; set; }

        public int mCenterLeftX { get; set; }
        public int mCenterLeftSheet { get; set; }
        public int mCenterCenterX { get; set; }
        public int mCenterCenterSheet { get; set; }
        public int mCenterRightX { get; set; }
        public int mCenterRightSheet { get; set; }

        public int mBottomLeftX { get; set; }
        public int mBottomLeftSheet { get; set; }
        public int mBottomCenterX { get; set; }
        public int mBottomCenterSheet { get; set; }
        public int mBottomRightX { get; set; }
        public int mBottomRightSheet { get; set; }

        public int mInnerTopLeftX { get; set; }
        public int mInnerTopLeftSheet { get; set; }
        public int mInnerBottomLeftX { get; set; }
        public int mInnerBottomLeftSheet { get; set; }
        public int mInnerTopRightX { get; set; }
        public int mInnerTopRightSheet { get; set; }
        public int mInnerBottomRightX { get; set; }
        public int mInnerBottomRightSheet { get; set; }

        public int mIsolatedWallX { get; set; }
        public int mIsolatedWallSheet { get; set; }
        public int mColumnTopX { get; set; }
        public int mColumnTopSheet { get; set; }
        public int mColumnCenterX { get; set; }
        public int mColumnCenterSheet { get; set; }
        public int mColumnBottomX { get; set; }
        public int mColumnBottomSheet { get; set; }

        public int mRowLeftX { get; set; }
        public int mRowLeftSheet { get; set; }
        public int mRowCenterX { get; set; }
        public int mRowCenterSheet { get; set; }
        public int mRowRightX { get; set; }
        public int mRowRightSheet { get; set; }
        
        public int mGroundAltX { get; set; }
        public int mGroundAltSheet { get; set; }
        public int mGroundAlt2X { get; set; }
        public int mGroundAlt2Sheet { get; set; }
        
        public int mTopLeftAltX { get; set; }
        public int mTopLeftAltSheet { get; set; }
        public int mTopCenterAltX { get; set; }
        public int mTopCenterAltSheet { get; set; }
        public int mTopRightAltX { get; set; }
        public int mTopRightAltSheet { get; set; }

        public int mCenterLeftAltX { get; set; }
        public int mCenterLeftAltSheet { get; set; }
        public int mCenterCenterAltX { get; set; }
        public int mCenterCenterAltSheet { get; set; }
        public int mCenterCenterAlt2X { get; set; }
        public int mCenterCenterAlt2Sheet { get; set; }
        public int mCenterRightAltX { get; set; }
        public int mCenterRightAltSheet { get; set; }

        public int mBottomLeftAltX { get; set; }
        public int mBottomLeftAltSheet { get; set; }
        public int mBottomCenterAltX { get; set; }
        public int mBottomCenterAltSheet { get; set; }
        public int mBottomRightAltX { get; set; }
        public int mBottomRightAltSheet { get; set; }

        public int mInnerTopLeftAltX { get; set; }
        public int mInnerTopLeftAltSheet { get; set; }
        public int mInnerBottomLeftAltX { get; set; }
        public int mInnerBottomLeftAltSheet { get; set; }
        public int mInnerTopRightAltX { get; set; }
        public int mInnerTopRightAltSheet { get; set; }
        public int mInnerBottomRightAltX { get; set; }
        public int mInnerBottomRightAltSheet { get; set; }

        public int mIsolatedWallAltX { get; set; }
        public int mIsolatedWallAltSheet { get; set; }
        public int mColumnTopAltX { get; set; }
        public int mColumnTopAltSheet { get; set; }
        public int mColumnCenterAltX { get; set; }
        public int mColumnCenterAltSheet { get; set; }
        public int mColumnBottomAltX { get; set; }
        public int mColumnBottomAltSheet { get; set; }

        public int mRowLeftAltX { get; set; }
        public int mRowLeftAltSheet { get; set; }
        public int mRowCenterAltX { get; set; }
        public int mRowCenterAltSheet { get; set; }
        public int mRowRightAltX { get; set; }
        public int mRowRightAltSheet { get; set; }
        
        
        public int mWaterX { get; set; }
        public int mWaterSheet { get; set; }
        public int mWaterAnimX { get; set; }
        public int mWaterAnimSheet { get; set; }

        public int mShoreTopLeftX { get; set; }
        public int mShoreTopLeftSheet { get; set; }
        public int mShoreTopRightX { get; set; }
        public int mShoreTopRightSheet { get; set; }
        public int mShoreBottomRightX { get; set; }
        public int mShoreBottomRightSheet { get; set; }
        public int mShoreBottomLeftX { get; set; }
        public int mShoreBottomLeftSheet { get; set; }
        public int mShoreDiagonalForwardX { get; set; }
        public int mShoreDiagonalForwardSheet { get; set; }
        public int mShoreDiagonalBackX { get; set; }
        public int mShoreDiagonalBackSheet { get; set; }
        public int mShoreTopX { get; set; }
        public int mShoreTopSheet { get; set; }
        public int mShoreRightX { get; set; }
        public int mShoreRightSheet { get; set; }
        public int mShoreBottomX { get; set; }
        public int mShoreBottomSheet { get; set; }
        public int mShoreLeftX { get; set; }
        public int mShoreLeftSheet { get; set; }
        public int mShoreVerticalX { get; set; }
        public int mShoreVerticalSheet { get; set; }
        public int mShoreHorizontalX { get; set; }
        public int mShoreHorizontalSheet { get; set; }
        public int mShoreInnerTopLeftX { get; set; }
        public int mShoreInnerTopLeftSheet { get; set; }
        public int mShoreInnerTopRightX { get; set; }
        public int mShoreInnerTopRightSheet { get; set; }
        public int mShoreInnerBottomRightX { get; set; }
        public int mShoreInnerBottomRightSheet { get; set; }
        public int mShoreInnerBottomLeftX { get; set; }
        public int mShoreInnerBottomLeftSheet { get; set; }
        public int mShoreInnerTopX { get; set; }
        public int mShoreInnerTopSheet { get; set; }
        public int mShoreInnerRightX { get; set; }
        public int mShoreInnerRightSheet { get; set; }
        public int mShoreInnerBottomX { get; set; }
        public int mShoreInnerBottomSheet { get; set; }
        public int mShoreInnerLeftX { get; set; }
        public int mShoreInnerLeftSheet { get; set; }
        public int mShoreSurroundedX { get; set; }
        public int mShoreSurroundedSheet { get; set; }
        
        public int mShoreTopLeftAnimX { get; set; }
        public int mShoreTopLeftAnimSheet { get; set; }
        public int mShoreTopRightAnimX { get; set; }
        public int mShoreTopRightAnimSheet { get; set; }
        public int mShoreBottomRightAnimX { get; set; }
        public int mShoreBottomRightAnimSheet { get; set; }
        public int mShoreBottomLeftAnimX { get; set; }
        public int mShoreBottomLeftAnimSheet { get; set; }
        public int mShoreDiagonalForwardAnimX { get; set; }
        public int mShoreDiagonalForwardAnimSheet { get; set; }
        public int mShoreDiagonalBackAnimX { get; set; }
        public int mShoreDiagonalBackAnimSheet { get; set; }
        public int mShoreTopAnimX { get; set; }
        public int mShoreTopAnimSheet { get; set; }
        public int mShoreRightAnimX { get; set; }
        public int mShoreRightAnimSheet { get; set; }
        public int mShoreBottomAnimX { get; set; }
        public int mShoreBottomAnimSheet { get; set; }
        public int mShoreLeftAnimX { get; set; }
        public int mShoreLeftAnimSheet { get; set; }
        public int mShoreVerticalAnimX { get; set; }
        public int mShoreVerticalAnimSheet { get; set; }
        public int mShoreHorizontalAnimX { get; set; }
        public int mShoreHorizontalAnimSheet { get; set; }
        public int mShoreInnerTopLeftAnimX { get; set; }
        public int mShoreInnerTopLeftAnimSheet { get; set; }
        public int mShoreInnerTopRightAnimX { get; set; }
        public int mShoreInnerTopRightAnimSheet { get; set; }
        public int mShoreInnerBottomRightAnimX { get; set; }
        public int mShoreInnerBottomRightAnimSheet { get; set; }
        public int mShoreInnerBottomLeftAnimX { get; set; }
        public int mShoreInnerBottomLeftAnimSheet { get; set; }
        public int mShoreInnerTopAnimX { get; set; }
        public int mShoreInnerTopAnimSheet { get; set; }
        public int mShoreInnerRightAnimX { get; set; }
        public int mShoreInnerRightAnimSheet { get; set; }
        public int mShoreInnerBottomAnimX { get; set; }
        public int mShoreInnerBottomAnimSheet { get; set; }
        public int mShoreInnerLeftAnimX { get; set; }
        public int mShoreInnerLeftAnimSheet { get; set; }
        public int mShoreSurroundedAnimX { get; set; }
        public int mShoreSurroundedAnimSheet { get; set; }

        #endregion
        
        public Tile GroundTile { get; set; }
        public Tile HallTile { get; set; }
        public Tile WaterTile { get; set; }
        public Tile WallTile { get; set; }

        public int NpcSpawnTime { get; set; }
        public int NpcMin { get; set; }
        public int NpcMax { get; set; }
        
        public List<RDungeonItem> Items { get; set; }
        public List <MapNpcPreset> Npcs { get; set; }
        public List<RDungeonTrap> SpecialTiles { get; set; }
        public List<Enums.Weather> Weather { get; set; }

        public RDungeonFloor()
        {
            GroundTile = new Tile(new DataManager.Maps.Tile());
            HallTile = new Tile(new DataManager.Maps.Tile());
            WaterTile = new Tile(new DataManager.Maps.Tile());
            WallTile = new Tile(new DataManager.Maps.Tile());
            SpecialTiles = new List<RDungeonTrap>();
            Weather = new List<Enums.Weather>();
            Npcs = new List<MapNpcPreset>();
            Items = new List<RDungeonItem>();
            options = new GeneratorOptions();
        }
    }
}
