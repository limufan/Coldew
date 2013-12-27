(function($){

    $.widget("ui.textInput", {
            options: {
                required: false
	        },
	        _create: function(){
                
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
            setValue: function(value){
                this.element.val(value);
            }
        }
    );
})(jQuery);