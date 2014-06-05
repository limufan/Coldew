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
            getControl: function(name){
                return this._controls[name];
            },
            setControl: function(name, control){
                return this._controls[name] = control;
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
                        var controlLabel = formGroup.find(".control-label").text(input.field.name);
                        if(input.required){
                            controlLabel.append("<span style='color: Red'>*</span>");    
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
                            .dateInput({name: field.code, required: input.required, defaultValueIsToday: field.defaultValueIsToday});
                        this._controls[field.code] = control.data("dateInput");
                        break;
                    case FieldType.Metadata:
                        control = $(dialogSelectTemplate)
                            .metadataSelect({name: field.code, required: input.required, objectId: field.valueObjectId, objectName: field.valueObjectName});
                        this._controls[field.code] = control.data("metadataSelect");
                        break;
                    case FieldType.User:
                        control = $(dialogSelectTemplate)
                            .userSelect({name: field.code, required: input.required, defaultValue: field.defaultValue});
                        this._controls[field.code] = control.data("userSelect");
                        break;
                    case FieldType.UserList:
                        control = $(dialogSelectTemplate)
                            .userListSelect({name: field.code, required: input.required});
                        this._controls[field.code] = control.data("userListSelect");
                        break;
                    case FieldType.Json:
                        control = $("<div></div>")
                            .placeholder({name: field.code, required: input.required});
                        this._controls[field.code] = control.data("placeholder");
                        break;
                    case FieldType.Code:
                        control = $("<input type='text' class='form-control'/>")
                            .codeInput({name: field.code, required: input.required, defaultValue: field.defaultValue});
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

                    var control = thiz._createControl(this);
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
                            .textInput({name: field.code});
                        this._controls[field.code] = control.data("textInput");
                        break;
                }
                return control;
            }
        }
    );

    $.widget("ui.coldewDetailsForm", {
            options: {
                sections: null
	        },
	        _create: function(){
                this._controls = {};
                var fieldsets = this._createFieldsets();
                this.element.append(fieldsets);
	        },
            setValue: function(value){
                $.each(this._controls, function(name, control){
                    control.setValue(value[name]);
                });
            },
            getControl: function(name){
                return this._controls[name];
            },
            setControl: function(name, control){
                return this._controls[name] = control;
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
                var field = input.field;
                var control = $("<p class='form-control-static'></p>")
                            .label({name: field.code});
                        this._controls[field.code] = control.data("label");
                return control;
            }
        }
    );
})(jQuery);

