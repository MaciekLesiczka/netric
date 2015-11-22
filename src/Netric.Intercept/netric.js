(function () {
    var requestId = 'GUID';
    var processTimings = function () {
        //connectEnd instead of requestStart is used. In IE11 timestamp between these values is related with real server side time
        var networkPre = calculateTimings('navigationStart', 'connectEnd'),
            networkPost = calculateTimings('responseStart', 'responseEnd'),
            network = networkPre + networkPost,
            server = calculateTimings('connectEnd', 'responseStart'),
            browser = calculateTimings('responseEnd', 'loadEventEnd');
        return { Network: network, Server: server, Browser: browser, Url: document.URL, Id: requestId };
    };

    var calculateTimings = function (startIndex, finishIndex) {
        return timingsRaw[finishIndex] - timingsRaw[startIndex];
    };

    var timingsRaw = (window.performance
               || window.mozPerformance
               || window.msPerformance
               || window.webkitPerformance
               || {}).timing;

    function post(param, url) {
        var xmlhttp;
        if (window.XMLHttpRequest) {
            xmlhttp = new XMLHttpRequest();
        } else {
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }

        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState == 4) {
                //todo handle errors
            }
        }

        var serialize = function (obj) {
            var str = [];
            for (var p in obj)
                if (obj.hasOwnProperty(p)) {
                    str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                }
            return str.join("&");
        }

        xmlhttp.open("GET", url +'?'+ serialize(param), true);
        xmlhttp.send();
    }

    if (timingsRaw !== undefined) {
        setTimeout(function () {
            post(processTimings(), window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '') + "/" + "netric.axd");
        }, 0);
    }    
})();
