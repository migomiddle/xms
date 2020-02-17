if (typeof (Xms) == "undefined") { Xms = { __namespace: true }; }
Xms.Form = function () { };
Xms.Form.FormControlType = {
    none: -1,
    standard: 0,
    hidden: 1,
    iFrame: 2,
    lookup: 3,
    optionSet: 4,
    subGrid: 5,
    webResource: 6,
    freeText: 7,
    label: 11
};
//models
Xms.Form.FormDescriptor = function () {
    var self = new Object();
    self.Name = '';
    self.Description = '';
    self.IsShowNav = true;
    self.Header = null;
    self.Footer = null;
    self.NavGroups = [];
    self.Panels = [];
    self.Sections = [];
    self.ClientResources = [];
    self.Events = [];
    return self;
};
Xms.Form.NavGroupDescriptor = function () {
    var self = new Object();
    self.Label = '';
    self.IsVisible = true;
    self.NavItems = [];
    return self;
};
Xms.Form.NavDescriptor = function () {
    var self = new Object();
    self.Id = Xms.Utility.Guid.NewGuid().ToString();
    self.Label = '';
    self.IsVisible = true;
    self.Icon = '';
    self.Url = '';
    self.RelationshipName = '';
    return self;
};
Xms.Form.PanelDescriptor = function () {
    var self = new Object();
    self.Id = Xms.Utility.Guid.NewGuid().ToString();
    self.CellLabelSettings = null;
    self.Name = '';
    self.Label = '';
    self.IsExpanded = true;
    self.IsShowLabel = true;
    self.IsVisible = true;
    self.Sections = [];
    return self;
};
Xms.Form.SectionDescriptor = function () {
    var self = new Object();
    self.Id = Xms.Utility.Guid.NewGuid().ToString();
    self.Name = '';
    self.Label = '';
    self.IsShowLabel = true;
    self.IsVisible = true;
    self.Columns = 2;
    self.CellLabelWidth = 100;
    self.CellLabelAlignment = 'left';
    self.CellLabelPosition = 'left';
    self.Rows = [];
    return self;
};
Xms.Form.RowDescriptor = function () {
    var self = new Object();
    self.IsVisible = true;
    self.Cells = [];
    return self;
};
Xms.Form.CellDescriptor = function () {
    var self = new Object();
    self.Id = Xms.Utility.Guid.NewGuid().ToString();
    self.Label = '';
    self.IsShowLabel = true;
    self.IsVisible = true;
    self.ColSpan = 0;
    self.RowSpan = 0;
    self.Control = new Xms.Form.ControlDescriptor();
    return self;
};
Xms.Form.ControlDescriptor = function () {
    var self = new Object();
    self.Name = '';
    self.EntityName = '';
    self.ControlType = Xms.Form.FormControlType.standard;
    self.ReadOnly = false;
    self.Parameters = null;
    self.Formula = null;
    return self;
};
Xms.Form.ExtendPropertyParameters = function () {
    var self = new Object();
    self.EntityName = null;
    self.AttributeName = null;
    self.SourceAttributeName = null;
    self.SourceAttributeType = null;
    self.Formula = null;
    return self;
};
Xms.Form.SubGridParameters = function () {
    var self = new Object();
    self.ViewId = null;
    self.PageSize = 5;
    self.TargetEntityName = null;
    self.RelationshipName = null;
    self.Editable = false;
    self.FieldEvents = [];
    return self;
};
Xms.Form.SubGridFormulars = function () {
    var self = new Object();
    self.name = '';
    self.type = '';
    self.expression = '';
    return self;
};
Xms.Form.LookUpParameters = function () {
    var self = new Object();
    self.DefaultViewReadOnly = null;
    self.DefaultViewId = null;
    self.ViewPickerReadOnly = null;
    self.DisableViewPicker = null;
    self.FilterRelationshipName = null;
    self.DependentAttributeName = null;
    self.DependentAttributeType = null;
    self.AllowFilterOff = false;
    return self;
};
Xms.Form.IframeParameters = function () {
    var self = new Object();
    self.Url = null;
    self.Border = null;
    self.Scroll = null;
    self.AllowCrossDomain = null;
    self.RowSize = null;
    return self;
};
Xms.Form.PresentationParameters = function () {
    var self = new Object();
    self.ItemColor = null;
    self.Series = [];
    return self;
};
Xms.Form.ChartsParameters = function () {
    var self = new Object();
    self.Type = null;
    self.Fetch = [];
    return self;
};
Xms.Form.ChartsAxisParameters = function () {
    var self = new Object();
    self.Attribute = null;
    self.Type = null;
    self.Aggregate = null;
    self.Dategrouping = null;
    self.Groupby = null;
    self.IgnoreNull = null;
    self.TopDirection = null;
    self.TopCount = 0;
    return self;
};
Xms.Form.WebSourceParameters = function () {
    var self = new Object();
    return self;
};
Xms.Form.EventDescriptor = function () {
    var self = new Object();
    self.Name = '';
    self.Application = true;
    self.Active = true;
    self.Attribute = '';
    self.JsAction = '';
    self.JsLibrary = '';
    return self;
};

//const
Xms.AttributeFormat = function () { };
Xms.AttributeType = function () { };
Xms.BooleanFormat = function () { };
Xms.ControlType = function () { };
Xms.FormSaveAction = function () { };
Xms.RequiredLevel = function () { };
Xms.SubmitMode = function () { };
Xms.TabDisplayState = function () { };
Xms.AttributeFormat.dateFormat = "date";
Xms.AttributeFormat.dateTimeFormat = "datetime";
Xms.AttributeFormat.durationFormat = "duration";
Xms.AttributeFormat.emailFormat = "email";
Xms.AttributeFormat.languageFormat = "language";
Xms.AttributeFormat.noneFormat = "none";
Xms.AttributeFormat.textFormat = "text";
Xms.AttributeFormat.textAreaFormat = "textarea";
Xms.AttributeFormat.tickerSymbolFormat = "tickersymbol";
Xms.AttributeFormat.timeZoneFormat = "timezone";
Xms.AttributeFormat.urlFormat = "url";
Xms.AttributeType.booleanType = "boolean";
Xms.AttributeType.dateTimeType = "datetime";
Xms.AttributeType.decimalType = "decimal";
Xms.AttributeType.doubleType = "double";
Xms.AttributeType.integerType = "integer";
Xms.AttributeType.lookupType = "lookup";
Xms.AttributeType.memoType = "memo";
Xms.AttributeType.moneyType = "money";
Xms.AttributeType.optionSetType = "optionset";
Xms.AttributeType.stringType = "string";
Xms.BooleanFormat.checkBox = "checkbox";
Xms.BooleanFormat.dropDown = "dropdown";
Xms.BooleanFormat.radioButton = "radiobutton";
Xms.ControlType.hidden = "hidden";
Xms.ControlType.iFrame = "iframe";
Xms.ControlType.lookup = "lookup";
Xms.ControlType.none = "none";
Xms.ControlType.optionSet = "optionset";
Xms.ControlType.standard = "standard";
Xms.ControlType.subGrid = "subgrid";
Xms.ControlType.webResource = "webresource";
Xms.FormSaveAction.save = "save";
Xms.FormSaveAction.saveAndClose = "saveandclose";
Xms.FormSaveAction.saveAndNew = "saveandnew";
Xms.RequiredLevel.none = "none";
Xms.RequiredLevel.recommended = "recommended";
Xms.RequiredLevel.required = "required";
Xms.SubmitMode.dirty = "dirty";
Xms.SubmitMode.always = "always";
Xms.SubmitMode.never = "never";
Xms.TabDisplayState.collapsed = "collapsed";
Xms.TabDisplayState.expanded = "expanded";

