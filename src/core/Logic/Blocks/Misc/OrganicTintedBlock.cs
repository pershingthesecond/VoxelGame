﻿// <copyright file="OrganicTintedBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

using VoxelGame.Core.Logic.Interfaces;

namespace VoxelGame.Core.Logic.Blocks
{
    /// <summary>
    /// A <see cref="TintedBlock"/> made out of organic, flammable materials.
    /// Data bit usage: <c>--cccc</c>
    /// </summary>
    public class OrganicTintedBlock : TintedBlock, IFlammable
    {
        internal OrganicTintedBlock(string name, string namedId, TextureLayout layout) :
            base(
                name,
                namedId,
                layout)
        {
        }
    }
}