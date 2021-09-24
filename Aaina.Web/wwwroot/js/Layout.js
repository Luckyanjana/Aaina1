(function ($) {
    function LayoutIndex() {
        var $this = this, form;
        var validationSettings, rowIndex = 0;
        var scrollerPrevious = 0;

        var statusFeedback = function (data) {
            var self = this;
            self.id = ko.observable(0);
            self.gameId = ko.observable('');
            self.subGameId = ko.observable('');
            self.feedback = ko.observable('');
            self.status = ko.observable('');
            self.progress = ko.observable(1);
            self.emotion = ko.observable(1);
            self.gameList = ko.observableArray(gameListArr);
            self.subGameList = ko.observableArray([]);
            self.statusList = ko.observableArray(statusListArr);
            //self.gameList = ko.observableArray([]);
            if (typeof data !== 'undefined') {
                self.id(data.id);
                self.gameId(data.gameId);
                self.subGameId(data.subGameId);
                self.feedback(data.feedback);
                self.status(data.status);
                self.emotion(data.emotion);
            }
        }



        var StatusFeedbackViewModel = function () {
            var self = this;
            self.ststuslistarr = ko.observableArray([]);
            self.ststuslistarr.push(new statusFeedback({ id: '', gameId: '', subGameId: '', feedback: '', status: '', progress: 1, emotion: 1 }));

            self.remove = function (data) {
                self.ststuslistarr.remove(data);
            };
            self.add = function () {
                self.ststuslistarr.push(new statusFeedback({ id: '', gameId: '', subGameId: '', feedback: '', status: '', progress: 1, emotion: 1 }));
                statusFeedbackChange();
            };
        };




        var shareExcel = function (id, lookId, presetId, atterbuteId, filterId, posturl) {
            var users = new Array();
            $("#tbl_share_user tbody tr").each(function () {
                var row = $(this);
                var checkbox = row.find("td").eq(2).find("input[type='checkbox']");
                if (checkbox.prop("checked") == true) {
                    users.push(checkbox.data("email"));
                }
            });
            if (users.length != 0) {
                $.ajax({
                    type: "POST",
                    url: posturl,
                    data: {
                        "users": users,
                        "id": id,
                        "lookId": lookId,
                        "presetId": presetId,
                        "atterbuteId": atterbuteId,
                        "filterId": filterId
                    },
                    success: function (r) {
                        $("#modal-share-users").modal('hide');
                        Global.ToastrSuccess(r.message);
                    }
                });
            } else {
                Global.ToastrWarning("Please Select at Least on User");
            }
        }

        var sharePdf = function (html, posturl) {
            var users = new Array();
            $("#tbl_share_user tbody tr").each(function () {
                var row = $(this);
                var checkbox = row.find("td").eq(2).find("input[type='checkbox']");
                if (checkbox.prop("checked") == true) {
                    users.push(checkbox.data("email"));
                }
            });
            if (users.length != 0) {
                $.ajax({
                    type: "POST",
                    url: posturl,
                    data: {
                        "users": users,
                        "htmlContent": html
                    },
                    success: function (r) {
                        $("#modal-share-users").modal('hide');
                        Global.ToastrSuccess(r.message);
                    }
                });
            } else {
                Global.ToastrWarning("Please Select at Least on User");
            }
        }
        function initilizeUsersModel() {
            Global.ModelHelper($("#modal-share-users"), function () {
                $("#buttons-modal-shareexcel").on('click', function () {
                    var actionurl = $("#actionurl").val();
                    var url = actionurl.substring(0, actionurl.indexOf("?") - 1) + 'Excel';
                    var querystring = actionurl.substring(actionurl.indexOf("?") + 1)
                    var params = parse_query_string(querystring);
                    shareExcel(params.id, params.lookId, params.presetId, params.atterbuteId, params.filterId, url);
                });
                $("#buttons-modal-sharepdf").on('click', function () {
                    debugger
                    var id = $(this).data('id');
                    var $html = $("#" + id).html();
                    $html = $html.replace('display: none;', '');
                    var newHtml = '<html><head><title>PDF Share</title></head><body >' + $html + '</body></html>';
                    var url = '/common/sharePdf/'
                    sharePdf(newHtml, url);
                });
            }, null);
        }

        function initilizeModel() {

            Global.ModelHelper($("#modal-add-edit-agenda_dynamicLink"), function () {
                form = new Global.FormHelper($("#modal-add-edit-agenda_dynamicLink").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-add-edit-agenda_dynamicLink'
                }, null, null);

                $(".session-attend").on('click', function () {
                    var gmeetLink = $(this).data('meet');
                    if (gmeetLink != "") {
                        window.open(gmeetLink, '_blank');
                    }

                    var guringSession = $(this).data('during');

                    if ($(this).hasClass('group-chat-start')) {
                        var chatStart = $(this).data('chatstart');
                        $.get(chatStart, function () {

                            window.location.href = guringSession;
                        })
                    } else {
                        window.location.href = guringSession;
                    }

                   
                   
                })

            }, null);

            Global.ModelHelper($("#modal-status-feedback"), function () {
                form = new Global.FormHelper($("#modal-status-feedback").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-status'
                }, function (response) {
                    $("#modal-status-feedback").modal('hide');
                    Global.ToastrSuccess('Feedback submit successfully');
                }, null);

                ko.applyBindings(new StatusFeedbackViewModel());
                statusFeedbackChange();


            }, null);



            $(".get-notification").on('click', function () {
                var $tbl = $("#notifiation-table tbody");
                $tbl.empty();
                rowIndex = $("#notifiation-table tr").length;
                GetAndBindNotificationMessage();
                $("#modal-notification-message").modal('show');
            });

            $(".share-file-dropbox").on('click', function () {
                Global.ShowLoading();
                var url = $(this).data('url');
                $.get(url, function (response) {
                    $("#message").html(response);
                    $("#message_newtab").attr('href', response);
                    Global.HideLoading();
                    $("#modal-action-share-dropbox").modal('show');
                })
            })

        }

        function parse_query_string(query) {
            var vars = query.split("&");
            var query_string = {};
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                var key = decodeURIComponent(pair[0]);
                var value = decodeURIComponent(pair[1]);
                // If first entry with this name
                if (typeof query_string[key] === "undefined") {
                    query_string[key] = decodeURIComponent(value);
                    // If second entry with this name
                } else if (typeof query_string[key] === "string") {
                    var arr = [query_string[key], decodeURIComponent(value)];
                    query_string[key] = arr;
                    // If third or later entry with this name
                } else {
                    query_string[key].push(decodeURIComponent(value));
                }
            }
            return query_string;
        }
        function GetAndBindNotificationMessage() {
            Global.ShowLoading();
            var $tbl = $("#notifiation-table tbody");
            var page = $("#notification_tbl_count").val();
            $.ajax({
                url: '/home/ShowNotificationMessage?page=' + page,
                type: 'get',
                cache: false,
                success: function (response) {
                    Global.HideLoading();
                    for (var i = 0; i < response.length; i++) {
                        var item = response[i];
                        $('<tr/>', {
                            'style': 'cursor:pointer"',
                            'onclick': `showHideRow("hidden_row${item.id}",this);`,
                            html: function () {
                                $('<td/>', {
                                    html: rowIndex
                                }).appendTo(this);
                                $('<td/>', {
                                    html: item.senderName
                                }).appendTo(this);
                                $('<td/>', {
                                    html: item.sendDate
                                }).appendTo(this);
                                $('<td/>', {
                                    html: item.reason
                                }).appendTo(this);
                                $('<td/>', {
                                    'style': 'font-size:16px;font-weight:bold',
                                    html: '<span style="cursor:pointer;"> + </span>'
                                }).appendTo(this);
                            }
                        }).appendTo($tbl);

                        $('<tr/>', {
                            'id': `hidden_row${item.id}`,
                            'class': 'hidden_row',
                            'style': 'display: none;',
                            html: function () {
                                $('<td/>', {
                                    'colspan': 5,
                                    html: item.message
                                }).appendTo(this);
                            }
                        }).appendTo($tbl);
                        rowIndex++;
                    }
                    $("#notification_tbl_count").val((parseInt(page) + 1));
                }
            });
        }

        function statusFeedbackChange() {

            $(".statusGame").on('change', function () {
                var dataId = $(this).data('id');
                var $subGame = $("#" + dataId);
                $subGame.empty();
                $('<option/>', {
                    'value': '',
                    'html': 'Select'
                }).appendTo($subGame)

                if (parseInt($(this).val()) > 0) {
                    var url = $(this).data('url') + $(this).val();

                    $.get(url, function (response) {
                        for (var i = 0; i < response.length; i++) {
                            var item = response[i];
                            $('<option/>', {
                                'value': item.id,
                                'html': item.name
                            }).appendTo($subGame)
                        }
                    })
                }
            });

            $(document).on('click', '.range_change', function () {
                var emotionId = $(this).attr('id');
                var view_emotion = $(this).data('id');
                var range = document.getElementById(emotionId),
                    rangeV = document.getElementById(view_emotion);
                const
                    newValue = Number((range.value - range.min) * 100 / (range.max - range.min)),
                    newPosition = 10 - (range.value * 5.5);
                rangeV.innerHTML = `<span><img src="${Global.GetEmojiName(range.value)}" class="imgemoji" /></span>`;
                rangeV.style.left = `calc(${newValue}% + (${newPosition}px))`;

            });

            $(document).on('click', '.range_progress', function () {
                var emotionId = $(this).attr('id');
                var view_emotion = $(this).data('id');
                var slider = document.getElementById(emotionId),
                    output = document.getElementById(view_emotion);
                output.innerHTML = slider.value;
                slider.oninput = function () {
                    output.innerHTML = this.value;
                }

            });
        }



        $(".notifiation-table-div").scroll(function () {
            var $this = $(this);
            var $results = $("#notifiation-table");

            var scrolloffset = 20;
            var scrolltop = $(this).scrollTop();
            var scrollheight = $results.height();
            var windowheight = $(window).height();

            if (scrollerPrevious != scrolltop && scrolltop >= (scrollheight - (windowheight + scrolloffset))) {

                if (scrollerPrevious + 40 < scrolltop) {
                    scrollerPrevious = scrolltop;
                    GetAndBindNotificationMessage();

                }
            }

        });

        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };

            initilizeModel();
            initilizeUsersModel();
        }


    }


    $(function () {
        var self = new LayoutIndex();
        self.init();
    })
})(jQuery)