//表单编辑
function renderFormHeader(Panels) {
    if (!Panels || Panels.length == 0) { return false; }
    var headerDom = $("#formHeader");
    var headerParam = {
        columns: Panels.Columns,
        label: Panels.Label,
        isvisible: Panels.IsVisible,
        isshowlabel: Panels.IsShowLabel,
        celllabelwidth: Panels.CellLabelWidth,
        celllabelalignment: Panels.CellLabelAlignment,
        celllabelposition: Panels.CellLabelPosition,
        timestamp: Panels.CellLabelAlignment
    }
    for (var jj in headerParam) {
        if (headerParam.hasOwnProperty(jj)) {
            headerDom.attr("data-" + jj, headerParam[jj]);
        }
    }
    var panelHtml = [];
    if (headerParam.IsShowLabel && (headerParam.IsShowLabel == true || headerParam.IsShowLabel == 'true')) {
        panelHtml.push('<div class="section-title header-title"><span class="glyphicon glyphicon-lock"></span>' + headerParam.label + '</div>');
    }
    else {
        panelHtml.push('<div class="section-title header-title hidden-text"><span class="glyphicon glyphicon-lock"></span>' + headerParam.label + '</div>');
    }

    panelHtml.push('<table class="table table-bordered"><tbody>');
    var Rows = Panels.Rows;
    var Columns = Panels.Columns;

    //头部表单 单元格
    for (var iii = 0; iii < Rows.length; iii++) {
        panelHtml.push('<tr>');
        var Cells = Rows[iii].Cells;
        if (Cells.length == 0) {
            for (var iiii = 0; iiii < Columns; iiii++) {
                panelHtml.push('<td class="col-sm-' + (Cells[iiii].ColSpan * 2) + ' field placeholder ui-droppable ui-sortable"></td>');
            }
        } else {
            for (var iiii = 0; iiii < Cells.length; iiii++) {
                var dataSubgrid = '';
                var dataLookup = '';
                var labelparam = '';
                var freetextparam = '';
                var readonly = Cells[iiii].Control.ReadOnly || "false";
                if (Cells[iiii].Control.ControlType != 'none') {
                    loadedEntitys.push(Cells[iiii].Control.Name);//保存已存在的字段名
                    if (!Cells[iiii].ColSpan || Cells[iiii].ColSpan == 0) {
                        Cells[iiii].ColSpan = 1
                    }
                    var itemcolspan = Cells[iiii].ColSpan;
                    if (typeof Cells[iiii].Control.ControlType != 'undefined' && (typeof Cells[iiii].Control.ControlType == "string") && Cells[iiii].Control.ControlType.toLowerCase() == 'subgrid') {
                        var subeditable = Cells[iiii].Control.Parameters.Editable || true;
                        // if ( Cells[iiii].Control.ControlType.toLowerCase() == 'subgrid') {
                        dataSubgrid = ' data-pagesize="' + Cells[iiii].Control.Parameters.PageSize + '" data-editable="' + subeditable + '" data-viewid="' + Cells[iiii].Control.Parameters.ViewId + '" data-relationshipname="' + Cells[iiii].Control.Parameters.RelationshipName + '" data-targetentityname="' + Cells[iiii].Control.Parameters.TargetEntityName + '" ';
                    }
                    if (typeof Cells[iiii].Control.ControlType === 'string' && Cells[iiii].Control.ControlType.toLowerCase() == 'lookup') {
                        if (Cells[iiii].Control.Parameters) {
                            var IsFilterRelation = false;
                            if (Cells[iiii].Control.Parameters.FilterRelationshipName && Cells[iiii].Control.Parameters.FilterRelationshipName != "") {
                                IsFilterRelation = true;
                            }
                            dataLookup += ' data-isfilterrelation="' + IsFilterRelation + '" data-filterrelationshipname="' + (Cells[iiii].Control.Parameters.FilterRelationshipName || '') + '" data-dependentattributetype="' + (Cells[iiii].Control.Parameters.DependentAttributeType || '') + '" data-dependentattributename="' + (Cells[iiii].Control.Parameters.DependentAttributeName || '') + '" data-allowfilteroff="' + (Cells[iiii].Control.Parameters.AllowFilterOff || false) + '"';
                        } else {
                            dataLookup += ' data-filterrelationshipname="" data-dependentattributetype="" data-dependentattributename="" data-isfilterrelation="false" data-allowfilteroff="false" ';
                        }
                    }
                    var labelflag = '';
                    if (typeof Cells[iiii].Control.ControlType === 'string' && Cells[iiii].Control.ControlType.toLowerCase() == 'label') {
                        labelflag = ' labelbox';
                        console.log(Cells[iiii].Control.Parameters)
                        if (Cells[iiii].Control.Parameters) {
                            console.log(Cells[iiii].Control.Parameters.EntityName)
                            labelparam += ' data-labelentityname="' + Cells[iiii].Control.Parameters.EntityName + '" data-attributename="' + (Cells[iiii].Control.Parameters.AttributeName) + '" data-sourceattributename="' + (Cells[iiii].Control.Parameters.SourceAttributeName) + '" data-sourceattributetype="' + (Cells[iiii].Control.Parameters.SourceAttributeType) + '"';
                        } else {
                            labelparam += ' data-labelentityname="" data-attributename="" data-sourceattributename=""';
                        }
                        // console.log(labelparam)
                    } else if (typeof Cells[iiii].Control.ControlType === 'string' && Cells[iiii].Control.ControlType.toLowerCase() == 'freetext') {
                        freetextflag = ' freetextbox';
                        //  console.log(Cells[iiii].Control.Parameters)
                        if (Cells[iiii].Control.Parameters) {
                            //     console.log(Cells[iiii].Control.Parameters.EntityName)
                            freetextparam += ' data-labelentityname="' + Cells[iiii].Control.Parameters.EntityName + '" data-attributename="' + (Cells[iiii].Control.Parameters.AttributeName) + '" data-sourceattributename="' + (Cells[iiii].Control.Parameters.SourceAttributeName) + '" data-paramcontent="' + (Cells[iiii].Control.Parameters.Content || '') + '" data-sourceattributetype="' + (Cells[iiii].Control.Parameters.SourceAttributeType) + '"';
                        } else {
                            freetextparam += ' data-labelentityname="" data-attributename="" data-sourceattributename=""';
                        }
                        //  console.log(labelparam)
                    }
                    var customcss = '';
                    if (Cells[iiii].CustomCss) {
                        customcss = ' data-customcss="' + Cells[iiii].CustomCss + '" '
                    }
                    panelHtml.push('<td class="col-sm-' + (Cells[iiii].ColSpan * 2) + ' field ui-droppable ui-sortable' + labelflag + '" colspan="' + Cells[iiii].ColSpan + '">');
                    panelHtml.push('<table class="table cell"  data-label="' + Cells[iiii].Label + '" data-name="' + Cells[iiii].Control.Name + '" data-entityname="' + Cells[iiii].Control.EntityName + '" data-type="" data-isshowlabel="' + Cells[iiii].IsShowLabel + '" data-isvisible="' + (Cells[iiii].IsVisible || true) + '" data-colspan="' + Cells[iiii].ColSpan + '" data-isreadonly="' + readonly + '" data-controltype="' + Cells[iiii].Control.ControlType + '" data-timestamp="" data-defaultviewreadonly="true" data-viewpickerreadonly="true" ' + customcss + ' data-disableviewpicker="true" ' + dataSubgrid + dataLookup + '><tbody><tr style="height:100%;">');

                    if (typeof Cells[iiii].Control.ControlType != 'undefined' && (typeof Cells[iiii].Control.ControlType == "string") && Cells[iiii].Control.ControlType.toLowerCase() == 'none') {
                        //if (Cells[iiii].Control.ControlType.toLowerCase() == 'none') {
                        panelHtml.push('<th class="col-sm-3 ' + ((Cells[iiii].IsShowLabel == 'true' || Cells[iiii].IsShowLabel == true) ? '' : ' disable-text') + ((Cells[iiii].IsVisible == 'true' || Cells[iiii].IsVisible == true) ? '' : ' visible-hidden') + '">空格</th><td  class="col-sm-4">空格</td></tr></tbody></table>');
                        // }
                    } else {
                        panelHtml.push('<th class="col-sm-3 ' + ((Cells[iiii].IsShowLabel == 'true' || Cells[iiii].IsShowLabel == true) ? '' : ' disable-text') + ((Cells[iiii].IsVisible == 'true' || Cells[iiii].IsVisible == true) ? '' : ' visible-hidden') + '">' + Cells[iiii].Label + '</th><td  class="col-sm-4">' + Cells[iiii].Label + '</td></tr></tbody></table>');
                    }
                    panelHtml.push('</td>');
                    if (itemcolspan > 1) {
                        // iiii += itemcolspan * 1 - 1;
                    }
                } else {
                    panelHtml.push('<td class="col-sm-2 field ui-droppable placeholder ui-sortable" colspan="1">');
                }
            }
        }
        panelHtml.push('</tr>');
    }

    panelHtml.push('</tbody></table>');

    headerDom.html(panelHtml.join(""));
}

function renderFormFooter(Panels) {
    if (!Panels || Panels.length == 0) { return false; }
    var footerDom = $("#formFooter");
    var footerParam = {
        columns: Panels.Columns,
        label: Panels.Label,
        isvisible: Panels.IsVisible,
        isshowlabel: Panels.IsShowLabel,
        celllabelwidth: Panels.CellLabelWidth,
        celllabelalignment: Panels.CellLabelAlignment,
        celllabelposition: Panels.CellLabelPosition,
        timestamp: Panels.CellLabelAlignment
    }
    for (var jj in footerParam) {
        if (footerParam.hasOwnProperty(jj)) {
            footerDom.attr("data-" + jj, footerParam[jj]);
        }
    }
    var panelHtml = [];

    if (footerParam.IsShowLabel && (footerParam.IsShowLabel == true || footerParam.IsShowLabel == 'true')) {
        panelHtml.push('<div class="section-title footer-title"><span class="glyphicon glyphicon-lock"></span>' + footerParam.label + '</div>');
    }
    else {
        panelHtml.push('<div class="section-title footer-title hidden-text"><span class="glyphicon glyphicon-lock"></span>' + footerParam.label + '</div>');
    }
    panelHtml.push('<table class="table table-bordered"><tbody>');
    var Rows = Panels.Rows;
    var Columns = Panels.Columns;

    //头部表单 单元格
    for (var iii = 0; iii < Rows.length; iii++) {
        panelHtml.push('<tr>');
        var Cells = Rows[iii].Cells;
        if (Cells.length == 0) {
            for (var iiii = 0; iiii < Columns; iiii++) {
                panelHtml.push('<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>');
            }
        } else {
            for (var iiii = 0; iiii < Cells.length; iiii++) {
                var dataSubgrid = '';
                var dataLookup = '';
                var labelparam = '';
                var freetextparam = '';
                if (Cells[iiii].Control.ControlType != 'none') {
                    loadedEntitys.push(Cells[iiii].Control.Name);//保存已存在的字段名
                    if (!Cells[iiii].ColSpan || Cells[iiii].ColSpan == 0) {
                        Cells[iiii].ColSpan = 1
                    }
                    var itemcolspan = Cells[iiii].ColSpan;
                    var readonly = Cells[iiii].Control.ReadOnly || "false";
                    if (typeof Cells[iiii].Control.ControlType != 'undefined' && (typeof Cells[iiii].Control.ControlType == "string") && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'subgrid') {
                        var subeditable = Cells[iiii].Control.Parameters.Editable || true;
                        // if ( Cells[iiii].Control.ControlType.toLowerCase() == 'subgrid') {
                        dataSubgrid = ' data-pagesize="' + Cells[iiii].Control.Parameters.PageSize + '" data-editable="' + subeditable + '" data-viewid="' + Cells[iiii].Control.Parameters.ViewId + '" data-relationshipname="' + Cells[iiii].Control.Parameters.RelationshipName + '" data-targetentityname="' + Cells[iiii].Control.Parameters.TargetEntityName + '" ';
                    }
                    if (typeof Cells[iiii].Control.ControlType === 'string' && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'lookup') {
                        if (Cells[iiii].Control.Parameters) {
                            var IsFilterRelation = false;
                            if (Cells[iiii].Control.Parameters.FilterRelationshipName && Cells[iiii].Control.Parameters.FilterRelationshipName != "") {
                                IsFilterRelation = true;
                            }
                            dataLookup += ' data-isfilterrelation="' + IsFilterRelation + '" data-filterrelationshipname="' + (Cells[iiii].Control.Parameters.FilterRelationshipName || '') + '" data-dependentattributetype="' + (Cells[iiii].Control.Parameters.DependentAttributeType || '') + '" data-dependentattributename="' + (Cells[iiii].Control.Parameters.DependentAttributeName || '') + '" data-allowfilteroff="' + (Cells[iiii].Control.Parameters.AllowFilterOff || false) + '"';
                        } else {
                            dataLookup += ' data-filterrelationshipname="" data-dependentattributetype="" data-dependentattributename="" data-isfilterrelation="false" data-allowfilteroff="false" ';
                        }
                    }
                    var labelflag = '';
                    if (typeof Cells[iiii].Control.ControlType != 'undefined' && (Cells[iiii].Control.ControlType == Xms.Form.FormControlType.label || Cells[iiii].Control.ControlType.toString().toLowerCase() == 'label')) {
                        labelflag = ' labelbox';
                        console.log(Cells[iiii].Control.Parameters)
                        if (Cells[iiii].Control.Parameters) {
                            console.log(Cells[iiii].Control.Parameters.EntityName)
                            labelparam += ' data-labelentityname="' + Cells[iiii].Control.Parameters.EntityName + '" data-attributename="' + (Cells[iiii].Control.Parameters.AttributeName) + '" data-sourceattributename="' + (Cells[iiii].Control.Parameters.SourceAttributeName) + '" data-sourceattributetype="' + (Cells[iiii].Control.Parameters.SourceAttributeType) + '"';
                        } else {
                            labelparam += ' data-labelentityname="" data-attributename="" data-sourceattributename=""';
                        }
                    } else if (typeof Cells[iiii].Control.ControlType != 'undefined' && (Cells[iiii].Control.ControlType == Xms.Form.FormControlType.label || Cells[iiii].Control.ControlType.toString().toLowerCase() == 'freetext')) {
                        freetextflag = ' freetextbox';
                        //  console.log(Cells[iiii].Control.Parameters)
                        if (Cells[iiii].Control.Parameters) {
                            //     console.log(Cells[iiii].Control.Parameters.EntityName)
                            freetextparam += ' data-labelentityname="' + Cells[iiii].Control.Parameters.EntityName + '" data-attributename="' + (Cells[iiii].Control.Parameters.AttributeName) + '" data-sourceattributename="' + (Cells[iiii].Control.Parameters.SourceAttributeName) + '" data-paramcontent="' + (Cells[iiii].Control.Parameters.Content || '') + '" data-sourceattributetype="' + (Cells[iiii].Control.Parameters.SourceAttributeType) + '"';
                        } else {
                            freetextparam += ' data-labelentityname="" data-attributename="" data-sourceattributename=""';
                        }
                        //  console.log(labelparam)
                    }

                    var customcss = '';
                    if (Cells[iiii].CustomCss) {
                        customcss = ' data-customcss="' + Cells[iiii].CustomCss + '" '
                    }
                    panelHtml.push('<td class="col-sm-' + (Cells[iiii].ColSpan * 2) + ' field ui-droppable ui-sortable' + labelflag + '" colspan="' + Cells[iiii].ColSpan + '">');
                    panelHtml.push('<table class="table cell"  data-label="' + Cells[iiii].Label + '" data-name="' + Cells[iiii].Control.Name + '" data-entityname="' + Cells[iiii].Control.EntityName + '" data-type="" data-isshowlabel="' + Cells[iiii].IsShowLabel + '" data-isvisible="' + Cells[iiii].IsVisible + '" data-colspan="' + Cells[iiii].ColSpan + '" data-isreadonly="' + readonly + '" data-controltype="' + Cells[iiii].Control.ControlType + '" data-timestamp="" data-defaultviewreadonly="true" data-viewpickerreadonly="true" ' + customcss + ' data-disableviewpicker="true" ' + dataSubgrid + dataLookup + '><tbody><tr style="height:100%;">');

                    if (typeof Cells[iiii].Control.ControlType != 'undefined' && (typeof Cells[iiii].Control.ControlType == "string") && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'none') {
                        //if (Cells[iiii].Control.ControlType.toLowerCase() == 'none') {
                        panelHtml.push('<th class="col-sm-3 ' + (Cells[iiii].IsShowLabel == 'true' ? '' : ' disable-text') + ((Cells[iiii].IsVisible == 'true' || Cells[iiii].IsVisible == true) ? '' : ' visible-hidden') + '">空格</th><td  class="col-sm-4">空格</td></tr></tbody></table>');
                        // }
                    } else {
                        panelHtml.push('<th class="col-sm-3 ' + (Cells[iiii].IsShowLabel == 'true' ? '' : ' disable-text') + ((Cells[iiii].IsVisible == 'true' || Cells[iiii].IsVisible == true) ? '' : ' visible-hidden') + '">' + Cells[iiii].Label + '</th><td  class="col-sm-4">' + Cells[iiii].Label + '</td></tr></tbody></table>');
                    }
                    panelHtml.push('</td>');
                    if (itemcolspan > 1) {
                        // iiii += itemcolspan * 1 - 1;
                    }
                } else {
                    panelHtml.push('<td class="col-sm-2 field ui-droppable placeholder ui-sortable" colspan="1">');
                }
            }
        }
        panelHtml.push('</tr>');
    }

    panelHtml.push('</tbody></table>');

    footerDom.html(panelHtml.join(""));
}

//
function renderFormContent(Panels) {
    var panelHtml = [];

    for (var i = 0; i < Panels.length; i++) {
        var tabid = 'tab_' + Math.round(new Date().getTime() / 1000);
        var displaystyle = ((Panels[i].DisplayStyle == 'true' || Panels[i].DisplayStyle == '1') ? "1" : "0");
        var _isAsync = Panels[i].Async || false;
        panelHtml.push('<div class="tab" data-name="' + Panels[i].Name + '" data-isasync="' + _isAsync + '" data-label="' + Panels[i].Label + '" data-isshowlabel="' + Panels[i].IsShowLabel + '" data-DisplayStyle="' + displaystyle + '" data-isexpanded="' + Panels[i].IsExpanded + '" data-isvisible="' + Panels[i].IsVisible + '">');
        var isshowlabel = (Panels[i].IsShowLabel && Panels[i].IsShowLabel != "false") ? "" : "hidden-text";

        panelHtml.push('<a data-toggle="collapse" href="#' + tabid + '" class="collapse-title tab-title ' + isshowlabel + '" data-target="' + tabid + '">');
        panelHtml.push('<span class="glyphicon glyphicon-chevron-down"></span> <span class="title ">' + Panels[i].Label + '</span>');
        panelHtml.push('</a>');
        panelHtml.push('<div id="' + tabid + '" class="panel-collapse collapse in">');
        var Section = Panels[i].Sections;
        for (var ii = 0; ii < Section.length; ii++) {
            var Columns = Section[ii].Columns;
            if (typeof Section[ii].Label === "undefined") {
                Section[ii].Label = "表格";
            }
            if (typeof Section[ii].CellLabelWidth === "undefined") {
                Section[ii].CellLabelWidth = 100
            }
            if (typeof Section[ii].CellLabelAlignment === "undefined") {
                Section[ii].CellLabelAlignment = "Left";
            }
            if (typeof Section[ii].CellLabelPosition === "undefined") {
                Section[ii].CellLabelPosition = "Left";
            }
            if (typeof Section[ii].CellLabelSettings === "undefined") {
                Section[ii].CellLabelSettings = {
                    Width: '',
                    Alignment: 'Left',
                    Position: 'Left'
                };
            }
            if (Section[ii].CellLabelSettings && Section[ii].CellLabelSettings.Width == 'undefined') {
                Section[ii].CellLabelSettings = {
                    Width: '',
                    Alignment: 'Left',
                    Position: 'Left'
                };
            }

            panelHtml.push('<div class="section" data-columns="' + Section[ii].Columns + '" data-label="' + Section[ii].Label + '" data-isvisible="' + Section[ii].IsVisible + '" data-isshowlabel="' + Section[ii].IsShowLabel + '" data-celllabelwidth="' + Section[ii].CellLabelWidth + '" data-isreadonly="false" data-celllabelalignment="' + Section[ii].CellLabelAlignment + '" data-celllabelposition="' + Section[ii].CellLabelPosition + '"  data-attrwidth="' + Section[ii].CellLabelSettings.Width + '" data-attralignment="' + Section[ii].CellLabelSettings.Alignment + '" data-attrposition="' + Section[ii].CellLabelSettings.Position + '" >');
            //console.log(Section[ii].IsShowLabel)
            if (Section[ii].IsShowLabel && (Section[ii].IsShowLabel == true || Section[ii].IsShowLabel == 'true')) {
                panelHtml.push('<div class="section-title  content-title title">' + Section[ii].Label + '</div>');
            }
            else {
                panelHtml.push('<div class="section-title content-title title hidden-text">' + Section[ii].Label + '</div>');
            }
            panelHtml.push('<table class="table table-bordered">');
            panelHtml.push('<tbody>');
            var Rows = Section[ii].Rows;
            for (var iii = 0; iii < Rows.length; iii++) {
                panelHtml.push('<tr>');
                var Cells = Rows[iii].Cells;
                var cellsLength = Cells.length;
                var cellsCurIndex = 0;
                if (Cells.length == 0) {
                    for (var iiii = 0; iiii < Columns; iiii++) {
                        //panelHtml.push('<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>');
                    }
                } else {
                    for (var iiii = 0; iiii < Cells.length; iiii++) {
                        var dataSubgrid = '';
                        var dataLookup = '';
                        var labelparam = '';
                        var freetextparam = '';
                        var extParam = '';
                        var readonly = Cells[iiii].Control.ReadOnly || "false";
                        if (Cells[iiii].Control.ControlType != 'none') {
                            loadedEntitys.push(Cells[iiii].Control.Name);//保存已存在的字段名
                            if (!Cells[iiii].ColSpan || Cells[iiii].ColSpan == 0) {
                                Cells[iiii].ColSpan = 1
                            }

                            var itemcolspan = Cells[iiii].ColSpan;
                            cellsCurIndex += itemcolspan * 1;
                            var formulaP = '';
                            if (typeof Cells[iiii].Control.ControlType != 'undefined' && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'subgrid') {
                                var subeditable = Cells[iiii].Control.Parameters.Editable || true;
                                var formulars = Cells[iiii].Control.Parameters.FieldEvents;
                                var formularStr = '';
                                if (formulars && formulars.length > 0) {
                                    $.each(formulars, function (key, item) {
                                        if (~item.expression.indexOf('$$$')) {//防止旧数据不能使用
                                            item.expression = item.expression.split('$$$');
                                        }
                                    });
                                    console.log()
                                    formularStr = encodeURIComponent(JSON.stringify(formulars));
                                    formularStr = ' data-formulavalue="' + formularStr + '" ';
                                    // formularStr = JSON.stringify(formularStr);
                                }
                                var PagingEnabled = (typeof Cells[iiii].Control.Parameters.PagingEnabled === 'undefined') ? true : Cells[iiii].Control.Parameters.PagingEnabled;
                                // if ( Cells[iiii].Control.ControlType.toLowerCase() == 'subgrid') {
                                dataSubgrid = ' data-pagesize="' + Cells[iiii].Control.Parameters.PageSize + '" data-editable="' + subeditable + '" data-viewid="' + Cells[iiii].Control.Parameters.ViewId + '" data-relationshipname="' + Cells[iiii].Control.Parameters.RelationshipName + '" data-rowcount="' + (Cells[iiii].Control.Parameters.DefaultEmptyRows || 5) + '"  data-ispager="' + PagingEnabled + '"  data-targetentityname="' + Cells[iiii].Control.Parameters.TargetEntityName + '"' + formularStr;
                                //}
                            } else {
                                Cells[iiii].Control.Formula && (formulaP = " data-formulajson='" + Cells[iiii].Control.Formula + "'");
                            }

                            if (typeof Cells[iiii].Control.ControlType === 'string' && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'lookup') {
                                if (Cells[iiii].Control.Parameters) {
                                    var IsFilterRelation = false;
                                    if (Cells[iiii].Control.Parameters.FilterRelationshipName && Cells[iiii].Control.Parameters.FilterRelationshipName != "") {
                                        IsFilterRelation = true;
                                    }

                                    dataLookup += ' data-defaultviewreadonly="' + (Cells[iiii].Control.DefaultViewReadOnly || '') + '" data-defaultviewid="' + (Cells[iiii].Control.Parameters.DefaultViewId || '') + '" data-viewpickerreadonly="' + (Cells[iiii].Control.Parameters.ViewPickerReadOnly || '') + '" data-disableviewpicker="' + (Cells[iiii].Control.Parameters.DisableViewPicker || '') + '" ';

                                    dataLookup += ' data-isfilterrelation="' + IsFilterRelation + '" data-filterrelationshipname="' + (Cells[iiii].Control.Parameters.FilterRelationshipName || '') + '" data-dependentattributetype="' + (Cells[iiii].Control.Parameters.DependentAttributeType || '') + '" data-dependentattributename="' + (Cells[iiii].Control.Parameters.DependentAttributeName || '') + '" data-allowfilteroff="' + (Cells[iiii].Control.Parameters.AllowFilterOff || false) + '"';
                                } else {
                                    dataLookup += ' data-filterrelationshipname="" data-dependentattributetype="" data-dependentattributename="" data-isfilterrelation="false" data-allowfilteroff="false" ';
                                }
                            }
                            var iframestr = '';
                            if (typeof Cells[iiii].Control.ControlType === 'string' && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'iframe') {
                                iframestr += ' data-url="' + Cells[iiii].Control.Parameters.Url + '" data-allowcrossdomain="' + Cells[iiii].Control.Parameters.AllowCrossDomain + '" data-border="' + Cells[iiii].Control.Parameters.Border + '" data-rowsize="' + Cells[iiii].Control.Parameters.RowSize + '" ';
                            }

                            var labelflag = '', freetextflag = '';
                            if (typeof Cells[iiii].Control.ControlType != 'undefined' && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'label') {
                                labelflag = ' labelbox';
                                //  console.log(Cells[iiii].Control.Parameters)
                                if (Cells[iiii].Control.Parameters) {
                                    //     console.log(Cells[iiii].Control.Parameters.EntityName)
                                    labelparam += ' data-labelentityname="' + Cells[iiii].Control.Parameters.EntityName + '" data-attributename="' + (Cells[iiii].Control.Parameters.AttributeName) + '" data-sourceattributename="' + (Cells[iiii].Control.Parameters.SourceAttributeName) + '" data-sourceattributetype="' + (Cells[iiii].Control.Parameters.SourceAttributeType) + '"';
                                } else {
                                    labelparam += ' data-labelentityname="" data-attributename="" data-sourceattributename=""';
                                }
                                //  console.log(labelparam)
                            } else if (typeof Cells[iiii].Control.ControlType != 'undefined' && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'freetext') {
                                freetextflag = ' freetextbox';
                                //  console.log(Cells[iiii].Control.Parameters)
                                if (Cells[iiii].Control.Parameters) {
                                    //     console.log(Cells[iiii].Control.Parameters.EntityName)
                                    freetextparam += ' data-labelentityname="' + Cells[iiii].Control.Parameters.EntityName + '" data-attributename="' + (Cells[iiii].Control.Parameters.AttributeName) + '" data-sourceattributename="' + (Cells[iiii].Control.Parameters.SourceAttributeName) + '" data-paramcontent="' + (Cells[iiii].Control.Parameters.Content || '') + '" data-sourceattributetype="' + (Cells[iiii].Control.Parameters.SourceAttributeType) + '"';
                                } else {
                                    freetextparam += ' data-labelentityname="" data-attributename="" data-sourceattributename=""';
                                }
                                //  console.log(labelparam)
                            } else {
                                if (Cells[iiii].Control.Parameters) {
                                    extParam += ' data-fieldentityname="' + (Cells[iiii].Control.Parameters.EntityName || '') + '" data-fieldattributename="' + (Cells[iiii].Control.Parameters.AttributeName || '') + '" data-fieldsourceattributename="' + (Cells[iiii].Control.Parameters.SourceAttributeName || '') + '"';
                                } else {
                                    extParam += ' data-fieldentityname="" data-fieldattributename="" data-fieldsourceattributename=""';
                                }
                            }
                            var customcss = '';
                            if (Cells[iiii].CustomCss) {
                                customcss = ' data-customcss=\'' + Cells[iiii].CustomCss + '\' '
                            }
                            panelHtml.push('<td class="col-sm-' + (Cells[iiii].ColSpan * 2) + ' field ui-droppable ui-sortable' + labelflag + ' ' + freetextflag + '" colspan="' + Cells[iiii].ColSpan + '">');
                            panelHtml.push('<table class="table cell"  data-label="' + Cells[iiii].Label + '" data-name="' + Cells[iiii].Control.Name + '" data-isreadonly="' + readonly + '" data-entityname="' + Cells[iiii].Control.EntityName + '" data-type="" data-isshowlabel="' + Cells[iiii].IsShowLabel + '" data-isvisible="' + Cells[iiii].IsVisible + '" data-colspan="' + Cells[iiii].ColSpan + '" data-controltype="' + Cells[iiii].Control.ControlType + '" data-timestamp="" data-defaultviewreadonly="true" data-viewpickerreadonly="true" ' + customcss + ' data-disableviewpicker="true" ' + dataSubgrid + dataLookup + labelparam + freetextparam + extParam + iframestr + formulaP + '><tbody><tr style="height:100%;">');
                            // console.log(Cells[iiii].Label, Cells[iiii].IsShowLabel=='true', Cells[iiii].IsVisible)
                            if (typeof Cells[iiii].Control.ControlType != 'undefined' && Cells[iiii].Control.ControlType.toString().toLowerCase() == 'none') {
                                //if (Cells[iiii].Control.ControlType.toLowerCase() == 'none') {
                                panelHtml.push('<th class="col-sm-3 ' + ((Cells[iiii].IsShowLabel == 'true' || Cells[iiii].IsShowLabel == true) ? '' : ' disable-text') + ((Cells[iiii].IsVisible == 'true' || Cells[iiii].IsVisible == true) ? '' : ' visible-hidden') + '">空格</th><td  class="col-sm-4">空格</td></tr></tbody></table>');
                                // }
                            } else {
                                panelHtml.push('<th class="col-sm-3  ' + ((Cells[iiii].IsShowLabel == 'true' || Cells[iiii].IsShowLabel == true) ? '' : ' disable-text') + ((Cells[iiii].IsVisible == 'true' || Cells[iiii].IsVisible == true) ? '' : ' visible-hidden') + '">' + Cells[iiii].Label + '</th><td  class="col-sm-4">' + Cells[iiii].Label + '</td></tr></tbody></table>');
                            }
                            panelHtml.push('</td>');
                            if (itemcolspan > 1) {
                                // iiii += itemcolspan * 1 - 1;
                            }
                        } else {
                            cellsCurIndex += 1;
                            panelHtml.push('<td class="col-sm-2 field ui-droppable ui-sortable" colspan="1">');
                            panelHtml.push('<table class="table cell"  data-label="none" data-name=" " data-entityname="none" data-type="" data-isedit="0" data-isshowlabel="true" data-isvisible="true" data-colspan="1" data-controltype="none" ><tbody><tr style="height:100%;">');
                            panelHtml.push('<th class="col-sm-3">空格</th><td  class="col-sm-4">空格</td></tr></tbody></table>');
                            panelHtml.push('</td>');
                            //panelHtml.push('<td class="col-sm-2 field ui-droppable placeholder ui-sortable" colspan="1">');
                        }
                    }
                    console.log('cellsCurIndex', cellsCurIndex, Columns)
                    if (cellsCurIndex < Columns) {
                        var moreCount = Columns - cellsCurIndex;
                        for (var iiii = 0; iiii < moreCount; iiii++) {
                            panelHtml.push('<td colspan="1" class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>');
                        }
                    }
                    //if (Cells.length % Columns != 0) {
                    //    for (var iiii = 0; iiii < Columns - (Cells.length % Columns) ; iiii++) {
                    //            panelHtml.push('<td colspan="1" class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>');

                    //   }
                    //}
                }
                panelHtml.push('</tr>');
            }
            panelHtml.push('</tbody></table>');
            panelHtml.push('</div>');
        }
        panelHtml.push('</div>');
        panelHtml.push(' </div>');
    }
    $('#formBody').append(panelHtml.join(''));
}

