//-------------------------------------------------------------------------------------------------
// <copyright file="Refresh.cs" company="Sitecore A/S">
// Copyright (C) 2010 by Sitecore A/S
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Sitecore.VersionManager.Commands
{
    using System;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;
    
    /// <summary>
    /// The class that implements UI refreshing
    /// </summary>
    [Serializable]
    public class Refresh : Command
    {
        #region Public methods
        
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            VersionManager.Refresh();
            SheerResponse.SetLocation(string.Empty);
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
    }
}