Xms.Form.RenderCell = function (columns, fill) {
    var _control = new Array();
    for (var i = 1; i <= fill; i++) {
        _control.push('<th class="col-md-2"></th><td></td>');
    }
    return _control.join('');
}
Xms.Form.RenderControl = function (columns, _cell, value, onlyLabel) {
    value = (value == undefined || value == null) ? '' : value;
    onlyLabel = onlyLabel || '';
    var _control = new Array();
    if (_cell.Control.Name.toLowerCase() == 'createdon') {
        _cell.Control.ReadOnly = true;
        if (value == '') {
            value = new Date().format('yyyy/MM/dd hh:mm:ss');
        }
    }
    var colHeaderWidth = 'col-sm-2', colContentWidth = 'col-sm-4';
    if (columns == 1) {
        colContentWidth = 'col-sm-10';
    }
    var isrequired = _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.IsRequired;
    var required = isrequired ? ' required' : '';
    var readonly = _cell.Control.ReadOnly ? ' disabled' : '';
    var visible = _cell.IsVisible ? '' : ' hide';
    var colspanAttr = (_cell.ColSpan && _cell.ColSpan > 0 ? 'colspan="' + (_cell.ColSpan + 1) + '"' : '');
    if (_cell.IsShowLabel == true) {
        _control.push('<th class="' + colHeaderWidth + '"><label for="' + _cell.Control.Name + '">' + _cell.Label);
        if (required != '') {
            _control.push('<span style="color:red;font-weight:bolder;">*</span>');
        }
        _control.push('</label></th>');
    }
    else {
        //colspanAttr += 1;
    }
    _control.push('<td class="' + colContentWidth + '" ' + colspanAttr + '>');
    if (_cell.Control.ControlType == Xms.Form.FormControlType.subGrid || _cell.Control.ControlType.toLowerCase() == 'subgrid') {
        _control.push('<div id="' + _cell.Control.Name + '" name="' + _cell.Control.Name + '" class="subgrid" data-controltype="subgrid" data-url="' + ORG_SERVERURL + '/entity/rendergridview?queryid=' + _cell.Control.Parameters.ViewId + '" data-pagesize="' + _cell.Control.Parameters.PageSize + '" data-relationshipname="' + _cell.Control.Parameters.RelationshipName + '" data-editable="' + _cell.Control.Parameters.Editable + '"></div>');
    }
    else if (_cell.Control.ControlType == Xms.Form.FormControlType.iFrame || _cell.Control.ControlType.toLowerCase() == 'iframe') {
        _control.push('<iframe src="' + _cell.Control.Parameters.Url + '" frameborder="' + _cell.Control.Parameters.Border + '" style="width:100%;height:100%;"></iframe>');
    }
    else if (_cell.Control.ControlType == Xms.Form.FormControlType.webResource || _cell.Control.ControlType.toLowerCase() == 'webresource') {
        var url = ORG_SERVERURL + '/api/webresource?ids=' + _cell.Control.Parameters;
        _control.push('<div id="' + _cell.Control.Name + '" name="' + _cell.Control.Name + '" class="webresource" data-controltype="webresource" data-url="' + url + '"></div>');
    }
    else if (_cell.Control.ControlType.toLowerCase() == "label") {
    }
    else if (_cell.Control.ControlType == Xms.Form.FormControlType.none || _cell.Control.ControlType.toLowerCase() == 'none') {
    }
    else {
        var attrType = _cell.Control.AttributeMetadata.AttributeTypeName || '';
        switch (attrType) {
            case "ntext":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<textarea rows="5" class="form-control input-sm ntext  ntext' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-onlylabel="' + onlyLabel + '" value="' + value + '"' + readonly + '>' + value + '</textarea>');
                }
                break;
            case "nvarchar":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm  nvarchar' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-onlylabel="' + onlyLabel + '" value="' + value + '"' + readonly + ' />');
                }
                break;
            case "int":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm int' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="int" data-onlylabel="' + onlyLabel + '" value="' + value + '"' + readonly + ' />');
                }
                break;
            case "float":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm float' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="number" data-onlylabel="' + onlyLabel + '" value="' + value + '"' + readonly + ' />');
                }
                break;
            case "money":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm money' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="number" data-onlylabel="' + onlyLabel + '" value="' + value + '"' + readonly + ' />');
                }
                break;
            case "datetime":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm datetime datepicker' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="date" data-onlylabel="' + onlyLabel + '" value="' + value + '"' + readonly + ' />');
                }
                break;
            case "lookup":
                _control.push('<input type="text" class="form-control input-sm lookup' + required + '" name="' + _cell.Control.Name.toLowerCase() + '_text" id="' + _cell.Control.Name.toLowerCase() + '_text" data-controltype="' + attrType + '" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-value="' + value + '" data-onlylabel="' + onlyLabel + '"' + readonly + ' />');
                _control.push('<input type="hidden" class="' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="guid" value="' + value + '" />');
                break;
            case "owner":
                _control.push('<input type="text" class="form-control input-sm owner lookup' + required + '" name="' + _cell.Control.Name.toLowerCase() + '_text" id="' + _cell.Control.Name.toLowerCase() + '_text" data-controltype="' + attrType + '" data-type="guid" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-value="' + value + '" data-onlylabel="' + onlyLabel + '"' + readonly + ' />');
                _control.push('<input type="hidden" class="' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="guid" value="' + value + '" />');
                break;
            case "customer":
                _control.push('<input type="text" class="form-control input-sm customer lookup' + required + '" name="' + _cell.Control.Name.toLowerCase() + '_text" id="' + _cell.Control.Name.toLowerCase() + '_text" data-controltype="' + attrType + '" data-type="guid" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-value="' + value + '" data-onlylabel="' + onlyLabel + '"' + readonly + ' />');
                _control.push('<input type="hidden" class="' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="guid" value="' + value + '" />');
                break;
            case "picklist":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm picklist' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '_text" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.OptionSet.Items)) + '" value="' + value + '" data-onlylabel="' + onlyLabel + '"' + readonly + ' />');
                }
                break;
            case "status":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm status picklist' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '_text" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.OptionSet.Items)) + '" value="' + value + '" data-onlylabel="' + onlyLabel + '"' + readonly + ' />');
                }
                break;
            case "bit":
                if (onlyLabel) {
                    var text = value;
                    if (_cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.PickLists) {
                        var a = $.grep(_cell.Control.AttributeMetadata.PickLists, function (n, i) {
                            return n.Value == value;
                        });
                        if (a && a.length > 0) text = a[0].Name;
                    }
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '" data-attributeid="' + attrId + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm bit' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.PickLists)) + '" data-onlylabel="' + onlyLabel + '" value="' + (value ? 1 : 0) + '"' + readonly + ' />');
                }
                break;
            case "state":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '">' + value + '</span>');
                }
                else {
                    _control.push('<input type="text" class="form-control input-sm state bit' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.PickLists)) + '" data-onlylabel="' + onlyLabel + '" value="' + (value ? 1 : 0) + '"' + readonly + ' />');
                }
                break;
        }
    }
    _control.push('</td>');
    return _control.join('');
}
Xms.Form.RenderSubGrid = function (columns, _cell, value) {
    var _html = [];
}
//create a form
Xms.Form.CreateForm = function (_form, _record, _container) {
    var _html = new Array();
    if (_form.IsShowNav) {
        //render nav
    }
    //render header
    var _header = new Array();
    var _headerSection = _form.Header;
    if (_headerSection != null) {
        var isvisible = _headerSection.IsVisible ? '' : 'hide';
        _header.push('<div class="header ' + isvisible + '" style="border:none;">');
        _header.push('<table class="table">');
        $(_headerSection.Rows).each(function (i, _row) {
            _header.push('<tr>');
            $(_row.Cells).each(function (j, _cell) {
                var value = null;
                if (_record && _record != null
                    && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                        || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup')) {
                    value = eval('_record.' + _cell.Control.Name.toLowerCase());
                }
                _header.push(Xms.Form.RenderControl(_headerSection.Columns, _cell, value, true));
            });
            if (_headerSection.Columns - _row.Cells.length > 0) {
                _header.push(Xms.Form.RenderCell(_headerSection.Columns, _headerSection.Columns - _row.Cells.length));
            }
            _header.push('</tr>');
        });
        _header.push('</table>');
        _header.push('</div>');
        _html.push(_header.join(''));
    }
    var _body = new Array();
    //render tabs
    _body.push('<div class="body">');
    if (_form.Panels != null) {
        console.log("panel:", _form.Panels)
        $(_form.Panels).each(function (a, _tab) {
            var tabid = 'tab_' + (parseInt(Math.random() * 100000000));
            console.log(_tab.IsShowLabel);
            if (_tab.DisplayStyle == '1' || _tab.DisplayStyle == "true" || _tab.DisplayStyle == "True") {
                _body.push('<div class="tab formTab">');
            } else {
                _body.push('<div class="tab">');
            }
            _body.push('<a data-toggle="collapse"');
            _body.push(' href="javascript:;" class="collapse-title" data-target="#' + tabid + '">');
            _body.push('<span class="glyphicon glyphicon-chevron-down"></span> ' + _tab.Label);
            _body.push('</a>');
            _body.push('<div id="' + tabid + '" class="panel-collapse collapse ' + (_tab.IsExpanded ? 'in' : '') + '">');
            $(_tab.Sections).each(function (b, _section) {
                _body.push('<div class="section">');
                if (_section.IsShowLabel) {
                    _body.push('<div class="section-title">' + _section.Label + '</div>');
                }
                _body.push('<table class="table table-bordered">');
                $(_section.Rows).each(function (c, _row) {
                    _body.push('<tr>');
                    var colspanCount = 0;
                    $(_row.Cells).each(function (d, _cell) {
                        var value = null;
                        if (_record && _record != null
                            && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                                || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup')) {
                            value = eval('_record.' + _cell.Control.Name.toLowerCase());
                        }
                        _body.push(Xms.Form.RenderControl(_section.Columns, _cell, value));
                        colspanCount += _cell.ColSpan;
                    });
                    var fillCount = _section.Columns - (_row.Cells.length + colspanCount);
                    if (fillCount > 0) {
                        _body.push(Xms.Form.RenderCell(_section.Columns, fillCount));
                    }
                    _body.push('</tr>');
                });
                _body.push('</table>');
                _body.push('</div>');
            });
            _body.push('</div>');
            _body.push('</div>');
        });
    }
    //render sections
    if (_form.Sections != null) {
        $(_form.Sections).each(function (b, _section) {
            _body.push('<div class="section">');
            if (_section.IsShowLabel) {
                _body.push('<div class="section-title">' + _section.Label + '</div>');
            }
            _body.push('<table class="table">');
            $(_section.Rows).each(function (c, _row) {
                _body.push('<tr>');
                var colspanCount = 0;
                $(_row.Cells).each(function (d, _cell) {
                    var value = null;
                    if (_record && _record != null
                        && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                            || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup')) {
                        value = eval('_record.' + _cell.Control.Name.toLowerCase());
                    }
                    _body.push(Xms.Form.RenderControl(_section.Columns, _cell, value));
                    colspanCount += _cell.ColSpan;
                });
                var fillCount = _section.Columns - (_row.Cells.length + colspanCount);
                if (fillCount > 0) {
                    _body.push(Xms.Form.RenderCell(_section.Columns, fillCount));
                }
                _body.push('</tr>');
            });
            _body.push('</table>');
            _body.push('</div>');
        });
        _body.push('</div>');
    }

    _html.push(_body.join(''));

    //render footer
    var _footer = new Array();
    var _footerSection = _form.Footer;
    if (_footerSection != null) {
        var isvisible = _footer.IsVisible ? '' : 'hide';
        _footer.push('<nav class="navbar navbar-default navbar-fixed-bottom ' + isvisible + '" role="navigation" style="margin:0;">');
        _footer.push('<table class="table">');
        $(_footerSection.Rows).each(function (i, _row) {
            _footer.push('<tr>');
            $(_row.Cells).each(function (j, _cell) {
                var value = null;
                if (_record && _record != null
                    && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                        || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup')) {
                    value = eval('_record.' + _cell.Control.Name.toLowerCase());
                }
                _footer.push(Xms.Form.RenderControl(_footerSection.Columns, _cell, value, true));
            });
            if (_footerSection.Columns - _row.Cells.length > 0) {
                _footer.push(Xms.Form.RenderCell(_footerSection.Columns, _footerSection.Columns - _row.Cells.length));
            }
            _footer.push('</tr>');
        });
        _footer.push('</table>');
        _footer.push('</nav>');
        _html.push(_footer.join(''));
    }
    //rendering
    _container.append(_html.join(''));
}
var _labelSettings = { align: ['Center', 'Left', 'Right'], position: ['Left', 'Top'] };

