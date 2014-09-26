//-------------------------------------------------------------------------------------------------
// <copyright file="CleanAll.cs" company="Sitecore A/S">
// Copyright (C) 2010 by Sitecore A/S
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sitecore.VersionManager.Commands
{
    using System;
    using System.Collections.Generic;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.Web.UI.WebControls;
    using Sitecore.Web.UI.XamlSharp.Continuations;
    
    /// <summary>
    /// The class that implements
    /// </summary>
    [Serializable]
    
    public class CleanAll : Command, ISupportsContinuation
    {
        #region Public Methods
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            ClientPipelineArgs args = new ClientPipelineArgs();
            ContinuationManager.Current.Start(this, "Run", args);
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

        #region Private Methods
        private void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.IsPostBack)
            {
                if (args.Result == "yes")
                {
                    List<GridItem> itemList = new List<GridItem>();
                    itemList = VersionManager.GetGridItem();
                    
                    ProgressBox.Execute("Clean obsolete versions", "Version Manager", string.Empty, new ProgressBoxMethod(RunSeparateThread), new object[] { itemList });

                    SheerResponse.SetLocation(string.Empty);
                }                
            }
            else
            {
              SheerResponse.Confirm(Translate.Text("Are you sure you want to delete all obsolete versions of all listed items in all languages?") + " " + Translate.Text("Only the latest") +
                " " + Sitecore.Configuration.Settings.GetSetting("VersionManager.NumberOfVersionsToKeep") + " " + Translate.Text("versions will be kept."));
            }

            args.WaitForPostBack();
        }

        private static void RunSeparateThread(params object[] parameters)
        {
          var itemList = parameters[0] as List<GridItem>;
          if (itemList == null)
          {
            return;
          }

          foreach (GridItem gridItem in itemList)
          {
            Item item = VersionManager.GetItemFromStr(gridItem.ItemLang);
            if (item != null)
            {
              VersionManager.DeleteItemVersions(item);
            }
          }
        }
        #endregion
    }
}
