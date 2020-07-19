﻿// <copyright file="GroundedBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using VoxelGame.Entities;

namespace VoxelGame.Logic.Blocks
{
    /// <summary>
    /// A BasicBlock that can only be placed on top of blocks that are both solid and full.
    /// </summary>
    public class GroundedBlock : BasicBlock
    {
        public GroundedBlock(string name, string namedId, TextureLayout layout, bool isOpaque = true, bool renderFaceAtNonOpaques = true, bool isSolid = true, bool isInteractable = false) :
            base(
                name,
                namedId,
                layout,
                isOpaque,
                renderFaceAtNonOpaques,
                isSolid,
                isInteractable)
        {
        }

        protected override bool Place(PhysicsEntity? entity, int x, int y, int z)
        {
            if (Game.World.GetBlock(x, y - 1, z, out _)?.IsSolidAndFull == true)
            {
                Game.World.SetBlock(this, 0, x, y, z);

                return true;
            }
            else
            {
                return false;
            }
        }

        internal override void BlockUpdate(int x, int y, int z, byte data, BlockSide side)
        {
            if (side == BlockSide.Bottom && Game.World.GetBlock(x, y - 1, z, out _)?.IsSolidAndFull != true)
            {
                Destroy(x, y, z, null);
            }
        }
    }
}