function filterInsertEdAttri(items) {
    var res = [];
    if (loadedEntitys && loadedEntitys.length > 0) {
        // console.log(items)
        if (items) {
            res = $.grep(items, function (item, key) {
                var flag = true;
                $.each(loadedEntitys, function (i, obj) {
                    if (item.name == obj || item.name.toLowerCase() == obj) {
                        flag = false;
                        return false;
                    }
                });
                return flag;
            });
        }
    }
    // console.log(res);
    return res;
}

//加载字段
function loadAttributes(callback) {
    Xms.Schema.GetAttributesByEntityId(entityid, function (data) {
        attributes = data;

        loadAttributesFormula(attributes);	//加载公式可选字段
        bindAttributes(attributes);
        var Enattrs = filterInsertEdAttri(attributes);
        if (Enattrs.length > 0) {
            bindAttributes(Enattrs);
        }
        callback && callback();
    });
}
function loadScript(type) {
    Xms.Web.OpenDialog('/customize/WebResource/Dialog?WebResourceType=Script&SolutionId=' + $("#SolutionId").val() + '&inputid=' + type + '', 'ScriptCallBack');
}

function insertToAttributeList(e, res) {
    var reid = '';
    Xms.Schema.GetAttributes({ getall: true, entityid: entityid }, function (data) {
        attributes = data;
        $.each(attributes, function (key, obj) {
            if (obj.attributeid == res) {
                reid = obj;
            }
        });

        $("#attributes").append('<li class="ui-state-default ui-draggable" data-name="' + reid.name + '" data-localizedname="' + reid.localizedname + '" data-entityname="' + reid.entityname + '" data-type="' + reid.attributetypename + '" >' + reid.localizedname + '</li>');
        newAttributeModal.modal('hide');

        initAttributeEvent();
        initSortEvent();
    });
}
var newAttributeModal = $("#newAttributeModal");
//新建字段
function createNewAttribute() {
    var link = ORG_SERVERURL + '/customize/attribute/createattribute?solutionid=' + SolutionId + '&entityid=' + entityid;
    $("#newAttributframe").attr("src", link);
    Xms.Web.Event.clearBy('success');
    Xms.Web.Event.subscribe('success', function (e, n) {
        // console.log(e.data);
        var resid = e.data.split("&&&")[1];
        insertToAttributeList(e, resid);
    }, true);

    newAttributeModal.modal('show');
}
function bindAttributes(items) {
    var _html = new Array();
    $(items).each(function (i, n) {
        if (n.attributetypename == 'primarykey' || n.attributetypename == 'timestamp') return true;
        _html.push('<li class="ui-state-default" data-name="' + n.name + '" data-localizedname="' + n.localizedname + '" data-entityname="' + n.entityname + '" data-type="' + n.attributetypename + '">');
        _html.push(n.localizedname);
        _html.push('</li>');
    });
    //console.log(_html)
    $('#attributes').html(_html.join(''));
    initAttributeEvent();
    //initFieldEvent();
    initSortEvent();
}
//设置字段.field节点属性
function setFieldAttrs(target, attrs) {
    if (attrs) {
        target.attr('data-label', attrs.label || '');
        target.attr('data-name', attrs.name || '');
        target.attr('data-entityname', attrs.entityname || '');
        target.attr('data-entityid', attrs.entityid || '');
        target.attr('data-type', attrs.type || '');
        target.attr('data-isshowlabel', attrs.isshowlabel || true);
        target.attr('data-isvisible', attrs.IsVisible || true);
        target.attr('data-colspan', attrs.colspan || 1);
        attrs.type = attrs.type || '';
        if (attrs.type.toLowerCase() == 'lookup' || attrs.type.toLowerCase() == 'owner' || attrs.type.toLowerCase() == 'customer') {
            target.attr('data-controltype', attrs.type);
            target.attr('data-dependentattributename', '');
            target.attr('data-filterrelationshipname', '');
            target.attr('data-allowfilteroff', false);
        }
        else {
            target.attr('data-controltype', 'standard');
        }
    }
    else {
        target.removeAttr('data-label').removeAttr('data-name').removeAttr('data-entityname').removeAttr('data-type').removeAttr('data-isshowlabel').removeAttr('data-isvisible').removeAttr('data-colspan').removeAttr('data-controltype');
    }
}

function getActiveMoveTd(pos, psize) {
    var context = $("#formContent");
    var tdlist = context.find("td.field");
    var activetd = null;
    tdlist.each(function (key, obj) {
        var $this = $(this);
        var offset = $this.offset();
        var size = { w: $this.width(), h: $this.height() };
        if (pos.left > offset.left
            && pos.left < offset.left + size.w
            && pos.top > offset.top
            && pos.top < offset.top + size.h) {
            activetd = $this;
            return false;
        }
    });
    return activetd;
}

//初始化字段拖动事件
function initAttributeEvent() {
    var activeTd = null;
    var activeField = null;
    var changeField = null;
    $("#attributes li").draggable({
        appendTo: "body",
        helper: "clone",
        revert: true,
        start: function (event, ui) {
            // console.log(event, ui);
            var sour = ui.helper, target = $(event.target);
            var context = $("#formContent");
            var tdlist = context.find("td.field");
            sour.bind("drag.removeSelf", function () {
                target.remove();
            });
            sour.bind("drag.removeClass", function () {
                tdlist.children("table").removeClass("activetd-top").removeClass("activetd-bottom");
            });
            if (activeTd) {
                activeTd.children("table").removeClass("activetd-top").removeClass("activetd-bottom");
            }
        },
        drag: function (event, ui) {
            // if (!ui.draggable.parent().is('#attributes')) return;
            var sour = ui.helper, target = $(event.target);
            sour.addClass('btn btn-primary');
            var context = $("#formContent");
            var tdlist = context.find("td.field");
            tdlist.children("table").removeClass("activetd-top").removeClass("activetd-bottom");
            var sortOffset = sour.offset();
            activeTd = getActiveMoveTd(sortOffset, { w: sour.width(), h: sour.height() });
            if (activeTd) {
                activeField = activeTd.children('table');
                var limi = activeTd.offset().top + (activeTd.height() / 2);
                if (sortOffset.top > limi) {
                    activeTd.children("table").addClass("activetd-bottom").removeClass("activetd-top");
                } else {
                    activeTd.children("table").addClass("activetd-top").removeClass("activetd-bottom");
                }
            }
        },
        stop: function (event, ui) {
            if ($('.modal.fade.ui-draggable.in').length > 0) return;
            var sour = ui.helper, target = $(event.target);
            ui.helper.trigger("drag.removeClass");
            var offset = ui.offset;
            var item = ui.helper;
            if (item.hasClass('section') || item.hasClass('modal')) return false;
            var tar = $(event.target);
            console.log(event);
            var offsetType = 'after';
            var tarOffset = tar.offset();
            if (tarOffset.top > offset.top) {
                offsetType = "after";
            } else {
                offsetType = "before";
            }
            if (activeTd && activeTd.length > 0) {
                var activeFieldPar = activeField.parent();
                changeField = activeTd.children();
                if (changeField && changeField.length > 0) {
                    var fieldcolspan = changeField.attr('data-colspan');
                    var activecolspan = activeField.attr('data-colspan');
                    if (fieldcolspan != activecolspan) {
                        activeFieldPar.append(changeField);
                        activeTd.append(activeField);
                    }
                }
            }
        }
    });
}

//字段拖动排序
function initFieldEvent() {
    var sortModel = {
        startEle: null,
        startEleColspan: 0,
        startElePar: null
    };
    var distModel = {
        distEle: null,
        distEleColspan: 0,
        distElePar: null
    }

    var dropModel = {}

    $(".field", "#editform")
        .droppable({
            //activeClass: "drag-placeholder",
            //hoverClass: "drag-placeholder",
            accept: ":not(.ui-sortable-helper)",
            tolerance: "intersect",
            over: function (event, ui) {
                //if (!ui.draggable.parent().is('#attributes')) return;
                //var sour = ui.helper, target = $(event.target);
                //console.log(sour);
                //console.log(target);
                //var sortOffset = sour.offset(), distOffest = target.offset();
                //dropModel.sortType = "bottom";

                //if (sortOffset.top > distOffest.top) {
                //    dropModel.sortType = "bottom";//放到目标位置的下面
                //} else {
                //    dropModel.sortType = "top";
                //}

                //var parent = $(event.target);
                //// if (sortType == "top") {
                ////     parent.addClass("redBorder");
                //// } else {
                //parent.css({ 'border-bottom': '1px red solid' });
                //// }
            },
            out: function (event, ui) {
                //if (!ui.draggable.parent().is('#attributes')) return;
                //var parent = $(event.target);
                //parent.css({ 'border-bottom': '' }).removeClass("redBorder");
            },
            drop: function (event, ui) {
                // if (!ui.helper.parent().is('#attributes')) return;

                ui.helper.trigger("drag.removeClass");
                var offset = ui.offset;
                var item = ui.helper;
                if (item.hasClass('section') || item.hasClass('modal')) return false;
                var tar = $(event.target);
                console.log(event);
                var offsetType = 'after';
                var tarOffset = tar.offset();
                if (tarOffset.top > offset.top) {
                    offsetType = "after";
                } else {
                    offsetType = "before";
                }
                var text = ui.draggable.text();
                var el = $('<table class="table cell"><tr style="height:100%;"><th class="col-sm-3">' + text + '</th><td  class="col-sm-4">' + text + '</td></tr></table');

                var parent = $(event.target);
                parent.css({ 'border-bottom': '' });
                var columns = parent.parents('.section').attr('data-columns');//列数
                //如果上一个单元为空时
                if (parent.parent().prev().find('td.field:eq(' + parent.parent().find('.field').index(parent) + ')').filter('.placeholder').length > 0) {
                    return false;
                }
                var fieldIndex = getCurrentTdIndex(parent);//parent.parent().find('td.field').index(parent);
                if (!parent.is('.placeholder')) {//如果移动到一个已有字段的单元格时
                    var elArr = [];
                    elArr.push('<tr>');
                    for (var i = 0; i < columns; i++) {
                        elArr.push('<td class="col-sm-2 field placeholder"></td>');
                    }
                    elArr.push('</tr>');
                    var newel = $(elArr.join(''));
                    var cell = newel.find('td.field:eq(' + fieldIndex + ')');
                    cell.removeClass('placeholder');
                    setFieldAttrs(el, {
                        label: item.attr('data-localizedname')
                        , name: item.attr('data-name')
                        , entityname: item.attr('data-entityname')
                        , type: item.attr('data-type')
                        , isshowlabel: true
                    });
                    if (parent.parents('tr:first').next().length > 0) {//如果有下一行时
                        var td = ui.helper.parents('tr:first').next().find('td.placeholder:eq(' + fieldIndex + ')');
                        if (td.length > 0) {//如果下一行有空单元时
                            td.append(parent.find('.cell:last')).removeClass('placeholder');
                        }
                        else {
                            cell.append(parent.find('.cell:last'));
                            parent.parents('tr:first').after(newel);//添加新行
                        }
                    }
                    else {
                        cell.append(parent.find('.cell:last'));
                        parent.parents('tr:first').after(newel);
                    }
                }
                setFieldAttrs(el, {
                    label: ui.draggable.attr('data-localizedname')
                    , name: ui.draggable.attr('data-name')
                    , entityname: ui.draggable.attr('data-entityname')
                    , type: ui.draggable.attr('data-type')
                    , isshowlabel: true
                });

                $(this).append(el);
                $(this).removeClass("placeholder");
                ui.helper.trigger("drag.removeSelf");
                ui.helper.remove();
                var trRows = el.parents("table.table.table-bordered").eq(0).find(">tbody>tr");

                resetTableVis(trRows);
                delEmptyTr(trRows)
                initSortEvent();
                initFieldEvent();//重新绑定事件
                //$("#attributes").css({ "height": "400", "overflow-y": "auto" });
            }
        })
        .sortable({
            connectWith: ".field",
            cancel: '.placeholder',
            items: ".cell",
            //placeholder: "drag-placeholder",
            over: function (event, ui) {
                // if (!ui.item.is('.cell')) return;
                var parent = $(event.target);
                parent.css({ 'border-bottom': '1px red solid' });
            },
            out: function (event, ui) {
                //  if (!ui.item.is('.cell')) return;
                var parent = $(event.target);
                parent.css({ 'border-bottom': '' });
            },
            start: function (event, ui) {
                //console.log(event)
                if (event.stopPropagation) {
                    event.stopPropagation();
                }
                sortModel.startEle = ui.item;
                sortModel.startElePar = sortModel.startEle.parent();
                sortModel.startEleColspan = ui.item.attr("data-colspan") || 1;
                sortModel.startEleIndex = sortModel.startElePar.index();
                sortModel.startEleWrap = sortModel.startEle.parents(".section");
                sortModel.startEleWrapColspan = sortModel.startEleWrap.attr("data-columns");
            },
            update: function (event, ui) {
            },
            stop: function (event, ui) {
                distModel.distEle = ui.item.siblings();
                if (distModel.distEle.length == 0) {
                    distModel.distEle = ui.item.parent();
                    distModel.distElePar = distModel.distEle;
                    distModel.distEleColspan = distModel.distEle.attr("colspan") || 1;
                    distModel.distEleIndex = distModel.distEle.index();
                    distModel.distEleWrap = distModel.distEle.parents(".section");
                    distModel.distEleWrapColspan = distModel.distEleWrap.attr("data-columns");
                } else {
                    distModel.distElePar = distModel.distEle.parent();
                    distModel.distEleColspan = distModel.distEle.attr("data-colspan") || 1;
                    distModel.distEleIndex = distModel.distElePar.index();
                    distModel.distEleWrap = distModel.distEle.parents(".section");
                    distModel.distEleWrapColspan = distModel.distEleWrap.attr("data-columns");
                }
                if (distModel.distElePar.get(0) == sortModel.startElePar.get(0)) return false;
                var sortOffset = sortModel.startEle.offset(), distOffest = distModel.distEle.offset();
                var columns = distModel.distEleWrap.attr("data-columns");
                var sortType = "bottom";
                if (sortOffset.top > distOffest.top) {
                    sortType = "bottom";//放到目标位置的下面
                } else {
                    sortType = "top";
                }
                var move = ui.item.attr("data-colspan");
                var flag = true;
                switch (move) {
                    case "1":
                        flag = moveFieldEvent.one(sortModel, distModel, sortType, columns);
                        break;
                    case "2":
                        flag = moveFieldEvent.two(sortModel, distModel, sortType, columns);
                        break;
                    case "3":
                        flag = moveFieldEvent.three(sortModel, distModel, sortType, columns);
                        break;
                    case "4":
                        flag = moveFieldEvent.four(sortModel, distModel, sortType, columns);
                        break;
                }
                var distEleWrap = distModel.distEleWrap;
                var trRows = distEleWrap.children('table').find(">tbody>tr");
                if (flag == false) {
                    return false;
                } else {
                    resetTableVis(trRows);
                    delEmptyTr(trRows)
                    initSortEvent();
                    initFieldEvent();//重新绑定事件
                }

                initFieldEvent();//重新绑定事件
                //return false;
            }
        }).disableSelection();
}
//初始化表单元素排序事件
function initSortEvent() {
    //导航链接排序
    var formNavGM = {};
    $("#formNav").sortable({
        cancel: '.locked',
        items: '.navGroup',
        placeholder: "drag-placeholder",
        helper: "clone",
        tolerance: "intersect",
        //containment: "parent",
        start: function (event, ui) {
            var drager = ui.helper, sourer = ui.item;
            formNavGM.par = sourer.parent();
        },
        over: function (event, ui) {
            var drager = ui.helper, sourer = ui.item;
            if (ui.item.hasClass("nav-item")) {
                if (formNavGM.par.children(".nav-item").length == 0
                    || ui.item.siblings(".navGroup").length > 0) {
                    return false;
                }
            }
        },
        stop: function (event, ui) {
            var drager = ui.helper, sourer = ui.item;
            if (ui.item.parents(".locked").length > 0) {
                return false;
            }
            if (ui.item.hasClass("navGroup")) {
                if (ui.item.parents(".navGroup").length > 0) {
                    return false;
                }
            } else if (ui.item.hasClass("nav-item")) {
                if (formNavGM.par.children(".nav-item").length == 0
                    || ui.item.siblings(".navGroup").length > 0) {
                    return false;
                }
            }
            initSortEvent();
        }
    }).disableSelection();
    var formNavM = {};
    $("#formNav").find("div[id^=collapseNav]").each(function () {
        $(this).sortable({
            cancel: '.locked',
            items: '.nav-item',
            connectWith: "div[id^=collapseNav]",
            placeholder: "drag-placeholder",
            helper: "clone",
            tolerance: "intersect",
            //containment: "parent",
            start: function (event, ui) {
                var drager = ui.helper, sourer = ui.item;
                formNavM.par = sourer.parent();
            },
            over: function (event, ui) {
            },
            stop: function (event, ui) {
                var drager = ui.helper, sourer = ui.item;
                if (ui.item.hasClass("nav-item")) {
                    if (formNavM.par.children(".nav-item").length == 0) {
                        //没有的时候要添加一个填充的空格
                    } else if (formNavM.par.children(".nav-item").length == 1 && ui.item.hasClass("drag-placeholder")) {
                        //如果只有一个填充的空格
                    }
                }
                initSortEvent();
            }
        }).disableSelection();
    });

    var sourMS = {};
    $("#formBody").find('div[id^=tab_]').each(function () {
        var self = this;
        $(this).sortable({
            connectWith: "div[id^=tab_]",
            cancel: '.locked',
            items: '.section',
            placeholder: "drag-placeholder",
            helper: "clone",
            //containment: "#formBody",
            start: function (event, ui) {
                var drager = ui.helper, sourer = ui.item;
                sourMS.par = sourer.parent();
            },
            stop: function (event, ui) {
                var drager = ui.helper, sourer = ui.item;
                if (ui.item.hasClass("tab")) {
                    if (ui.item.parents(".tab").length > 0) {
                        return false;
                    }
                } else if (ui.item.hasClass("section")) {
                    if (sourMS.par.children(".section").length == 0) {
                        return false;
                    }
                }
                initSortEvent();
                initFieldEvent();
            }
        }).disableSelection();
    });
    var sourM = {};
    $("#formBody").each(function () {
        $(this).sortable({
            // connectWith: "#formBody .tab",
            cancel: '.locked',
            items: '.tab',
            placeholder: "drag-placeholder",
            helper: "clone",
            // containment: "#formBody",
            start: function (event, ui) {
            },
            stop: function (event, ui) {
                initFieldEvent();
                initSortEvent();
            }
        }).disableSelection();
    });
}
//空表单元素处理
function handleEmptyElement(field) {
    var fieldIndex = field.parent().find('td.field').index(field);
    var section = field.parents('table:first');
    //查找下面所有单元格，并上移
    var nextall = field.parent().nextAll().find('td.field:eq(' + fieldIndex + ')').not('.placeholder');
    if (nextall.length > 0) {
        var nextallrow = field.parent().nextAll();
        field.removeClass('placeholder');//上一个单元格
        $(nextallrow).each(function (i, n) {
            if (i >= nextall.length) return false;
            var row = $(n);
            field.replaceWith($(nextall[i]).clone());
            field = row.find('td.field:eq(' + fieldIndex + ')');
            $(nextall[i]).empty();//.addClass('placeholder');
            setFieldAttrs($(nextall[i]), null);
        });
    }
    //空行处理
    var rows = section.find('> tbody > tr');
    if (rows.length > 1) {
        rows.each(function (i, n) {
            var row = $(n);
            if (row.find('.cell').length == 0) {
                if (section.find('> tbody > tr').length > 1) {//保留一行
                    row.remove();
                }
            }
        });
    }
}
//单元格所占列数
function handleFieldColspan(field) {
    var cols = field.children().attr('data-colspan');
    if (cols == 1) {
        var ocols = field.prop('colspan');//原有的列数
        field.prop('colspan', cols);
        if (ocols > 1) {
            for (var i = 0; i < ocols - cols; i++) {
                field.after('<td class="col-sm-2 field placeholder"></td>');
            }
            handleEmptyElement(field.next());
        }
    }
    else if (cols == 2) {
        //右边单元为空时
        if (field.next().is('.placeholder')) {
            field.next().remove();
            field.prop('colspan', cols);
        }
        else if (field.prev().is('.placeholder')) {//向左挤
            field.prev().remove();
            field.prop('colspan', cols);
        }
        else {
            //右边元素下移
            var rightField = field.next();
            //如果右边元素存在，则向右挤
            if (rightField.length > 0) {
                var rightFieldIndex = 1;
                field.parent().nextAll().each(function (i, n) {//下面所有行
                    var row = $(n);
                    var o = row.find('td.field:eq(' + rightFieldIndex + ')');//当前行中的同位置元素
                    var tmp = o.clone();
                    o.replaceWith(rightField.clone());
                    rightField = tmp;
                });
                var columns = field.parents('.section:first').attr('data-columns');
                var elArr = [];
                elArr.push('<tr>');
                for (var i = 0; i < columns; i++) {
                    elArr.push('<td class="col-sm-2 field placeholder"></td>');
                }
                elArr.push('</tr>');
                var el = $(elArr.join(''));
                var cell = el.find('td:eq(' + rightFieldIndex + ')').removeClass('placeholder');
                setFieldAttrs(cell, {
                    label: field.next().children().attr('data-label')
                    , name: field.next().children().attr('data-name')
                    , entityname: field.next().children().attr('data-entityname')
                    , type: field.next().children().attr('data-type')
                    , isshowlabel: field.next().children().attr('data-isshowlabel')
                });
                cell.replaceWith(rightField.clone());
                field.parents('table:first').append(el);
                field.next().remove();
            }
            else {//向左挤
            }
            field.prop('colspan', cols);
            if (field.children("td").length > 0) {
                field.children("td").attr("data-colspan", cols);
            }
            initFieldEvent();
        }
    }
    if (cols == 3) {
    }
}
//插入表格
function insertSection(col) {
    var selected = $('#formBody').find('.selected');
    var temp = new Array();
    temp.push('<div class="section" data-columns="' + col + '" data-name="' + new Date().getTime() + '" data-label="表格" data-isvisible="true" data-isshowlabel="false" data-celllabelwidth="100" data-celllabelalignment="Left" data-celllabelposition="Left"><div class="section-title table-title title hidden-text">表格</div>');
    temp.push('<table class="table table-bordered">');
    temp.push('<tr>');
    for (var i = 0; i < col; i++) {
        temp.push('<td class="col-sm-2 field placeholder"></td>');
    }
    temp.push('</tr>');
    temp.push('</table>');
    temp.push('</div>');
    var el = $(temp.join(''));
    if (selected.length > 0) {
        if (selected.hasClass('section')) {
            selected.after(el);
        } else if (selected.hasClass('cell')) {
            selected.parents('.section').after(el);
        } else if (selected.hasClass('tab')) {
            selected.append(el);
        }
    } else {
        $('#formBody').find('.tab:first').append(el);
    }
    el.click();
    initSortEvent();
    initFieldEvent();
}
//插入选项卡
function insertTab(col, width) {
    var id = 'tab_' + Math.round(new Date().getTime() / 1000);
    var selected = $('#formBody').find('.selected');
    var temp = new Array();
    temp.push('<div class="tab" id="' + id + '" data-label="选项卡"  data-name="' + new Date().getTime() + '"  data-isshowlabel="true" data-isexpanded="true" data-displaystyle="1" data-isvisible="true">');
    temp.push('<a data-toggle="collapse"');
    temp.push('href="#collapse_' + id + '" class="collapse-title tab-title">');
    temp.push('<span class="glyphicon glyphicon-chevron-down"></span><span class="title">选项卡</span>');
    temp.push('</a>');
    temp.push('<div id="collapse_' + id + '" class="panel-collapse collapse in">');
    temp.push('<div class="section" data-columns="' + col + '" data-label="表格" data-isvisible="true" data-isshowlabel="true" data-celllabelwidth="100" data-celllabelalignment="Left" data-celllabelposition="Left"><div class="section-title content-title title">表格</div>');
    temp.push('<table class="table table-bordered">');
    temp.push('<tr>');
    for (var i = 0; i < col; i++) {
        temp.push('<td class="col-sm-2 field placeholder"></td>');
    }
    temp.push('</tr>');
    temp.push('</table>');
    temp.push('</div>');
    temp.push('</div>');
    temp.push('</div>');
    var el = $(temp.join(''));
    if (selected.length > 0) {
        //insert after
        if (selected.is('.section') || selected.is('.cell')) {
            selected.parents('.tab').after(el);
        }
        else if (selected.is('.tab')) {
            selected.after(el);
        }
    }
    else {
        $('#formBody').append(el);
    }
    el.click();
    initSortEvent();
    initFieldEvent();
}
//插入单据体
function insertBill() {
    var target = $('#subGridModal');
    //Entities
    target.attr('data-isnew', 'true');
    target.find('#subgrid-name').val('');
    target.find('#subgrid-label').val('');
    target.find('#subgrid-pagesize').val('5');
    target.find('#subgrid-entity').empty();
    target.find('#subgrid-viewid').empty();
    target.find('input[name="subgrid-record"]:checked').trigger('change');
    target.find('#subgrid-isshowlabel').prop('checked', false);
    target.find('#subgrid-isvisible').prop('checked', 'checked');
    $('#subgrid-editable').change(function () {
        if ($(this).prop('checked') == true) {
            $('#subgrid-rowcount-input').show();
        } else {
            $('#subgrid-rowcount-input').hide();
        }
    });
    $('#subgrid-ispager').change(function () {
        if ($(this).prop('checked') == true) {
            $('#subgrid-pagesize-input').show();
        } else {
            $('#subgrid-pagesize-input').hide();
        }
    });
    $('#subgrid-entityedit').empty();
    $('#subgrid-ext-edit').hide();
    target.modal('show');
}
function getViewid(e, callback, type) {
    var that = $(e);
    var entityid = that.find('option:selected').val();
    var target = $('#subGridModal');
    type = type || 'edit';
    var isNew = target.attr('data-isnew');
    if (entityid == '') {
        target.find('#subgrid-viewid').empty();
        return false;
    }
    var html = [];
    Xms.Web.GetJson('/customize/QueryView/index?EntityId=' + entityid + '&getall=true&LoadData=true', null, function (data) {
        $(data.content.items).each(function (i, n) {
            html.push('<option value="' + n.queryviewid + '">' + n.name + '</option>');
        });
        target.find('#subgrid-viewid').html(html.join(''));

        if (typeof (callback) == "function") {
            callback.call(this);
        }
        if (isNew && isNew == 'true') {
            if ($('#tempSubgridnew').length > 0) {
                $('#tempSubgridnew').remove();
                $('<div id="tempSubgridnew" />').appendTo($('body'));
            } else {
                $('<div id="tempSubgridnew" />').appendTo($('body'));
            }
            subGridchangeView($('#tempSubgridnew'), type);
        } else {
            $('#subgrid-viewid').trigger('change', { type: type });
            //subGridchangeView($('.selected'),type);
        }
    });
}
function getEntity(e, callback) {
    var html = [];
    var that = $(e);
    var target = $('#subGridModal');
    if (that.val() == 'related') {
        Xms.Web.GetJson('/api/schema/relationship/GetReferenced/' + entityid + '', null, function (data) {
            html.push('<option value="">请选择</option>');
            $(data.content).each(function (ii, nn) {
                html.push('<option value="' + nn.referencingentityid + '" data-relationshipname="' + nn.name + '">' + nn.referencingentitylocalizedname + '(' + nn.referencingattributelocalizedname + ')' + '</option>');
            });
            target.find('#subgrid-entity').html(html.join(''));
            if (typeof (callback) == "function") {
                callback.call(this);
            }
        });
    } else {
        Xms.Schema.GetEntities({}, function (data) {
            html.push('<option value="">请选择</option>');
            $(data).each(function (i, n) {
                html.push('<option value="' + n.entityid + '" data-relationshipname="">' + n.localizedname + '</option>');
            });
            target.find('#subgrid-entity').html(html.join(''));
            if (typeof (callback) == "function") {
                callback.call(this);
            }
        });
    }
}
function getFormularValue(arr) {
    var res = [];
    $.each(arr, function (key, nn) {
        if (nn.name == "+" || nn.name == "-" || nn.name == "*" || nn.name == "/" || nn.name == "=" || nn.name == "(" || nn.name == ")") {
            res.push(nn.name);
        } else {
            res.push(Math.floor(Math.random() * 30) + 5);
        }
    });
    return res.join("");
}
function checkFormular(formular) {
    if (!formular || formular.length == 0) { return true; }
    var flag = true;
    formularObj = JSON.parse(formular.toJson());
    $.each(formularObj, function (key, item) {
        var ruler = getFormularValue(item);
        console.log(ruler);
        try {
            var ruleritem = ruler.split("=");
            $.each(ruleritem, function (i, n) {
                eval(n);
            });
        } catch (e) {
            flag = false;
            return false;
        }
    });
    return flag;
}
function saveSubGrid() {
    var target = $('#subGridModal');
    var nameval = target.find('#subgrid-name').val();
    if (nameval == '') {
        Xms.Web.Alert(false, '请输入名称');
        return false;
    }
    if (/[\u4e00-\u9fa5]/g.test(nameval)) {
        Xms.Web.Alert(false, '不能输入中文');
        return false;
    }
    if (target.find('#subgrid-label').val() == '') {
        Xms.Web.Alert(false, '请输入标签');
        return false;
    }
    if (Xms.Web.SelectedValue(target.find('#subgrid-viewid')) == null) {
        Xms.Web.Alert(false, '请选择视图');
        return false;
    }
    //if (Xms.Web.SelectedValue(target.find('#subgrid-entityedit')) == -1) {
    //    Xms.Web.Alert(false, '请先设置字段');
    //    return false;
    //}
    // if (target.find('#subgrid-pagesize').val() == '' || target.find('#subgrid-pagesize').val() == 0) {
    //     Xms.Web.Alert(false, '行数请填写大于0的数字');
    //      return false;
    // }

    if (target.attr('data-isnew') == 'false') {
        var selected = $('#formBody').find('.selected');
        var selTd = selected;
    }
    else {
        $('#formBody').find('.selected').removeClass('selected');
        insertTab(1);
        $('#formBody').find('.selected').find('.section').click();
        insertSection(1);
        $('#formBody').find('.selected').siblings('.section').remove();

        var selected = $('#formBody').find('.selected');
        var selTd = selected.find("td:first");
        selTd.removeClass('placeholder');
        var html = [];
        html.push('<table class="table cell">');
        html.push('<tbody>');
        html.push('<tr style="height:100%;"><th class="col-sm-3">标签</th><td class="col-sm-4">标签</td></tr>');
        html.push('</tbody>');
        html.push('</table>');
        selTd.append($(html.join('')));
        selTd = selTd.children('table');
    }
    if (target.attr('data-isnew') == 'true') {
        selTd.parents('.tab:first').attr('data-label', target.find('#subgrid-label').val());
        selTd.parents('.tab:first').find('>a .title').text(target.find('#subgrid-label').val());
        if ($('#tempSubgridnew').length > 0) {
            selTd.data().formula = $('#tempSubgridnew').data().formula;
            selTd.data().lookuplistCache = $('#tempSubgridnew').data().lookuplistCache;
        }
    }
    if (($("#subgrid-entityedit>option:selected").val() != -1 && $("#subgrid-entityedit>option:selected").val() != '') && $("#subgrid-entityedit-type>option:selected").val() == 1) {
        if (target.attr('data-isnew') != 'true') {
            var curformula = selected.data().formula;
        } else {
            var curformula = selTd.data().formula;
        }
        if (!checkFormular(curformula)) {
            Xms.Web.Alert(false, '输入公式有误，请重新输入');
            return false;
        }
    }
    selTd.removeClass('placeholder');
    selTd.attr('data-label', target.find('#subgrid-label').val());
    selTd.attr('data-name', target.find('#subgrid-name').val());
    selTd.attr('data-entityname', target.find('#subgrid-entityname').val());
    selTd.attr('data-type', '');
    selTd.attr('data-controltype', 'subGrid');
    selTd.attr('data-isshowlabel', target.find('#subgrid-isshowlabel').prop('checked'));
    selTd.attr('data-isvisible', target.find('#subgrid-isvisible').prop('checked'));
    selTd.attr('data-editable', target.find('#subgrid-editable').prop('checked'));
    selTd.attr('data-viewid', Xms.Web.SelectedValue(target.find('#subgrid-viewid')));
    selTd.attr('data-pagesize', target.find('#subgrid-pagesize').val());
    selTd.attr('data-rowcount', target.find('#subgrid-rowcount').val());
    selTd.attr('data-ispager', target.find('#subgrid-ispager').prop('checked'));
    selTd.attr('data-colspan', 2);
    selTd.attr('data-targetentityname', Xms.Web.SelectedValue(target.find('#subgrid-entity')));
    selTd.attr('data-relationshipname', target.find('#subgrid-entity').find('option:selected').attr('data-relationshipname'));
    selTd.find('th').text(target.find('#subgrid-label').val());
    selTd.find('td').text(target.find('#subgrid-label').val());

    if (target.find('#subgrid-isshowlabel').prop('checked') == false) {
        selected.find('th').addClass("disable-text");
    } else {
        selected.find('th').removeClass("disable-text");
    }
    (function () {
        var viewid = $('#subgrid-viewid>option:selected').val();
        var curentdata = [];
        var lookupdataRes = [];
        if (target.attr('data-isnew') != 'true') {
            var currentformula = selected.data().formula;
        } else {
            var currentformula = selTd.data().formula;
        }

        if (currentformula) {
            for (var di = 0, dlen = currentformula.list.length; di < dlen; di++) {
                var ditem = currentformula.list[di];
                if (ditem.viewid == viewid) {
                    var arrtemp = [];
                    var name = ditem.id;//.replace(viewid, '');
                    var type = 'formular';
                    $.each(ditem.list, function (p, n) {
                        var objtemp = {};
                        objtemp.key = n.key;
                        objtemp.name = n.value;
                        arrtemp.push(objtemp.key);
                    });
                    if (arrtemp.length > 0) {
                        if (JSON.stringify(arrtemp) != '') {
                            curentdata.push({ name: name, expression: JSON.stringify(arrtemp), type: type, viewid: viewid });
                        }
                    }
                }
            };//获取值公式的数据;
            console.log(curentdata);
        }
        if (target.attr('data-isnew') != 'true') {
            var currentlookupdata = selected.data().lookuplistCache;
        } else {
            var currentlookupdata = selTd.data().lookuplistCache;
        }
        if (currentlookupdata) {
            var lookupdata = currentlookupdata;
            var temp = [];
            for (var iik in lookupdata) {
                if (lookupdata.hasOwnProperty(iik)) {
                    if (iik.indexOf(viewid) > -1) {
                        var expression = [];
                        $.each(lookupdata[iik], function (key, item) {
                            if (item) {
                                expression.push(item);
                            }
                        });
                        lookupdataRes.push({
                            type: 'append',
                            expression: JSON.stringify(expression),
                            name: iik.replace(viewid, ""),
                            viewid: viewid
                        });
                    }
                }
            }
        }

        var newres = [];

        if (curentdata.length > 0) {
            newres = newres.concat(curentdata);
        }
        if (lookupdataRes.length > 0) {
            newres = newres.concat(lookupdataRes);
        }
        //console.log(newres)
        if (target.attr('data-isnew') != 'true') {
            if (newres.length == 0) {
                selected.attr("data-formulaValue", '');
            } else {
                selected.attr("data-formulaValue", encodeURIComponent(JSON.stringify(newres)));
            }
        } else {
            if (newres.length == 0) {
                selTd.attr("data-formulaValue", '');
            } else {
                selTd.attr("data-formulaValue", encodeURIComponent(JSON.stringify(newres)));
            }
        }

        selected.removeAttr('data-isloaded');
    })();

    target.modal('hide');
    initSortEvent();
    initFieldEvent();
}
function subGridchangeView(selected, type) {
    if (selected.data().formula) {
        selected.data().formula.clear();
    }
    if (selected.data().lookuplistCache) {
        selected.data().lookuplistCache = [];
    }

    var subgrid_viewid = $("#subgrid-viewid>option:selected").val();
    subgridEntityEdit(subgrid_viewid, function (_html) {
        $("#subgrid-entityedit").html(_html);
        subgridEntityEdit(subgrid_viewid, function (_html) {
            $('#subgrid-formula-entitys').html(_html);
        }, 'type');
        subgridEntityEdit(subgrid_viewid, function (_html) {
            $('#subgrid-entityedit-ext').html(_html);
        }, 'all');
        subgridGetAttributes($('#subgrid-entityedit>option:selected').attr('data-referencedentityid'), function (_html) {
            $('#subgrid-entityedit-ext-res').html(_html);
        }, null, true);
        formulaEdit(selected, subgrid_viewid, type);
    });
}
var currentFormula = null;
var allFormulaWrap = new xmsFormHandler.xmsFormulaWrapList();
function editSubGrid() {
    var selected = $('.selected');
    var target = $('#subGridModal');
    target.attr('data-isnew', 'false');
    target.find('#subgrid-name').val(selected.attr('data-name'));
    target.find('#subgrid-label').val(selected.attr('data-label'));
    target.find('#subgrid-pagesize').val(selected.attr('data-pagesize') || 5);
    target.find('#subgrid-rowcount').val(selected.attr('data-rowcount') || 5);
    if (selected.attr('data-isshowlabel') == 'true') {
        target.find('#subgrid-isshowlabel').prop('checked', true);
    } else {
        target.find('#subgrid-isshowlabel').removeProp('checked');
    }
    if (selected.attr('data-isvisible') == 'true') {
        target.find('#subgrid-isvisible').prop('checked', true);
    } else {
        target.find('#subgrid-isvisible').removeProp('checked');
    }
    if (selected.attr('data-editable') == 'true') {
        target.find('#subgrid-editable').prop('checked', true);
        $('#subgrid-rowcount-input').show();
    } else {
        target.find('#subgrid-editable').removeProp('checked');
        $('#subgrid-rowcount-input').hide();
    }
    if (selected.attr('data-ispager') == 'true') {
        target.find('#subgrid-ispager').prop('checked', true);
        $('#subgrid-pagesize-input').show();
    } else {
        target.find('#subgrid-ispager').removeProp('checked');
        $('#subgrid-pagesize-input').hide();
    }
    $('#subgrid-editable').change(function () {
        if ($(this).prop('checked') == true) {
            $('#subgrid-rowcount-input').show();
        } else {
            $('#subgrid-rowcount-input').hide();
        }
    });
    $('#subgrid-ispager').change(function () {
        if ($(this).prop('checked') == true) {
            $('#subgrid-pagesize-input').show();
        } else {
            $('#subgrid-pagesize-input').hide();
        }
    });
    if (selected.attr('data-relationshipname') == '') {
        target.find('input[name="subgrid-record"][value="entity"]').prop("checked", true);
        getEntity(target.find('input[name="subgrid-record"]:checked'), function () {
            target.find('#subgrid-entity option[value="' + selected.attr('data-targetentityname') + '"]').attr("selected", true);

            getViewid(target.find('#subgrid-entity'), function () {
                target.find('#subgrid-viewid option[value="' + selected.attr('data-viewid') + '"]').attr("selected", true);

                var subgrid_viewid = $("#subgrid-viewid>option:selected").val();
                subgridEntityEdit(subgrid_viewid, function (_html) {
                    $("#subgrid-entityedit").html(_html);
                    subgridEntityEdit(subgrid_viewid, function (_html) {
                        $('#subgrid-formula-entitys').html(_html);
                    }, 'type');
                    subgridEntityEdit(subgrid_viewid, function (_html) {
                        $('#subgrid-entityedit-ext').html(_html);
                    }, 'all');
                    subgridGetAttributes($('#subgrid-entityedit>option:selected').attr('data-referencedentityid'), function (_html) {
                        $('#subgrid-entityedit-ext-res').html(_html);
                    }, null, true);
                    // formulaEdit(selected, subgrid_viewid);
                });
                $("#subgrid-viewid").off('change').on("change", function (e, param) {
                    subGridchangeView(selected, param && param.type || 'edit');
                });
            }, 'new');
        });
    }
    else {
        target.find('input[name="subgrid-record"][value="related"]').prop("checked", true);
        getEntity(target.find('input[name="subgrid-record"]:checked'), function () {
            var relationname = selected.attr('data-relationshipname');
            if (relationname) {
                target.find('#subgrid-entity option[value="' + selected.attr('data-targetentityname') + '"][data-relationshipname="' + relationname + '"]').attr("selected", true);
            } else {
                target.find('#subgrid-entity option[value="' + selected.attr('data-targetentityname') + '"]').attr("selected", true);
            }
            getViewid(target.find('#subgrid-entity'), function () {
                target.find('#subgrid-viewid option[value="' + selected.attr('data-viewid') + '"]').attr("selected", true);

                var subgrid_viewid = $("#subgrid-viewid>option:selected").val();
                subgridEntityEdit(subgrid_viewid, function (_html) {
                    $("#subgrid-entityedit").html(_html);
                    subgridEntityEdit(subgrid_viewid, function (_html) {
                        $('#subgrid-formula-entitys').html(_html);
                    }, 'type');
                    subgridEntityEdit(subgrid_viewid, function (_html) {
                        $('#subgrid-entityedit-ext').html(_html);
                        //$('#subgrid-entityedit-ext-res').html(_html);
                    }, 'all');
                    subgridGetAttributes($('#subgrid-entityedit>option:selected').attr('data-referencedentityid'), function (_html) {
                        $('#subgrid-entityedit-ext-res').html(_html);
                    }, null, true);
                    // formulaEdit(selected, subgrid_viewid);
                });
                $("#subgrid-viewid").off('change').on("change", function (e, param) {
                    subGridchangeView(selected, param && param.type || 'edit');
                });
            }, 'new');
        });
    }

    var formulabox = $('#subgrid-formula-show');
    formulabox.show();
    //if (Xms.Web.SelectedValue(target.find('#subgrid-viewid')) == null) {
    //    Xms.Web.Alert(false, '请选择视图');
    //    return false;
    //}
    target.modal('show');
}

