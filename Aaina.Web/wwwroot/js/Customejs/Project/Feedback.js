

(function ($) {
    function feedbackAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        function initilizeModel() {

            form = new Global.FormHelper($("#frm-add-edit-feedback").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            $("#lookId").on('change', function () {
                var id = $(this).val();
                var gameId = $("#GameId").val();
                window.location.href = '/project/feedback?gid=' + gameId + '&lookId=' + id;
            });

            var $tblhead = $("#tbl_game thead");
            $('<tr/>', {
                html: function () {
                    $('<th/>', {
                        'html': 'Id'
                    }).appendTo(this);

                    $('<th/>', {
                        'html': 'Games'
                    }).appendTo(this);

                    for (var i = 0; i < attributeList.length; i++) {
                        var attr = attributeList[i];
                        $('<th/>', {
                            'html': attr.Name
                        }).appendTo(this);
                    }

                }

            }).appendTo($tblhead);

            BindGameTable(allGame, '', '');


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

            $(document).on('click', '.attr_feedback', function () {
                var attrid = parseInt($(this).data('attrid'));
                var attrname = $(this).data('attrname');
                var attrdesc = $(this).data('attrdesc');
                var gameid = $(this).data('gameid');
                var gamename = $(this).data('gamename');

                $("#sub_title").html(gamename + ">>" + attrname);
                $("#attrid").val(attrid);
                $("#gameid").val(gameid);
                var subAtterFeedArr = GetSubAtterList(subAttributeList, attrid);

                var $div = $("#sub_attr_feedback_div");
                $div.empty();
                if (subAtterFeedArr != '' && subAtterFeedArr.length > 0) {

                    for (var i = 0; i < subAtterFeedArr.length; i++) {
                        var subAtter = subAtterFeedArr[i];
                        var feedback = GetFeedback(gameFeedbackDetailList, gameid, attrid, subAtter.SubAttributeId);
                        let emojifeedback = feedback != '' ? feedback[0].Feedback : 1;
                        let quantity = feedback != '' ? feedback[0].Quantity : '';
                        let percent = feedback != '' ? feedback[0].Percentage : '0';
                        // var emoji = GetEmojiList(emojiList, emojifeedback);
                        var newValue = Number((emojifeedback - 1) * 100 / (10 - 1))
                        var newPosition = 10 - (emojifeedback * 5.5);

                        $('<div/>', {
                            'class': 'row ',
                            'style': 'margin-top: 10px;',
                            'html': function () {
                                $('<div/>', {
                                    'class': 'col-md-4',
                                    'html': function () {
                                        $('<label/>', {
                                            //'html': subAtter.Name
                                            'html': subAtter.Name
                                        }).appendTo(this);
                                    }

                                }).appendTo(this);

                                $('<div/>', {
                                    'class': 'col-md-8',
                                    'html': function () {
                                        $('<p/>', {
                                            'html': attrdesc
                                        }).appendTo(this);
                                    }

                                }).appendTo(this);

                            }
                        }).appendTo($div);

                        $('<div/>', {
                            'class': 'row wrap_input',
                            'style': 'margin-top: 30px;',
                            'html': function () {
                                $('<div/>', {
                                    'class': 'col-md-4',
                                    'html': function () {
                                        $('<label/>', {
                                            'html': 'Emotion'
                                        }).appendTo(this);
                                    }

                                }).appendTo(this);

                                var col = subAtter.IsQuantity ? 6 : 8;

                                $('<div/>', {
                                    'class': `col-md-${col}`,
                                    'html': function () {
                                        $('<div/>', {
                                            'class': 'range-wrap',
                                            'html': function () {
                                                $('<div/>', {
                                                    'class': 'range-value',
                                                    'style': `left:calc(${newValue}% + (${newPosition}px))`,
                                                    'id': `rangeV_${subAtter.SubAttributeId}`,
                                                    'html': function () {
                                                        $('<span/>', {
                                                            'html': `<img src="${Global.GetEmojiName(emojifeedback)}" class="imgemoji" />`
                                                        }).appendTo(this);
                                                    }
                                                }).appendTo(this);



                                                $('<input/>', {
                                                    'type': 'hidden',
                                                    'class': 'subattribute',
                                                    'id': `subattribute_${subAtter.SubAttributeId}`,
                                                    'value': subAtter.SubAttributeId
                                                }).appendTo(this)

                                                $('<input/>', {
                                                    'type': 'hidden',
                                                    'class': 'feedbackatterId',
                                                    'id': `feedbackatterId_${subAtter.Id}`,
                                                    'value': subAtter.Id
                                                }).appendTo(this)

                                                $('<input/>', {
                                                    'type': 'range',
                                                    'class': 'range_change',
                                                    'id': 'range_' + subAtter.SubAttributeId,
                                                    'data-id': subAtter.SubAttributeId,
                                                    'min': '1',
                                                    'max': '10',
                                                    'value': emojifeedback,
                                                    'step': '1'
                                                }).appendTo(this)

                                            }
                                        }).appendTo(this);

                                        $('<p/>', {
                                            'html': subAtter.SubAttributeDesc
                                        }).appendTo(this)
                                    }

                                }).appendTo(this);

                                if (subAtter.IsQuantity) {

                                    $('<div/>', {
                                        'class': 'col-md-2',
                                        'html': function () {
                                            $('<input/>', {
                                                'type': 'text',
                                                'class': 'form-control input_quantity',
                                                'id': 'input_' + subAtter.SubAttributeId,
                                                'value': quantity
                                            }).appendTo(this)
                                            $('<span/>', {
                                                'html': subAtter.Unit
                                            }).appendTo(this);
                                        }

                                    }).appendTo(this);
                                }

                            }
                        }).appendTo($div);

                        if (i == (subAtterFeedArr.length-1)) {
                            $('<div/>', {
                                'class': 'row ',
                                'style': 'margin-top: 10px;',
                                'html': function () {
                                    $('<div/>', {
                                        'class': 'col-md-4',
                                        'html': function () {
                                            $('<label/>', {
                                                //'html': subAtter.Name
                                                'html': "Confidance level"
                                            }).appendTo(this);
                                        }
                                    }).appendTo(this);

                                    var col = subAtter.IsQuantity ? 6 : 8;

                                    $('<div/>', {
                                        'class': `col-md-${col}`,
                                        'html': function () {
                                            $('<div/>', {
                                                'class': 'range-wrap',
                                                'html': function () {
                                                    $('<div/>', {
                                                        'class': 'range-value',
                                                        'style': `left:calc(${percent}% + (${newPosition}px))`,
                                                        'id': `rangeV_${subAtter.SubAttributeId}`,
                                                        'html': function () {
                                                            $('<span/>', {
                                                                'html': percent + `%`,
                                                                'class': 'span-range-value-percent',
                                                            }).appendTo(this);
                                                        }
                                                    }).appendTo(this);


                                                    //$('<input/>', {
                                                    //    'type': 'hidden',
                                                    //    'class': 'subattribute',
                                                    //    'id': `subattribute_${subAtter.SubAttributeId}`,
                                                    //    'value': subAtter.SubAttributeId
                                                    //}).appendTo(this)

                                                    //$('<input/>', {
                                                    //    'type': 'hidden',
                                                    //    'class': 'feedbackatterId',
                                                    //    'id': `feedbackatterId_${subAtter.Id}`,
                                                    //    'value': subAtter.Id
                                                    //}).appendTo(this)

                                                    $('<input/>', {
                                                        'type': 'range',
                                                        'class': 'range_change range_change_persent',
                                                        'id': 'range_' + subAtter.SubAttributeId,
                                                        'data-id': subAtter.SubAttributeId,
                                                        'min': '0',
                                                        'max': '10',
                                                        'value': (percent == '0' ? '0' : (percent / 10)),
                                                        'step': '0'
                                                    }).appendTo(this)

                                                }
                                            }).appendTo(this);

                                            $('<p/>', {
                                                'html': subAtter.SubAttributeDesc
                                            }).appendTo(this)
                                        }

                                    }).appendTo(this);

                                    //if (subAtter.IsQuantity) {

                                    //    $('<div/>', {
                                    //        'class': 'col-md-2',
                                    //        'html': function () {
                                    //            $('<input/>', {
                                    //                'type': 'text',
                                    //                'class': 'form-control input_quantity',
                                    //                'id': 'input_' + subAtter.SubAttributeId,
                                    //                'value': quantity
                                    //            }).appendTo(this)
                                    //            $('<span/>', {
                                    //                'html': subAtter.Unit
                                    //            }).appendTo(this);
                                    //        }

                                    //    }).appendTo(this);
                                    //}

                                }
                            }).appendTo($div);
                        }
                    }

                }

                $("#modal-add-edit-sub_attr_feedback").modal('show');
            });

            $(document).on('click', '.range_change', function () {
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

            $(document).on('click', '#btnsubmit', function () {
                var attrid = $("#attrid").val();
                var gameid = $("#gameid").val();
                var spanId = `span_${attrid}_${gameid}`;
                var totalFeedback = 0;
                var totalQuantityFeedback = 0;
                var totalSubAttr = $("#sub_attr_feedback_div .wrap_input").length;

                $("#sub_attr_feedback_div .wrap_input").each(function () {

                    var emoji = $(this).find('.range_change').val();
                    var quantity = $(this).find('.input_quantity') !== undefined ? $(this).find('.input_quantity').val() : '';
                    var subatterId = $(this).find('.subattribute').val();
                    var id = $(this).find('.feedbackatterId').val();
                    var Percentage = $(".span-range-value-percent").html().replace('%', '');;
                    totalFeedback += parseInt(emoji);
                    totalQuantityFeedback += quantity == undefined ? 0 : parseInt(quantity);
                    var feedbackDetail = GetFeedback(gameFeedbackDetailList, gameid, attrid, subatterId);
                    var feedbackDetail1 = feedbackDetail[0];
                    if (feedbackDetail != "") {

                        var startIndex = $.inArray(feedbackDetail1, gameFeedbackDetailList);
                        gameFeedbackDetailList.splice(startIndex, 1);
                        feedbackDetail1.Feedback = emoji;
                        feedbackDetail1.Quantity = quantity;
                        feedbackDetail1.Percentage = Percentage;

                        gameFeedbackDetailList.push(feedbackDetail1);
                    } else {

                        gameFeedbackDetailList.push({ Id: id, GameId: gameid, AttributeId: attrid, SubAttributeId: subatterId, Feedback: emoji, Quantity: quantity, Percentage: Percentage });
                    }
                });
                if (!isQuentityFeedback) {
                    if (totalFeedback > 0) {
                        var ff = Math.round(totalFeedback / totalSubAttr);
                        if (ff > 0 && ff <= 10) {
                            $("#" + spanId).html(`<img src="${Global.GetEmojiName(ff)}" class="imgemoji" />` + ' ' + $(".span-range-value-percent").html() );
                        } else {
                            $("#spanId").html('');
                        }
                    }
                } else if (totalQuantityFeedback > 0) {
                    var ff = Math.round(totalQuantityFeedback / totalSubAttr);
                    if (ff > 0) {
                        $("#" + spanId).html(`${ff}` + ' ' + ' ' + $(".span-range-value-percent").html());
                    } else {
                        $("#spanId").html('');
                    }
                }


                $("#modal-add-edit-sub_attr_feedback").modal('hide');
            });

            $("#btn-submit").on('click', function () {

                var $divd = $("#feedback_atter_data");
                $divd.empty();
                for (var i = 0; i < gameFeedbackDetailList.length; i++) {
                    var fdetail = gameFeedbackDetailList[i];
                    $('<input/>', {
                        'type': 'hidden',
                        'name': `GameFeedbackDetails[${i}].Id`,
                        'value': fdetail.Id
                    }).appendTo($divd);

                    $('<input/>', {
                        'type': 'hidden',
                        'name': `GameFeedbackDetails[${i}].GameId`,
                        'value': fdetail.GameId
                    }).appendTo($divd);

                    $('<input/>', {
                        'type': 'hidden',
                        'name': `GameFeedbackDetails[${i}].AttributeId`,
                        'value': fdetail.AttributeId
                    }).appendTo($divd)
                    $('<input/>', {
                        'type': 'hidden',
                        'name': `GameFeedbackDetails[${i}].SubAttributeId`,
                        'value': fdetail.SubAttributeId
                    }).appendTo($divd);

                    $('<input/>', {
                        'type': 'hidden',
                        'name': `GameFeedbackDetails[${i}].Feedback`,
                        'value': fdetail.Feedback
                    }).appendTo($divd);

                    $('<input/>', {
                        'type': 'hidden',
                        'name': `GameFeedbackDetails[${i}].Quantity`,
                        'value': fdetail.Quantity
                    }).appendTo($divd);

                    $('<input/>', {
                        'type': 'hidden',
                        'name': `GameFeedbackDetails[${i}].Percentage`,
                        'value': fdetail.Percentage
                    }).appendTo($divd);
                }
            });

            $('#brndraft').on('click', function () {

                $('#IsDraft').val('true');
                $("#btn-submit").click();
            });
        }

        function BindGameTable(allarr, extraSpace, style) {
            rowIndex++;
            let extraSpace1 = "";
            let style1 = ''
            var sb = "";
            var $tbl = $("#tbl_game tbody");
            var $div = $("#div_add_game");
            for (var i = 0; i < allarr.length; i++) {
                var row = allarr[i];


                $('<tr/>', {
                    'class': row.ParentId,
                    'style': style,
                    html: function () {
                        $('<td/>', {
                            'html': row.Id
                        }).appendTo(this);

                        $('<td/>', {
                            'html': extraSpace + ' ' + row.Name
                        }).appendTo(this);

                        for (var i = 0; i < attributeList.length; i++) {

                            var attr = attributeList[i];
                            var feedback = GetFeedbackByAttrId(gameFeedbackDetailList, row.Id, attr.AttributeId);
                            var subattr = GetSubAtterList(subAttributeList, attr.AttributeId);
                            let emojifeedback = 0;
                            let quantityfeedback = 0;
                            if (feedback != '' && feedback.length > 0) {
                                let totalFeedback = 0;
                                let totalQuantityFeedback = 0;
                                for (var t = 0; t < feedback.length; t++) {
                                    totalFeedback += parseInt(feedback[t].Feedback);
                                    totalQuantityFeedback += parseInt(feedback[t].Quantity);

                                }

                                if (totalFeedback > 0 && subattr != '' && subattr.length > 0) {
                                    emojifeedback = Math.round(totalFeedback / subattr.length);
                                }

                                if (totalQuantityFeedback > 0 && subattr != '' && subattr.length > 0) {
                                    quantityfeedback = Math.round(totalQuantityFeedback / subattr.length);
                                }
                            }

                            $('<td/>', {
                                'html': function () {
                                    $('<button/>', {
                                        'type': 'button',
                                        'class': 'btn btn-default attr_feedback',
                                        'data-attrid': attr.AttributeId,
                                        'data-attrname': attr.Name,
                                        'data-attrdesc': attr.Desciption,
                                        'data-gameid': row.Id,
                                        'data-gamename': row.Name,
                                        'html': 'Feedback'
                                    }).appendTo(this)

                                    $('<span/>', {
                                        'id': `span_${attr.AttributeId}_${row.Id}`,
                                        'style': 'margin-left: 5px',
                                        'html': (isQuentityFeedback ? `${quantityfeedback}` : (emojifeedback > 0 && emojifeedback <= 10 ? `<img src="${Global.GetEmojiName(emojifeedback)}" class="imgemoji" />` : ''))

                                    }).appendTo(this)

                                }
                            }).appendTo(this);


                        }

                    }

                }).appendTo($tbl);

                if (row.ChildGame.length > 0) {
                    extraSpace1 = extraSpace + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                }
                BindGameTable(row.ChildGame, extraSpace1, style1);

            }



        }



        function GetSubAtterList(subAtterArr, atterId) {
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

        function GetFeedback(feedbackArr, gameId, atterId, subatterId) {
            if (gameId && atterId && subatterId) {
                var result = $.grep(feedbackArr, function (e) { return e.GameId == gameId && e.AttributeId == atterId && e.SubAttributeId == subatterId; });
                if (result != null && result.length > 0)
                    return result;
                else
                    return '';
            } else {
                return '';
            }
        }

        function GetFeedbackByAttrId(feedbackArr, gameId, atterId) {
            if (gameId && atterId) {
                var result = $.grep(feedbackArr, function (e) { return e.GameId == gameId && e.AttributeId == atterId });
                if (result != null && result.length > 0)
                    return result;
                else
                    return '';
            } else {
                return '';
            }
        }

        function GetEmojiList(emojiArr, id) {
            if (id) {
                var result = $.grep(emojiArr, function (e) { return e.Id == id; });
                if (result != null && result.length > 0)
                    return result[0];
                else
                    return '';
            } else {
                return '';
            }
        }

        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };
            initilizeModel();
        }
    }

    $(function () {
        var self = new feedbackAddEdit();
        self.init();
    })
})(jQuery)