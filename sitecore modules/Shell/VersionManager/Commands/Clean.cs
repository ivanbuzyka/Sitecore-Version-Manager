//-------------------------------------------------------------------------------------------------
// <copyright file="Clean.cs" company="Sitecore A/S">
// Copyright (C) 2010 by Sitecore A/S
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sitecore.VersionManager.Commands
{
  using System;
  using System.Collections.Specialized;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Shell.Framework.Commands;
  using Sitecore.Text;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Web.UI.WebControls;
  using Sitecore.Web.UI.XamlSharp.Continuations;

  /// <summary>
  /// The class that performs deleting item versions
  /// </summary>
  [Serializable]
  public class Clean : Command, ISupportsContinuation
  {
    /// <summary>
    /// determine if the Command begins execute in Content Editor
    /// </summary>
    private bool isFromCE = false;
    
    #region Public methods

    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      string str;
      if (context.Items.Length != 0)
      {
        str = context.Items[0].ID + "^" + context.Items[0].Language.Name;
        this.isFromCE = true;
      }
      else
      {
        str = context.Parameters["item"];
        this.isFromCE = false;
      }

      if (string.IsNullOrEmpty(str))
      {
        SheerResponse.Alert("Select an item first.", new string[0]);
      }
      else
      {
        var parameters = new NameValueCollection();
        parameters["item"] = str;
        var args = new ClientPipelineArgs(parameters);
        if (this.isFromCE)
        {
          Sitecore.Context.ClientPage.Start(this, "Run", args);
        }
        else
        {
          ContinuationManager.Current.Start(this, "Run", args);
        }
      }
    }

    public override CommandState QueryState(CommandContext context)
    {
        Assert.ArgumentNotNull(context, "context");
        if (VersionManager.IsDisabled)
        {
            return CommandState.Disabled;
        }
        else
        {
            return CommandState.Enabled;
        }
    }

    #endregion

    #region Protected methods

    protected void Run(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (args.IsPostBack)
      {
        if (args.Result == "yes")
        {
          var str = new ListString(args.Parameters["item"]);
          foreach (string str2 in str)
          {
            Item item2 = VersionManager.GetItemFromStr(str2);
            if (item2 != null)
            {
              VersionManager.DeleteItemVersions(item2);
              Log.Audit(this, "Delete version from item {0}, language \"{1}\"", new[] { item2.Name, item2.Language.ToString() });
            }
          }
          if (!this.isFromCE)
          {
            SheerResponse.SetLocation(string.Empty);
          }
        }
      }
      else
      {
        var str3 = new ListString(args.Parameters["item"]);
        if (str3.Count == 1)
        {
          string str4 = str3[0];
          Item item = VersionManager.GetItemFromStr(str4);
          if (item == null)
          {
            SheerResponse.Alert("Item not found.", new string[0]);
            return;
          }

          SheerResponse.Confirm(Translate.Text("Are you sure you want to delete all obsolete versions of the \"{1}\" in \"{0}\" language? Only the latest {2} versions will be kept.", new object[] { item.Language.ToString(), item.Name, Settings.GetSetting("VersionManager.NumberOfVersionsToKeep") }));
        }
        else
        {
          SheerResponse.Confirm(Translate.Text("Are you sure you want to delete versions of these {0} items?", new object[] { str3.Count }));
        }

        args.WaitForPostBack();
      }
    }

    #endregion
  }
}