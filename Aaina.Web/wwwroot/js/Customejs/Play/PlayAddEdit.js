(function ($) {
    function FilterAddEdit() {
        var $this = this, form;
        var validationSettings;

        function initilizeModel() {

            form = new Global.FormHelper($("#frm-add-edit-play").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            $('.datepicker').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });


            $('#StartDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var minDate = new Date(selected.date.valueOf());
                $('#DeadlineDate').datepicker('setStartDate', minDate);
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $('#DeadlineDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var maxDate = new Date(selected.date.valueOf());
                $('#StartDate').datepicker('setEndDate', new Date(maxDate));
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });



            if ($('#DeadlineDate').val() != "") {
                var minDate = new Date(moment($("#DeadlineDate").val(), "DD/MM/YYYY"));
                $('#StartDate').datepicker('setEndDate', minDate);
            }

            if ($('#StartDate').val() != "") {
                var minDate = new Date(moment($("#StartDate").val(), "DD/MM/YYYY"));
                $('#DeadlineDate').datepicker('setStartDate', minDate);
            }

            let emojifeedback = Emotion;
            var newValue = Number((emojifeedback - 1) * 100 / (10 - 1))
            var newPosition = 10 - (emojifeedback * 5.5);

            $("#view_emotion").attr('style', `left:calc(${newValue}% + (${newPosition}px))`);



            $("#PersonInvolved").select2();

            $("#GameId").on('change', function () {
                var forId = $("#SubGameId");
                forId.empty();
                $('<option/>', {
                    'value': '',
                    'html': 'Select'
                }).appendTo(forId)

                if (parseInt($(this).val()) > 0) {
                    var url = $(this).data('url') + $(this).val();

                    $.get(url, function (response) {
                        for (var i = 0; i < response.length; i++) {
                            var item = response[i];
                            $('<option/>', {
                                'value': item.id,
                                'html': item.name
                            }).appendTo(forId)
                        }
                    })
                }
            });


            $(document).on('click', '.range_change', function () {

                var range = document.getElementById('Emotion'),
                    rangeV = document.getElementById('view_emotion');
                const
                    newValue = Number((range.value - range.min) * 100 / (range.max - range.min)),
                    newPosition = 10 - (range.value * 5.5);
                rangeV.innerHTML = `<span><img src="${Global.GetEmojiName(range.value)}" class="imgemoji" /></span>`;
                rangeV.style.left = `calc(${newValue}% + (${newPosition}px))`;

            });
        }


        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };

            initilizeModel();
        }

        $(document).on('click', '#btnFeedback', function () {
            LoadSlider(0);
            $("#modal-add-feedback").modal('show');

        });


        function LoadSlider(Id) {

            $("#feedbackid").val(Id);


            var $div = $("#play_feedback_div");
            $div.empty();

            //var subAtter = subAtterFeedArr[i];
            //var feedback = GetFeedback(gameFeedbackDetailList, gameid, attrid, subAtter.SubAttributeId);
            //let emojifeedback = feedback != '' ? feedback[0].Feedback : 1;
            //let quantity = feedback != '' ? feedback[0].Quantity : '';
            //let percent = feedback != '' ? feedback[0].Percentage : '0';
            //// var emoji = GetEmojiList(emojiList, emojifeedback);
            var newValue = Number((0 - 1) * 100 / (10 - 1))
            var newPosition = 10 - (0 * 5.5);

            $('<div/>', {
                'class': 'row wrap_input',
                'style': 'margin-top: 10px;',
                'html': function () {
                    $('<div/>', {
                        'class': 'col-md-4',
                        'html': function () {
                            $('<label/>', {
                                'html': "Emotions"
                            }).appendTo(this);
                        }

                    }).appendTo(this);

                    var col = 6;

                    $('<div/>', {
                        'class': `col-md-${col}`,
                        'html': function () {
                            $('<div/>', {
                                'class': 'range-wrap',
                                'html': function () {
                                    $('<div/>', {
                                        'class': 'range-value',
                                        'style': `left:calc(${newValue}% + (${newPosition}px))`,
                                        'id': `rangeV_${1}`,
                                        'html': function () {
                                            $('<span/>', {
                                                'html': `<img src="${Global.GetEmojiName(0)}" class="imgemoji" />`
                                            }).appendTo(this);
                                        }
                                    }).appendTo(this);



                                    //$('<input/>', {
                                    //    'type': 'hidden',
                                    //    'class': 'subattribute',
                                    //    'id': `subattribute_${1}`,
                                    //    'value': 1
                                    //}).appendTo(this)

                                    //$('<input/>', {
                                    //    'type': 'hidden',
                                    //    'class': 'feedbackatterId',
                                    //    'id': `feedbackatterId_${subAtter.Id}`,
                                    //    'value': subAtter.Id
                                    //}).appendTo(this)

                                    $('<input/>', {
                                        'type': 'range',
                                        'class': ' range_change_emoji',
                                        'id': 'range_1',
                                        'data-id': 1,
                                        'min': '1',
                                        'max': '10',
                                        'value': 0,
                                        'step': '1'
                                    }).appendTo(this)

                                }
                            }).appendTo(this);

                            $('<p/>', {
                                'html': ""
                            }).appendTo(this)
                        }

                    }).appendTo(this);
                }
            }).appendTo($div);

            $('<div/>', {
                'class': 'row ',
                'style': 'margin-top: 10px;',
                'html': function () {
                    $('<div/>', {
                        'class': 'col-md-4',
                        'html': function () {
                            $('<label/>', {
                                //'html': subAtter.Name
                                'html': "Complition"
                            }).appendTo(this);
                        }
                    }).appendTo(this);

                    var col = 6;

                    $('<div/>', {
                        'class': `col-md-${col}`,
                        'html': function () {
                            $('<div/>', {
                                'class': 'range-wrap',
                                'html': function () {
                                    $('<div/>', {
                                        'class': 'range-value',
                                        'style': `left:calc(${0}% + (${newPosition}px))`,
                                        'id': `rangeV_${1}`,
                                        'html': function () {
                                            $('<span/>', {
                                                'html': 0 + `%`,
                                                'class': 'span-range-value-percent',
                                            }).appendTo(this);
                                        }
                                    }).appendTo(this);


                                    $('<input/>', {
                                        'type': 'range',
                                        'class': ' range_change_persent',
                                        'id': 'range_' + 0,
                                        'data-id': 0,
                                        'min': '0',
                                        'max': '10',
                                        'value': 0,
                                        'step': '0'
                                    }).appendTo(this)

                                }
                            }).appendTo(this);

                            $('<p/>', {
                                'html': ""
                            }).appendTo(this)
                        }

                    }).appendTo(this);
                }
            }).appendTo($div);


        };
    }

    function GetPlayFeedbackList(subAtterArr, atterId) {
        if (atterId) {
            var result = $.grep(subAtterArr, function (e) { return e.AttributeId == atterId; });
            if (result != null && result.length > 0)
                return result;
            else
                return '';
        } else {
            return '';
        }
    }

    $(document).on('click', '.range_change_emoji', function () {
        var attrid = parseInt($(this).data('id'));
        var range = document.getElementById('range_' + attrid),
            rangeV = document.getElementById('rangeV_' + attrid);
        const
            newValue = Number((range.value - range.min) * 100 / (range.max - range.min)),
            newPosition = 10 - (range.value * 5.5);
        // var emoji = GetEmojiList(emojiList, range.value);
        rangeV.innerHTML = `<span><img src="${Global.GetEmojiName(range.value)}" class="imgemoji" /></span>`;
        rangeV.style.left = `calc(${newValue}% + (${newPosition}px))`;

        //document.addEventListener("DOMContentLoaded", setValue);
        //range.addEventListener('input', setValue);
    });


    $(document).on('change', '.range_change_persent', function () {
        var value = $(this).val();
        var Persentage = "0%";
        if (value == 0) {
            Persentage = Persentage;
        } else {
            Persentage = parseFloat(value) * 10 + "%";
        }
        $(".span-range-value-percent").html(Persentage);
    });


    $(document).on('click', '#btnsubmitFeedBack', function () {
        var url = $(this).data('url');
        var playid = $("#feedbackplayid").val();

        var playstatus = $("#StatusID").val();
        var FeedbackComment = $("#FeedbackComment").val();
        var FeedbackPriority = $("#FeedbackPriority").val();
        var FeedbackStatus = $("#FeedbackStatus").val();

        var emoji = $('.range_change_emoji').val();
        var Percentage = $(".span-range-value-percent").html().replace('%', '');;

        if (FeedbackComment == "") {
            alert('comment is required');
        }

        var data = {
            PlayId: playid,
            FeedbackStatus: FeedbackStatus,
            playstatus: playstatus,
            FeedbackPriority: FeedbackPriority,
            Description: FeedbackComment,
            emoji: emoji,
            Percentage: Percentage
        };
        UpdateFeedBackData(url, data)


    });

    function UpdateFeedBackData(url, dataRequest) {
        Global.ShowLoading();
        $.ajax(url, {
            type: "POST",
            data: dataRequest,
            success: function (result) {
                window.location.reload();
            },
            error: function (jqXHR, status, error) {

                if (onError !== null && onError !== undefined) {
                    onError(jqXHR, status, error);
                } else {


                }
                Global.HideLoading();
            }, complete: function () {
                Global.HideLoading();
            }
        });
    }

    $(function () {
        var self = new FilterAddEdit();
        self.init();
    })
})(jQuery)