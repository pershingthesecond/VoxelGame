﻿// <copyright file="Block.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

using System.Diagnostics;
using VoxelGame.Core.Collections;
using VoxelGame.Core.Entities;
using VoxelGame.Core.Logic.Interfaces;
using VoxelGame.Core.Physics;
using VoxelGame.Core.Visuals;

namespace VoxelGame.Core.Logic
{
    /// <summary>
    /// The basic block class. Blocks are used to construct the world.
    /// </summary>
    public abstract partial class Block : IBlockBase, IIdentifiable<uint>, IIdentifiable<string>
    {
        /// <summary>
        /// Gets the block id which can be any value from 0 to 4095.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Gets the localized name of the block.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// An unlocalized string that identifies this block.
        /// </summary>
        public string NamedId { get; }

        /// <summary>
        /// Gets whether this block completely fills a 1x1x1 volume or not. If a block is not full, it cannot be opaque.
        /// </summary>
        public bool IsFull { get; }

        /// <summary>
        /// Gets whether it is possible to see through this block. This will affect the rendering of this block and the blocks around it.
        /// </summary>
        public bool IsOpaque { get; }

        /// <summary>
        /// This property is only relevant for non-opaque full blocks. It decides if their faces should be rendered next to another non-opaque block.
        /// </summary>
        public bool RenderFaceAtNonOpaques { get; }

        /// <summary>
        /// Gets whether this block hinders movement.
        /// </summary>
        public bool IsSolid { get; }

        /// <summary>
        /// Gets whether the collision method should be called in case of a collision with an entity.
        /// </summary>
        public bool ReceiveCollisions { get; }

        /// <summary>
        /// Gets whether this block should be checked in collision calculations even if it is not solid.
        /// </summary>
        public bool IsTrigger { get; }

        /// <summary>
        /// Gets whether this block can be replaced when placing a block.
        /// </summary>
        public bool IsReplaceable { get; }

        /// <summary>
        /// Gets whether this block responds to interactions.
        /// </summary>
        public bool IsInteractable { get; }

        /// <summary>
        /// Gets the section buffer this blocks mesh data should be stored in.
        /// </summary>
        public TargetBuffer TargetBuffer { get; }

        /// <summary>
        /// Gets whether this block is solid and full.
        /// </summary>
        public bool IsSolidAndFull => IsSolid && IsFull;

        private BoundingBox boundingBox;

        protected Block(string name, string namedId, bool isFull, bool isOpaque, bool renderFaceAtNonOpaques, bool isSolid, bool receiveCollisions, bool isTrigger, bool isReplaceable, bool isInteractable, BoundingBox boundingBox, TargetBuffer targetBuffer)
        {
            Name = name;
            NamedId = namedId;

            IsFull = isFull;
            IsOpaque = isOpaque;
            RenderFaceAtNonOpaques = renderFaceAtNonOpaques;
            IsSolid = isSolid;
            ReceiveCollisions = receiveCollisions;
            IsTrigger = isTrigger;
            IsReplaceable = isReplaceable;
            IsInteractable = isInteractable;

            this.boundingBox = boundingBox;

            TargetBuffer = targetBuffer;

            Debug.Assert(targetBuffer != TargetBuffer.Simple ^ isFull, $"TargetBuffer '{nameof(TargetBuffer.Simple)}' requires {nameof(isFull)} to be {!isFull}, all other target buffers cannot be full.");
            Debug.Assert(isFull || !isOpaque, "A block that is not full cannot be opaque.");
#pragma warning disable S3060 // "is" should not be used with "this"
            Debug.Assert((targetBuffer == TargetBuffer.VaryingHeight) == (this is IHeightVariable), $"The target buffer should be {nameof(TargetBuffer.VaryingHeight)} if and only if the block implements {nameof(IHeightVariable)}.");
#pragma warning restore S3060 // "is" should not be used with "this"

            if (blockDictionary.Count < BlockLimit)
            {
                blockDictionary.Add((uint)blockDictionary.Count, this);
                namedBlockDictionary.Add(namedId, this);

                Id = (uint)(blockDictionary.Count - 1);
            }
            else
            {
                Debug.Fail($"Not more than {BlockLimit} blocks are allowed.");
            }
        }

        uint IIdentifiable<uint>.Id => Id;
        string IIdentifiable<string>.Id => NamedId;

        /// <summary>
        /// Called when loading blocks, meant to setup vertex data, indices etc.
        /// </summary>
        /// <param name="indexProvider"></param>
        protected virtual void Setup(ITextureIndexProvider indexProvider)
        {
        }

