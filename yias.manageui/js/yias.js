
if (typeof (yias) == "undefined") {
    var yias = {};
    if (typeof (window.top.yias) != 'undefined')
        yias = window.top.yias;
    jQuery.support.cors = true;
    //样式
    var _ajax = $.ajax;
    $.ajax = function (options) {
        //2.每次调用发送ajax请求的时候定义默认的error处理方法
        var fn = {
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(XMLHttpRequest.responseText);
            },
            success: function (data, textStatus) { },
            beforeSend: function (XHR) { },
            complete: function (XHR, TS) { }
        }
        //3.扩展原生的$.ajax方法，返回最新的参数
        var _options = $.extend({}, {
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                fn.error(XMLHttpRequest, textStatus, errorThrown);
            },
            success: function (data, textStatus) {
                fn.success(data, textStatus);
            },
            beforeSend: function (XHR) {
                fn.beforeSend(XHR);
            },
            complete: function (XHR, TS) {
                fn.complete(XHR, TS);
            }
        }, options);
        //4.将最新的参数传回ajax对象
        _ajax(_options);
    };
    yias.util = {
        getquery: function (name, urlSrc) {
            urlSrc ? null : urlSrc = window.location.search;
            var reg = new RegExp('(^|&)' + name + '=([^&]*)(&|$)', 'i');
            var r = urlSrc.substr(1).match(reg);
            if (r != null) {
                return unescape(r[2]);
            }
            return '';
        },
        jqGrid: {
            loadGrid: function ($grid, data, url) {
                var postData = $grid.getGridParam("postData").postData || {};
                if (typeof (postData) != "string")
                    $.each(postData, function (k, v) {
                        delete postData[k];
                    });
                else { postData = {} }
                if (typeof (data) == "undefined") data = {};
                if (typeof (url) == "undefined") {
                    $grid.setGridParam({ postData: { postData: data }, page: 1, datatype: 'json' }).trigger("reloadGrid");
                }
                else {
                    url = core.urls.apiRoute + url;
                    $grid.setGridParam({ postData: { postData: data }, page: 1, url: url, datatype: 'json' }).trigger("reloadGrid");
                }
            },
            mergeCell: function (cellvalue, options, rowObject) {
                var value = ``;
                var mergeObj = options.colModel.mergeOptions || {};
                var colon = '';
                var title;
                for (var key in mergeObj) {
                    title = '';
                    if (typeof mergeObj[key] === 'string') {
                        colon = mergeObj[key] ? ' ' : '';
                        if (~mergeObj[key].indexOf('font') && mergeObj[key].match(/title\="([\S]*)"/)) {
                            title = mergeObj[key].match(/title\="([\S]*)"/)[1];
                        }
                        value += `<div>
                                <span class="display-none">${title}</span>
                                <span>${mergeObj[key]}${colon}</span>
                                <span style="font-weight:bold">${rowObject[key] || ''}</span>
                            </div>`;

                    }
                    if (typeof mergeObj[key] === 'object') {
                        colon = mergeObj[key]['value'] ? ' ' : '';
                        if (mergeObj[key]['value']) {
                            if (~mergeObj[key]['value'].indexOf('font') && mergeObj[key]['value'].match(/title\="([\S]*)"/)) {
                                title = mergeObj[key]['value'].match(/title\="([\S]*)"/)[1];
                            }
                        }
                        if (mergeObj[key]['dateFormat'] && rowObject[key] && (rowObject[key] !== '暂无')) {
                            rowObject[key] = $$.tools.dateFormat(new Date(rowObject[key]), mergeObj[key]['dateFormat']);
                        }
                        if (mergeObj[key]['float'] && rowObject[key] !== undefined) {
                            rowObject[key] = $$.tools.getFloat(rowObject[key], mergeObj[key]['float']);
                        }
                        if (typeof mergeObj[key]['openurl'] === 'object') {
                            if (mergeObj[key]['openurl']['url'] && mergeObj[key]['openurl']['id']) {
                                var url = mergeObj[key]['openurl']['url'];
                                var id = rowObject[mergeObj[key]['openurl']['id']];
                                var title = mergeObj[key]['openurl']['title'];
                                var type = mergeObj[key]['openurl']['type'];
                                var typeValue = rowObject[mergeObj[key]['openurl']['type']];
                                if (type) {
                                    rowObject[key] = "<a href='#' title='" + title + "' onclick=\"$$.openWindowWithFunction('" + title + "', '" + url + id + "&type=" + typeValue + "');\">" + rowObject[key] + "</a>";
                                } else {
                                    rowObject[key] = "<a href='#' title='" + title + "' onclick=\"$$.openWindowWithFunction('" + title + "', '" + url + id + "');\">" + rowObject[key] + "</a>";
                                }

                            }
                        }

                        value += `<div>
                                <span class="display-none">${title}</span>
                                <span>${mergeObj[key]['value'] || ''}${colon}</span>
                                <span class="${mergeObj[key]['className'] || ''}" style="font-weight:bold;">${rowObject[key] || ''}</span>
                            </div>`;
                    }
                }
                var newVal = value.replace(/\s+/gm, ' ');
                return newVal;
            },
            cellFormat: function (cellvalue, options, rowObject) {
                if (cellvalue == null || cellvalue === '') return options.colModel.def || '';
                var strFmt = options.colModel.dataformat;
                if (cellvalue.constructor == Date) {
                    return cellvalue.format(strFmt);
                }
                try {
                    switch (strFmt) {
                        case "w":
                            if (cellvalue == 0) return "0";
                            cellvalue = (parseFloat(cellvalue) / 10000).toFixed(2) + '万';
                            break;
                        case "c":
                            if (cellvalue == 0) return "0";
                            cellvalue = (parseFloat(cellvalue) / 10000).toFixed(2);
                            break;
                        case "p":
                            if (cellvalue == 0) return 0 + '%';
                            cellvalue = (parseFloat(cellvalue) * 100).toFixed(2) + '%';
                            break;
                        case "d":

                            break;
                        case "int":
                            cellvalue = parseInt(cellvalue);
                            if (isNaN(cellvalue) || !cellvalue) {
                                cellvalue = 0;
                            }
                            break;
                        case "float4":
                            cellvalue = $$.tools.getFloat(cellvalue, 4);
                            break;
                        case "m":
                            break;
                        case "status":
                            cellvalue = cellvalue === 1 ? '<span class="label">启用</span>' : '<span class="label label-danger">停用</span>';
                            break;
                        case "hv_type":
                            cellvalue = cellvalue === 1 ? '高值物资' : '普耗物资';
                            break;
                        case "void":
                            cellvalue = cellvalue === 0 ? '<span class="label">启用</span>' : '<span class="label label-danger">停用</span>';
                            break;
                        case "isdefault":
                            cellvalue = cellvalue === 1 ? '<span class="label">是</span>' : '<span class="label label-danger">否</span>';
                            break;
                        case 'multiply':
                            var value1 = rowObject[options.colModel.formatoptions.value1],
                                value2 = rowObject[options.colModel.formatoptions.value2];
                            (isNaN(value1) || isNaN(value1)) && (cellvalue = 0);
                            cellvalue = value1 * value2;
                            break;
                        default:
                            if (typeof strFmt == "object") {
                                for (var key in strFmt) {
                                    if (key == cellvalue + "") {
                                        return strFmt[key];
                                    }
                                }
                            }
                            return cellvalue;
                    }
                    return cellvalue;
                }
                catch (err) {
                    return cellvalue;
                }
            },
            gridColStyle: {
                timeCol: function (l, n, w) {//时间列
                    if (!w) { w = 120 }
                    return { label: l, name: n, index: n, width: w, align: 'center', formatter: "date", formatoptions: { srcformat: 'Y-m-d H:i', newformat: 'Y-m-d H:i' } };
                },
                dateCol: function (l, n, w) {//日期列
                    if (!w) { w = 100 }
                    return { label: l, name: n, index: n, width: w, align: 'center', formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' } };
                },
                billNumCol: function (l, n, w) {//编码列
                    if (!w) { w = 120 }
                    return { label: l, name: n, index: n, width: w, align: 'left' };
                },
                amtCol: function (l, n, w, option) {//金额列
                    var formatter;
                    if (!w) { w = 88 }
                    if (option) {
                        formatter = function (cellValue, options, rowObject) {
                            return $$.tools.getFloat(rowObject[option.price] * rowObject[option.quan], 2);
                        }
                    } else {
                        formatter = 'currency'
                    }
                    return { label: l, name: n, index: n, width: w, align: 'right', formatter };
                },
                numCol: function (l, n, w, defaultVal) {//数字列
                    if (!w) { w = 68 }
                    var result = { label: l, name: n, index: n, width: w, align: 'right' };
                    if (defaultVal !== undefined && defaultVal !== '') {
                        result.formatter = function (cellValue) {
                            if (cellValue === '0') {
                                cellValue = defaultVal;
                            }
                            return cellValue || defaultVal;
                        }
                    }
                    return result;
                },
                priceCol: function (l, n, w, p) {//价格列
                    p = p || 2;
                    if (!w) { w = 80 }
                    return { label: l, name: n, index: n, width: w, align: 'right', formatter: 'number', formatoptions: { decimalPlaces: p } }
                },
                txtCol: function (l, n, w) {//文字列
                    if (!w) { w = 100; }
                    return { label: l, name: n, index: n, width: w, align: 'left' }
                },
                editCol: function (l, n, w, maxlength) {//文字列 可编辑
                    if (!w) { w = 100; }
                    if (!maxlength) { maxlength = ''; }
                    return {
                        label: l, name: n, index: n, width: w, classes: "edittd", editable: true, align: 'left', editoptions: {
                            dataInit: function (element) {
                                $(element).attr('maxlength', maxlength);
                            }
                        }
                    }
                },
                hideList: function (list) {
                    var hideCol = [];
                    list.forEach(function (val) {
                        hideCol.push({ label: val, name: val, index: val, hidden: true });
                    })
                    return hideCol;
                },
                hidCol: function (l, n, option) {//隐藏列
                    return $.extend({ label: l, name: n, index: n, hidden: true }, option);
                },
                centCol: function (l, n, w) {//居中列
                    if (!w) { w = 80; }
                    return { label: l, name: n, index: n, width: w, align: 'center' }
                },
                cusCol: function (l, n, w, f, def) {//自定义列（默认为是否）
                    if (!w) { w = 80 }
                    if (!f) f = "isdefault";

                    return { label: l, name: n, index: n, width: w, align: 'center', dataformat: f, def: def, formatter: $$.plugins.jqGrid.cellFormat }
                },
                mergeCol: function (l, n, mop, w) {
                    if (!w) w = 200;
                    return { label: l, name: n, index: n, width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mop };
                },
                withCol: function (l, n, w) {
                    if (!w) w = 120;
                    return { label: l, name: n, index: n, width: w, align: 'left', cellattr: function (rowId, tv, rawObject, cm, rdata) { return 'id=\'' + n + rowId + "\'"; } }
                },
                prdCol: function (pd, pi, w) {//物资列
                    var pname = (pd ? pd : "pd_name");
                    var piname = (pi ? pi : "spec");
                    var mp = {};
                    mp[pname] = '<i class="font font-piname" title="物资名称"></i>';
                    mp[piname] = { value: '<i class="font font-spec" title="规格"></i>', className: 'dict_name' };
                    if (!w) { w = 200 }
                    return { label: '物资名称/规格', name: 'pd_name&spec', index: 'pd_name&spec', width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mp };
                },
                splCol: function (mp, w) {//供应商列
                    if (!mp) mp = {
                        spl_name: '<i class="font font-dispatching" title="配送商"></i>',
                        mnf_name: '<i class="font font-mnf" title="生产厂家"></i>'
                    };
                    if (!w) { w = 250 }
                    return { label: '配送商/厂家', name: 'spl_name&mnf_name', index: 'spl_name&mnf_name', width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mp };
                },
                splFromCol: function (mp, w) {//供应商列
                    if (!mp) mp = {
                        spl_name: '<i class="font font-dispatching" title="配送商"></i>',
                        spl_from_name: '<i class="font font-spl" title="供应商"></i>',
                        mnf_name: '<i class="font font-mnf" title="生产厂家"></i>'
                    };
                    if (!w) { w = 250 }
                    return { label: '配送商/供应商/厂家', name: 'spl_name&spl_from_name&mnf_name', index: 'spl_name&spl_from_name&mnf_name', width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mp };
                },
                editDateCol: function (l, n, w) {
                    if (!w) { w = 120 }
                    return { label: l, name: n, index: n, width: w, align: 'center', classes: "edittd", formatter: "date", editable: true, editoptions: { dataInit: function (element) { $$.ui.datePickerIni($(element)); } }, align: 'center', datefmt: 'yyyy-MM-dd' }
                },
                bcCol: function (mp, w) {
                    if (!mp) mp = {
                        bc: '<i class="font font-bc" title="主码"></i>',
                        bc_sub: '<i class="font font-bc_sub" title="副码"></i>'
                    };
                    if (!w) { w = 250 }
                    return { label: '主码/副码', name: 'bc&bc_sub', index: 'bc&bc_sub', width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mp };
                },
                invCol: function (inv_no, inv_date, w) {
                    var inv_no = (inv_no ? inv_no : "inv_no");
                    var inv_date = (inv_date ? inv_date : "inv_date");
                    var mp = {};
                    mp[inv_no] = '<i class="font font-code" title="发票号码"></i>';
                    mp[inv_date] = { value: '<i class="font font-date" title="发票日期"></i>', dateFormat: 'D' };
                    if (!w) { w = 200 }
                    return { label: '发票号码/发票日期', name: 'inv_no&inv_date', index: 'inv_no&inv_date', width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mp };
                },
                lotCol: function (lot, exp_date, w) {
                    var lot = (lot ? lot : "lot");
                    var exp_date = (exp_date ? exp_date : "exp_date");
                    var mp = {};
                    mp[lot] = '<i class="font font-lot" title="生产批号"></i>';
                    mp[exp_date] = { value: '<i class="font font-lose-efficacy" title="失效日期"></i>', dateFormat: 'D' };
                    if (!w) { w = 120 }
                    return { label: '批号/效期', name: 'lot&exp_date', index: 'lot&exp_date', width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mp };
                },
                prdexpCol: function (mp, w) {
                    if (!mp) mp = {
                        prd_date: {
                            value: '<i class="font font-produce" title="生产日期"></i>',
                            dateFormat: 'D'
                        },
                        exp_date: {
                            value: '<i class="font font-lose-efficacy" title="失效日期"></i>',
                            dateFormat: 'D'
                        }
                    };
                    if (!w) { w = 150 }
                    return { label: '生产日期/效期', name: 'prd&exp', index: 'prd&exp', width: w, align: 'left', formatter: $$.plugins.jqGrid.mergeCell, mergeOptions: mp };
                },
                prdQuanCol: function (l, n, minn, maxn, allowZero, allowNegative, cb) {
                    if (!l) { l = '数量'; }
                    if (!n) { n = "quan"; }
                    if (!minn) { minn = "min_count"; }
                    if (!maxn) { maxn = "max_count"; }
                    return {
                        label: l, name: n, index: n, width: 100, align: 'right', classes: "edittd", editable: true, dataformat: "int", formatter: $$.plugins.jqGrid.cellFormat, editoptions: {
                            dataInit: function (element, row) {
                                var gridid = $(element).parents('td').parents('tr').parents('table').attr("id");
                                var rowid = $(element).attr("rowid")
                                //var rowdata = $("#" + gridid).jqGrid('getRowData', $(element).attr("rowid"));
                                var min_count = $(element).parents('td').siblings('[aria-describedby="' + gridid + '_' + minn + '"]').attr('title');
                                var max_count = $(element).parents('td').siblings('[aria-describedby="' + gridid + '_' + maxn + '"]').attr('title');
                                if (!cb) {
                                    cb = function (val) {
                                        var allIds = $("#" + gridid).jqGrid('getDataIDs');
                                        var allAmount = 0;
                                        allIds.forEach(function (id) {
                                            var rowdata = $("#" + gridid).jqGrid('getRowData', id);
                                            rowdata.amount = parseFloat((rowdata["price"] || 0) * (rowdata[n].indexOf('<') != -1 ? val : rowdata[n]));
                                            $("#" + gridid).jqGrid("setCell", id, 'amount', rowdata.amount);
                                            allAmount += rowdata.amount;
                                        });
                                        $('#amount').val($$.tools.getFloat(allAmount, 2));
                                    };
                                }

                                $$.plugins.initInput(element, null, min_count, undefined, max_count, allowZero, '', cb);
                            }
                        }
                    };
                }
            }
        },
        from: {
            init: function (frmid, datas, cbf) {

                var exsel = [];
                var htmlmodels = [];

                //获取表单html
                for (var i = 0; i < datas.length; i++) {
                    htmlmodels.push(this._addcontrol(frmid, datas[i], exsel));
                }
                if (exsel.length > 0) {
                    yias.ajax.add("selkey", exsel);
                    var postfrmid = frmid;
                    var posthtmlmodels = htmlmodels;
                    var postexsel = exsel;
                    yias.ajax.send("ToolBLL", "DataTestService", "GetTest", function (ret) {
                        yias.util.from._setfrmhtml(postfrmid, posthtmlmodels);
                        //赋值数据
                        for (var i = 0; i < postexsel.length; i++) {

                        }
                        yias.util.from._topagecbfed(cbf);
                    });
                }
                else {
                    this._setfrmhtml(frmid, htmlmodels);
                    this._topagecbfed(cbf);
                }
            },
            _addcontrol: function (code, item, exsel) {
                var conhtmlmodel = {};
                conhtmlmodel.code = item.code;
                conhtmlmodel.name = item.name;
                switch (item.type) {
                    case "txtcon":
                        conhtmlmodel.controlhtml = '<input type="text" class="input" placeholder="请输入' + item.name + '" name="' + item.code + '"' + (item.len ? "maxlength='" + item.len + "'" : "") + '/>';
                        break;
                    case "datecon":
                        conhtmlmodel.controlhtml = '<input type="text" class="input" placeholder="请选择' + item.name + '" name="' + item.code + '"' + ' onClick="WdatePicker();" />';
                        break;
                    case "timecon":
                        conhtmlmodel.controlhtml = '<input type="text" class="input" placeholder="请选择' + item.name + '" name="' + item.code + '"' + ' onClick="WdatePicker({dateFmt:\'yyyy-MM-dd HH:mm:ss\'})"' + (item.len ? "maxlength='" + item.len + "'" : "") + '/>';
                        break;
                    case "s_to_econ":
                        conhtmlmodel.controlhtml = '<input type="text" class="input" placeholder="请输入' + item.name + '" name="' + item.code.split('|')[0] + '" id="' + item.code.split('|')[0] + '" onClick="WdatePicker({maxDate:\'#F{$dp.$D(\'' + item.code.split('|')[0] + '\');}\'})"/>到' +
                            '<input type="text" class="form-control" placeholder="请输入' + item.name + '" name="' + item.code.split('|')[1] + '"' + ' id="' + item.code.split('|')[1] + '" onClick="WdatePicker({minDate:\'#F{$dp.$D(\'' + item.code.split('|')[1] + '\')}\'})"/>';
                        break;
                    case "sel2con":
                        exsel.push({ id: (code + item.code), selkey: item.othkey });
                        conhtmlmodel.controlhtml = '<select class="select" id="' + code + item.code + '" name="' + item.code + '"></select>';
                        break;
                    case "selcon":
                        conhtmlmodel.controlhtml = '<select class="select" id="' + code + item.code + '" name="' + item.code + '"></select>';
                        break;
                    case "selconnotall":
                        conhtmlmodel.controlhtml = '<select class="select" id="' + code + item.code + '" name="' + item.code + '"></select>';
                        break;
                    case "checkbox":
                        conhtmlmodel.controlhtml = '<input type="checkbox" class="checkbox" id="' + code + item.code + '" name="' + item.code + '"></input>';
                        break;
                    default:
                }
                return conhtmlmodel;
            },
            _setfrmhtml: function (frmid, htmls) {
                var tohtml = "";
                var fromTemp = "";
                for (var i = 0; i < htmls.length; i++) {
                    tohtml += "";
                }
                $("#" + frmid).html(tohtml);
            },
            _topagecbfed: function (cbf) {
                if (typeof (cbf) != "undefined" && typeof (cbf) == "function") cbf();
            }
        }
    };

    yias.ajax = {
        _ajaxurl: "/service/sendobj",
        _root: { "type": "json" },
        load: function () {
            this._root = { "type": "json" };
        },
        add: function (Key, Value) {
            this._root[Key] = Value;
        },
        _getXML: function () {
            var retXml = this._toJSON(this._root);
            while (retXml.indexOf("'") != -1) {
                retXml = retXml.replace("'", "[+|+]");
            }
            return (retXml + "");
        },
        _toJSON: function (o) {
            var f = function (n) {
                return n < 10 ? '0' + n : n;
            },
                meta = {    // table of character substitutions
                    '\b': '\\b',
                    '\t': '\\t',
                    '\n': '\\n',
                    '\f': '\\f',
                    '\r': '\\r',
                    '"': '\\"',
                    '\\': '\\\\'
                },
                escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
                quote = function (value) {
                    escapable.lastIndex = 0;
                    return escapable.test(value) ?
                        '"' + value.replace(escapable, function (a) {
                            var c = meta[a];
                            return typeof c === 'string' ? c :
                                '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                        }) + '"' :
                        '"' + value + '"';
                };
            if (o === null) return 'null';
            var type = typeof o;
            if (type === 'undefined') return undefined;
            if (type === 'string') return quote(o);
            if (type === 'number' || type === 'boolean') return '' + o;
            if (type === 'object') {
                if (o.constructor === Date) {
                    return isFinite(o.valueOf()) ?
                        '"' + o.getFullYear() + '-' +
                        f(o.getMonth() + 1) + '-' +
                        f(o.getDate()) + ' ' +
                        f(o.getHours()) + ':' +
                        f(o.getMinutes()) + ':' +
                        f(o.getSeconds()) + '"' : null;
                }
                if (typeof o._toJSON === 'function') {
                    return this._toJSON(o.toJSON());
                }
                var pairs = [];
                if (o.constructor === Array) {
                    for (var i = 0, l = o.length; i < l; i++) {
                        pairs.push(this._toJSON(o[i]) || 'null');
                    }
                    return '[' + pairs.join(',') + ']';
                }
                var name, val;
                for (var k in o) {
                    type = typeof k;
                    if (type === 'number') {
                        name = '"' + k + '"';
                    } else if (type === 'string') {
                        name = quote(k);
                    } else {
                        continue;
                    }
                    type = typeof o[k];
                    if (type === 'function' || type === 'undefined') {
                        continue;
                    }
                    val = this._toJSON(o[k]);
                    pairs.push(name + ':' + val);
                }
                return '{' + pairs.join(',') + '}';
            }
        },
        get: function (Key) {
            return this._root[Key];
        },
        send: function (BusinessName, ClassName, Method, DBResponse) {
            if (!BusinessName || BusinessName == "") {
                alert("请添加类库名称");
                return;
            }
            if (!ClassName || ClassName == "") {
                alert("请添加类名称");
                return;
            }
            if (!Method || Method == "") {
                alert("请添加方法名称");
                return;
            }
            this.add("typeName", BusinessName + "." + ClassName);
            this.add("Method", Method);
            this.add("types", "Method");
            this.add("BusinessName", BusinessName);
            $.ajax({// ajax Begin
                type: 'post',
                dataType: 'json',
                url: yias.ajax._ajaxurl,
                data: yias.ajax._getXML(),
                contentType: "application/json; charset=utf-8",
                cache: false,
                async: false,
                success: function (rea) {
                    if (rea != null && rea.type != null) {
                        if (rea.type == "error") {
                            if (rea.msg.toLowerCase() == "notlogin") {
                                alert("您的登陆已超时，请重新登陆！");
                                window.top.location = "/login.html";
                            } else
                                alert(rea.msg);
                            return;
                        }
                    }
                    var func = eval(DBResponse);
                    new func(rea);

                }, //success end
                error: function (result) {
                    var obj = null;
                    try {
                        if (result.response) {
                            obj = eval('(' + result.response + ')')
                        } else {
                            obj = eval('(' + result.responseText + ')')
                        }
                        if (obj.Message == "已拒绝为此请求授权。" || obj.ExceptionMessage == "notLogin") {
                            alert("您的登陆已超时，请重新登陆！");
                            window.top.location = "/loginpage.html";
                        } else
                            alert("错误：" + obj.ExceptionMessage);
                    } catch (e) {
                        alert("返回值错误：" + e.message);
                    }
                }
            });
            this.load();
        }
    }
}
else
    alert("尚未加载yias");

