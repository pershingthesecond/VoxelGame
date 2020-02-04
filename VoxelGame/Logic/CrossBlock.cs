﻿using System;

using VoxelGame.Rendering;

namespace VoxelGame.Logic
{
    public class CrossBlock : Block
    {
        protected float[] vertices;
        protected uint[] indices =
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

        public CrossBlock(string name) : base(name, false, false)
        {
            Setup();
        }

        protected virtual void Setup()
        {
            int textureIndex = Game.Atlas.GetTextureIndex(Name);

            if (textureIndex == -1)
            {
                throw new Exception($"No texture '{Name}' found!");
            }

            AtlasPosition uv = Game.Atlas.GetTextureUV(textureIndex);

            vertices = new float[]
            {
                // Two sides: /
                0f, 0f, 1f, uv.bottomLeftU, uv.bottomLeftV,
                0f, 1f, 1f, uv.bottomLeftU, uv.topRightV,
                1f, 1f, 0f, uv.topRightU, uv.topRightV,
                1f, 0f, 0f, uv.topRightU, uv.bottomLeftV,

                // Two sides: \
                0f, 0f, 0f, uv.bottomLeftU, uv.bottomLeftV,
                0f, 1f, 0f, uv.bottomLeftU, uv.topRightV,
                1f, 1f, 1f, uv.topRightU, uv.topRightV,
                1f, 0f, 1f, uv.topRightU, uv.bottomLeftV
            };
        }

        public override uint GetMesh(BlockSide side, out float[] vertices, out uint[] indices)
        {
            vertices = this.vertices;
            indices = this.indices;

            return 8;
        }
    }
}
