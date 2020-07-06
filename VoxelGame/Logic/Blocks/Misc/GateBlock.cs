﻿// <copyright file="GateBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using VoxelGame.Logic.Interfaces;
using VoxelGame.Visuals;
using VoxelGame.Physics;
using VoxelGame.Utilities;
using OpenToolkit.Mathematics;
using VoxelGame.Entities;

namespace VoxelGame.Logic.Blocks
{
    /// <summary>
    /// A simple gate that can be used in fences and walls. It can be opened and closed.
    /// Data bit usage: <c>--coo</c>
    /// </summary>
    public class GateBlock : Block, IConnectable
    {
        private protected float[][] verticesClosed = new float[4][];
        private protected float[][] verticesOpen = new float[4][];

        private protected int[] texIndicesClosed = null!;
        private protected int[] texIndicesOpen = null!;

        private protected uint[] indicesClosed = null!;
        private protected uint[] indicesOpen = null!;

        private protected uint vertexCountClosed;
        private protected uint vertexCountOpen;

        private protected string closed, open;

        public GateBlock(string name, string closed, string open) :
        base(
            name,
            isFull: false,
            isOpaque: false,
            renderFaceAtNonOpaques: true,
            isSolid: true,
            recieveCollisions: false,
            isTrigger: false,
            isReplaceable: false,
            isInteractable: true,
            BoundingBox.Block,
            TargetBuffer.Complex)
        {
            this.closed = closed;
            this.open = open;
        }

        protected override void Setup()
        {
            BlockModel closed = BlockModel.Load(this.closed);
            BlockModel open = BlockModel.Load(this.open);

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    closed.ToData(out verticesClosed[i], out texIndicesClosed, out indicesClosed);
                    open.ToData(out verticesOpen[i], out texIndicesOpen, out indicesOpen);
                }
                else
                {
                    closed.RotateY(1, false);
                    closed.ToData(out verticesClosed[i], out _, out _);
                    open.RotateY(1, false);
                    open.ToData(out verticesOpen[i], out _, out _);
                }
            }

