﻿(function($){
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
        Json : "Json"
    }

    var formGroupTemplate = 
        "<div class='form-group'>"+
            "<label class='col-sm-3 control-label' ></label>"+
            "<div class='col-sm-5 control'></div>"+
        "</div>";

    $.widget("ui.coldewForm", {
            options: {
                sections: null,
                values: null,
                editview: true
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
                var value = {};
                $.each(this._controls, function(name, control){
                    control.setValue(value[name]);
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
            _createFieldsets: function(){
                var thiz = this;
                var fieldsets = [];
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

                        var formGroup = $(formGroupTemplate);
                        formGroup.find(".control-label").text(input.field.name);
                        if(input.required){
                            formGroup.append("<font style='color: Red'>*</font>");    
                        }

                        var control = thiz._createControl(input);
                        formGroup.find(".control").append(control);

                        $("<div></div>")
                            .addClass(columnClass)
                            .append(formGroup)
                            .appendTo(row);
                    });
                    fieldsets.push(fieldset);
                });
                return fieldsets;
            },
            _createControl: function(input){
                var dialogSelectTemplate = 
                    "<div class='input-group'>"+
                        "<input type='text' readonly='readonly' class='form-control'>"+
                        "<span class='input-group-btn'>"+
                        "<button class='btn btn-default btnSelect' type='button'>选择</button>"+
                        "</span>"+
                    "</div>";

                var field = input.field;
                var control;
                switch (field.type){
                    case FieldType.String:
                    case FieldType.Name:
                        control = $("<input type='text' class='form-control'/>")
                            .textInput({name: field.code, required: input.required, suggestions: field.suggestions});
                        this._controls[field.code] = control.data("textInput");
                        break;
                    case FieldType.Text:
                        control = $("<textarea class='form-control' rows='3' ></textarea>")
                            .textarea({name: field.code, required: input.required});
                        this._controls[field.code] = control.data("textarea");
                        break;
                    case FieldType.DropdownList:
                        control = $("<select class='form-control'><option></option></select>")
                            .select({name: field.code, required: input.required, selectList: field.selectList});
                        this._controls[field.code] = control.data("select");
                        break;
                    case FieldType.RadioList:
                        control = $("<div></div>")
                            .radioList({name: field.code, required: input.required, selectList: field.selectList});
                        this._controls[field.code] = control.data("radioList");
                        break;
                    case FieldType.CheckboxList:
                        control = $("<div></div>")
                            .checkboxList({name: field.code, required: input.required, selectList: field.selectList});
                        this._controls[field.code] = control.data("checkboxList");
                        break;
                    case FieldType.Number:
                        control = $("<input type='text' class='form-control'/>")
                            .numberInput({name: field.code, required: input.required, max: field.max, min: field.min, precision: field.precision});
                        this._controls[field.code] = control.data("numberInput");
                        break;
                    case FieldType.Date:
                        control = $("<input type='text' class='form-control date'/>")
                            .dateInput({name: field.code, required: input.required});
                        this._controls[field.code] = control.data("dateInput");
                        break;
                    case FieldType.Metadata:
                        control = $(dialogSelectTemplate)
                            .metadataSelect({name: field.code, required: input.required, objectId: field.valueObjectId, objectName: field.valueObjectName});
                        this._controls[field.code] = control.data("metadataSelect");
                        break;
                    case FieldType.User:
                    case FieldType.UserList:
                        control = $(dialogSelectTemplate)
                            .userSelect({name: field.code, required: input.required});
                        this._controls[field.code] = control.data("userSelect");
                        break;
                }
                return control;
            }
        }
    );
})(jQuery);

