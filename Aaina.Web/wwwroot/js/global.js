

/*global window, $*/
var Global = {};
Global.FormHelper = function (formElement, options, onSucccess, onError) {
    
    //"use strict";
    var settings = {};
    settings = $.extend({}, settings, options);
    $.validator.unobtrusive.parse(formElement);
    formElement.validate(settings.validateSettings);
    formElement.submit(function (e) {
        var submitBtn = formElement.find(':submit');
        if (formElement.validate().valid()) {
            Global.ShowLoading();
            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formElement.serializeArray(),
                beforeSend: function (xhr) {
                    $(':input[type="submit"]').prop('disabled', true);
                },
                success: function (result) {
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            if (settings.refreshGrid) {
                                if (settings.modelId) {
                                    $("#" + settings.modelId).modal('hide');
                                }
                                if (result.message)
                                    Global.ToastrSuccess(result.message);
                                if ($('[data-table="grid"]').length >= 1) {
                                    $('[data-table="grid"]').DataTable().ajax.reload();
                                }
                            } else {
                                                              
                                window.location.href = result.redirectUrl;
                            }
                        } else {

                            if (settings.updateTargetId) {
                                if (result.data == undefined) {
                                    formElement.find("#" + settings.updateTargetId).html("<span>" + result + "</span>");
                                }
                                else {
                                    formElement.find("#" + settings.updateTargetId).html("<span>" + result.message + "</span>");
                                }
                            }
                        }
                    } else {

                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {

                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                    } else {
                        
                        var $div = $("#validation-summary");
                        $('<div/>', {
                            'class': 'validation-summary-errors',
                            'data-valmsg-summary': 'true',
                            'html': function () {
                                $('<ul/>', {
                                    'html': function () {
                                        $('<li/>', {
                                            'html': jqXHR.responseJSON.message
                                        }).appendTo(this);

                                        //$('<li/>', {
                                        //    'html': jqXHR.responseJSON.data
                                        //}).appendTo(this);
                                    }
                                }).appendTo(this);
                            }
                        }).appendTo($div);
                        
                    }
                    Global.HideLoading();
                    $(':input[type="submit"]').prop('disabled', false);
                }, complete: function () {
                    Global.HideLoading();
                    $(':input[type="submit"]').prop('disabled', false);
                }
            });
        }
        e.preventDefault();
    });

    return formElement;
};

Global.FormHelperWithFiles = function (formElement, options, onSucccess, onError, loadingElementId, onComplete) {
    "use strict";
    var settings = {};

    settings = $.extend({}, settings, options);
    formElement.validate(settings.validateSettings);
    formElement.submit(function (e) {

        var formdata = new FormData();
        formElement.find('input[type="file"]').each(function () {

            var elem = document.getElementById($(this).attr('id'));
            if (elem != undefined && elem.files != undefined && elem.files.length > 0) {
                for (var i = 0; i < elem.files.length; i++) {
                    var file = elem.files[i];
                    formdata.append($(this).attr('name'), file);
                }
            }
        });

        $.each(formElement.serializeArray(), function (i, item) {
            formdata.append(item.name, item.value);
        });


        var submitBtn = formElement.find('#btn-submit');
        if (formElement.validate().valid()) {
            Global.ShowLoading();
            submitBtn.find('i').removeClass("fa fa-arrow-circle-right");
            submitBtn.find('i').addClass("fa fa-refresh");
            submitBtn.prop('disabled', true);
            submitBtn.find('span').html('Submiting..');
            $.ajax(formElement.attr("action"), {
                type: "POST",
                data: formdata,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                        $("#" + settings.loadingElementId).show();
                        submitBtn.hide();
                    }
                },
                success: function (result) {
                    if (onSucccess === null || onSucccess === undefined) {
                        if (result.isSuccess) {
                            $(window).unbind('beforeunload');
                            if (settings.refreshGrid) {
                                if (settings.modelId) {
                                    $("#" + settings.modelId).modal('hide');
                                }
                                if (result.message)
                                    Global.ToastrSuccess(result.message);
                                if ($('[data-table="grid"]').length >= 1) {
                                    $('[data-table="grid"]').DataTable().ajax.reload();
                                }
                            } else {
                                
                                window.location.href = result.redirectUrl;
                            }
                        } else {
                            if (settings.updateTargetId) {
                                var datatresult = (result.message == null || result.message == undefined) ? ((result.data == null || result.data == undefined) ? result : result.data) : result.message;
                                $("#" + settings.updateTargetId).html(datatresult);
                            }
                        }
                    } else {
                        onSucccess(result);
                    }
                },
                error: function (jqXHR, status, error) {
                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                        Global.HideLoading();
                    }
                },
                complete: function (result) {
                    if (onComplete === null || onComplete === undefined) {
                        if (settings.loadingElementId != null || settings.loadingElementId != undefined) {
                            $("#" + settings.loadingElementId).hide();
                        }
                        submitBtn.find('i').removeClass("fa fa-refresh");
                        submitBtn.find('i').addClass("fa fa-arrow-circle-right");
                        submitBtn.find('span').html('Submit');
                        submitBtn.prop('disabled', false);
                    } else {
                        onComplete(result);
                    }
                    Global.HideLoading();
                }
            });
        }

        e.preventDefault();
    });

    return formElement;
};