Xms.Form.CreateFormDiv = function (_form, _record, _container) {
    var _html = new Array();
    if (_form.IsShowNav) {
        //render nav
    }
    //render header
    // console.log(_form)
    var _header = new Array();
    var _headerSection = _form.Header;
    if (_headerSection != null) {
        var isvisible = _headerSection.IsVisible ? '' : 'hide';
        _header.push('<div class="header ' + isvisible + '" style="border:none;">');
        var celllabelPos = '';
        if (_headerSection && typeof _headerSection.CellLabelSettings == 'undefined') {
            _headerSection.CellLabelSettings = {
                Width: '115',
                Alignment: '1',
                Position: '0'
            };
        }
        if (_labelSettings.position[_headerSection.CellLabelSettings.Position] == 'Top') {
            celllabelPos = ' section-topw';
        }
        if (_headerSection.CellLabelSettings.Width != '') {
            cellwidthSetion = ' section-table';
        }
        _header.push('<div class="section ' + celllabelPos + ' ' + cellwidthSetion + '">');
        _header.push('<div class="row container-fluid">');
        $(_headerSection.Rows).each(function (i, _row) {
            _header.push('<div class="form-group">');
            $(_row.Cells).each(function (j, _cell) {
                var value = null;
                if (_record && _record != null
                    && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                        || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup')) {
                    value = eval('_record.' + _cell.Control.Name.toLowerCase());
                }
                _header.push(Xms.Form.RenderControlDiv(_headerSection.Columns, _cell, value, true, null, null, true));
            });
            if (_headerSection.Columns - _row.Cells.length > 0) {
                _header.push(Xms.Form.RenderCell(_headerSection.Columns, _headerSection.Columns - _row.Cells.length));
            }
            _header.push('</div>');
        });
        _header.push('</div>');
        _header.push('</div>');
        _header.push('</div>');
        _html.push(_header.join(''));
    }
    var _body = new Array();
    //render tabs
    _body.push('<div class="body">');
    if (_form.Panels != null) {
        $(_form.Panels).each(function (a, _tab) {
            var tabid = 'tab_' + _tab.Id;

            var _tabShow = ' display:none';
            if (_tab.IsVisible) {
                _tabShow = '';
            }
            // true True为了兼容旧数据
            if (_tab.DisplayStyle == "1" || _tab.DisplayStyle == "true" || _tab.DisplayStyle == "True") {
                if (_tab.IsShowLabel) {
                    if (_tab.Async == true) {
                        _body.push('<div class="tab formTab asyncTab" data-tabid="' + _tab.Id + '">');
                    } else {
                        _body.push('<div class="tab formTab noasyncTab" data-tabid="' + _tab.Id + '">');
                    }
                }
            } else {
                _body.push('<div class="tab" data-tabid="' + _tab.Id + '" style="' + _tabShow + '">');
            }
            _body.push('<a data-toggle="collapse"');
            _body.push(' href="javascript:;" class="collapse-title" id="formTab_' + _tab.Id + '" data-target="#' + tabid + '">');
            _body.push('<span class="glyphicon glyphicon-chevron-down"></span> ' + _tab.Label);
            _body.push('</a>');
            _body.push('<div id="' + tabid + '" class="panel-collapse collapse ' + (_tab.IsExpanded ? 'in' : '') + '">');
            $(_tab.Sections).each(function (b, _section) {
                var _sectionShow = ' display:none';
                if (_section.IsVisible) {
                    _sectionShow = '';
                }
                var celllabelPos = '';
                if (_section && typeof _section.CellLabelSettings == 'undefined') {
                    _section.CellLabelSettings = {
                        Width: '115',
                        Alignment: '1',
                        Position: '0'
                    };
                }
                if (_labelSettings.position[_section.CellLabelSettings.Position] == 'Top') {
                    celllabelPos = ' section-topw';
                }
                if (_section.CellLabelSettings.Width != '') {
                    cellwidthSetion = ' section-table';
                }
                _body.push('<div class="section ' + celllabelPos + ' ' + cellwidthSetion + '" style="' + _sectionShow + '">');
                if (_section.IsShowLabel) {
                    _body.push('<div class="section-title">' + _section.Label + '</div>');
                }
                _body.push('<div class="row container-fluid">');
                $(_section.Rows).each(function (c, _row) {
                    _body.push('<div class="form-group">');
                    var colspanCount = 0;
                    $(_row.Cells).each(function (d, _cell) {
                        _cell.Control.Formula && formula.transfer(JSON.parse(_cell.Control.Formula));

                        var value = null;
                        if (_record && _record != null
                            && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                                || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup'
                            )) {
                            value = eval('_record.' + _cell.Control.Name.toLowerCase());
                        }
                        _body.push(Xms.Form.RenderControlDiv(_section.Columns, _cell, value, undefined, _section, _tab));
                        colspanCount += _cell.ColSpan;
                    });
                    var fillCount = _section.Columns - (_row.Cells.length + colspanCount);
                    if (fillCount > 0) {
                        _body.push(Xms.Form.RenderCell(_section.Columns, fillCount));
                    }
                    _body.push('</div>');
                });
                _body.push('</div>');
                _body.push('</div>');
            });
            _body.push('</div>');
            _body.push('</div>');
        });
    }
    //render sections
    if (_form.Sections != null) {
        $(_form.Sections).each(function (b, _section) {
            var _sectionShow = ' display:none';
            if (_section.IsVisible) {
                _sectionShow = '';
            }
            var celllabelPos = '';
            var cellwidthSetion = '';
            if (_section && typeof _section.CellLabelSettings == 'undefined') {
                _section.CellLabelSettings = {
                    Width: '115',
                    Alignment: '1',
                    Position: '0'
                };
            }
            if (_labelSettings.position[_section.CellLabelSettings.Position] == 'Top') {
                celllabelPos = ' section-topw';
            }
            if (_section.CellLabelSettings.Width != '') {
                cellwidthSetion = ' section-table';
            }
            _body.push('<div class="section ' + celllabelPos + ' ' + cellwidthSetion + '" style="' + _sectionShow + '">');
            if (_section.IsShowLabel) {
                _body.push('<div class="section-title">' + _section.Label + '</div>');
            }
            _body.push('<div class="row">');
            $(_section.Rows).each(function (c, _row) {
                _body.push('<tr>');
                var colspanCount = 0;
                $(_row.Cells).each(function (d, _cell) {
                    var value = null;
                    if (_record && _record != null
                        && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                            || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup')) {
                        value = eval('_record.' + _cell.Control.Name.toLowerCase());
                    }
                    _body.push(Xms.Form.RenderControlDiv(_section.Columns, _cell, value, undefined, _section, _tab));
                    colspanCount += _cell.ColSpan;
                });
                var fillCount = _section.Columns - (_row.Cells.length + colspanCount);
                if (fillCount > 0) {
                    _body.push(Xms.Form.RenderCell(_section.Columns, fillCount));
                }
                _body.push('</div>');
            });
            _body.push('</div>');
            _body.push('</div>');
        });
        _body.push('</div>');
    }

    _html.push(_body.join(''));

    //render footer
    var _footer = new Array();
    var _footerSection = _form.Footer;
    if (_footerSection != null && _footerSection.IsVisible) {
        var isvisible = _footerSection.IsVisible ? '' : 'hide';
        _footer.push('<nav class="navbar navbar-default navbar-fixed-bottom ' + isvisible + '" role="navigation" id="xmsFormFooter" style="margin:0;">');
        if (_footerSection && typeof _footerSection.CellLabelSettings == 'undefined') {
            _footerSection.CellLabelSettings = {
                Width: '115',
                Alignment: '1',
                Position: '0'
            };
        }
        if (_labelSettings.position[_footerSection.CellLabelSettings.Position] == 'Top') {
            celllabelPos = ' section-topw';
        }
        if (_footerSection.CellLabelSettings.Width != '') {
            cellwidthSetion = ' section-table';
        }
        _footer.push('<div class="section ' + celllabelPos + ' ' + cellwidthSetion + '">');
        _footer.push('<div class="row container-fluid">');
        $(_footerSection.Rows).each(function (i, _row) {
            _footer.push('<div style="line-height:25px;">');
            $(_row.Cells).each(function (j, _cell) {
                var value = null;
                if (_record && _record != null
                    && (_cell.Control.ControlType == Xms.Form.FormControlType.standard || _cell.Control.ControlType == Xms.Form.FormControlType.lookup
                        || _cell.Control.ControlType.toLowerCase() == 'standard' || _cell.Control.ControlType.toLowerCase() == 'lookup')) {
                    value = eval('_record.' + _cell.Control.Name.toLowerCase());
                }
                _footer.push(Xms.Form.RenderControlDiv(_footerSection.Columns, _cell, value, true, null, null, true));
            });
            if (_footerSection.Columns - _row.Cells.length > 0) {
                _footer.push(Xms.Form.RenderCell(_footerSection.Columns, _footerSection.Columns - _row.Cells.length));
            }
            _footer.push('</div>');
        });
        _footer.push('</div>');
        _footer.push('</div>');
        _footer.push('</nav>');
        _html.push(_footer.join(''));
    }
    //rendering
    _container.append(_html.join(''));
}
Xms.Form.RenderCellDiv = function (columns, fill) {
    var _control = new Array();
    for (var i = 1; i <= fill; i++) {
        _control.push('<div class="col-md-2"></div><div></div>');
    }
    return _control.join('');
}

