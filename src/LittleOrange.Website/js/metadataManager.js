(function($){
    var objectPermissionValue = {
        None: 0,
        View: 1,
        Create: 1 << 1,
        Export: 1 << 2,
        Import: 1 << 3,
        PermissionSetting: 1 << 4
    };
    objectPermissionValue.All =
        objectPermissionValue.View | objectPermissionValue.Create | objectPermissionValue.Export 
            | objectPermissionValue.Import | objectPermissionValue.PermissionSetting;

    $.widget("ui.metadataManager", {
            options: {
                objectId: null,
                viewId: null,
                columns: null,
                fields: null,
                nameField: null
	        },
	        _create: function(){
                var thiz = this;
                this._createLeftMenus();
                this._initToolbar();
                this._createDatagrid();
                this._createPager();
                this._bindEdit();
                this._bindDelete();
                this._bindExport();
                this._bindFavorite();
                this._bindRefresh();
                this._bindSearch();
                this.loadMetadataGrid(null, 0);
	        },
            _createLeftMenus: function(){
                var thiz = this;
                var objectId = this.options.objectId;
                var container = $("#leftMenuGroup");
                $.each(this.options.menus, function(){
                    var url = $.resolveUrl("Metadata/Index", {objectId: objectId, viewId: this.viewId});
                    var menu = $("<a class='list-group-item'></a>").attr("href", url).text(this.name).appendTo(container);
                    if(thiz.options.viewId == this.viewId){
                        menu.addClass("active");
                    }
                });
            },
            _initToolbar: function(){
                var perm = this.options.permission;
                if((perm & objectPermissionValue.Create) == objectPermissionValue.Create){
                    $("#btnCreate").show();
                }
                else{
                    $("#btnCreate").hide();
                }
                if((perm & objectPermissionValue.Import) == objectPermissionValue.Import){
                    $("#btnImport").show();
                }
                else{
                    $("#btnImport").hide();
                }
                if((perm & objectPermissionValue.Export) == objectPermissionValue.Export){
                    $("#btnExport").show();
                }
                else{
                    $("#btnExport").hide();
                }
            },
            _createDatagrid: function(){
                var thiz = this;
                var columns = this.options.columns;
                columns.splice(0, 0, {field: "favorited", title: "", width: 30, render: function(ui, args){
                    if(args.value){
                        return '<span class="glyphicon glyphicon-star"></span>';
                    }
                    else{
                        return '<span class="glyphicon glyphicon-star-empty"></span>';
                    }
                }});

                $.each(columns, function(i, column){
                    if(column.field == thiz.options.nameField){
                        column.render = function(ui, args){
                            var params = {};
                            params.metadataId = args.data.id;
                            params.objectId = thiz.options.objectId;
                            var url = $.resolveUrl("Metadata/Details", params);
                            return "<a href='${0}' target='_blank' >${1}</a>".format(url, args.value);
                        }
                        return false;
                    }
                });

                this._metadataGrid = $("#metadataGrid").datagrid({
		            columns: columns,
                    height: "auto",
		            canSort: true,
		            singleSelect: false,
		            showNumberRow: true,
                    selectedRow: function(){
                        thiz.refreshToolbar();
                    },
                    unselectedRow: function(){
                        thiz.refreshToolbar();
                    },
                    sort: function(sender, args){
                        var pageInfo = thiz._pager.pager("option").pageInfo;
                        thiz.loadMetadataGrid(thiz._searchInfo, pageInfo.start, null, args);
                        return false;
                    }
	            });
            },
            _createPager: function(){
                var thiz = this;
                thiz._pager = $("#pager").pager({change: function(event, args){
                    thiz.loadMetadataGrid(thiz._searchInfo, args.start);
                }});
            },
            refreshToolbar: function(){
                var rows = this._metadataGrid.datagrid("getSelectedRows");
                $("#btnDelete, #btnFavorite").prop("disabled", false);
                if(rows.length > 0){
                    if(rows.length == 1 && rows[0].datarow("getValue").canModify){
                        $("#btnEdit").show();
                    }
                    else{
                        $("#btnEdit").hide();
                    }
            
                    var hasCannotDeleteRow = false;
                    $.each(rows, function(i, row){
                        if(!row.datarow("getValue").canDelete){
                            hasCannotDeleteRow = true;
                            return false;
                        }
                    })
                    if(hasCannotDeleteRow){
                        $("#btnDelete").hide();
                    }
                    else{
                        $("#btnDelete").show();
                    }

                    $("#btnFavorite").show();   
                }
                else{
                    $("#btnEdit, #btnDelete, #btnFavorite").hide();
                }
            },
            loadMetadataGrid: function(searchInfo, start, cb, orderBy){
                var thiz = this;
                this._searchInfo = searchInfo;
                
                var args = {objectId: this.options.objectId, viewId: this.options.viewId, start: start, size: 20, orderBy: orderBy};
                if(this._searchInfo){
                    var searchInfoJson = $.toJSON(this._searchInfo);
                    args.searchInfoJson = searchInfoJson;
                }
                
                $.get($.resolveUrl("Metadata/Metadatas"), args, function(model){
                    if(cb){
                        cb();
                    }
                    if(model.result == 0){
                        thiz._metadataGrid.datagrid("setValue", model.data.list);
                        if(model.data.footer){
                            thiz._metadataGrid.datagrid("setFooter", model.data.footer);
                        }
                        thiz._pager.pager("option", "pageInfo", {start: start, size: 20, count: model.data.count})
                        thiz.refreshToolbar();
                    }
                    else{
                        alert(model.message)
                    }
                });
            },
            _bindSearch: function(){
                var thiz = this;
                $("#btnSearch").click(function(){
                    $("#btnSearch").button("loading");
                    var keyword = $("#txtKeyword").val();
                    thiz.loadMetadataGrid({keyword: keyword}, 0, function(){
                        $("#btnSearch").button("reset");
                    });
                    return false;
                });

                var searchPopover = $("#searchPopover").metadataSearchPopover({fields: this.options.fields});
                var btnPopoverSearch = $("#btnPopoverSearch").click(function(event){
                    searchPopover.metadataSearchPopover("search", btnPopoverSearch, function(searchInfo){
                        thiz.loadMetadataGrid(searchInfo, 0);
                    });
                    event.stopPropagation();
                    return false;
                });
            },
            _bindRefresh: function(){
                var thiz = this;
                $("#btnRefresh").click(function(){
                    var pageInfo = thiz._pager.pager("option").pageInfo;
                    $("#btnRefresh").button("loading");
                    thiz.loadMetadataGrid(thiz._searchInfo, pageInfo.start, function(){
                        $("#btnRefresh").button("reset");
                    });
                    return false;
                });
            },
            _bindExport: function(){
                var thiz = this;
                $("#btnExport").click(function(){
                    $("#btnExport").button("loading");
                    var searchInfoJson = "";
                    if(thiz._searchInfo){
                        searchInfoJson = $.toJSON(thiz._searchInfo);
                    }
                    $.get($.resolveUrl("Metadata/Export"), {objectId: thiz.options.objectId, viewId: thiz.options.viewId, searchInfoJson: searchInfoJson}, function(model){
                        $("#btnExport").button("reset");
                        if(model.result == 0){
                            open($.resolveUrl("Metadata/DownloadExportFile", {objectId: thiz.options.objectId, fileName: model.data}));
                        }
                        else{
                            alert(model.message);
                        }
                    });
                    return false;
                });
            },
            _bindFavorite: function(){
                var thiz = this;
                $("#btnFavorite").click(function(){
                    var rows = thiz._metadataGrid.datagrid("getSelectedRows");
                    var metadataIds = $.map(rows, function(row){
                        return row.datarow("getValue").id;
                    })
                    $("#btnFavorite").button("loading");
                    $.post($.resolveUrl("Metadata/Favorites"), {objectId: thiz.options.objectId, metadataIdsJson: $.toJSON(metadataIds)}, function(model){
                        $("#btnFavorite").button("reset");
                        if(model.result == 0){
                            var pageInfo = thiz._pager.pager("option").pageInfo;
                            thiz.loadMetadataGrid(thiz._searchInfo, pageInfo.start);
                        }
                        else{
                            alert(model.message)
                        }
                    });
                    return false;
                })
            },
            _bindEdit: function(){
                var thiz = this;
                $("#btnEdit").click(function(){
                    var row = thiz._metadataGrid.datagrid("getSelectedRow");
                    var editUrl = $.resolveUrl("Metadata/Edit", {objectId: thiz.options.objectId, viewId: thiz.options.viewId, metadataId: row.datarow("getValue").id});
                    location = editUrl;
                    return false;
                });
            },
            _bindDelete: function(){
                var thiz = this;
                $("#btnDelete").click(function(){
                    if(!confirm("确实要删除吗?"))
                    {
                        return false;
                    }
                    var rows = thiz._metadataGrid.datagrid("getSelectedRows");
                    var metadataIds = $.map(rows, function(row){
                        return row.datarow("getValue").id;
                    })
                    $("#btnDelete").button("loading");
                    $.post($.resolveUrl("Metadata/Delete"), {objectId: thiz.options.objectId, metadataIdsJson: $.toJSON(metadataIds)}, function(model){
                        $("#btnDelete").button("reset");
                        if(model.result == 0){
                            var pageInfo = thiz._pager.pager("option").pageInfo;
                            thiz.loadMetadataGrid(thiz._searchInfo, pageInfo.start);
                        }
                        else{
                            alert(model.message)
                        }
                    });
                    return false;
                });
            }
        }
    );
})(jQuery);
