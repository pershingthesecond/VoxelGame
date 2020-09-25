﻿// <copyright file="GLManager.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using Microsoft.Extensions.Logging;
using System;
using VoxelGame.Client.Rendering.Versions;
using VoxelGame.Core.Utilities;

namespace VoxelGame.Client.Rendering
{
    public class GLManager
    {
        private static ILogger logger = LoggingHelper.CreateLogger<GLManager>();

        public static void Initialize(int version)
        {
            if (version < 33)
            {
                logger.LogCritical("Versions below OpenGL 3.3 are not supported.");

                throw new NotSupportedException();
            }

            if (version == 33)
            {
                InitializeOpenGL33();
            }
            else
            {
                InitializeOpenGL46();
            }
        }

        public static Version Version { get; private set; } = null!;

        private static void InitializeOpenGL33()
        {
            Version = new Version(3, 3);

            ArrayTextureFactory = new Versions.OpenGL33.ArrayTextureFactory();
            BoxRendererFactory = new Versions.OpenGL33.BoxRendererFactory();
            ScreenFactory = new Versions.OpenGL33.ScreenFactory();
            ScreenElementRendererFactory = new Versions.OpenGL33.ScreenElementRendererFactory();
            SectionRendererFactory = new Versions.OpenGL33.SectionRendererFactory();
            TextureFactory = new Versions.OpenGL33.TextureFactory();

            logger.LogInformation("Initialized rendering for OpenGL 3.3");
        }

        private static void InitializeOpenGL46()
        {
            Version = new Version(4, 6);

            ArrayTextureFactory = new Versions.OpenGL46.ArrayTextureFactory();
            BoxRendererFactory = new Versions.OpenGL46.BoxRendererFactory();
            ScreenFactory = new Versions.OpenGL46.ScreenFactory();
            ScreenElementRendererFactory = new Versions.OpenGL46.ScreenElementRendererFactory();
            SectionRendererFactory = new Versions.OpenGL46.SectionRendererFactory();
            TextureFactory = new Versions.OpenGL46.TextureFactory();

            logger.LogInformation("Initialized rendering for OpenGL 4.6");
        }

        #region FACTORIES

        internal static ArrayTextureFactory ArrayTextureFactory { get; private set; } = null!;

        internal static BoxRendererFactory BoxRendererFactory { get; private set; } = null!;

        internal static ScreenFactory ScreenFactory { get; private set; } = null!;

        internal static ScreenElementRendererFactory ScreenElementRendererFactory { get; private set; } = null!;

        internal static SectionRendererFactory SectionRendererFactory { get; private set; } = null!;

        internal static TextureFactory TextureFactory { get; private set; } = null!;

        #endregion FACTORIES
    }
}