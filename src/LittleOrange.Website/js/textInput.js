(function($){

    $.widget("ui.textInput", {
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
	        },
            validate: function(){
                if(this.options.required){
                    var value = this.getValue();
                    if(!value){
                        this.element.focus();
                        this.element.closest('.form-group').addClass('error');
                        this.element.next('.help-inline').show();
                    }
                    else{
                        this.element.closest('.form-group').removeClass('error');
                        this.element.next('.help-inline').hide();
                    }
                }
            },
            getValue: function(){
                var value = this.element.val();
                return jQuery.trim(value);
            },
            getParm: function(){
                var value = this.getValue();
                var parm = {};
                param[this.options.name] = value;
            },
            setValue: function(value){
                this.element.val(value);
            }
        }
    );
})(jQuery);