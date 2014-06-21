(function($){
    $.widget("ui.shoukuanEditDialog", {
            options: {
                
	        },
	        _create: function(){
                var thiz = this;
                this._modal = this.element.find(".modal");
                this._modal.css({display: "block" });
                var detailsForm = this._detailsForm = this.element.find(".shoukuanDetails").coldewForm({sections: shoukuanModel.sections}).data("coldewForm");
                this._modal.css({display: "" });
                this.element.find(".shoukuanDetails").find("fieldset").remove();
                var shoukuanRiqiInput = detailsForm.getInput("shoukuanRiqi");
                var shoukuanJineInput = detailsForm.getInput("shoukuanJine");
                var tichengInput = detailsForm.getInput("ticheng");
                function jisuanTicheng(){
                    if(!detailsForm.validate()){
                        return;
                    }
                    var formValue = detailsForm.getValue();
                    var shoukuanJson = $.toJSON(formValue);
                    $.post($.resolveUrl("ShoukuanGuanli/JisuanTicheng"), 
                        {objectId: objectId, metadataId: metadataId, shoukuanJson: shoukuanJson}, 
                        function(model){
                            if(model.result == 0){
                                tichengInput.setValue(model.data);
                            }
                            else{
                                alert(model.message);
                            }
                        }
                    );
                }
                shoukuanJineInput.inputing(jisuanTicheng, 500);
                shoukuanRiqiInput.inputing(jisuanTicheng, 500);
                this.element.find(".btnSave").click(function(){
                    if(detailsForm.validate()){
                        var formValue = detailsForm.getValue();
                        thiz._modal.modal("hide");
                        thiz._editedCb(formValue);
                        detailsForm.reset();
                    }
                    return false;
                });
	        },
            edit: function(editedCb, initInfo){
                var thiz = this;
                thiz._editedCb = editedCb;
                this._modal.modal("show");
                if(initInfo){
                    this._detailsForm.setValue(initInfo);
                }
            }
        }
    );
})(jQuery);


(function($){
    $.widget("ui.shoukuanGrid", $.webui.input, {
            options: {
                name: null,
                shoukuanEditDialog: null,
                required: false
	        },
	        _create: function(){
                var shoukuanEditDialog = this.options.shoukuanEditDialog.data("shoukuanEditDialog");
                var toolbar = 
                    "<div class='btn-group'>"+
                        "<button class='btn btn-default'>添加</button>"+
                        "<button disabled='disabled' class='btn btn-default'>编辑</button> "+
                        "<button disabled='disabled' class='btn btn-default'>删除</button> "+
                    "</div>";
                this._toolbar = $(toolbar).appendTo(this.element);
                var buttons = this._toolbar.find("button");
                var btnAddShoukuan = buttons.eq(0)
                    .click(function(){
                        shoukuanEditDialog.edit(function(formValue){
                            shoukuanGrid.datagrid("appendRow", formValue);
                        });
                        return false;
                    });
                var btnEditShoukuan = buttons.eq(1)
                    .click(function(){
                        var row = shoukuanGrid.datagrid("getSelectedRow");
                        var editInfo = row.datarow("getValue");
                        shoukuanEditDialog.edit(function(formValue){
                            row.datarow("setValue", formValue);
                        }, editInfo);
                        return false;
                    });
                var btnDeleteShoukuan = buttons.eq(2)
                    .click(function(){
                        if(confirm("确实要删除吗?")){
                            shoukuanGrid.datagrid("deleteSelectedRows");
                            btnEditShoukuan.prop("disabled", true);
                            btnDeleteShoukuan.prop("disabled", true);
                        }
                        return false;
                    });

                var shoukuanGrid = this._shoukuanGrid = $("<div></div>")
                    .appendTo(this.element)
                    .datagrid({
                        columns:[
                            {
                                title: "收款日期", width: 150, field:"shoukuanRiqi", name:"shoukuanRiqi",
                                render: function(event, args){
                                    return $.formatDate(args.value);
                                }
                            },
			                {title: "收款金额", width: 120, field:"shoukuanJine", name:"shoukuanJine"},
			                {title: "提成", width: 120, field:"ticheng", name:"ticheng"},
			                {title: "备注", width: 200, field:"beizhu"}
		                ],
		                canSort: false,
		                singleSelect: true,
		                showNumberRow: false,
                        selectedRow: function(){
                            btnEditShoukuan.prop("disabled", false);
                            btnDeleteShoukuan.prop("disabled", false);
                        },
                        unselectedRow: function(){
                            btnEditShoukuan.prop("disabled", true);
                            btnDeleteShoukuan.prop("disabled", true);
                        },
                        footer: [{columnName: "shoukuanRiqi", valueType: "fixed", value: "合计"}, {columnName: "shoukuanJine", valueType: "sum"}, {columnName: "ticheng", valueType: "sum"}]
                    });
	        },
            getValue: function(){
                return this._shoukuanGrid.datagrid("getRowsData");
            },
            setValue: function(value){
                this._shoukuanGrid.datagrid("setValue", value);
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