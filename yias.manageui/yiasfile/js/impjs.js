var basefile = "yiasfile";
var scripturl = [
    "<script src='/" + basefile + "/js/jquery-2.2.4.min.js'></script>",
    "<script src='/" + basefile + "/js/jquery.cookie.js'></script>",
    "<script src='/" + basefile + "/js/yias.util.js'></script>",
];

function ImputScript() {
    for (var i = 0; i < scripturl.length; i++) {
        document.write(scripturl[i]);
    }
}
