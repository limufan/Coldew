(function($){
    $.widget("ui.chanpinEditDialog", {
            options: {
                
	        },
	        _create: function(){
                var thiz = this;
                this._modal = this.element.find(".modal");
                this._form = this.element.find("form").eq(0);
                this.element.find(".btnSaveAndContinue").click(function(){

                });
                this.element.find(".btnSave").click(function(){
                    thiz._modal.modal("hide");
                });
                this._form.validate({
                    sendForm : false,
                    onBlur: true,
                    onChange: true,
				    eachValidField : function() {
					    $(this).closest('.form-group').removeClass('error');
				    },
				    eachInvalidField : function() {
					    $(this).closest('.form-group').addClass('error');
				    },
                    valid : function(event, options) {
                        var formValue = thiz._form.getFormValue();
                        thiz._editedCb(formValue);
                        thiz._form[0].reset();
                    }
                });
	        },
            edit: function(editedCb, initInfo){
                var thiz = this;
                thiz._editedCb = editedCb;
                this._modal.modal("show");
                if(initInfo){
                    thiz._form.setFormValue(initInfo);
                }
            }
        }
    );
})(jQuery);


(function($){
    $.widget("ui.chanpinGrid", $.ui.coldewControl, {
            options: {
                name: null,
                chanpinAddDialog: null,
                chanpinEditDialog: null,
                required: false
	        },
	        _create: function(){
                var chanpinAddDialog = this.options.chanpinAddDialog;
                var chanpinEditDialog = this.options.chanpinEditDialog;
                var toolbar = 
                    "<div class='btn-toolbar'>"+
                        "<button class='btn btn-default'>添加</button>"+
                        "<button disabled='disabled' class='btn btn-default'>编辑</button> "+
                        "<button disabled='disabled' class='btn btn-default'>删除</button> "+
                    "</div>";
                this._toolbar = $(toolbar).appendTo(this.element);
                var buttons = this._toolbar.find("button");
                var btnAddChanpin = buttons.eq(0)
                    .click(function(){
                        chanpinAddDialog.chanpinEditDialog("edit", function(formValue){
                            chanpinGrid.datagrid("appendRow", formValue);
                        });
                        return false;
                    });
                var btnEditChanpin = buttons.eq(1)
                    .click(function(){
                        var row = chanpinGrid.datagrid("getSelectedRow");
                        var editInfo = row.datarow("option", "data");
                        chanpinEditDialog.chanpinEditDialog("edit", function(formValue){
                            row.datarow("option", "data", formValue);
                        }, editInfo);
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

                var chanpinGrid = this._chanpinGrid = $("<div></div>").datagrid({
                    columns:[
			            {title: "产品名称", width: 150, field:"name"},
			            {title: "规格", width: 50, field:"guige"},
			            {title: "单位", width: 50, field:"danwei"},
			            {title: "数量", width: 50, field:"shuliang"},
			            {title: "桶数", width: 50, field:"tongshu"},
			            {title: "单价", width: 80, field:"xiaoshouDanjia"},
			            {title: "金额", width: 50, field:"zongjine"},
			            {title: "业务率", width: 60, field:"yewulv"},
			            {title: "业务费", width: 60, field:"yewufei"},
			            {title: "是否开票", width: 80, field:"shifouKaipiao"}
		            ],
		            canSort: false,
		            singleSelect: true,
		            showNumberRow: false,
                    height: 150,
                    selectedRow: function(){
                        btnEditChanpin.prop("disabled", false);
                        btnDeleteChanpin.prop("disabled", false);
                    },
                    unselectedRow: function(){
                        btnEditChanpin.prop("disabled", true);
                        btnDeleteChanpin.prop("disabled", true);
                    }
                })
                .appendTo(this.element);
	        },
            getValue: function(){
                return this._chanpinGrid.datagrid("getRowsData");
            },
            setValue: function(value){
                this._chanpinGrid.datagrid("option", "data", value);
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