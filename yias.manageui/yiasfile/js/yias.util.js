
function getMyHeaders() {
    return $.cookie('myHeaders');
}
function setMyHeaders(vals) {
    return $.cookie('myHeaders', vals);
}

function toJSON(o) {
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
        if (typeof o.toJSON === 'function') {
            return toJSON(o.toJSON());
        }
        var pairs = [];
        if (o.constructor === Array) {
            for (var i = 0, l = o.length; i < l; i++) {
                pairs.push(toJSON(o[i]) || 'null');
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
            val = toJSON(o[k]);
            pairs.push(name + ':' + val);
        }
        return '{' + pairs.join(',') + '}';
    }
};

if (!getMyHeaders()) {
    setMyHeaders('NotLogin');
}
if (typeof (yias) == "undefined") {
    var yias = {};
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
        }
    }
    yias.ajax = {
        _ajaxurl: "/api/yias/send",
        _root: { "type": "json" },
        load: function () {
            this._root = { "type": "json" };
        },
        add: function (Key, Value) {
            this._root[Key] = Value;
        },
        _getXML: function () {
            var retXml = toJSON(this._root);
            while (retXml.indexOf("'") != -1) {
                retXml = retXml.replace("'", "[+|+]");
            }
            return (retXml + "");
        },
        get: function (Key) {
            return this._root[Key];
        },
        send: function (BusinessName, ClassName, Method, DBResponse, ConfigFileName) {
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
            this.add("key", getMyHeaders());
            $.ajax({// ajax Begin
                type: 'post',
                dataType: 'json',
                url: yias.ajax._ajaxurl,
                data: yias.ajax._getXML(),
                headers: { "Authorization": "Win10UI " + getMyHeaders() },
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

