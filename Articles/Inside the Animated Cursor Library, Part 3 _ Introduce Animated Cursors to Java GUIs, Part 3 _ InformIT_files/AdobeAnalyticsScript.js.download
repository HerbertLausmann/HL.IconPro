function GetFilterData() {
    var queryString = window.location.search;
    queryString = queryString.replace('+', ' ');
    var queryStringInfo = queryString.split('&');
    var subjectData = '';
    var typeData = '';
    var publisherData = '';
    var filter = '';

    for (var position in queryStringInfo) {
        var data = queryStringInfo[position];
        if (data.indexOf('subject=') != -1) {
            var arr = data.split('=');
            if (subjectData.length == 0) {
                subjectData = arr[1];
            } else {
                subjectData = subjectData + ',' + arr[1];
            }
        }
    }

    for (var position in queryStringInfo) {
        var data = queryStringInfo[position];
        if (data.indexOf('format=') != -1 || data.indexOf('pageTypes=') != -1) {
            var arr = data.split('=');
            if (typeData.length == 0) {
                typeData = arr[1];
            } else {
                typeData = typeData + ',' + arr[1];
            }
        }
    }

    for (var position in queryStringInfo) {
        var data = queryStringInfo[position];
        if (data.indexOf('imprint=') != -1) {
            var arr = data.split('=');
            if (publisherData.length == 0) {
                publisherData = arr[1];
            } else {
                publisherData = publisherData + ',' + arr[1];
            }
        }
    }

    if (subjectData.length > 0) {
        filter = 'subject:' + decodeURIComponent(subjectData).replace(/\+/g, ' ');
    }

    if (typeData.length > 0) {
        if (filter.length > 0) {
            filter = filter + ',' + 'type:' + decodeURIComponent(typeData).replace(/\+/g, ' ');
        } else {
            filter = 'type:' + decodeURIComponent(typeData).replace(/\+/g, ' ');
        }
    }

    if (publisherData.length > 0) {
        if (filter.length > 0) {
            filter = filter + ',' + 'publisher:' + decodeURIComponent(publisherData).replace(/\+/g, ' ');
        } else {
            filter = 'publisher:' + decodeURIComponent(publisherData).replace(/\+/g, ' ');
        }
    }

    return filter;
}

function CheckFormPage() {
    var arr = window.location.pathname.split('/');

    if (arr[1] == 'promotions' ||
        arr[1] == 'newsletters' ||
        (arr[1] == 'my_account' && arr[2].indexOf('newsletters.aspx') != -1)) {
        return true;
    } else {
        return false;
    }
}

function RemoveDuplicateChildProducts() {
    var arr = [];
    for (i = 0; i < digitalData["products"]["products"].length; i++) {
        var temp = digitalData['products']['products'][i];
        var isRecordAva = 0;
        for (var p = 0; p < arr.length; p++) {
            if (isRecordAva == 0) {
                var nTemp = arr[p];
                if (nTemp["SKU"] == temp["SKU"]) {
                    isRecordAva = 1;
                }
            }
        }

        if (isRecordAva == 0) {
            arr.push(temp);
        }
    }

    digitalData["products"]["products"] = arr;
}

function RemoveParentProducts() {
    var arr = [];
    for (i = 0; i < digitalData["products"]["parentProduct"].length; i++) {
        var temp = digitalData['products']['parentProduct'][i];
        var isRecordAva = 0;
        for (var p = 0; p < arr.length; p++) {
            if (isRecordAva == 0) {
                var nTemp = arr[p];
                if (nTemp["programID"] == temp["programID"]) {
                    isRecordAva = 1;
                }
            }
        }

        if (isRecordAva == 0) {
            arr.push(temp);
        }
    }

    digitalData["products"]["parentProduct"] = arr;
}