        /// <summary>
        /// Returns the bounding box of this block if it would be at the given position.
        /// </summary>
        /// <param name="world">The world in which the block is.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="z">The z position.</param>
        /// <returns>The bounding box.</returns>
        public BoundingBox GetBoundingBox(World world, int x, int y, int z)
        {
            return (world.GetBlock(x, y, z, out uint data) == this ? GetBoundingBox(data) : boundingBox).Translated(x, y, z);
        }

        protected virtual BoundingBox GetBoundingBox(uint data)
        {
            return boundingBox;
        }

        /// <summary>
        /// Returns the mesh of a block side at given conditions.
        /// </summary>
        /// <param name="info">Information about the conditions the mesh should be created in.</param>
        /// <returns>The mesh data.</returns>
        public abstract BlockMeshData GetMesh(BlockMeshInfo info);

        public bool Place(World world, int x, int y, int z, Entities.PhysicsEntity? entity = null)
        {
            (Block? block, Liquid? liquid) = world.GetPosition(x, y, z, out _, out LiquidLevel level, out _);

            bool canPlace = block?.IsReplaceable == true && CanPlace(world, x, y, z, entity);

            if (canPlace)
            {
                DoPlace(world, x, y, z, entity);
            }

            liquid ??= Liquid.None;

            if (liquid != Liquid.None && this is IFillable fillable)
            {
                fillable.LiquidChange(world, x, y, z, liquid, level);
            }

            return canPlace;
        }

        internal virtual bool CanPlace(World world, int x, int y, int z, Entities.PhysicsEntity? entity)
        {
            return true;
        }

        protected virtual void DoPlace(World world, int x, int y, int z, Entities.PhysicsEntity? entity)
        {
            world.SetBlock(this, 0, x, y, z);
        }

        public bool Destroy(World world, int x, int y, int z, Entities.PhysicsEntity? entity = null)
        {
            if (world.GetBlock(x, y, z, out uint data) == this && CanDestroy(world, x, y, z, data, entity))
            {
                DoDestroy(world, x, y, z, data, entity);
                return true;
            }
            else
            {
                return false;
            }
        }

        internal virtual bool CanDestroy(World world, int x, int y, int z, uint data, Entities.PhysicsEntity? entity)
        {
            return true;
        }

        internal virtual void DoDestroy(World world, int x, int y, int z, uint data, Entities.PhysicsEntity? entity)
        {
            world.SetDefaultBlock(x, y, z);
        }

        /// <summary>
        /// This method is called when an entity collides with this block.
        /// </summary>
        /// <param name="entity">The entity that caused the collision.</param>
        /// <param name="x">The x position of the block the entity collided with.</param>
        /// <param name="y">The y position of the block the entity collided with.</param>
        /// <param name="z">The z position of the block the entity collided with.</param>
        public void EntityCollision(Entities.PhysicsEntity entity, int x, int y, int z)
        {
            if (entity.World.GetBlock(x, y, z, out uint data) == this)
            {
                EntityCollision(entity, x, y, z, data);
            }
        }

        protected virtual void EntityCollision(Entities.PhysicsEntity entity, int x, int y, int z, uint data)
        {
        }

        /// <summary>
        /// Called when a block and an entity collide.
        /// </summary>
        /// <param name="entity">The entity that collided with the block.</param>
        /// <param name="x">The x position of the block.</param>
        /// <param name="y">The y position of the block.</param>
        /// <param name="z">The z position of the block.</param>
        public void EntityInteract(Entities.PhysicsEntity entity, int x, int y, int z)
        {
            if (entity.World.GetBlock(x, y, z, out uint data) == this)
            {
                EntityInteract(entity, x, y, z, data);
            }
        }

        protected virtual void EntityInteract(Entities.PhysicsEntity entity, int x, int y, int z, uint data)
        {
        }

        /// <summary>
        /// This method is called on blocks next to a position that was changed.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="x">The x position of the block next to the changed position.</param>
        /// <param name="y">The y position of the block next to the changed position.</param>
        /// <param name="z">The z position of the block next to the changed position.</param>
        /// <param name="data">The data of the block next to the changed position.</param>
        /// <param name="side">The side of the block where the change happened.</param>
        internal virtual void BlockUpdate(World world, int x, int y, int z, uint data, BlockSide side)
        {
        }

        /// <summary>
        /// This method is called randomly on some blocks every update.
        /// </summary>
        internal virtual void RandomUpdate(World world, int x, int y, int z, uint data)
        {
        }

        protected virtual void ScheduledUpdate(World world, int x, int y, int z, uint data)
        {
        }

        public sealed override string ToString()
        {
            return NamedId;
        }
    }
}