(function($){

    $.widget("ui.input", {
            validate: function(){
                if(this.options.required){
                    var value = this.getValue();
                    if(!value){
                        this.element.focus();
                        this.element.closest('.form-group').addClass('has-error');
                        this.element.next('.help-block').show();
                        return false;
                    }
                    else{
                        this.element.closest('.form-group').removeClass(has-'error');
                        this.element.next('.help-block').hide();
                    }
                }
                return true;
            },
            getValue: function(){
                var value = this.element.val();
                return jQuery.trim(value);
            },
            setValue: function(value){
                this.element.val(value);
            },
            readonly: function(readonly){
                this.element.prop("readonly", readonly);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.textInput", $.ui.input, {
            options: {
                required: false,
                suggestions: null,
                name: null
	        },
	        _create: function(){
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                if(this.options.suggestions){
                    this.element.addClass("suggestion-input")
                        .data("suggestions", this.options.suggestions);
                }
	        }
        }
    );
})(jQuery);

(function($){

    $.widget("ui.textarea", $.ui.input, {
            options: {
                required: false,
                name: null
	        },
	        _create: function(){
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
	        }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.select", $.ui.input, {
            options: {
                required: false,
                name: null,
                selectList: null
	        },
	        _create: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                $("<option></option>").appendTo(thiz.element);
                $.each(this.options.selectList, function(){
                    $("<option></option>").text(this).appendTo(thiz.element);
                })
	        }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.radioList", $.ui.input, {
            options: {
                required: false,
                name: null,
                selectList: null
	        },
	        _create: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                $.each(this.options.selectList, function(){
                    var radio = $("<input type='radio'/>")
                        .attr("name", field.code)
                        .attr("value", this);
                    if(thiz.options.required){
                        radio.data("required", true);
                    }
                    $("<label class='radio'></label>")
                        .text(this)
                        .prepend(radio)
                        .appendTo(thiz.element);
                });
	        },
            getValue: function(){
                var value = this.element.find(":checked").val();
                return value;
            },
            setValue: function(value){
                this.element.find("input").each(function(){
                    if(value && $(this).val() == value.toString()){
                        $(this).prop("checked", true);
                    }
                });
            },
            readonly: function(readonly){
                this.element.find("input").prop("readonly", readonly);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.checkboxList", $.ui.input, {
            options: {
                required: false,
                name: null,
                selectList: null
	        },
	        _create: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                $.each(this.options.selectList, function(){
                    var radio = $("<input type='checkbox'/>")
                        .attr("name", field.code)
                        .attr("value", this);
                    if(thiz.options.required){
                        radio.data("required", true);
                    }
                    $("<label class='checkbox'></label>")
                        .text(this)
                        .prepend(radio)
                        .appendTo(thiz.element);
                });
	        },
            getValue: function(){
                var value = this.element.find(":checked")
                    .map(function(){
                        return $(this).val();
                    });
                return value;
            },
            setValue: function(value){
                this.element.find("input").each(function(){
                    if(value && $.inArray($(this).val(), value) > -1){
                        $(this).prop("checked", true);
                    }
                });
            },
            readonly: function(readonly){
                this.element.find("input").prop("readonly", readonly);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.numberInput", $.ui.input, {
            options: {
                required: false,
                name: null,
                max: null,
                min: null,
                precision: 2
	        },
	        _create: function(){
                
                if(this.options.precision > 0){
                    this._numberRegex = new RegExp("/^[-,+][0-9]+(.[0-9]{0, ${0}})?$/".format(this.options.precision));
                }
                else{
                    this._numberRegex = new RegExp("/^[-,+][0-9]*[1-9][0-9]*$/");
                }

                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
	        },
            _setError: function(){
                this.element.focus();
                this.element.closest('.form-group').addClass('has-error');
                this.element.next('.help-block').show();
            },
            validate: function(){
                var value = this.element.val();
                if(this.options.required){
                    if(!value){
                        this._setError();
                        return false;
                    }
                    else{
                        this.element.closest('.form-group').removeClass(has-'error');
                        this.element.next('.help-block').hide();
                    }
                }
                if(value){
                    if(!this._numberRegex.test(value)){
                        this._setError();
                        return false;
                    }
                    value = parseFloat(value);
                    if(this.options.max && value > this.options.max){
                        this._setError();
                        return false;
                    }
                    if(this.options.min && value < this.options.min){
                        this._setError();
                        return false;
                    }
                }
                return true;
            },
            getValue: function(){
                var value = this.element.val();
                value = parseFloat(value);
                if(!value){
                    return null;
                }
                return value;
            },
            setValue: function(value){
                this.element.val(value);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.metadataSelect", $.ui.input, {
            options: {
                required: false,
                name: null,
                objectId: null,
                objectName: null
	        },
	        _create: function(){
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
                        this.element.closest('.form-group').removeClass(has-'error');
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
                if(value){
                    this.element.find("input").val(value.name);
                }
            },
            readonly: function(readonly){
                if(readonly){
                    this._btnSelect.prop("disabled", true);
                }
                else{
                    this._btnSelect.prop("disabled", false);
                }
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.userSelect", $.ui.input, {
            options: {
                required: false,
                name: null,
                single: null
	        },
	        _create: function(){
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
                        this.element.closest('.form-group').removeClass(has-'error');
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
                if(value){
                    this.element.find("input").val(value.name);
                }
            },
            readonly: function(readonly){
                if(readonly){
                    this._btnSelect.prop("disabled", true);
                }
                else{
                    this._btnSelect.prop("disabled", false);
                }
            }
        }
    );
})(jQuery);