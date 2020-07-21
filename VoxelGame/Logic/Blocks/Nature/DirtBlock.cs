﻿// <copyright file="DirtBlock.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using VoxelGame.Logic.Interfaces;

namespace VoxelGame.Logic.Blocks
{
    /// <summary>
    /// A simple block which allows the spread of grass
    /// </summary>
    public class DirtBlock : BasicBlock, IPlantable
    {
        public DirtBlock(string name, string namedId, TextureLayout layout) :
            base(
                name,
                namedId,
                layout,
                isOpaque: true,
                renderFaceAtNonOpaques: true,
                isSolid: true,
                isInteractable: false)
        {
        }

        internal override void RandomUpdate(int x, int y, int z, byte data)
        {
            // Check if this block can be grass
            if ((Game.World.GetBlock(x, y + 1, z, out _) ?? Block.AIR).IsSolidAndFull)
            {
                return;
            }

            // Check surrounding blocks for grass
            // Same height:
            if (Game.World.GetBlock(x + 1, y, z, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x - 1, y, z, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x, y, z + 1, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x, y, z - 1, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            // One block up:
            else if (Game.World.GetBlock(x + 1, y + 1, z, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x - 1, y + 1, z, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x, y + 1, z + 1, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x, y + 1, z - 1, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            // One block down:
            else if (Game.World.GetBlock(x + 1, y - 1, z, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x - 1, y - 1, z, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x, y - 1, z + 1, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
            else if (Game.World.GetBlock(x, y - 1, z - 1, out _) == Block.GRASS)
            {
                Game.World.SetBlock(Block.GRASS, 0, x, y, z);
            }
        }
    }
}