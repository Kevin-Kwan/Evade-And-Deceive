//
// Klak - Utilities for creative coding with Unity
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using Klak.Math;

namespace Klak.Wiring
{
    [AddComponentMenu("Klak/Wiring/Transform Map")]
    public class TransformMap : MonoBehaviour
    {
        #region Translation Settings

        public enum TranslationMode {
            Off, XAxis, YAxis, ZAxis, Vector, Random
        };

        [SerializeField]
        TranslationMode _translationMode = TranslationMode.Off;

        [SerializeField]
        Vector3 _translationVector = Vector3.forward;

        [SerializeField]
        float _translationAmount0 = 0.0f;

        [SerializeField]
        float _translationAmount1 = 10.0f;

        public TranslationMode translationMode {
            get { return _translationMode; }
            set { _translationMode = value; }
        }

        public Vector3 translationVector {
            get { return _translationVector; }
            set { _translationVector = value; }
        }

        public float translationAmount0 {
            get { return _translationAmount0; }
            set { _translationAmount0 = value; }
        }

        public float translationAmount1 {
            get { return _translationAmount1; }
            set { _translationAmount1 = value; }
        }

        #endregion

        #region Rotation Settings

        public enum RotationMode {
            Off, XAxis, YAxis, ZAxis, Vector, Random
        }

        [SerializeField]
        RotationMode _rotationMode = RotationMode.Off;

        [SerializeField]
        Vector3 _rotationAxis = Vector3.up;

        [SerializeField]
        float _rotationAngle0 = 0.0f;

        [SerializeField]
        float _rotationAngle1 = 90.0f;

        public RotationMode rotationMode {
            get { return _rotationMode; }
            set { _rotationMode = value; }
        }

        public Vector3 rotationAxis {
            get { return _rotationAxis; }
            set { _rotationAxis = value; }
        }

        public float rotationAngle0 {
            get { return _rotationAngle0; }
            set { _rotationAngle0 = value; }
        }

        public float rotationAngle1 {
            get { return _rotationAngle1; }
            set { _rotationAngle1 = value; }
        }

        #endregion

        #region Scale Settings

        public enum ScaleMode {
            Off, Uniform, Vector, Random
        }

        [SerializeField]
        ScaleMode _scaleMode = ScaleMode.Off;

        [SerializeField]
        Vector3 _scaleVector = Vector3.one;

        [SerializeField]
        float _scaleAmount0 = 0.0f;

        [SerializeField]
        float _scaleAmount1 = 1.0f;

        public ScaleMode scaleMode {
            get { return _scaleMode; }
            set { _scaleMode = value; }
        }

        public Vector3 scaleVector {
            get { return _scaleVector; }
            set { _scaleVector = value; }
        }

        public float scaleAmount0 {
            get { return _scaleAmount0; }
            set { _scaleAmount0 = value; }
        }

        public float scaleAmount1 {
            get { return _scaleAmount1; }
            set { _scaleAmount1 = value; }
        }

        #endregion

        #region Miscellaneous Settings

        [SerializeField]
        Transform _targetTransform;

        [SerializeField]
        bool _addToOriginal = true;

        public Transform targetTransform {
            get { return _targetTransform; }
            set {
                if (_targetTransform != null) OnDisable();
                _targetTransform = value;
                if (_targetTransform != null) OnEnable();
            }
        }

        public bool addToOriginal {
            get { return _addToOriginal; }
            set { _addToOriginal = value; }
        }

        #endregion

        #region Public Properties

        public float inputValue {
            set {
                if (enabled && _targetTransform != null)
                {
                    if (_translationMode != TranslationMode.Off)
                        UpdatePosition(value);

                    if (_rotationMode != RotationMode.Off)
                        UpdateRotation(value);

                    if (_scaleMode != ScaleMode.Off)
                        UpdateScale(value);
                }
            }
        }

        #endregion

        #region Private Variables And Properties

        Vector3 _originalPosition;
        Quaternion _originalRotation;
        Vector3 _originalScale;
        Vector3 _randomVectorT;
        Vector3 _randomVectorR;
        Vector3 _randomVectorS;

        Vector3 TranslationVector {
            get {
                switch (_translationMode)
                {
                    case TranslationMode.XAxis:  return Vector3.right;
                    case TranslationMode.YAxis:  return Vector3.up;
                    case TranslationMode.ZAxis:  return Vector3.forward;
                    case TranslationMode.Vector: return _translationVector;
                }
                // TranslationMode.Random
                return _randomVectorT;
            }
        }

        Vector3 RotationAxis {
            get {
                switch (_rotationMode)
                {
                    case RotationMode.XAxis:  return Vector3.right;
                    case RotationMode.YAxis:  return Vector3.up;
                    case RotationMode.ZAxis:  return Vector3.forward;
                    case RotationMode.Vector: return _rotationAxis;
                }
                // RotationMode.Random
                return _randomVectorR;
            }
        }

        Vector3 ScaleVector {
            get {
                if (_scaleMode == ScaleMode.Uniform)
                    return Vector3.one;
                else if (_scaleMode == ScaleMode.Vector)
                    return _scaleVector;
                else // ScaleMode.Random
                    return _randomVectorS;
            }
        }

        void UpdatePosition(float value)
        {
            var a = BasicMath.Lerp(_translationAmount0, _translationAmount1, value);
            var p = TranslationVector * a;
            if (_addToOriginal) p += _originalPosition;
            _targetTransform.localPosition = p;
        }

        void UpdateRotation(float value)
        {
            var a = BasicMath.Lerp(_rotationAngle0, _rotationAngle1, value);
            var r = Quaternion.AngleAxis(a, RotationAxis);
            if (_addToOriginal) r = _originalRotation * r;
            _targetTransform.localRotation = r;
        }

        void UpdateScale(float value)
        {
            var a = BasicMath.Lerp(_scaleAmount0, _scaleAmount1, value);
            var s = ScaleVector * a;
            if (_addToOriginal) s += _originalScale;
            _targetTransform.localScale = s;
        }

        #endregion

        #region MonoBehaviour Functions

        void OnEnable()
        {
            if (_targetTransform != null)
            {
                _originalPosition = _targetTransform.localPosition;
                _originalRotation = _targetTransform.localRotation;
                _originalScale = _targetTransform.localScale;
            }
        }

        void OnDisable()
        {
            if (_targetTransform != null)
            {
                _targetTransform.localPosition = _originalPosition;
                _targetTransform.localRotation = _originalRotation;
                _targetTransform.localScale = _originalScale;
            }
        }

        void Start()
        {
            _randomVectorT = Random.onUnitSphere;
            _randomVectorR = Random.onUnitSphere;
            _randomVectorS = new Vector3(Random.value, Random.value, Random.value);
        }

        #endregion
    }
}
