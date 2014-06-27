jQuery(function($){
	$.datepicker.regional['zh-CN'] = {
	    closeText: '关闭',
		prevText: '&#x3c;上月',
		nextText: '下月&#x3e;',
		currentText: '今天',
		monthNames: ['一月','二月','三月','四月','五月','六月',
		'七月','八月','九月','十月','十一月','十二月'],
		monthNamesShort: ['一','二','三','四','五','六',
		'七','八','九','十','十一','十二'],
		dayNames: ['星期日','星期一','星期二','星期三','星期四','星期五','星期六'],
		dayNamesShort: ['周日','周一','周二','周三','周四','周五','周六'],
		dayNamesMin: ['日','一','二','三','四','五','六'],
		dateFormat: 'yy-mm-dd', firstDay: 1,
		isRTL: false
		};
	$.datepicker.setDefaults($.datepicker.regional['zh-CN']);
});

jQuery.extend({
    resolveUrl: function(path, param){
        var query = "";
        if(param){
            query = $.param(param);
        }
        if(path.indexOf("?") > -1){
            return $.baseUrl + path + query;
        }
        else{
            return $.baseUrl + path + "?" + query;
        }
    },
    toJSON: function(json){
        return JSON.stringify(json);
    }
});
jQuery.extend({
    rselect: /^(?:select)/i,
    rtextarea: /^(?:textarea)/i,
    rinput: /^(?:color|date|datetime|email|hidden|month|number|password|range|search|tel|text|time|url|week|hidden)$/i,
    rradio: /^(?:radio)$/i,
    rcheckbox: /^(?:checkbox)$/i,
    getFormValue: function (form) {
        return $(form).getFormValue();
    }
});

