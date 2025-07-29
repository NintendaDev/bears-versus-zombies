using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace OM
{
    /// <summary>
    /// Provides a collection of static utility/helper methods for common operations
    /// within the OM namespace, often related to Unity types and functionality like
    /// type checking, geometry, UI checks, string formatting, and object manipulation.
    /// </summary>
    public static class OM_Utility
    {
        /// <summary>
        /// Checks if a given type derives from a specific base type, traversing up the inheritance hierarchy.
        /// </summary>
        /// <param name="derivedType">The type to check.</param>
        /// <param name="baseType">The potential base type.</param>
        /// <returns>True if derivedType is derived from baseType, false otherwise.</returns>
        /// <remarks>This check includes direct inheritance and inheritance further up the chain. It stops when it reaches System.Object.</remarks>
        public static bool IsDerivedFromBaseType(this Type derivedType, Type baseType)
        {
            // Iterate up the inheritance chain as long as we haven't reached the top (object)
            // or hit a null type (shouldn't happen with valid types but good practice).
            while (derivedType != null && derivedType != typeof(object))
            {
                // Check if the current type in the hierarchy is the base type we're looking for.
                if (derivedType == baseType)
                {
                    return true;
                }
                // Move up to the parent type.
                derivedType = derivedType.BaseType;
            }
            // Reached the top without finding the base type.
            return false;
        }

        /// <summary>
        /// Formats a large long integer into a more readable string representation using metric-like suffixes
        /// (k, M, B, T, aa, ab, etc.). Useful for displaying scores, currency, etc.
        /// </summary>
        /// <param name="t">The long integer value to format.</param>
        /// <returns>A formatted string (e.g., "1.2k", "5.0M", "123aa").</returns>
        /// <remarks>
        /// The formatting logic divides the number by 1000 repeatedly (approximately, using 100f/10f steps).
        /// It displays one decimal place if the number isn't whole after scaling (e.g., "1.2k"),
        /// otherwise displays it as a whole number (e.g., "5M").
        /// Handles numbers up to the limit defined by the length of the `scoreNames` array.
        /// </remarks>
        public static string FormatNumberWithSuffix(this long t)
        {
            var amount = (double)t; // Work with double for division.
            string result;
            // Suffixes for large numbers. "" is for numbers < 1000.
            var scoreNames = new string[] {"", "k","M", "B", "T",
                                          "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak",
                                          "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av",
                                          "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg",
                                          "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br",
                                          "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", }; // Extend as needed
            int i;

            // Find the appropriate suffix index.
            for (i = 0; i < scoreNames.Length; i++)
            {
                if (amount <= 999) // If the number is small enough for the current suffix, stop.
                    break;
                else // Otherwise, scale it down for the next suffix.
                    // Equivalent to dividing by 1000 but done in steps to manage potential floating point inaccuracies slightly differently.
                    amount = Math.Floor(amount / 100f) / 10f;
            }

            // Format the result string.
            if (Math.Abs(amount - Math.Floor(amount)) < .1f) // Check if the scaled number is effectively whole.
                result = amount.ToString() + scoreNames[i]; // Format as integer + suffix.
            else
                result = amount.ToString("F1") + scoreNames[i]; // Format as one decimal place + suffix.

            return result;
        }

        /// <summary>
        /// Calculates the world position on a custom plane defined by a normal and a point on the plane,
        /// based on a screen position and camera.
        /// </summary>
        /// <param name="screenPosition">The screen position (e.g., Input.mousePosition).</param>
        /// <param name="currentCamera">The camera rendering the scene.</param>
        /// <param name="normal">The normal vector of the custom plane.</param>
        /// <param name="planePos">A point that lies on the custom plane.</param>
        /// <returns>The world position where the ray from the camera through the screen position intersects the defined plane.</returns>
        /// <remarks>DEPRECATED/BUGGY? The `Plane` constructor takes `(normal, point)`, but the code uses `(screenPosition, planePos)`. This seems incorrect. It should likely be `new Plane(normal, planePos)`. The current implementation defines a plane based on the screen position vector used as a normal, which is unusual.</remarks>
        public static Vector3 GetMouseWorldPositionOnPlane(Vector3 screenPosition, Camera currentCamera, Vector3 normal, Vector3 planePos)
        {
            var ray = currentCamera.ScreenPointToRay(screenPosition);
            // !!! Potential Issue: Plane constructor arguments might be incorrect.
            // Should probably be: var xy = new Plane(normal, planePos);
            var xy = new Plane(screenPosition, planePos); // Using screen position as normal? Seems wrong.
            xy.Raycast(ray, out var distance);
            return ray.GetPoint(distance);
        }

        /// <summary>
        /// Calculates the world position on the Z=0 plane (XY plane relative to world origin),
        /// based on a screen position and camera. Assumes a standard forward direction.
        /// </summary>
        /// <param name="screenPosition">The screen position (e.g., Input.mousePosition).</param>
        /// <param name="currentCamera">The camera rendering the scene.</param>
        /// <returns>The world position on the Z=0 plane.</returns>
        public static Vector3 GetWorldPositionOnPlaneForward(Vector3 screenPosition, Camera currentCamera)
        {
            var ray = currentCamera.ScreenPointToRay(screenPosition);
            // Define the XY plane at Z=0 (normal is Vector3.forward, point is origin).
            var xy = new Plane(Vector3.forward, Vector3.zero);
            xy.Raycast(ray, out var distance); // Find intersection distance.
            return ray.GetPoint(distance); // Get world point at that distance along the ray.
        }

        /// <summary>
        /// Calculates the world position on the Y=0 plane (XZ plane relative to world origin),
        /// based on a screen position and camera. Assumes a standard downward direction for the plane normal.
        /// </summary>
        /// <param name="screenPosition">The screen position (e.g., Input.mousePosition).</param>
        /// <param name="currentCamera">The camera rendering the scene.</param>
        /// <returns>The world position on the Y=0 plane.</returns>
        public static Vector3 GetWorldPositionOnPlaneDown(Vector3 screenPosition, Camera currentCamera)
        {
            var ray = currentCamera.ScreenPointToRay(screenPosition);
            // Define the XZ plane at Y=0 (normal is Vector3.down, point is origin).
            var xy = new Plane(Vector3.down, Vector3.zero);
            xy.Raycast(ray, out var distance); // Find intersection distance.
            return ray.GetPoint(distance); // Get world point at that distance along the ray.
        }

        /// <summary>
        /// Attempts to detect a Collider2D within a small radius around the mouse cursor's world position
        /// (projected onto the Z=0 plane) and returns the specified Component type T
        /// from the detected object or its parents.
        /// </summary>
        /// <typeparam name="T">The type of Component to search for.</typeparam>
        /// <param name="mousePosition">The current mouse position from Input.</param>
        /// <param name="cam">The relevant camera (e.g., Camera.main).</param>
        /// <param name="layerMask">The LayerMask to filter which colliders are considered.</param>
        /// <param name="result">Output parameter for the found Component.</param>
        /// <param name="radius">The radius of the circle check around the mouse world position.</param>
        /// <returns>True if a matching Component is found, false otherwise.</returns>
        public static bool TryGetObjectUnderMouse2D<T>(Vector3 mousePosition, Camera cam, LayerMask layerMask, out T result, float radius = .2f) where T : Component
        {
            // Calculate the world position on the Z=0 plane corresponding to the mouse position.
            Vector3 worldPos = GetWorldPositionOnPlaneForward(mousePosition, cam);
            // Perform a 2D circle overlap check at that world position.
            var overlapCircle = Physics2D.OverlapCircle(worldPos, radius, layerMask);

            if (overlapCircle != null) // If a collider was hit
            {
                // Try to get the desired Component type from the hit collider's GameObject or its parents.
                result = overlapCircle.GetComponentInParent<T>();
                return result != null; // Return true if the component was found.
            }

            // No collider hit or component not found.
            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to detect an object with a Collider within a specified distance and radius
        /// along a ray cast from the camera through the mouse position in 3D space,
        /// and returns the specified Component type T from the detected object or its parents.
        /// </summary>
        /// <typeparam name="T">The type of Component to search for.</typeparam>
        /// <param name="mousePosition">The current mouse position from Input.</param>
        /// <param name="cam">The relevant camera (e.g., Camera.main).</param>
        /// <param name="layerMask">The LayerMask to filter which colliders are considered.</param>
        /// <param name="result">Output parameter for the found Component.</param>
        /// <param name="maxDistance">The maximum distance the sphere cast will travel.</param>
        /// <param name="radius">The radius of the sphere cast.</param>
        /// <returns>True if a matching Component is found, false otherwise.</returns>
        public static bool TryGetObjectUnderMouse3D<T>(Vector3 mousePosition, Camera cam, LayerMask layerMask, out T result, float maxDistance = 100, float radius = .2f) where T : Component
        {
            var ray = cam.ScreenPointToRay(mousePosition); // Create a ray from camera through mouse.
            // Perform a sphere cast along the ray.
            if (Physics.SphereCast(ray, radius, out var hit, maxDistance, layerMask))
            {
                // If the sphere cast hit something
                // Try to get the desired Component type from the hit transform's GameObject or its parents.
                result = hit.transform.GetComponentInParent<T>();
                return result != null; // Return true if the component was found.
            }

            // Sphere cast didn't hit anything within the distance/layerMask.
            result = null;
            return false;
        }

        /// <summary>
        /// Calculates the average position of all immediate children of a Transform.
        /// </summary>
        /// <param name="transform">The parent Transform.</param>
        /// <returns>The average world position (center) of the children. Returns Vector3.zero if there are no children.</returns>
        public static Vector3 GetCenterOfChildren(this Transform transform)
        {
            var center = Vector3.zero;
            var childCount = transform.childCount;
            if (childCount == 0) return center; // Handle case with no children.

            // Sum up the positions of all children.
            for (var i = 0; i < childCount; i++)
            {
                center += transform.GetChild(i).position;
            }
            // Divide by the number of children to get the average.
            return center / childCount;
        }

        /// <summary>
        /// Destroys all immediate children GameObjects of a given Transform.
        /// </summary>
        /// <param name="transform">The parent Transform whose children will be destroyed.</param>
        /// <remarks>Uses Object.Destroy, which typically happens at the end of the current frame.</remarks>
        public static void DestroyChildren(this Transform transform)
        {
            var childCount = transform.childCount;
            // Iterate backwards because destroying an object modifies the child collection.
            for (var i = childCount - 1; i >= 0; i--)
            {
                // Destroy the child GameObject.
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Gets the screen coordinates (in pixels) corresponding to a predefined anchor point (enum).
        /// </summary>
        /// <param name="anchor">The desired anchor point (e.g., TopLeft, Center, BottomRight).</param>
        /// <returns>The pixel coordinates on the screen for the specified anchor.</returns>
        public static Vector2 GetScreenAnchor(OM_Anchor anchor = OM_Anchor.Center)
        {
            switch (anchor)
            {
                case OM_Anchor.Top: return new Vector2(Screen.width / 2f, Screen.height);
                case OM_Anchor.Bottom: return new Vector2(Screen.width / 2f, 0);
                case OM_Anchor.Right: return new Vector2(Screen.width, Screen.height / 2f);
                case OM_Anchor.Left: return new Vector2(0, Screen.height / 2f);
                case OM_Anchor.TopRight: return new Vector2(Screen.width, Screen.height);
                case OM_Anchor.TopLeft: return new Vector2(0, Screen.height);
                case OM_Anchor.BottomRight: return new Vector2(Screen.width, 0);
                case OM_Anchor.BottomLeft: return Vector2.zero; // Bottom-left is (0, 0)
                case OM_Anchor.Center:
                default: return new Vector2(Screen.width / 2f, Screen.height / 2f);
            }
        }

        /// <summary>
        /// Gets the screen coordinates (in pixels) corresponding to a normalized anchor point (Vector2).
        /// (0,0) is bottom-left, (1,1) is top-right, (0.5, 0.5) is center.
        /// </summary>
        /// <param name="anchor">The normalized anchor position (components typically 0 to 1).</param>
        /// <returns>The pixel coordinates on the screen for the specified normalized anchor.</returns>
        public static Vector2 GetScreenAnchor(Vector2 anchor)
        {
            return new Vector2(Screen.width * anchor.x, Screen.height * anchor.y);
        }

        /// <summary>
        /// Destroys a Unity Object after a specified delay.
        /// </summary>
        /// <param name="target">The Object (Component, GameObject, Asset) to destroy.</param>
        /// <param name="duration">The delay in seconds before destroying the object.</param>
        public static void KillAfter(this Object target, float duration)
        {
            // Uses the built-in Destroy overload with a delay.
            Object.Destroy(target, duration);
        }

        /// <summary>
        /// Executes a callback function after a specified delay, using the OMTimer system.
        /// The target object itself is not destroyed by this method.
        /// </summary>
        /// <param name="target">The object initiating the delayed action (used for context, not destroyed).</param>
        /// <param name="duration">The delay in seconds.</param>
        /// <param name="callback">The Action to execute after the delay.</param>
        /// <param name="unscaledTime">If true, use unscaled time (ignores Time.timeScale). If false, use scaled time.</param>
        public static void After(this Object target, float duration, Action callback, bool unscaledTime = false)
        {
            // Creates a timer using a separate timer system (OMTimer).
            OMTimer.Create(duration, callback, unscaledTime);
        }

        /// <summary>
        /// Calculates points lying on the circumference of a circle in the XY plane.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="pointsCount">The number of points to generate around the circle.</param>
        /// <returns>An array of Vector3 points lying on the circle's circumference.</returns>
        public static Vector3[] GetCirclePoints2D(Vector3 center, float radius, int pointsCount)
        {
            if (pointsCount <= 0) return Array.Empty<Vector3>(); // Handle invalid count

            var points = new Vector3[pointsCount];
            // Calculate the angle between each point.
            var slice = 2 * Mathf.PI / pointsCount;
            for (var i = 0; i < pointsCount; i++)
            {
                var angle = slice * i;
                // Calculate X and Y using cosine and sine.
                var newX = center.x + radius * Mathf.Cos(angle);
                var newY = center.y + radius * Mathf.Sin(angle);
                // Keep the original Z coordinate.
                points[i] = new Vector3(newX, newY, center.z);
            }
            return points;
        }

        /// <summary>
        /// Calculates points lying on the circumference of a circle in the XZ plane.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="pointsCount">The number of points to generate around the circle.</param>
        /// <returns>An array of Vector3 points lying on the circle's circumference.</returns>
        public static Vector3[] GetCirclePoints3D(Vector3 center, float radius, int pointsCount)
        {
             if (pointsCount <= 0) return Array.Empty<Vector3>(); // Handle invalid count

            var points = new Vector3[pointsCount];
             // Calculate the angle between each point.
            var slice = 2 * Mathf.PI / pointsCount;
            for (var i = 0; i < pointsCount; i++)
            {
                var angle = slice * i;
                 // Calculate X and Z using cosine and sine.
                var newX = center.x + radius * Mathf.Cos(angle);
                var newZ = center.z + radius * Mathf.Sin(angle);
                 // Keep the original Y coordinate.
                points[i] = new Vector3(newX, center.y, newZ);
            }
            return points;
        }

        /// <summary>
        /// Calculates points lying on the circumference of a circle in the XY plane, with an additional starting angle offset.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="pointsCount">The number of points to generate.</param>
        /// <param name="angle">The starting angle offset in radians.</param>
        /// <returns>An array of Vector3 points lying on the circle's circumference, offset by the angle.</returns>
        public static Vector3[] GetCirclePoints2DWithAngle(Vector3 center, float radius, int pointsCount, float angle)
        {
             if (pointsCount <= 0) return Array.Empty<Vector3>(); // Handle invalid count

            var points = new Vector3[pointsCount];
            var slice = 2 * Mathf.PI / pointsCount;
            for (var i = 0; i < pointsCount; i++)
            {
                // Add the offset angle to the calculated slice angle.
                var newAngle = slice * i + angle;
                var newX = center.x + radius * Mathf.Cos(newAngle);
                var newY = center.y + radius * Mathf.Sin(newAngle);
                points[i] = new Vector3(newX, newY, center.z);
            }
            return points;
        }

        /// <summary>
        /// Generates points within a rectangular grid centered around a point, potentially with noise displacement and excluding inner points (hollow).
        /// </summary>
        /// <param name="center">The center point of the grid.</param>
        /// <param name="width">The number of points along the X axis.</param>
        /// <param name="depth">The number of points along the Z axis.</param>
        /// <param name="spread">A multiplier applied to the grid spacing.</param>
        /// <param name="noise">A multiplier for the Perlin noise displacement (applied to X and Z based on world position).</param>
        /// <param name="hollow">If true, only generate points on the outer border of the grid.</param>
        /// <returns>An IEnumerable yielding Vector3 points representing the grid.</returns>
        public static IEnumerable<Vector3> EvaluatePointsBox(Vector3 center, int width, int depth, float spread, float noise, bool hollow)
        {
            // Calculate offset to center the grid around (0,0) before adding the actual center.
            var middleOffset = new Vector3(width * .5f, 0, depth * .5f);

            for (var x = 0; x < width; x++)
            {
                for (var z = 0; z < depth; z++)
                {
                    // If hollow is requested, skip points that are not on the border.
                    if (hollow && (x != 0 && x != width - 1 && z != 0 && z != depth - 1))
                        continue;

                    // Calculate base position relative to grid origin.
                    var pos = new Vector3(x, 0, z);
                    pos -= middleOffset; // Center the grid around (0,0).
                    pos += center;      // Move to the final center point.
                    pos *= spread;      // Apply spacing multiplier.
                    pos += GetNoiseAt(pos, noise); // Apply Perlin noise displacement.
                    yield return pos; // Return the calculated point.
                }
            }
        }

        /// <summary>
        /// Gets a 2D Perlin noise value based on Vector2 input coordinates.
        /// </summary>
        /// <param name="noise">The X and Y coordinates for the noise function.</param>
        /// <returns>A float value between 0.0 and 1.0 (typically).</returns>
        public static float GetNoise(Vector2 noise)
        {
            return Mathf.PerlinNoise(noise.x, noise.y);
        }

        /// <summary>
        /// Gets a 2D Perlin noise value using the X and Z components of a Vector3 position,
        /// scaled by a noise factor. Returns the noise value replicated in the X and Z components
        /// of a new Vector3, with Y being 0.
        /// </summary>
        /// <param name="pos">The input position (X and Z used for noise calculation).</param>
        /// <param name="noise">A scaling factor applied to the position components before querying Perlin noise.</param>
        /// <returns>A Vector3 where X and Z are the calculated noise value, and Y is 0.</returns>
        public static Vector3 GetNoiseAt(Vector3 pos, float noise)
        {
            // Calculate Perlin noise using scaled X and Z coordinates.
            var f = Mathf.PerlinNoise(pos.x * noise, pos.z * noise);
            // Return noise value in X and Z components.
            return new Vector3(f, 0, f);
        }

        /// <summary>
        /// Checks if the mouse pointer is currently over a UI element managed by the EventSystem.
        /// </summary>
        /// <returns>True if the pointer is over a UI element, false otherwise.</returns>
        /// <remarks>Requires an EventSystem component to be active in the scene.</remarks>
        public static bool IsOverUI()
        {
            // Check if the EventSystem exists and if the pointer is over a GameObject it manages.
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// Checks if a float value is within a specified range (inclusive) of a target value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="target">The target value.</param>
        /// <param name="range">The maximum allowed difference (absolute) between value and target.</param>
        /// <returns>True if the value is within the range, false otherwise.</returns>
        public static bool IsWithinRange(float value, float target, float range)
        {
            // Calculate the absolute difference and compare it to the allowed range.
            return Mathf.Abs(value - target) <= range;
        }

        /// <summary>
        /// Formats a label string (typically camelCase or PascalCase) by inserting spaces before uppercase letters.
        /// Useful for converting variable names into more readable display text.
        /// </summary>
        /// <param name="label">The input string (e.g., "someValueName").</param>
        /// <returns>A formatted string with spaces inserted (e.g., "some Value Name").</returns>
        public static string FormatLabel(string label)
        {
            if (string.IsNullOrEmpty(label)) return ""; // Handle null or empty input.

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(label[0]); // Add the first character directly.

            // Iterate through the rest of the string.
            for (int i = 1; i < label.Length; i++)
            {
                // If the current character is uppercase and not preceded by a space (or start of string), insert a space.
                if (char.IsUpper(label[i])) // Removed `&& i > 0` as loop starts at 1 anyway. Added check for preceding space implicitly by only adding space *before* upper char.
                {
                     // Check if previous char was also upper or a space to avoid double spaces (e.g., "HTTPRequest" -> "HTTP Request" not "H T T P Request")
                     if(!char.IsUpper(label[i-1]) && label[i-1] != ' ')
                     {
                        stringBuilder.Append(" ");
                     }
                }
                stringBuilder.Append(label[i]); // Add the current character.
            }
            return stringBuilder.ToString();
        }
    }
}