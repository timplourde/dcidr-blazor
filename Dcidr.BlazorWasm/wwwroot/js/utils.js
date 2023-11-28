(function () {


    function applyResultBarWidths(resultBarSelector, reportBarWidths) {
        if (!reportBarWidths) return;
        document.querySelectorAll(resultBarSelector).forEach((el, key) => {
            setTimeout(() => {
                el.setAttribute('style', 'width: ' + reportBarWidths[key]);
            }, 200 * key);
        });
    }

    function saveAsFile(filename, bytesBase64) {
        var link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Needed for Firefox
        link.click();
        document.body.removeChild(link);
    }

    window.dcidr = {
        interop: {
            applyResultBarWidths,
            saveAsFile
        }
    };
})();