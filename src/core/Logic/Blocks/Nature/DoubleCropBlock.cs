﻿// <copyright file="DoubeCropBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

using VoxelGame.Core.Entities;
using VoxelGame.Core.Logic.Interfaces;
using VoxelGame.Core.Physics;
using VoxelGame.Core.Visuals;

namespace VoxelGame.Core.Logic.Blocks
{
    /// <summary>
    /// A block which grows on farmland and has multiple growth stages, of which some are two blocks tall.
    /// Data bit usage: <c>--hsss</c>
    /// </summary>
    // s = stage
    // h = height
    public class DoubleCropBlock : Block, IFlammable, IFillable
    {
        private float[] vertices = null!;

        private int[] stageTexIndicesLow = null!;
        private int[] stageTexIndicesTop = null!;

        private uint[] indices = null!;

        private readonly string texture;
        private int dead, first, second, third;
        private (int low, int top) fourth, fifth, sixth, final;

        internal DoubleCropBlock(string name, string namedId, string texture, int dead, int first, int second, int third, (int low, int top) fourth, (int low, int top) fifth, (int low, int top) sixth, (int low, int top) final) :
            base(
                name,
                namedId,
                isFull: false,
                isOpaque: false,
                renderFaceAtNonOpaques: true,
                isSolid: false,
                receiveCollisions: false,
                isTrigger: false,
                isReplaceable: false,
                isInteractable: false,
                BoundingBox.Block,
                TargetBuffer.Complex)
        {
            this.texture = texture;

            this.dead = dead;
            this.first = first;
            this.second = second;
            this.third = third;

            this.fourth = fourth;
            this.fifth = fifth;
            this.sixth = sixth;
            this.final = final;
        }

        protected override void Setup(ITextureIndexProvider indexProvider)
        {
            vertices = new[]
            {
                //X----Y---Z---U---V---N---O---P
                0.25f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0.25f, 1f, 0f, 0f, 1f, 0f, 0f, 0f,
                0.25f, 1f, 1f, 1f, 1f, 0f, 0f, 0f,
                0.25f, 0f, 1f, 1f, 0f, 0f, 0f, 0f,

                0.75f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
                0.75f, 1f, 0f, 0f, 1f, 0f, 0f, 0f,
                0.75f, 1f, 1f, 1f, 1f, 0f, 0f, 0f,
                0.75f, 0f, 1f, 1f, 0f, 0f, 0f, 0f,

                0f, 0f, 0.25f, 0f, 0f, 0f, 0f, 0f,
                0f, 1f, 0.25f, 0f, 1f, 0f, 0f, 0f,
                1f, 1f, 0.25f, 1f, 1f, 0f, 0f, 0f,
                1f, 0f, 0.25f, 1f, 0f, 0f, 0f, 0f,

                0f, 0f, 0.75f, 0f, 0f, 0f, 0f, 0f,
                0f, 1f, 0.75f, 0f, 1f, 0f, 0f, 0f,
                1f, 1f, 0.75f, 1f, 1f, 0f, 0f, 0f,
                1f, 0f, 0.75f, 1f, 0f, 0f, 0f, 0f
            };

            int baseIndex = indexProvider.GetTextureIndex(texture);

            if (baseIndex == 0)
            {
                dead = first = second = third = 0;
                fourth = fifth = sixth = final = (0, 0);
            }

            stageTexIndicesLow = new[]
            {
                baseIndex + dead,
                baseIndex + first,
                baseIndex + second,
                baseIndex + third,
                baseIndex + fourth.low,
                baseIndex + fifth.low,
                baseIndex + sixth.low,
                baseIndex + final.low,
            };

            stageTexIndicesTop = new[]
            {
                0,
                0,
                0,
                0,
                baseIndex + fourth.top,
                baseIndex + fifth.top,
                baseIndex + sixth.top,
                baseIndex + final.top,
            };

            indices = new uint[]
            {
                0, 2, 1,
                0, 3, 2,
                0, 1, 2,
                0, 2, 3,

                4, 6, 5,
                4, 7, 6,
                4, 5, 6,
                4, 6, 7,

                8, 10, 9,
                8, 11, 10,
                8, 9, 10,
                8, 10, 11,

                12, 14, 13,
                12, 15, 14,
                12, 13, 14,
                12, 14, 15
            };
        }

