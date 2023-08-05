// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function Format_Integer(classNameGroup, PositveOnly = true) {

    if (PositveOnly) {
        $("." + classNameGroup).on("keypress", function () {
            return (event.charCode >= 48 && event.charCode <= 57)
        });
    }
    else {
        $("." + classNameGroup).on("keypress", function (e) {
            var specialKeys = new Array();
            specialKeys.push(45);
            var keyCode = e.which ? e.which : e.keyCode;
            var ret = ((keyCode >= 48 && keyCode <= 57) || (specialKeys.indexOf(keyCode) != -1));
            return ret;
        });
     }
}

function Format_ZIP(className) {
    $("." + className).each(function () {
        new Cleave(this, {
            numericOnly: true,
            blocks: [5]
        });
    });
}

function Format_PhoneNumber(className) {
    $("." + className).each(function () {
        new Cleave(this, {
            numericOnly: true,
            blocks: [11]
        });
    });
}

function Format_ISBN(className) {
    $("." + className).each(function () {
        new Cleave(this, {
            numericOnly: true,
            blocks: [13]
        });
    });
}




