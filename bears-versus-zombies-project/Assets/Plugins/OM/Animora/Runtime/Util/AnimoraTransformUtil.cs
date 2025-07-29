using UnityEngine;

namespace OM.Animora.Runtime
{
    /// <summary>
    /// Utility class providing extension methods for Unity's Transform component,
    /// allowing easy getting and setting of position and rotation in both world and local space.
    /// </summary>
    public static class AnimoraTransformUtil
    {
        /// <summary>
        /// Gets the position of the transform in the specified space.
        /// </summary>
        /// <param name="transform">The target transform.</param>
        /// <param name="space">The space in which to get the position (World or Self/Local). Defaults to World.</param>
        /// <returns>The position vector in the specified space.</returns>
        public static Vector3 GetPosition(this Transform transform, Space space = Space.World)
        {
            // Return world position if space is World, otherwise return local position.
            return space == Space.World ? transform.position : transform.localPosition;
        }

        /// <summary>
        /// Sets the position of the transform in the specified space.
        /// </summary>
        /// <param name="transform">The target transform.</param>
        /// <param name="position">The new position vector.</param>
        /// <param name="space">The space in which to set the position (World or Self/Local). Defaults to World.</param>
        public static void SetPosition(this Transform transform, Vector3 position, Space space = Space.World)
        {
            // Set world position if space is World, otherwise set local position.
            if (space == Space.World)
            {
                transform.position = position;
            }
            else
            {
                transform.localPosition = position;
            }
        }

        /// <summary>
        /// Gets the rotation of the transform as a Quaternion in the specified space.
        /// </summary>
        /// <param name="transform">The target transform.</param>
        /// <param name="space">The space in which to get the rotation (World or Self/Local). Defaults to World.</param>
        /// <returns>The rotation quaternion in the specified space.</returns>
        public static Quaternion GetRotation(this Transform transform, Space space = Space.World)
        {
            // Return world rotation if space is World, otherwise return local rotation.
            return space == Space.World ? transform.rotation : transform.localRotation;
        }

        /// <summary>
        /// Sets the rotation of the transform using a Quaternion in the specified space.
        /// </summary>
        /// <param name="transform">The target transform.</param>
        /// <param name="rotation">The new rotation quaternion.</param>
        /// <param name="space">The space in which to set the rotation (World or Self/Local). Defaults to World.</param>
        public static void SetRotation(this Transform transform, Quaternion rotation, Space space = Space.World)
        {
            // Set world rotation if space is World, otherwise set local rotation.
            if (space == Space.World)
            {
                transform.rotation = rotation;
            }
            else
            {
                transform.localRotation = rotation;
            }
        }

        /// <summary>
        /// Gets the rotation of the transform as Euler angles (Vector3) in the specified space.
        /// </summary>
        /// <param name="transform">The target transform.</param>
        /// <param name="space">The space in which to get the Euler angles (World or Self/Local). Defaults to World.</param>
        /// <returns>The rotation as Euler angles in the specified space.</returns>
        public static Vector3 GetRotationEuler(this Transform transform, Space space = Space.World)
        {
            // Return world Euler angles if space is World, otherwise return local Euler angles.
            return space == Space.World ? transform.eulerAngles : transform.localEulerAngles;
        }

        /// <summary>
        /// Sets the rotation of the transform using Euler angles (Vector3) in the specified space.
        /// </summary>
        /// <param name="transform">The target transform.</param>
        /// <param name="rotation">The new rotation as Euler angles.</param>
        /// <param name="space">The space in which to set the Euler angles (World or Self/Local). Defaults to World.</param>
        public static void SetRotationEuler(this Transform transform, Vector3 rotation, Space space = Space.World)
        {
            // Set world Euler angles if space is World, otherwise set local Euler angles.
            if (space == Space.World)
            {
                transform.eulerAngles = rotation;
            }
            else
            {
                transform.localEulerAngles = rotation;
            }
        }
    }
}