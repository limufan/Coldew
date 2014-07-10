function bindChanpinFormEvent(detailsForm){
    var nameInput = detailsForm.getInput("name");
    nameInput.element
        .metadataAutoComplete({objectCode: "chanpin", select: function(event, chanpin){
            detailsForm.setValue({guige: chanpin.guige, danwei: chanpin.danwei, xiaoshouDijia: chanpin.xiaoshouDijia });
        }});
    function jisuanZongjine(formValue){
        var shuliang = formValue.shuliang;
        var danjia = formValue.xiaoshouDanjia;
        detailsForm.setValue({zongjine: shuliang * danjia});
    }
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
    function jisuanShijiDanjia(formValue){
        var zongjine = formValue.zongjine;
        var yewufei = formValue.yewufei;
        var shuliang = formValue.shuliang;
        var shijiDanjia = (zongjine - yewufei) / shuliang
        if(formValue.shifouKaipiao == "是"){
            shijiDanjia = shijiDanjia * 0.83;
        }
        detailsForm.setValue({shijiDanjia: shijiDanjia});
    }
    function jisuanButie(formValue){
        var shijiDanjia = formValue.shijiDanjia;
        var shuliang = formValue.shuliang;
        detailsForm.setValue({butie: shijiDanjia * shuliang * 0.01});
    }
    detailsForm.inputing(function(){
        var formValue = detailsForm.getValue();
        jisuanZongjine(formValue);
        formValue = detailsForm.getValue();
        jisuanYewufei(formValue);
        formValue = detailsForm.getValue();
        jisuanShijiDanjia(formValue);
        formValue = detailsForm.getValue();
        jisuanButie(formValue);
    });
    detailsForm.changed(function(){
        var formValue = detailsForm.getValue();
        jisuanZongjine(formValue);
        formValue = detailsForm.getValue();
        jisuanYewufei(formValue);
        formValue = detailsForm.getValue();
        jisuanShijiDanjia(formValue);
        formValue = detailsForm.getValue();
        jisuanButie(formValue);
    });
}