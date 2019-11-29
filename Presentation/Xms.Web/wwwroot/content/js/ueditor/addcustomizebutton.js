; (function () {
    UE.registerUI('addpicimg', function (editor, uiName) {
        //注册按钮执行时的command命令，使用命令默认就会带有回退操作
        editor.registerCommand(uiName, {
            execCommand: function () {
                alert('execCommand:' + uiName)
            }
        });
        //ntext_upload
        //if ($('.ntext_uploadify-field').length > 0) {
        //    $('.ntext_uploadify-field').uploadTo64({
        //        uploadEnd: function (txshow, value,result) {
        //            console.log(result);
        //            editor.execCommand('insertimage', {
        //                src: ''
        //            });
        //        }
        //    });
        //}
        //创建一个button
        var btn = new UE.ui.Button({
            //按钮的名字
            name: uiName,
            //提示
            title: uiName,
            class: uiName,
            //需要添加的额外样式，指定icon图标，这里默认使用一个重复的icon
            cssRules: 'background-position: -380px 0;',
            //点击时执行的命令
            onclick: function (e) {
                //这里可以不用执行命令,做你自己的操作也可
                console.log($(this.target).parents('.edui-editor:first').parent().next('input:first'));
                var url = '/file/Create';
                if (url.indexOf(ORG_SERVERURL) !== 0) {
                    url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
                }
                var _form = $('<form class="hide" action="' + url + '" method="post" enctype="multipart/form-data"></form>');
                var $entityid = $('<input type="hidden" name="entityid" value="' + Xms.Page.PageContext.EntityId + '" />');
                var $objectid = $('<input type="hidden" name="objectid" value="' + Xms.Page.PageContext.RecordId + '" />');
                var $file = $('<input type="file" name="attachment" >');
                $('body').append(_form);
                _form.append($entityid).append($objectid).append($file);
                $file.trigger('click');
                $file.on('change', function (e, opts) {
                    console.log('uploadform', e, opts);
                    setTimeout(function () {
                        _form.ajaxForm(function (response) {
                            if (response.IsSuccess) {
                                var _protocol = location.protocol;
                                var _host = location.host;
                                //console.log(_host);
                                var src = response.Extra.filepath;
                                var allPath = _protocol + '//' + _host + src;
                                editor.execCommand('insertimage', {
                                    src: allPath
                                });
                                $file.off();
                                _form.remove();
                                return;
                            }
                            Xms.Web.Alert(false, response.Content);
                        }).submit();
                    }, 0);
                })

                //    uploadTo64({
                //    uploadEnd: function (txshow, value) {
                //        editor.execCommand('insertimage', {
                //            src: $(value).val()
                //        });
                //        var id = $(value).attr("id");
                //        dirtyChecker.setValue(id, $(value).val());
                //        dirtyChecker.checkWatchs(function () {
                //            bindBeforeUnload();
                //        });
                //    }
                //});
            }
        }, ['0', ['mypicman']]);

        //当点到编辑内容上时，按钮要做的状态反射
        editor.addListener('selectionchange', function () {
            var state = editor.queryCommandState(uiName);
            if (state == -1) {
                btn.setDisabled(true);
                btn.setChecked(false);
            } else {
                btn.setDisabled(false);
                btn.setChecked(state);
            }
        });

        //因为你是添加button,所以需要返回这个button
        return btn;
    }/*index 指定添加到工具栏上的那个位置，默认时追加到最后,editorId 指定这个UI是那个编辑器实例上的，默认是页面上所有的编辑器都会添加这个按钮*/);
})();