function getEntityCnName(data, name) {
    var res = '';
    $.each(data, function (key, item) {
        if (item && item.name && (name.toLowerCase() == item.name.toLowerCase())) {
            res = item.localizedname;
            return false;
        }
    });
    return res;
}
//*************字段 值计算
function formulaEdit(selected, subgrid_viewid, type) {
    console.log(selected.data().lookuplistCache)
    /*********** 公式 初始值   start  ***************/
    var formulaValue = selected.attr("data-formulaValue");
    var isLoaded = selected.attr('data-isloaded');

    if (formulaValue && formulaValue != "") {
        if (!selected.data().formula) {
            selected.data().formula = new xmsFormHandler.xmsFormulaWrap();
            selected.data().formula.ele = $("#formula-show-rule");
        } else {
            selected.data().formula.ele = $("#formula-show-rule");
        }
        if (!selected.data().lookuplistCache) {
            var lookuplistCache = selected.data().lookuplistCache = [];
        } else {
            var lookuplistCache = selected.data().lookuplistCache;
        }

        var valueObj = JSON.parse(decodeURIComponent(formulaValue));
        if (valueObj && valueObj.length > 0) {
            if (type == 'new') {
                $.each(valueObj, function (key, item) {
                    item.viewid = subgrid_viewid;
                });
                selected.attr("data-formulaValue", encodeURIComponent(JSON.stringify(valueObj)));
            }
            $.each(valueObj, function (key, item) {
                if (item.type == 'append') {
                    if (type == 'new') {
                        if (!lookuplistCache[item.name + subgrid_viewid]) {
                            lookuplistCache[item.name + subgrid_viewid] = [];
                            var temparr = JSON.parse(item.expression);
                            $.each(temparr, function (iii, nnn) {
                                lookuplistCache[item.name + subgrid_viewid].push(nnn)
                            });
                        }
                    } else if (type == 'edit' && item.viewid) {
                        if (!lookuplistCache[item.name + item.viewid]) {
                            lookuplistCache[item.name + item.viewid] = [];
                            var temparr = JSON.parse(item.expression);
                            $.each(temparr, function (iii, nnn) {
                                lookuplistCache[item.name + item.viewid].push(nnn)
                            });
                        }
                    }
                } else {
                    if (type == 'edit' && item.viewid) {
                        var liItem = selected.data().formula.addList(item.name);
                        liItem.viewid = item.viewid;
                    } else {
                        var liItem = selected.data().formula.addList(item.name);
                        liItem.viewid = subgrid_viewid;
                    }
                    var expressArr = JSON.parse(item.expression);
                    $.each(expressArr, function (ii, nn) {
                        var etype = 'entity';
                        var temp = nn;
                        nn = {};
                        nn.key = temp;
                        //    console.log(subGridCache[subgrid_viewid].alldatas.content)

                        if (nn.key == "+" || nn.key == "-" || nn.key == "*" || nn.key == "/" || nn.key == "=" || nn.key == "(" || nn.key == ")") {
                            etype = "type";
                            nn.name = temp;
                        } else {
                            if (type == 'edit' && item.viewid) {
                                nn.name = getEntityCnName(subGridCache[item.viewid].alldatas, temp);
                            } else {
                                nn.name = getEntityCnName(subGridCache[subgrid_viewid].alldatas, temp);
                            }
                        }
                        liItem.addItem(nn.key, nn.name, etype);
                    });
                }
            });
            //  }
        }
    } else {
        if (!selected.data().formula) {
            selected.data().formula = new xmsFormHandler.xmsFormulaWrap();
            selected.data().formula.ele = $("#formula-show-rule");
        } else {
            selected.data().formula.ele = $("#formula-show-rule");
        }
        if (!selected.data().lookuplistCache) {
            var lookuplistCache = selected.data().lookuplistCache = [];
        } else {
            var lookuplistCache = selected.data().lookuplistCache;
        }
    }

    /*********** 公式   end  ***************/

    $("#subgrid-viewid").on("change", function () {
        selected.data().formula.clear();
    });

    $("#formula-show-rule").empty();
    // var formulaWrapValue = '';

    //currentFormula = formulaWrap;
    var curetFormula = selected.data().formula;
    var currentFormularItem = null;
    var oldSelectedValue = '';

    var formula_entitys = $("#subgrid-formula-entitys");
    var formula_hidden = $("#subgrid-formulaedit");
    var formula_show = $('#formula-show-rule');
    if (curetFormula && curetFormula.length > 0) {
        $("#subgrid-entityedit").trigger("change");
    }
    var current_formulaList = null; //当前编辑的值计算行
    formula_entitys.children("option").on("click", function () {
        var $this = $(this);
        var viewid = $('#subgrid-viewid>option:selected').val();
        if ($("#subgrid-entityedit>option:selected").val() == -1 || $("#subgrid-entityedit>option:selected").val() == '') {
            Xms.Web.Toast('请先选择上方字段', false)
            return false;
        }
        var value = $this.attr("value");
        if (!current_formulaList) {
            if ($('.formula-list-item.active').length == 0) {
                current_formulaList = curetFormula.list[0];
            } else {
                current_formulaList = curetFormula.get(value);
            }
            var curListLast = current_formulaList.list[current_formulaList.list.length - 1];
            if (curListLast && curListLast.type == "entity") {//如果想连续输入两个字段名的时候
                Xms.Web.Alert(false, '不可以连续输入两个字段名');
                return false;
            }
            if (current_formulaList.length > 0) {
                var checke = curetFormula.checkByOtherList(current_formulaList, $this.val());//检查是否在其他字段里已经添加
                if (checke) {
                    Xms.Web.Alert(false, "该字段已添加在其他字段的计算中");
                    return false;
                }
                current_formulaList.addItem($this.val(), $this.text(), 'entity');
            }
        }
        if (curetFormula.length > 0) {
            if ($('.formula-list-item.active').length == 0) {
                current_formulaList = curetFormula.get($(this).val());
            } else {
                current_formulaList = curetFormula.getList($('.formula-list-item.active').attr("id"));
            }
            if (current_formulaList.length == 0) {
                current_formulaList = curetFormula.list[0];
            } else {
                current_formulaList = current_formulaList[0];
            }
            var curListLast = current_formulaList.list[current_formulaList.list.length - 1];
            if (curListLast && curListLast.type == "entity") {//如果想连续输入两个字段名的时候
                Xms.Web.Alert(false, '不可以连续输入两个字段名');
                return false;
            }
            var checke = curetFormula.checkByOtherList(current_formulaList, $this.val());//检查是否在其他字段里已经添加
            if (checke) {
                Xms.Web.Alert(false, "该字段已添加在其他字段的计算中");
                return false;
            }
            current_formulaList.addItem($this.val(), $this.text(), 'entity');
        }
    });

    $(".formula-type-item").off("click").on("click", function () {
        var $this = $(this);
        if (!current_formulaList) {
            return false;
        }
        var isrunN = true;
        if ($this.attr('data-key') == '=') {
            try {
                var curStr = current_formulaList.getRulerRan();
                var resStr = eval(curStr);
                current_formulaList.addItem($this.text(), $this.attr("data-key"), 'type');
            } catch (e) {
                Xms.Web.Alert(false, '等式左边输入错误！')
            }
        } else {
            current_formulaList.addItem($this.text(), $this.attr("data-key"), 'type');
        }
    });

    function entityeditChange(obj) {
        var $this = $(obj);
        var viewid = $('#subgrid-viewid>option:selected').val();
        if ($this.val() == "" || !$this.val()) { return false; }
        var type = $(obj).children('option:selected').attr("data-type");
        $('#subgrid-entityedit-type>option[value="2"]').prop('selected', true);
        if (type != "lookup" && type != "owner" && type != "customer") {
            if ($this.val() == -1) {
                curetFormula.render([]);
                return false;
            }
            $('#subgrid-entityedit-type>option[value="1"]').prop('selected', true);
            $('#subgrid-formula-show').show();
            $('#subgrid-ext-edit').hide();
            if (current_formulaList && current_formulaList.checkHasItem) {
                if (!current_formulaList.checkHasItem(oldSelectedValue) && current_formulaList.length > 0) {
                    Xms.Web.Alert(false, '该公式必须包含选中字段');
                    $this.children('option[value="' + oldSelectedValue + '"]').prop('selected', true);
                    return false;
                }
            }
            oldSelectedValue = $this.val();
            $('#subgrid-entityedit-type').children('option[value="1"]').prop("selected", true).trigger('change');
            if (curetFormula.length == 0) {
                var formulaitem = curetFormula.addList($this.val());// 增加值计算规则列表
                formulaitem.viewid = viewid;
                var getlist = curetFormula.get($this.val());
                if (getlist.length == 0) {
                    getlist = [curetFormula.list[0]];
                } else {
                    getlist[0].ele.siblings().removeClass("active");
                    getlist[0].ele.addClass('active');
                    current_formulaList = getlist[0];
                }
                currentFormularItem = getlist;
                curetFormula.render(getlist);
            } else {
                var getlist = curetFormula.get($this.val());
                if (getlist.length > 0) {
                    curetFormula.render(getlist);
                } else {
                    getlist = curetFormula.get($this.val());
                    if (getlist.length == 0) {
                        getlist = curetFormula.getList($this.val());
                        if (getlist.length == 0) {
                            var formulaitem = curetFormula.addList($this.val());// 增加值计算规则列表
                            formulaitem.viewid = viewid;
                            getlist = [formulaitem];
                        }
                    } else {
                        getlist[0].ele.siblings().removeClass("active");
                        getlist[0].ele.addClass('active');
                        current_formulaList = getlist[0];
                    }
                    curetFormula.render(getlist);
                }
                currentFormularItem = getlist;
                getlist[0].ele.siblings().removeClass("active");
                getlist[0].ele.addClass('active');
                current_formulaList = getlist[0];
            }
        } else {
            subgridGetAttributes($('#subgrid-entityedit>option:selected').attr('data-referencedentityid'), function (_html) {
                $('#subgrid-entityedit-ext-res').html(_html);

                $('#subgrid-formula-show').hide();
                $('#subgrid-ext-edit').show();
                var listable = $('.subgrid-entityedit-extable');
                var list = listable.children('tbody');
                var reentityid = $('#subgrid-entityedit>option:selected').attr('data-referencedentityid');
                list.empty();
                // $('#subgrid-entityedit-ext').children('option').prop('selected', false);
                if (typeof lookuplistCache[$this.val() + viewid] == 'undefined') {
                    lookuplistCache[$this.val() + viewid] = [];
                } else {
                    $.each(lookuplistCache[$this.val() + viewid], function (key, item) {
                        if (item) {
                            var temp = item.split("§");
                            var tarv = temp[0], sourv = temp[1];
                            var tarvcn = getEntityCnName(subGridCache[viewid].alldatas, tarv);
                            var sourvcn = getEntityCnName(subGridCache[reentityid + "&getall=true"].alldatas, sourv);
                            subGridInsertToTable(list, tarvcn, sourvcn, lookuplistCache[$this.val() + viewid], key);
                        }
                    });
                }
            }, null, true);

            //setTimeout(function () {
            //    $('#subgrid-entityedit-type').children('option[value="2"]').prop("selected", true).trigger('change');
            //}, 0)
        }
        //console.log(lookuplistCache)
    }
    //关联字段
    //$('#subgrid-entityedit-ext').off('change').on('change', function () {
    //    var value = $('#subgrid-entityedit').children('option:selected').val();
    //    var viewid = $('#subgrid-viewid>option:selected').val();
    //    if (typeof lookuplistCache[value + viewid] == 'undefined') {
    //        lookuplistCache[value + viewid] = [];
    //    } else {
    //        lookuplistCache[value + viewid] = [];
    //        $(this).children('option:selected').each(function () {
    //            lookuplistCache[value + viewid].push($(this).val());
    //        });
    //    }
    //    // console.log(lookuplistCache)
    //});
    $('.subgrid-entityedit-addext').off('click').on('click', function () {
        var viewid = $('#subgrid-viewid>option:selected').val();
        var sourv = $('#subgrid-entityedit-ext-res>option:selected').val();
        var tarv = $('#subgrid-entityedit-ext>option:selected').val();
        var sourvcn = $('#subgrid-entityedit-ext-res>option:selected').text();
        var tarvcn = $('#subgrid-entityedit-ext>option:selected').text();
        var tansv = $('#subgrid-entityedit>option:selected').val();//该字段
        if (typeof lookuplistCache[tansv + viewid] == 'undefined') {
            lookuplistCache[tansv + viewid] = [];
        }
        var listable = $('.subgrid-entityedit-extable');
        var list = listable.children('tbody');
        if ($.inArray(lookuplistCache[tansv + viewid], tarv + '§' + sourv) == -1) {
            subGridInsertToTable(list, tarvcn, sourvcn, lookuplistCache[tansv + viewid], lookuplistCache[tansv + viewid].length + 1);
            lookuplistCache[tansv + viewid].push(tarv + '§' + sourv);
        } else {
            Xms.Web.Alert(false, '已添加，请重新选择');
        }
    });

    $("#subgrid-entityedit").off("change").on("change", function () {
        entityeditChange(this);
    });
}
//*************字段 值计算  end
function subGridInsertToTable(_context, tarvcn, sourvcn, datas, index) {
    var item = $('<tr class="exttable-item-' + setTimeout(0) + '"></tr>');
    var itemvalue = '<td>' + tarvcn + '</td><td>' + sourvcn + '</td>';
    var itemlast = $('<td></td>');
    item.html(itemvalue).append(itemlast);
    var butn = $('<div class=""><span class="glyphicon glyphicon-remove"></span></div>');
    butn.off('click').on('click', function () {
        datas[index] = null;
        butn.off();
        item.remove();
    });
    itemlast.append(butn);
    _context.append(item);
}

