
window.onload = function () {
    //$("#content").height(getTotalHeight() - 38);
    $(".t-grid-content").removeAttr("style");
    //$(".t-grid-content").attr("style", "overflow-y:no");
}

function getTotalHeight() {
    if ($.browser.msie) {
        return document.compatMode == "CSS1Compat" ? document.documentElement.clientHeight : document.body.clientHeight;
    }
    else {
        return self.innerHeight;
    }
}

function More() {
    $("#more").empty();
    if ($("#divMore").is(":hidden ")) {
        $("#divMore").fadeIn("slow");
        $("#more").append("Hide");
    }
    else {
        $("#divMore").hide("slow");
        $("#more").append("More...");
    }
}


function EmptyChange(e) {
    if (e.value != "") {
        var combobox = $(this).data("tComboBox");
        if (combobox == undefined || combobox.selectedIndex == undefined) {
            return;
        }
        else if (combobox.selectedIndex == -1) {
            combobox.value("");
            combobox.reload();
        }
    }
}

function EmptyDataBinding() {
}

function PrintOrder(printUrl) {
    if (printUrl == null || printUrl.length == 0) {
        return;
    }
    var xlApp = null;
    try {
        xlApp = new ActiveXObject("Excel.Application");
    } catch (e) {
        //alert("${Common.Warning.Please.Send.The.Site.To.Join.Trust.Site}");
        alert("Please add a site to trust the current site!");
        return;
    }
    var xlBook = xlApp.WorkBooks.open(printUrl);
    var xlsheet = xlBook.Worksheets(1);
    try {
        xlsheet.PrintOut(); //打印工作表
    } catch (e) {
    }
    xlBook.Close(false); //关闭文档
    xlApp.Quit();   //结束excel对象
    xlApp = null;   //释放excel对象
}

function CleanTabMessage() {
    ClearMessage();
}