        protected override BoundingBox GetBoundingBox(uint data)
        {
            var stage = (GrowthStage)(data & 0b00_0111);

            if (((data & 0b00_1000) == 0 && stage == GrowthStage.Initial) ||
                ((data & 0b00_1000) != 0 && (stage == GrowthStage.Fourth || stage == GrowthStage.Fifth)))
            {
                return BoundingBox.BlockWithHeight(7);
            }
            else
            {
                return BoundingBox.BlockWithHeight(15);
            }
        }

        public override BlockMeshData GetMesh(BlockMeshInfo info)
        {
            int[] textureIndices = new int[24];

            var tex = (int)(info.Data & 0b00_0111);

            if ((info.Data & 0b00_1000) == 0)
            {
                for (var i = 0; i < 24; i++)
                {
                    textureIndices[i] = stageTexIndicesLow[tex];
                }
            }
            else
            {
                for (var i = 0; i < 24; i++)
                {
                    textureIndices[i] = stageTexIndicesTop[tex];
                }
            }

            return new BlockMeshData(16, vertices, textureIndices, indices);
        }

        internal override bool CanPlace(World world, int x, int y, int z, PhysicsEntity? entity)
        {
            return world.GetBlock(x, y - 1, z, out _) is IPlantable;
        }

        protected override void DoPlace(World world, int x, int y, int z, PhysicsEntity? entity)
        {
            world.SetBlock(this, (uint)GrowthStage.Initial, x, y, z);
        }

        internal override void DoDestroy(World world, int x, int y, int z, uint data, PhysicsEntity? entity)
        {
            world.SetDefaultBlock(x, y, z);

            if ((data & 0b00_0111) >= (int)GrowthStage.Fourth)
            {
                world.SetDefaultBlock(x, y + ((data & 0b00_1000) == 0 ? 1 : -1), z);
            }
        }

        internal override void BlockUpdate(World world, int x, int y, int z, uint data, BlockSide side)
        {
            // Check if this block is the lower part and if the ground supports plant growth.
            if (side == BlockSide.Bottom && (data & 0b00_1000) == 0 && !((world.GetBlock(x, y - 1, z, out _) ?? Block.Air) is IPlantable))
            {
                Destroy(world, x, y, z);
            }
        }

        internal override void RandomUpdate(World world, int x, int y, int z, uint data)
        {
            var stage = (GrowthStage)(data & 0b00_0111);

            // If this block is the upper part, the random update is ignored.
            if ((data & 0b00_1000) != 0) return;

            if (world.GetBlock(x, y - 1, z, out _) is IPlantable plantable)
            {
                if ((int)stage > 2 && !plantable.SupportsFullGrowth) return;

                if (stage != GrowthStage.Final && stage != GrowthStage.Dead)
                {
                    if (stage >= GrowthStage.Third)
                    {
                        Block? above = world.GetBlock(x, y + 1, z, out _);

                        if (plantable.TryGrow(world, x, y - 1, z, Liquid.Water, LiquidLevel.One) && ((above?.IsReplaceable ?? false) || above == this))
                        {
                            world.SetBlock(this, (uint)(stage + 1), x, y, z);
                            world.SetBlock(this, (uint)(0b00_1000 | (int)stage + 1), x, y + 1, z);
                        }
                        else
                        {
                            world.SetBlock(this, (uint)GrowthStage.Dead, x, y, z);
                            if (stage != GrowthStage.Third) world.SetDefaultBlock(x, y + 1, z);
                        }
                    }
                    else
                    {
                        world.SetBlock(this, (uint)(stage + 1), x, y, z);
                    }
                }
            }
        }

        public void LiquidChange(World world, int x, int y, int z, Liquid liquid, LiquidLevel level)
        {
            if (liquid.Direction > 0 && level > LiquidLevel.Four) ScheduleDestroy(world, x, y, z);
        }

        private enum GrowthStage
        {
            /// <summary>
            /// One Block tall.
            /// </summary>
            Dead,

            /// <summary>
            /// One Block tall.
            /// </summary>
            Initial,

            /// <summary>
            /// One Block tall.
            /// </summary>
            Second,

            /// <summary>
            /// One Block tall.
            /// </summary>
            Third,

            /// <summary>
            /// Two blocks tall.
            /// </summary>
            Fourth,

            /// <summary>
            /// Two blocks tall.
            /// </summary>
            Fifth,

            /// <summary>
            /// Two blocks tall.
            /// </summary>
            Sixth,

            /// <summary>
            /// Two blocks tall.
            /// </summary>
            Final,
        }
    }
}