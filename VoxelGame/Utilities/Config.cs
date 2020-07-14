﻿// <copyright file="Configuration.cs" company="VoxelGame">
//     MIT License
//	   For full license see the repository.
// </copyright>
// <author>pershingthesecond</author>
using OpenToolkit.Mathematics;
using System;
using System.Configuration;

namespace VoxelGame.Utilities
{
    /// <summary>
    /// Helper class to simplify configuration value retrieval.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Retrieves a <see cref="float"/> value from the configuration file.
        /// </summary>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <param name="fallback">The fallback to use in case of failed retrieval.</param>
        /// <returns>The retrieved value.</returns>
        public static float GetFloat(string key, float fallback = default, float min = float.NegativeInfinity, float max = float.PositiveInfinity)
        {
            return Math.Clamp(float.TryParse(ConfigurationManager.AppSettings[key], out float f) ? f : fallback, min, max);
        }

        /// <summary>
        /// Retrieves an <see cref="int"/> value from the configuration file.
        /// </summary>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <param name="fallback">The fallback to use in case of failed retrieval.</param>
        /// <returns>The retrieved value.</returns>
        public static int GetInt(string key, int fallback = default, int min = int.MinValue, int max = int.MaxValue)
        {
            return Math.Clamp(int.TryParse(ConfigurationManager.AppSettings[key], out int i) ? i : fallback, min, max);
        }

        /// <summary>
        /// Retrieves a <see cref="Vector3"/> value from the configuration file.
        /// </summary>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <param name="fallback">The fallback to use in case of failed retrieval.</param>
        /// <returns>The retrieved value.</returns>
        public static Vector3 GetVector3(string key, Vector3 fallback = default)
        {
            string[] r = ConfigurationManager.AppSettings[key]?.Split(';') ?? Array.Empty<string>();

            return r.Length == 3 && float.TryParse(r[0], out float x) && float.TryParse(r[1], out float y) && float.TryParse(r[2], out float z) ? new Vector3(x, y, z) : fallback;
        }
    }
}