﻿// <copyright file="TextureLayout.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>

namespace VoxelGame.Logic
{
    /// <summary>
    /// Provides functionality to define the textures of a default six-sided block.
    /// </summary>
    public struct TextureLayout
    {
        public int Front { get; }
        public int Back { get; }
        public int Left { get; }
        public int Right { get; }
        public int Bottom { get; }
        public int Top { get; }

        public TextureLayout(int front, int back, int left, int right, int bottom, int top)
        {
            Front = front;
            Back = back;
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }

        /// <summary>
        /// Returns a texture layout where every side has the same texture.
        /// </summary>
        public static TextureLayout Uniform(string texture)
        {
            int i = Game.BlockTextureArray.GetTextureIndex(texture);

            return new TextureLayout(i, i, i, i, i, i);
        }

        /// <summary>
        /// Returns a texture layout where every side has a different texture.
        /// </summary>
        public static TextureLayout Unique(string front, string back, string left, string right, string bottom, string top)
        {
            return new TextureLayout(
                front: Game.BlockTextureArray.GetTextureIndex(front),
                back: Game.BlockTextureArray.GetTextureIndex(back),
                left: Game.BlockTextureArray.GetTextureIndex(left),
                right: Game.BlockTextureArray.GetTextureIndex(right),
                bottom: Game.BlockTextureArray.GetTextureIndex(bottom),
                top: Game.BlockTextureArray.GetTextureIndex(top));
        }

        /// <summary>
        /// Returns a texture layout where every side has a different texture and the textures are specified as offset from one single texture.
        /// </summary>
        public static TextureLayout Unique(string texture, int front, int back, int left, int right, int bottom, int top)
        {
            int i = Game.BlockTextureArray.GetTextureIndex(texture);

            return new TextureLayout(i + front, i + back, i + left, i + right, i + bottom, i + top);
        }

        /// <summary>
        /// Returns a texture layout where two textures are used, one for top/bottom, the other for the sides around it.
        /// </summary>
        public static TextureLayout Column(string sides, string ends)
        {
            int sideIndex = Game.BlockTextureArray.GetTextureIndex(sides);
            int endIndex = Game.BlockTextureArray.GetTextureIndex(ends);

            return new TextureLayout(sideIndex, sideIndex, sideIndex, sideIndex, endIndex, endIndex);
        }

        /// <summary>
        /// Returns a texture layout where two textures are used, one for top/bottom, the other for the sides around it.
        /// </summary>
        public static TextureLayout Column(string texture, int sideOffset, int endOffset)
        {
            int sideIndex = Game.BlockTextureArray.GetTextureIndex(texture) + sideOffset;
            int endIndex = Game.BlockTextureArray.GetTextureIndex(texture) + endOffset;

            return new TextureLayout(sideIndex, sideIndex, sideIndex, sideIndex, endIndex, endIndex);
        }

        /// <summary>
        /// Returns a texture layout where three textures are used, one for top, one for bottom, the other for the sides around it.
        /// </summary>
        public static TextureLayout UnqiueColumn(string sides, string bottom, string top)
        {
            int sideIndex = Game.BlockTextureArray.GetTextureIndex(sides);
            int bottomIndex = Game.BlockTextureArray.GetTextureIndex(bottom);
            int topIndex = Game.BlockTextureArray.GetTextureIndex(top);

            return new TextureLayout(sideIndex, sideIndex, sideIndex, sideIndex, bottomIndex, topIndex);
        }

        /// <summary>
        /// Returns a texture layout where three textures are used, one for top, one for bottom, the other for the sides around it.
        /// </summary>
        public static TextureLayout UnqiueColumn(string texture, int sideOffset, int bottomOffset, int topOffset)
        {
            int sideIndex = Game.BlockTextureArray.GetTextureIndex(texture) + sideOffset;
            int bottomIndex = Game.BlockTextureArray.GetTextureIndex(texture) + bottomOffset;
            int topIndex = Game.BlockTextureArray.GetTextureIndex(texture) + topOffset;

            return new TextureLayout(sideIndex, sideIndex, sideIndex, sideIndex, bottomIndex, topIndex);
        }

        /// <summary>
        /// Returns a texture layout where all sides but the front have the same texture.
        /// </summary>
        public static TextureLayout UnqiueFront(string front, string rest)
        {
            int frontIndex = Game.BlockTextureArray.GetTextureIndex(front);
            int restIndex = Game.BlockTextureArray.GetTextureIndex(rest);

            return new TextureLayout(frontIndex, restIndex, restIndex, restIndex, restIndex, restIndex);
        }

        /// <summary>
        /// Returns a texture layout where all sides but the front have the same texture.
        /// </summary>
        public static TextureLayout UnqiueFront(string texture, int frontOffset, int restOffset)
        {
            int frontIndex = Game.BlockTextureArray.GetTextureIndex(texture) + frontOffset;
            int restIndex = Game.BlockTextureArray.GetTextureIndex(texture) + restOffset;

            return new TextureLayout(frontIndex, restIndex, restIndex, restIndex, restIndex, restIndex);
        }

        /// <summary>
        /// Returns a texture layout where all sides but the top side have the same texture.
        /// </summary>
        public static TextureLayout UnqiueTop(string rest, string top)
        {
            int topIndex = Game.BlockTextureArray.GetTextureIndex(top);
            int restIndex = Game.BlockTextureArray.GetTextureIndex(rest);

            return new TextureLayout(restIndex, restIndex, restIndex, restIndex, restIndex, topIndex);
        }
    }
}