Xms.Form.getTabIsAsync = function (tabs) {
    var index = -1;
    $.each(tabs, function (key, item) {
        if (item.Async == true) {
            index = key;
            return false;
        }
    });
    return index;
}
Xms.Form.RenderControlDiv = function (columns, _cell, value, onlyLabel, _section, _tab, isheaderfooter) {
    // if (!pos) return false;
    var labelWidth = '115';
    var labelAlign = 'right';
    var labelPosition = 'left';

    if (_section) {
        labelWidth = _section.CellLabelSettings.Width;
        labelAlign = _labelSettings.align[_section.CellLabelSettings.Alignment].toLowerCase();
        labelPosition = _labelSettings.align[_section.CellLabelSettings.Position].toLowerCase();
    }

    //console.log(_section && _section.CellLabelSettings)
    value = (value == undefined || value == null) ? '' : value;
    onlyLabel = onlyLabel || '';
    columns = columns || 1;
    var _control = new Array();
    if (_cell.Control.Name && (_cell.Control.Name.toLowerCase() == 'createdon' || _cell.Control.Name.toLowerCase() == 'modifiedon')) {
        _cell.Control.ReadOnly = true;
        if (value == '') {
            value = new Date().format('yyyy/MM/dd hh:mm:ss');
        }
    }
    //console.log(_cell)
    var colArr = [{ h: 1, c: 11, n: 12 }, { h: 1, c: 5, n: 6 }, { h: 1, c: 3, n: 4 }, { h: 1, c: 2, n: 3 }];//所占列宽

    var colspans = _cell.ColSpan || 1;
    var colHeaderWidth = '', colContentWidth = '';
    colHeaderWidth = 'col-sm-' + colArr[columns - 1].h;
    colContentWidth = 'col-sm-' + (colArr[columns - 1].c * (colspans) + (colArr[columns - 1].h * colspans - 1));
    if (columns == 1) {
        colHeaderWidth = 'col-sm-' + colArr[columns - 1].h;
        colContentWidth = 'col-sm-' + colArr[columns - 1].c;
    }
    var isrequired = _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.IsRequired;
    var required = (isrequired && !isheaderfooter) ? ' required' : '';
    var readonly = _cell.Control.ReadOnly ? ' readonly' : '';
    var isvalidate = '';
    if (readonly == 'readonly') {
        isvalidate = '.readonly';
    }

    var visible = _cell.IsVisible ? '' : ' hide';
    var issecured = _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.IsSecured;
    var colspanAttr = (colspans && colspans > 0 ? 'colspan="' + (colspans + 1) + '"' : '');
    var inner_Width = 'width:' + labelWidth + "px; ";
    var cellCols = parseInt(12 / columns) * colspans;
    if (cellCols == 0) { cellCols = 1 }
    if (cellCols > 12) { cellCols = 12 };
    var issubgrid = (_cell.Control.ControlType == Xms.Form.FormControlType.subGrid || _cell.Control.ControlType.toLowerCase() == 'subgrid');
    var _subgirdCl = '';
    if (issubgrid) {
        _subgirdCl = ' subgrid-wrap';
    }

    //样式配置信息
    var cell_styles = null;
    var labelstyle = null;
    var inputstyle = null;
    if (_cell.CustomCss && _cell.CustomCss != "") {
        if (_cell.CustomCss.length > 1) {//新增实体保存后数据会有 {，未解决
            var cell_styles = JSON.parse(_cell.CustomCss);
            labelstyle = cell_styles.labels;
            inputstyle = cell_styles.inputs;
            if (!pageWrap_Create.cellsStyles) {
                pageWrap_Create.cellsStyles = [];
            }
        }
    }

    _control.push('<div class="' + ("col-sm-" + cellCols) + ' form-lists ' + _subgirdCl + '" data-columns="' + columns + '" data-colspan="' + colspans + '" >');
    _control.push('<div class="form-items-row" >');
    if (_cell.IsShowLabel == true) {
        _control.push('<div class="form-cell-label' + visible + ' ' + ("text-" + labelAlign) + '" style="' + inner_Width + (labelstyle ? $.changeobjtoStyle(labelstyle) : '') + '" >');
        if (issecured) {
            _control.push('<span class="glyphicon glyphicon-loc"></span>');
        }
        if (_cell.Control.Name && _cell.Control.Name != " " && _cell.Control.ControlType.toLowerCase() !== "none") {
            _control.push('<label for="' + _cell.Control.Name.toLowerCase() + '" style="' + (labelstyle ? $.changeobjtoStyle({ 'font-size': labelstyle['font-size'] }) : '') + '">' + _cell.Label);
        }
        if (required != '') {
            _control.push('<span style="color:red;font-weight:bolder;">*</span>');
        }
        _control.push('</label>');
        _control.push('</div>');
    }
    else {
        //colspanAttr += 1;
        if (columns == 1) {
            colContentWidth = 'col-sm-' + colArr[columns - 1].n;
        } else {
            colContentWidth = 'col-sm-' + colArr[columns - 1].n * colspans;
        }
    }

    var cellMargin_left = ''//'margin-left:' + (labelWidth*1+8) + "px; ";
    // var cellParams = JSON.stringify(_cell);
    _control.push('<div class="form-cell-ctrl' + visible + '" ' + colspanAttr + ' style="' + cellMargin_left + '">');
    //_control.push('<div class="input-group">');
    // _control.push('<textarea type="hidden" id="_cellId_' + _cell.Control.Name + '" style="display:none;">' + cellParams + '</textarea>')
    if (_cell.Control.ControlType == Xms.Form.FormControlType.subGrid || _cell.Control.ControlType.toLowerCase() == 'subgrid') {
        var formalar = _cell.Control.Parameters.FieldEvents;
        var formalarStr = ''
        if (formalar && formalar != '') {
            formalarStr = encodeURIComponent(JSON.stringify(formalar));
        }
        var _subgridIsSync = 'noasncSubgrid';
        if (_tab && _tab.Async) {
            _subgridIsSync = 'asyncSubgrid';
        }
        var ispager = (typeof _cell.Control.Parameters.PagingEnabled === 'undefined') ? true : _cell.Control.Parameters.PagingEnabled;
        var rowcount = _cell.Control.Parameters.DefaultEmptyRows === '' ? 5 : _cell.Control.Parameters.DefaultEmptyRows;
        _control.push('<div id="' + _cell.Control.Name.toLowerCase() + '" name="' + _cell.Control.Name.toLowerCase() + '" class="subgrid subgridview_' + _cell.Control.Parameters.ViewId + ' ' + _subgridIsSync + '" data-controltype="subgrid" data-url="' + ORG_SERVERURL + '/entity/rendergridview" data-viewid="' + _cell.Control.Parameters.ViewId + '" data-pagesize="' + _cell.Control.Parameters.PageSize + '" data-relationshipname="' + _cell.Control.Parameters.RelationshipName + '" data-editable="' + _cell.Control.Parameters.Editable + '" data-pagingenabled="' + ispager + '" data-defaultemptyrows="' + rowcount + '" data-formular="' + formalarStr + '"></div>');
    }
    else if (_cell.Control.ControlType == Xms.Form.FormControlType.iFrame || _cell.Control.ControlType.toLowerCase() == 'iframe') {
        _control.push('<iframe id="' + _cell.Control.Name.toLowerCase() + '" name="' + _cell.Control.Name.toLowerCase() + '" src="' + _cell.Control.Parameters.Url + '" frameborder="' + _cell.Control.Parameters.Border + '" style="width:100%;height:100%;"></iframe>');
    }
    else if (_cell.Control.ControlType == Xms.Form.FormControlType.webResource || _cell.Control.ControlType.toLowerCase() == 'webresource') {
        var url = ORG_SERVERURL + '/api/webresource?ids=' + _cell.Control.Parameters;
        _control.push('<div id="' + _cell.Control.Name.toLowerCase() + '" name="' + _cell.Control.Name.toLowerCase() + '" class="webresource" data-controltype="webresource" data-url="' + url + '"></div>');
    }
    else if (_cell.Control.ControlType.toLowerCase() == "label") {
        var fixedn = _cell.Control.Name.toLowerCase() + setTimeout(0);
        var EntityName = "";
        var AttributeName = "";
        var SourceAttributeName = "";
        if (_cell.Control.Parameters) {
            EntityName = _cell.Control.Parameters.EntityName;
            AttributeName = _cell.Control.Parameters.AttributeName;
            SourceAttributeName = _cell.Control.Parameters.SourceAttributeName;
            SourceAttributeType = _cell.Control.Parameters.SourceAttributeType || "";
        }
        _control.push('<div id="' + fixedn + '_text" name="' + fixedn + '" class="attributesLabel attributesLabel-box" data-EntityName="' + EntityName + '" data-SourceAttributeType="' + SourceAttributeType + '" data-AttributeName="' + AttributeName.toLowerCase() + '" data-SourceAttributeName="' + SourceAttributeName.toLowerCase() + '" data-controltype="label"></div>');
        _control.push('<input type="hidden" name="' + fixedn + '" id="' + fixedn + '" >');
    }

    else if (_cell.Control.ControlType == Xms.Form.FormControlType.none || _cell.Control.ControlType.toLowerCase() == 'none') {
    }
    else {
        if (!_cell.Control) {//编辑表单数据错误时也可正常显示
            _control.push('</div>');
            _control.push('</div>');
            _control.push('</div>');
            return _control.join('');
        }
        var attrType = (_cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.AttributeTypeName) ? _cell.Control.AttributeMetadata.AttributeTypeName : _cell.Control.ControlType ? _cell.Control.ControlType : '';
        var attrId = (_cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.AttributeId) || '';
        var dataFormat = (_cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.DataFormat) || '';
        var maxlength = (_cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.MaxLength) || '';
        var localizedname = (_cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.LocalizedName) || '';
        var extparam = {
            EntityName: _cell.Control.Parameters.EntityName || "",
            AttributeName: _cell.Control.Parameters.AttributeName || "",
            SourceAttributeName: _cell.Control.Parameters.SourceAttributeName || ""
        }
        var extParamStr = ' data-extEntityName="' + extparam.EntityName.toLowerCase() + '" data-extAttributeName="' + extparam.AttributeName.toLowerCase() + '" data-extSourceAttributeName="' + extparam.SourceAttributeName.toLowerCase() + '" ';
        if (_cell.Control.Parameters) {
            var FilterRelationshipName = _cell.Control.Parameters.FilterRelationshipName || "";
            var DependentAttributeName = _cell.Control.Parameters.DependentAttributeName || "";
            var DependentAttributeType = _cell.Control.Parameters.DependentAttributeType || "";
            var AllowFilterOff = _cell.Control.Parameters.AllowFilterOff || false;
            DependentAttributeName = DependentAttributeName.toLowerCase()
        }
        //if (_cell.Control.Name == "StatusCode") {
        //    console.log("", _cell.Control)
        //}
        //if (_cell.Control.Name == "OrganizationId") {
        //    console.log("fffffffff", _cell.Control)
        //}
        //添加输入框样式
        if (pageWrap_Create.cellsStyles) {
            pageWrap_Create.cellsStyles.push($.changeobjtoStyle(inputstyle, '.xmsformContent [name="' + _cell.Control.Name.toLowerCase() + '"]', ''));
            pageWrap_Create.cellsStyles.push($.changeobjtoStyle(inputstyle, '.xmsformContent [name="' + _cell.Control.Name.toLowerCase() + '_text"]', ''));
        }
        switch (attrType) {
            case "ntext":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + value + '</span>');
                }
                else {
                    if (dataFormat == "fileupload") {
                        var hasValue = value == '' ? "" : "点击查看";

                        _control.push('<div class="form-field-box upload-file-box"><span title="" data-imgurl="' + value + '"  data-name="' + _cell.Control.Name.toLowerCase() + '" class="upload-file-input" >' + hasValue + '</span><a class="upload-file"><input type="file" class="uploadify-field" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name + '" /><span class="glyphicon glyphicon-open"></span></a><a class="upload-file-del"><span class="glyphicon glyphicon-remove-sign"></span></a><input name="___file_upload_' + _cell.Control.Name.toLowerCase() + '" type="text" class="uploadinput dirtylisten form-control input-sm nvarchar ' + required + '" style="visible:hidden;" value="' + value + '" id="___file_upload_' + _cell.Control.Name.toLowerCase() + '" ' + readonly + ' ></div>');
                    } else if (dataFormat == "email") {
                        if (value && value != '') {
                            value = decodeURIComponent(value);
                            //console.log('xxxxxxxxxxxxxxxxxxxxxx',value);
                        }

                        _control.push('<script id="ntext_' + _cell.Control.Name.toLowerCase() + '" type="text/plain">' + value + '</script>');
                        //
                        _control.push('<textarea rows="5" class="dirtylisten form-control extParamEnti input-sm hide  ntext' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-format="email" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" value="' + value + '"' + readonly + extParamStr + '>' + value + '</textarea>');
                    } else {
                        _control.push('<textarea rows="5" class="dirtylisten form-control extParamEnti input-sm  ntextarea' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-format="text" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" value="' + value + '"' + readonly + extParamStr + '>' + value + '</textarea>');
                    }
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "FreeText":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + value + '</span>');
                }
                else {
                    if (_cell.Control && _cell.Control.Parameters && _cell.Control.Parameters.Content) {
                        value = _cell.Control.Parameters.Content || '';
                        value = (value);
                    }

                    _control.push('<div  class=" freetext" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-format="email" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" value="' + value + '"></div>');
                    // dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "nvarchar":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + value + '</span>');
                }
                else {
                    if (dataFormat == 'textarea') {
                        _control.push('<textarea type="text" class="dirtylisten form-control extParamEnti input-sm nvarchar' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" rows="2" maxlength="' + maxlength + '"' + readonly + extParamStr + '>' + value + '</textarea>');
                    }
                    else if (dataFormat == "fileupload") {
                        var hasValue = value == '' ? "" : "点击查看";
                        _control.push('<div class="form-field-box upload-file-box"><span title="" data-imgurl="' + value + '" data-name="' + _cell.Control.Name.toLowerCase() + '" class="upload-file-input" >' + hasValue + '</span><a class="upload-file"><input type="file" class="uploadify-field" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name + '" /><span class="glyphicon glyphicon-open"></span></a><a class="upload-file-del"><span class="glyphicon glyphicon-remove-sign"></span></a><input name="___file_upload_' + _cell.Control.Name.toLowerCase() + '" type="text" class="uploadinput dirtylisten form-control input-sm nvarchar ' + required + '" style="visible:hidden;" value="' + value + '" id="___file_upload_' + _cell.Control.Name.toLowerCase() + '" ' + readonly + ' ></div>');
                    }
                    else if (dataFormat == "password") {
                        _control.push('<input type="password" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm nvarchar' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-onlylabel="' + onlyLabel + '" data-format="' + dataFormat + '" data-type="' + dataFormat + '" data-localizedname="' + localizedname + '" maxlength="' + maxlength + '" value="' + value + '"' + readonly + extParamStr + ' />');
                    }
                    else {
                        _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm nvarchar' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-onlylabel="' + onlyLabel + '" data-format="' + dataFormat + '" data-type="' + dataFormat + '" data-localizedname="' + localizedname + '" maxlength="' + maxlength + '" value="' + value + '"' + readonly + extParamStr + ' />');
                    }
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "int":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '">' + value + '</span>');
                }
                else {
                    var range = ' data-range="[' + _cell.Control.AttributeMetadata.MinValue + ',' + _cell.Control.AttributeMetadata.MaxValue + ']"';
                    _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm int' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="int" ' + range + '  data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" value="' + value + '"' + readonly + extParamStr + ' />');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "float":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '" data-attributeid="' + attrId + '">' + value + '</span>');
                }
                else {
                    var range = ' data-range="[' + _cell.Control.AttributeMetadata.MinValue + ',' + _cell.Control.AttributeMetadata.MaxValue + ']"';
                    value = value != '' ? value : value;
                    _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm float' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="number"' + range + ' data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" value="' + value + '"' + readonly + extParamStr + ' />');
                    dirtyChecker.addWatch(_cell.Control.Name, value);
                }
                break;
            case "money":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + value + '</span>');
                }
                else {
                    var range = ' data-range="[' + _cell.Control.AttributeMetadata.MinValue + ',' + _cell.Control.AttributeMetadata.MaxValue + ']"';
                    _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm money' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="number"' + range + ' data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" value="' + value + '"' + readonly + extParamStr + ' />');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "datetime":
                if (onlyLabel) {
                    var dataformat = _cell.Control.AttributeMetadata.DataFormat;
                    if (dataformat == "yyyy/MM/dd") {
                        val = value.match(/^[0-9]{4}\/[0-9]{1,2}\/[0-9]{1,2}/);
                        if (val && val.length > 0) {
                            if (document.all) {
                                value = value.replace(/\-/, '\/');
                            }
                            value = new Date(value).format('yyyy/MM/dd');
                        }
                    }
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + value + '</span>');
                }
                else {
                    var dataformat = _cell.Control.AttributeMetadata.DataFormat;

                    if (value != "" && dataformat == "yyyy/MM/dd") {
                        if (document.all) {
                            value = value.replace(/\-/, '\/');
                        }
                        value = new Date(value).format('yyyy-MM-dd');
                    }
                    _control.push('<div class="input-group datepicker-col">');
                    _control.push('<span class="input-group-addon"><em class="glyphicon glyphicon-calendar"></em></span><input type="text" class="dirtylisten form-control input-sm extParamEnti datepicker' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="date" data-fmdata="' + dataformat + '" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '" value="' + value + '"' + readonly + extParamStr + ' />');
                    _control.push('</div>');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "lookup":
                //console.log(_cell.Control.Name)
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" class="lookupLabel" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + value + '</span>');
                } else {
                    var isorg = '';
                     if (_cell.Control.Name == "OrganizationId") {
                         isorg = ' readonly ';
                     }
                    _control.push('<input type="text" autocomplete="off" class="nodirty form-control input-sm extParamEnti lookup' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '_text" id="' + _cell.Control.Name.toLowerCase() + '_text" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-FilterRelationshipName="' + FilterRelationshipName + '" data-DependentAttributeName="' + DependentAttributeName + '" data-DependentAttributeType="' + DependentAttributeType + '" data-AllowFilterOff="' + AllowFilterOff + '"  data-value="' + value + '" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '"' + readonly + extParamStr + isorg+ ' />');
                    _control.push('<input type="hidden" class="dirtylisten haslookup' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="value_' + attrId + '" data-controltype="' + attrType + '" data-type="guid" value="' + value + '" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-localizedname="' + localizedname + '" />');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "owner":
                // console.log(_cell.Control.Name + ":" + readonly)
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" class="lookupLabel" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + value + '</span>');
                } else {
                    _control.push('<input type="text" autocomplete="off" class="nodirty form-control input-sm extParamEnti lookup' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '_text" id="' + _cell.Control.Name.toLowerCase() + '_text" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="guid" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-value="' + value + '" data-onlylabel="' + onlyLabel + '" data-FilterRelationshipName="' + FilterRelationshipName + '" data-DependentAttributeName="' + DependentAttributeName + '" data-DependentAttributeType="' + DependentAttributeType + '" data-AllowFilterOff="' + AllowFilterOff + '"  data-localizedname="' + localizedname + '"' + extParamStr + readonly + ' />');
                    _control.push('<input type="hidden" class="dirtylisten haslookup' + required + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="value_' + attrId + '" data-controltype="' + attrType + '" data-type="guid" value="' + value + '" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-localizedname="' + localizedname + '" />');

                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "customer":
                if (onlyLabel) {
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" class="lookupLabel" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '"></span>');
                } else {
                    _control.push('<input type="text" autocomplete="off" class="nodirty form-control input-sm extParamEnti lookup' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '_text" id="' + _cell.Control.Name.toLowerCase() + '_text" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="guid" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-value="' + value + '" data-onlylabel="' + onlyLabel + '" data-FilterRelationshipName="' + FilterRelationshipName + '" data-DependentAttributeName="' + DependentAttributeName + '" data-DependentAttributeType="' + DependentAttributeType + '" data-AllowFilterOff="' + AllowFilterOff + '"  data-localizedname="' + localizedname + '"' + readonly + extParamStr + ' />');
                    _control.push('<input type="hidden" class="dirtylisten haslookup' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="value_' + attrId + '" data-controltype="' + attrType + '" data-type="guid" value="' + value + '" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-localizedname="' + localizedname + '" />');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "picklist":
                if (onlyLabel) {
                    var text = value;
                    if (value.toString() != '' && _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.OptionSet && _cell.Control.AttributeMetadata.OptionSet.Items) {
                        var a = $.grep(_cell.Control.AttributeMetadata.OptionSet.Items, function (n, i) {
                            return n.Value == value;
                        });
                        if (a && a.length > 0) {
                            text = a[0].Name;
                        } else {
                            text = '';
                        }
                    }
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + text + '</span>');
                }
                else {
                    _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm picklist' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" data-displaystyle="' + _cell.Control.AttributeMetadata.DisplayStyle.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '_text" data-attributeid="value_' + attrId + '" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.OptionSet.Items)) + '" value="' + value + '" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '"' + readonly + extParamStr + ' />');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase() + '_text', value);
                }
                break;
            case "status":
                if (onlyLabel) {
                    var text = value;
                    if (value.toString() != '' && _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.OptionSet && _cell.Control.AttributeMetadata.OptionSet.Items) {
                        var a = $.grep(_cell.Control.AttributeMetadata.OptionSet.Items, function (n, i) {
                            return n.Value == value;
                        });
                        if (a && a.length > 0) {
                            text = a[0].Name;
                        } else {
                            text = '';
                        }
                    }
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + text + '</span>');
                }
                else {
                    _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm picklist' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '_text" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.OptionSet.Items)) + '" value="' + value + '" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '"' + readonly + extParamStr + ' />');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase() + '_text', value);
                }
                break;
            case "bit":
                if (onlyLabel) {
                    var text = value;
                    if (value.toString() != '' && _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.PickLists) {
                        var a = $.grep(_cell.Control.AttributeMetadata.PickLists, function (n, i) {
                            return n.Value == value;
                        });
                        if (a && a.length > 0) {
                            text = a[0].Name;
                        } else {
                            text = '';
                        }
                    }
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + text + '</span>');
                }
                else {
                    _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm bit' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.PickLists)) + '" data-onlylabel="' + onlyLabel + ' data-localizedname="' + localizedname + '"" value="' + (value ? 1 : 0) + '"' + readonly + extParamStr + ' />');
                    console.log('html', _control)
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            case "state":
                if (onlyLabel) {
                    var text = value;
                    if (value.toString() != '' && _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.PickLists) {
                        var a = $.grep(_cell.Control.AttributeMetadata.PickLists, function (n, i) {
                            return n.Value == value;
                        });
                        if (a && a.length > 0) {
                            text = a[0].Name;
                        } else {
                            text = '';
                        }
                    }
                    _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + text + '</span>');
                }
                else {
                    _control.push('<input type="text" autocomplete="off" class="dirtylisten form-control extParamEnti input-sm bit' + required + ' ' + isvalidate + '" name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(_cell.Control.AttributeMetadata.PickLists)) + '" data-onlylabel="' + onlyLabel + ' data-localizedname="' + localizedname + '"" value="' + value + '"' + readonly + extParamStr + ' />');
                    dirtyChecker.addWatch(_cell.Control.Name.toLowerCase(), value);
                }
                break;
            default:
                if (_cell.Control.Name == "ModifiedBy") {
                    //if (onlyLabel) {
                    //    _control.push('<span name="' + _cell.Control.Name + '" class="lookupLabel" id="' + _cell.Control.Name + '" data-attributeid="' + attrId + '">' + value + '</span>');
                    //} else {
                    //    _control.push('<input type="text" class="nodirty form-control input-sm extParamEnti lookup' + required + '" name="' + _cell.Control.Name + '_text" id="' + _cell.Control.Name + '_text" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-FilterRelationshipName="' + FilterRelationshipName + '" data-DependentAttributeName="' + DependentAttributeName + '" data-DependentAttributeType="' + DependentAttributeType + '" data-AllowFilterOff="' + AllowFilterOff + '"  data-value="' + value + '" data-onlylabel="' + onlyLabel + '" data-localizedname="' + localizedname + '"' + readonly + extParamStr + ' />');
                    //    _control.push('<input type="hidden" class="dirtylisten haslookup' + required + '" name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-type="guid" value="' + value + '" data-lookup="' + _cell.Control.AttributeMetadata.ReferencedEntityId + '" data-relationshipname="' + ((_cell.Control.Parameters && _cell.Control.Parameters.RelationshipName) || '') + '" data-defaultviewid="' + ((_cell.Control.Parameters && _cell.Control.Parameters.DefaultViewId) || '') + '" data-localizedname="' + localizedname + '" />');
                    //    dirtyChecker.addWatch(_cell.Control.Name, value);
                    //}
                } else {
                    if (onlyLabel) {
                        if (_cell.Control.Name == "StatusCode") {
                            var text = value;
                            if (value.toString() != '' && _cell.Control.AttributeMetadata && _cell.Control.AttributeMetadata.PickLists) {
                                if (_cell.Control.AttributeMetadata.PickLists) {
                                    var a = $.grep(_cell.Control.AttributeMetadata.PickLists, function (n, i) {
                                        return n.Value == value;
                                    });
                                    if (a && a.length > 0) text = a[0].Name;
                                }
                            }
                            _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '">' + text + '</span>');
                        } else {
                            _control.push('<span name="' + _cell.Control.Name.toLowerCase() + '" class="lookupLabel" id="' + _cell.Control.Name.toLowerCase() + '" data-attributeid="' + attrId + '"></span>');
                        }
                    }
                }
        }
        //if (onlyLabel) {
        //    _control.push('<input type="hidden" name="' + _cell.Control.Name + '" id="' + _cell.Control.Name + '" data-attributeid="' + attrId + '" data-controltype="' + attrType + '" data-localizedname="' + localizedname + '" value="' + value + '" />');
        //}
    }
    //_control.push('</div>');//input-group  end
    _control.push('</div>');
    _control.push('</div>');
    _control.push('</div>');
    return _control.join('');
}