(function($){

    $.widget("ui.coldewControl", {
            _create: function(){
                this._createControl();
            },
            _createControl: function(){
                
            },
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
                        this.element.closest('.form-group').removeClass('has-error');
                        this.element.next('.help-block').hide();
                    }
                }
                return true;
            },
            getValue: function(){
                var value = this.element.val();
                return jQuery.trim(value);
            },
            getText: function(){
                return this.getValue();
            },
            _setText: function(){
                if(!this._staticControl){
                    this._staticControl = $("<p style='display: none' class='form-control-static'></p>");
                    this.element.after(this._staticControl);
                }
                var text = this.getText();
                    if(!text){
                        text = "";
                    }
                this._staticControl.html(text);
            },
            setValue: function(value){
                this.element.val(value);
                this._setText();
            },
            setReadonly: function(readonly){
                if(readonly){
                    this._setText();
                    this._staticControl.show();
                    this.element.hide();
                }
                else{
                    if(this._staticControl){
                        this._staticControl.hide();
                    }
                    this.element.show();
                }
                this._readonly = readonly;
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.dateInput", $.ui.coldewControl, {
            options: {
                required: false,
                name: null,
                defaultValueIsToday: false
	        },
            _createControl: function(){
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                if(this.options.defaultValueIsToday){
                    this.setValue(new Date());
                }
                this.element.datepicker();
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
                        this.element.closest('.form-group').removeClass('has-error');
                        this.element.next('.help-block').hide();
                    }
                }
                if(value){
                    value = this.getValue();
                    if(!value){
                        this._setError();
                        return false;
                    }
                }
                return true;
            },
            getValue: function(){
                var value = this.element.val();
                if(value){
                    return value + "T00:00:00";
                }
                return null;
            },
            setValue: function(value){
                this.element.val($.formatISODate(value));
            },
            getText: function(){
                var value = this.getValue();
                return $.formatISODate(value);
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.textInput", $.ui.coldewControl, {
            options: {
                required: false,
                suggestions: null,
                name: null
	        },
	        _createControl: function(){
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

    $.widget("ui.textarea", $.ui.coldewControl, {
            options: {
                required: false,
                name: null
	        },
	        _createControl: function(){
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
	        },
            getText: function(){
                var value = this.getValue();
                return value.replace(/\n/g, "</br>");
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.select", $.ui.coldewControl, {
            options: {
                required: false,
                name: null,
                selectList: null
	        },
	        _createControl: function(){
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
    $.widget("ui.radioList", $.ui.coldewControl, {
            options: {
                required: false,
                name: null,
                selectList: null
	        },
	        _createControl: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                $.each(this.options.selectList, function(){
                    var radio = $("<input type='radio'/>")
                        .attr("name", thiz.options.name)
                        .attr("value", this);
                    if(thiz.options.required){
                        radio.data("required", true);
                    }
                    $("<label class='radio-inline'></label>")
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
                this._setText();
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.checkboxList", $.ui.coldewControl, {
            options: {
                required: false,
                name: null,
                selectList: null
	        },
	        _createControl: function(){
                var thiz = this;
                if(this.options.name){
                    this.element.attr("name", this.options.name);
                }
                else{
                    this.options.name = this.element.attr("name");
                }
                $.each(this.options.selectList, function(){
                    var radio = $("<input type='checkbox'/>")
                        .attr("name", thiz.options.name)
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
                this._setText();
            },
            getText: function(readonly){
                var value = this.getValue();
                if(value){
                    return value.toString();
                }
                return "";
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.numberInput", $.ui.coldewControl, {
            options: {
                required: false,
                name: null,
                max: null,
                min: null,
                precision: 2
	        },
	        _createControl: function(){
                
                if(this.options.precision > 0){
                    this._numberRegex = new RegExp("^[-,+]?[0-9]+(.[0-9]{0,${0}})?$".format(this.options.precision));
                }
                else{
                    this._numberRegex = new RegExp("^[-,+]?[0-9]*[1-9][0-9]*$");
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
                        this.element.closest('.form-group').removeClass('has-error');
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
                this._setText();
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.metadataSelect", $.ui.coldewControl, {
            options: {
                required: false,
                name: null,
                objectId: null,
                objectName: null
	        },
	        _createControl: function(){
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
                if(value){
                    this.element.find("input").val(value.name);
                }
                this._setText();
            },
            getText: function(){
                var value = this.getValue();
                if(value){
                    return value.name;
                }
                return "";
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.userSelect", $.ui.coldewControl, {
            options: {
                required: false,
                name: null,
                single: null,
                defaultValue: null
	        },
	        _createControl: function(){
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
                if(value){
                    this.element.find("input").val(value.name);
                }
            },
            getText: function(){
                var value = this.getValue();
                if(value){
                    return value.name;
                }
                return "";
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.userListSelect", $.ui.coldewControl, {
            options: {
                required: false,
                name: null
	        },
	        _createControl: function(){
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
                if(value){
                    this.element.find("input").val(this.getText());
                }
                this._setText();
            },
            getText: function(){
                var value = this.getValue();
                if(value){
                    var nameArray = $.map(value, function(user){
                        return user.name;
                    });
                    return nameArray.toString();
                }
                return "";
            }
        }
    );
})(jQuery);

(function($){
    $.widget("ui.numberRangeInput", $.ui.coldewControl, {
            options: {
                name: null
	        },
	        _create: function(){
                var thiz = this;
                this._minInput = this.element.find("input").eq(0);
                this._maxInput = this.element.find("input").eq(1);
	        },
            getValue: function(){
                var min = this._minInput.val();
                var max = this._maxInput.val();
                var value = {};
                value.min = this._parseFloat(min);
                value.max = this._parseFloat(max);
                return value;
            },
            setValue: function(value){
                this.element.setFormValue(value);
                this._setText();
            },
            _parseFloat: function(value){
                value = parseFloat(value);
                if(!value){
                    return null;
                }
                else{
                    return value;
                }
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
    $.widget("ui.dateRangeInput", {
            options: {
                name: null
	        },
            _create: function(){
                this._startInput = this.element.find("input").eq(0);
                this._endInput = this.element.find("input").eq(1);
                this.element.find("input").datepicker();
	        },
            setReadonly: function(readonly){
                this.element.find("input").datepicker("option", "disabled", readonly);
            },
            _setError: function(){
                this.element.focus();
                this.element.closest('.form-group').addClass('has-error');
                this.element.next('.help-block').show();
            },
            getValue: function(){
                var value = {};
                value.start = this._startInput.val();
                value.end = this._endInput.val();
                if(value.start){
                    value.start += "T00:00:00";
                }
                else{
                    value.start = null;
                }
                if(value.end){
                    value.end += "T00:00:00";
                }
                else{
                    value.end = null;
                }
                return value;
            },
            setValue: function(value){
                this._startInput.val($.formatISODate(value.start));
                this._endInput.val($.formatISODate(value.end));
            }
        }
    );


})(jQuery);

(function($){
    $.widget("ui.label", {
            options: {
                name: null
	        },
	        _create: function(){
                
	        },
            setValue: function(value){
                this.element.text(value);
            }
        }
    );
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
    $.widget("ui.codeInput", $.ui.coldewControl, {
            options: {
                required: false,
                defaultValue: null,
                name: null
	        },
	        _createControl: function(){
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