Global.FormValidationReset = function (formElement, validateOption) {
    if ($(formElement).data('validator')) {
        $(formElement).data('validator', null);
    }

    $(formElement).validate(validateOption);
    return $(formElement);
};

Global.DataTable = function (formElement) {
   var table= $(formElement).dataTable({
        "searching": true,
        "paging": true,
       "ordering": true,
       
       "dom": 'Bfrtip',
       "buttons": ['csv', 'excel', 'pdf', 'print'],
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });
    return table;
};

Global.DataTableWithOutPage = function (formElement) {
    var table = $(formElement).dataTable({
        "searching": true,
        "paging": true,
        "ordering": true,
        "fixedHeader": true,
        "dom": 'Bfrtip',
        "buttons": ['csv', 'excel', 'pdf', 'print']
        
    });
    return table;
};

Global.DataTableWithOutPageOrder = function (formElement) {
    var table = $(formElement).dataTable({
        "searching": true,
        "paging": false,
        "ordering": true,
        "dom": 'Bfrtip',
        "buttons": ['csv', 'excel', 'pdf', 'print'],
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });
    return table;
};

Global.DataTableWithSearchOnly = function (formElement) {
    var table = $(formElement).dataTable({
        "searching": true,
        "paging": false,
        "ordering": true,
        "dom": 'Bfrtip',
        "buttons": [],
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });
    return table;
};


Global.DataTable = function (formElement) {
    var table = $(formElement).dataTable({
        "searching": true,
        "paging": true,
        "ordering": true,
        "dom": 'Bfrtip',
        "buttons": ['csv', 'excel', 'pdf', 'print'],
        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });
    return table;
};

Global.DataTableWithSearch = function (formElement) {
    var table = $(formElement).dataTable({
        "searching": true,
        "paging": false,
        "ordering": true            
    });
    return table;
};

Global.DateProcess = function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}

Date.prototype.isSameDateAs = function (pDate) {
    return (
        this.getFullYear() === pDate.getFullYear() &&
        this.getMonth() === pDate.getMonth() &&
        this.getDate() === pDate.getDate()
    );
}

