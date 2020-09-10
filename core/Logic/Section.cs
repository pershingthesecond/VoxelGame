﻿// <copyright file="Section.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using OpenToolkit.Mathematics;
using System;
using System.Runtime.CompilerServices;

namespace VoxelGame.Core.Logic
{
    [Serializable]
    public abstract class Section : IDisposable
    {
        public const int SectionSize = 32;
        public const int TickBatchSize = 4;

        public const int DATASHIFT = 12;
        public const int LIQUIDSHIFT = 18;
        public const int LEVELSHIFT = 23;
        public const int STATICSHIFT = 26;

        public const uint BLOCKMASK = 0b0000_0000_0000_0000_0000_1111_1111_1111;
        public const uint DATAMASK = 0b0000_0000_0000_0011_1111_0000_0000_0000;
        public const uint LIQUIDMASK = 0b0000_0000_0111_1100_0000_0000_0000_0000;
        public const uint LEVELMASK = 0b0000_0011_1000_0000_0000_0000_0000_0000;
        public const uint STATICMASK = 0b0000_0100_0000_0000_0000_0000_0000_0000;

        public static Vector3 Extents { get => new Vector3(SectionSize / 2f, SectionSize / 2f, SectionSize / 2f); }

#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly uint[] blocks;
#pragma warning restore CA1051 // Do not declare visible instance fields

        protected Section()
        {
            blocks = new uint[SectionSize * SectionSize * SectionSize];

#pragma warning disable S1699 // Constructors should only call non-overridable methods
            Setup();
#pragma warning restore S1699 // Constructors should only call non-overridable methods
        }

        /// <summary>
        /// Sets up all non serialized members.
        /// </summary>
        public abstract void Setup();

        public void Tick(int sectionX, int sectionY, int sectionZ)
        {
            for (int i = 0; i < TickBatchSize; i++)
            {
                int index = Game.Random.Next(0, SectionSize * SectionSize * SectionSize);
                uint val = blocks[index];

                int z = index & 31;
                index = (index - z) >> 5;
                int y = index & 31;
                index = (index - y) >> 5;
                int x = index;

                Block.TranslateID(val & BLOCKMASK)?.RandomUpdate(x + (sectionX * SectionSize), y + (sectionY * SectionSize), z + (sectionZ * SectionSize), (val & DATAMASK) >> DATASHIFT);
            }
        }

        /// <summary>
        /// Gets or sets the block at a section position.
        /// </summary>
        /// <param name="x">The x position of the block in this section.</param>
        /// <param name="y">The y position of the block in this section.</param>
        /// <param name="z">The z position of the block in this section.</param>
        /// <returns>The block at the given position.</returns>
        public uint this[int x, int y, int z]
        {
            get
            {
                return blocks[(x << 10) + (y << 5) + z];
            }

            set
            {
                blocks[(x << 10) + (y << 5) + z] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Block GetBlock(int x, int y, int z)
        {
            return Block.TranslateID(this[x, y, z] & BLOCKMASK);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Liquid GetLiquid(int x, int y, int z, out int level)
        {
            uint val = this[x, y, z];

            level = (int)((val & LEVELMASK) >> LEVELSHIFT);
            return Liquid.TranslateID((val & LIQUIDMASK) >> LIQUIDSHIFT);
        }

        #region IDisposable Support

        protected abstract void Dispose(bool disposing);

        ~Section()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}