            vertexCountClosed = (uint)closed.VertexCount;
            vertexCountOpen = (uint)open.VertexCount;
        }

        protected override BoundingBox GetBoundingBox(int x, int y, int z, byte data)
        {
            bool isClosed = (data & 0b0_0100) == 0;
            float offst = 0.375f;

            switch ((Orientation)(data & 0b0_0011))
            {
                case Orientation.North:

                    if (isClosed)
                    {
                        return new BoundingBox(new Vector3(0.96875f, 0.71875f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f),
                        new BoundingBox(new Vector3(0.96875f, 0.28125f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f)),
                        new BoundingBox(new Vector3(0.03125f, 0.71875f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f)),
                        new BoundingBox(new Vector3(0.03125f, 0.28125f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f)),
                        // Moving parts.
                        new BoundingBox(new Vector3(0.75f, 0.71875f, 0.5f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)),
                        new BoundingBox(new Vector3(0.75f, 0.28125f, 0.5f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)),
                        new BoundingBox(new Vector3(0.25f, 0.71875f, 0.5f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)),
                        new BoundingBox(new Vector3(0.25f, 0.28125f, 0.5f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)));
                    }
                    else
                    {
                        return new BoundingBox(new Vector3(0.96875f, 0.71875f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f),
                        new BoundingBox(new Vector3(0.96875f, 0.28125f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f)),
                        new BoundingBox(new Vector3(0.03125f, 0.71875f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f)),
                        new BoundingBox(new Vector3(0.03125f, 0.28125f, 0.5f) + new Vector3(x, y, z), new Vector3(0.03125f, 0.15625f, 0.125f)),
                        // Moving parts.
                        new BoundingBox(new Vector3(0.875f, 0.71875f, offst) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)),
                        new BoundingBox(new Vector3(0.875f, 0.28125f, offst) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)),
                        new BoundingBox(new Vector3(0.125f, 0.71875f, offst) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)),
                        new BoundingBox(new Vector3(0.125f, 0.28125f, offst) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)));
                    }

                case Orientation.East:

                    offst = 0.625f;
                    goto case Orientation.West;

                case Orientation.South:

                    offst = 0.625f;
                    goto case Orientation.North;

                case Orientation.West:

                    if (isClosed)
                    {
                        return new BoundingBox(new Vector3(0.5f, 0.71875f, 0.96875f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f),
                        new BoundingBox(new Vector3(0.5f, 0.28125f, 0.96875f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f)),
                        new BoundingBox(new Vector3(0.5f, 0.71875f, 0.03125f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f)),
                        new BoundingBox(new Vector3(0.5f, 0.28125f, 0.03125f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f)),
                        // Moving parts.
                        new BoundingBox(new Vector3(0.5f, 0.71875f, 0.75f) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)),
                        new BoundingBox(new Vector3(0.5f, 0.28125f, 0.75f) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)),
                        new BoundingBox(new Vector3(0.5f, 0.71875f, 0.25f) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)),
                        new BoundingBox(new Vector3(0.5f, 0.28125f, 0.25f) + new Vector3(x, y, z), new Vector3(0.0625f, 0.09375f, 0.1875f)));
                    }
                    else
                    {
                        return new BoundingBox(new Vector3(0.5f, 0.71875f, 0.96875f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f),
                        new BoundingBox(new Vector3(0.5f, 0.28125f, 0.96875f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f)),
                        new BoundingBox(new Vector3(0.5f, 0.71875f, 0.03125f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f)),
                        new BoundingBox(new Vector3(0.5f, 0.28125f, 0.03125f) + new Vector3(x, y, z), new Vector3(0.125f, 0.15625f, 0.03125f)),
                        // Moving parts.
                        new BoundingBox(new Vector3(offst, 0.71875f, 0.875f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)),
                        new BoundingBox(new Vector3(offst, 0.28125f, 0.875f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)),
                        new BoundingBox(new Vector3(offst, 0.71875f, 0.125f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)),
                        new BoundingBox(new Vector3(offst, 0.28125f, 0.125f) + new Vector3(x, y, z), new Vector3(0.1875f, 0.09375f, 0.0625f)));
                    }

                default:
                    goto case Orientation.North;
            }
        }

        public override uint GetMesh(BlockSide side, byte data, out float[] vertices, out int[] textureIndices, out uint[] indices, out TintColor tint)
        {
            if ((data & 0b0_0100) == 0)
            {
                vertices = verticesClosed[data & 0b0_0011];
                textureIndices = texIndicesClosed;
                indices = indicesClosed;

                tint = TintColor.None;

                return vertexCountClosed;
            }
            else
            {
                vertices = verticesOpen[data & 0b0_0011];
                textureIndices = texIndicesOpen;
                indices = indicesOpen;

                tint = TintColor.None;

                return vertexCountOpen;
            }
        }

        protected override bool Place(int x, int y, int z, bool? replaceable, PhysicsEntity? entity)
        {
            if (replaceable != true)
            {
                return false;
            }

            Orientation orientation = entity?.LookingDirection.ToOrientation() ?? Orientation.North;

            bool connectX = (Game.World.GetBlock(x + 1, y, z, out _) is IConnectable east && east.IsConnetable(BlockSide.Left, x + 1, y, z)) || (Game.World.GetBlock(x - 1, y, z, out _) is IConnectable west && west.IsConnetable(BlockSide.Right, x - 1, y, z));
            bool connectZ = (Game.World.GetBlock(x, y, z + 1, out _) is IConnectable south && south.IsConnetable(BlockSide.Back, x, y, z + 1)) || (Game.World.GetBlock(x, y, z - 1, out _) is IConnectable north && north.IsConnetable(BlockSide.Front, x, y, z - 1));

            if ((orientation == Orientation.North || orientation == Orientation.South) && !connectX)
            {
                if (connectZ)
                {
                    orientation = orientation.Rotate();
                }
                else
                {
                    return false;
                }
            }
            else if ((orientation == Orientation.East || orientation == Orientation.West) && !connectZ)
            {
                if (connectX)
                {
                    orientation = orientation.Rotate();
                }
                else
                {
                    return false;
                }
            }

            Game.World.SetBlock(this, (byte)orientation, x, y, z);

            return true;
        }

        protected override void EntityInteract(PhysicsEntity entity, int x, int y, int z, byte data)
        {
            Orientation orientation = (Orientation)(data & 0b0_0011);
            bool isClosed = (data & 0b0_0100) == 0;

            // Check if orientation has to be inverted.
            if (isClosed && Vector2.Dot(orientation.ToVector().Xz, entity.Position.Xz - new Vector2(x + 0.5f, z + 0.5f)) < 0)
            {
                orientation = orientation.Invert();
            }

            Vector3 center = isClosed ? new Vector3(0.5f, 0.5f, 0.5f) + (orientation.ToVector() * 0.09375f) : new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 extents = (orientation == Orientation.North || orientation == Orientation.South) ? new Vector3(0.5f, 0.375f, 0.125f + (isClosed ? 0.09375f : 0f)) : new Vector3(0.125f + (isClosed ? 0.09375f : 0f), 0.375f, 0.5f);

            if (entity.BoundingBox.Intersects(new BoundingBox(center + new Vector3(x, y, z), extents)))
            {
                return;
            }

            Game.World.SetBlock(this, (byte)((isClosed ? 0b0_0100 : 0b0_0000) | (int)orientation.Invert()), x, y, z);
        }

        internal override void BlockUpdate(int x, int y, int z, byte data, BlockSide side)
        {
            Orientation orientation = (Orientation)(data & 0b0_0011);

            switch (side)
            {
                case BlockSide.Left:
                case BlockSide.Right:

                    if (orientation == Orientation.North || orientation == Orientation.South)
                    {
                        if (!((Game.World.GetBlock(x + 1, y, z, out _) is IConnectable east && east.IsConnetable(BlockSide.Left, x + 1, y, z)) || (Game.World.GetBlock(x - 1, y, z, out _) is IConnectable west && west.IsConnetable(BlockSide.Right, x - 1, y, z))))
                        {
                            Destroy(x, y, z, null);
                        }
                    }

                    break;

                case BlockSide.Front:
                case BlockSide.Back:

                    if (orientation == Orientation.East || orientation == Orientation.West)
                    {
                        if (!((Game.World.GetBlock(x, y, z + 1, out _) is IConnectable south && south.IsConnetable(BlockSide.Back, x, y, z + 1)) || (Game.World.GetBlock(x, y, z - 1, out _) is IConnectable north && north.IsConnetable(BlockSide.Front, x, y, z - 1))))
                        {
                            Destroy(x, y, z, null);
                        }
                    }

                    break;
            }
        }

        public bool IsConnetable(BlockSide side, int x, int y, int z)
        {
            if (Game.World.GetBlock(x, y, z, out byte data) == this)
            {
                Orientation orientation = (Orientation)(data & 0b0_0011);

                return orientation switch
                {
                    Orientation.North => side == BlockSide.Left || side == BlockSide.Right,
                    Orientation.East => side == BlockSide.Front || side == BlockSide.Back,
                    Orientation.South => side == BlockSide.Left || side == BlockSide.Right,
                    Orientation.West => side == BlockSide.Front || side == BlockSide.Back,
                    _ => false
                };
            }
            else
            {
                return false;
            }
        }
    }
}