var subGridCache = [];
function handleSubgridEntityEdit(alldatas, callback) {
    console.log(alldatas.content);
    var _suportType = ["int", "float", "money", "lookup", "customer", "owner"];
    var _html = [];
    _html.push('<option value=""></option>');
    $.each(alldatas.content, function (key, obj) {
        if (_suportType.indexOf(obj.attributetypename) > -1) {//过滤不支持的字段类型
            _html.push('<option value="' + obj.name + '" data-referencedentityid="' + obj.referencedentityid + '" data-attributeid="' + obj.attributeid + '" data-entityid="' + obj.entityid + '" data-type="' + obj.attributetypename + '">' + obj.localizedname + '</option>');
        }
    });
    callback && callback(_html);
}
function handleSubgriditemEdit(alldatas, callback) {
    var _suportType = ["int", "float", "money"];
    var _html = [];
    if (!alldatas.content) return false;
    $.each(alldatas.content, function (key, obj) {
        if (_suportType.indexOf(obj.attributetypename) > -1) {//过滤不支持的字段类型
            _html.push('<option value="' + obj.name + '" data-referencedentityid="' + obj.referencedentityid + '" data-attributeid="' + obj.attributeid + '" data-entityid="' + obj.entityid + '" data-type="' + obj.attributetypename + '">' + obj.localizedname + '</option>');
        }
    });
    callback && callback(_html);
}
function handleSubgridAllEdit(alldatas, callback) {
    //var _suportType = ["int", "float", "money"];
    var _html = [];
    if (!alldatas.content) return false;
    $.each(alldatas.content, function (key, obj) {
        //if (_suportType.indexOf(obj.attributetypename) > -1) {//过滤不支持的字段类型
        _html.push('<option value="' + obj.name + '" data-referencedentityid="' + obj.referencedentityid + '" data-attributeid="' + obj.attributeid + '" data-entityid="' + obj.entityid + '" data-type="' + obj.attributetypename + '">' + obj.localizedname + '</option>');
        // }
    });
    callback && callback(_html);
}
function handleSubgridAllAttribute(alldatas, callback) {
    //var _suportType = ["int", "float", "money"];
    var _html = [];
    if (!alldatas.content) return false;
    $.each(alldatas.content, function (key, obj) {
        //if (_suportType.indexOf(obj.attributetypename) > -1) {//过滤不支持的字段类型
        _html.push('<option value="' + obj.name + '" data-referencedentityid="' + obj.referencedentityid + '" data-entityid="' + obj.entityid + '" data-type="' + obj.attributetypename + '">' + obj.localizedname + '</option>');
        // }
    });
    callback && callback(_html);
}
function subgridEntityEdit(subgrid_viewid, callback, type) {
    if (subgrid_viewid && subgrid_viewid != "") {
        if (typeof subGridCache[subgrid_viewid] == 'undefined') {//缓存中是否已有数据
            Xms.Web.GetJson("/api/schema/queryview/getattributes/" + subgrid_viewid, null, function (alldatas) {
                // alldatas = alldatas.content;
                /*缓存数据*/
                subGridCache[subgrid_viewid] = {};
                subGridCache[subgrid_viewid].alldatas = alldatas;
                if (!type) {
                    handleSubgridEntityEdit(alldatas, callback);
                } else if (type == 'type') {
                    handleSubgriditemEdit(alldatas, callback);
                } else if (type == 'all') {
                    handleSubgridAllEdit(alldatas, callback)
                }
            });
            //});
        } else {//读取缓存数据处理
            if (!type) {
                handleSubgridEntityEdit(subGridCache[subgrid_viewid].alldatas, callback)
            } else if (type == 'type') {
                handleSubgriditemEdit(subGridCache[subgrid_viewid].alldatas, callback)
            } else if (type == 'all') {
                handleSubgridAllEdit(subGridCache[subgrid_viewid].alldatas, callback)
            }
        }
    }
}

function subgridGetAttributes(entityid, callback, type, isall) {
    var getall = isall ? "&getall=true" : "";
    if (entityid && entityid != "") {
        if (typeof subGridCache[entityid + getall] == 'undefined') {//缓存中是否已有数据
            Xms.Schema.GetAttributes({ getall: true, entityid: entityid }, function (_alldatas) {
                var alldatas = {};
                alldatas.content = _alldatas;
                /*缓存数据*/
                subGridCache[entityid + getall] = {};
                subGridCache[entityid + getall].alldatas = alldatas;
                handleSubgridAllAttribute(alldatas, callback)
            });
            //});
        } else {//读取缓存数据处理
            handleSubgridAllAttribute(subGridCache[entityid + getall].alldatas, callback)
        }
    }
}

function getEntityLocName(name, datas) {//获取layoutconfig对应的字段的中文名
    var res = null;
    //  console.log(datas);
    $.each(datas, function (key, obj) {
        if (name == obj.name.toLowerCase()) {
            res = obj;
            return false;
        }
    });
    return res;
}

function changeSubgridTypeShow(obj) {
    var $obj = $(obj);
    if ($obj.val() == 1) {
        $("#subgrid-formula-edit").show();
        $("#subgrid-ext-edit").hide();
    } else if ($obj.val() == 2) {
        $("#subgrid-ext-edit").show();
        $("#subgrid-formula-edit").hide();
    }
}

//编辑元素
function editObject() {
    var selected = $('.selected');
    if (selected.is('.navGroup')) {
        editNavGroup();
    }
    else if (selected.is('.nav-item')) {
        editNavItem();
    }
    else if (selected.is('.tab')) {
        editTab();
    }
    else if (selected.is('.section')) {
        editSection();
    }
    else if (selected.is('.header')) {
        editHeader();
    }
    else if (selected.is('.footer')) {
        editFooter();
    }
    else if (selected.is('.field.labelbox')) {
        editLabel();
    }
    else if (selected.is('.field')) {
        editField();
    }
}

//判断表格里是否被清空了
function checkCellIsEmpty(selected) {
    var parWrap = selected.parents(".section:first");
    var cells = parWrap.find("table.cell");
    if (cells.length == 0) {
        return true;
    }
    return false;
}

//往一个空的表格插入一行空的占位列
function insertCelltoEmpty(selected) {
    var parWrap = selected.parents(".section:first");
}

//删除元素
function removeObject() {
    var selected = $('.selected:not(.locked)');
    if (selected.length == 0) return;
    Xms.Web.Confirm("确认", "请确认是否移除？", function () {
        var candelete = false;
        if (selected.is('.navGroup')) {
            candelete = true;
        }
        else if (selected.is('.nav-item')) {
            candelete = true;
        }
        else if (selected.is('.tab')) {
            candelete = true;
        }
        else if (selected.is('.section')) {
            if (selected.parents('.tab').length > 0 && selected.parents('.tab').find('.section').length <= 1)
                candelete = false;
            else
                candelete = true;
        }
        else if (selected.is('.header')) {
            candelete = false;
        }
        else if (selected.is('.footer')) {
            candelete = false;
        }
        else if (selected.is('.cell')) {
            setFieldAttrs(selected.parent(), null);
            selected.parent().addClass('placeholder');
            candelete = true;
            //重新绑定字段列表
            //var usedAttrs = [];
            //$('#formBody').find('.field').not('.placeholder').each(function (i, n) {
            //    var self = $(n);
            //    usedAttrs.push(self.attr('data-name'));
            //});
            //var attrs = $.map(attributes, function (n) {
            //    return $.inArray(n.name, usedAttrs) < 0 ? n : null;
            //});
            //bindAttributes(attrs);
            var Enattrs = filterInsertEdAttri(attributes);
            if (Enattrs.length > 0) {
                // bindAttributes(Enattrs);
            }
        }
        if (candelete) {
            var trRows = selected.parents(".section:first").find(">table>tbody>tr");
            if (selected.attr("data-controltype") == "None" || selected.attr("data-controltype") == "none" || selected.attr("data-controltype") == "label") {//初始化为空格
            } else {
                if (selected.get(0).tagName.toLowerCase() == "table") {
                    insertToAddList(selected);
                } else if ((selected.hasClass("section") || selected.hasClass("tab")) && selected.get(0).tagName.toLowerCase() == "div") {
                    selected.find("td.field").each(function () {
                        if ($(this).children("table").length > 0) {
                            insertToAddList($(this).children("table"));
                        }
                    });
                }
            }
            selected.remove();

            resetTableVis(trRows);
            var isEm = checkCellIsEmpty(selected);
            if (!isEm) {
                delEmptyTr(trRows);
            }

            initAttributeEvent();
            initSortEvent();
        }
    });
}
//吧删除的字段返回到可添加列表
function insertToAddList(selected) {
    var AddList = $("#attributes");
    var param = {
        name: selected.attr("data-name"),
        localizedname: selected.attr("data-label"),
        entityname: selected.attr("data-entityname"),
        type: selected.attr("data-type")
    }
    var selectedLabel = selected.attr("data-label");
    var tempItem = $('<li class="ui-state-default ui-draggable"  style="position: relative;">' + selectedLabel + '</li>');
    for (var i in param) {
        if (param.hasOwnProperty(i)) {
            tempItem.attr("data-" + i, param[i]);
        }
    }
    AddList.append(tempItem);
}
function updateAttrsList() {
    //重新绑定字段列表
    var usedAttrs = [];
    $('#formBody').find('.field').not('.placeholder').each(function (i, n) {
        var self = $(n);
        usedAttrs.push(self.attr('data-name'));
    });
    var attrs = $.map(attributes, function (n) {
        return $.inArray(n.name, usedAttrs) < 0 ? n : null;
    });
    return attrs;
}

var loadedEntitys = [];
function initFormNav() {
    var formNavDatas = FormConfig.NavGroups;
    if (!formNavDatas || formNavDatas.length == 0) return false;
    for (var i = 0, len = formNavDatas.length; i < len; i++) {
        var itemD = formNavDatas[i];
        var _html = [];
        var fixId = (i + 2);
        _html.push('<div class="navGroup" data-label="' + itemD.Label + '">');
        _html.push('<a data-toggle="collapse" href="#collapseNav' + fixId + '" class="collapse-title nav-title" data-target="collapseNav' + fixId + '">');
        _html.push('<span class="glyphicon glyphicon-chevron-down"></span> ' + itemD.Label);
        _html.push('</a>');
        _html.push('<div id="collapseNav' + fixId + '" class="panel-collapse collapse in ui-sortable">');
        _html.push('<ul class="list-unstyled nav-child">');
        for (var j = 0, jlen = itemD.NavItems.length; j < jlen; j++) {
            var navItem = itemD.NavItems[j];
            _html.push(createNavLinkElement(navItem));
        }
        _html.push('</ul>');
        _html.push('</div>');
    }
    var htmlDom = (_html.join(""));
    $('#formNav').append(htmlDom);
}
function initFormBody(callback) {
    var headers = FormConfig.Header;
    var footers = FormConfig.Footer;
    var Panels = FormConfig.Panels;
    // console.log(Panels)
    //加载表单
    renderFormHeader(headers);
    renderFormContent(Panels);
    renderFormFooter(footers);
    //加载配公式规则方法
    formFormula();

    //初始化事件
    initSortEvent();
    initFieldEvent();
    var Enattrs = filterInsertEdAttri(attributes);
    if (Enattrs.length > 0) {
        bindAttributes(Enattrs);
    }
    callback && callback();
    //  console.log(loadedEntitys)
}

//表单基本信息
function editFormParams() {
    var target = $('#formModal');
    target.find('#form-name').val($('#Name').val());
    if ($('#isshownav').val() == 'true') {
        target.find('#form-isshownav').prop('checked', true);
    } else {
        target.find('#form-isshownav').prop('checked', false);
    }
    if (FormConfig && FormConfig.CustomCss) {
        try {
            var _formcss = JSON.parse(FormConfig.CustomCss)

            if (_formcss && _formcss.labels) {
                var _labels = _formcss.labels;
                _labels['color'] ? $('input[name="table-islabelfontcolor"]').prop('checked', true) && $('input[name="table-fontcolor"]').spectrum("set", _labels['color']) : $('input[name="table-islabelfontcolor"]').prop('checked', false);
                _labels['font-size'] ? $('input[name="table-islabelfontsize"]').prop('checked', true) && $('input[name="table-labelfontsize"]').jRange('setValue', _labels['font-size']) : $('input[name="table-islabelfontsize"]').prop('checked', false)
                _labels['background-color'] ? $('input[name="table-islabelbackgroundcolor"]').prop('checked', true) && $('input[name="table-labelbackgroundcolor"]').spectrum("set", _labels['background-color']) : $('input[name="table-islabelbackgroundcolor"]').prop('checked', false);
                _labels['text-align'] ? $('input[name="table-islabeltextalign"]').prop('checked', true) && $('input[name="table-labeltextalign"][value="' + _labels['text-align'] + '"]').prop('checked', true) : $('input[name="table-islabeltextalign"]').prop('checked', false);

                //$('input[name="table-labelfontsize"]').val();
                //$('input[name="table-labelbackgroundcolor"]').val();
                //$('input[name="table-labeltextalign"]').val();
            }
            if (_formcss && _formcss.inputs) {
                var _inputs = _formcss.inputs;
                _inputs['border-color'] ? $('input[name="table-isinputbordercolor"]').prop('checked', true) && $('input[name="table-inputbordercolor"]').spectrum("set", _inputs['border-color']) : $('input[name="table-isinputbordercolor"]').prop('checked', false)
                _inputs['border-width'] ? $('input[name="table-isinputbordersize"]').prop('checked', true) && $('input[name="table-inputbordersize"]').jRange('setValue', _inputs['border-width']) : $('input[name="table-isinputbordersize"]').prop('checked', false)
                _inputs['background-color'] ? $('input[name="table-isinputbackgroundcolor"]').prop('checked', true) && $('input[name="table-inputbackgroundcolor"]').spectrum("set", _inputs['background-color']) : $('input[name="table-isinputbackgroundcolor"]').prop('checked', false)
                _inputs['text-align'] ? $('input[name="table-isinputtextalign"]').prop('checked', true) && $('input[name="table-inputtextalign"][value="' + _inputs['text-align'] + '"]').prop('checked', true) : $('input[name="table-isinputtextalign"]').prop('checked', false)
                //$('input[name="table-inputbordercolor"]').val();
                //$('input[name="table-inputbordersize"]').val();;
                //$('input[name="table-inputbackgroundcolor"]').val();
                //$('input[name="table-inputtextalign"]').val();
            }
        } catch (e) { console.error(e) }
    }
    var scriptHtml = [];
    var scriptselect = [];
    for (var i = 0; i < scriptlist.length; i++) {
        //  if (scriptlist[i].Attribute=='') {
        scriptHtml.push('<tr onclick="chooseRowInfo(this)" class="scriptrow" data-id="' + scriptlist[i].Id + '"><td data-value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</td><td>' + scriptlist[i].Info + '</td></tr>');
        scriptselect.push('<option value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</option>');
        //  }
    }
    target.find('#formScript').html(scriptHtml.join(''));
    var eventHtml = [];
    target.find('#formEvent').empty();
    for (var i = 0; i < eventlist.length; i++) {
        if (eventlist[i].Attribute == '') {
            target.find('#formEvent').append('<tr class="eventitem" data-id="' + eventlist[i].eventid + '" onclick="chooseRowInfo(this)"><td><select class="scriptlist">' + scriptselect.join('') + '</select></td><td><select><option value="onchange">onchange</option><option value="onload">onload</option></select></td><td><input type="text" value="' + eventlist[i].JsAction + '" /></td></tr>');
            target.find('#formEvent').find('tr:last').find('td:eq(0)').find('option[value="' + eventlist[i].JsLibrary + '"]').attr('selected', "selected");
            target.find('#formEvent').find('tr:last').find('td:eq(1)').find('option[value="' + eventlist[i].Name + '"]').attr('selected', "selected");
        }
    }
    target.modal({
        keyboard: true
    })
}
function saveFormParams() {
    var target = $('#formModal');
    var selected = $('.selected');
    $('#Name').val(target.find('#form-name').val());
    if (target.find('#form-isshownav').prop('checked')) {
        $('#isshownav').val("true");
    } else {
        $('#isshownav').val("false");
    }

    var styles = {
        labels: {
            //'font-size': '12',
            //'color': '#555',
            //'background-color': 'none',
            //'text-align':'left'
        },
        inputs: {
            //'border-color': '#ddd',
            //'border-width': '1',
            //'background-color': 'none',
            //'text-align': 'left'
        }
    };
    if ($('input[name="table-islabelfontcolor"]').prop('checked')) {
        styles.labels['color'] = $('input[name="table-fontcolor"]').val() ? $('input[name="table-fontcolor"]').val() : '';
    }
    if ($('input[name="table-islabelfontsize"]').prop('checked')) {
        styles.labels['font-size'] = $('input[name="table-labelfontsize"]').val() ? $('input[name="table-labelfontsize"]').val() : '';
    }
    if ($('input[name="table-islabelbackgroundcolor"]').prop('checked')) {
        styles.labels['background-color'] = $('input[name="table-labelbackgroundcolor"]').val() ? $('input[name="table-labelbackgroundcolor"]').val() : '';
    }
    if ($('input[name="table-islabeltextalign"]').prop('checked')) {
        styles.labels['text-align'] = $('input[name="table-labeltextalign"]:checked').val() ? $('input[name="table-labeltextalign"]:checked').val() : '';
    }
    if ($('input[name="table-isinputbordercolor"]').prop('checked')) {
        styles.inputs['border-color'] = $('input[name="table-inputbordercolor"]').val() ? $('input[name="table-inputbordercolor"]').val() : '';
    }
    if ($('input[name="table-isinputbordersize"]').prop('checked')) {
        styles.inputs['border-width'] = $('input[name="table-inputbordersize"]').val() ? $('input[name="table-inputbordersize"]').val() : '';
    }
    if ($('input[name="table-isinputbackgroundcolor"]').prop('checked')) {
        styles.inputs['background-color'] = $('input[name="table-inputbackgroundcolor"]').val() ? $('input[name="table-inputbackgroundcolor"]').val() : '';
    }
    if ($('input[name="table-isinputtextalign"]').prop('checked')) {
        styles.inputs['text-align'] = $('input[name="table-inputtextalign"]:checked').val() ? $('input[name="table-inputtextalign"]:checked').val() : '';
    }

    if (FormConfig.CustomCss && FormConfig.CustomCss != "") {
        var cuscss = JSON.parse(FormConfig.CustomCss);
        styles = $.extend(true, {}, styles);
    }
    FormConfig.CustomCss = JSON.stringify(styles);
    $('#CustomCss').val(JSON.stringify(styles));
    removeSriptEvent('');
    target.find('#formEvent').find('tr').each(function (i, n) {
        var eventitem = {
            'Attribute': '',
            'Name': $(n).find('td:eq(1)').find('option:selected').val(),
            'JsAction': $(n).find('td:eq(2)').find('input').val(),
            'JsLibrary': $(n).find('td:eq(0)').find('option:selected').val(),
            'eventid': Xms.Utility.Guid.NewGuid().ToString()
        };
        eventlist.push(eventitem);
    });
    target.find('#formScript').find('tr').each(function (i, n) {
        var scriptitem = {
            'Attribute': '',
            'Name': $(n).find('td:eq(0)').text(),
            'Info': $(n).find('td:eq(1)').text(),
            'Id': $(n).attr('data-id')
        };
        var flag = true;
        if (scriptlist.length > 0) {
            $.each(scriptlist, function (key, item) {
                if (item.Id == scriptitem.Id) {
                    flag = false;
                    return false;
                }
            });
        }
        if (flag == true) {
            scriptlist.push(scriptitem);
        }
    });

    target.modal('hide');
    saveForm();
}

//添加导航链接
function insertNavLink() {
    var html = [];
    var navlinkModal = $("#addNavLinkModal");
    navlinkModal.attr("state", "new");
    $("#navLink-name").val('');
    $("#navLink-icon").val('');
    $("#navLink-url").val('');
    navlinkModal.find("input[name='navLinkType'][value=0]").prop("checked", true);
    $("#navLink-url").prop("disabled", false);
    $("#navLink-entity").prop("disabled", true);
    Xms.Web.GetJson('/api/schema/relationship/GetReferenced/' + $("#EntityId").val(), null, function (data) {
        html.push('<option value="">请选择</option>');
        $(data.content).each(function (i, n) {
            html.push('<option value="' + n.referencingentityid + '" data-relationshipname="' + n.name + '">' + n.referencingentitylocalizedname + "(" + n.referencingattributelocalizedname + ') </option>');
        });
        $('#navLink-entity').html(html.join(''));
        //  $("#navLink-entity>option[data-relationshipname='" + navlinkModel.RelationshipName + "']").prop("selected", true);
    });
    navlinkModal.modal({
        keyboard: true
    })
}
function addNavLink() {
    if ($('#navLink-name').val() == '') {
        Xms.Web.Alert(false, '请输入名称');
        return false;
    }
    var selected = $(".selected"), insertWrap = $(".navGroup:not('.locked')").eq(0).children('div[id^=collapseNav]').eq(0).children();
    var isGroup = selected.hasClass(".navGroup");
    if (isGroup) {
        insertWrap = selected;
    }
    var navlinkModal = $("#addNavLinkModal");
    var type = navlinkModal.attr("state");
    var navlinkModel = new Xms.Form.NavDescriptor();
    navlinkModel.Label = $("#navLink-name").val();
    navlinkModel.Icon = $("#navLink-icon").val();
    var urlType = navlinkModal.find("input[name='navLinkType']:checked").val();
    navlinkModel.type = urlType;
    if (urlType == 0) {
        navlinkModel.Url = $("#navLink-url").val();
        navlinkModel.RelationshipName = "";
        navlinkModel.Id = $("#EntityId").val();
    } else {
        navlinkModel.Url = "";
        navlinkModel.RelationshipName = $("#navLink-entity>option:selected").attr("data-relationshipname");
        navlinkModel.Id = $("#navLink-entity>option:selected").val();
    }
    //console.log(type)
    if (type == "edit") {
        selected.attr("data-label", navlinkModel.Label);
        selected.attr("data-icon", navlinkModel.Icon || 'glyphicon glyphicon-list');
        selected.attr("data-url", navlinkModel.Url);
        selected.attr("data-relationshipname", navlinkModel.RelationshipName);
        selected.attr("data-entityid", navlinkModel.Id);
        selected.attr("data-type", urlType);
        selected.children('a').text(navlinkModel.Label)
    } else {
        var element = createNavLinkElement(navlinkModel);
        insertWrap.append($(element));
    }

    navlinkModal.modal('hide');
    initSortEvent();
}
function createNavLinkElement(obj) {
    var _html = [];
    var type = obj.type;
    if (!type) {
        if (obj.RelationshipName == "") {
            type = 0;
        } else {
            type = 1;
        }
    }
    _html.push('<li class="nav-item" data-label="' + obj.Label + '" data-entityid="' + obj.Id + '" data-type="' + type + '" data-icon="' + (obj.Icon || "glyphicon glyphicon-minus") + '" data-RelationshipName="' + obj.RelationshipName + '" data-url="' + obj.Url + '"><span class="' + (obj.Icon || "glyphicon glyphicon-minus") + '"></span><a>' + obj.Label + '</a></li>');

    return _html.join("");
}
//导航设置
function editNavGroup() {
    var selected = $('.selected');
    var target = $('#navGroupModal');
    target.find('#navgroup-name').val(selected.attr('data-label'));
    target.modal({
        keyboard: true
    })
}
function saveNavGroup() {
    var target = $('#navGroupModal');
    var selected = $('.selected');

    selected.attr('data-label', target.find('#navgroup-name').val());

    target.modal('hide');
}
function editNavItem() {
    var html = [];
    var navlinkModal = $("#addNavLinkModal");
    navlinkModal.attr("state", "edit");
    var selected = $('.selected');
    var navlinkModel = new Xms.Form.NavDescriptor();
    navlinkModel.Label = selected.attr("data-label");
    navlinkModel.Icon = selected.attr("data-icon");
    navlinkModel.type = selected.attr("data-type");
    if (navlinkModel.type == "0") {
        navlinkModel.Url = selected.attr("data-url");
        navlinkModel.RelationshipName = "";
        $("#navLink-url").prop("disabled", false);
        $("#navLink-entity").prop("disabled", true);
    } else if (navlinkModel.type == 1) {
        $("#navLink-url").prop("disabled", true);
        $("#navLink-entity").prop("disabled", false);
        navlinkModel.Url = "";
        navlinkModel.RelationshipName = selected.attr("data-relationshipname");
    }
    Xms.Web.GetJson('/api/schema/relationship/GetReferenced/' + $("#EntityId").val(), null, function (data) {
        html.push('<option value="">请选择</option>');
        console.log(data.content);
        $(data.content).each(function (i, n) {
            html.push('<option value="' + n.referencingentityid + '" data-relationshipname="' + n.name + '">' + n.referencingentitylocalizedname + "(" + n.referencingattributelocalizedname + ') </option>');
        });
        $('#navLink-entity').html(html.join(''));
        $("#navLink-entity>option[data-relationshipname='" + navlinkModel.RelationshipName + "']").prop("selected", true);
    });
    $("#navLink-name").val(navlinkModel.Label);
    $("#navLink-icon").val(navlinkModel.Icon);
    $("#navLink-url").val(navlinkModel.Url);
    navlinkModal.find("input[name='navLinkType'][value='" + navlinkModel.type + "']").prop("checked", true);

    navlinkModal.modal({
        keyboard: true
    })
}
function saveNavItem() {
    var target = $('#navItemModal');
    var selected = $('.selected');
    var entitySel = $('#navLink-entity');
    var navLinkType = $('input[name="navLinkType"]:checked');
    selected.attr('data-label', target.find('#navitem-label').val());
    selected.attr('data-icon', target.find('#navitem-icon').val());
    if (navLinkType == 1) {
        selected.attr('data-url', '');
        selected.attr('data-entityid', entitySel.find('option:selected').val());
        selected.attr('data-relationshipname', entitySel.find('option:selected').attr('data-relationshipname'));
    } else {
        selected.attr('data-url', target.find('#navitem-url').val());
        selected.attr('data-entityid', '');
        selected.attr('data-relationshipname', '');
    }

    target.modal('hide');
}
//选项卡设置
function editTab() {
    var selected = $('.selected');
    var target = $('#tabModal');
    target.find('#tab-label').val(selected.attr('data-label'));
    if (selected.attr('data-isshowlabel') == "true") {
        target.find('#tab-isshowlabel').prop("checked", true);
    } else {
        target.find('#tab-isshowlabel').prop("checked", false);
    }
    if (selected.attr('data-isexpanded') == "true") {
        target.find('#tab-isexpanded').prop("checked", true);
    } else {
        target.find('#tab-isexpanded').prop("checked", false);
    }
    if (selected.attr('data-isvisible') == "true") {
        target.find('#tab-isvisible').prop("checked", true);
    } else {
        target.find('#tab-isvisible').prop("checked", false);
    }
    if (selected.attr('data-displaystyle') == "1") {
        target.find('#tab-DisplayStyle').prop("checked", true);
    } else {
        target.find('#tab-DisplayStyle').prop("checked", false);
    }
    if (selected.attr('data-isasync') == "true") {
        target.find('#tab-isAsync').prop("checked", true);
    } else {
        target.find('#tab-isAsync').prop("checked", false);
    }
    if (typeof selected.attr('data-name') == 'undefined') {
        selected.attr('data-name', 'tab-' + new Date().getTime());
    }
    //event
    var scriptHtml = [];
    var scriptselect = [];
    for (var i = 0; i < scriptlist.length; i++) {
        if (scriptlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
            scriptHtml.push('<tr onclick="chooseRowInfo(this)" class="scriptrow" data-id="' + scriptlist[i].Id + '"><td data-value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</td><td>' + scriptlist[i].Info + '</td></tr>');
            scriptselect.push('<option value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</option>');
        }
    }
    target.find('#tabScript').html(scriptHtml.join(''));
    var eventHtml = [];
    target.find('#tabEvent').html('');
    for (var i = 0; i < eventlist.length; i++) {
        if (eventlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
            target.find('#tabEvent').append('<tr onclick="chooseRowInfo(this)" data-id="' + eventlist[i].eventid + '"><td><select class="scriptlist">' + scriptselect.join('') + '</select></td><td><select><option value="onstatechange">onstatechange</option></select></td><td><input type="text" value="' + eventlist[i].JsAction + '" /></td></tr>');
            target.find('#tabEvent').find('tr:last').find('td:eq(0)').find('option[value="' + eventlist[i].JsLibrary + '"]').attr('selected', "selected");
            target.find('#tabEvent').find('tr:last').find('td:eq(1)').find('option[value="' + eventlist[i].Name + '"]').attr('selected', "selected");
        }
    }
    $("#tab-tab3").hide();
    $('#tab-isvisible').off('change').on('change', function () {
        if ($(this).prop('checked')) {
            $('#tab-DisplayStyle').prop('checked', false);
        } else {
            $('#tab-DisplayStyle').prop('checked', false);
        }
    });
    var $isasync = $('#tab-isAsync');
    $('#tab-DisplayStyle').off('change').on('change', function () {
        if ($(this).prop('checked')) {
            $('#tab-isvisible').prop('checked', true);
        } else {
            $isasync.prop('checked', false);
        }
    });

    $isasync.off('change').on('change', function () {
        if ($(this).prop('checked')) {
            $('#tab-DisplayStyle').prop('checked', true);
        }
    });

    $("a[href='#tab-tab3']").hide();
    target.modal({
        keyboard: true
    });
}
function saveTab() {
    var target = $('#tabModal');
    var selected = $('.selected');
    if (typeof selected.attr('data-name') == 'undefined') {
        selected.attr('data-name', 'tab-' + new Date().getTime());
    }
    selected.attr('data-label', target.find('#tab-label').val());
    if (target.find('#tab-isshowlabel').prop("checked")) {
        selected.attr('data-isshowlabel', true);
    } else {
        selected.attr('data-isshowlabel', false);
    }
    if (target.find('#tab-isexpanded').prop("checked")) {
        selected.attr('data-isexpanded', true);
    } else {
        selected.attr('data-isexpanded', false);
    }
    if (target.find('#tab-isvisible').prop("checked")) {
        selected.attr('data-isvisible', true);
    } else {
        selected.attr('data-isvisible', false);
    }
    if (target.find('#tab-DisplayStyle').prop("checked")) {
        selected.attr('data-displaystyle', 1);
    } else {
        selected.attr('data-displaystyle', 0);
    }
    if (target.find('#tab-isAsync').prop("checked")) {
        selected.attr('data-isasync', true);
    } else {
        selected.attr('data-isasync', false);
    }
    selected.find('.title:first').text(target.find('#tab-label').val());
    removeSriptEvent(selected.attr('data-name'));

    if (target.find('#tab-isshowlabel').prop('checked') == false) {
        selected.find('.tab-title:first').addClass("hidden-text");
    }
    else {
        selected.find('.tab-title:first').removeClass("hidden-text");
    }

    target.find('#tabEvent').find('tr').each(function (i, n) {
        var eventitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(1)').find('option:selected').val(),
            'JsAction': $(n).find('td:eq(2)').find('input').val(),
            'JsLibrary': $(n).find('td:eq(0)').find('option:selected').val(),
            'eventid': Xms.Utility.Guid.NewGuid().ToString()
        };
        eventlist.push(eventitem);
    });
    target.find('#tabScript').find('tr').each(function (i, n) {
        var scriptitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(0)').text(),
            'Info': $(n).find('td:eq(1)').text(),
            'Id': $(n).attr('data-id')
        };
        scriptlist.push(scriptitem);
    });
    target.modal('hide');
}
//表格设置
function editSection() {
    var selected = $('.selected');
    var target = $('#sectionModal');
    var attrWidth = selected.attr('data-attrwidth') || '';
    var attrAlignment = selected.attr('data-attralignment') || 'Left';
    var attrPosition = selected.attr('data-attrposition') || 'Left';
    target.find('#section-label').val(selected.attr('data-label'));
    if (selected.attr('data-isshowlabel') == "true") {
        target.find('#section-isshowlabel').prop("checked", true);
    } else {
        target.find('#section-isshowlabel').prop("checked", false);
    }
    if (selected.attr('data-isvisible') == "true") {
        target.find('#section-isvisible').prop("checked", true);
    } else {
        target.find('#section-isvisible').prop("checked", false);
    }
    target.find('#section-columns').val(selected.attr('data-columns'));
    target.find('#section-celllabelwidth').val(selected.attr('data-celllabelwidth'));
    target.find('#section-celllabelalignment').val(selected.attr('data-celllabelalignment'));
    target.find('#section-celllabelposition').val(selected.attr('data-celllabelposition'));
    target.find('#section-attrwidth').val(attrWidth);
    target.find('#section-attralignment').val(attrAlignment);
    target.find('#section-attrposition').val(attrPosition);
    $("#section-tab3").hide();
    $("a[href='#section-tab3']").hide();
    target.modal({
        keyboard: true
    })
}
function saveSection() {
    var target = $('#sectionModal');
    var selected = $('.selected');
    selected.attr('data-label', target.find('#section-label').val());
    selected.attr('data-isshowlabel', target.find('#section-isshowlabel').prop('checked'));
    selected.attr('data-isvisible', target.find('#section-isvisible').prop('checked'));
    selected.attr('data-columns', target.find('#section-columns').val());
    selected.attr('data-celllabelwidth', target.find('#section-celllabelwidth').val());
    selected.attr('data-celllabelalignment', target.find('#section-celllabelalignment').val());
    selected.attr('data-celllabelposition', target.find('#section-celllabelposition').val());
    selected.find('.title:first').text(target.find('#section-label').val());
    selected.attr('data-attrwidth', target.find('#section-attrwidth').val());
    selected.attr('data-attralignment', target.find('#section-attralignment').val());
    selected.attr('data-attrposition', target.find('#section-attrposition').val());
    if (target.find('#section-isshowlabel').prop('checked') == false) {
        selected.find('.header-title:first,.content-title:first,.footer-title:first,.table-title:first,.tab-title:first').addClass("hidden-text");
    }
    else {
        selected.find('.header-title:first,.content-title:first,.footer-title:first,.table-title:first,.tab-title:first').removeClass("hidden-text");
    }
    target.modal('hide');
}

