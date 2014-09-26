//-------------------------------------------------------------------------------------------------
// <copyright file="GridItem.cs" company="Sitecore A/S">
// Copyright (C) 2010 by Sitecore A/S
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sitecore.VersionManager
{
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    
    /// <summary>
    /// the source class for a grid
    /// </summary>
    public class GridItem
    {
        #region Fields

        /// <summary>
        /// consist of item id and item language, separated by '^'
        /// </summary>
        private string itemLang;

        /// <summary>
        /// stores item name
        /// </summary>
        private string name;

        /// <summary>
        /// stores item language
        /// </summary>
        private string language;

        /// <summary>
        /// stores nubmer of versions in one item language
        /// </summary>
        private int versionCount;

        /// <summary>
        /// stores item path
        /// </summary>
        private string itemPath;

        /// <summary>
        /// stores item id
        /// </summary>
        private ID id;

        #endregion

        /// <summary>
        /// Initializes a new instance of the GridItem class
        /// </summary>
        /// <param name="item">source item for initialization</param>
        public GridItem(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            this.itemLang = item.ID + "^" + item.Language;
            this.name = item.Name;
            this.language = item.Language.ToString();
            this.versionCount = item.Versions.Count;
            this.itemPath = item.Paths.FullPath;
            this.id = item.ID;
        }

        #region Properties

        /// <summary>
        /// Gets itemLang value
        /// </summary>
        public string ItemLang
        {
            get
            {
                return this.itemLang;
            }
        }

        /// <summary>
        /// Gets itemPath value
        /// </summary>
        public string ItemPath
        {
            get
            {
                return this.itemPath;
            }
        }
        
        /// <summary>
        /// Gets versionCount value
        /// </summary>
        public int VersionCount
        {
            get
            {
                return this.versionCount;
            }
        }

        /// <summary>
        /// Gets name value
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets language value
        /// </summary>
        public string Language
        {
            get
            {
                return this.language;
            }
        }

        /// <summary>
        /// Gets id value
        /// </summary>
        public ID ID
        {
            get
            {
                return this.id;
            }
        }

        #endregion
    }
}
