(function ($) {
    function PreSesionAddEdit() {
        var $this = this, form;

        var participant = function (data) {
            var self = this;
            self.id = ko.observable(0);
            self.userId = ko.observable('');
            self.user = ko.observable('');
            self.typeId = ko.observable('');
            self.type = ko.observable('');
            self.participantTyprId = ko.observable('');
            self.participantType = ko.observable('');
            self.status = ko.observable(false);
            self.statusStr = ko.observable('');
            self.remarks = ko.observable('');
            if (typeof data !== 'undefined') {
                self.id(data.id);
                self.user(data.user);
                self.userId(data.userId);
                self.typeId(data.typeId);
                self.type(data.type);
                self.participantTyprId(data.participantTyprId);
                self.participantType(data.participantType);
                self.status(data.status);
                self.statusStr(data.statusStr);
                self.remarks(data.remarks);
            }
        }

        var agenda = function (data) {
            var self = this;
            self.id = ko.observable(0);
            self.gameId = ko.observable('');
            self.typeId = ko.observable('');
            self.subGameId = ko.observable(false);
            self.status = ko.observable('');
            self.name = ko.observable('');
            self.description = ko.observable('');
            self.accountableId = ko.observable('');
            self.dependancyId = ko.observable('');
            self.priority = ko.observable('');
            self.addedOn = ko.observable('');
            self.startDate = ko.observable('');
            self.deadlineDate = ko.observable('');
            self.feedbackType = ko.observable('');
            self.actualStartDate = ko.observable('');
            self.actualEndDate = ko.observable('');
            self.user = ko.observable('');
            self.user = ko.observable('');
            if (typeof data !== 'undefined') {
                self.id(data.id);
                self.gameId(data.gameId);
                self.typeId(data.typeId);

                self.subGameId(data.subGameId);
                self.status(data.status);
                self.description(data.description);
                self.accountableId(data.accountableId);
                self.dependancyId(data.dependancyId);
                self.priority(data.priority);
                self.addedOn(data.addedOn);
                self.startDate(data.startDate);
                self.deadlineDate(data.deadlineDate);
                self.feedbackType(data.feedbackType);
                self.actualStartDate(data.actualStartDate);
                self.actualEndDate(data.actualEndDate);
            }
        }

        var PreSessionViewModel = function () {
            var self = this;
            self.participantlist = ko.observableArray([]);

            self.agendalist = ko.observableArray([]);

            if (typeof participantListarr !== 'undefined' && participantListarr.length > 0) {

                $.each(participantListarr, function (i, el) {
                    self.participantlist.push(new participant({
                        id: el.Id, userId: el.UserId, typeId: el.TypeId, type: el.Type, participantTyprId: el.ParticipantTyprId, participantType: el.ParticipantType,
                        status: el.Status, statusStr: el.StatusStr, remarks: el.Remarks, user: el.User
                    }));
                });
            }

            if (typeof agendaListarr !== 'undefined' && agendaListarr.length > 0) {

                $.each(agendaListarr, function (i, el) {
                    self.agendalist.push(new agenda({
                        id: el.Id, gameId: el.GameId, subGameId: el.SubGameId, status: el.Status, description: el.Description, accountableId: el.AccountableId,
                        dependancyId: el.DependancyId, priority: el.Priority, addedOn: el.AddedOn, startDate: el.StartDate,
                        deadlineDate: el.DeadlineDate, feedbackType: el.FeedbackType, actualStartDate: el.ActualStartDate, actualEndDate: el.ActualEndDate
                    }));
                });

            }

            self.remove = function (data) {
                self.participantlist.remove(data);
            };
        };


        function initilizeModel() {

            Global.ModelHelper($("#modal-add-edit-playfeebback"), function () {
                var form1 = new Global.FormHelper($("#modal-add-edit-playfeebback").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-add-edit-playfeebback'
                }, function (reponse) {
                        var $tbl = $("#agenda_feedback_play_grid tbody");
                        var preSesssion = reponse.data;

                        pindex = $("#agenda_feedback_play_grid tbody tr").length;
                        if (preSesssion != '') {

                            $('<tr/>', {
                                html: function () {

                                    $('<td/>', {
                                        html: function () {

                                            $('<span/>', {
                                                html: preSesssion.id
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].Id`,
                                                'value': ''
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].PlayId`,
                                                'value': preSesssion.id
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].GameId`,
                                                'value': preSesssion.gameId
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].AccountableId`,
                                                'value': preSesssion.accountableId
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].ActualEndDate`,
                                                'value': preSesssion.actualEndDate
                                            }).appendTo(this);


                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].ActualStartDate`,
                                                'value': preSesssion.actualStartDate
                                            }).appendTo(this);


                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].AddedOn`,
                                                'value': preSesssion.addedOn
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].DeadlineDate`,
                                                'value': preSesssion.deadlineDate
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].DependancyId`,
                                                'value': preSesssion.dependancyId
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].Description`,
                                                'value': preSesssion.description
                                            }).appendTo(this);


                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].FeedbackType`,
                                                'value': preSesssion.feedbackType
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].Name`,
                                                'value': preSesssion.name
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].Priority`,
                                                'value': preSesssion.priority
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].StartDate`,
                                                'value': preSesssion.startDate
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].Status`,
                                                'value': preSesssion.status
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].SubGameId`,
                                                'value': preSesssion.subGameId
                                            }).appendTo(this);

                                            $('<input/>', {
                                                'type': 'hidden',
                                                'name': `PreSessionAgenda[${pindex}].Type`,
                                                'value': preSesssion.type
                                            }).appendTo(this);
                                        }
                                    }).appendTo(this);


                                    $('<td/>', {
                                        html: preSesssion.game
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.name
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.description
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.prioritystr
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.dependancy
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.statusStr
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.typeStr
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: moment(preSesssion.deadlineDate, "DD/MM/YYYY")
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: function () {
                                            $('<input/>', {
                                                'type': 'file',
                                                'multiple': 'multiple',
                                                'id': `PreSessionAgenda_${pindex}_files`,
                                                'name': `PreSessionAgenda[${pindex}].files`
                                            }).appendTo(this);
                                        }
                                    }).appendTo(this);
                                }
                            }).appendTo($tbl);
                            pindex++;
                        }
                       
                        $("#modal-add-edit-playfeebback").modal('hide');
                }, null);

                form1.find('.datepicker').datepicker({
                    keyboardNavigation: false,
                    forceParse: false,
                    toggleActive: false,
                    autoclose: true,
                    format: 'dd/mm/yyyy'
                }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });


                form1.find('#StartDate').datepicker({
                    keyboardNavigation: false,
                    forceParse: false,
                    toggleActive: false,
                    autoclose: true,
                    format: 'dd/mm/yyyy'
                }).on('changeDate', function (selected) {
                    var minDate = new Date(selected.date.valueOf());
                    form1.find('#DeadlineDate').datepicker('setStartDate', minDate);
                }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

                form1.find('#DeadlineDate').datepicker({
                    keyboardNavigation: false,
                    forceParse: false,
                    toggleActive: false,
                    autoclose: true,
                    format: 'dd/mm/yyyy'
                }).on('changeDate', function (selected) {
                    var maxDate = new Date(selected.date.valueOf());
                    $('#StartDate').datepicker('setEndDate', new Date(maxDate));
                }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

               

                let emojifeedback = Emotion;
                var newValue = Number((emojifeedback - 1) * 100 / (10 - 1))
                var newPosition = 10 - (emojifeedback * 5.5);

                form1.find("#view_emotion").attr('style', `left:calc(${newValue}% + (${newPosition}px))`);



                form1.find("#PersonInvolved").select2();

                form1.find("#GameId").on('change', function () {
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

            }, null);

            form = new Global.FormHelperWithFiles($("#modal-add-edit-agenda").find("form"), {
                updateTargetId: "validation-summary",
                refreshGrid: false,
                modelId: 'modal-add-edit-agenda'
            }, null, null);

            ko.applyBindings(new PreSessionViewModel(), $('#sub_tbl')[0]);

            $('.datepicker').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $(".timepicker").inputmask("99:99", { clearIncomplete: false });
            $(".timepicker").blur(function () {
                var currentMask = '';
                var arr = $(this).val().split('');
                if (arr[1] == '_' && arr[0] != '_') {
                    arr[1] = arr[0];
                    arr[0] = '0';
                }

                if (arr[4] == '_' && arr[3] != '_') {
                    arr[4] = arr[3];
                    arr[3] = '0';
                }

                $(arr).each(function (index, value) {
                    if (value == '_')
                        arr[index] = '0';
                    currentMask += arr[index];
                });
                var time = currentMask.split(':');
                if (time[0] == "" || time[0] == 'undefined' || time[0] == '__' || parseInt(time[0]) > 23)
                    time[0] = '';
                if (time[1] == "" || time[1] == 'undefined' || time[1] == '__' || parseInt(time[1]) > 59)
                    time[1] = '';
                var newVal = time[0] + ":" + time[1];
                if (newVal.indexOf("undefined") != -1) {
                    newVal = "";
                }
                $(this).val(newVal);
            });

            $("#add_agenda_feedback").on('click', function () {
                var url = $(this).data('url');
                var $tbl = $("#agenda_feedback_play tbody");
                $tbl.empty();

                $.get(url, function (response) {
                    PreSessionAgendaListarr = response;
                    if (response != undefined && response.length > 0) {
                        for (var i = 0; i < response.length; i++) {

                            var preSesssion = response[i];
                            $('<tr/>', {
                                html: function () {

                                    $('<td/>', {
                                        html: function () {
                                            $('<input/>', {
                                                'type': 'checkbox',
                                                'class': 'feedback_check',
                                                'value': preSesssion.id
                                            }).appendTo(this);
                                        }
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.game
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.name
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.description
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.prioritystr
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.dependancy
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.statusStr
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: preSesssion.typeStr
                                    }).appendTo(this);

                                    $('<td/>', {
                                        html: moment(preSesssion.deadlineDate, "DD/MM/YYYY")
                                    }).appendTo(this);
                                }
                            }).appendTo($tbl);
                        }
                    }

                });



                $("#modal-agenda_feedback_play").modal('show');

            });

            $("#btnsubmit").on('click', function () {

                var $tbl = $("#agenda_feedback_play_grid tbody");
                $tbl.empty();
                let pindex = 0;
                $(".feedback_check:checked").each(function () {
                    var id = $(this).val();

                    var preSesssion = GetPresessionValue(PreSessionAgendaListarr, id);
                    if (preSesssion != '') {

                        $('<tr/>', {
                            html: function () {

                                $('<td/>', {
                                    html: function () {

                                        $('<span/>', {
                                            html: id
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].Id`,
                                            'value': ''
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].PlayId`,
                                            'value': id
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].GameId`,
                                            'value': preSesssion.gameId
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].AccountableId`,
                                            'value': preSesssion.accountableId
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].ActualEndDate`,
                                            'value': preSesssion.actualEndDate
                                        }).appendTo(this);


                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].ActualStartDate`,
                                            'value': preSesssion.actualStartDate
                                        }).appendTo(this);


                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].AddedOn`,
                                            'value': preSesssion.addedOn
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].DeadlineDate`,
                                            'value': preSesssion.deadlineDate
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].DependancyId`,
                                            'value': preSesssion.dependancyId
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].Description`,
                                            'value': preSesssion.description
                                        }).appendTo(this);


                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].FeedbackType`,
                                            'value': preSesssion.feedbackType
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].Name`,
                                            'value': preSesssion.name
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].Priority`,
                                            'value': preSesssion.priority
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].StartDate`,
                                            'value': preSesssion.startDate
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].Status`,
                                            'value': preSesssion.status
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].SubGameId`,
                                            'value': preSesssion.subGameId
                                        }).appendTo(this);

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': `PreSessionAgenda[${pindex}].Type`,
                                            'value': preSesssion.type
                                        }).appendTo(this);
                                    }
                                }).appendTo(this);


                                $('<td/>', {
                                    html: preSesssion.game
                                }).appendTo(this);

                                $('<td/>', {
                                    html: preSesssion.name
                                }).appendTo(this);

                                $('<td/>', {
                                    html: preSesssion.description
                                }).appendTo(this);

                                $('<td/>', {
                                    html: preSesssion.prioritystr
                                }).appendTo(this);

                                $('<td/>', {
                                    html: preSesssion.dependancy
                                }).appendTo(this);

                                $('<td/>', {
                                    html: preSesssion.statusStr
                                }).appendTo(this);

                                $('<td/>', {
                                    html: preSesssion.typeStr
                                }).appendTo(this);

                                $('<td/>', {
                                    html: moment(preSesssion.DeadlineDate, "DD/MM/YYYY")
                                }).appendTo(this);

                                $('<td/>', {
                                    html: function () {
                                        $('<input/>', {
                                            'type': 'file',
                                            'multiple': 'multiple',
                                            'id': `PreSessionAgenda_${pindex}_files`,
                                            'name': `PreSessionAgenda[${pindex}].files`
                                        }).appendTo(this);
                                    }
                                }).appendTo(this);
                            }
                        }).appendTo($tbl);
                        pindex++;
                    }
                });

                $("#modal-agenda_feedback_play").modal('hide');
            });

            $('.all_check').on('change', function () {
                $(".feedback_check").prop('checked', $(this).is(":checked"));
            });

            $(".agenda-approve").on('click', function () {
                var url = $(this).data('url');
                var $that = $(this);
                $.get(url, function (response) {
                    if (response.isSucess) {
                        Global.ToastrSuccess("Approved");
                        $that.hide();
                        $that.parent().find(".agenda-disapprove").show();
                    } else {
                        Global.ToastrError("Already Approved");
                    }

                });
            });

            $(".agenda-delete").on('click', function () {
                var url = $(this).data('url');
                var classId = $(this).data('class');
                $.get(url, function (response) {
                    if (response.isSucess) {
                        Global.ToastrSuccess("Deleted successfully !");
                        
                        $('.' + classId).remove();
                    }

                });
            });

            

            $(".agenda-disapprove").on('click', function () {
                var url = $(this).data('url');
                var $that = $(this);
                $.get(url, function (response) {
                    if (response.isSucess) {
                        Global.ToastrSuccess("Dis approved");
                        $that.hide();
                        $that.parent().find(".agenda-approve").show();
                    }
                });
            });

            $(".agenda-send-noti").on('click', function () {
                var url = $(this).data('url');
                var $that = $(this).parent();
                $.get(url, function (response) {
                    Global.ToastrSuccess(response.message);
                });
            });

            $(".aganda-change").on('change', function () {
                var v = parseInt($(this).val());
                if (v == 1) {
                    $(".add-agenda").show();
                    $(".list-agenda").hide();
                } else {
                    $(".add-agenda").hide();
                    $(".list-agenda").show();
                }
            });

            $(".btnAccept").on('click', function () {
                var url = $(this).data('url');
                $.get(url, function (response) {
                    Global.ToastrSuccess("Accept successfully");
                    window.location.reload();
                });
            });

            $(".btnReject").on('click', function () {
                var url = $(this).data('url');
                $.get(url, function (response) {
                    Global.ToastrSuccess("Reject successfully");
                    window.location.reload();
                });
            });

            $(".reschedule").on('click', function () {
                var url = $(this).data('url');
                $("#urlHide").val(url);
                $("#modal-update-reschedule").modal('show');
            });



            $("#btnsubmit_reschedule").on('click', function () {
                var date = $("#Rescheduledate").val();
                var time = $("#RescheduledateTime").val();
                var hideUrl = $("#urlHide").val();
                if (date != "" && time != "") {
                    var dateArr = date.split('/');
                    var rescheDate = dateArr[1] + '/' + dateArr[0] + '/' + dateArr[2] + ' ' + time;
                    hideUrl = hideUrl + '&reSchedule=' + rescheDate;

                    $.get(hideUrl, function (response) {
                        Global.ToastrSuccess("Re schedule successfully");
                        window.location.reload();
                    });
                }
            });

            $(".delegate").on('click', function () {
                var url = $(this).data('url');
                $("#urlHidedelegate").val(url);
                $("#modal-update-deleget").modal('show');
            });

            $("#btnsubmit_deleget").on('click', function () {
                var userId = $("#delegetid_user").val();
                var hideUrl = $("#urlHidedelegate").val();
                if (userId != "") {
                    hideUrl = hideUrl + '&delegateId=' + userId;
                    $.get(hideUrl, function (response) {
                        Global.ToastrSuccess("Delegate assign successfully");
                        window.location.reload();
                    });
                }
            });
        }

        function GetPresessionValue(groupArr, id) {
            if (id) {
                var result = $.grep(groupArr, function (e) { return e.id == id; });
                if (result != null && result.length > 0)
                    return result[0];
                else
                    return '';
            } else {
                return '';
            }
        }

        $this.init = function () {

            initilizeModel();
        }
    }

    $(function () {
        var self = new PreSesionAddEdit();
        self.init();
    })
})(jQuery)