Global.ShowLoading = function (setting) {
    setting = jQuery.extend({
        effect: 'bounce',
        text: '',
        bg: 'rgba(255, 255, 255, 0.7)',
        color: '#000',
        maxSize: '',
        waitTime: -1,
        textPos: 'vertical',
        fontSize: '',
        source: '',
        onClose: function () { },
        dvContent: 'dvWaitMe'
    }, setting);

    var $container = $('#' + setting.dvContent);
    if ($container.length) {
        //clear if already opened.
        $container.removeAttr('class').hide().children().remove();
        $container.css({
            'width': '100%',
            'height': '100%',
            'position': 'fixed',
            'top': '0',
            'left': '0',
            'z-index': '99999',
            'display': 'none'
        });
        $('body').addClass('ajax-waitme');
        $container.waitMe({
            effect: setting.Effect,
            text: setting.Text,
            bg: setting.background,
            color: setting.ColorCode,
            sizeW: setting.SizeW,
            sizeH: setting.SizeH
        });
        $container.show();
    }
};

Global.HideLoading = function (setting) {
    setting = jQuery.extend({ dvContent: 'dvWaitMe' }, setting);
    var $container = $('#' + setting.dvContent);
    $container.removeAttr('class').hide().children().remove();
    $('body').removeClass('ajax-waitme');
};

Global.ShowSucessMessage = function (message) {
    var $html = '<div class="alert alert-success alert-dismissable">' +
        '<button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>' + message + '</div>';
    $("#notificationMessage").html($html);
}

Global.ShowErrorMessage = function (message) {
    var $html = '<div class="alert alert-danger alert-dismissable">' +
        '<button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>' + message + '</div>';
    $("#notificationMessage").html($html);
}

Global.GetEmojiName =function (rating) {
    if (rating) {
        var result = $.grep(emojiArrList, function (e) { return e.Rating == rating});
        if (result != null && result.length > 0)
            return `/DYF/${companyId}/EmojiImages/${result[0].Emoji}`;
        else
            return `/DYF/${companyId}/EmojiImages/1.png`;
    } else {
        return `/DYF/${companyId}/EmojiImages/1.png`;
    }
}

Global.GetEmojiNameMini = function (rating) {
    if (rating) {
        var result = $.grep(emojiArrList, function (e) { return e.Rating == rating });
        if (result != null && result.length > 0) {
            var emoji = result[0].Emoji;
            return `/DYF/${companyId}/EmojiImages/${emoji.Split('.')[0]}-mini.${emoji.Split('.')[1]}`;
        }
        else
            return `/DYF/${companyId}/EmojiImages/1-mini.png`;
    } else {
        return `/DYF/${companyId}/EmojiImages/1-mini.png`;
    }
}

Global.showSessionPendingMsgCount = function () {

    $.ajax({
        url: '/home/messageCount',
        type: 'get',
        cache: false,
        success: function (msgCount) {
            if (parseInt(msgCount) > 0) {
                $('.pending-msg').text(msgCount);
            }
        }
    });

    setTimeout(function () {
        $.ajax({
            url: '/session/sessionMessageCount',
            type: 'get',
            cache: false,
            success: function (msgCount) {
                if (parseInt(msgCount) > 0) {
                    $('.session-pending-msg').text(msgCount);
                }
            }
        });
    }, 800);
}

Global.ValidateImage = function (input, fileId, displayId) {
    var reader = new FileReader();
    if (input.files[0].size > 528385) {
        $("#" + fileId).val('');

        alert("Image Size should not be greater than 500Kb");

        $("#" + fileId).val('');
        return false;
    }

    if (input.files[0].type.indexOf("image/jpeg") == -1 && input.files[0].type.indexOf("image/png") == -1) {
        alert("Invalid format");
        $("#" + fileId).val('');
        return false;
    }


    reader.onload = function (e) {
        debugger;
        $("#" + displayId).attr("src", e.target.result);
    };
    // read the image file as a data URL.
    reader.readAsDataURL(input.files[0]);
}

Global.DocumentValidate = function (input, fileId) {

    var reader = new FileReader();
    if (input.files[0].size > 528385) {
        $("#" + fileId).val('');
        alert("file Size should not be greater than 500Kb");
        $("#" + fileId).val('');
        return false;
    }
    if (input.files[0].type.indexOf("application/pdf") == -1) {
        alert("Invalid file");
        $("#" + fileId).val('');
        return false;
    }
    // read the image file as a data URL.
    reader.readAsDataURL(input.files[0]);
}

