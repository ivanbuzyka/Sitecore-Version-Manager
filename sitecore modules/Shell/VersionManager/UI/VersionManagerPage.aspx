<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/sitecore modules/Shell/VersionManager/UI/VersionManagerPage.cs"
    Inherits="Sitecore.VersionManager.VersionManagerPage" %>

<%@ Register Assembly="Sitecore.Kernel" Namespace="Sitecore.Web.UI.HtmlControls"
    TagPrefix="sc" %>
<%@ Register Assembly="Sitecore.Kernel" Namespace="Sitecore.Web.UI.WebControls" TagPrefix="sc" %>
<%@ Register Assembly="Sitecore.Kernel" Namespace="Sitecore.Web.UI.WebControls.Ribbons"
    TagPrefix="sc" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ca" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head id="Head1" runat="server">
    <title>Sitecore</title>
    <sc:Stylesheet ID="Stylesheet1" Src="Content Manager.css" DeviceDependant="true"
        runat="server" />
    <sc:Stylesheet ID="Stylesheet2" Src="Ribbon.css" DeviceDependant="true" runat="server" />
    <sc:Stylesheet ID="Stylesheet3" Src="Grid.css" DeviceDependant="true" runat="server" />
    <sc:Script ID="Script1" Src="/sitecore/shell/Controls/InternetExplorer.js" runat="server" />
    <sc:Script ID="Script2" Src="/sitecore/shell/Controls/Sitecore.js" runat="server" />
    <sc:Script ID="Script3" Src="/sitecore/shell/Controls/SitecoreObjects.js" runat="server" />
    <sc:Script ID="Script4" Src="/sitecore/shell/Applications/Content Manager/Content Editor.js"
        runat="server" />
  <style type="text/css">    
    html body
    {
      overflow: hidden;
    }
  </style>
    <script type="text/javascript" language="javascript">
      
      var resizeTimeoutId1;      

      function OnResize() {
         var doc = $(document.documentElement);
         var ribbon = $("RibbonContainer");
         var grid = $("GridContainer");           

         grid.style.height = doc.getHeight() - ribbon.getHeight() + 'px';
         grid.style.width = doc.getWidth() + 'px';
         
         itemsGrid.render();
         
          //to workaround ie bug with resizing
          window.clearTimeout(resizeTimeoutId1); 
          resizeTimeoutId1 = window.setTimeout('itemsGrid.render()',150);          
      }      
     
      window.onresize = OnResize;
    </script>

</head>
<body style="background: transparent">
    <form id="VersionManagerPage" runat="server">   
    <sc:AjaxScriptManager runat="server" />
    <sc:ContinuationManager runat="server" />
    
    <div id="RibbonContainer">
      <sc:Ribbon runat="server" ID="Ribbon1" />
    </div>
    
    <table runat="server" id="Disabled" border="0" cellpadding="0" cellspacing="0"
        style="width: 100%; height: 100%;">
        <tr style="background: white; height:100%;">    
            <td style="text-align: center; font-size:small;"> Version Manager is disabled.                           
            </td>
        </tr>
    </table>
    
    
     <table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">  
        <tr id="itemsgrd">
            <td height="100%" valign="top" style="background:#e9e9e9">
            <sc:Border runat="server" ID="GridContainer">
                <ca:Grid ID="itemsGrid" 
                AutoFocusSearchBox="false" 
                RunningMode="Callback" 
                
                CssClass="Grid"
                
                FillContainer="true" 
                ShowHeader="true" 
                
                HeaderCssClass="GridHeader" 
                FooterCssClass="GridFooter"
                    
                    GroupByCssClass="GroupByCell" GroupByTextCssClass="GroupByText" GroupBySortAscendingImageUrl="group_asc.gif"
                    GroupBySortDescendingImageUrl="group_desc.gif" GroupBySortImageWidth="10" GroupBySortImageHeight="10"
                    GroupingNotificationTextCssClass="GridHeaderText" GroupingPageSize="5" PagerStyle="Slider"
                    PagerTextCssClass="GridFooterText" PagerButtonWidth="41" PagerButtonHeight="22"
                    PagerImagesFolderUrl="/sitecore/shell/themes/standard/componentart/grid/pager/"
                    ShowSearchBox="true" SearchTextCssClass="GridHeaderText" SearchBoxCssClass="SearchBox"
                    SliderHeight="20" SliderWidth="150" SliderGripWidth="9" SliderPopupOffsetX="20"
                    SliderPopupClientTemplateId="SliderTemplate" TreeLineImagesFolderUrl="/sitecore/shell/themes/standard/componentart/grid/lines/"
                    TreeLineImageWidth="22" TreeLineImageHeight="19" PreExpandOnGroup="false" ImagesBaseUrl="/sitecore/shell/themes/standard/componentart/grid/"
                    IndentCellWidth="22" LoadingPanelClientTemplateId="LoadingFeedbackTemplate" LoadingPanelPosition="MiddleCenter"                                     
                    runat="server">
                    <Levels>
                        <ca:GridLevel DataKeyField="itemLang" ShowTableHeading="false" ShowSelectorCells="false"
                            RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif" DataCellCssClass="DataCell"
                            HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" GroupHeadingCssClass="GroupHeading"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="13"
                            SortImageHeight="13">
                            <Columns>
                                <ca:GridColumn DataField="ItemLang" Visible="false" runat="server"/>
                                <ca:GridColumn DataField="Name" AllowGrouping="false" IsSearchable="true" SortedDataCellCssClass="SortedDataCell" HeadingText="Name" runat="server" />
                                <ca:GridColumn DataField="ItemPath" AllowGrouping="true" IsSearchable="true" SortedDataCellCssClass="SortedDataCell" HeadingText="Item Path" runat="server" />
                                <ca:GridColumn DataField="Language" AllowGrouping="true" IsSearchable="true" SortedDataCellCssClass="SortedDataCell" HeadingText="Language" runat="server" />
                                <ca:GridColumn DataField="VersionCount" AllowGrouping="false" IsSearchable="true" SortedDataCellCssClass="SortedDataCell" HeadingText="VersionsCount" runat="server" />
                            </Columns>
                        </ca:GridLevel>
                    </Levels>                    
                    <ClientTemplates>
                        <ca:ClientTemplate ID="LoadingFeedbackTemplate" runat="server">
                            <table cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td style="font-size: 10px;">
                                        <sc:Literal ID="Literal1" Text="Loading..." runat="server" />
                                        ;
                                    </td>
                                    <td>
                                        <img src="/sitecore/shell/themes/standard/componentart/grid/spinner.gif" width="16"
                                            height="16" border="0">
                                    </td>
                                </tr>
                            </table>
                        </ca:ClientTemplate>
                        <ca:ClientTemplate ID="SliderTemplate">
                            <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td>
                                        <div style="padding: 4px; font: 8pt tahoma; white-space: nowrap; overflow: hidden">
                                            ## DataItem.GetMember('Name').Value ##</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 14px; background-color: #666666; padding: 1px 8px 1px 8px; color: white">
                                        ## DataItem.PageIndex + 1 ## / ## itemsGrid.PageCount ##
                                    </td>
                                </tr>
                            </table>
                        </ca:ClientTemplate>
                    </ClientTemplates>
                </ca:Grid>
                </sc:Border>
            </td>
        </tr>        
    </table>
    </form>
</body>
</html>
