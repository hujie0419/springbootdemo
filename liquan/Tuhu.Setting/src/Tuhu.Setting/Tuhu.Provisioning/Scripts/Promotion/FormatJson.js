
function formatJson(json, options) {
    var reg = null,
        formatted = '',
        pad = 0,
        PADDING = '    '; // one can also use '\t' or a different number of spaces
    // optional settings
    options = options || {};
    // remove newline where '{' or '[' follows ':'
    options.newlineAfterColonIfBeforeBraceOrBracket = (options.newlineAfterColonIfBeforeBraceOrBracket === true) ? true : false;
    // use a space after a colon
    options.spaceAfterColon = (options.spaceAfterColon === false) ? false : true;

    // begin formatting...

    // make sure we start with the JSON as a string
    if (typeof json !== 'string') {
        json = JSON.stringify(json);
    }
    // parse and stringify in order to remove extra whitespace
    json = JSON.parse(json);
    json = JSON.stringify(json);

    // add newline before and after curly braces
    reg = /([\{\}])/g;
    json = json.replace(reg, '\r\n$1\r\n');

    // add newline before and after square brackets
    reg = /([\[\]])/g;
    json = json.replace(reg, '\r\n$1\r\n');

    // add newline after comma
    reg = /(\,)/g;
    json = json.replace(reg, '$1\r\n');

    // remove multiple newlines
    reg = /(\r\n\r\n)/g;
    json = json.replace(reg, '\r\n');

    // remove newlines before commas
    reg = /\r\n\,/g;
    json = json.replace(reg, ',');

    // optional formatting...
    if (!options.newlineAfterColonIfBeforeBraceOrBracket) {
        reg = /\:\r\n\{/g;
        json = json.replace(reg, ':{');
        reg = /\:\r\n\[/g;
        json = json.replace(reg, ':[');
    }
    if (options.spaceAfterColon) {
        reg = /\:/g;
        json = json.replace(reg, ': ');
    }

    $.each(json.split('\r\n'), function (index, node) {
        var i = 0,
            indent = 0,
            padding = '';

        if (node.match(/\{$/) || node.match(/\[$/)) {
            indent = 1;
        } else if (node.match(/\}/) || node.match(/\]/)) {
            if (pad !== 0) {
                pad -= 1;
            }
        } else {
            indent = 0;
        }

        for (i = 0; i < pad; i++) {
            padding += PADDING;
        }

        formatted += padding + node + '\r\n';
        pad += indent;
    });

    return formatted;
};

function comparKeys(leftJson, rightJson) {
    var diffKeys = [];
    var allKeys = [];
    for (var key in leftJson) {
        allKeys.push(key);
        if (Object.prototype.toString.call(leftJson[key])=='[object Object]') {
            var keys = comparKeys(leftJson[key], rightJson[key]);
            for (var keyIndex = 0; keyIndex < keys.diff.length; keyIndex++) {
                diffKeys.push(keys.diff[keyIndex]);
            }
            for (var allKeyIndex = 0; allKeyIndex < keys.all.length; allKeyIndex++) {
                allKeys.push(keys.all[allKeyIndex]);
            }
        } else {
            if (JSON.stringify(leftJson[key]) != JSON.stringify(rightJson[key])) {
                diffKeys.push(key);
            }
        }
    }
    return { diff: diffKeys, all: allKeys };
}

function comparJson(leftJson, rightJson) {
    var keys = comparKeys(leftJson,rightJson);
    var diffKeys = keys.diff;
    var allKeys = keys.all;
    
    var fotmatJsonStr = formatJson(JSON.stringify(leftJson));
    var leftArr = fotmatJsonStr.split('\n');
    var formatJsonRightStr = formatJson(JSON.stringify(rightJson));
    var rightArr = formatJsonRightStr.split('\n');
    for (var i = 0; i < leftArr.length; i++) {
        var diff = false;
        for (var j = 0; j < diffKeys.length; j++) {
            if (leftArr[i].indexOf('"' + diffKeys[j] + '"') !== -1) {
                diff = true;
                break;
            }
        }
        if (diff) {
            leftArr[i] = "<font style='color:red;'>" + leftArr[i] + "</font>";
            while (true) {
                var nextDiff = true;
                for (var k = 0; k < allKeys.length; k++) {
                    if (leftArr.length > (i + 1) && leftArr[i + 1].indexOf('"' + allKeys[k] + '"') === 4
                    ) { //如果所有的key里面没有包含，说明是上一个diff
                        nextDiff = false;
                    }
                }
                if (nextDiff) {
                    i++;
                    if (i >= leftArr.length) break;
                    if (leftArr[i] !== rightArr[i])
                        leftArr[i] = "<font style='color:red;'>" + leftArr[i] + "</font>";
                } else {
                    break;
                }
            }
        }
    }
    return leftArr.join("");
}

$(function () {
    var formatJsonDialog = $("<div/>").attr("id", "formatJsonDialog");
    $("body").append(formatJsonDialog);
    $("#formatJsonDialog").html('<pre id="formatJsonDialogContent" readonly="readonly" style="width:100%; height: auto;"></pre>');
    $("body").delegate("a[data-type=viewPromotionLogDetail]",
        "click",
        function () {
            var leftJson = $(this).data("json");

            var beforeJson = $(this).data("beforejson");
            var formatJsonStr = '';
            if (beforeJson != '') {
                formatJsonStr = comparJson(leftJson, beforeJson);
            } else {
                formatJsonStr = formatJson(leftJson);
            }

            $("#formatJsonDialogContent").html(formatJsonStr);
            $("#formatJsonDialog").dialog({
                title: "操作历史",
                width: 600,
                height: 'auto',
                top: "20%"
        });
        });
});
