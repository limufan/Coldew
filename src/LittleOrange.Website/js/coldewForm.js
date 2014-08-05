(function($){
    var FieldType ={
        CreatedUser : "CreatedUser",
        CreatedTime : "CreatedTime",
        ModifiedUser : "ModifiedUser",
        ModifiedTime : "ModifiedTime",
        Name : "Name",
        String : "String",
        Text : "Text",
        DropdownList : "DropdownList",
        RadioList : "RadioList",
        CheckboxList : "CheckboxList",
        Number : "Number",
        Date : "Date",
        User : "User",
        UserList : "UserList",
        Metadata : "Metadata",
        RelatedField : "RelatedField",
        Json : "Json",
        Code : "Code"
    }

    var dialogSelectTemplate = 
        "<div>"+
            "<div class='input-group'>"+
                "<input type='text' readonly='readonly' class='form-control'>"+
                "<span class='input-group-btn'>"+
                "<button class='btn btn-default btnSelect' type='button'>选择</button>"+
                "</span>"+
            "</div>"+
        "</div>";

    var formGroupTemplate = 
        "<div class='form-group'>"+
            "<label class='control-label' ></label>"+
            "<div class='control-input'></div>"+
        "</div>";

    $.widget("ui.coldewForm", {
            options: {
                controls: null
	        },
	        _create: function(){
                this._controls = {};
                var thiz = this;
                var container = null;
                var element = this.element;
                
                $.each(this.options.controls, function(){
                    thiz._createControl(this, element);
                })
                this.element.horizontalForm();
	        },
            getValue: function(){
                var value = {};
                $.each(this._controls, function(name, control){
                    value[name] = control.getValue();
                });
                return value;
            },
            setValue: function(value){
                for(name in value){
                    if(name in this._controls){
                        if(name == "chanpinGrid"){
                            ;
                        }
                        this._controls[name].setValue(value[name]);
                    }
                }
            },
            setReadonly: function(readonly){
                $.each(this._controls, function(name, control){
                    control.setReadonly(readonly);
                });
            },
            validate: function(){
                var value = {};
                var valid = true;
                $.each(this._controls, function(name, control){
                    if(!control.validate()){
                       valid = false; 
                    }
                });
                return valid;
            },
            getInput: function(name){
                return this._controls[name];
            },
            setInput: function(name, control){
                return this._controls[name] = control;
            },
            setDefaultValue: function(value){
                for(name in value){
                    if(name in this._controls){
                        this._controls[name].setDefaultValue(value[name]);
                    }
                }
            },
            reset: function(){
                $.each(this._controls, function(name, control){
                    control.reset();
                });
            },
            changed: function(callback){
                $.each(this._controls, function(name, control){
                    control.changed(function(_input, value){
                        callback(_input, value);
                    });
                });
            },
            inputing: function(callback, delay){
                $.each(this._controls, function(name, control){
                    control.inputing(function(_input, value){
                        callback(_input, value);
                    }, delay);
                });
            },
            _createControl: function(info, container){
                var thiz = this;
                switch(info.type){
                    case "input": 
                    case "grid": 
                        var columnClass = "col-md-" + info.width;
                        var formGroup = $(formGroupTemplate);
                        $("<div></div>").addClass(columnClass).append(formGroup).appendTo(container);
                        var controlLabel = formGroup.find("label");
                        controlLabel.text(info.field.name);
                        if(info.required){
                            controlLabel.prepend("<span style='color: Red'>*</span>");    
                        }
                        thiz._createInput(info, formGroup.find(".control-input"));
                        break;
                    case "row": 
                        var row = $("<div class='row'></div>").appendTo(container);
                        if(info.children && info.children.length){
                            $.each(info.children, function(){
                                thiz._createControl(this, row)
                            });
                        }
                        break;
                    case "fieldset": 
                        $("<fieldset><legend></legend></fieldset>")
                            .appendTo(container)
                            .find("legend")
                            .text(info.title);
                        break;
                    case "tab":
                        var nav_tabs = $("<ul class='nav nav-tabs' role='tablist'></ul>")
                            .appendTo(container);
                        var tab_content = $("<div class='tab-content'></div>")
                            .appendTo(container);
                        tab_content.data("nav_tabs", nav_tabs);
                        if(info.children && info.children.length){
                            $.each(info.children, function(){
                                thiz._createControl(this, tab_content)
                            });
                        }
                        break;
                    case "tabPane":
                        var tab_content = container;
                        var nav_tabs = tab_content.data("nav_tabs");
                        var paneId = $.now();
                        var pane = $("<div class='tab-pane'></div>")
                            .attr("id", paneId)
                            .appendTo(tab_content);
                        var nav = $("<li><a role='tab' data-toggle='tab'></a></li>")
                            .appendTo(nav_tabs);
                        if(info.active){
                            pane.addClass("active");
                            nav.addClass("active");
                        }
                        nav.find("a").attr("href", "#" + paneId).text(info.title);
                        if(info.children && info.children.length){
                            $.each(info.children, function(){
                                thiz._createControl(this, pane)
                            });
                        }
                        break;
                    case "datagrid":
                        var datagrid = $("<div></div>").appendTo(container).datagrid(info).data("datagrid");
                        this._controls[info.name] = datagrid;
                        break;
                }
            },
            _createInput: function(input, container){
                var field = input.field;
                var inputOptions = { name: field.code, required: input.required, readonly: input.isReadonly, defaultValue: field.defaultValue };
                var control;
                if(input.type == "grid"){
                    $.extend(inputOptions, {addForm: input.addForm, editForm: input.editForm, columns: input.columns, footer: input.footer, editable: input.editable});
                    var coldewGrid = $("<div></div>").appendTo(container).coldewGrid(inputOptions).data("coldewGrid");
                    this._controls[field.code] = coldewGrid;
                }
                switch (field.type){
                    case FieldType.String:
                        inputOptions.suggestions = field.suggestions;
                        control = $("<input type='text' class='form-control'/>")
                            .appendTo(container)
                            .textbox(inputOptions);
                        this._controls[field.code] = control.data("textbox");
                        break;
                    case FieldType.Text:
                        control = $("<textarea class='form-control' rows='3' ></textarea>")
                            .appendTo(container)
                            .textarea(inputOptions);
                        this._controls[field.code] = control.data("textarea");
                        break;
                    case FieldType.DropdownList:
                        var select = $("<select class='form-control'><option></option></select>");
                        $.each(field.selectList, function(){
                            $("<option></option>").text(this).appendTo(select);
                        })
                        control = select
                            .appendTo(container)
                            .simpleSelect(inputOptions);
                        this._controls[field.code] = control.data("simpleSelect");
                        break;
                    case FieldType.RadioList:
                        var radioList = $("<div></div>");
                        $.each(field.selectList, function(){
                            var radio = $("<input type='radio'/>")
                                .attr("name", field.name)
                                .attr("value", this);
                            $("<label class='radio-inline'></label>")
                                .text(this)
                                .prepend(radio)
                                .appendTo(radioList);
                        });
                        inputOptions.selectList = field.selectList;
                        control = radioList
                            .appendTo(container)
                            .radioList(inputOptions);
                        this._controls[field.code] = control.data("radioList");
                        break;
                    case FieldType.CheckboxList:
                        var checkboxList = $("<div></div>");
                        $.each(field.selectList, function(){
                            var radio = $("<input type='checkbox'/>")
                                .attr("name", field.name)
                                .attr("value", this);
                            $("<label class='checkbox'></label>")
                                .text(this)
                                .prepend(radio)
                                .appendTo(checkboxList);
                        });
                        inputOptions.selectList = field.selectList;
                        control = checkboxList
                            .appendTo(container)
                            .checkboxList(inputOptions);
                        this._controls[field.code] = control.data("checkboxList");
                        break;
                    case FieldType.Number:
                        $.extend(inputOptions, {max: field.max, min: field.min, precision: field.precision});
                        control = $("<input type='text' class='form-control'/>")
                            .appendTo(container)
                            .numberInput(inputOptions);
                        this._controls[field.code] = control.data("numberInput");
                        break;
                    case FieldType.Date:
                        inputOptions.defaultValue = field.defaultValue;
                        control = $("<input type='text' class='form-control date'/>")
                            .appendTo(container)
                            .dateInput(inputOptions);
                        this._controls[field.code] = control.data("dateInput");
                        break;
                    case FieldType.Metadata:
                        $.extend(inputOptions, {objectId: field.valueObjectId, objectName: field.valueObjectName});
                        control = $(dialogSelectTemplate)
                            .appendTo(container)
                            .metadataSelect(inputOptions);
                        this._controls[field.code] = control.data("metadataSelect");
                        break;
                    case FieldType.User:
                        control = $(dialogSelectTemplate)
                            .appendTo(container)
                            .userSelect(inputOptions);
                        this._controls[field.code] = control.data("userSelect");
                        break;
                    case FieldType.UserList:
                        control = $(dialogSelectTemplate)
                            .appendTo(container)
                            .userListSelect(inputOptions);
                        this._controls[field.code] = control.data("userListSelect");
                        break;
                    case FieldType.Code:
                        control = $("<input type='text'  class='form-control'/>")
                            .appendTo(container)
                            .codeInput(inputOptions);
                        this._controls[field.code] = control.data("codeInput");
                        break;
                }
                return control;
            }
        }
    );

    $.widget("ui.coldewSearchForm", $.ui.coldewForm, {
            options: {
                fields: null
	        },
	        _create: function(){
                this._controls = {};
                var fieldsets = this._createFieldsets();
                this.element.append(fieldsets);
	        },
            _createFieldsets: function(){
                var thiz = this;
                var fieldsets = [];
                var element = this.element;
                var row;
                $.each(this.options.fields, function(i){
                    if(i % 2 == 0){
                        row = $("<div class='row'></div>").appendTo(element);
                    }
                    var formGroup = $(formGroupTemplate);
                    formGroup.find(".control-label").text(this.name);
                    var controlInput = formGroup.find(".control-input");
                    $("<div class='col-md-6'></div>")
                        .append(formGroup)
                        .appendTo(row);
                    var control = thiz._createControl(this, controlInput);
                });
                return fieldsets;
            },
            _createControl: function(field, container){
                var control;
                switch (field.type){
                    case FieldType.Number:
                        var numberRangeInputTemplate = 
                            "<div class='input-group webui-numberRangeInput'>"+
				    	        "<input class='form-control' type='text'/>"+
				    	        "<span class='input-group-addon' >到</span>"+
				    	        "<input class='form-control' type='text'/>"+
			    	        "</div>"
                        control = $(numberRangeInputTemplate)
                            .appendTo(container)
                            .numberRangeInput({name: field.code});
                        this._controls[field.code] = control.data("numberRangeInput");
                        break;
                    case FieldType.Date:
                    case FieldType.ModifiedTime:
                    case FieldType.CreatedTime:
                        var dateRangeTemplate = 
                            "<div class='input-group webui-dateRangeInput'>"+
				    	        "<input class='form-control' type='text'/>"+
				    	        "<span class='input-group-addon'>到</span>"+
				    	        "<input class='form-control' type='text' />"+
			    	        "</div>";
                        control = $(dateRangeTemplate)
                            .appendTo(container)
                            .dateRangeInput({name: field.code});
                        this._controls[field.code] = control.data("dateRangeInput");
                        break;
                    default :
                        control = $("<input class='form-control' type='text'/>")
                            .appendTo(container)
                            .textbox({name: field.code});
                        this._controls[field.code] = control.data("textbox");
                        break;
                }
                return control;
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.metadataSelect", $.webui.input, {
            options: {
                required: false,
                name: null,
                objectId: null,
                objectName: null
	        },
	        _onCreated: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                this._btnSelect = this.element.find(".btnSelect");
                this._txtName = this.element.find("input");
                this._btnSelect.click(function(){
                    $.metadataSelectDialog.metadataSelectDialog("select", thiz.options.objectId, thiz.options.objectName, function(args){
                        thiz._txtName.val(args.name);
                        thiz._metadata = {};
                        thiz._metadata.name = args.name;
                        thiz._metadata.id = args.id;
                    });
                    return false;
                });
	        },
            _setError: function(){
                this.element.focus();
                this.element.closest('.form-group').addClass('has-error');
                this.element.next('.help-block').show();
            },
            validate: function(){
                var value = this._metadata;
                if(this.options.required){
                    if(!value){
                        this._setError();
                        return false;
                    }
                    else{
                        this.element.closest('.form-group').removeClass('has-error');
                        this.element.next('.help-block').hide();
                    }
                }
                return true;
            },
            getValue: function(){
                return this._metadata;
            },
            setValue: function(value){
                this._metadata = value;
                var text = "";
                if(value){
                    text = value.name;
                }
                this.element.find("input").val(text);
                this._textElement.html(text);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.userSelect", $.webui.input, {
            options: {
                required: false,
                name: null,
                single: null,
                defaultValue: null
	        },
	        _onCreated: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                this._btnSelect = this.element.find(".btnSelect");
                this._txtName = this.element.find("input");
                this._btnSelect.click(function(){
                    $.chengyuanDialog.chengyuanDialog("xuanze", {
                        yonghu: true,
                        xuanzehou: function(args){
                            var yonghu = args.yonghuList[0];
                            thiz._txtName.val(yonghu.name);
                            thiz._user = {};
                            thiz._user.name = yonghu.name;
                            thiz._user.account = yonghu.account;
                            return true;
                        }             
                    });
                    return false;
                });
                if(this.options.defaultValue){
                    this.setValue(this.options.defaultValue);
                }
	        },
            _setError: function(){
                this.element.focus();
                this.element.closest('.form-group').addClass('has-error');
                this.element.next('.help-block').show();
            },
            validate: function(){
                var value = this._user;
                if(this.options.required){
                    if(!value){
                        this._setError();
                        return false;
                    }
                    else{
                        this.element.closest('.form-group').removeClass('has-error');
                        this.element.next('.help-block').hide();
                    }
                }
                return true;
            },
            getValue: function(){
                return this._user;
            },
            setValue: function(value){
                this._user = value;
                var text = "";
                if(value){
                    text = value.name;
                }
                this.element.find("input").val(text);
                this._textElement.html(text);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.userListSelect", $.webui.input, {
            options: {
                required: false,
                name: null
	        },
	        _onCreated: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                this._btnSelect = this.element.find(".btnSelect");
                this._txtName = this.element.find("input");
                this._btnSelect.click(function(){
                    $.chengyuanDialog.chengyuanDialog("xuanze", {
                        yonghu: true,
                        xuanzehou: function(args){
                            thiz.setValue(args.yonghuList);
                            return true;
                        }             
                    });
                    return false;
                });
	        },
            _setError: function(){
                this.element.focus();
                this.element.closest('.form-group').addClass('has-error');
                this.element.next('.help-block').show();
            },
            validate: function(){
                var value = this._userList;
                if(this.options.required){
                    if(!value){
                        this._setError();
                        return false;
                    }
                    else{
                        this.element.closest('.form-group').removeClass('has-error');
                        this.element.next('.help-block').hide();
                    }
                }
                return true;
            },
            getValue: function(){
                return this._userList;
            },
            setValue: function(value){
                this._userList = value;
                var text = "";
                if(value){
                    var nameArray = $.map(value, function(user){
                        return user.name;
                    });
                    text = nameArray.toString();
                }
                this.element.find("input").val(text);
                this._textElement.html(text);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.placeholder", {
        }
    );
})(jQuery);

(function($){
    $.widget("ui.metadataAutoComplete", {
            options: {
                objectCode: null
	        },
	        _create: function(){
                var thiz = this;
                var objectCode = this.options.objectCode; 
                var sourceUrl = $.baseUrl + "Metadata/AutoCompleteList?objectCode=" + objectCode;
                this.element.autocomplete({
                    source: sourceUrl,
                    minLength: 0,
                    focus: function (event, ui) {
                        return false;
                    },
                    select: function (event, ui) {
                        $(this).val(ui.item.name);
                        thiz._trigger("select", null, ui.item);
                        return false;
                    }
                })
                .click(function () {
                    $(this).autocomplete("search", "");
                })
                .data("autocomplete")._renderItem = function (ul, item) {
                    return $("<li>")
                    .data("item.autocomplete", item)
                    .append("<a>" + item.name + "<br>" + item.summary + "</a>")
                    .appendTo(ul);
                };
	        }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.codeInput", $.webui.input, {
            options: {
                required: false,
                defaultValue: null,
                name: null
	        },
	        _onCreated: function(){
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                if(this.options.defaultValue){
                    this.element.val(this.options.defaultValue);
                }
                this.element.prop("readonly", true);
	        }
        }
    );
})(jQuery);

(function($){
    var gridModalHtml =  
        '<div class="modal fade" style="display: block">'+
            '<div class="modal-dialog" style="width: 650px">'+
                '<div class="modal-content">'+
                    '<div class="modal-header">'+
                        '<button type="button" class="close" data-dismiss="modal">&times;</button>'+
                        '<h3 class="modal-title"></h3>'+
                    '</div>'+
                    '<form class="form-horizontal">'+
                        '<div class="modal-body" style="height:300px;overflow: auto;">'+
                            '<div class="form-details"></div>'+
                        '</div>'+
                        '<div class="modal-footer">'+
                            '<button class="btn btn-default btnSave" >保存</button>'+
                            '<button class="btn btn-default" data-dismiss="modal" aria-hidden="true">关闭</button>'+
                        '</div>'+
                    '</form>'+
                '</div>'+
            '</div>'+
        '</div>';
    $.widget("ui.coldewDialog", {
        _create: function(){
            var thiz = this;
            var dialog = this._dialog = this.element;
            dialog.find(".modal-title").text(this.options.form.title);
            var detailsForm = this._detailsForm = dialog.find(".form-details").coldewForm({controls: this.options.form.controls}).data("coldewForm");
            dialog.css({display: "" });
            dialog.find(".btnSave").click(function(){
                if(detailsForm.validate()){
                    var formValue = detailsForm.getValue();
                    try{
                        thiz._trigger("save", null, formValue);
                    }
                    catch(e){
                        alert(e);
                    }
                    detailsForm.reset();
                    dialog.modal("hide");
                }
                return false;
            });
        },
        show: function(){
            this._dialog.modal("show");
        },
        hide: function(){
            this._dialog.modal("hide");
        },
        getForm: function(){
            return this._detailsForm;
        }
    });

    $.widget("ui.coldewGrid", $.webui.input, {
            options: {
                required: false,
                name: null
	        },
	        _onCreated: function(){
                var thiz = this;
                if(this.options.addForm){
                    var addDialog = $(gridModalHtml).appendTo("body")
                        .coldewDialog({
                            form: this.options.addForm,
                            save: function(event, formValue){
                                chanpinGrid.datagrid("appendRow", formValue);
                            }
                        })
                        .data("coldewDialog");
                    this._addForm = addDialog.getForm();
                }
                if(this.options.editForm){
                    var editDialog = $(gridModalHtml).appendTo("body")
                        .coldewDialog({
                            form: this.options.editForm,
                            save: function(event, formValue){
                                thiz._editRow.datarow("setValue", formValue);
                            }
                        })
                        .data("coldewDialog");
                    this._editForm = editDialog.getForm();
                }
                var toolbar = 
                    "<div class='btn-group'>"+
                        "<button class='btn btn-default btnAdd'>添加</button>"+
                        "<button disabled='disabled' class='btn btn-default btnEdit'>编辑</button> "+
                        "<button disabled='disabled' class='btn btn-default btnDelete'>删除</button> "+
                    "</div>";
                this._toolbar = $(toolbar).appendTo(this.element);
                var buttons = this._toolbar.find("button");
                var btnAddChanpin = buttons.eq(0)
                    .click(function(){
                        thiz._addForm.reset();
                        addDialog.show();
                        return false;
                    });

                var btnEditChanpin = buttons.eq(1)
                    .click(function(){
                        var row = thiz._editRow = chanpinGrid.datagrid("getSelectedRow");
                        var editInfo = row.datarow("getValue");
                        editDialog.show();
                        thiz._editForm.setValue(editInfo);
                        return false;
                    });
                var btnDeleteChanpin = buttons.eq(2)
                    .click(function(){
                        if(confirm("确实要删除吗?")){
                            chanpinGrid.datagrid("deleteSelectedRows");
                            btnEditChanpin.prop("disabled", true);
                            btnDeleteChanpin.prop("disabled", true);
                        }
                        return false;
                    });
                var columns = this.options.columns;
                var footer = this.options.footer;
                var chanpinGrid = this._chanpinGrid = $("<div></div>")
                    .appendTo(this.element)
                    .datagrid({
                        columns: columns,
		                canSort: false,
		                singleSelect: true,
		                showNumberRow: false,
                        selectedRow: function(){
                            btnEditChanpin.prop("disabled", false);
                            btnDeleteChanpin.prop("disabled", false);
                        },
                        unselectedRow: function(){
                            btnEditChanpin.prop("disabled", true);
                            btnDeleteChanpin.prop("disabled", true);
                        },
                        footer: footer
                    });
                if(this.options.editable){
                    this.setEditable();
                }
	        },
            getAddForm: function(){
                return this._addForm;
            },
            getEditForm: function(){
                return this._editForm;
            },
            getValue: function(){
                return this._chanpinGrid.datagrid("getRowsData");
            },
            setValue: function(value){
                this._chanpinGrid.datagrid("setValue", value);
            },
            setReadonly: function(readonly){
                if(readonly){
                    this._toolbar.hide();
                }
                else{
                    this._toolbar.show();
                }
                this._readonly = readonly;
            },
            getReadonly: function(){
                return this._readonly;
            },
            setEditable: function(){
                this._toolbar.find(".btnAdd, .btnDelete").hide();
            },
            validate: function(){
                var value = this.getValue();
                if(this.options.required){
                    if(!value.length){
                        this.element.closest('.form-group').addClass('has-error');
                        return false;
                    }
                    else{
                        this.element.closest('.form-group').removeClass('has-error');
                    }
                }
                return true;
            }
        }
    );
})(jQuery);