Global.GenerateGuid = function () {
    var d = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
};

Global.Print = function (html, title) {
    var mywindow = window.open('', 'new div', 'height=400,width=600');
    mywindow.document.write('<html><head><title>' + title + '</title>');
    mywindow.document.write('</head><body >');
    mywindow.document.writeln(html);
    mywindow.document.write('</body></html>');
    mywindow.print();
    mywindow.close();
    return true;
};

Global.Pdf = function (html) {
    var pdfForm = $('#pdfForm');
    pdfForm.find("input[name='htmlContent']").val(html);
    pdfForm.submit();
};

Global.Excel = function (html) {
    var pdfForm = $('#excelForm');
    pdfForm.find("input[name='htmlContent']").val(html);
    pdfForm.submit();
};

Global.ToastrSuccess = function (message) {
    toastr.success(message)
}

Global.ToastrInfo = function (message) {
    toastr.info(message)
}

Global.ToastrError = function (message) {
    toastr.error(message)
}

Global.ToastrWarning = function (message) {
    toastr.warning(message)
}
Global.AddUpdateQueryStringParameter = function (uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        return uri + separator + key + "=" + value;
    }
};

Global.ModelHelper = function (modelElement, onSucess, OnError) {
    $(modelElement).on('show.bs.modal', function (event) {
        
        var button = $(event.relatedTarget); // Button that triggered the modal
        var url = button.attr("href");
        var modal = $(this);
        if (url !== "" && url != undefined) {
            // note that this will replace the content of modal-content everytime the modal is opened
            modal.find('.modal-content').load(url, function (response, status, xhr) {
                if (status == "success") {
                    onSucess(response)
                } else {
                    modal.find('.modal-content').html(response);
                    OnError(response);
                    if (onError !== null && onError !== undefined) {
                        onError(response);
                    } else {                        
                        var $div = $("#validation-summary");
                        $('<div/>', {
                            'class': 'validation-summary-errors',
                            'data-valmsg-summary': 'true',
                            'html': function () {
                                $('<ul/>', {
                                    'html': function () {
                                        $('<li/>', {
                                            'html': jqXHR.responseJSON.message
                                        }).appendTo(this);

                                        //$('<li/>', {
                                        //    'html': jqXHR.responseJSON.data
                                        //}).appendTo(this);
                                    }
                                }).appendTo(this);
                            }
                        }).appendTo($div);

                    }
                }
            });
        }
    }).on('hidden.bs.modal', function (e) {
        $(this).removeData('bs.modal');
        $(this).find(".modal-content").empty();
    });
    return modelElement;
};

$(document).on('keypress', '.pincode', function (event) {
    //Added by arnav
    if ((event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();

    }
    
});

$(document).on('keypress', '.number', function (event) {
    //Added by arnav
    if ((event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});


$(document).on('keypress', '.decimal', function (event) {
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

$(document).on('keypress', '.decimalNegative', function (event) {
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which != 45 || $(this).val().indexOf('-') != -1) && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 0) {
        event.preventDefault();
    }
});

$(document).on('change', '.number, .decimal, .decimalNegative', function () { if ($(this).val() != "0" && $(this).val() != "" && !$.isNumeric($(this).val())) { alert("Please enter a valid value"); return false; } });

$(document).on('focus', '.number, .decimal, .decimalNegative', function (event) {
    var default_value = 0;
    if ($(this).val() == default_value) $(this).val("");
});

$(document).on('blur', '.number, .decimal, .decimalNegative', function (event) {
    var default_value = 0;
    if ($(this).val().length == 0) $(this).val(default_value);
});

$(document).on('bind', '.disablecopy', function (e) {
    e.preventDefault();
});

$(document).on('cut copy paste', '.disablecopy', function (e) {
    e.preventDefault();
});

$(".disableRightClick").on("contextmenu", function (e) {
    return false;
});

