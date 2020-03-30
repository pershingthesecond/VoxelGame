﻿// <copyright file="Block.cs" company="VoxelGame">
//     All rights reserved.
// </copyright>
// <author>pershingthesecond</author>
using OpenTK;
using System;
using System.Collections.Generic;
using VoxelGame.Physics;

namespace VoxelGame.Logic
{
    /// <summary>
    /// The basic block class. Blocks are used to construct the world.
    /// </summary>
    public abstract class Block
    {
        #region STATIC BLOCK MANAGMENT

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static Block AIR;
        public static Block GRASS;
        public static Block TALL_GRASS;
        public static Block DIRT;
        public static Block STONE;
        public static Block COBBLESTONE;
        public static Block LOG;
        public static Block LEAVES;
        public static Block SAND;
        public static Block GLASS;
        public static Block ORE_COAL;
        public static Block ORE_IRON;
        public static Block ORE_GOLD;
        public static Block SNOW;
        public static Block FLOWER;

        public static Dictionary<ushort, Block> blockDictionary = new Dictionary<ushort, Block>();
#pragma warning restore CA2211 // Non-constant fields should not be visible

        public static void LoadBlocks()
        {
            AIR = new AirBlock("air");
            GRASS = new BasicBlock("grass", true, true, (0, 0, 0, 0, 1, 2), true);
            TALL_GRASS = new CrossBlock("tall_grass", true,BoundingBox.Block);
            DIRT = new BasicBlock("dirt", true, true, (0, 0, 0, 0, 0, 0), true);
            STONE = new BasicBlock("stone", true, true, (0, 0, 0, 0, 0, 0), true);
            COBBLESTONE = new BasicBlock("cobblestone", true, true, (0, 0, 0, 0, 0, 0), true);
            LOG = new BasicBlock("log", true, true, (0, 0, 0, 0, 1, 1), true);
            SAND = new BasicBlock("sand", true, true, (0, 0, 0, 0, 0, 0), true);
            LEAVES = new BasicBlock("leaves", false, true, (0, 0, 0, 0, 0, 0), true);
            GLASS = new BasicBlock("glass", false, false, (0, 0, 0, 0, 0, 0), true);
            ORE_COAL = new BasicBlock("ore_coal", true, true, (0, 0, 0, 0, 0, 0), true);
            ORE_IRON = new BasicBlock("ore_iron", true, true, (0, 0, 0, 0, 0, 0), true);
            ORE_GOLD = new BasicBlock("ore_gold", true, true, (0, 0, 0, 0, 0, 0), true);
            SNOW = new BasicBlock("snow", true, true, (0, 0, 0, 0, 0, 0), true);
            FLOWER = new CrossBlock("flower", false, new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.25f, 0.5f, 0.25f)));
        }

        #endregion STATIC BLOCK MANAGMENT

        /// <summary>
        /// Gets the block id which can be any value from 0 to 4095.
        /// </summary>
        public ushort Id { get; }

        /// <summary>
        /// Gets the name of the block, which is also used for finding the right texture.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets whether this block completely fills a 1x1x1 volume or not.
        /// </summary>
        public bool IsFull { get; }

        /// <summary>
        /// Gets whether it is possible to see through this block. This will affect the rendering of this block and the blocks around him.
        /// </summary>
        public bool IsOpaque { get; }

        /// <summary>
        /// This property is only relevant for non-opaque full blocks. It decides if their faces should be rendered next to another non-opaque block.
        /// </summary>
        public bool RenderFaceAtNonOpaques { get; protected set; } = true;

        /// <summary>
        /// Gets whether this block hinders movement.
        /// </summary>
        public bool IsSolid { get; }

        /// <summary>
        /// Gets whether the collision method should be called in case of a collision with an entity.
        /// </summary>
        public bool RecieveCollisions { get; }

        /// <summary>
        /// Gets whether this block should be checked in collision calculations even if it is not solid.
        /// </summary>
        public bool IsTrigger { get; }

        /// <summary>
        /// Gets whether this block can be replaced when placing a block.
        /// </summary>
        public bool IsReplaceable { get; }

        private BoundingBox boundingBox;

        public Block(string name, bool isFull, bool isOpaque, bool isSolid, bool recieveCollisions, bool isTrigger, bool isReplaceable, BoundingBox boundingBox)
        {
            Name = name;
            IsFull = isFull;
            IsOpaque = isOpaque;
            IsSolid = isSolid;
            RecieveCollisions = recieveCollisions;
            IsTrigger = isTrigger;
            IsReplaceable = isReplaceable;

            this.boundingBox = boundingBox;

            if (blockDictionary.Count < 4096)
            {
                blockDictionary.Add((ushort)blockDictionary.Count, this);
                Id = (ushort)(blockDictionary.Count - 1);
            }
        }

        public virtual BoundingBox GetBoundingBox(int x, int y, int z)
        {
            return new BoundingBox(boundingBox.Center + new Vector3(x, y, z), boundingBox.Extents);
        }

        /// <summary>
        /// Tries to place a block in the world.
        /// </summary>
        /// <param name="x">The x position where a block should be placed.</param>
        /// <param name="y">The y position where a block should be placed.</param>
        /// <param name="z">The z position where a block should be placed.</param>
        /// <param name="entity">The entity that tries to place the block.</param>
        /// <returns>Returns true if placing the block was successful.</returns>
        public virtual bool Place(int x, int y, int z, Entities.PhysicsEntity entity)
        {
            if (Game.World.GetBlock(x, y, z)?.IsReplaceable == false)
            {
                return false;
            }

            Game.World.SetBlock(this, x, y, z);

            return true;
        }

        /// <summary>
        /// Destroys a block in the world if it is the same type as this block.
        /// </summary>
        /// <param name="x">The x position of the block to destroy.</param>
        /// <param name="y">The y position of the block to destroy.</param>
        /// <param name="z">The z position of the block to destroy.</param>
        /// <param name="entity">The entity which caused the destruction.</param>
        /// <returns>Returns true if the block has been destroyed.</returns>
        public virtual bool Destroy(int x, int y, int z, Entities.PhysicsEntity entity)
        {
            if (Game.World.GetBlock(x, y, z) != this)
            {
                return false;
            }

            Game.World.SetBlock(Block.AIR, x, y, z);

            return true;
        }

        public abstract uint GetMesh(BlockSide side, ushort data, out float[] vertecies, out uint[] indicies);

        public abstract void OnCollision(Entities.PhysicsEntity entity, int x, int y, int z);
    }
}