﻿// <copyright file="DirtBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

using VoxelGame.Core.Logic.Interfaces;
using VoxelGame.Core.Visuals;

namespace VoxelGame.Core.Logic.Blocks
{
    /// <summary>
    /// A simple block which allows the spread of grass.
    /// Data bit usage: <c>------</c>
    /// </summary>
    public class DirtBlock : BasicBlock, IPlantable, IGrassSpreadable, IFillable
    {
        private int[] wetTextureIndices = null!;

        private readonly TextureLayout wet;

        internal DirtBlock(string name, string namedId, TextureLayout normal, TextureLayout wet) :
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
            this.wet = wet;
        }

        protected override void Setup(ITextureIndexProvider indexProvider)
        {
            base.Setup(indexProvider);

            wetTextureIndices = wet.GetTexIndexArray();
        }

        public override BlockMeshData GetMesh(BlockMeshInfo info)
        {
            BlockMeshData mesh = base.GetMesh(info);

            if (info.Liquid.Direction > 0)
                mesh = mesh.SwapTextureIndex(wetTextureIndices[(int)info.Side]);

            return mesh;
        }

        internal override void RandomUpdate(World world, int x, int y, int z, uint data)
        {
            Liquid? liquid = world.GetLiquid(x, y, z, out LiquidLevel level, out _);

            if (liquid == Liquid.Water && level == LiquidLevel.Eight)
            {
                world.SetBlock(Block.Mud, 0, x, y, z);
            }
        }

        public virtual bool AllowInflow(World world, int x, int y, int z, BlockSide side, Liquid liquid)
        {
            return liquid.Viscosity < 100;
        }
    }
}