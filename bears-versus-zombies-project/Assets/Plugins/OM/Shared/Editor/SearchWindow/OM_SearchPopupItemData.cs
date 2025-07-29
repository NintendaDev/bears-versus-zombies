using System.Collections.Generic;
using System.Linq;

namespace OM.Editor
{
    /// <summary>
    /// Represents a single searchable item displayed in the OM_SearchPopup.
    /// Holds metadata like name, path, icon, description, and search keywords.
    /// </summary>
    public struct OM_SearchPopupItemData
    {
        /// <summary>
        /// Display name of the item.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Hierarchical path used to organize the item in folders or groups.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Raw data object associated with the item (can be any type).
        /// </summary>
        public readonly object Data;

        /// <summary>
        /// Icon identifier (name in resources or Unity icon string).
        /// </summary>
        public readonly string Icon;

        /// <summary>
        /// Specifies the type of icon to use (e.g., Unity icon or custom resource).
        /// </summary>
        public readonly OM_IconType IconType;

        /// <summary>
        /// Optional description shown when the item is highlighted.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Searchable keywords associated with the item.
        /// </summary>
        public readonly List<string> Keywords;

        /// <summary>
        /// Creates a new instance of OM_SearchPopupItemData with all display and metadata information.
        /// </summary>
        /// <param name="name">Display name of the item.</param>
        /// <param name="path">Group or folder path of the item.</param>
        /// <param name="data">Custom data object attached to the item.</param>
        /// <param name="iconType">Type of icon (Unity or resource-based).</param>
        /// <param name="icon">Icon string (optional).</param>
        /// <param name="description">Description shown in info section (optional).</param>
        /// <param name="keywords">Keywords for search filtering (optional).</param>
        public OM_SearchPopupItemData(string name, string path, object data, OM_IconType? iconType, string icon = null, string description = null, string[] keywords = null)
        {
            Name = name;
            Path = path;
            Data = data;
            IconType = iconType ?? OM_IconType.DefaultUnityIcon;
            Icon = icon;
            Description = description;
            Keywords = keywords != null ? keywords.ToList() : new List<string>();
        }
    }
}
