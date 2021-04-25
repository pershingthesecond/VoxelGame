﻿// <copyright file="GrassBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

using VoxelGame.Core.Entities;
using VoxelGame.Core.Logic.Interfaces;
using VoxelGame.Core.Utilities;
using VoxelGame.Core.Visuals;

namespace VoxelGame.Core.Logic.Blocks
{
    /// <summary>
    /// A block that changes into dirt when something is placed on top of it. This block can use a neutral tint if specified in the constructor.
    /// Data bit usage: <c>------</c>
    /// </summary>
    public class CoveredDirtBlock : BasicBlock, IFillable, IPlantable
    {
        private readonly bool hasNeutralTint;
        private readonly bool supportsFullGrowth;

        private int[] wetTextureIndices = null!;
        private readonly TextureLayout wet;

        public bool SupportsFullGrowth => supportsFullGrowth;

        protected CoveredDirtBlock(string name, string namedId, TextureLayout normal, TextureLayout wet, bool hasNeutralTint, bool supportsFullGrowth) :
            base(
                name,
                namedId,
                layout: normal,
                isOpaque: true,
                renderFaceAtNonOpaques: true,
                isSolid: true,
                receiveCollisions: false,
                isTrigger: false,
                isInteractable: false)
        {
            this.hasNeutralTint = hasNeutralTint;
            this.supportsFullGrowth = supportsFullGrowth;

            this.wet = wet;
        }

        protected override void Setup()
        {
            base.Setup();

            wetTextureIndices = wet.GetTexIndexArray();
        }

        public override BlockMeshData GetMesh(BlockMeshInfo info)
        {
            BlockMeshData mesh = base.GetMesh(info);

            mesh = mesh.Modified((hasNeutralTint) ? TintColor.Neutral : TintColor.None);

            if (info.Liquid.Direction > 0) mesh = mesh.SwapTextureIndex(wetTextureIndices[(int)info.Side]);

            return mesh;
        }

        internal override bool CanPlace(int x, int y, int z, PhysicsEntity? entity)
        {
            return !Game.World.HasSolidTop(x, y, z) || Block.Dirt.CanPlace(x, y, z, entity);
        }

        protected override void DoPlace(int x, int y, int z, PhysicsEntity? entity)
        {
            if (Game.World.HasSolidTop(x, y, z))
            {
                Block.Dirt.Place(x, y, z, entity);
            }
            else
            {
                Game.World.SetBlock(this, 0, x, y, z);
            }
        }

        internal override void BlockUpdate(int x, int y, int z, uint data, BlockSide side)
        {
            if (side == BlockSide.Top && Game.World.HasOpaqueTop(x, y, z))
            {
                Game.World.SetBlock(Block.Dirt, 0, x, y, z);
            }
        }

        public virtual bool AllowInflow(int x, int y, int z, BlockSide side, Liquid liquid)
        {
            return liquid.Viscosity < 100;
        }
    }
}