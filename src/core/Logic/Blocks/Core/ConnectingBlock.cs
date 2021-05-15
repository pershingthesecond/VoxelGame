﻿// <copyright file="ConnectingBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

using System;
using VoxelGame.Core.Entities;
using VoxelGame.Core.Logic.Interfaces;
using VoxelGame.Core.Physics;
using VoxelGame.Core.Visuals;

namespace VoxelGame.Core.Logic.Blocks
{
    /// <summary>
    /// A base class for blocks that connect to other blocks, like fences or walls.
    /// Data bit usage: <c>--nesw</c>
    /// </summary>
    // n = connected north
    // e = connected east
    // s = connected south
    // w = connected west
    public abstract class ConnectingBlock : Block, IConnectable, IFillable
    {
        private uint postVertexCount;
        private uint extensionVertexCount;

        private float[] postVertices = null!;

        private float[] northVertices = null!;
        private float[] eastVertices = null!;
        private float[] southVertices = null!;
        private float[] westVertices = null!;

        private int[][] textureIndices = null!;

        private uint[][] indices = null!;

        private protected readonly string texture;
        private readonly string post;
        private readonly string extension;

        protected ConnectingBlock(string name, string namedId, string texture, string post, string extension, BoundingBox boundingBox) :
            base(
                name,
                namedId,
                isFull: false,
                isOpaque: false,
                renderFaceAtNonOpaques: true,
                isSolid: true,
                receiveCollisions: false,
                isTrigger: false,
                isReplaceable: false,
                isInteractable: false,
                boundingBox,
                TargetBuffer.Complex)
        {
            this.texture = texture;
            this.post = post;
            this.extension = extension;
        }

        protected override void Setup(ITextureIndexProvider indexProvider)
        {
            BlockModel postModel = BlockModel.Load(this.post);
            BlockModel extensionModel = BlockModel.Load(this.extension);

            postVertexCount = (uint)postModel.VertexCount;
            extensionVertexCount = (uint)extensionModel.VertexCount;

            postModel.ToData(out postVertices, out _, out _);

            extensionModel.RotateY(0, false);
            extensionModel.ToData(out northVertices, out _, out _);

            extensionModel.RotateY(1, false);
            extensionModel.ToData(out eastVertices, out _, out _);

            extensionModel.RotateY(1, false);
            extensionModel.ToData(out southVertices, out _, out _);

            extensionModel.RotateY(1, false);
            extensionModel.ToData(out westVertices, out _, out _);

            int tex = indexProvider.GetTextureIndex(texture);

            textureIndices = new int[5][];

            for (var i = 0; i < 5; i++)
            {
                int[] texInd = new int[postModel.VertexCount + (i * extensionModel.VertexCount)];

                for (var v = 0; v < texInd.Length; v++)
                {
                    texInd[v] = tex;
                }

                textureIndices[i] = texInd;
            }

            indices = new uint[5][];

            for (var i = 0; i < 5; i++)
            {
                uint[] ind = new uint[(postModel.Quads.Length * 6) + (i * extensionModel.Quads.Length * 6)];

                for (var f = 0; f < postModel.Quads.Length + (i * extensionModel.Quads.Length); f++)
                {
                    var offset = (uint)(f * 4);

                    ind[(f * 6) + 0] = 0 + offset;
                    ind[(f * 6) + 1] = 2 + offset;
                    ind[(f * 6) + 2] = 1 + offset;
                    ind[(f * 6) + 3] = 0 + offset;
                    ind[(f * 6) + 4] = 3 + offset;
                    ind[(f * 6) + 5] = 2 + offset;
                }

                indices[i] = ind;
            }
        }

        public override BlockMeshData GetMesh(BlockMeshInfo info)
        {
            bool north = (info.Data & 0b00_1000) != 0;
            bool east = (info.Data & 0b00_0100) != 0;
            bool south = (info.Data & 0b00_0010) != 0;
            bool west = (info.Data & 0b00_0001) != 0;

            int extensions = (north ? 1 : 0) + (east ? 1 : 0) + (south ? 1 : 0) + (west ? 1 : 0);
            var vertexCount = (uint)(postVertexCount + (extensions * extensionVertexCount));

            float[] vertices = new float[vertexCount * 8];
            int[] currentTextureIndices = this.textureIndices[extensions];
            uint[] currentIndices = this.indices[extensions];

            // Combine the required vertices into one array
            var position = 0;
            Array.Copy(postVertices, 0, vertices, 0, postVertices.Length);
            position += postVertices.Length;

            if (north)
            {
                Array.Copy(northVertices, 0, vertices, position, northVertices.Length);
                position += northVertices.Length;
            }

            if (east)
            {
                Array.Copy(eastVertices, 0, vertices, position, eastVertices.Length);
                position += eastVertices.Length;
            }

            if (south)
            {
                Array.Copy(southVertices, 0, vertices, position, southVertices.Length);
                position += southVertices.Length;
            }

            if (west)
            {
                Array.Copy(westVertices, 0, vertices, position, westVertices.Length);
            }

            return new BlockMeshData(vertexCount, vertices, currentTextureIndices, currentIndices);
        }

        protected override void DoPlace(int x, int y, int z, PhysicsEntity? entity)
        {
            uint data = 0;

            // Check the neighboring blocks
            if (Game.World.GetBlock(x, y, z - 1, out _) is IConnectable north && north.IsConnectable(BlockSide.Front, x, y, z - 1))
                data |= 0b00_1000;
            if (Game.World.GetBlock(x + 1, y, z, out _) is IConnectable east && east.IsConnectable(BlockSide.Left, x + 1, y, z))
                data |= 0b00_0100;
            if (Game.World.GetBlock(x, y, z + 1, out _) is IConnectable south && south.IsConnectable(BlockSide.Back, x, y, z + 1))
                data |= 0b00_0010;
            if (Game.World.GetBlock(x - 1, y, z, out _) is IConnectable west && west.IsConnectable(BlockSide.Right, x - 1, y, z))
                data |= 0b00_0001;

            Game.World.SetBlock(this, data, x, y, z);
        }

        internal override void BlockUpdate(int x, int y, int z, uint data, BlockSide side)
        {
            uint newData = data;

            newData = side switch
            {
                BlockSide.Back => CheckNeighbor(x, y, z - 1, BlockSide.Front, 0b00_1000, newData),
                BlockSide.Right => CheckNeighbor(x + 1, y, z, BlockSide.Left, 0b00_0100, newData),
                BlockSide.Front => CheckNeighbor(x, y, z + 1, BlockSide.Back, 0b00_0010, newData),
                BlockSide.Left => CheckNeighbor(x - 1, y, z, BlockSide.Right, 0b00_0001, newData),
                _ => newData
            };

            if (newData != data)
            {
                Game.World.SetBlock(this, newData, x, y, z);
            }

            static uint CheckNeighbor(int x, int y, int z, BlockSide side, uint mask, uint newData)
            {
                if (Game.World.GetBlock(x, y, z, out _) is IConnectable neighbor && neighbor.IsConnectable(side, x, y, z))
                {
                    newData |= mask;
                }
                else
                {
                    newData &= ~mask;
                }

                return newData;
            }
        }
    }
}