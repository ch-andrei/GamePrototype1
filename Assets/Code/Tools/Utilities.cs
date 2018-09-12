using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Unity;

namespace Code.Tools
{
    public static class Utilities
    {
        public static List<T> GetComponentsInHierarchy<T>(Transform transform)
        {
            var components = transform.GetComponents<T>().ToList();
            components.AddRange(transform.GetComponentsInChildren<T>());
            return components;
        }
    }

    // Nori Ray Tracer style Warp fuctions
    public static class WarpSampler
    {
        public enum EWarpType {
            EUniformSquare,
            EUniformDisk,
            EConcentricDisk,
            EUniformSphere,
            EUniformHemisphere,
            ECosineHemisphere, 
            EUniformCone,
        };
                
        public const float Epsilon = 1e-9f;
        
        public const float PI_OVER_4 = Mathf.PI / 4f;
        public const float PI_OVER_2 = Mathf.PI / 2f;
        
        public static float2 SquareToUniformSquare(float2 sample) {
            return sample;
        }

        public static float2 SquareToUniformDisk(float2 sample) {
            float r = Mathf.Sqrt(sample.x);
            float theta = 2f * Mathf.PI * sample.y;
            return new float2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        }

        public static float2 SquareToConcentricDisk(float2 sample) {
            float2 uo = 2f * sample - new float2(1, 1);

            if (Mathf.Abs(uo.x) < Epsilon && Mathf.Abs(uo.y) < Epsilon) {
                return new float2(Epsilon, Epsilon);
            }

            float theta, r;
            if (Mathf.Abs(uo.x) > Mathf.Abs(uo.y)) {
                r = uo.x;
                theta = PI_OVER_4 * uo.y/ uo.x;
            }
            else {
                r = uo.y;
                theta = PI_OVER_2 - PI_OVER_4 * uo.x / uo.y;
            }

            return r * new float2(Mathf.Cos(theta), Mathf.Sin(theta));
        }

        public static float3 SquareToUniformSphere(float2 sample) {
            float z = 1f - 2f * sample.x;
            float r = Mathf.Sqrt(Mathf.Max(Epsilon, 1f - z * z));
            float phi = 2f * Mathf.PI * sample.y;
            return new float3(r * Mathf.Cos(phi), r * Mathf.Sin(phi), z);
        }

        public static float3 SquareToUniformHemisphere(float2 sample) {
            float z = sample.x;
            float r = Mathf.Sqrt(Mathf.Max(Epsilon, 1f - z * z));
            float phi = 2f * Mathf.PI * sample.y;
            return new float3(r * Mathf.Cos(phi), r * Mathf.Sin(phi), z);
        }

        public static float3 SquareToCosineHemisphere(float2 sample) {
            float2 u = SquareToConcentricDisk(sample);
            float z = Mathf.Sqrt(
                Mathf.Max(
                    Epsilon,
                    1f - u.x * u.x - u.y * u.y
                )
            );
            return new float3 (u.x, u.y, z);
        }

        public static float3 SquareToUniformCone(float2 sample, float cosThetaMax) {
            float cosTheta = 1f - sample.x + sample.x * cosThetaMax;
            float sinTheta = Mathf.Sqrt(1f - cosTheta * cosTheta);
            float phi = sample.y * 2f * Mathf.PI;
            return new float3(Mathf.Cos(phi) * sinTheta, Mathf.Sin(phi) * sinTheta, cosTheta);
        }

        public static float3 Warp(EWarpType warpType, float2 sample, float param = 0)
        {
            float3 warped;
            switch (warpType) 
            {
                case EWarpType.EUniformSquare:
                {
                    float2 p = SquareToUniformSquare(sample);
                    warped = new float3(p.x, p.y, 0);
                    break;
                }

                case EWarpType.EUniformDisk:
                {
                    float2 p = SquareToUniformDisk(sample);
                    warped = new float3(p.x, p.y, 0);
                    break;
                }

                case EWarpType.EConcentricDisk:
                {
                    float2 p = SquareToConcentricDisk(sample);
                    warped = new float3(p.x, p.y, 0);
                    break;
                }
                case EWarpType.EUniformSphere:
                    warped = SquareToUniformSphere(sample);
                    break;
                case EWarpType.EUniformHemisphere:
                    warped = SquareToUniformHemisphere(sample);
                    break;
                case EWarpType.ECosineHemisphere:
                    warped = SquareToCosineHemisphere(sample);
                    break;
                case EWarpType.EUniformCone:
                    warped = SquareToUniformCone(sample, param);
                    break;
                default:
                    throw new Exception("Unknown warpType queried for Warp.");
                    break;
            }

            return warped;
        }

        public static float3 Warp(EWarpType warpType, float param = 0)
        {
            float2 sample = new float2(SampleUniform(), SampleUniform());
            return Warp(warpType, sample, param);
        }

        public static float SampleUniform()
        {
            return UnityEngine.Random.Range(0f, 1f);
        }
    }
}