﻿// <copyright file="DoubleCrossPlant.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using VoxelGame.Entities;
using VoxelGame.Logic.Interfaces;
using VoxelGame.Physics;
using VoxelGame.Rendering;

namespace VoxelGame.Logic.Blocks
{
    /// <summary>
    /// Similar to <see cref="CrossPlantBlock"/>, but is two blocks high.
    /// Data bit usage: <c>----h</c>
    /// </summary>
    // h = height
    public class DoubleCrossPlantBlock : Block
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected float[] vertices = null!;
        protected int[] bottomTexIndices = null!;
        protected int[] topTexIndices = null!;

        protected readonly uint[] indices =
        {
            // Direction: /
            0, 2, 1,
            0, 3, 2,

            0, 1, 2,
            0, 2, 3,

            // Direction: \
            4, 6, 5,
            4, 7, 6,

            4, 5, 6,
            4, 6, 7
        };

#pragma warning disable CA1051 // Do not declare visible instance fields

        public DoubleCrossPlantBlock(string name, string bottomTexture, int topTexOffset, BoundingBox boundingBox) :
            base(
                name,
                isFull: false,
                isOpaque: false,
                renderFaceAtNonOpaques: false,
                isSolid: false,
                recieveCollisions: false,
                isTrigger: false,
                isReplaceable: false,
                boundingBox,
                TargetBuffer.Complex)
        {
#pragma warning disable CA2214 // Do not call overridable methods in constructors
            Setup(bottomTexture, topTexOffset);
#pragma warning restore CA2214 // Do not call overridable methods in constructors
        }

        protected virtual void Setup(string bottomTexture, int topTexOffset)
        {
            vertices = new float[]
            {
                // Two sides: /
                0f, 0f, 1f, 0f, 0f, 0f, 0f, 0f,
                0f, 1f, 1f, 0f, 1f, 0f, 0f, 0f,
                1f, 1f, 0f, 1f, 1f, 0f, 0f, 0f,
                1f, 0f, 0f, 1f, 0f, 0f, 0f, 0f,

                // Two sides: \
                0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0f, 1f, 0f, 0f, 1f, 0f, 0f, 0f,
                1f, 1f, 1f, 1f, 1f, 0f, 0f, 0f,
                1f, 0f, 1f, 1f, 0f, 0f, 0f, 0f
            };

            int tex = Game.BlockTextureArray.GetTextureIndex(bottomTexture);
            bottomTexIndices = new int[] { tex, tex, tex, tex, tex, tex, tex, tex };

            tex += topTexOffset;
            topTexIndices = new int[] { tex, tex, tex, tex, tex, tex, tex, tex };
        }

        public override bool Place(int x, int y, int z, PhysicsEntity? entity)
        {
            if (Game.World.GetBlock(x, y, z, out _)?.IsReplaceable != true || Game.World.GetBlock(x, y + 1, z, out _)?.IsReplaceable != true || !((Game.World.GetBlock(x, y - 1, z, out _) ?? Block.AIR) is IPlantable))
            {
                return false;
            }

            Game.World.SetBlock(this, 0, x, y, z);
            Game.World.SetBlock(this, 1, x, y + 1, z);

            return true;
        }

        public override bool Destroy(int x, int y, int z, PhysicsEntity? entity)
        {
            if (Game.World.GetBlock(x, y, z, out byte data) != this)
            {
                return false;
            }

            bool isBase = (data & 0b1) == 0;

            Game.World.SetBlock(Block.AIR, 0, x, y, z);
            Game.World.SetBlock(Block.AIR, 0, x, y + (isBase ? 1 : -1), z);

            return true;
        }

        public override uint GetMesh(BlockSide side, byte data, out float[] vertices, out int[] textureIndices, out uint[] indices, out TintColor tint)
        {
            vertices = this.vertices;
            textureIndices = ((data & 0b1) == 0) ? bottomTexIndices : topTexIndices;
            indices = this.indices;
            tint = TintColor.Neutral;

            return 8;
        }

        public override void BlockUpdate(int x, int y, int z, byte data)
        {
            // Check if this block is the lower part and if the ground supports plant growth.
            if ((data & 0b1) == 0 && !((Game.World.GetBlock(x, y - 1, z, out _) ?? Block.AIR) is IPlantable))
            {
                Destroy(x, y, z, null);
            }
        }
    }
}