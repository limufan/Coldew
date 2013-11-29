(function($){
    $.widget("ui.userInfoMoidfyDialog", {
            options: {
                
	        },
	        _create: function(){
                var thiz = this;
                this._modal = this.element.find(".modal");
                this._actionUrl = $.baseUrl + "Org/MoidfyCurrentUserInfo";
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
                        if (formValue.newPassword != formValue.confirmNewPassword) {
                            alert("两次输入密码不一致, 请重新输入!");
                            return;
                        }
                        $.post(thiz._actionUrl, formValue, function(model){
                            if(model.result == 0){
                                thiz._modal.modal("hide");
                            }
                            else{
                                alert(model.message)
                            }
                        });
                    }
                });
                
	        },
            modify: function(){
                var thiz = this;
                this._modal.modal("show");
                $.post($.baseUrl + "Org/GetCurrentUserInfo", null, function (model) {
                    if (model.result == 0) {
                        thiz.element.find("form").setFormValue(model.data);
                    }
                    else {
                        alert(model.message)
                    }
                });
            }
        }
    );
})(jQuery);