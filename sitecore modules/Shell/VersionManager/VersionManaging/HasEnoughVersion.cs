//-------------------------------------------------------------------------------------------------
// <copyright file="HasEnoughVersion.cs" company="Sitecore A/S">
// Copyright (C) 2010 by Sitecore A/S
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sitecore.VersionManager.Pipelines.GetContentEditorWarnings
{
  using System.Configuration;
  using System.Text;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Globalization;
  using Sitecore.Pipelines.GetContentEditorWarnings;

  /// <summary>
  /// Implements the render field
  /// </summary>
  public class HasEnoughVersion
  {
    // Methods
    #region Public methods

    /// <summary>
    /// Gets the field value.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public void Process(GetContentEditorWarningsArgs args)
    {
      if (args != null)
      {
        Item item = args.Item;
        if (item != null)
        {
          if (Settings.GetBoolSetting("VersionManager.ShowContentEditorWarnings", true) && VersionManager.IsItemUnderRoots(item))
          {
            int count = item.Versions.Count;
            int maximum = Settings.GetIntSetting("VersionManager.NumberOfVersionsToKeep", 5);
            if (maximum < 1)
            {
                return;
            }

            if (count >= maximum)
            {
              GetContentEditorWarningsArgs.ContentEditorWarning warning = args.Add();
              if (count == maximum)
              {
                warning.Title = Translate.Text("The current item has reached maximum allowed number of versions.");
              }

              if (count > maximum)
              {
                warning.Title = Translate.Text("The current item has exceeded maximum allowed number of versions.");
                warning.AddOption("Delete obsolete versions", "version:clean");
              }

              if (Settings.GetBoolSetting("VersionManager.AutomaticCleanupEnabled", false))
              {
                warning.Text = this.GetText(item, count - maximum + 1) + ".";
              }
              else
              {
                warning.Text = string.Empty;
              }

              warning.IsExclusive = false;
              warning.HideFields = false;
            }
          }
        }
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the string with version number, which will be deleted 
    /// </summary>
    /// <param name="item"> Current item </param>
    /// <param name="k"> Number of version, which will be deleted  </param>
    /// <returns> string for warning's text with version's number, which will be deleted </returns>
    private string GetText(Item item, int k)
    {
      var result = new StringBuilder(100);
      if (!Context.IsAdministrator && (!item.Locking.IsLocked()) && Settings.RequireLockBeforeEditing && TemplateManager.IsFieldPartOfTemplate(FieldIDs.Lock, item))
      {
        result.Append(Translate.Text("Editing an item"));
      }
      else
      {
        result.Append(Translate.Text("Adding a new version"));
      }

      result.Append(" ");
      result.Append(Translate.Text("will result in version(s) number"));
      Item[] versions = item.Versions.GetVersions(false);

      for (int i = 0; i < k; i++)
      {
        result.Append(" " + versions[i].Version.Number);
        int nextstep = 0;
        if (i == k - 1)
        {
          result.Append(",");
          break;
        }

        for (int j = i; j < k - 1; j++)
        {
          if (versions[j + 1].Version.Number == versions[j].Version.Number + 1)
          {
            nextstep++;
          }
          else
          {
            break;
          }
        }

        if (nextstep == 0)
        {
          result.Append(",");
        }
        else if (nextstep == 1)
        {
          result.Append(", " + versions[i + nextstep].Version.Number + ",");
        }
        else
        {
          result.Append("-" + versions[i + nextstep].Version.Number + ",");
        }

        i += nextstep;
      }

      result = result.Remove(result.Length - 1, 1);
      result.Append(" ");
      result.Append(Translate.Text("being deleted"));
      return result.ToString();
    }

    #endregion
  }
}