var selerin = null;
//字段设置
function editField() {
    var selected = $('.selected');
    if (selected.attr('data-controltype') == 'subGrid') {
        editSubGrid();
        return false;
    }
    else if (selected.attr('data-controltype') == 'iFrame') {
        editIframe();
        return false;
    }
    var target = $('#attributeModal');
    var controltype = selected.attr('data-controltype');
    var attrType = selected.attr("data-type");
    var attrName = selected.attr("data-name");
    if (controltype == 'lookup' || controltype == 'owner' || controltype == 'customer') {
        $(".field-IsFilterRelationctrl").show();
        if (selected.attr('data-isfilterrelation') == "true") {
            target.find('#field-IsFilterRelation').prop("checked", true).trigger('change');
            $("#field-RelationRecordBox").show();
        } else {
            target.find('#field-IsFilterRelation').prop("checked", false).trigger('change');
            $("#field-RelationRecordBox").hide();
        }
        var entityid = '';

        attributes.forEach(function (n) {
            if (n.name == selected.attr('data-name')) {
                entityid = n.referencedentityid;
            }
        });
        if (entityid != '') {
            target.find('#field-viewbox').show();

            var html = [];
            Xms.Web.GetJson('/customize/QueryView/index?getall=true&LoadData=true&EntityId=' + entityid + '', null, function (data) {
                console.log(data.content)
                $(data.content.items).each(function (ii, nn) {
                    var attrName = nn.attributetypename;
                    attributes.forEach(function (j) {
                        if (j.referencedentityid == nn.entityid) {
                            attrName = j.name;
                        }
                    });
                    console.log(nn);
                    html.push('<option value="' + nn.queryviewid + '">' + nn.name + '</option>');
                });
                target.find('#field-viewid').html(html.join(''));
                //$("#field-RelationRecord").html(html.join(""));
                if (selected.attr('data-DefaultViewId') != '') {
                    target.find('#field-viewid option[value="' + selected.attr('data-DefaultViewId') + '"]').attr('selected', 'selected');
                }
            });
        }
    }
    else {
        target.find('#field-viewbox').hide();
        $(".field-IsFilterRelationctrl").hide();
    }

    //字段设置扩展属性
    Xms.Web.GetJson('/api/schema/relationship/GetReferencing/' + $('#EntityId').val() + '', null, function (data) {
        renderSelectByAttrs($("#field-EntityName"), data.content);
        if (selected.attr('data-fieldentityname') != '') {
            $("#field-EntityName").find('>option[data-referencedentityname="' + selected.attr('data-fieldentityname') + '"][data-referencingattributename="' + selected.attr('data-fieldsourceattributename') + '"]').attr('selected', 'selected');
            changeGetEntityName($("#field-EntityName"), function (sel) {
                //设置字段已有的值
                sel.find('>option[data-name="' + selected.attr('data-fieldattributename') + '"]').attr('selected', 'selected');
            }, $("#field-AttributeName"), function (datas) {
                var res = [];
                res = $.grep(datas, filterAttrTypeList(attrType));
                return res;
            });
        } else {
            changeGetEntityName($("#field-EntityName"), null, $("#field-AttributeName"), function (datas) {
                var res = [];
                res = $.grep(datas, filterAttrTypeList(attrType));
                return res;
            });//自动加载对应的实体
        }
    });

    //绑定扩展属性多选框的事件
    $("#field-EntityName").off("change").on("change", function () {
        changeGetEntityName($("#field-EntityName"), null, $("#field-AttributeName"), function (datas) {
            var res = [];
            res = $.grep(datas, filterAttrTypeList(attrType));
            return res;
        });
    });

    target.find('#field-name').val(selected.attr('data-name'));
    target.find('#field-label').val(selected.attr('data-label'));
    if (selected.attr('data-isshowlabel') == "true") {
        target.find('#field-isshowlabel').prop("checked", true);
    } else {
        target.find('#field-isshowlabel').prop("checked", false);
    }
    if (selected.attr('data-isvisible') == "true") {
        target.find('#field-isvisible').prop("checked", true);
    } else {
        target.find('#field-isvisible').prop("checked", false);
    }
    if (selected.attr('data-isreadonly') == "true") {
        target.find('#field-isreadonly').prop("checked", true);
    } else {
        target.find('#field-isreadonly').prop("checked", false);
    }

    target.find('#field-colspan').val(selected.attr('data-colspan'));

    var colsItems = getTargetColnums(selected);
    //$("#field-colspan").attr("data-value", selected.attr('data-colspan')).val(selected.attr('data-colspan'));
    $('#field-colspan').removeAttr("data-picklistinit");

    $('#field-colspan').on("picklist.getTarget", function (e, obj) {
        var test = null;
        selerin = obj.target.get(0).id;
    });
    if (selerin) {
        if ($("#" + selerin).length > 0) {
            $("#" + selerin).remove();
            selerin = null;
        }
    }
    $('#field-colspan').picklist({
        required: true,
        items: colsItems
    });
    //console.log(selected.attr('data-colspan'))
    $('#field-colspan').attr("data-value", selected.attr('data-colspan'))
        //.next('select').find("option").prop("selected", false)
        .next('select').find("option[value='" + selected.attr('data-colspan') + "']").prop("selected", true);
    //设置是否显示
    $("#field-IsVisible").prop("checked", setTrueOrFalse(selected.attr("data-isvisible")));
    //设置是否显示标题
    $("#field-isshowlabel").prop("checked", setTrueOrFalse(selected.attr("data-isshowlabel")));
    //target.find('#field-rowspan').val(selected.attr('data-rowspan'));
    //event
    var scriptHtml = [];
    var scriptselect = [];
    for (var i = 0; i < scriptlist.length; i++) {
        // if (scriptlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
        scriptHtml.push('<tr onclick="chooseRowInfo(this)" class="scriptrow" data-id="' + scriptlist[i].Id + '"><td data-value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</td><td>' + scriptlist[i].Info + '</td></tr>');
        scriptselect.push('<option value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</option>');
        // }
    }
    target.find('#fieldScript').html(scriptHtml.join(''));
    var eventHtml = [];
    target.find('#fieldEvent').html('');
    for (var i = 0; i < eventlist.length; i++) {
        if (eventlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
            target.find('#fieldEvent').append('<tr class="eventitem" data-id="' + eventlist[i].eventid + '" onclick="chooseRowInfo(this)"><td><select class="scriptlist">' + scriptselect.join('') + '</select></td><td><select><option value="onchange">onchange</option></select></td><td><input type="text" value="' + eventlist[i].JsAction + '" /></td></tr>');
            target.find('#fieldEvent').find('tr:last').find('td:eq(0)').find('option[value="' + eventlist[i].JsLibrary + '"]').attr('selected', "selected");
            target.find('#fieldEvent').find('tr:last').find('td:eq(1)').find('option[value="' + eventlist[i].Name + '"]').attr('selected', "selected");
        }
    }

    //如果是数字类型显示公式选项卡
    $('#attributeTab li:first a').click();
    if (attrType == 'int' || attrType == 'float' || attrType == 'money') {
        $('#attributeTab li:eq(2)').show();
        showFormulaRule();
    } else {
        $('#attributeTab li:eq(2)').hide();
    }

    //样式配置
    var customcss = selected.attr('data-customcss');
    if (customcss && customcss != "") {
        try {
            var styles = JSON.parse(customcss)

            if (styles && styles.labels) {
                var _labels = styles.labels;
                _labels['color'] ? $('input[name="tableattribute-islabelfontcolor"]').prop('checked', true) && $('input[name="tableattribute-labelfontcolor"]').spectrum("set", _labels['color']) : $('input[name="tableattribute-islabelfontcolor"]').prop('checked', false)

                _labels['font-size'] ? $('input[name="tableattribute-islabelfontsize"]').prop('checked', true) && $('input[name="tableattribute-labelfontsize"]').jRange('setValue', _labels['font-size']) : $('input[name="tableattribute-islabelfontsize"]').prop('checked', false)

                _labels['background-color'] ? $('input[name="tableattribute-islabelbackgroundcolor"]').prop('checked', true) && $('input[name="tableattribute-labelbackgroundcolor"]').spectrum("set", _labels['background-color']) : $('input[name="tableattribute-islabelbackgroundcolor"]').prop('checked', false);

                _labels['text-align'] ? $('input[name="tableattribute-islabeltextalign"]').prop('checked', true) && $('input[name="tableattribute-labeltextalign"][value="' + _labels['text-align'] + '"]').prop('checked', true) : $('input[name="tableattribute-islabeltextalign"]').prop('checked', false);
            }
            if (styles && styles.inputs) {
                var _inputs = styles.inputs;
                _inputs['border-color'] ? $('input[name="tableattribute-isinputbordercolor"]').prop('checked', true) && $('input[name="tableattribute-inputbordercolor"]').spectrum("set", _inputs['border-color']) : $('input[name="tableattribute-isinputbordercolor"]').prop('checked', false)
                _inputs['border-width'] ? $('input[name="tableattribute-isinputbordersize"]').prop('checked', true) && $('input[name="tableattribute-inputbordersize"]').jRange('setValue', _inputs['border-width']) : $('input[name="tableattribute-isinputbordersize"]').prop('checked', false)
                _inputs['background-color'] ? $('input[name="tableattribute-isinputbackgroundcolor"]').prop('checked', true) && $('input[name="tableattribute-inputbackgroundcolor"]').spectrum("set", _inputs['background-color']) : $('input[name="tableattribute-isinputbackgroundcolor"]').prop('checked', false)
                _inputs['text-align'] ? $('input[name="tableattribute-isinputtextalign"]').prop('checked', true) && $('input[name="tableattribute-inputtextalign"][value="' + _inputs['text-align'] + '"]').prop('checked', true) : $('input[name="tableattribute-isinputtextalign"]').prop('checked', false);
            }
        } catch (e) { console.error(e) }
    } else {
        $('input[name="tableattribute-islabelfontcolor"]').prop('checked', false);
        $('input[name="tableattribute-islabelfontsize"]').prop('checked', false);
        $('input[name="tableattribute-islabelbackgroundcolor"]').prop('checked', false);
        $('input[name="tableattribute-islabeltextalign"]').prop('checked', false)
        $('input[name="tableattribute-isinputbordercolor"]').prop('checked', false);
        $('input[name="tableattribute-isinputbordersize"]').prop('checked', false);
        $('input[name="tableattribute-isinputbackgroundcolor"]').prop('checked', false);
        $('input[name="tableattribute-isinputtextalign"]').prop('checked', false);
    }
    changeStylePreviewTarget();
    activeAttributeScriptTag();
    target.modal({
        keyboard: true
    })
}

function filterAttrTypeList(attrtype) {
    return function (obj, key) {
        var objtype = obj.attributetypename
        if (attrtype == 'lookup') {
            if (objtype == 'owner' || objtype == 'lookup' || objtype == 'customer') {
                return true;
            }
        } else {
            return objtype == attrtype;
        }
    }
}
function saveField() {
    var target = $('#attributeModal');
    var selected = $('.selected');
    var reColspan = selected.attr('data-colspan');//没变列数之前
    selected.parent().removeClass('col-sm-' + (reColspan * 2));
    selected.attr('data-label', target.find('#field-label').val());
    selected.attr('data-isshowlabel', target.find('#field-isshowlabel').prop('checked'));
    selected.attr('data-isvisible', target.find('#field-isvisible').prop('checked'));
    selected.attr('data-isreadonly', target.find('#field-isreadonly').prop('checked'));
    selected.attr('data-colspan', target.find('#field-colspan').val());
    selected.attr('data-rowspan', target.find('#field-rowspan').val());
    selected.parent().addClass('col-sm-' + (selected.attr('data-colspan') * 2));
    if (selected.attr('data-controltype') == 'lookup' || selected.attr('data-controltype') == 'owner' || selected.attr('data-controltype') == 'customer') {
        selected.attr('data-defaultviewreadonly', true);
        selected.attr('data-defaultviewId', Xms.Web.SelectedValue(target.find('#field-viewid')));
        selected.attr('data-viewpickerreadOnly', true);
        selected.attr('data-disableviewpicker', true);
        if ($('#field-IsFilterRelation').prop("checked")) {
            selected.attr('data-filterrelationshipname', $('#field-RelationRecord>option:selected').attr("data-relationshipname"));
            selected.attr('data-dependentattributename', $('#field-RelationRecordSourc>option:selected').attr("data-attributename"));
            selected.attr('data-dependentattributetype', '');
            selected.attr('data-isfilterrelation', true);
            selected.attr('data-allowfilteroff', $('#field-AllowFilterOff').prop("checked"));
        } else {
            selected.attr('data-filterrelationshipname', '');
            selected.attr('data-dependentattributename', '');
            selected.attr('data-dependentattributetype', '');
            selected.attr('data-allowfilteroff', false);
            selected.attr('data-isfilterrelation', false);
        }
    }

    selected.attr('data-fieldentityname', $("#field-EntityName>option:selected").attr("data-referencedentityname") || "");
    selected.attr('data-fieldattributename', $("#field-AttributeName>option:selected").attr("data-name") || "");
    selected.attr('data-fieldsourceattributename', $("#field-EntityName>option:selected").attr("data-referencingattributename") || "");
    selected.attr('data-fieldsourceattributetype', $("#field-AttributeName>option:selected").attr("data-type") || "");
    selected.find('th').text(target.find('#field-label').val());
    selected.find('td').text(target.find('#field-label').val());
    if (target.find('#field-isshowlabel').prop('checked') == false) {
        selected.find('th').addClass("disable-text");
    } else {
        selected.find('th').removeClass("disable-text");
    }
    if (target.find('#field-isvisible').prop('checked') == false) {
        selected.find('th').addClass("visible-hidden");
    } else {
        selected.find('th').removeClass("visible-hidden");
    }
    removeSriptEvent(selected.attr('data-name'));
    target.find('#fieldEvent').find('tr').each(function (i, n) {
        var eventitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(1)').find('option:selected').val(),
            'JsAction': $(n).find('td:eq(2)').find('input').val(),
            'JsLibrary': $(n).find('td:eq(0)').find('option:selected').val(),
            'eventid': Xms.Utility.Guid.NewGuid().ToString()
        };
        eventlist.push(eventitem);
    });
    target.find('#fieldScript').find('tr').each(function (i, n) {
        var scriptitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(0)').text(),
            'Info': $(n).find('td:eq(1)').text(),
            'Id': $(n).attr('data-id')
        };
        var flag = true;
        if (scriptlist.length > 0) {
            $.each(scriptlist, function (key, item) {
                if (item.Id == scriptitem.Id) {
                    flag = false;
                    return false;
                }
            });
        }
        if (flag == true) {
            scriptlist.push(scriptitem);
        }
    });
    //console.log(scriptlist);
    var columnsMax = selected.parents(".section").attr("data-columns");
    fieldColspanReset(selected, $('#field-colspan').val(), columnsMax, columnsMax, reColspan);
    var trRows = selected.parents(".section").find(">table>tbody>tr");
    // delEmptyTr(trRows)
    resetTableVis(trRows);
    delEmptyTr(trRows)
    initFieldEvent();
    formulaSave();		//保存公式

    //保存样式
    var styles = {
        labels: {
            //  'font-size': '12',
            //   'color': '#555',
            //   'background-color': 'none',
            //   'text-align': 'left'
        },
        inputs: {
            //  'border-color': '#ddd',
            //  'border-width': '1',
            ////  'background-color': 'none',
            //  'text-align': 'left'
        }
    };
    if ($('input[name="tableattribute-islabelfontcolor"]').prop('checked')) {
        styles.labels['color'] = $('input[name="tableattribute-labelfontcolor"]').val() ? $('input[name="tableattribute-labelfontcolor"]').val() : '';
    }
    if ($('input[name="tableattribute-islabelfontsize"]').prop('checked')) {
        styles.labels['font-size'] = $('input[name="tableattribute-labelfontsize"]').val() ? $('input[name="tableattribute-labelfontsize"]').val() : '';
    }
    if ($('input[name="tableattribute-islabelbackgroundcolor"]').prop('checked')) {
        styles.labels['background-color'] = $('input[name="tableattribute-labelbackgroundcolor"]').val() ? $('input[name="tableattribute-labelbackgroundcolor"]').val() : '';
    }
    if ($('input[name="tableattribute-islabeltextalign"]').prop('checked')) {
        styles.labels['text-align'] = $('input[name="tableattribute-labeltextalign"]:checked').val() ? $('input[name="tableattribute-labeltextalign"]:checked').val() : '';
    }
    if ($('input[name="tableattribute-isinputbordercolor"]').prop('checked')) {
        styles.inputs['border-color'] = $('input[name="tableattribute-inputbordercolor"]').val() ? $('input[name="tableattribute-inputbordercolor"]').val() : '';
    }
    if ($('input[name="tableattribute-isinputbordersize"]').prop('checked')) {
        styles.inputs['border-width'] = $('input[name="tableattribute-inputbordersize"]').val() ? $('input[name="tableattribute-inputbordersize"]').val() : '';
    }
    if ($('input[name="tableattribute-isinputbackgroundcolor"]').prop('checked')) {
        styles.inputs['background-color'] = $('input[name="tableattribute-inputbackgroundcolor"]').val() ? $('input[name="tableattribute-inputbackgroundcolor"]').val() : '';
    }
    if ($('input[name="tableattribute-isinputtextalign"]').prop('checked')) {
        styles.inputs['text-align'] = $('input[name="tableattribute-inputtextalign"]:checked').val() ? $('input[name="tableattribute-inputtextalign"]:checked').val() : '';
    }

    if (selected.attr('data-customcss') && selected.attr('data-customcss') != "") {
        var cuscss = JSON.parse(selected.attr('data-customcss'));
        styles = $.extend(true, {}, styles);
    }
    selected.attr('data-customcss', JSON.stringify(styles));
    activeAttributeScriptTag()
    //handleFieldColspan(selected.parent());
    target.modal('hide');
}

function changeStylePreviewTarget() {
    var styles = {
        labels: {},
        inputs: {}
    }
    if ($('input[name="tableattribute-islabelfontcolor"]').prop('checked')) {
        styles.labels['color'] = $('input[name="tableattribute-labelfontcolor"]').val() ? $('input[name="tableattribute-labelfontcolor"]').val() : '';
    } else {
        styles.labels['color'] = 'initial';
    }
    if ($('input[name="tableattribute-islabelfontsize"]').prop('checked')) {
        styles.labels['font-size'] = $('input[name="tableattribute-labelfontsize"]').val() ? $('input[name="tableattribute-labelfontsize"]').val() + 'px' : '';
    } else {
        styles.labels['font-size'] = 12 + 'px';
    }
    if ($('input[name="tableattribute-islabelbackgroundcolor"]').prop('checked')) {
        styles.labels['background-color'] = $('input[name="tableattribute-labelbackgroundcolor"]').val() ? $('input[name="tableattribute-labelbackgroundcolor"]').val() : '';
    } else {
        styles.labels['background-color'] = 'initial';
    }
    if ($('input[name="tableattribute-islabeltextalign"]').prop('checked')) {
        styles.labels['text-align'] = $('input[name="tableattribute-labeltextalign"]:checked').val() ? $('input[name="tableattribute-labeltextalign"]:checked').val() : '';
    } else {
        styles.labels['text-align'] = 'left';
    }
    $('#previewshowLabel').css(styles.labels);
    if ($('input[name="tableattribute-isinputbordercolor"]').prop('checked')) {
        styles.inputs['border-color'] = $('input[name="tableattribute-inputbordercolor"]').val() ? $('input[name="tableattribute-inputbordercolor"]').val() : '';
    } else {
        styles.inputs['border-color'] = 'initial';
    }
    if ($('input[name="tableattribute-isinputbordersize"]').prop('checked')) {
        styles.inputs['border-width'] = $('input[name="tableattribute-inputbordersize"]').val() ? $('input[name="tableattribute-inputbordersize"]').val() + 'px' : '';
    } else {
        styles.inputs['border-width'] = '0';
    }
    if ($('input[name="tableattribute-isinputbackgroundcolor"]').prop('checked')) {
        styles.inputs['background-color'] = $('input[name="tableattribute-inputbackgroundcolor"]').val() ? $('input[name="tableattribute-inputbackgroundcolor"]').val() : '';
    } else {
        styles.inputs['background-color'] = 'initial';
    }
    if ($('input[name="tableattribute-isinputtextalign"]').prop('checked')) {
        styles.inputs['text-align'] = $('input[name="tableattribute-inputtextalign"]:checked').val() ? $('input[name="tableattribute-inputtextalign"]:checked').val() : '';
    } else {
        styles.inputs['text-align'] = 'left';
    }
    $('#previewshowInput').css(styles.inputs);
}

//插入空格
function insertSpace() {
    var selected = $('#formBody').find('.selected');
    if (selected.length <= 0) {
        selected = $('#formBody').find('.tab:first');
    }
    if (selected.is('.section') || selected.is('.tab')) {
        if (selected.find('.placeholder').length > 0) {
        }
        else {
            //var html = [];
            //html.push('<tr>');
            //html.push('<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>')
            //html.push('</tr>');
            var count = selected.find("div.section:first").attr("data-columns");
            createNewTrAfter(selected.find('table:first > tbody>tr:last'), 1, count, "after");
            //selected.find('table:first > tbody').append($(html.join('')));
        }
        var ifhtml = [];
        ifhtml.push('<table class="table cell" data-colspan="1">');
        ifhtml.push('<tbody>');
        ifhtml.push('<tr style="height:100%;"><th class="col-sm-3">空格</th><td class="col-sm-4">空格</td></tr>');
        ifhtml.push('</tbody>');
        ifhtml.push('</table>');

        var selTd = selected.find('.placeholder:first'); selTd.attr('data-label', '');
        selTd.html(ifhtml.join('')).removeClass('placeholder');
        selTd.children('table').attr('data-name', 'none');
        selTd.children('table').attr('data-entityname', 'none');
        selTd.children('table').attr('data-controltype', 'none');
        selTd.children('table').attr('data-type', '');
        selTd.children('table').attr('data-isedit', '0');
        selTd.children('table').attr('data-isshowlabel', 'true');
        selTd.children('table').attr('data-isvisible', 'true');

        initFieldEvent();
    }
}

//嵌入页面
function insertIframe() {
    var target = $('#iframeModal');
    target.attr('showtype', 'new');
    target.find('#iframe-name').val('');
    target.find('#iframe-url').val('');
    target.find('#iframe-rowsize').val(1);
    target.find('#iframe-border').attr('checked', false);
    target.find('#iframe-isshowlabel').attr('checked', false);
    target.modal('show');
}
function editIframe() {
    var selected = $('.selected');
    var target = $('#iframeModal');
    target.attr('showtype', 'edit');
    target.find('#iframe-name').val(selected.attr('data-name'));
    target.find('#iframe-url').val(selected.attr('data-url'));
    target.find('#iframe-rowsize').val(selected.attr('data-rowsize'));
    target.find('#iframe-scroll').find('option[value=' + selected.attr('data-scroll') + ']').attr('selected', 'selected');
    if (selected.attr('data-border') == 'true') {
        target.find('#iframe-border').prop('checked', true);
    } else {
        target.find('#iframe-border').prop('checked', false);
    }
    if (selected.attr('data-allowcrossdomain') == 'true') {
        target.find('#iframe-allowcrossdomain').prop('checked', true);
    } else {
        target.find('#iframe-allowcrossdomain').prop('checked', false);
    }
    if (selected.attr('data-isshowlabel') == 'true') {
        target.find('#iframe-isshowlabel').prop('checked', true);
    } else {
        target.find('#iframe-isshowlabel').prop('checked', false);
    }
    target.find('#iframe-entityname').val(selected.attr('data-entityname'));
    target.find('#iframe-isvisible').val(selected.attr('data-isvisible'));
    target.modal('show');
}
function saveIframe() {
    var target = $('#iframeModal');
    var savetype = target.attr('showtype');
    if (target.find('#iframe-name').val() == '') {
        Xms.Web.Alert(false, '请输入名称');
        return false;
    }
    if (target.find('#iframe-url').val() == '') {
        Xms.Web.Alert(false, '请输入链接');
        return false;
    }
    if (target.find('#iframe-rowsize').val() == '') {
        Xms.Web.Alert(false, '请输入行数');
        return false;
    }

    var selected = $('#formBody').find('.selected');
    if (savetype && savetype == 'new') {
        if (selected.length <= 0) {
            selected = $('#formBody').find('.tab:first');
        }
        else if (selected.length > 0 && $('#formBody').find('.selected.cell').length > 0) {
            selected = selected.parents('.tab');
        }
        var count = selected.find("div.section:first").attr("data-columns");
        createNewTrAfter(selected.find('table:first > tbody>tr:last'), 1, count, "after");
        var selTd = selected.find('.placeholder:first');
        var ifhtml = [];
        ifhtml.push('<table class="table cell">');
        ifhtml.push('<tbody>');
        ifhtml.push('<tr style="height:100%;"><th class="col-sm-3">' + target.find('#iframe-name').val() + '</th><td class="col-sm-4">' + target.find('#iframe-name').val() + '</td></tr>');
        ifhtml.push('</tbody>');
        ifhtml.push('</table>');
        selTd.html(ifhtml.join('')).removeClass('placeholder');
        selTd = selTd.children('table');
    } else {
        selTd = selected;
    }
    selTd.find('th').text(target.find('#iframe-name').val());
    selTd.find('td').text(target.find('#iframe-name').val());
    selTd.attr('data-label', target.find('#iframe-name').val());
    selTd.attr('data-name', target.find('#iframe-name').val());
    selTd.attr('data-entityname', target.find('#iframe-entityname').val());
    selTd.attr('data-controltype', 'iFrame');
    selTd.attr('data-type', '');
    selTd.attr('data-isshowlabel', target.find('#iframe-isshowlabel').prop('checked'));
    selTd.attr('data-url', target.find('#iframe-url').val());
    selTd.attr('data-rowsize', target.find('#iframe-rowsize').val());
    selTd.attr('data-border', target.find('#iframe-border').prop('checked'));
    selTd.attr('data-isvisible', 'true');
    selTd.attr('data-colspan', 1);
    selTd.attr('data-allowcrossdomain', target.find('#iframe-allowcrossdomain').prop('checked'));

    target.modal('hide');
    initFieldEvent();
}

