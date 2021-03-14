/*
 * Noise generation based on fastnoise lite by Jordan Peck (jordan.me2@gmail.com).
 * This part written by Dante Cavallin (dantecavallin@gmail.com).
 * Distribution made according to MIT LICENSE
 * I'm lazy. Look it up.
 */
using System;
using Unity.Mathematics;

namespace GenerationTools
{
    

    [Serializable]
    public struct NoiseData
    {

        /// Results are given as 1-result within range of 0.0 ... 1.0
        public bool invert;
        
        /// Results are generated with z additional z axis.
        public bool generate3D;
    
        /// Sets seed for all noise types
        public int seed;
        
        /// Sets frequency for all noise types.
        public float frequency;
    
        /// What noise algorithm to use.
        public FastNoiseLite.NoiseType noiseType;
        
        /// Sets domain rotation type for 3D Noise and 3D DomainWarp.
        /// Can aid in reducing directional artifacts when sampling a 2D plane in 3D
        public FastNoiseLite.RotationType3D rotationType3D;

        /// Sets method for combining octaves in all fractal noise types
        public FastNoiseLite.FractalType fractalType;

        /// Sets octave count for all fractal noise types 
        public int octaves;

        /// Sets octave lacunarity for all fractal noise types
        public float lacunarity;

        /// Sets octave gain for all fractal noise types
        public float gain;

        /// Sets octave weighting for all none DomainWarp fratal types
        public float weightedStrength;

        /// Sets strength of the fractal ping pong effect
        public float pingPongStrength;

        /// Sets distance function used in cellular noise calculations
        public FastNoiseLite.CellularDistanceFunction distanceFunction;

        /// Sets return type from cellular noise calculations
        public FastNoiseLite.CellularReturnType returnType;

        /// Sets the maximum distance a cellular point can move from it's grid position
        public float jitter;

        /// Sets the warp algorithm when using DomainWarp(...)
        public FastNoiseLite.DomainWarpType warpType;

        /// Sets domain rotation type for 3D Noise and 3D DomainWarp.
        /// Can aid in reducing directional artifacts when sampling a 2D plane in 3D
        /// Specifically for domain noise
        public FastNoiseLite.RotationType3D domainRotationType3D;

        /// Sets the maximum warp distance from original position when using DomainWarp(...)
        public float domainAmplitude;

        /// Sets frequency for all noise types. Results in 1 / frequency opposed to how its made traditionally.
        /// Specifically for domain noise
        public float domainFrequency;
        
        /// Sets method for combining octaves in all fractal noise types
        /// Specifically for domain noise
        public FastNoiseLite.FractalType domainFractalType;
        
        /// Sets octave count for all fractal noise types 
        /// Specifically for domain noise
        public int domainOctaves;
        
        /// Sets octave lacunarity for all fractal noise types
        /// Specifically for domain noise
        public float domainLacunarity;
        
        /// Sets octave gain for all fractal noise types
        /// Specifically for domain noise
        public float domainGain;
        
        private FastNoiseLite _fractalNoise;
        private FastNoiseLite _domainNoise;

        public NoiseData(int seed = 1337)
        {
            invert = false;
            generate3D = false;
            this.seed = seed;
            frequency = 100;
            noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
            rotationType3D = FastNoiseLite.RotationType3D.None;
            fractalType = FastNoiseLite.FractalType.None;
            octaves = 3;
            lacunarity = 2;
            gain = .5f;
            weightedStrength = 0;
            pingPongStrength = 2;
            distanceFunction = FastNoiseLite.CellularDistanceFunction.EuclideanSq;
            returnType = FastNoiseLite.CellularReturnType.Distance;
            jitter = 1;
            warpType = FastNoiseLite.DomainWarpType.OpenSimplex2;
            domainAmplitude = 1;

            domainRotationType3D = FastNoiseLite.RotationType3D.None;
            domainFrequency = 3;
            domainFractalType = FastNoiseLite.FractalType.DomainWarpProgressive;
            domainOctaves = 8;
            domainLacunarity = 3;
            domainGain = .6f;
            _fractalNoise = new FastNoiseLite();
            _domainNoise = new FastNoiseLite();
        }
        
        
        /// <summary>
        /// Applies any changes made to the noise settings.
        /// Required to get any updates to the noise
        /// </summary>
        public readonly void Apply()
        {

            _fractalNoise.SetNoiseType(noiseType);
            _fractalNoise.SetRotationType3D(rotationType3D);
            _fractalNoise.SetFrequency(frequency);
            _fractalNoise.SetFractalGain(gain);
            _fractalNoise.SetFractalLacunarity(lacunarity);
            _fractalNoise.SetFractalOctaves(octaves);
            _fractalNoise.SetFractalType(fractalType);
            _fractalNoise.SetFractalWeightedStrength(weightedStrength);
            _fractalNoise.SetFractalPingPongStrength(pingPongStrength);

            _fractalNoise.SetCellularDistanceFunction(distanceFunction);
            _fractalNoise.SetCellularReturnType(returnType);
            _fractalNoise.SetCellularJitter(jitter);

            _fractalNoise.SetDomainWarpAmp(domainAmplitude);
            _fractalNoise.SetDomainWarpType(warpType);

            _domainNoise.SetNoiseType(noiseType);
            _domainNoise.SetRotationType3D(domainRotationType3D);
            _domainNoise.SetFrequency(domainFrequency);
            _domainNoise.SetFractalGain(domainGain);
            _domainNoise.SetFractalLacunarity(domainLacunarity);
            _domainNoise.SetFractalOctaves(domainOctaves);
            _domainNoise.SetFractalType(domainFractalType);
            _domainNoise.SetFractalWeightedStrength(weightedStrength);
            _domainNoise.SetFractalPingPongStrength(pingPongStrength);

            _domainNoise.SetCellularDistanceFunction(distanceFunction);
            _domainNoise.SetCellularReturnType(returnType);
            _domainNoise.SetCellularJitter(jitter);

            _domainNoise.SetDomainWarpAmp(domainAmplitude);
            _domainNoise.SetDomainWarpType(warpType);
        }
        
        /// <summary>
        /// Calls selected fractal noise algorithm
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns>
        /// The fractal noise height value from selected fractal noise algorithm algorithm.
        /// </returns>
        public readonly float GetNoiseValue(float x, float z, float y = 0)
        {

            float ret = generate3D ? 
                _fractalNoise.GetNoise(x, z, y) : 
                _fractalNoise.GetNoise(x, z);
            

            if (invert)
                ret = 1 - ret;
            
            return ret;
        }
        
        public float GetNoiseValue(float3 xyz)
        { return GetNoiseValue(xyz.x, xyz.z, xyz.y); }
        
        /// <summary>
        /// warps vector according to domain noise.
        /// </summary>
        /// <param name="pos"></param>
        public void WarpDomain(ref float3 pos)
        {
            if(generate3D)
                _domainNoise.DomainWarp(ref pos.x, ref pos.z, ref pos.y);
            else
                _domainNoise.DomainWarp(ref pos.x, ref pos.z);
        }

        /// <summary>
        /// warps vector according to domain noise and adds fractal noise on the y axis.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public void GetWarpDomainValue(ref float3 pos)
        {
            var posXYZ = pos.xyz;
            WarpDomain(ref posXYZ);
            pos.y = GetNoiseValue(posXYZ);
            if (invert)
                pos.y = 1 - pos.y;
        }
        
        public FastNoiseLite GetFractalNoise()
        {
            return _fractalNoise;
        }
        
        public FastNoiseLite GetDomainNoise()
        {
            return _domainNoise;
        }

    }
}