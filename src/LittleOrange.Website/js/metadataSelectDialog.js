(function($){
    $.widget("ui.metadataSelectDialog", {
            options: {
                singleSelect: true
	        },
	        _create: function(){
                var thiz = this;
                thiz._init();
	        },
            _init: function(){
                var thiz = this;
                this._dataGetUrl = $.baseUrl + "Metadata/SelectMetadatas";
                this._modal = this.element;
                this._modal.modal({keyboard: false, show: false});
                this._headerName = this.element.find(".header-name");
                this._btnSelect = this.element.find(".btnSelect").click(function(){
                    var row = thiz._metadataGrid.datagrid("getSelectedRow");
                    var id = row.datarow("getValue").id;
                    var name = row.datarow("getValue").name;
                    var result = thiz._selectCallback({id: id, name: name});
                    if(result == undefined || result == true){
                        thiz._modal.modal("hide");
                    }
                    return false;
                });
                $(".btnKeywordSearch").click(function(){
                    var btn = $(this).button("loading");
                    thiz.loadMetadataGrid(0, function(){
                        btn.button("reset");
                    });
                    return false;
                });
                this._txtKeyword = this.element.find(".txtKeyword");
                this._metadataGrid = this.element.find(".metadataGrid").datagrid({
                    columns: [{ title: "名称", width: 150, field: "name" }, { title: "摘要", width: 250, field: "summary" }],
                    height: 280,
		            canSort: false,
		            singleSelect: thiz.options.singleSelect,
		            showNumberRow: true,
                    selectedRow: function(){
                        thiz._btnSelect.prop("disabled", false);
                    },
                    unselectedRow: function(){
                        var rows = thiz._metadataGrid.datagrid("getSelectedRows");
                        thiz._btnSelect.prop("disabled", rows.length == 0);
                    }
	            });
                this._metadataPager = this.element.find(".metadataPager")
                  .pager({change: function(event, args){
                      thiz.loadMetadataGrid(args.start);
                  }});
            },
            select: function(formId, formName, callback){
                this._headerName.text(formName);
                this._selectCallback = callback;
                this._formId = formId;
                var pageInfo = this._metadataPager.pager("option").pageInfo;
                var start = pageInfo.start || 0;
                this.loadMetadataGrid(start);
                this._modal.modal("show");
            },
            loadMetadataGrid: function(start, cb){
                var thiz = this;
                var args = {objectId: this._formId, start: start, size: 20, keyword: this._txtKeyword.val()};
                $.get(this._dataGetUrl, args, function(model){
                    if(cb){
                        cb();
                    }
                    if(model.result == 0){
                        thiz._metadataGrid.datagrid("setValue", model.data.list);
                        thiz._metadataPager.pager("option", "pageInfo", {start: start, size: 20, count: model.data.count})
                        thiz._btnSelect.prop("disabled", true);
                    }
                    else{
                        alert(model.message)
                    }
                });
            }
        }
    );
})(jQuery);
$(function(){
    $.get($.baseUrl + "Metadata/SelectDialog", null, function(html){
        $.metadataSelectDialog = $(html).metadataSelectDialog().appendTo(document.body);
    });
});