//保存表单设置
function saveForm() {
    var formObject = new Xms.Form.FormDescriptor();
    formObject.IsShowNav = $('#form-isshownav').prop('checked');
    formObject.Name = $('#form-name').val();
    formObject.Description = $('#form-description').val();
    formObject.NavGroups = new Array();
    formObject.CustomCss = $('#CustomCss').val();
    //nav
    $('#formNav .navGroup:not(.locked)').each(function (i, n) {
        var ng = new Xms.Form.NavGroupDescriptor();
        ng.Label = $(n).attr('data-label');
        ng.IsVisible = true;
        ng.NavItems = new Array();
        $(n).find('.nav-item').each(function (i, n) {
            var item = new Xms.Form.NavDescriptor();
            item.Label = $(n).attr('data-label');
            item.Icon = $(n).attr('data-icon');
            // console.log($(n).attr('data-type') == "1")
            if ($(n).attr('data-type') == "1") {
                item.Url = '';
                item.RelationshipName = $(n).attr('data-relationshipname');
                var entityid = $(n).attr('data-entityid');
                if (entityid) {
                    item.Id = $(n).attr('data-entityid') || '';
                }
            } else if ($(n).attr('data-type') == "0") {
                item.Url = $(n).attr('data-url') || '';
                item.RelationshipName = '';
                var entityid = $(n).attr('data-entityid');
                if (entityid) {
                    item.Id = $(n).attr('data-entityid') || '';
                }
            }
            item.IsVisible = true;
            ng.NavItems.push(item);
        });
        formObject.NavGroups.push(ng);
    });
    // console.log(formObject.NavGroups)
    //header
    var headerNode = $('#formContent .header');
    var header = new Xms.Form.SectionDescriptor();
    header.Name = 'formHeader';
    header.Label = headerNode.attr('data-label');
    header.IsShowLabel = headerNode.attr('data-isshowlabel');
    header.IsVisible = headerNode.attr('data-isvisible');
    header.CellLabelWidth = headerNode.attr('data-celllabelwidth');
    header.CellLabelAlignment = headerNode.attr('data-celllabelalignment');
    header.CellLabelPosition = headerNode.attr('data-celllabelposition');
    header.Columns = headerNode.attr('data-columns');
    header.Rows = new Array();
    $('#formContent .header > table > tbody > tr').each(function (i, n) {
        var row = new Xms.Form.RowDescriptor();
        row.IsVisible = true;
        row.Cells = new Array();
        $(n).find('.field').each(function (ii, nn) {
            if ($(nn).hasClass = "placeholder") {
                // return false;
            }
            var cell = new Xms.Form.CellDescriptor();
            if ($(nn).children('table').length > 0) {
                cell.Label = $(nn).children('table').attr('data-label');
                cell.IsShowLabel = $(nn).children('table').attr('data-isshowlabel');
                cell.IsVisible = $(nn).children('table').attr('data-isvisible');
                cell.ColSpan = $(nn).children('table').attr('data-colspan');
                cell.RowSpan = $(nn).children('table').attr('rowspan');
                cell.Control.ControlType = $(nn).children('table').attr('data-controltype');
                cell.Control = new Xms.Form.ControlDescriptor();
                cell.Control.Name = $(nn).children('table').attr('data-name');
                if ($(nn).children('table').attr('data-customcss')) {
                    cell.CustomCss = $(nn).children('table').attr('data-customcss')
                }
            } else {
                cell.Control = new Xms.Form.ControlDescriptor();
                cell.Control.Name = ' ';
                cell.Control.ControlType = 'none';
                cell.Label = undefined;
                cell.IsShowLabel = undefined;
                cell.IsVisible = undefined;
                cell.ColSpan = undefined;
                cell.RowSpan = undefined;
            }
            if (cell.Control.Name && cell.Control.Name != '') {
                row.Cells.push(cell);
            }
        });
        header.Rows.push(row);
    });
    formObject.Header = header;
    //footer
    var footerNode = $('#formContent .footer');
    var footer = new Xms.Form.SectionDescriptor();
    footer.Name = 'formFooter';
    footer.Label = footerNode.attr('data-label');
    footer.IsShowLabel = footerNode.attr('data-isshowlabel');
    footer.IsVisible = footerNode.attr('data-isvisible');
    footer.CellLabelWidth = footerNode.attr('data-celllabelwidth');
    footer.CellLabelAlignment = footerNode.attr('data-celllabelalignment');
    footer.CellLabelPosition = footerNode.attr('data-celllabelposition');
    footer.Columns = footerNode.attr('data-columns');
    footer.Rows = new Array();
    $('#formContent .footer > table > tbody > tr').each(function (i, n) {
        var row = new Xms.Form.RowDescriptor();
        row.IsVisible = true;
        row.Cells = new Array();

        $(n).find('.field').each(function (ii, nn) {
            if ($(nn).hasClass('placeholder')) {
                //  return false;
            }
            var cell = new Xms.Form.CellDescriptor();
            if ($(nn).children('table').length > 0) {
                cell.Label = $(nn).children('table').attr('data-label');
                cell.IsShowLabel = $(nn).children('table').attr('data-isshowlabel');
                cell.IsVisible = $(nn).children('table').attr('data-isvisible');
                cell.ColSpan = $(nn).children('table').attr('data-colspan');
                cell.RowSpan = $(nn).children('table').attr('rowspan');
                cell.Control = new Xms.Form.ControlDescriptor();
                cell.Control.Name = $(nn).children('table').attr('data-name');
            } else {
                cell.Control = new Xms.Form.ControlDescriptor();
                cell.Control.Name = ' ';
                cell.Control.ControlType = 'none';
                cell.Label = undefined;
                cell.IsShowLabel = undefined;
                cell.IsVisible = undefined;
                cell.ColSpan = undefined;
                cell.RowSpan = undefined;
            }
            if (cell.Control.Name && cell.Control.Name != '') {
                row.Cells.push(cell);
            }
        });
        footer.Rows.push(row);
    });
    formObject.Footer = footer;
    //body
    formObject.Panels = new Array();
    $('#formBody > .tab').each(function (i, n) {
        var panel = new Xms.Form.PanelDescriptor();
        panel.Name = $(n).attr('data-name') || (new Date() * 1 + setTimeout(0));
        panel.Label = $(n).attr('data-label');
        panel.IsExpanded = $(n).attr('data-isexpanded') == "true" ? true : false;
        panel.IsShowLabel = $(n).attr('data-isshowlabel') == "true" ? true : false;
        panel.IsVisible = $(n).attr('data-isvisible') == "true" ? true : false;
        panel.Async = $(n).attr('data-isasync') == "true" ? true : false;
        panel.DisplayStyle = $(n).attr('data-displaystyle');
        panel.Sections = new Array();
        $(n).find('.section').each(function (ii, nn) {
            var sec = new Xms.Form.SectionDescriptor();
            sec.Name = $(nn).attr('data-name') || "formBody";
            sec.Label = $(nn).attr('data-label');
            sec.IsShowLabel = $(nn).attr('data-isshowlabel') == "true" ? true : false;
            sec.IsVisible = $(nn).attr('data-isvisible') == "true" ? true : false;
            sec.CellLabelWidth = $(nn).attr('data-celllabelwidth');
            sec.CellLabelAlignment = $(nn).attr('data-celllabelalignment');
            sec.CellLabelPosition = $(nn).attr('data-celllabelposition');
            sec.CellLabelSettings = {};
            sec.CellLabelSettings.Width = $(nn).attr('data-attrwidth') || '';
            sec.CellLabelSettings.Alignment = $(nn).attr('data-attralignment') || 'Left';
            sec.CellLabelSettings.Position = $(nn).attr('data-attrposition') || 'Left';
            sec.Columns = $(nn).attr('data-columns');
            sec.Rows = new Array();
            $(nn).find('> table > tbody > tr').each(function (i, n) {
                var row = new Xms.Form.RowDescriptor();
                row.IsVisible = true;
                row.Cells = new Array();
                $(n).find('.field').each(function (iii, nnn) {
                    if ($(nnn).hasClass('placeholder')) {
                        //   return false;
                    }
                    var cell = new Xms.Form.CellDescriptor();
                    if ($(nnn).children('table').length > 0) {
                        cell.Label = $(nnn).children('table').attr('data-label');
                        cell.IsShowLabel = $(nnn).children('table').attr('data-isshowlabel') == "true" ? true : false;
                        cell.IsVisible = $(nnn).children('table').attr('data-isvisible') == "true" ? true : false;
                        //cell.Control.ReadOnly = $(nnn).children('table').attr('data-isreadonly');
                        cell.ColSpan = $(nnn).children('table').attr('data-colspan');
                        cell.RowSpan = $(nnn).children('table').attr('data-rowspan');
                        cell.Control = new Xms.Form.ControlDescriptor();
                        cell.Control.Name = $(nnn).children('table').attr('data-name');
                        cell.Control.ControlType = $(nnn).children('table').attr('data-controltype');

                        if ($(nnn).children('table').attr('data-customcss')) {
                            cell.CustomCss = $(nnn).children('table').attr('data-customcss')
                        }

                        if (($(nnn).children('table').attr('data-type') == 'lookup' || $(nnn).children('table').attr('data-controlType') == 'lookup') && $(nnn).children('table').attr('data-defaultviewid') != '') {
                            var parm = {
                                DefaultViewReadOnly: $(nnn).children('table').attr('data-defaultviewreadonly'),
                                DefaultViewId: $(nnn).children('table').attr('data-defaultviewid'),
                                ViewPickerReadOnly: $(nnn).children('table').attr('data-viewpickerreadonly'),
                                DisableViewPicker: $(nnn).children('table').attr('data-disableviewpicker')
                            };
                            cell.Control.Parameters = parm;
                        }
                        if ($(nnn).children('table').attr('data-controlType') == 'lookup' || $(nnn).children('table').attr('data-controlType') == 'owner' || $(nnn).children('table').attr('data-controlType') == 'customer') {
                            var lookupparam = {
                                FilterRelationshipName: $(nnn).children('table').attr('data-filterrelationshipname'),
                                DependentAttributeName: $(nnn).children('table').attr('data-dependentattributename'),
                                DependentAttributeType: $(nnn).children('table').attr('data-dependentattributetype'),
                                AllowFilterOff: $(nnn).children('table').attr('data-allowfilteroff')
                            }
                            cell.Control.ControlType = "lookup";
                            cell.Control.Parameters = $.extend({}, cell.Control.Parameters, lookupparam);
                        }
                        if ($(nnn).children('table').attr('data-controlType') == 'label') {
                            var labelparam = {
                                EntityName: $(nnn).children('table').attr('data-labelentityname'),
                                AttributeName: $(nnn).children('table').attr('data-attributename'),
                                SourceAttributeName: $(nnn).children('table').attr('data-sourceattributename'),
                                SourceAttributeType: $(nnn).children('table').attr('data-sourceattributetype') || ""
                            }
                            cell.Control.ControlType = "label";
                            cell.Control.Parameters = $.extend({}, cell.Control.Parameters, labelparam);
                        } else if ($(nnn).children('table').attr('data-controlType') == 'freetext') {
                            var labelparam = {
                                EntityName: $(nnn).children('table').attr('data-labelentityname'),
                                AttributeName: $(nnn).children('table').attr('data-attributename'),
                                Content: $(nnn).children('table').attr('data-paramcontent')
                            }
                            cell.Control.ControlType = "freetext";
                            cell.Control.Parameters = $.extend({}, cell.Control.Parameters, labelparam);
                        } else {
                            var extendParam = {
                                EntityName: $(nnn).children('table').attr('data-fieldentityname') || "",
                                AttributeName: $(nnn).children('table').attr('data-fieldattributename') || "",
                                SourceAttributeName: $(nnn).children('table').attr('data-fieldsourceattributename') || ""
                            }
                            cell.Control.Parameters = $.extend({}, cell.Control.Parameters, extendParam);
                        }
                        if (typeof $(nnn).children('table').attr('data-controltype') != 'undefined') {
                            if ($(nnn).children('table').attr('data-controltype').toLowerCase() == 'subgrid') {
                                cell.ColSpan = $(nnn).children('table').attr('data-colspan');
                                cell.RowSpan = $(nnn).children('table').attr('data-pagesize');
                                /*读取单据体字段间的值计算 数据*/
                                var formulars = [];
                                var formularvalue = $(nnn).children('table').attr('data-formulavalue');
                                var formularArr = [];
                                if (formularvalue && formularvalue != '') {
                                    formularArr = JSON.parse(decodeURIComponent(formularvalue));
                                }
                                console.log(formularArr)
                                /*读取单据体字段间的值计算 数据   end */
                                var parm = {
                                    ViewId: $(nnn).children('table').attr('data-viewid'),
                                    PageSize: $(nnn).children('table').attr('data-pagesize'),
                                    TargetEntityName: $(nnn).children('table').attr('data-targetentityname'),
                                    RelationshipName: $(nnn).children('table').attr('data-relationshipname'),
                                    Editable: $(nnn).children('table').attr('data-editable'),
                                    PagingEnabled: $(nnn).children('table').attr('data-ispager') == 'true' ? true : false,
                                    DefaultEmptyRows: $(nnn).children('table').attr('data-rowcount'),
                                    FieldEvents: formularArr
                                };

                                cell.Control.Parameters = parm;
                            } else {
                                $(nnn).children('table').attr('data-formulajson') && (cell.Control.Formula = $(nnn).children('table').attr('data-formulajson'));
                            }
                            if ($(nnn).children('table').attr('data-controltype').toLowerCase() == 'iframe') {
                                cell.ColSpan = $(nnn).children('table').attr('data-colspan');
                                cell.RowSpan = $(nnn).children('table').attr('data-rowsize');
                                var parm = {
                                    Url: $(nnn).children('table').attr('data-url'),
                                    Border: $(nnn).children('table').attr('data-border'),
                                    Scroll: $(nnn).children('table').attr('data-scroll'),
                                    AllowCrossDomain: $(nnn).children('table').attr('data-allowcrossdomain'),
                                    RowSize: $(nnn).children('table').attr('data-rowsize')
                                };
                                cell.Control.Parameters = parm;
                            }
                        }
                        if ($(nnn).children('table').attr('data-controltype').toLowerCase() == 'none') {
                            cell.Control = new Xms.Form.ControlDescriptor();
                            cell.Control.Name = ' ';
                            cell.Control.ControlType = 'none';
                            cell.Label = undefined;
                            cell.IsShowLabel = undefined;
                            cell.IsVisible = undefined;
                            cell.ColSpan = undefined;
                            cell.RowSpan = undefined;
                        }
                        cell.Control.ReadOnly = $(nnn).children('table').attr('data-isreadonly');
                    }

                    if (cell.Control.Name && cell.Control.Name != '') {
                        row.Cells.push(cell);
                    }
                });
                sec.Rows.push(row);
            });
            panel.Sections.push(sec);
        });
        console.log(formObject);
        formObject.Panels.push(panel);
    });
    formObject.Sections = new Array();
    $('#formBody > .section').each(function (i, nn) {
        var sec = new Xms.Form.SectionDescriptor();
        sec.Name = '';
        sec.Label = $(nn).attr('data-label');
        sec.IsShowLabel = $(nn).attr('data-isshowlabel');
        sec.IsVisible = $(nn).attr('data-isvisible');
        sec.CellLabelWidth = $(nn).attr('data-celllabelwidth');
        sec.CellLabelAlignment = $(nn).attr('data-celllabelalignment');
        sec.CellLabelPosition = $(nn).attr('data-celllabelposition');
        sec.Columns = $(nn).attr('data-columns');
        sec.Rows = new Array();
        $(nn).find('> table > tbody > tr').each(function (i, n) {
            var row = new Xms.Form.RowDescriptor();
            row.IsVisible = true;
            row.Cells = new Array();
            $(n).find('.field').each(function (iii, nnn) {
                if ($(nnn).hasClass('placeholder')) {
                    return false;
                }
                var cell = new Xms.Form.CellDescriptor()
                cell.Label = $(nnn).children('table').attr('data-label');
                cell.IsShowLabel = $(nnn).children('table').attr('data-isshowlabel');
                cell.IsVisible = $(nnn).children('table').attr('data-isvisible');
                cell.ColSpan = $(nnn).children('table').attr('colspan');
                cell.RowSpan = $(nnn).children('table').attr('rowspan');
                cell.Control = new Xms.Form.ControlDescriptor();
                cell.Control.Name = $(nnn).children('table').attr('data-name');
                cell.Control.ControlType = $(nnn).children('table').attr('data-controltype');
                if ($(nnn).children('table').attr('data-type') == 'lookup' && $(nnn).children('table').attr('data-defaultviewid') != '') {
                    var parm = {
                        DefaultViewReadOnly: $(nnn).children('table').attr('data-defaultviewreadonly'),
                        DefaultViewId: $(nnn).children('table').attr('data-defaultviewid'),
                        ViewPickerReadOnly: $(nnn).children('table').attr('data-viewpickerreadonly'),
                        DisableViewPicker: $(nnn).children('table').attr('data-disableviewpicker')
                    };
                    cell.Control.Parameters = parm;
                }
                if ($(nnn).children('table').attr('data-type') == 'lookup') {
                    var lookupparam = {
                        FilterRelationshipName: $(nnn).children('table').attr('data-filterrelationshipname'),
                        DependentAttributeName: $(nnn).children('table').attr('data-dependentattributename'),
                        DependentAttributeType: $(nnn).children('table').attr('data-dependentattributetype'),
                        AllowFilterOff: $(nnn).children('table').attr('data-allowfilteroff')
                    }
                    cell.Control.Parameters = $.extend({}, cell.Control.Parameters, lookupparam);
                }
                if (typeof $(nnn).children('table').attr('data-controltype') != 'undefined') {
                    if ($(nnn).children('table').attr('data-controltype').toLowerCase() == 'subgrid') {
                        cell.ColSpan = $(nnn).children('table').attr('data-colspan');
                        cell.RowSpan = $(nnn).children('table').attr('data-pagesize');

                        var parm = {
                            ViewId: $(nnn).children('table').attr('data-viewid'),
                            PageSize: $(nnn).children('table').attr('data-pagesize'),
                            TargetEntityName: $(nnn).children('table').attr('data-targetentityname'),
                            RelationshipName: $(nnn).children('table').attr('data-relationshipname'),
                            Editable: $(nnn).children('table').attr('data-editable')
                        };
                        cell.Control.Parameters = parm;
                    }
                    if ($(nnn).children('table').attr('data-controltype').toLowerCase() == 'iframe') {
                        cell.ColSpan = $(nnn).children('table').attr('data-colspan');
                        cell.RowSpan = $(nnn).children('table').attr('data-rowsize');
                        var parm = {
                            Url: $(nnn).children('table').attr('data-url'),
                            Border: $(nnn).children('table').attr('data-border'),
                            Scroll: $(nnn).children('table').attr('data-scroll'),
                            AllowCrossDomain: $(nnn).children('table').attr('data-allowcrossdomain'),
                            RowSize: $(nnn).children('table').attr('data-rowsize')
                        };
                        cell.Control.Parameters = parm;
                    }
                }
                if (cell.Control.Name && cell.Control.Name != '') {
                    row.Cells.push(cell);
                }
            });
            sec.Rows.push(row);
        });
        formObject.Sections.push(sec);
    });

    var xClientResources = [];
    $.each(scriptlist, function (key, item) {
        xClientResources.push(item.Id);
    });

    //event
    formObject.Events = eventlist;
    formObject.JsLibrary = scriptlist;
    formObject.ClientResources = xClientResources;
    $('#FormConfig').val(JSON.stringify(formObject));
}
function submitForm() {
    saveForm();
    Xms.Web.Form($('#editform'), function (response) {
        var autoRunEdit = false;
        var toEditId = null;
        if (formEditType == "create") {
            // Xms.Web.Alert(response.IsSuccess, response.Content, function () {
            Xms.Web.Event.localStorageEvent.trigger('list_form_rebind');
            toEditId = response.Extra.id;
            location.href = ORG_SERVERURL + "/customize/systemform/editform?id=" + response.Extra.id;
            autoRunEdit = true;
            //  });
            setTimeout(function () {
                if (toEditId != null && autoRunEdit == false) {
                    location.href = ORG_SERVERURL + "/customize/systemform/editform?id=" + toEditId;
                }
            }, 5000);
        } else {
            Xms.Web.Alert(response.IsSuccess, response.Content);
            Xms.Web.Event.publish('refresh');
        }
    });
    $('#editform').trigger('submit');
}
//脚本库  event
function ScriptCallBack(result, inputid) {
    if (result.length > 0) {
        var target = '', inputid = inputid || 'form';
        if (inputid == 'field') {
            target = $('#fieldScript');
        }
        else if (inputid == 'form') {
            target = $('#formScript');
        }
        else if (inputid == 'tab') {
            target = $('#tabScript');
        }
        console.log(result);
        $.each(result, function (key, obj) {
            if (target.find('tr[data-id="' + obj.id + '"]').length <= 0) {
                target.append('<tr onclick="chooseRowInfo(this)" class="scriptrow" data-id=' + obj.id + '><td data-value=' + obj.name + '>' + obj.name + '</td><td>' + obj.description + '</td></tr>');
            }
        });

        updateScriptItem(target);
    }
}

function addEvent(type) {
    if (type == 'field') {
        var eventhtml = [];
        eventhtml.push('<option value="onchange">onchange</option>');
        var eventtype = 'onchange';
        var name = '';
        var scripthtml = [];
        var scriptselect = $('#attributeTabContent').find('#fieldScript').find('tr');
        var eventTarget = $('#attributeTabContent').find('#fieldEvent');
    } else if (type == 'form') {
        var eventhtml = [];
        eventhtml.push('<option value="onload">onload</option>');
        eventhtml.push('<option value="onsave">onsave</option>');
        var name = '';
        var scripthtml = [];
        var scriptselect = $('#formTabContent').find('#formScript').find('tr');
        var eventTarget = $('#formTabContent').find('#formEvent');
    }
    else if (type = 'tab') {
        var eventhtml = [];
        eventhtml.push('<option value="onstatechange">onstatechange</option>');
        var name = '';
        var scripthtml = [];
        var scriptselect = $('#tabTabContent').find('#tabScript').find('tr');
        var eventTarget = $('#tabTabContent').find('#tabEvent');
    }
    if (scriptselect.length == 0) {
        Xms.Web.Alert(false, '请先添加脚本库');
        return false;
    }
    scriptselect.each(function (i, n) {
        scripthtml.push('<option value="' + $(n).find('td:eq(0)').text() + '">' + $(n).find('td:eq(0)').text() + '</option>');
    });
    var target = eventTarget;
    target.append('<tr class="eventitem" onclick="chooseRowInfo(this)" ><td><select class="scriptlist" >' + scripthtml.join('') + '</select></td><td><select>' + eventhtml.join('') + '</select></td><td><input type="text" value="' + name + '" placeholder="函数名" /></td></tr>');
    //<td><span class="btn btn-danger btn-xs" onclick="chooseRowInfo(this,\'item\');removeRow(this, \'event\');"><em class="glyphicon glyphicon-minus"></em>移除</span></td>
    if (target.length == 1) {
        target.find('tr:first').addClass('info');
    }
}
function updateScriptItem(target) {
    var rowlist = $(target).find('.scriptrow');
    var scriptselect = $(target).parents('.tab-content').find('.scriptlist');
    rowlist.each(function (i, n) {
        if (scriptselect.find('option[value="' + $(n).find('td:eq(0)').text() + '"]').length == 0) {
            scriptselect.append('<option value="' + $(n).find('td:eq(0)').text() + '">' + $(n).find('td:eq(0)').text() + '</option>');
        }
    });
    scriptselect.find('option').each(function (i, n) {
        if (rowlist.find('td:eq(0)[data-value="' + $(n).val() + '"]').length == 0) {
            $(n).remove();
        }
    });
    if (rowlist.length == 0) {
        $(target).parents('.tab-content').find('.eventitem').remove();
    }
}
function chooseRowInfo(e, type) {
    if (type == 'item') {
        $(e).parents('tr:first').addClass('info').siblings().removeClass('info');
    } else {
        $(e).addClass('info').siblings().removeClass('info');
    }
}
function removeRow(e, type) {
    var _par = $(e).parents("fieldset:first");
    _par.find('.info').remove();

    if (type == 'script') {
        var target = _par.find('tbody');
        updateScriptItem(target);
    } else if (type == 'event') {
        // var target = _par.find('.info');
        //  updateEventItem(target);
        //  _par.find('.info').remove();
    }
}
function updateEventItem(target) {
    //var eventid = $(target).attr('data-id');
    //for (var i = 0; i < eventlist.length; i++) {
    //    if (eventlist[i].eventid == eventid) {
    //        eventlist.splice(i, 1);
    //        i--;
    //    }
    //}
}
function moveRow(direction, e) {
    var target = $(e).parent().parent().find('.info');

    if (direction == 'after' && target.next().length > 0) {
        target.insertAfter(target.next());
    }
    else if (direction == 'before' && target.prev().length > 0) {
        target.insertBefore(target.prev());
    }
}
//移除脚本库和事件
function removeSriptEvent(attribute) {
    for (var i = 0; i < eventlist.length; i++) {
        if (eventlist[i].Attribute == attribute) {
            eventlist.splice(i, 1);
            i--;
        }
    }
    for (var i = 0; i < scriptlist.length; i++) {
        if (scriptlist[i].Attribute == attribute) {
            scriptlist.splice(i, 1);
            i--;
        }
    }
}
//选择Web资源
function bindSelectedResource(data, inputid) {
    console.log(data, inputid);
    var target = $('#' + inputid);
    var _html = [];
    $(data).each(function (i, n) {
        _html.push('<tr>');
        _html.push('<td><input type="hidden" name="id" value="' + n.id + '" />' + n.name + '</td>');
        _html.push('<td>' + n.description + '</td>');
        _html.push('</tr>');
    });
    target.find('tbody').append($(_html.join('')));
}

//
function findSourNext(sour, tar) {
    var next = sour;
    while (next.length == 1) {
        if (next == tar) {
            return true;
        }
        next = sour.next();
    }
    return false;
}
function findSourPrev(sour, tar) {
    var prev = sour;
    while (prev.length == 1) {
        if (prev == tar) {
            return true;
        }
        prev = sour.prev();
    }
    return false;
}

//交换两个元素
function exchange(a, b) {
    var n = a.next(), p = b.next();
    if (n.length > 0) {
        n.before(b);
        p.before(a);
    } else {
        var n = a.prev(), p = b.prev();
        n.after(b);
        p.after(a);
    }
};

//获得当前行的TD的index，包括colspan,//判断当前行是否有colspan>1的TD
function getCurrentTdIndex(obj) {
    var prev = obj.prev();
    var index = 0;
    while (prev.length > 0) {
        var colspan = prev.attr("colspan") * 1;
        if (colspan) {
            index += colspan;
        } else {
            index += 1;
        }
        prev = prev.prev();
    }
    return index;
}

//从当前列  查找是否存在有有元素的TD
function getSelectTdByIndex(obj, tr, trs, index) {
    var res = null;
    var nextHasEmpty = 0;
    var objIndex = index;
    var trsLen = trs.length;
    trs.attr("isCheckTd", false);
    var colspan = obj.attr("colspan") || 1;
    var next = tr.next();
    var now = tr;
    //判断当前行是否有colspan>1的TD，需要把index纠正；
    var curIndex = getCurrentTdIndex(obj);
    if (curIndex != objIndex) {
        objIndex = curIndex;
    }

    while (next.length > 0) {
        var count = 0;
        var flag = true;
        next.children("td").each(function () {
            var $this = $(this);
            var cols = $this.attr("colspan") || 1;
            // console.log(count, objIndex);
            if (count == objIndex) {
                if ($this.children("table").length > 0) {
                    res = $this;
                    flag = false;
                    return false;
                }
            }
            count += cols * 1;
        });
        if (flag == false) {
            break;
        }
        next = next.next();
    }
    return res;
}

//把一列的td往上移一格;  obj为当前空的TD
function moveTdByIndex(obj, tr, trs, index) {
    // return false;
    var nextHasEmpty = 0;
    var objIndex = index;
    var trsLen = trs.length;
    trs.attr("isCheckTd", false);
    // var clone = obj.clone();
    var colspan = obj.attr("colspan") || 1;
    // var tdTar = obj.next();
    var next = tr.next();
    var now = tr;
    //判断当前行是否有colspan>1的TD，需要把index纠正；
    var curIndex = getCurrentTdIndex(obj);
    if (curIndex != objIndex) {
        objIndex = curIndex;
    }
    var tdobj = getSelectTdByIndex(obj, tr, trs, index);
    if (tdobj) {
        if ((tdobj.attr("colspan") || 1) == (obj.attr("colspan") || 1))
            exchange(tdobj, obj);
    }
}

//创建新的行
function createNewTrAfter(obj, cols, count, type) {
    cols = cols || 1;
    type = type || "before"
    var objPar = obj;
    count = count || obj.children("td").length;
    var elArr = [];
    elArr.push('<tr>');
    for (var i = 0; i < count; i++) {
        elArr.push('<td class="col-sm-2 field placeholder" colspan="' + cols + '"></td>');
    }
    elArr.push('</tr>');
    var el = $(elArr.join(''));
    objPar[type](el);
    return el;
}

function checkTopPos(obj) {
    var objTd = obj.parent();
    var trs = objTd.parent();
    var objTdIndex = objTd.index();
    var tdList = [];
    var top = false;
    trs.each(function () {
        var item = $(this);
        var itd = item.children("td").eq(objTdIndex);
        if (itd.children('table').length == 0) {
            top = itd;
            return false;
        }
    });
    return top;
}

//检查目标行是否有足够的列可以放置
function checkRowColSum(t, s, scol, tcol) {
    var par = t.parent();
    var left = { count: 0, obj: null, empty: 0 }, right = { count: 0, obj: null, empty: 0 };
    var res = { left: left, right: right };
    var prev = par.prev();
    left.obj = prev;
    var leftFlag = true;
    while (prev.length > 0) {
        if (prev.children("table").length == 0) {
            var count = prev.attr("colspan") || 1;
            count = count * 1;
            left.count += count;
            if (leftFlag) {
                left.empty++;
            }
        } else {
            // if (left.empty > 0) {//如果是隔开的，则减1
            leftFlag = false;
            // }
        }
        prev = prev.prev();
    }
    var next = par.next();
    right.obj = next;
    var rightFlag = true;
    while (next.length > 0) {
        if (next.children("table").length == 0) {
            var count = next.attr("colspan") || 1;
            count = count * 1;
            right.count += count;

            if (rightFlag) {
                right.empty++;
            }
        } else {
            //  if (right.empty > 0) {//如果是隔开的，则减1
            rightFlag = false;
            // }
        }
        next = next.next();
    }
    return res;
}

function checkTrEmpty(trlist) {
    var res = [];
    trlist.each(function () {
        var $this = $(this), trIndex = $this.index();
        var tds = $this.children("td.field"), tdsLen = tds.length;
        var emptyCount = 0;
        var flag = true;
        tds.each(function () {
            var $td = $(this);
            if ($td.children("table").length == 0) {
                emptyCount++;
            }
        });
        if (tdsLen == emptyCount) {
            res.push(trIndex);
        }
    });
    return res;
}

function delEmptyTr(trRows) {
    //删除空的行
    var emptyRows = checkTrEmpty(trRows);
    for (var j = 0, len = emptyRows.length; j < len; j++) {
        if (trRows.eq(emptyRows[j]).length > 0) {
            trRows.eq(emptyRows[j]).remove();
        }
    }
}

