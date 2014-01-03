(function($){
    $.widget("ui.metadataSearchPopover", {
            options: {
                fields: null
	        },
	        _create: function(){
                var thiz = this;
                this._popover = this.element.find(".popover");
                this._popover.click(function(event){
                    event.stopPropagation();
                });
                var form = this.element.find(".searchForm").coldewSearchForm({fields: this.options.fields}).data("coldewSearchForm");
                this.element.find(":submit").click(function(){
                    var serach = form.getValue();
                    
                    if(thiz._cb){
                       thiz._cb(serach); 
                    }
                    thiz._popover.hide();
                    return false;
                });
                this.element.find(".btnCancel").click(function(){
                    thiz._popover.hide();
                });
                
                this.element.parent().click(function(event){
                    thiz._popover.hide();
                });
	        },
            search: function(of, cb){
                this._cb = cb;
                this._popover.show().position({my: "center top", at: "center bottom", of: of});
            }
        }
    );
})(jQuery);