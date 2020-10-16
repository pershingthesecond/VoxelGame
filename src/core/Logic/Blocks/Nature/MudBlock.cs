﻿// <copyright file="MudBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using VoxelGame.Core.Entities;
using VoxelGame.Core.Logic.Interfaces;
using VoxelGame.Core.Utilities;

namespace VoxelGame.Core.Logic.Blocks
{
    /// <summary>
    /// A block that slows down entities.
    /// </summary>
    public class MudBlock : BasicBlock, IFillable
    {
        private protected float maxVelocity;

        public MudBlock(string name, string namedId, TextureLayout layout, float maxVelocity) :
            base(
                name,
                namedId,
                layout,
                isOpaque: true,
                renderFaceAtNonOpaques: true,
                isSolid: true,
                recieveCollisions: true,
                isTrigger: false,
                isInteractable: false)
        {
            this.maxVelocity = maxVelocity;
        }

        protected override void EntityCollision(PhysicsEntity entity, int x, int y, int z, uint data)
        {
            entity.Velocity = VMath.Clamp(entity.Velocity, -1f, maxVelocity);
        }

        public virtual bool IsFillable(int x, int y, int z, Liquid liquid)
        {
            return liquid.Viscosity < 200;
        }
    }
}