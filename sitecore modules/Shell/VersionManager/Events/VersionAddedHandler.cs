//-------------------------------------------------------------------------------------------------
// <copyright file="VersionAddedHandler.cs" company="Sitecore A/S">
// Copyright (C) 2010 by Sitecore A/S
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sitecore.VersionManager.Handlers
{
  using System;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Events;
    using Sitecore.VersionManager;

  /// <summary>
  /// Represents a version added event. Used for deleting old versions.
  /// </summary>
  public class VersionAddedHandler
  {
    // Methods

    /// <summary>
    /// Called when the version has added
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="args">The arguments</param>
    protected void OnVersionAdded(object sender, EventArgs args)
    {
      if (args != null)
      {
        if (Settings.GetBoolSetting("VersionManager.AutomaticCleanupEnabled", true))
        {
          var item = Event.ExtractParameter(args, 0) as Item;
          if ((item != null) && VersionManager.IsItemUnderRoots(item)) 
          {
            VersionManager.DeleteItemVersions(item);
          }
        }
      }
    }
  }
}