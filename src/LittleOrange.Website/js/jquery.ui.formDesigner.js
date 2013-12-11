(function($){
    $.widget("ui.formDesigner", {
            options: {
                data: null
	        },
	        _create: function(){
	        	var element = this.element;
                element.sortable({placeholder: "sortable-placeholder"})
                	.droppable({
                		accept: ".designer-tool-section",
	       	  			activeClass: "droppable-active",
	       	  			hoverClass: "droppable-hover", 
                		drop: function(event, ui){
	                	var section = ui.helper.clone()
		                	.css({"left": 0, "top": 0, "position": "relative"})
		                	.sectionDesigner();
	                	element.append(section);
					}});
	        }
        }
    );
})(jQuery);

(function($){

    $.widget("ui.designerTool", {
            options: {
                data: null
	        },
	        _create: function(){
	        	var helperHtml = this.element.find(".drag-helper").html();
	        	this.element.find(".designer-tool-element").draggable({
	        		helper: function(){return helperHtml;}, 
	        		revert: "invalid"});
	        }
        }
    );

    $.widget("ui.textDesigner", {
            options: {
                data: null
	        },
	        _create: function(){
	        }
        }
    );

    $.widget("ui.sectionDesigner", {
	        options: {
	            data: null
	        },
	        _create: function(){
	        	var thiz = this;
	        	var element = this.element;
	        	this._propertyModal = $(element.data("proerpty-model"));
	        	element.find(".designer-action").click(function(){
	        		thiz._propertyModal.modal("show");
	        		return false;
	        	});
	        	element.find(".section-clomn")
	        		.sortable({
	        			connectWith: ".section-clomn",
	        			placeholder: "sortable-placeholder"
	        		})
	       	  		.droppable({
	       	  			accept: ".designer-tool-input", 
	       	  			activeClass: "droppable-active",
	       	  			hoverClass: "droppable-hover",
	       	  			drop: function(event, ui){
		       	  			var input = ui.helper.clone()
		       	  				.css({"left": 0, "top": 0, "position": "relative"});
		           			$(this).append(input);
	               		}
	               });
	    	}
    	}
    );
})(jQuery);