//拖动后对表格的操作
function resetTableVis(trRows) {
    //return false;
    var recheckCounts = 0;//检查列的空TD数
    //往上移动所有TD
    resetTrRow(trRows);
    for (var i = 0; i < recheckCounts; i++) {
        //    resetTrRow(trRows);
    }

    //delEmptyTr(trRows);

    function resetTrRow(trlist) {
        trlist.each(function () {
            var $this = $(this);
            var tds = $this.children("td.field");
            var flag = true;
            tds.each(function () {
                var $td = $(this);
                if ($td.children('table').length > 0) {
                    findPrevEmptyChange($td.children('table'))
                }
                //if ($td.children("table").length == 0) {
                //    var hasEmptyCount = moveTdByIndex($td, $this, trRows, $td.index());
                //    if (hasEmptyCount > recheckCounts) {
                //        recheckCounts = hasEmptyCount;
                //    }
                //}
            });
        });
    }
}
//判断下面，或同行左边是否有足够位置  放得下
function checkEnEmpty(tar) {
    var res = {};
    res.isInsert = false;//是否可以放得下
    res.obj = null;//要进去的td
    res.tar = null;//如果有足够位置时，需要交换位置，或者不够位置要往下移动的td
    res.changes = [];//
    res.left = 0;
    res.right = 0;
    res.sumEmpty = 0;
    var tarPar = tar.distElePar;
    var prev = tarPar.prev(), next = tarPar.next();
    var prevFlag = true, nextFlag = true;
    while (prev.length > 0) {
        if (prev.children("table").length == 0) {
            res.left++;
            res.obj = prev;
        } else {
            prevFlag = false;
        }
        if (prevFlag == false) {
            break;
        }
        prev = prev.prev();
    }

    while (next.length > 0) {
        if (next.children("table").length == 0) {
            if (!res.obj) {
                res.obj = next;
            }
            res.right++;
        } else {
            nextFlag = false;
        }
        if (nextFlag == false) {
            break;
        }
        next = next.next();
    }

    res.sumEmpty = res.left + res.right;
    return res;
}

//移动时
var moveFieldEvent = {};
moveFieldEvent.one = function (sour, tar, pos, columns) {
    var tdIndex = getCurrentTdIndex(tar.distElePar);
    var type = "after";
    if (pos == "bottom") {
        type = "before";
    } else if (pos == "top") {
        type = "after";
    }
    if (tar.distElePar.children("table").length == 2) {//如果移动到一个已有元素的单元格
        var itemCols = tar.distEle.attr("data-colspan");
        if (itemCols == 1) {
            var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            sour.startElePar.addClass('placeholder');
            newTr.children("td").eq(tdIndex).removeClass("placeholder").append(tar.distEle);
            return true;
        } else if (itemCols == 2) {
            //判断下面，或同行左边是否有足够位置  放得下
            var colres = checkEnEmpty(tar);
            return false;
        } else if (itemCols == 3) {
            return false;
        } else if (itemCols == 4) {
            return false;
        }
    } else if (tar.distElePar.children("table").length == 1) {//如果移动到一个空的单元格
        sour.startElePar.addClass('placeholder');
        tar.distElePar.removeClass("placeholder").append(sour.startEle);
        return true;
    }
}
moveFieldEvent.two = function (sour, tar, pos, columns) {
    var tdIndex = getCurrentTdIndex(tar.distElePar);
    var type = "after";
    if (pos == "bottom") {
        type = "before";
    } else if (pos == "top") {
        type = "after";
    }
    if (tar.distElePar.children("table").length == 2) {//如果移动到一个已有元素的单元格
        var itemCols = tar.distEle.attr("data-colspan");
        var colobj = getSelectTdByIndex(tar.distEle, tar.distElePar.parent(), tar.distEleWrap.find(">tbody>tr"), tdIndex);
        //  console.log(colobj);
        if (itemCols == 1) {
            //var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            //var clone = tar.distElePar.parent().clone();
            //tar.distElePar.parent()[type](clone);
            if (tar.distElePar.parent().get(0) == sour.startElePar.parent().get(0)) {
                return false;
            } else {
                var startIndex = getCurrentTdIndex(tar.distElePar)
                var colres = checkEnEmpty(tar);
                if (colres.sumEmpty < 1) {//位置不够
                    var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
                    sour.startElePar.attr("colspan", 1).addClass("placeholder");
                    sour.startElePar.after($('<td class="col-sm-2 field ui-droppable placeholder ui-sortable"></td>'))
                    newTr.children("td").eq(tdIndex)
                        .append(sour.startEle)
                        .attr("colspan", 2)
                        .next().remove();
                    newTr.children("td").eq(tdIndex).removeClass("placeholder")
                } else {
                    if (colres.left > 0) {
                        sour.startElePar.attr("colspan", 1).addClass("placeholder");
                        sour.startElePar.after($('<td class="col-sm-2 field ui-droppable placeholder ui-sortable"></td>'));
                        if (colres.left == 2) {
                            colres.obj = colres.obj.next();
                        } else if (colres.left == 3) {
                            colres.obj = colres.obj.next().next();
                        }
                        colres.obj.attr("colspan", 2).removeClass("placeholder");
                        colres.obj.append(sour.startEle);
                        var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
                        newTr.children("td").eq(tdIndex).append(tar.distEle).removeClass("placeholder");
                        colres.obj.next().remove();
                    } else if (colres.right > 0) {
                        sour.startElePar.attr("colspan", 1).addClass("placeholder");
                        sour.startElePar.before($('<td class="col-sm-2 field ui-droppable placeholder ui-sortable"></td>'));
                        if (colres.right == 2) {
                            colres.obj = colres.obj.prev();
                        } else if (colres.right == 3) {
                            colres.obj = colres.obj.prev().prev();
                        }
                        colres.obj.attr("colspan", 2).removeClass("placeholder");
                        colres.obj.append(sour.startEle);
                        var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
                        newTr.children("td").eq(tdIndex).append(tar.distEle).removeClass("placeholder");
                        colres.obj.prev().remove();
                    }
                }
            }
            //tar.distEle.remove();
            //if (type == "after") {
            //    tar.distElePar.parent().after(sour.startElePar.parent());
            //} else {
            //    sour.startElePar.parent().after(tar.distElePar.parent());
            //}
            //sour.startElePar.append(sour.startEle);
            //return false;
        } else if (itemCols == 2) {
            var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            newTr.children("td").eq(0).remove();
            sour.startElePar.addClass('placeholder');
            newTr.children("td").eq(tdIndex).attr("colspan", 2).removeClass("placeholder").append(tar.distEle);

            return true;
        } else if (itemCols == 3) {
            return false;
        } else if (itemCols == 4) {
            return false;
        }
    } else if (tar.distElePar.children("table").length == 1) {//如果移动到一个空的单元格
        var colsObj = checkRowColSum(sour.startEle);
        var startIndex = getCurrentTdIndex(tar.distElePar)
        if (sour.startElePar.parent().get(0) != tar.distElePar.parent().get(0)) {
            if (colsObj.left.empty > 0) {
                tar.distElePar.prev().remove();
                tar.distElePar.attr("colspan", 2);
                sour.startElePar.attr("colspan", 1);
                var $ntd = $('<td class="col-sm-2 field placeholder" colspan="1"></td>');
                sour.startElePar.after($ntd);

                sour.startElePar.addClass('placeholder');
                tar.distElePar.removeClass("placeholder").append(sour.startEle);
            } else if (colsObj.right.empty > 0) {
                tar.distElePar.next().remove();
                tar.distElePar.attr("colspan", 2);
                sour.startElePar.attr("colspan", 1);
                var $ntd = $('<td class="col-sm-2 field placeholder" colspan="1"></td>');
                sour.startElePar.after($ntd);

                sour.startElePar.addClass('placeholder');
                tar.distElePar.removeClass("placeholder").append(sour.startEle);
            } else {
                //if (colsObj.left.count > 0) {
                //    var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
                //    newTr.children("td").eq(startIndex).append(sour.startEle).removeClass("placeholder").attr("colspan",2).prev().remove();
                //    sour.startElePar.addClass('placeholder');
                //} else if (colsObj.right.count > 0) {
                //    var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
                //    newTr.children("td").eq(startIndex).append(sour.startEle).removeClass("placeholder").attr("colspan", 2).next().remove();
                //    sour.startElePar.addClass('placeholder');
                //}
                return false;
            }
        } else {//如果是在同一行移动
            if (colsObj.left.empty > 0) {
                tar.distElePar.attr("colspan", 2);
            } else if (colsObj.right.empty > 0) {
                tar.distElePar.attr("colspan", 2);
            } else {
                return false;
            }
            sour.startElePar.attr("colspan", 1);
            sour.startElePar.addClass('placeholder');
            tar.distElePar.removeClass("placeholder").append(sour.startEle);
        }
        return true;
    }
}
moveFieldEvent.three = function (sour, tar, pos, columns) {
    var tdIndex = getCurrentTdIndex(tar.distElePar);
    var type = "after";
    if (pos == "bottom") {
        type = "before";
    } else if (pos == "top") {
        type = "after";
    }
    if (tar.distElePar.children("table").length == 2) {//如果移动到一个已有元素的单元格
        var itemCols = tar.distEle.attr("data-colspan");
        if (itemCols == 1) {
            //    if (tar.distElePar.parent().get(0) == sour.startElePar.parent().get(0)) {
            //        return false;
            //    } else {
            //        var startIndex = getCurrentTdIndex(tar.distElePar)
            //        var colres = checkEnEmpty(tar);
            //        if (colres.sumEmpty < 2) {//位置不够
            //            var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            //            sour.startElePar.attr("colspan", 3).addClass("placeholder");
            //            sour.startElePar.after($('<td class="col-sm-2 field ui-droppable placeholder ui-sortable"></td><td class="col-sm-2 field ui-droppable placeholder ui-sortable"></td>'));
            //            newTr.children("td").eq(tdIndex)
            //                .append(sour.startEle)
            //                .attr("colspan", 3)
            //                .next().remove();
            //            newTr.children("td").eq(tdIndex).next().remove();
            //            newTr.children("td").eq(tdIndex).removeClass("placeholder")

            //        } else {
            //            if (colres.left > 0) {
            //                sour.startElePar.attr("colspan", 1).addClass("placeholder");
            //                sour.startElePar.after($('<td class="col-sm-2 field ui-droppable placeholder ui-sortable"></td>'));
            //                if (colres.left == 2) {
            //                    colres.obj = colres.obj.next();
            //                } else if (colres.left == 3) {
            //                    colres.obj = colres.obj.next().next();
            //                }
            //                colres.obj.attr("colspan", 3).removeClass("placeholder");
            //                colres.obj.append(sour.startEle);
            //                var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            //                newTr.children("td").eq(tdIndex).append(tar.distEle).removeClass("placeholder");
            //                colres.obj.next().remove();
            //            } else if (colres.right > 0) {
            //                sour.startElePar.attr("colspan", 1).addClass("placeholder");
            //                sour.startElePar.before($('<td class="col-sm-2 field ui-droppable placeholder ui-sortable"></td>'));
            //                if (colres.right == 2) {
            //                    colres.obj = colres.obj.prev();
            //                } else if (colres.right == 3) {
            //                    colres.obj = colres.obj.prev().prev();
            //                }
            //                colres.obj.attr("colspan", 2).removeClass("placeholder");
            //                colres.obj.append(sour.startEle);
            //                var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            //                newTr.children("td").eq(tdIndex).append(tar.distEle).removeClass("placeholder");
            //                colres.obj.prev().remove();
            //            }

            //        }
            //    }
            return false;
        } else if (itemCols == 2) {
            return false;
        } else if (itemCols == 3) {
            var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            newTr.children("td").eq(0).remove();
            sour.startElePar.addClass('placeholder');
            newTr.children("td").eq(tdIndex).attr("colspan", itemCols).removeClass("placeholder").append(tar.distEle);

            return true;
        } else if (itemCols == 4) {
            return false;
        }
    } else if (tar.distElePar.children("table").length == 1) {//如果移动到一个空的单元格
        var colsObj = checkRowColSum(sour.startEle);
        if (colsObj.left.empty > 1) {
            tar.distElePar.prev().remove();
            tar.distElePar.prev().remove();
            tar.distElePar.attr("colspan", 3);
            sour.startElePar.attr("colspan", 1);
            var $ntd = $('<td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td>');
            sour.startElePar.after($ntd);
        } else if (colsObj.right.empty > 1) {
            tar.distElePar.next().remove();
            tar.distElePar.next().remove();
            tar.distElePar.attr("colspan", 3);
            sour.startElePar.attr("colspan", 1);
            var $ntd = $('<td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td>');
            sour.startElePar.after($ntd);
        } else {
            return false;
        }
        sour.startElePar.addClass('placeholder');
        tar.distElePar.removeClass("placeholder").append(sour.startEle);
        return true;
    }
}
moveFieldEvent.four = function (sour, tar, pos, columns) {
    var tdIndex = getCurrentTdIndex(tar.distElePar);
    var type = "after";
    if (pos == "bottom") {
        type = "before";
    } else if (pos == "top") {
        type = "after";
    }
    if (tar.distElePar.children("table").length == 2) {//如果移动到一个已有元素的单元格
        var itemCols = tar.distEle.attr("data-colspan");
        if (itemCols == 1) {
            return false;
        } else if (itemCols == 2) {
            return false;
        } else if (itemCols == 3) {
            return false;
        } else if (itemCols == 4) {
            var newTr = createNewTrAfter(tar.distElePar.parent(), 1, columns, type);
            newTr.children("td").eq(0).remove();
            sour.startElePar.addClass('placeholder');
            newTr.children("td").eq(tdIndex).attr("colspan", itemCols).removeClass("placeholder").append(tar.distEle);

            return true;
        }
    } else if (tar.distElePar.children("table").length == 1) {//如果移动到一个空的单元格
        var colsObj = checkRowColSum(sour.startEle);
        if (colsObj.left.empty > 2) {
            tar.distElePar.prev().remove();
            tar.distElePar.prev().remove();
            tar.distElePar.prev().remove();
            tar.distElePar.attr("colspan", 4);
            sour.startElePar.attr("colspan", 1);
            var $ntd = $('<td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td>');
            sour.startElePar.after($ntd);
        } else if (colsObj.right.empty > 2) {
            tar.distElePar.next().remove();
            tar.distElePar.next().remove();
            tar.distElePar.next().remove();
            tar.distElePar.attr("colspan", 4);
            sour.startElePar.attr("colspan", 1);
            var $ntd = $('<td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td>');
            sour.startElePar.after($ntd);
        } else {
            return false;
        }
        sour.startElePar.addClass('placeholder');
        tar.distElePar.removeClass("placeholder").append(sour.startEle);
        return true;
    }
}

function findPrevEmpty(obj, obj_index, obj_cols) {
    var objTd = obj.parent(), objTr = objTd.parent();
    var prev = objTr.prev();
    var res = null;
    var flag = true;
    obj_cols = obj_cols || 1;
    if (obj_cols == 1) {
        while (prev.length > 0) {
            prev.children("td").each(function () {
                var $this = $(this), tcol = $this.attr("colspan") || 1;
                var t_index = getCurrentTdIndex($this);
                if (tcol == obj_cols && obj_index == t_index && $this.children("table").length == 0) {
                    res = $this;
                    return false;
                } else if (obj_index == t_index && $this.children("table").length > 0) {
                    flag = false;
                } else if (tcol > 1 && (tcol * 1 + t_index - 1) >= obj_index && $this.children("table").length > 0) {
                    flag = false;
                }
            });
            if (flag == false) {
                break;
            }
            prev = prev.prev();
        }
        return res;
    } else if (obj_cols == 2) {
        while (prev.length > 0) {
            var count = 0;
            prev.children("td").each(function () {
                var $this = $(this), tcol = $this.attr("colspan") || 1;
                var t_index = getCurrentTdIndex($this);
                if (t_index >= obj_index && t_index < obj_cols * 1 + obj_index) {
                    if ($this.children("table").length == 0) {
                        count += tcol * 1;
                        flag = true;
                    } else {
                        if (count > tcol - 1) {
                            flag = false;
                        }
                    }
                    if (count >= obj_cols) {
                        res = $this.prev();
                        return false;
                    }
                    if (count <= 0) {
                        flag = false;
                        return false;
                    }
                } else {
                    flag = false;
                }
            });
            if (flag == false) {
                break;
            }
            prev = prev.prev();
        }
        return res;
    } else if (obj_cols == 3) {
        while (prev.length > 0) {
            var count = 0;
            prev.children("td").each(function () {
                var $this = $(this), tcol = $this.attr("colspan") || 1;
                var t_index = getCurrentTdIndex($this);
                if (t_index >= obj_index && t_index < obj_cols * 1 + obj_index) {
                    if ($this.children("table").length == 0) {
                        count += tcol * 1;
                        flag = true;
                    } else {
                        if (count > tcol - 1) {
                            flag = false;
                        }
                    }
                    if (count >= obj_cols) {
                        res = $this.prev().prev();
                        return false;
                    }
                    if (count <= 0) {
                        flag = false;
                        return false;
                    }
                } else {
                    flag = false;
                }
            });
            if (flag == false) {
                break;
            }
            prev = prev.prev();
        }
        return res;
    } else if (obj_cols == 4) {
        while (prev.length > 0) {
            var count = 0;
            prev.children("td").each(function () {
                var $this = $(this), tcol = $this.attr("colspan") || 1;
                var t_index = getCurrentTdIndex($this);
                if (t_index >= obj_index && t_index < obj_cols * 1 + obj_index) {
                    if ($this.children("table").length == 0) {
                        count += tcol * 1;
                        flag = true;
                    } else {
                        if (count > tcol - 1) {
                            flag = false;
                        }
                    }
                    if (count >= obj_cols) {
                        res = $this.prev().prev().prev();
                        return false;
                    }
                    if (count == 0) {
                        flag = false;
                        return false;
                    }
                } else {
                    flag = false;
                }
            });
            if (flag == false) {
                break;
            }
            prev = prev.prev();
        }
        return res;
    }
}

//移动后往上移动
function findPrevEmptyChange(obj) {
    var obj_cols = obj.attr("data-colspan");
    var obj_index = getCurrentTdIndex(obj.parent());
    //   console.log(obj_cols, obj_index);
    var emptyObj = findPrevEmpty(obj, obj_index, obj_cols)
    if (obj_cols == "1") {
        if (emptyObj) {
            exchange(emptyObj, obj.parent());
        }
    } else if (obj_cols == "2") {
        if (emptyObj) {
            //obj.parent()
            var objPar = obj.parent(); emptyObj.next().remove();
            emptyObj.append(obj).attr("colspan", 2).removeClass("placeholder");
            objPar.attr("colspan", 1).addClass("placeholder").after('<td class="col-sm-2 field placeholder" colspan="1"></td>');
        }
    } else if (obj_cols == "3") {
        if (emptyObj) {
            //obj.parent()
            var objPar = obj.parent(); emptyObj.next().remove(); emptyObj.next().remove();
            emptyObj.append(obj).attr("colspan", 3).removeClass("placeholder");
            objPar.attr("colspan", 1).addClass("placeholder").after('<td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td>');
        }
    } else if (obj_cols == "4") {
        if (emptyObj) {
            //obj.parent()
            var objPar = obj.parent(); emptyObj.next().remove(); emptyObj.next().remove(); emptyObj.next().remove();
            emptyObj.append(obj).attr("colspan", 4).removeClass("placeholder");
            objPar.attr("colspan", 1).addClass("placeholder").after('<td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td><td class="col-sm-2 field placeholder" colspan="1"></td>');
        }
    }
}

//两列时
var colspanTd = {};
colspanTd.two = function (obj, column, columns) {
    var par = obj.parent(), tr = par.parent();
    var ocolNum = checkRowColSum(obj);
    var flag = false;//是否已经插入到目标行
    if (column) {
        if (ocolNum.left.count >= column) {
            fieldInsertToTar("left", obj, ocolNum);
            flag = true;
        }
        if (flag == false && ocolNum.right.count >= column) {
            fieldInsertToTar("right", obj, ocolNum);
            flag = true;
        }

        if (flag == false && ocolNum.left.count < column && ocolNum.left.obj.length > 0) {
            var objIndex = obj.index();
            var newtr = createNewTrAfter(tr, 1, columns, "after");
            par.siblings("td").each(function () {
                if ($(this).children("table").length > 0) {
                    var $index = $(this).index();
                    $(this).addClass("placeholder");
                    newtr.children("td").eq($index).append($(this).children("table")).removeClass("placeholder");
                }
            });
            par.attr("colspan", column);
            for (var i = 0, len = column - 1; i < len; i++) {
                var ttd = tr.children("td").eq(0);
                if (ttd.get(0) != par.get(0)) {
                    ttd.remove();
                } else {
                    ttd = tr.children("td").eq(1);
                    ttd.remove();
                }
            }

            flag = true;
        }
        if (flag == false && ocolNum.right.count < column && ocolNum.right.obj.length > 0) {
            var objIndex = obj.index();
            var newtr = createNewTrAfter(tr, 1, columns, "after");
            par.siblings("td").each(function () {
                if ($(this).children("table").length > 0) {
                    var $index = $(this).index();
                    $(this).addClass("placeholder");
                    newtr.children("td").eq($index).append($(this).children("table")).removeClass("placeholder");
                }
            });
            par.attr("colspan", column);
            for (var i = 0, len = column - 1; i < len; i++) {
                var ttd = tr.children("td").eq(0);
                if (ttd.get(0) != par.get(0)) {
                    ttd.remove();
                } else {
                    ttd = tr.children("td").eq(1);
                    ttd.remove();
                }
            }

            flag = true;
        }
    } else if (column == 1) {//变回一列时
        par.attr("colspan", 1);
        var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
        var $ntd = $(ntd);
        tr.append($ntd);
    }
}

