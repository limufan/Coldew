function bindShoukuanFormEvent(detailsForm, dingdanForm){
    var shoukuanRiqiInput = detailsForm.getInput("shoukuanRiqi");
    var shoukuanJineInput = detailsForm.getInput("shoukuanJine");
    var tichengInput = detailsForm.getInput("ticheng");
    function jisuanTicheng(){
        if(!detailsForm.validate()){
            return;
        }
        var dingdanJson = $.toJSON(dingdanForm.getValue());
        var formValue = detailsForm.getValue();
        var shoukuanJson = $.toJSON(formValue);
        $.post($.resolveUrl("ShoukuanGuanli/JisuanTicheng"), 
            {objectId: objectId, dingdanJson: dingdanJson, shoukuanJson: shoukuanJson}, 
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
}
