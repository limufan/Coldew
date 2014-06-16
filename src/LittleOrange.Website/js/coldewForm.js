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

    var formGroupTemplate = 
        "<div class='form-group'>"+
            "<label style='max-width: 150px;' class='col-sm-4 control-label' ></label>"+
            "<div class='col-sm-8 control'></div>"+
        "</div>";

    $.widget("ui.coldewForm", {
            options: {
                sections: null
	        },
	        _create: function(){
                this._controls = {};
                var fieldsets = this._createFieldsets();
                this.element.append(fieldsets);
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
            reset: function(){
                $.each(this._controls, function(name, control){
                    control.reset();
                });
            },
            _createFieldsets: function(){
                var thiz = this;
                var fieldsets = [];
                var container = null;
                $.each(this.options.sections, function(){
                    var fieldset = $("<fieldset><legend></legend></fieldset>");
                    var columnCount = this.columnCount;
                    var columnClass = "col-md-" + (12 / columnCount).toString();
                    fieldset.find("legend").text(this.name);

                    var row;
                    $.each(this.inputs, function(i, input){
                        if(i % columnCount == 0){
                            row = $("<div class='row'></div>").appendTo(fieldset);
                        }
                        if(this.field.type == FieldType.Json)
                        {
                            container = $("<div style='margin-bottom: 3px;'></div>")
                            .addClass("col-md-12")
                            .appendTo(row);
                        }
                        else{
                            var formGroup = $(formGroupTemplate);
                            var controlLabel = formGroup.find(".control-label").text(input.field.name);
                            if(input.required){
                                controlLabel.append("<span style='color: Red'>*</span>");    
                            }
                            container = formGroup.find(".control");
                            $("<div></div>")
                                .addClass(columnClass)
                                .append(formGroup)
                                .appendTo(row);
                        }
                        thiz._createControl(this, container);
                    });
                    fieldsets.push(fieldset);
                });
                return fieldsets;
            },
            _createControl: function(input, container){
                var dialogSelectTemplate = 
                    "<div>"+
                        "<div class='input-group'>"+
                            "<input type='text' readonly='readonly' class='form-control'>"+
                            "<span class='input-group-btn'>"+
                            "<button class='btn btn-default btnSelect' type='button'>选择</button>"+
                            "</span>"+
                        "</div>"+
                    "</div>";

                var field = input.field;
                var inputOptions = { name: field.code, required: input.required, readonly: input.isReadonly, defaultValue: field.defaultValue };
                var control;
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
                    case FieldType.Json:
                        control = $("<div></div>")
                            .appendTo(container)
                            .placeholder(inputOptions);
                        this._controls[field.code] = control.data("placeholder");
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
                    var control = thiz._createControl(this);
                    var formGroup = $(formGroupTemplate);
                    formGroup.find(".control-label").text(this.name);
                    formGroup.find(".control").append(control);
                    $("<div></div>")
                        .addClass("col-md-6")
                        .append(formGroup)
                        .appendTo(row);
                });
                return fieldsets;
            },
            _createControl: function(field){
                var control;
                switch (field.type){
                    case FieldType.Number:
                        var numberRangeInputTemplate = 
                            "<div class='input-group'>"+
                                "<input type='text' name='min' class='form-control'/>"+
                                "<span class='input-group-addon'>到</span>"+
                                "<input type='text' name='max' class='form-control'/>"+
                            "</div>"
                        control = $(numberRangeInputTemplate)
                            .numberRangeInput({name: field.code});
                        this._controls[field.code] = control.data("numberRangeInput");
                        break;
                    case FieldType.Date:
                    case FieldType.ModifiedTime:
                    case FieldType.CreatedTime:
                        var dateRangeTemplate = 
                            "<div class='input-group'>"+
                                "<input type='text' name='start' class='form-control'/> "+
                                "<span class='input-group-addon'>到</span>"+
                                "<input type='text' name='end' class='form-control'/>"+
                            "</div>";
                        control = $(dateRangeTemplate)
                            .dateRangeInput({name: field.code});
                        this._controls[field.code] = control.data("dateRangeInput");
                        break;
                    default :
                        control = $("<input class='form-control' type='text'/>")
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