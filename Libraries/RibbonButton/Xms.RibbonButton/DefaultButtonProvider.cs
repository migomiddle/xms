using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Core;
using Xms.Localization.Abstractions;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 默认按钮提供者
    /// </summary>
    public class DefaultButtonProvider : IDefaultButtonProvider
    {
        private readonly ILocalizedTextProvider _loc;

        public DefaultButtonProvider(IAppContext appContext)
        {
            _loc = appContext.GetFeature<ILocalizedTextProvider>();
        }

        public List<Domain.RibbonButton> Get(EntityMaskEnum entityMask)
        {
            List<Domain.RibbonButton> buttons = new List<Domain.RibbonButton>();
            //listhead
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000100"), CssClass = "btn btn-link btn-sm", DisplayOrder = 0, Icon = "glyphicon glyphicon-plus-sign", JsAction = "CreateRecord(true)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["add"], ShowArea = RibbonButtonArea.ListHead });
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000101"), CssClass = "btn btn-link btn-sm", DisplayOrder = 1, Icon = "glyphicon glyphicon-trash", JsAction = "DeleteRecord()", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["delete"], ShowArea = RibbonButtonArea.ListHead });
            //listrow
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000102"), CssClass = "btn btn-link btn-xs", DisplayOrder = 0, Icon = "glyphicon glyphicon-edit", JsAction = "EditRecord(true)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["edit"], ShowArea = RibbonButtonArea.ListRow });
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000103"), CssClass = "btn btn-link btn-xs", DisplayOrder = 1, Icon = "glyphicon glyphicon-trash", JsAction = "DeleteRecord()", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["delete"], ShowArea = RibbonButtonArea.ListRow });
            //form
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000104"), CssClass = "btn btn-link btn-sm", DisplayOrder = 0, Icon = "glyphicon glyphicon-plus-sign", JsAction = "CreateRecord()", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["add"], ShowArea = RibbonButtonArea.Form });
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000105"), CssClass = "btn btn-link btn-sm", DisplayOrder = 1, Icon = "glyphicon glyphicon-floppy-disk", JsAction = "Save()", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["save"], ShowArea = RibbonButtonArea.Form });
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000106"), CssClass = "btn btn-link btn-sm", DisplayOrder = 2, Icon = "glyphicon glyphicon-floppy-disk", JsAction = "SaveAndNew()", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["saveandnew"], ShowArea = RibbonButtonArea.Form });
            if (entityMask == EntityMaskEnum.User)
            {
                string rules = "{\"FormStateRules\":{\"States\":[\"create\"],\"Enabled\":false,\"Visibled\":false}}";
                //share
                buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000107"), CssClass = "btn btn-link btn-sm", DisplayOrder = 6, Icon = "glyphicon glyphicon-share-alt", JsAction = "Sharing(Xms.Page.PageContext.EntityId, Xms.Page.PageContext.RecordId)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["share"], ShowArea = RibbonButtonArea.Form, CommandRules = rules });
                //assign
                buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000108"), CssClass = "btn btn-link btn-sm", DisplayOrder = 7, Icon = "glyphicon glyphicon-user", JsAction = "Assigning(Xms.Page.PageContext.EntityId, Xms.Page.PageContext.RecordId)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["assign"], ShowArea = RibbonButtonArea.Form, CommandRules = rules });
            }
            //delete
            string rules2 = "{\"FormStateRules\":{\"States\":[\"create\"],\"Enabled\":false,\"Visibled\":false}}";
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000109"), CssClass = "btn btn-link btn-sm", DisplayOrder = 3, Icon = "glyphicon glyphicon-trash", JsAction = "DeleteOneRecord()", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["delete"], ShowArea = RibbonButtonArea.Form, CommandRules = rules2 });
            //enabled
            string rules3 = "{\"FormStateRules\":{\"States\":[\"create\"],\"Enabled\":false,\"Visibled\":false},\"ValueRules\":{\"Values\":[{\"Field\":\"statecode\",\"Value\":\"1\"}],\"Enabled\":false,\"Visibled\":false}}";
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000110"), CssClass = "btn btn-link btn-sm", DisplayOrder = 4, Icon = "glyphicon glyphicon-ok-circle", JsAction = "SetRecordState(1)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["enabled"], ShowArea = RibbonButtonArea.Form, CommandRules = rules3 });
            //disabled
            string rules4 = "{\"FormStateRules\":{\"States\":[\"create\"],\"Enabled\":false,\"Visibled\":false},\"ValueRules\":{\"Values\":[{\"Field\":\"statecode\",\"Value\":\"0\"}],\"Enabled\":false,\"Visibled\":false}}";
            buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000111"), CssClass = "btn btn-link btn-sm", DisplayOrder = 5, Icon = "glyphicon glyphicon-ban-circle", JsAction = "SetRecordState(0)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["disabled"], ShowArea = RibbonButtonArea.Form, CommandRules = rules4 });
            ////active
            //string rules5 = "{\"FormStateRules\":[{\"State\":\"create\",\"Enabled\":false,\"Visibled\":false},{\"State\":\"disabled\",\"Enabled\":false,\"Visibled\":true}],\"ValueRules\":[{\"Field\":\"statuscode\",\"Value\":\"1\",\"Enabled\":false,\"Visibled\":false},{\"Field\":\"statuscode\",\"Value\":\"2\",\"Enabled\":false,\"Visibled\":false}],\"PrivilegeRules\":[]}";
            //buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000112"), CssClass = "btn btn-link btn-sm", DisplayOrder = 5, Icon = "glyphicon glyphicon-ok-sign", JsAction = "SetRecordStatus(1)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["status_toactived"], ShowArea = RibbonButtonArea.Form, CommandRules = rules5 });
            ////draft
            //string rules6 = "{\"FormStateRules\":[{\"State\":\"create\",\"Enabled\":false,\"Visibled\":false},{\"State\":\"disabled\",\"Enabled\":false,\"Visibled\":true}],\"ValueRules\":[{\"Field\":\"statuscode\",\"Value\":\"0\",\"Enabled\":false,\"Visibled\":false}],\"PrivilegeRules\":[]}";
            //buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000113"), CssClass = "btn btn-link btn-sm", DisplayOrder = 5, Icon = "glyphicon glyphicon-eye-open", JsAction = "SetRecordStatus(0)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["status_todraft"], ShowArea = RibbonButtonArea.Form, CommandRules = rules6 });
            ////invalid
            //string rules7 = "{\"FormStateRules\":[{\"State\":\"create\",\"Enabled\":false,\"Visibled\":false},{\"State\":\"disabled\",\"Enabled\":false,\"Visibled\":true}],\"ValueRules\":[{\"Field\":\"statuscode\",\"Value\":\"2\",\"Enabled\":false,\"Visibled\":false}],\"PrivilegeRules\":[]}";
            //buttons.Add(new Domain.RibbonButton() { RibbonButtonId = new Guid("00000000-0000-0000-0000-130000000114"), CssClass = "btn btn-link btn-sm", DisplayOrder = 5, Icon = "glyphicon glyphicon-eye-close", JsAction = "SetRecordStatus(2)", JsLibrary = SystemRibbonButtonJsLib.ENTITY, Label = _loc["status_toinvalid"], ShowArea = RibbonButtonArea.Form, CommandRules = rules7 });
            buttons.ForEach((b) =>
            {
                b.ShowLabel = true;
                b.StateCode = RecordState.Enabled;
            });
            return buttons;
        }
    }
}