/* 
*  jQuery tui tablespan plugin 0.2 
* 
*  Copyright (c) 2010 china yewf 
* 
* Dual licensed under the MIT and GPL licenses: 
*   http://www.opensource.org/licenses/mit-license.php 
*   http://www.gnu.org/licenses/gpl.html 
* 
* Create: 2010-09-16 10:34:51 yewf $ 
* Revision: $Id: tui.tablespan.js  2010-09-21 10:08:36 yewf $  
* 
* Table rows or cols span 
*/
/* 行合并。索引从0开始，包含隐藏列，注意jqgrid的自动序号列也是一列。 
使用方法： 
$("#jqGridId").tuiTableRowSpan("3, 4, 8"); 
*/
jQuery.fn.tuiTableRowSpan = function (colIndexs) {
    return this.each(function () {
        var indexs = eval("([" + colIndexs + "])");
        for (var i = 0; i < indexs.length; i++) {
            var colIdx = indexs[i];
            var that;
            $('tr', this).each(function (row) {
                $('td:eq(' + colIdx + ')', this).filter(':visible').each(function (col) {
                    if (that != null && $(this).html() == $(that).html()) {
                        rowspan = $(that).attr("rowSpan");
                        if (rowspan == undefined) {
                            $(that).attr("rowSpan", 1);
                            rowspan = $(that).attr("rowSpan");
                        }
                        rowspan = Number(rowspan) + 1;
                        $(that).attr("rowSpan", rowspan);
                        $(this).hide();
                    } else {
                        that = this;
                    }
                });
            })
        }
    });
};
/* 列表头合并。
索引从0开始，包含隐藏列，注意jqgrid的自动序号列也是一列。
   
使用方法：
$("#jqGridId").tuiJqgridColSpan({ 
cols: [
{ indexes: "3, 4", title: "合并后的大标题" },
{ indexes: "6, 7", title: "合并后的大标题" },
{ indexes: "11, 12, 13", title: "合并后的大标题" }
]
});
注意事项： 
1.没有被合并的rowSpan=2，即两行。列的拖拉有BUG，不能和jqgrid的显示层位置同步；    
2.jqgrid的table表头必须有aria-labelledby='gbox_tableid' 这样的属性；
3.只适用于jqgrid；
*/
var tuiJqgridColSpanInit_kkccddqq = false;
jQuery.fn.tuiJqgridColSpan = function (options) {
    options = $.extend({}, { cols: null }, options);
    if (tuiJqgridColSpanInit_kkccddqq) {
        return;
    }
    // 验证参数
    if (options.cols == null || options.cols.length == 0) {
        alert("cols参数必须设置");
        return;
    }
    // 传入的列参数必须是顺序列，由小到大排列，如3,4,5
    var error = false;
    for (var i = 0; i < options.cols.length; i++) {
        var colIndexs = eval("([" + options.cols[i].indexes + "])");
        for (var j = 0; j < colIndexs.length; j++) {
            if (j == colIndexs.length - 1) break;
            if (colIndexs[j] != colIndexs[j + 1] - 1) {
                error = true;
                break;
            }
        }
        if (error) break;
    }
    if (error) {
        alert("传入的列参数必须是顺序列，如：3,4,5");
        return;
    }
    // 下面是对jqgrid的表头进行改造
    var resizing = false,
    currentMoveObj, startX = 0;
    var tableId = $(this).attr("id");
    // thead
    var jqHead = $("table[aria-labelledby='gbox_" + tableId + "']");
    var jqDiv = $("div#gbox_" + tableId);
    var oldTr = $("thead tr", jqHead);
    var oldThs = $("thead tr:first th", jqHead);
    // 在原来的th上下分别增加一行，下面这行克隆，上面这行增加且height=0
    var ftr = $("<tr/>").css("height", "auto").addClass("ui-jqgrid-labels").attr("role", "rowheader").insertBefore(oldTr);
    var ntr = $("<tr/>").addClass("ui-jqgrid-labels").attr("role", "rowheader").insertAfter(oldTr);
    oldThs.each(function (index) {
        var cth = $(this);
        var cH = cth.css("height"), cW = cth.css("width"),
        nth = $("<th/>").css("height", cH),
        fth = $("<th/>").css("height", 0);
        // 在IE8或firefox下面，会出现多一条边线，因此要去掉。
        if (($.browser.msie && $.browser.version == "8.0") || $.browser.mozilla) {
            fth.css({ "border-top": "solid 0px #fff", "border-bottom": "solid 0px #fff" });
        }
        if (cth.css("display") == "none") {
            nth.css({ "display": "none", "white-space": "nowrap", "width": 0 });
            fth.css({ "display": "none", "white-space": "nowrap", "width": 0 });
        }
        else {
            nth.css("width", cW);
            fth.css("width", cW);
            // 这里增加一个事件，解决列的拖动
            var res = cth.children("span.ui-jqgrid-resize");
            res && res.bind("mousedown", function (e) {
                currentMoveObj = $(this);
                startX = getEventPos(e).x;
                resizing = true;
                document.onselectstart = new Function("return false");
            });
        }
        // 增加第一行
        fth.addClass(cth.attr("class")).attr("role", "columnheader").appendTo(ftr);
        // 增加第三行
        cth.children().clone().appendTo(nth);
        nth.addClass(cth.attr("class")).attr("role", "columnheader").appendTo(ntr);
    });
    // 列合并。注意：这里不放在上面的循环中处理，因为每个遍历都要执行下面的操作。
    for (var i = 0; i < options.cols.length; i++) {
        var colIndexs = eval("([" + options.cols[i].indexes + "])");
        var colTitle = options.cols[i].title;
        var isrowSpan = false;
        for (var j = 0; j < colIndexs.length; j++) {
            oldThs.eq(colIndexs[j]).attr({ "colSpan": colIndexs.length, "rowSpan": "1" });
            // 把被合并的列隐藏，不能remove，这样jqgrid的排序功能会错位。
            if (j != 0) {
                oldThs.eq(colIndexs[j]).attr("colSpan", "1").hide();
            }
            // 标记删除clone后多余的th
            $("thead tr:last th", jqHead).eq(colIndexs[j]).attr("tuidel", "false");
            // 增加列标题
            if (j == 0) {
                var div = oldThs.eq(colIndexs[j]).find("div.ui-jqgrid-sortable");
                var divCld = div.children();
                div.text(colTitle);
                div.append(divCld);
            }
        }
    }
    // 移除多余列
    $("thead tr:last th[tuidel!='false']", jqHead).remove();
    // 对不需要合并的列增加rowSpan属性
    oldThs.each(function () {
        if ($(this).attr("colSpan") == 1) {
            $(this).attr("rowSpan", 2);
        }
    });
    var jqBody = $(this);
    // 绑定拖动事件
    $(document).bind("mouseup", function (e) {
        var ret = true;
        if (resizing) {
            var parentTh = currentMoveObj.parent();
            var currentIndex = parentTh.parents("tr").find("th").index(parentTh);
            var width, diff;
            var tbodyTd = $("tbody tr td", jqBody);
            var currentTh = $("thead tr:first th", jqHead).eq(currentIndex);
            // 先使用td的宽度，如果td不存在，则使用事件宽度
            if (tbodyTd.length > 0) {
                diff = 0;
                width = parseInt(tbodyTd.eq(currentIndex).css("width"));
            }
            else {
                diff = getEventPos(e).x - startX;
                width = parseInt(currentTh.css("width"));
            }
            var lastWidth = diff + width;
            currentTh.css("width", lastWidth + "px");
            resizing = false;
            ret = false;
        }
        document.onselectstart = new Function("return true");
        return ret;
    });
    // 设置为已初始化
    tuiJqgridColSpanInit_kkccddqq = true;
    // 适应不同浏览器获取鼠标坐标
    getEvent = function (evt) {
        evt = window.event || evt;
        if (!evt) {
            var fun = getEvent.caller;
            while (fun != null) {
                evt = fun.arguments[0];
                if (evt && evt.constructor == Event)
                    break;
                fun = fun.caller;
            }
        }
        return evt;
    }
    getAbsPos = function (pTarget) {
        var x_ = y_ = 0;
        if (pTarget.style.position != "absolute") {
            while (pTarget.offsetParent) {
                x_ += pTarget.offsetLeft;
                y_ += pTarget.offsetTop;
                pTarget = pTarget.offsetParent;
            }
        }
        x_ += pTarget.offsetLeft;
        y_ += pTarget.offsetTop;
        return { x: x_, y: y_ };
    }
    getEventPos = function (evt) {
        var _x, _y;
        evt = getEvent(evt);
        if (evt.pageX || evt.pageY) {
            _x = evt.pageX;
            _y = evt.pageY;
        } else if (evt.clientX || evt.clientY) {
            _x = evt.clientX + (document.body.scrollLeft || document.documentElement.scrollLeft) - (document.body.clientLeft || document.documentElement.clientLeft);
            _y = evt.clientY + (document.body.scrollTop || document.documentElement.scrollTop) - (document.body.clientTop || document.documentElement.clientTop);
        } else {
            return getAbsPos(evt.target);
        }
        return { x: _x, y: _y };
    }
};


/** 
* 时间对象的格式化;  
* eg:nowDate = new Date().format("yyyy-MM-dd hh:mm:ss")

Date.prototype.format1 = function (format) {
    var o = {
        "M+": this.getMonth() + 1, // month  
        "d+": this.getDate(), // day  
        "h+": this.getHours(), // hour  
        "m+": this.getMinutes(), // minute  
        "s+": this.getSeconds(), // second  
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter  
        "S": this.getMilliseconds()
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
}  
*/