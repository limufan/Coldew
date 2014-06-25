function bindChanpinFormEvent(detailsForm){
    var nameInput = detailsForm.getInput("name");
    nameInput.element
        .metadataAutoComplete({objectCode: "chanpin", select: function(event, chanpin){
            detailsForm.setValue({guige: chanpin.guige, danwei: chanpin.danwei, xiaoshouDijia: chanpin.xiaoshouDijia });
        }});
    function jisuanYewufei(formValue){
        var yewulvFangshi = formValue.yewulvFangshi;
        if(yewulvFangshi == "按金额"){
            var yewulv = formValue.yewulv;
            var zongjine = formValue.zongjine;
            detailsForm.setValue({yewufei: yewulv * zongjine});
        }
        else if(yewulvFangshi == "按重量"){
            var yewulv = formValue.yewulv;
            var shuliang = formValue.shuliang;
            detailsForm.setValue({yewufei: yewulv * shuliang});
        }
    }
    function jisuanZongjine(formValue){
        var shuliang = formValue.shuliang;
        var danjia = formValue.xiaoshouDanjia;
        detailsForm.setValue({zongjine: shuliang * danjia});
    }
    function jisuanShijiDanjia(formValue){
        var zongjine = formValue.zongjine;
        var yewufei = formValue.yewufei;
        var shuliang = formValue.shuliang;
        detailsForm.setValue({shijiDanjia: (zongjine-yewufei) / shuliang * 0.83});
    }
    function jisuanButie(formValue){
        var shijiDanjia = formValue.shijiDanjia;
        var shuliang = formValue.shuliang;
        detailsForm.setValue({butie: shijiDanjia * shuliang * 0.01});
    }
    detailsForm.inputing(function(){
        var formValue = detailsForm.getValue();
        jisuanZongjine(formValue);
        jisuanYewufei(formValue);
        jisuanShijiDanjia(formValue);
        jisuanButie(formValue);
    });
    detailsForm.changed(function(){
        var formValue = detailsForm.getValue();
        jisuanZongjine(formValue);
        jisuanYewufei(formValue);
        jisuanShijiDanjia(formValue);
        jisuanButie(formValue);
    });
}