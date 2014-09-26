//-------------------------------------------------------------------------------------------------
// <copyright file="VersionManager.cs" company="Sitecore A/S">
// Copyright (C) 2010 by Sitecore A/S
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sitecore.VersionManager
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using System.Xml;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Serialization;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using StringExtensions;

    /// <summary>
    /// Class that will manage deleting, serialization, finding items
    /// </summary>
    public class VersionManager
    {
        /// <summary>
        /// Contains overflowed items. As key used Item.ID+"^"+Item.Language 
        /// </summary>
        private static Dictionary<string, Item> sourceList = new Dictionary<string, Item>();

        private static bool isDisabled;

        #region Properties

        /// <summary>
        /// Gets a list of overflowed items
        /// </summary>
        public static List<Item> ItemVersions
        {
            get
            {
                if (sourceList == null || sourceList.Count == 0)
                {
                    GetItemVersions();
                }

                return new List<Item>(sourceList.Values);
            }
        }

        /// <summary>
        /// Gets a value indicating whether Version Manager is disabled.
        /// </summary>
        public static bool IsDisabled
        {
            get
            {
                XmlNodeList nodes = Factory.GetConfigNodes("settings/setting[@name='VersionManager.Roots']/root");
                isDisabled = (nodes.Count == 0) || (Settings.GetIntSetting("VersionManager.NumberOfVersionsToKeep", 5) < 1);
                return isDisabled;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets an item with specified language and guid
        /// </summary>
        /// <param name="str">Contains item guid and item language</param>
        /// <returns>item from guid and language values</returns>
        public static Item GetItemFromStr(string str)
        {
            string[] itemParams = str.ToString().Split(new char[] { '^' });
            Language language = Language.Parse(itemParams[1]);
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
            Item item = master.GetItem(itemParams[0], language);
            return item;
        }

        /// <summary>
        /// Converts list of Item to the list of GridItem
        /// </summary>
        /// <returns>a list of a GridItem</returns>
        public static List<GridItem> GetGridItem()
        {
            List<GridItem> tempList = new List<GridItem>();
            foreach (Item item in ItemVersions)
            {
                tempList.Add(new GridItem(item));
            }

            return tempList;
        }

        /// <summary>
        /// Clear sourceList
        /// </summary>
        public static void Refresh()
        {
            if (sourceList != null)
            {
                sourceList.Clear();
            }
        }

        /// <summary>
        /// Delete versions of item if item has more than maximum allowed number of versions. 
        /// Deleted versions are serialized. 
        /// </summary>
        /// <param name="item">Current item</param>
        public static void DeleteItemVersions(Item item)
        {
            if (IsDisabled)
            {
                return;
            }

            int maximum = Settings.GetIntSetting("VersionManager.NumberOfVersionsToKeep", 5);
            bool isSerialize = Settings.GetBoolSetting("VersionManager.ArchiveDeletedVersions", true);
            int current = item.Versions.Count;
            if (current - maximum > 0)
            {
                Item[] versions = item.Versions.GetVersions(false);
                if (isSerialize)
                {
                    SerializeItemVersions(item, versions[0].Version.Number, versions[versions.Length - 1].Version.Number);
                }

                for (int i = 0; i < current - maximum; i++)
                {
                    versions[i].Versions.RemoveVersion();
                    Log.Info("Version Manager: Removed version {0}; Item: {1}; Language: {2}".FormatWith(i, item.Paths.Path, item.Language.ToString()), "DeleteItemVersions");
                }

                sourceList.Remove(item.ID.ToString() + "^" + item.Language.ToString());
            }
        }

        /// <summary>
        /// Gets/removes all descendants overflowed versions of specified item
        /// </summary>
        public static void GetItemVersions()
        {
            Database master = Factory.GetDatabase("master");
            foreach (string str in GetAllRoots())
            {
                Item startItem = master.GetItem(str);
                CheckVersion(startItem);
            }
        }

        /// <summary>
        /// Checks if the item is under the any root that are defined in the config file
        /// </summary>
        /// <param name="item"> The item for analyze </param>
        /// <returns> True if item is under some root, false in other way </returns>
        public static bool IsItemUnderRoots(Item item)
        {
            foreach (string root in GetAllRoots())
            {
                if (item.Paths.FullPath.ToUpper().Contains(root.ToUpper()))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// delete roots that is childs of some other roots
        /// </summary>
        /// <param name="roots">List of roots</param>
        private static void CheckRoots(List<string> roots)
        {
            Database master = Factory.GetDatabase("master");
            for (int i = 0; i < roots.Count; i++)
            {
                for (int j = 0; j < roots.Count; j++)
                {
                    if (i != j)
                    {
                        if (roots[i].ToUpper().Contains(roots[j].ToUpper()))
                        {
                            roots.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            for (int i = 0; i < roots.Count; i++)
            {
                Item home = master.GetItem(roots[i]);
                if (home == null)
                {
                    roots.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Adds overflowed versions of an item tree to the GridItem list
        /// </summary>
        /// <param name="item">processed item</param>
        private static void CheckVersion(Item item)
        {
            int maxVersions = Settings.GetIntSetting("VersionManager.NumberOfVersionsToKeep", 5);
            Database master = Factory.GetDatabase("master");
            foreach (Language language in item.Languages)
            {
                Item langItem = master.GetItem(item.ID, language);
                if (langItem.Versions.Count > maxVersions)
                {
                    sourceList.Add(langItem.ID.ToString() + "^" + langItem.Language.ToString(), langItem);
                }
            }

            foreach (Item item1 in item.Children)
            {
                CheckVersion(item1);
            }
        }

        /// <summary>
        /// Get all valid roots from web.config
        /// </summary>
        /// <returns>roots from web.config</returns>
        private static List<string> GetAllRoots()
        {
            var result = new List<string>();
            XmlNodeList nodes = Factory.GetConfigNodes("settings/setting[@name='VersionManager.Roots']/root");
            if (nodes.Count == 0)
            {
                isDisabled = true;
                return result;
            }
            else
            {
                foreach (XmlNode node in nodes)
                {
                    result.Add(node.Attributes["value"].Value);
                }
            }

            CheckRoots(result);
            return result;
        }

        /// <summary>
        /// Serializes the version
        /// </summary>
        /// <param name="item">Version for serializing</param>
        /// <param name="first">First version number  </param>
        /// <param name="last">Last version number</param>
        private static void SerializeItemVersions(Item item, int first, int last)
        {
            Assert.ArgumentNotNull(item, "item");
            var reference = new ItemReference(item);
            Log.Info("Serializing {0}", new object[] { reference });
            var path = new StringBuilder("VersionManager/");
            path.Append(DateTime.Now.Year + "/");
            path.Append(DateTime.Now.Month + "/");
            path.Append(DateTime.Now.Day + "/");
            path.Append(item.Name + item.ID + "/");
            if (first != last)
            {
                path.Append("Versions_" + first + "-" + last);
            }
            else
            {
                path.Append("Versions_" + first);
            }

            Manager.DumpItem(PathUtils.GetFilePath(path.ToString()), item);
        }

        #endregion
    }
}