using UnityEngine;

namespace Phezu.Util {

    public static class GameObjectUtil {

        /// <summary>
        /// Returns whether the layer is contained in layerMask
        /// </summary>
        /// <param name="layerMask">The LayerMask</param>
        /// <param name="layer">The layer to check</param>
        /// <returns>True if layer is contained in the layer mask</returns>
        public static bool IsInLayerMask(LayerMask layerMask, int layer) {
            return layerMask == (layerMask | 1 << layer);
        }
    }
}