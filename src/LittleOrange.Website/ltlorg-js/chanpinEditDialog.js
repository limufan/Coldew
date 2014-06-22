(function($){
    $.widget("ui.chanpinEditDialog", {
            options: {
                
	        },
	        _create: function(){
                var thiz = this;
                this._modal = this.element.find(".modal");
                this._modal.css({display: "block" });
                var detailsForm = this._detailsForm = this.element.find(".chanpinDetails").coldewForm({controls: chanpinModel.controls}).data("coldewForm");
                this._modal.css({display: "" });
                var nameInput = detailsForm.getInput("name");
                var zongjineInput = detailsForm.getInput("zongjine");
                var shuliangInput = detailsForm.getInput("shuliang");
                var yewulvInput = detailsForm.getInput("yewulv");
                var yewulvFangshiInput = detailsForm.getInput("yewulvFangshi");
                var yewufeiInput = detailsForm.getInput("yewufei");
                var xiaoshouDanjiaInput = detailsForm.getInput("xiaoshouDanjia");
                var zongjineInput = detailsForm.getInput("zongjine");
                var shijiDanjiaInput = detailsForm.getInput("shijiDanjia");
                var butieInput = detailsForm.getInput("butie");
                this.element.find(".chanpinDetails").find("legend").remove();
                nameInput.element
                    .metadataAutoComplete({objectCode: "chanpin", select: function(event, chanpin){
                        detailsForm.setValue({guige: chanpin.guige, danwei: chanpin.danwei, xiaoshouDijia: chanpin.xiaoshouDijia });
                    }});
                function jisuanYewufei(){
                    var yewulvFangshi = yewulvFangshiInput.getValue();
                    if(yewulvFangshi == "按金额"){
                        var yewulv = yewulvInput.getValue();
                        var zongjine = zongjineInput.getValue();
                        yewufeiInput.setValue(yewulv * zongjine);
                    }
                    else if(yewulvFangshi == "按重量"){
                        var yewulv = yewulvInput.getValue();
                        var shuliang = shuliangInput.getValue();
                        yewufeiInput.setValue(yewulv * shuliang);
                    }
                }
                function jisuanZongjine(){
                    var shuliang = shuliangInput.getValue();
                    var danjia = xiaoshouDanjiaInput.getValue();
                    zongjineInput.setValue(shuliang * danjia);
                }
                function jisuanShijiDanjia(){
                    var zongjine = zongjineInput.getValue();
                    var yewufei = yewufeiInput.getValue();
                    var shuliang = shuliangInput.getValue();
                    shijiDanjiaInput.setValue((zongjine-yewufei) / shuliang * 0.83);
                }
                function jisuanButie(){
                    var shijiDanjia = shijiDanjiaInput.getValue();
                    var shuliang = shuliangInput.getValue();
                    butieInput.setValue(shijiDanjia * shuliang * 0.01);
                }
                detailsForm.inputing(function(){
                    jisuanZongjine();
                    jisuanYewufei();
                    jisuanShijiDanjia();
                    jisuanButie();
                });
                detailsForm.changed(function(){
                    jisuanZongjine();
                    jisuanYewufei();
                    jisuanShijiDanjia();
                    jisuanButie();
                });

                this.element.find(".btnSave").click(function(){
                    try{
                    if(detailsForm.validate()){
                        var formValue = detailsForm.getValue();
                        thiz._modal.modal("hide");
                        thiz._editedCb(formValue);
                        detailsForm.reset();
                    }
                        }catch(e){
                            alert(e)
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
    $.widget("ui.chanpinGrid", {
            options: {
                name: null,
                chanpinAddDialog: null,
                chanpinEditDialog: null,
                required: false,
                xianshiTicheng: false
	        },
	        _create: function(){
                var chanpinAddDialog = this.options.chanpinAddDialog;
                var chanpinEditDialog = this.options.chanpinEditDialog;
                var toolbar = 
                    "<div class='btn-group'>"+
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
                        }, {yewulv: selectKehu.yewulv, yewulvFangshi: selectKehu.yewulvFangshi});
                        return false;
                    });
                var btnEditChanpin = buttons.eq(1)
                    .click(function(){
                        var row = chanpinGrid.datagrid("getSelectedRow");
                        var editInfo = row.datarow("getValue");
                        chanpinEditDialog.chanpinEditDialog("edit", function(formValue){
                            row.datarow("setValue", formValue);
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
               var columns = [
			                {title: "产品名称", width: 100, field:"name"},
			                {title: "规格", width: 80, field:"guige"},
			                {title: "单位", width: 60, field:"danwei"},
			                {title: "数量", width: 60, field:"shuliang"},
			                {title: "桶数", width: 60, field:"tongshu"},
			                {title: "销售底价", width: 80, field:"xiaoshouDijia"},
			                {title: "单价", width: 60, field:"xiaoshouDanjia", name:"xiaoshouDanjia"},
			                {title: "实际单价", width: 80, field:"shijiDanjia", name:"shijiDanjia"},
			                {title: "金额", width: 80, field:"zongjine", name:"zongjine"},
			                {title: "业务率", width: 60, field:"yewulv"},
			                {title: "业务费", width: 70, field:"yewufei", name:"yewufei"},
			                {title: "是否开票", width: 70, field:"shifouKaipiao"}
		                ];
                
                var footer = [
                            {columnName: "xiaoshouDanjia", valueType: "fixed", value: "合计"}, 
                            {columnName: "zongjine", valueType: "sum"},
                            {columnName: "yewufei", valueType: "sum"}
                        ]
                if(this.options.xianshiTicheng){
                    columns.push({title: "收款金额", width: 70, field:"shoukuanJine", name:"shoukuanJine"});
                    columns.push({title: "提成", width: 70, field:"ticheng", name:"ticheng"});
                    footer.push({columnName: "shoukuanJine", valueType: "sum"});
                    footer.push({columnName: "ticheng", valueType: "sum"});
                }
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