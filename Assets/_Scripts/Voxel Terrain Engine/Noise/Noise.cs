using UnityEngine;

namespace Suscraft.Core.VoxelTerrainEngine.PerlinNoise
{
    public static class Noise
    {
        public static float RemapValue(float value, float initialMin, float initialMax, float outputMin, float outputMax)
        {
            return outputMin + (value - initialMin) * (outputMax - outputMin) / (initialMax - initialMin);
        }

        public static float RemapValue01(float value, float outputMin, float outputMax)
        {
            return RemapValue(value, 0, 1, outputMin, outputMax);
        }

        public static int RemapValue01ToInt(float value, float outputMin, float outputMax)
        {
            return (int)RemapValue01(value, outputMin, outputMax);
        }

        public static float Redistribution(float noise, NoiseSettings settings)
        {
            return Mathf.Pow(noise * settings.redistributionModifier, settings.exponent);
        }

        public static float OctavePerlin(float x, float z, NoiseSettings settings)
        {
            x *= settings.noiseZoom;
            z *= settings.noiseZoom;
            x += settings.noiseZoom;
            z += settings.noiseZoom;

            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float amplitudeSum = 0;

            for (int i = 0; i < settings.octaves; ++i)
            {
                total += Mathf.PerlinNoise((settings.offset.x + settings.worldOffset.x + x) * frequency,
                    (settings.offset.y + settings.worldOffset.y + z) * frequency) * amplitude;

                amplitudeSum += amplitude;
                amplitude *= settings.persistance;
                frequency *= 2;
            }

            return total / amplitudeSum;
        }
    }
}