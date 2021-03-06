﻿// <copyright file="Texture.cs" company="VoxelGame">
//     Code from https://github.com/opentk/LearnOpenTK
// </copyright>
// <author>pershingthesecond</author>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Extensions.Logging;
using OpenToolkit.Graphics.OpenGL4;
using VoxelGame.Logging;
using PixelFormat = OpenToolkit.Graphics.OpenGL4.PixelFormat;

namespace VoxelGame.Graphics.Objects
{
    public class Texture : IDisposable
    {
        private static readonly ILogger Logger = LoggingHelper.CreateLogger<Texture>();

        private int Handle { get; }

        public TextureUnit TextureUnit { get; private set; }

        public Texture(string path, TextureUnit unit, int fallbackResolution = 16)
        {
            TextureUnit = unit;

            GL.CreateTextures(TextureTarget.Texture2D, 1, out int handle);
            Handle = handle;

            Use(TextureUnit);

            try
            {
                using var bitmap = new Bitmap(path);
                SetupTexture(bitmap);
            }
            catch (Exception exception) when (exception is FileNotFoundException || exception is ArgumentException)
            {
                using (Bitmap bitmap = CreateFallback(fallbackResolution))
                {
                    SetupTexture(bitmap);
                }

                Logger.LogWarning(Events.MissingResource, exception, "The texture could not be loaded and a fallback was used instead because the file was not found: {path}", path);
            }

            GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.GenerateTextureMipmap(Handle);
        }

        private void SetupTexture(Bitmap bitmap)
        {
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TextureStorage2D(Handle, 1, SizedInternalFormat.Rgba8, bitmap.Width, bitmap.Height);
            GL.TextureSubImage2D(Handle, 0, 0, 0, bitmap.Width, bitmap.Height, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.BindTextureUnit(unit - TextureUnit.Texture0, Handle);
            TextureUnit = unit;
        }

        #region IDisposable Support

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    GL.DeleteTexture(Handle);
                }
                else
                {
                    Logger.LogWarning(Events.UndeletedTexture, "A texture has been disposed by GC, without deleting the texture storage.");
                }

                disposed = true;
            }
        }

        ~Texture()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        public static Bitmap CreateFallback(int resolution)
        {
            var fallback = new Bitmap(resolution, resolution, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Color magenta = Color.FromArgb(64, 255, 0, 255);
            Color black = Color.FromArgb(64, 0, 0, 0);

            for (var x = 0; x < fallback.Width; x++)
            {
                for (var y = 0; y < fallback.Height; y++)
                {
                    if (x % 2 == 0 ^ y % 2 == 0)
                    {
                        fallback.SetPixel(x, y, magenta);
                    }
                    else
                    {
                        fallback.SetPixel(x, y, black);
                    }
                }
            }

            return fallback;
        }
    }
}