jQuery.fn.extend({
    getViewColumn: function(){
        var columns = [];
        this.find("tbody tr").each(function(){
            var checkbox = $(this).find(":checkbox");
            if(checkbox.prop("checked")){
                var column = $(this).getFormValue();
                columns.push(column);
            }
        })
        return columns;
    },
    setViewColumn: function(value){
        var columns = [];
        this.find("tbody tr").each(function(){
            var checkbox = $(this).find(":checkbox");
            var text = $(this).find(":text");
            var fieldId = $(this).data("fieldId");
            if(value[fieldId]){
                checkbox.prop("checked", true);
                text.val(value[fieldId].width);
            }
        })
        return columns;
    },
    getSearch: function(){
        var serach = {};
        this.find(".keywordSearch")
        .each(function (i, elem) {
            serach[elem.name] = $(elem).val();
					
		});

        this.find(".numberSearch, .dateSearch")
        .each(function (i, elem) {
            var value = $(this).getFormValue();
            var fieldCode = $(this).data("fieldCode");
            serach[fieldCode] = value;
					
		});

        var keyword = this.find(".keyword").val();
        serach.keyword = keyword;
        return serach;
    },
    setSearch: function(serach){
        if(!serach){
            return;
        }
        this.find(".keywordSearch")
        .each(function (i, elem) {
            $(elem).val(serach[elem.name]);
		});

        this.find(".numberSearch, .dateSearch")
        .each(function (i, elem) {
            var fieldCode = $(this).data("fieldCode");
            if(fieldCode in serach){
                $(this).setFormValue(serach[fieldCode]);
            }
		});
        return this;
    },
    getFormValue: function () {
        var valueArray = this.find(":input")
		.filter(function () {
		    return this.name &&
				(this.checked || $.rselect.test(this.nodeName) || $.rtextarea.test(this.nodeName) ||
					$.rinput.test(this.type));
		})
		.map(function (i, elem) {
		    var val = jQuery(this).val();

		    return val == null ?
				null :
				jQuery.isArray(val) ?
					jQuery.map(val, function (val, i) {
					    return { name: elem.name, value: val.replace(/\r?\n/g, "\r\n"), type: elem.type };
					}) :
					{ name: elem.name, value: val.replace(/\r?\n/g, "\r\n"), type: elem.type };
		}).get();

        var valueObj = {};
        
        $.each(valueArray, function (i, value) {
            if($.rcheckbox.test(value.type)){
                if(!valueObj[value.name]){
                    valueObj[value.name] = [];
                }
                valueObj[value.name].push(value.value);
            }
            else{
                valueObj[value.name] = value.value;
            }
        });
        return valueObj;
    },
    setFormValue: function (obj) {
        this.find(":input")
        .filter(function () {
            return this.name &&
				($.rradio.test(this.type) || $.rselect.test(this.nodeName) || $.rtextarea.test(this.nodeName) ||
					$.rinput.test(this.type) || $.rcheckbox.test(this.type));
        })
        .each(function () {
            if($.rradio.test(this.type)){
                if(obj[this.name] && $(this).val() == obj[this.name].toString()){
                    $(this).prop("checked", true);
                }
            }
            else if($.rcheckbox.test(this.type)){
                if($.isArray(obj[this.name])){
                    if(obj[this.name] && $.inArray($(this).val(), obj[this.name]) > -1){
                        $(this).prop("checked", true);
                    }
                    else{
                        $(this).prop("checked", false);
                    }
                }
            }
            else{
                $(this).val(obj[this.name]);
            }
        });
        return this;
    },
    setFormReadOnly: function () {
        this.map(function () {
            return this.elements ? jQuery.makeArray(this.elements) : this;
        })
        .filter(function () {
            return this.name &&
				($.rradio.test(this.type) || $.rselect.test(this.nodeName) || $.rtextarea.test(this.nodeName) ||
					$.rinput.test(this.type));
        })
        .each(function () {
            if ($.rradio.test(this.type) || $.rselect.test(this.nodeName)) {
                $(this).prop("disabled", true);
            }
            else {
                $(this).prop("readonly", true);
            }
        });
        return this;
    },
    validAndFocus: function () {
        var validValue = this.valid();
        if (!validValue) {
            this.validate().focusInvalid();
        }
        return validValue;
    },
    dateRange: function(){
        this.each(function(){
            var inputs = $(this).find("input");
            inputs.eq(0).datepicker({ changeMonth: true, changeYear: true, onSelect: function(selectedDate){
                inputs.eq(1).datepicker( "option", "minDate", selectedDate );
            }});
            inputs.eq(1).datepicker({ changeMonth: true, changeYear: true, onSelect: function(selectedDate){
                inputs.eq(0).datepicker( "option", "maxDate", selectedDate );
            }});
        }); 
    },
    setLoading: function(){
        if(!this.data("val")){
            this.data("val", this.val());
        }
        if(!this.data("loadingText")){
            this.data("loadingText", "loading...");
        }
        this.val(this.data("loadingText"));
        this.prop("disabled", true);
        return this;
    },
    resetLoading: function(){
        this.val(this.data("val"));
        this.prop("disabled", false);
        return this;
    }
});
$.ajaxSetup ({
    cache: false
});
function split(val) {
    return val.split(/[,，、\\；;]\s*/);
}
function extractLast(term) {
    return split(term).pop();
}
$(document).ready(function() {

    $(".date").datepicker();

    $(".suggestion-input").each(function () {
        var multipleSymbolRegex = /[,，、\\；;]\s*/;
        var multipleSymbolEndRegex = /[,，、\\；;]$/;
        var suggestions = $(this).data("suggestions");
        $(this).autocomplete({
            source: function (request, response) {
                response($.ui.autocomplete.filter(suggestions, extractLast(request.term)));
            },
            focus: function () {
                return false;
            },
            minLength: 0,
            select: function (event, ui) {
                var val = $(this).val();
                if (multipleSymbolEndRegex.test(val)) {
                    $(this).val( val + ui.item.value)
                }
                else {
                    $(this).val(ui.item.value)
                }
                return false;
            }
        })
        .click(function () {
            $(this).autocomplete("search", "");
        });
        
    });
});
String.prototype.format = function() {
 
    // Convert `arguments` to real []
 
    var args = Array.prototype.slice.call(arguments);
   
    // First arg is an object map
   
    if (args.length === 1 && typeof args[0] === "object") {
        args = args[0];
    }
   
    // Do the replacing/formatting; args is now an object
   
    var result = this, match;
        
    for (var i = 0; match = /\${(\d+|\w+)?}/gm.exec(result); i++) {
    
        var key = match[1];
        
        if (!key) {
            result = result.replace("{}", args[i]);
        }
        else {
            result = result.replace(new RegExp("\\$\\{" + key + "\\}", "gm"), args[key]);
        }
    }
    
    return result;
};