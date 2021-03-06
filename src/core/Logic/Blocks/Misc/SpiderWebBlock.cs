﻿// <copyright file="SpiderWeb.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

using VoxelGame.Core.Entities;
using VoxelGame.Core.Logic.Interfaces;
using VoxelGame.Core.Physics;
using VoxelGame.Core.Utilities;

namespace VoxelGame.Core.Logic.Blocks
{
    /// <summary>
    /// A block that slows down entities that collide with it.
    /// Data bit usage: <c>------</c>
    /// </summary>
    public class SpiderWebBlock : CrossBlock, IFlammable, IFillable
    {
        private readonly float maxVelocity;

        /// <summary>
        /// Creates a SpiderWeb block, a block that slows down entities that collide with it.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <param name="namedId">The unique and unlocalized name of this block.</param>
        /// <param name="texture">The texture of this block.</param>
        /// <param name="maxVelocity">The maximum velocity of entities colliding with this block.</param>
        internal SpiderWebBlock(string name, string namedId, string texture, float maxVelocity) :
            base(
                name,
                namedId,
                texture,
                receiveCollisions: true,
                isTrigger: true,
                isReplaceable: false,
                BoundingBox.CrossBlock)
        {
            this.maxVelocity = maxVelocity;
        }

        protected override void EntityCollision(PhysicsEntity entity, int x, int y, int z, uint data)
        {
            entity.Velocity = VMath.Clamp(entity.Velocity, -1f, maxVelocity);
        }

        public void LiquidChange(World world, int x, int y, int z, Liquid liquid, LiquidLevel level)
        {
            if (liquid.Direction > 0) ScheduleDestroy(world, x, y, z);
        }
    }
}