using System;
using System.Collections.Generic;
using VRM;
using MediaPipeBlendshapes = HumanLandmarks.Blendshapes.Types.BlendshapesIndex;

namespace VioletSolver.FaceApplier
{
    /// <summary>
    /// Applies blendshapes to VRM 0.x format avatars.
    /// </summary>
    public class Vrm0xFaceApplier : IFaceApplier
    {
        readonly VRMBlendShapeProxy _proxy;

        /// <summary>
        /// Initializes a FaceApplier for VRM 0.x.
        /// </summary>
        /// <param name="proxy">The VRMBlendShapeProxy component.</param>
        public Vrm0xFaceApplier(VRMBlendShapeProxy proxy) => _proxy = proxy;

        /// <inheritdoc/>
        public void Apply(IReadOnlyDictionary<BlendShapePreset, float> blendshapes)
        {
            var bs = new Dictionary<BlendShapeKey, float>();

            var tmpArray = Enum.GetValues(typeof(BlendShapePreset));
            foreach (var value in tmpArray)
            {
                var blendshapeIndex = (BlendShapePreset)value;
                if (blendshapes.TryGetValue(blendshapeIndex, out var blendshape))
                    bs[BlendShapeKey.CreateFromPreset(blendshapeIndex)] = blendshape;
            }

            _proxy.SetValues(bs);
        }

        /// <inheritdoc/>
        public void Apply(IReadOnlyDictionary<MediaPipeBlendshapes, float> blendshapes)
        {
            var bs = new Dictionary<BlendShapeKey, float>();

            var tmpArray = Enum.GetValues(typeof(MediaPipeBlendshapes));
            foreach (var value in tmpArray)
            {
                var blendshapeIndex = (MediaPipeBlendshapes)value;
                if (blendshapes.TryGetValue(blendshapeIndex, out var blendshape))
                    bs[BlendShapeKey.CreateUnknown(blendshapeIndex.ToString())] = blendshape;
            }

            _proxy.SetValues(bs);
        }
    }
}