colspanTd.three = function (obj, column, columns, reColspan) {
    var par = obj.parent(), tr = par.parent(), objcols = reColspan;
    var ocolNum = checkRowColSum(obj);
    // console.log(ocolNum);
    //变成一列
    if (column == "1") {
        if (objcols == "2") {//两列变一列
            par.attr("colspan", 1);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        } else if (objcols == "3") {//三列变一列
            par.attr("colspan", 1);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td><td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        }
    } else if (column == "2") {//变成两列
        if (objcols == "1") {//一列变两列
            par.attr("colspan", 2);
            if (ocolNum.left.empty >= column - 1) {//如果左边右足够的空的列
                var prev_one = par.prev();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        } else if (objcols == "3") {//三列变两列
            par.attr("colspan", 2);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        }
    } else if (column == "3") {
        if (objcols == "1") {//一列变三列
            par.attr("colspan", 3);
            if (ocolNum.left.empty >= column - 1) {//如果左边右足够的空的列
                var prev_one = par.prev();
                var prev_two = par.prev().prev();
                prev_two.remove();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                var next_two = par.next().next();
                next_two.remove();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        } else if (objcols == "2") {//两列变三列
            par.attr("colspan", 3);
            if (ocolNum.left.empty >= column) {//如果左边右足够的空的列
                var prev_one = par.prev();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        }
    }
}

colspanTd.four = function (obj, column, columns, reColspan) {
    var par = obj.parent(), tr = par.parent(), objcols = reColspan;
    var ocolNum = checkRowColSum(obj);
    //  console.log(ocolNum);
    if (column == "1") {
        if (objcols == "2") {//两列变一列
            par.attr("colspan", 1);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        } else if (objcols == "3") {//三列变一列
            par.attr("colspan", 1);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td><td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        }
        else if (objcols == "4") {//三列变一列
            par.attr("colspan", 1);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td><td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td><td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        }
    } else if (column == "2") {
        if (objcols == "1") {//一列变两列
            par.attr("colspan", 2);
            if (ocolNum.left.empty >= column - 1) {//如果左边右足够的空的列
                var prev_one = par.prev();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        } else if (objcols == "3") {//三列变两列
            par.attr("colspan", 2);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        } else if (objcols == "4") {//4列变2列
            par.attr("colspan", 1);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td><td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        }
    } else if (column == "3") {
        if (objcols == "1") {//一列变三列
            par.attr("colspan", 3);
            if (ocolNum.left.empty >= column - 1) {//如果左边右足够的空的列
                var prev_one = par.prev();
                var prev_two = par.prev().prev();
                prev_two.remove();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                var next_two = par.next().next();
                next_two.remove();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        } else if (objcols == "2") {//两列变三列
            par.attr("colspan", 3);
            if (ocolNum.left.empty >= column) {//如果左边右足够的空的列
                var prev_one = par.prev();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        } else if (objcols == "4") {//4列变三列
            par.attr("colspan", 3);
            var ntd = '<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>';
            var $ntd = $(ntd);
            par.after($ntd);
        }
    } else if (column == "4") {
        if (objcols == "1") {//一列变4列
            par.attr("colspan", 4);
            if (ocolNum.left.empty >= column - 1) {//如果左边右足够的空的列
                var prev_one = par.prev();
                var prev_two = par.prev().prev();
                var prev_th = par.prev().prev().prev();
                prev_th.remove();
                prev_two.remove();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                var next_two = par.next().next();
                var next_th = par.next().next().next();
                next_th.remove();
                next_two.remove();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        } else if (objcols == "2") {//两列变4列
            par.attr("colspan", 4);
            if (ocolNum.left.empty >= column) {//如果左边右足够的空的列
                var prev_one = par.prev();
                var prev_two = par.prev().prev();
                prev_two.remove();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                var next_two = par.next().next();
                next_two.remove();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        } else if (objcols == "3") {//3列变4列
            par.attr("colspan", 4);
            if (ocolNum.left.empty >= column) {//如果左边右足够的空的列
                var prev_one = par.prev();
                prev_one.remove();
            } else if (ocolNum.right.empty >= column - 1) {
                var next_one = par.next();
                next_one.remove();
            } else {//如果都不够,则把其他的列往下挤
                moveAfterNewTr(obj, tr, columns, column, par);
            }
        }
    }
}

//新建TR且往下移动
function moveAfterNewTr(obj, tr, columns, column, par) {
    var objIndex = obj.index();
    var newtr = createNewTrAfter(tr, 1, columns, "after");
    par.siblings("td").each(function () {
        if ($(this).get(0) == par.get(0)) { return true; }
        if ($(this).children("table").length > 0) {
            var $index = $(this).index();
            $(this).addClass("placeholder");
            var tcolspan = $(this).attr("colspan");
            if (tcolspan > 1) {
                var newtd = newtr.children("td").eq($index);
                for (var i = 0, len = tcolspan - 1; i < len; i++) {
                    newtd.next().remove();
                }
                newtd.attr("colspan", tcolspan).append($(this).children("table")).removeClass("placeholder");
            } else {
                newtr.children("td").eq($index).append($(this).children("table")).removeClass("placeholder");
            }
        }
    });
    par.attr("colspan", column);
    var newtr2 = createNewTrAfter(tr, 1, columns, "after");
    var sourTd = newtr2.children("td").eq(par.index());
    for (var i = 0, len = column - 1; i < len; i++) {
        if (sourTd.prev().length > 0) {
            sourTd.prev().remove();
        } else if (sourTd.next().length) {
            sourTd.next().remove();
        }
    }
    sourTd.append(par.children("table")).attr("colspan", column).removeClass("placeholder");
    tr.remove();
    //for (var i = 0, len = column - 1; i < len; i++) {
    //    var ttd = tr.children("td").eq(0);

    //    if (ttd.get(0) != par.get(0)) {
    //    } else {
    //        ttd = tr.children("td").eq(1);

    //    }
    //    if (ttd.attr("colspan") > 1) {
    //        var count = ttd.attr("colspan") - 1;
    //        i += count;
    //        continue;
    //    }
    //    ttd.remove();
    //}
}

//
function fieldColspanReset(obj, cols, columns, type, reColspan) {
    var objcols = reColspan;
    if (objcols == cols) {
        return false;
    }
    var column = cols;
    type = type || "2";
    switch (type * 1) {
        case 2:
            colspanTd.two(obj, column, columns, reColspan);
            break;
        case 3:
            colspanTd.three(obj, column, columns, reColspan);
            break;
        case 4:
            colspanTd.four(obj, column, columns, reColspan);
            break;
    }
}
function createTrInBefore(tr) {
    var cloneTr = tr.clone();
    cloneTr.children("td").empty();
    tr.after(cloneTr);
    return cloneTr;
}

//有足够的列时插入
function fieldInsertToTar(dir, obj, ocolNum) {
    switch (dir) {
        case "left":
            fieldInsertToLeft(obj, ocolNum);
            break;
        case "right":
            fieldInsertToRight(obj, ocolNum);
            break;
    }
}

//获取列数
function getTargetColnums(obj, val) {
    var colsDefaults = [{ text: '一列', value: 1 }, { text: '两列', value: 2 }, { text: '三列', value: 3 }, { text: '四列', value: 4 }];
    var maxCol = $(obj).parents(".section").attr("data-columns");
    if (!maxCol) maxCol = 4;
    var colsItems = [];
    colsItems = $.grep(colsDefaults, function (obj, key) {
        return obj.value <= maxCol;
    });
    return colsItems;
}
function setTrueOrFalse(str) {
    if (str == "true") {
        return true;
    }
    if (str == "false") {
        return false;
    }
}

//--------------------------------------------------------------------------------------------------------------------------------
//create.html表单   导航链接
function renderFormNav(_form, container) {
    var navDatas = _form.NavGroups;
    console.log(navDatas);
    if (!navDatas || navDatas.length == 0) { return false; }
    var _html = [];
    _html.push('<dl class="list-group col-sm-2" id="formNav">');
    for (var i = 0, len = navDatas.length; i < len; i++) {
        var item = navDatas[i];
        var timestrap = new Date() * 1 + new Date().toString(16);
        _html.push('<dt class="list-group-item" ><span class="glyphicon glyphicon-chevron-down collapse-title" data-target="#tab_' + timestrap + '" aria-expanded="true" data-toggle="collapse"></span>' + item.Label + '</dt>');
        _html.push('<dd id="tab_' + timestrap + '" class="panel-collapse collapse in" aria-expanded="true">');
        for (var j = 0, jlen = item.NavItems.length; j < jlen; j++) {
            var jitem = item.NavItems[j];
            //console.log(jitem);
            if ((jitem.Url == "" && jitem.RelationshipName && jitem.RelationshipName.length > 0) || !jitem.Url) {
                _html.push('<div class="list-group-item"><span class="' + jitem.Icon + '"></span><a data-type="1" class="form-navlink" target="_blank" data-href="' + ORG_SERVERURL + '/entity/rendergridview?relationshipname=' + jitem.RelationshipName + '&entityid=' + jitem.Id + '">' + jitem.Label + '</a></div>');
            } else if (!jitem.RelationshipName || (jitem.RelationshipName == "" && jitem.Url && jitem.Url.length > 0)) {
                _html.push('<div class="list-group-item"><span class="' + jitem.Icon + '"></span><a data-type="0" target="_blank" href="' + jitem.Url + '" class="form-navlink">' + jitem.Label + '</a></div>');
            }
        }
        _html.push('</dd>')
    }
    _html.push('</dl>');
    var formNav = $(_html.join(""));
    container.append(formNav);
    return formNav;
}

//
function changeRelationState(obj) {
    var fieldRelationRecord = $("#field-RelationRecordBox");
    if ($(obj).prop("checked")) {
        fieldRelationRecord.show();
        var selected = $('.selected');
        var entityid = '';
        var _htmlArr = [];
        console.log('attributes', attributes)
        $.each(attributes, function (i, n) {
            if (n.name == selected.attr('data-name')) {
                entityid = n.referencedentityid;
                return true;
            }
            var type = n.attributetypename;
            if (type == "lookup" || type == "owner" || type == "customer") {
                _htmlArr.push('<option value="' + n.entityid + '" data-referencedentityid="' + n.referencedentityid + '" data-attributename="' + n.name + '">' + n.localizedname + '(' + n.entitylocalizedname + ')' + '</option>');
            }
        });

        $('#field-RelationRecordSourc').empty().html(_htmlArr.join(''));
        var attributename = selected.attr('data-dependentattributename');
        if (attributename && attributename != "") {
            $('#field-RelationRecordSourc').children('option[data-attributename="' + attributename + '"]').prop('selected', true);
        }

        $('#field-RelationRecordSourc').off('change').on('change', function () {
            var referencedentityid = $(this).children('option:selected').attr('data-referencedentityid');
            Xms.Web.GetJson('/api/schema/relationship/GetReferencing/' + entityid + '?referencedEntityId=' + referencedentityid, null, function (data) {
                var html = [];
                //console.log('customize/EntityRelations?referencingEntityId=' + entityid, data)
                $(data.content).each(function (ii, nn) {
                    var attrName = nn.referencedattributename;
                    //attributes.forEach(function (j) {
                    //    if (j.referencedentityid == nn.referencedentityid) {
                    //        attrName = j.name;
                    //    }
                    //});
                    html.push('<option value="' + nn.referencedentityid + '" data-relationshipname="' + nn.name + '" data-attributename="' + attrName + '">' + nn.referencingattributelocalizedname + '(' + nn.referencedentitylocalizedname + ')' + '(' + nn.referencingentitylocalizedname + ')' + '</option>');
                });
                $("#field-RelationRecord").html(html.join(""));
                if (selected.attr('data-filterrelationshipname') != "") {
                    console.log($('#field-RelationRecord option[data-relationshipname="' + selected.attr('data-filterrelationshipname') + '"]'))
                    $('#field-RelationRecord option[data-relationshipname="' + selected.attr('data-filterrelationshipname') + '"]').attr('selected', 'selected');
                }
                if (selected.attr('data-allowfilteroff') == "true") {
                    $('#field-AllowFilterOff').prop("checked", true);
                } else {
                    $('#field-AllowFilterOff').prop("checked", false);
                }
            });
        }).trigger('change');
    } else {
        fieldRelationRecord.hide();
    }
}

//lookup类型字段加载  实体
function getRelationEntity(entityid, context) {
    if (typeof entityid === "undefined") { return false; }
    var dfd = $.Deferred();
    var $context = $(context);
    Xms.Web.GetJson('/api/schema/relationship/GetReferencing/' + entityid, null, function (data) {
        if (!data || data.content.length == 0) return;
        var _html = [];
        $(data.content).each(function (i, n) {
            _html.push('<option data-relationship="' + n.name + '" data-referencingattributelocalizedname="' + n.referencingattributelocalizedname + '" value="' + n.referencedentityid + '">' + n.referencingattributelocalizedname + '(' + n.referencedentitylocalizedname + ')' + '</option>');
        });
        $context.html(_html.join(""));
        dfd.resolve($context);
    });
    return dfd.promise();
}

function renderSelectByAttrs(context, attrs, isrequire) {
    var $context = $(context);
    if (typeof attrs === "undefined") return false;
    if (attrs.length == 0) return "";
    var res = [];
    console.log(attrs);
    res.push('<option value=""></option>');
    $.each(attrs, function (key, obj) {
        res.push('<option value="" data-name="' + obj.name + '" data-referencedattributeid="' + obj.referencedattributeid + '" data-referencedentityid="' + obj.referencedentityid + '" data-referencingattributename="' + obj.referencingattributename + '"  data-referencedentityname ="' + obj.referencedentityname + '">' + obj.referencedentitylocalizedname + ' (' + obj.referencingattributelocalizedname + ') </option>')
    });
    $context.html(res.join(""));
}

function changeGetEntityName(obj, callback, target, filter, isrequire) {
    var selectObj = $(obj).find(">option:selected");
    var selector = target || $("#setLabel-AttributeName");
    if (!selectObj.attr('data-name') || selectObj.attr('data-name') == "") { selector.empty(); return false; }
    var entityid = selectObj.attr("data-referencedentityid");

    Xms.Schema.GetAttributes({ getall: true, entityid: entityid }, function (data) {
        var html = [];
        var repdatas = data;
        if (filter && typeof filter === "function") {
            repdatas = filter(repdatas);
        }
        if (!isrequire) {
            // html.push('<option value="">请选择</option>');
        }
        $(repdatas).each(function (ii, nn) {
            html.push('<option value="' + nn.attributeid + '" data-type="' + nn.attributetypename + '" data-name="' + nn.name + '" >' + nn.localizedname + '</option>');
        });
        selector.html(html.join(''));
        callback && callback(selector);
    });
}

function insertLabel() {
    var selected = $('#formBody').find('.selected');
    if (selected.length <= 0) {
        selected = $('#formBody').find('.tab:first');
    }
    if (selected.is('.section') || selected.is('.tab')) {
    } else if (selected.is('.cell')) {
        selected = selected.parents('.section:first');
    }
    if (selected.find('.placeholder').length > 0) {
    }
    else {
        //var html = [];
        //html.push('<tr>');
        //html.push('<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>')
        //html.push('</tr>');
        var count = selected.find("div.section:first").attr("data-columns");
        createNewTrAfter(selected.find('table:first > tbody>tr:last'), 1, count, "after");
        //selected.find('table:first > tbody').append($(html.join('')));
    }
    var ifhtml = [];
    var ifDom = $('<table class="table cell" data-labelentityname="" data-attributename="" data-sourceattributename="" data-colspan="1"></table>');
    ifhtml.push('<tbody>');
    ifhtml.push('<tr style="height:100%;"><th class="col-sm-3">标签</th><td class="col-sm-4">标签</td></tr>');
    ifhtml.push('</tbody>');
    ifDom.html(ifhtml.join(""));
    var selTd = selected.find('.placeholder:first');
    var randGuid = 'label_' + Xms.Utility.Guid.NewGuid().ToString('N');
    ifDom.attr('data-label', '标签');
    ifDom.attr('data-name', randGuid);
    ifDom.attr('data-labelentityname', '');
    ifDom.attr('data-controltype', 'label');
    ifDom.attr('data-type', 'label');
    ifDom.attr('data-isshowlabel', 'true');
    ifDom.attr('data-isvisible', 'true');
    selTd.append(ifDom).removeClass('placeholder');
    selTd.addClass('labelbox');
    initFieldEvent();
}

function editLabel() {
    var selected = $('.selected');
    if (selected.attr('data-controltype') == 'subGrid') {
        editSubGrid();
        return false;
    }
    else if (selected.attr('data-controltype') == 'iFrame') {
        editIframe();
        return false;
    }
    var target = $('#setLabelModal');

    //设置字段的值
    target.find('#setLabel-name').val(selected.attr('data-name'));
    target.find('#setLabel-label').val(selected.attr('data-label'));
    if (selected.attr('data-isshowlabel') == "true") {
        target.find('#setLabel-isshowlabel').prop("checked", true);
    } else {
        target.find('#setLabel-isshowlabel').prop("checked", false);
    }
    if (selected.attr('data-isvisible') == "true") {
        target.find('#setLabel-isvisible').prop("checked", true);
    } else {
        target.find('#setLabel-isvisible').prop("checked", false);
    }
    if (selected.attr('data-isreadonly') == "true") {
        target.find('#setLabel-isreadonly').prop("checked", true);
    } else {
        target.find('#setLabel-isreadonly').prop("checked", false);
    }
    target.find('#setLabel-colspan').val(selected.attr('data-colspan'));
    //设置扩展属性
    Xms.Web.GetJson('/api/schema/relationship/GetReferencing/' + entityid, null, function (data) {
        renderSelectByAttrs($("#setLabel-EntityName"), data.content);
        if (selected.attr('data-labelentityname') != '') {
            console.log('option[data-referencedentityname="' + selected.attr('data-labelentityname') + '"][data-referencingattributename="' + selected.attr('data-sourceattributename') + '"]')
            $("#setLabel-EntityName").find('option[data-referencedentityname="' + selected.attr('data-labelentityname') + '"][data-referencingattributename="' + selected.attr('data-sourceattributename') + '"]').attr('selected', 'selected');
            changeGetEntityName($("#setLabel-EntityName"), function (sel) {
                //设置字段已有的值
                sel.find('>option[data-name="' + selected.attr('data-attributename') + '"]').attr('selected', 'selected');
            });
        } else {
            changeGetEntityName($("#setLabel-EntityName"));//自动加载对应的实体
        }
    });

    /***设置列数***/
    var colsItems = getTargetColnums(selected);
    //$("#setLabel-colspan").attr("data-value", selected.attr('data-colspan')).val(selected.attr('data-colspan'));
    $('#setLabel-colspan').removeAttr("data-picklistinit");

    $('#setLabel-colspan').on("picklist.getTarget", function (e, obj) {
        var test = null;
        selerin = obj.target.get(0).id;
    });
    if (selerin) {
        if ($("#" + selerin).length > 0) {
            $("#" + selerin).remove();
            selerin = null;
        }
    }
    $('#setLabel-colspan').picklist({
        required: true,
        items: colsItems
    });
    $('#setLabel-colspan').attr("data-value", selected.attr('data-colspan'))
        .next('select').find("option[value='" + selected.attr('data-colspan') + "']").prop("selected", true);
    /***设置列数     end***/

    //设置是否显示
    $("#setLabel-IsVisible").prop("checked", setTrueOrFalse(selected.attr("data-isvisible")));
    //设置是否显示标题
    $("#setLabel-isshowlabel").prop("checked", setTrueOrFalse(selected.attr("data-isshowlabel")));
    //target.find('#setLabel-rowspan').val(selected.attr('data-rowspan'));

    //event
    var scriptHtml = [];
    var scriptselect = [];
    for (var i = 0; i < scriptlist.length; i++) {
        // if (scriptlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
        scriptHtml.push('<tr onclick="chooseRowInfo(this)" class="scriptrow" data-id="' + scriptlist[i].Id + '"><td data-value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</td><td>' + scriptlist[i].Info + '</td><td></td></tr>');
        scriptselect.push('<option value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</option>');
        // }
    }
    target.find('#fieldScript').html(scriptHtml.join(''));
    var eventHtml = [];
    target.find('#fieldEvent').html('');
    for (var i = 0; i < eventlist.length; i++) {
        if (eventlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
            target.find('#fieldEvent').append('<tr onclick="chooseRowInfo(this)" data-id="' + eventlist[i].eventid + '"><td><select class="scriptlist">' + scriptselect.join('') + '</select></td><td><select><option value="onchange">onchange</option></select></td><td><input type="text" value="' + eventlist[i].JsAction + '" /></td></tr>');
            target.find('#fieldEvent').find('tr:last').find('td:eq(0)').find('option[value="' + eventlist[i].JsLibrary + '"]').attr('selected', "selected");
            target.find('#fieldEvent').find('tr:last').find('td:eq(1)').find('option[value="' + eventlist[i].Name + '"]').attr('selected', "selected");
        }
    }

    target.modal({
        keyboard: true
    });
}

function saveLabel() {
    var target = $('#setLabelModal');
    var selected = $('.selected');
    var reColspan = selected.attr('data-colspan');//没变列数之前

    //***********save param
    selected.attr('data-name', target.find('#setLabel-name').val());
    selected.attr('data-label', target.find('#setLabel-label').val());
    selected.attr('data-isshowlabel', target.find('#setLabel-isshowlabel').prop('checked'));
    selected.attr('data-isvisible', target.find('#setLabel-isvisible').prop('checked'));
    selected.attr('data-isreadonly', target.find('#setLabel-isreadonly').prop('checked'));
    selected.attr('data-colspan', target.find('#setLabel-colspan').val());
    selected.attr('data-rowspan', target.find('#setLabel-rowspan').val());

    selected.find('th').text(target.find('#setLabel-label').val());
    selected.find('td').text(target.find('#setLabel-label').val());
    if (target.find('#setLabel-isshowlabel').prop('checked') == false) {
        selected.find('th').addClass("disable-text");
    } else {
        selected.find('th').removeClass("disable-text");
    }

    selected.attr('data-labelentityname', $("#setLabel-EntityName>option:selected").attr("data-referencedentityname"));
    selected.attr('data-attributename', $("#setLabel-AttributeName>option:selected").attr("data-name"));
    selected.attr('data-sourceattributename', $("#setLabel-EntityName>option:selected").attr("data-referencingattributename"));
    selected.attr('data-SourceAttributeType', $("#setLabel-AttributeName>option:selected").attr("data-type"));
    //***********save event
    removeSriptEvent(selected.attr('data-name'));
    target.find('#fieldEvent').find('tr').each(function (i, n) {
        var eventitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(1)').find('option:selected').val(),
            'JsAction': $(n).find('td:eq(2)').find('input').val(),
            'JsLibrary': $(n).find('td:eq(0)').find('option:selected').val(),
            'eventid': Xms.Utility.Guid.NewGuid().ToString()
        };
        eventlist.push(eventitem);
    });
    target.find('#fieldScript').find('tr').each(function (i, n) {
        var scriptitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(0)').text(),
            'Info': $(n).find('td:eq(1)').text(),
            'Id': $(n).attr('data-id')
        };
        var flag = true;
        if (scriptlist.length > 0) {
            $.each(scriptlist, function (key, item) {
                if (item.Id == scriptitem.Id) {
                    flag = false;
                    return false;
                }
            });
        }
        if (flag == true) {
            scriptlist.push(scriptitem);
        }
    });
    var columnsMax = selected.parents(".section").attr("data-columns");
    fieldColspanReset(selected, $('#setLabel-colspan').val(), columnsMax, columnsMax, reColspan);
    var trRows = selected.parents(".section").find(">table>tbody>tr");
    resetTableVis(trRows);
    delEmptyTr(trRows)
    initFieldEvent();
    target.modal('hide');
}

function insertFreeText() {
    var selected = $('#formBody').find('.selected');
    if (selected.length <= 0) {
        selected = $('#formBody').find('.tab:first');
    }
    if (selected.is('.section') || selected.is('.tab')) {
    } else if (selected.is('.cell')) {
        selected = selected.parents('.section:first');
    }
    if (selected.find('.placeholder').length > 0) {
    }
    else {
        //var html = [];
        //html.push('<tr>');
        //html.push('<td class="col-sm-2 field placeholder ui-droppable ui-sortable"></td>')
        //html.push('</tr>');
        var count = selected.find("div.section:first").attr("data-columns");
        createNewTrAfter(selected.find('table:first > tbody>tr:last'), 1, count, "after");
        //selected.find('table:first > tbody').append($(html.join('')));
    }
    var ifhtml = [];
    var ifDom = $('<table class="table cell" data-labelentityname="" data-attributename="" data-sourceattributename="" data-colspan="1"></table>');
    ifhtml.push('<tbody>');
    ifhtml.push('<tr style="height:100%;"><th class="col-sm-3">自定义内容</th><td class="col-sm-4">自定义内容</td></tr>');
    ifhtml.push('</tbody>');
    ifDom.html(ifhtml.join(""));
    var selTd = selected.find('.placeholder:first');
    var randGuid = 'freetext_' + Xms.Utility.Guid.NewGuid().ToString('N');
    ifDom.attr('data-label', '自定义内容');
    ifDom.attr('data-name', randGuid);
    ifDom.attr('data-labelentityname', '');
    ifDom.attr('data-controltype', 'freetext');
    ifDom.attr('data-type', 'freetext');
    ifDom.attr('data-isshowlabel', 'true');
    ifDom.attr('data-isvisible', 'true');
    selTd.append(ifDom).removeClass('placeholder');
    selTd.addClass('freetextbox');
    initFieldEvent();
}

function editFreeText() {
    var selected = $('.selected');
    if (selected.attr('data-controltype') == 'subGrid') {
        editSubGrid();
        return false;
    }
    else if (selected.attr('data-controltype') == 'iFrame') {
        editIframe();
        return false;
    }
    var target = $('#setfreetextModal');

    //设置字段的值
    target.find('#setfreetext-name').val(selected.attr('data-name'));
    target.find('#setfreetext-freetext').val(selected.attr('data-label'));
    if (selected.attr('data-isshowlabel') == "true") {
        target.find('#setfreetext-isshowfreetext').prop("checked", true);
    } else {
        target.find('#setfreetext-isshowfreetext').prop("checked", false);
    }
    if (selected.attr('data-isvisible') == "true") {
        target.find('#setfreetext-isvisible').prop("checked", true);
    } else {
        target.find('#setfreetext-isvisible').prop("checked", false);
    }
    if (selected.attr('data-isreadonly') == "true") {
        target.find('#setfreetext-isreadonly').prop("checked", true);
    } else {
        target.find('#setfreetext-isreadonly').prop("checked", false);
    }
    target.find('#setfreetext-colspan').val(selected.attr('data-colspan'));

    var ue = loadFreeText();
    var contents = selected.attr('data-paramcontent') || '';
    if (contents && contents !== '') {
        contents = decodeURIComponent(contents);
    }
    ue.setContent(contents);

    //设置扩展属性
    Xms.Web.GetJson('/api/schema/relationship/GetReferencing/' + entityid, null, function (data) {
        renderSelectByAttrs($("#setfreetext-EntityName"), data.content);
        if (selected.attr('data-freetextentityname') != '') {
            console.log('option[data-referencedentityname="' + selected.attr('data-freetextentityname') + '"][data-referencingattributename="' + selected.attr('data-sourceattributename') + '"]')
            $("#setfreetext-EntityName").find('option[data-referencedentityname="' + selected.attr('data-freetextentityname') + '"][data-referencingattributename="' + selected.attr('data-sourceattributename') + '"]').attr('selected', 'selected');
            changeGetEntityName($("#setfreetext-EntityName"), function (sel) {
                //设置字段已有的值
                sel.find('>option[data-name="' + selected.attr('data-attributename') + '"]').attr('selected', 'selected');
            });
        } else {
            changeGetEntityName($("#setfreetext-EntityName"));//自动加载对应的实体
        }
    });

    /***设置列数***/
    var colsItems = getTargetColnums(selected);
    //$("#setfreetext-colspan").attr("data-value", selected.attr('data-colspan')).val(selected.attr('data-colspan'));
    $('#setfreetext-colspan').removeAttr("data-picklistinit");

    $('#setfreetext-colspan').on("picklist.getTarget", function (e, obj) {
        var test = null;
        selerin = obj.target.get(0).id;
    });
    if (selerin) {
        if ($("#" + selerin).length > 0) {
            $("#" + selerin).remove();
            selerin = null;
        }
    }
    $('#setfreetext-colspan').picklist({
        required: true,
        items: colsItems
    });
    $('#setfreetext-colspan').attr("data-value", selected.attr('data-colspan'))
        .next('select').find("option[value='" + selected.attr('data-colspan') + "']").prop("selected", true);
    /***设置列数     end***/

    //设置是否显示
    $("#setfreetext-IsVisible").prop("checked", setTrueOrFalse(selected.attr("data-isvisible")));
    //设置是否显示标题
    $("#setfreetext-isshowfreetext").prop("checked", setTrueOrFalse(selected.attr("data-isshowfreetext")));
    //target.find('#setfreetext-rowspan').val(selected.attr('data-rowspan'));

    //event
    var scriptHtml = [];
    var scriptselect = [];
    for (var i = 0; i < scriptlist.length; i++) {
        // if (scriptlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
        scriptHtml.push('<tr onclick="chooseRowInfo(this)" class="scriptrow" data-id="' + scriptlist[i].Id + '"><td data-value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</td><td>' + scriptlist[i].Info + '</td><td></td></tr>');
        scriptselect.push('<option value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</option>');
        // }
    }
    target.find('#fieldScript').html(scriptHtml.join(''));
    var eventHtml = [];
    target.find('#fieldEvent').html('');
    for (var i = 0; i < eventlist.length; i++) {
        if (eventlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
            target.find('#fieldEvent').append('<tr onclick="chooseRowInfo(this)" data-id="' + eventlist[i].eventid + '"><td><select class="scriptlist">' + scriptselect.join('') + '</select></td><td><select><option value="onchange">onchange</option></select></td><td><input type="text" value="' + eventlist[i].JsAction + '" /></td></tr>');
            target.find('#fieldEvent').find('tr:last').find('td:eq(0)').find('option[value="' + eventlist[i].JsLibrary + '"]').attr('selected', "selected");
            target.find('#fieldEvent').find('tr:last').find('td:eq(1)').find('option[value="' + eventlist[i].Name + '"]').attr('selected', "selected");
        }
    }

    target.modal({
        keyboard: true
    });
}

function savefreetext() {
    var target = $('#setfreetextModal');
    var selected = $('.selected');
    var reColspan = selected.attr('data-colspan');//没变列数之前

    //***********save param
    selected.attr('data-name', target.find('#setfreetext-name').val());
    selected.attr('data-label', target.find('#setfreetext-freetext').val());
    selected.attr('data-isshowlabel', target.find('#setfreetext-isshowfreetext').prop('checked'));
    selected.attr('data-isvisible', target.find('#setfreetext-isvisible').prop('checked'));
    selected.attr('data-isreadonly', target.find('#setfreetext-isreadonly').prop('checked'));
    selected.attr('data-colspan', target.find('#setfreetext-colspan').val());
    selected.attr('data-rowspan', target.find('#setfreetext-rowspan').val());

    selected.find('th').text(target.find('#setfreetext-freetext').val());
    selected.find('td').text(target.find('#setfreetext-freetext').val());
    if (target.find('#setfreetext-isshowfreetext').prop('checked') == false) {
        selected.find('th').addClass("disable-text");
    } else {
        selected.find('th').removeClass("disable-text");
    }

    selected.attr('data-freetextentityname', $("#setfreetext-EntityName>option:selected").attr("data-referencedentityname"));
    selected.attr('data-attributename', $("#setfreetext-AttributeName>option:selected").attr("data-name"));
    selected.attr('data-sourceattributename', $("#setfreetext-EntityName>option:selected").attr("data-referencingattributename"));
    selected.attr('data-SourceAttributeType', $("#setfreetext-AttributeName>option:selected").attr("data-type"));
    var contents = loadFreeText().getContent();
    if (contents && contents !== '') {
        contents = encodeURIComponent(contents);
    }
    selected.attr('data-paramcontent', contents);
    //***********save event
    removeSriptEvent(selected.attr('data-name'));
    target.find('#fieldEvent').find('tr').each(function (i, n) {
        var eventitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(1)').find('option:selected').val(),
            'JsAction': $(n).find('td:eq(2)').find('input').val(),
            'JsLibrary': $(n).find('td:eq(0)').find('option:selected').val(),
            'eventid': Xms.Utility.Guid.NewGuid().ToString()
        };
        eventlist.push(eventitem);
    });
    target.find('#fieldScript').find('tr').each(function (i, n) {
        var scriptitem = {
            'Attribute': selected.attr('data-name'),
            'Name': $(n).find('td:eq(0)').text(),
            'Info': $(n).find('td:eq(1)').text(),
            'Id': $(n).attr('data-id')
        };
        var flag = true;
        if (scriptlist.length > 0) {
            $.each(scriptlist, function (key, item) {
                if (item.Id == scriptitem.Id) {
                    flag = false;
                    return false;
                }
            });
        }
        if (flag == true) {
            scriptlist.push(scriptitem);
        }
    });
    var columnsMax = selected.parents(".section").attr("data-columns");
    fieldColspanReset(selected, $('#setfreetext-colspan').val(), columnsMax, columnsMax, reColspan);
    var trRows = selected.parents(".section").find(">table>tbody>tr");
    resetTableVis(trRows);
    delEmptyTr(trRows)
    initFieldEvent();
    target.modal('hide');
}

function loadFreeText() {
    var $serfreetextContextHtml = $('#serfreetextContextHtml');
    var width = $serfreetextContextHtml.parent().width();

    if (!$serfreetextContextHtml.data().ue) {
        var ue = UE.getEditor('serfreetextContextHtml', {
            toolbars: [
                ['bold', 'italic', 'underline', 'fontborder', 'strikethrough', '|', 'forecolor', 'backcolor', 'insertorderedlist', 'insertunorderedlist', 'selectall', 'cleardoc', '|',
                    'rowspacingtop', 'rowspacingbottom', 'lineheight', '|',]
            ],
            initialFrameHeight: 250,
            initialFrameWidth: width - 50,
            autoHeightEnabled: false
        });

        ue.addListener('ready', function (editor) {
        });
        ue.addListener("contentChange", function () {
        });
        $serfreetextContextHtml.data().ue = ue;
    } else {
        var ue = $serfreetextContextHtml.data().ue;
    }

    return ue;
}

function getEntityTypeBAttrs(entName) {
    if (!attributes || attributes.length == 0 || !entName) { return false; }
    var res = [];
    res = $.grep(attributes, function (obj, key) {
        return obj.name.toLowerCase() == entName.toLowerCase();
    });
    return res;
}

function editEvents() {
    var target = $('#setEventsModal');
    var scriptHtml = [];
    var scriptselect = [];
    for (var i = 0; i < scriptlist.length; i++) {
        // if (scriptlist[i].Attribute.toLowerCase() == selected.attr('data-name').toLowerCase()) {
        scriptHtml.push('<tr onclick="chooseRowInfo(this)" class="scriptrow" data-attributename="' + scriptlist[i].Attribute + '" data-id="' + scriptlist[i].Id + '"><td data-value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</td><td>' + scriptlist[i].Info + '</td></tr>');
        scriptselect.push('<option value="' + scriptlist[i].Name + '">' + scriptlist[i].Name + '</option>');
        // }
    }
    target.find('#fieldScript').html(scriptHtml.join(''));
    var eventHtml = [];
    target.find('#fieldEvent').empty();
    for (var i = 0; i < eventlist.length; i++) {
        target.find('#fieldEvent').append('<tr class="eventitem" data-id="' + eventlist[i].eventid + '" onclick="chooseRowInfo(this)"><td>' + eventlist[i].Attribute + '</td><td><select class="scriptlist">' + scriptselect.join('') + '</select></td><td><select><option value="onchange">onchange</option></select></td><td><input type="text" value="' + eventlist[i].JsAction + '" /></td></tr>');
        target.find('#fieldEvent').find('tr:last').find('td:eq(1)').find('option[value="' + eventlist[i].JsLibrary + '"]').attr('selected', "selected");
        target.find('#fieldEvent').find('tr:last').find('td:eq(2)').find('option[value="' + eventlist[i].Name + '"]').attr('selected', "selected");
    }
    $('#setEventsModal').modal('show');
    console.log()
}

function saveEvents() {
    var target = $('#setEventsModal');
    var selected = $('.selected');
    eventlist = [];
    scriptlist = [];
    target.find('#fieldEvent').find('tr').each(function (i, n) {
        var eventitem = {
            'Attribute': $(n).find('td:eq(0)').text(),
            'Name': $(n).find('td:eq(2)').find('option:selected').val(),
            'JsAction': $(n).find('td:eq(3)').find('input').val(),
            'JsLibrary': $(n).find('td:eq(1)').find('option:selected').val(),
            'eventid': Xms.Utility.Guid.NewGuid().ToString()
        };
        eventlist.push(eventitem);
    });
    target.find('#fieldScript').find('tr').each(function (i, n) {
        var scriptitem = {
            'Attribute': $(n).attr('data-attributename'),
            'Name': $(n).find('td:eq(0)').text(),
            'Info': $(n).find('td:eq(1)').text(),
            'Id': $(n).attr('data-id')
        };
        var flag = true;
        if (scriptlist.length > 0) {
            $.each(scriptlist, function (key, item) {
                if (item.Id == scriptitem.Id) {
                    flag = false;
                    return false;
                }
            });
        }
        if (flag == true) {
            scriptlist.push(scriptitem);
        }
    });
    $('#setEventsModal').modal('hide');
}

function activeAttributeScriptTag() {
    if (eventlist.length > 0) {
        $('.cell[data-name]').removeClass('right-top-tag');
        $.each(eventlist, function (i, n) {
            if (n.Attribute) {
                var $attribute = $('.cell[data-name="' + n.Attribute + '"]');
                if ($attribute.length > 0) {
                    $attribute.addClass('right-top-tag')
                }
            }
        });
    }
}

//列移动排序
function moveColumn(direction) {
    var $this = $("#views").find('.selected');
    if (direction == 'before' && $this.prev().length > 0) {
        $this.insertBefore($this.prev());
    }
    else if (direction == 'after' && $this.next().length > 0) {
        $this.insertAfter($this.next());
    }
    saveGridConfig();
}

$(function () {
    $('.changestylecheckbox').on('click', function () {
        changeStylePreviewTarget()
    })
})