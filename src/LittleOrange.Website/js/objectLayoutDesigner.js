(function($){
    var sectionSettingDialog;
    var designerTemplates; 

    $.widget("ui.objectLayoutDesigner", {
            options: {
                model: null,
                designerTemplates: designerTemplates
	        },
	        _create: function(){
                designerTemplates = this.options.designerTemplates;
	            var element = this.element;
                this._renderComponent();
                this._renderFormDesigner();
                this._sectionPropetyModel = $("#section-propetyModel").sectionPropetyModel();
	        },
            
            _renderComponent: function(){
                var rowTemplate = "<tr><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>";
                var componentTable = $("<table></table>")
                    .addClass("table table-bordered")
                    .appendTo($("#components"));
                //section
                var sectionComponent = $("<button class='btn btn-small'>表单区域</button>")
                    .addClass("sectionComponent")
                    .draggable({
                        cancel: "",
                        helper: "clone",
                        revert: "invalid",
                    })
                    .data("columnCount", 1);
                var lastRow = $(rowTemplate).appendTo(componentTable);
                lastRow.find("td").eq(0).append(sectionComponent);
                //field
                var componentCount = 1;
                $.each(this.options.model.fields, function(i){
                    if(componentCount % 10 == 0){
                        lastRow = $(rowTemplate).appendTo(componentTable);
                    }
                    var component = $("<button class='btn btn-small'>"+this.name+"</button>")
                        .addClass("inputComponent")
                        .draggable({
                            cancel: "",
                            helper: "clone",
                            revert: "invalid",
                        })
                        .data("field", this);
                    lastRow.find("td").eq(componentCount % 10).append(component);
                    componentCount++;
                });
            },
            _renderFormDesigner: function(){
                var thiz = this;
                var element = this._formDesigner = $("#formDesigner");
                var sections = this.options.model.sections;

                $.each(sections, function(i){
                    element.append(thiz._createSection(this));
                });

	        	element.sortable({
	        			placeholder: "sortable-placeholder"
	        		})
	       	  		.droppable({
	       	  			accept: ".sectionComponent", 
	       	  			activeClass: "droppable-active",
	       	  			hoverClass: "droppable-hover",
	       	  			drop: function(event, ui){
                            var columnCount = ui.draggable.data("columnCount");
		       	  			var section = thiz._createSection({name: "表单区域", columnCount: columnCount});
		           			$(this).append(section);
	               		}
	               });
            },
            _getSectionControls: function(section){
                var columns = section.find(".section-clomn");
                var columnLength = columns.length;
                var controls = [];

                columns.each(function(columnIndex){
                    $(this).find(".control-group").each(function(controlIndex){
                        var index = controlIndex * columnLength - columnIndex;
                        if(columnIndex > 0){
                            index += columnLength;
                        }
                        controls[index] = $(this);
                    });
                });
                return controls;
            },
            _createSection: function(section){
                var thiz = this;
                var fieldset = $("<fieldset class='form-horizontal section'><legend class='designer-element-draggable'><label class='section-name'></label></legend></fieldset>");
                fieldset.data("section", section);
                var legend = fieldset.find("legend")
                    .dblclick(function(){
                        var section = fieldset.data("section");
                        thiz._sectionPropetyModel.sectionPropetyModel("edit", section, function(value){
                            fieldset.data("section", value);
                            legend.find(".section-name").text(value.name);
                            if(value.columnCount != section.columnCount){
                                var controls = thiz._getSectionControls(fieldset);
                                var sectionRow = thiz._createSectionRow(value.columnCount);
                                var columns = sectionRow.find(".section-clomn");
                                $.each(controls, function(i){
                                    if($(this).hasClass("control-group")){
                                        var column = columns.eq(i % columns.length);
                                        column.append($(this));
                                    }
                                });
                                fieldset.find(".row-fluid").remove();
                                fieldset.append(sectionRow);
                            }
                        });
                    });
                var btnClose = $("<button type='button' style='display: none; margin-right: 5px;' class='close' >&times;</button>").prependTo(legend);
                legend.find(".section-name").text(section.name);
                
                var sectionRow = this._createSectionRow(section.columnCount);
                fieldset.append(sectionRow);
                var columns = sectionRow.find(".section-clomn");
                if(section.fields){
                    $.each(section.fields, function(i){
		                var column = columns.eq(i % columns.length);
                        var control = thiz._createControl(this);
                        thiz._disableInputComponent(this);
                        column.append(control);
                    });
                }
                
                btnClose.click(function(){
                    fieldset.find(".control-group").each(function(){
                        var field = $(this).data("field");
                        thiz._enableInputComponent(field);
                    });
                    fieldset.remove();
                });
                legend.hover(
                    function(){
                        btnClose.show();
                    }, function(){
                        btnClose.hide();
                    });

                return fieldset;
            },
            _createSectionRow: function(columnCount){
                var thiz = this;
                var row = $("<div class='row-fluid'></div>");
                var columnSpanClass = "span" + (12 / columnCount);
                
                for(var i = 0; i < columnCount; i++){
                    $("<div class='section-clomn' style='min-height:50px;'></div>").addClass(columnSpanClass).appendTo(row);
                }

	            var columns = row.find(".section-clomn")
	        		.sortable({
	        			connectWith: ".section-clomn",
	        			placeholder: "sortable-placeholder"
	        		})
	       	  		.droppable({
	       	  			accept: ".inputComponent", 
	       	  			activeClass: "droppable-active",
	       	  			hoverClass: "droppable-hover",
	       	  			drop: function(event, ui){
                            var field = ui.draggable.data("field");
                            var control = thiz._createControl(field);
                            $(this).append(control);
                            ui.draggable.prop("disabled", true);
	               		}
	               });
                return row;
            },
            _createControl: function (field) {
                var thiz = this;
                var group = $("<div class='control-group designer-element-draggable'><button type='button' style='display: none; margin-right: 5px;' class='close' >&times;</button><label class='control-label' ></label><div class='controls'><label style='padding-top: 5px;' class='control-value' ></label></div></div>");
                group.find(".control-label").text(field.name);
                group.find(".control-value").text(field.name + " 值");
                group.data("field", field);
                var btnClose = group.find(".close");
                btnClose.click(function(){
                    group.remove();
                    thiz._enableInputComponent(field)
                });
                group.hover(
                    function(){
                        btnClose.show();
                    }, function(){
                        btnClose.hide();
                    });
                return group;
            },
            _disableInputComponent: function(field){
                $(".inputComponent").each(function(){
                    var thisField = $(this).data("field");
                    if(thisField.code == field.code){
                        $(this).addClass("disabled")
                            .draggable("disable");
                    }
                });
            },
            _enableInputComponent: function(field){
                $(".inputComponent").each(function(){
                    var thisField = $(this).data("field");
                    if(thisField.code == field.code){
                        $(this).removeClass("disabled")
                            .draggable("enable");
                    }
                });
            },
            getSections: function(){
                var thiz = this;
                var sections = [];
                this._formDesigner.find(".section").each(function(i){
                    var sectionData = $(this).data("section");
                    var sectionControls = thiz._getSectionControls($(this));
                    var fields = [];
                    $.each(sectionControls, function(){
                        if($(this).hasClass("control-group")){
                            fields[fields.length] = $(this).data("field").code;
                        }
                    });
                    sectionData.fields = fields;
                    sections[sections.length] = sectionData;
                })  
                return sections;
            }
        }
    );

    $.widget("ui.sectionPropetyModel", {
            options: {
                
	        },
	        _create: function(){
                var thiz = this;
                this.element.find("form").validate({
                    sendForm : false,
                    onBlur: true,
                    onChange: true,
	                eachValidField : function() {
		                $(this).closest('.control-group').removeClass('error');
                        $(this).next('.help-inline').hide();
	                },
	                eachInvalidField : function() {
		                $(this).closest('.control-group').addClass('error');
                        $(this).next('.help-inline').show();
	                },
                    valid: function(){
                        var formValue = thiz.element.find("form").getFormValue();
                        thiz.element.modal("hide");
                        thiz._editedCallback(formValue);
                    }
                });
                
	        },
            edit: function(property, callback){
                this._editedCallback = callback;
                this.element.modal("show");
                this.element.find("form").setFormValue(property);
            }
        }
    );
})(jQuery);