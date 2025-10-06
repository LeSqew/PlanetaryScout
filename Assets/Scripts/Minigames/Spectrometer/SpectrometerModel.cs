using System;
using System.Linq;
using UnityEngine;

namespace Minigames.Spectrometer
{
    public class SpectrometerModel
    {
        public float RedFilter { get; private set; }
        public float GreenFilter { get; private set; }
        public float BlueFilter { get; private set; }
        public float UVFilter { get; private set; }

        private readonly double[] _targetSpectrum;
        private readonly int _resolution = 256;

        public SpectrometerModel()
        {
            _targetSpectrum = GenerateRandomTarget(_resolution);
        }

        public void SetFilters(float redFilter, float greenFilter, float blueFilter, float uvFilter = 0f)
        {
            RedFilter = Mathf.Clamp01(redFilter);
            GreenFilter = Mathf.Clamp01(greenFilter);
            BlueFilter = Mathf.Clamp01(blueFilter);
            UVFilter = Mathf.Clamp01(uvFilter);
        }

        public double[] GetTargetSpectrum()
        {
            var copy = new double[_targetSpectrum.Length];
            Array.Copy(_targetSpectrum, copy, _targetSpectrum.Length);
            return copy;
        }

        public double[] GetMeasuredSpectrum()
        {
            var result = new double[_resolution];
            AddGaussian(result, 0.25, 0.08, RedFilter);
            AddGaussian(result, 0.5, 0.10, GreenFilter);
            AddGaussian(result, 0.78, 0.06, BlueFilter);
            if (UVFilter > .01f)
                AddGaussian(result, 0.08, 0.04, UVFilter);
            Normalize(result);
            return result;
            
        }

        public float CalculateAccuracy()
        {
            var measured = GetMeasuredSpectrum();
            double diffSum = 0;
            for (int i = 0; i < _resolution; i++)
                diffSum += Math.Abs(measured[i] - _targetSpectrum[i]);
            double meanDiff = diffSum / _resolution;
            return (float)Math.Max(0, 1.0 - meanDiff);
        }

        public bool TryGetResult(out string result)
        {
            float accuracy = CalculateAccuracy();
            if (accuracy >= 0.95f)
            {
                result = $"Fe: {Mathf.RoundToInt(60 + RedFilter * 30)}%, " +
                         $"Si: {Mathf.RoundToInt(20 + GreenFilter * 20)}%, " +
                         $"Xr: {Mathf.RoundToInt(5 + BlueFilter * 10)}%";
                return true;
            }
            result = string.Empty;
            return false;
        }

        private void Normalize(double[] arr)
        {
            var max = arr.Prepend(0).Max();
            if (max > 0) for (var i = 0; i < arr.Length; i++) arr[i] /= max;
        }

        private void AddGaussian(double[] arr, double center, double width, double amplitude)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                double x = (double)i / (arr.Length - 1);
                arr[i] += amplitude * Math.Exp(-Math.Pow((x - center)/width, 2) / 2.0);
            }
        }

        private double[] GenerateRandomTarget(int n)
        {
            var data = new double[n];
            AddGaussian(data, 0.25, 0.08, 1.0);
            AddGaussian(data, 0.5, 0.12, 0.8);
            AddGaussian(data, 0.78, 0.06, 0.6);
            Normalize(data);
            return data;
        }
    }
}
