
(function ($) {
    function PollAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        var NotificationsList = function (data) {
            var self = this;
            self.id = ko.observable(0);
            self.typeId = ko.observable('');
            self.every = ko.observable('');
            self.unit = ko.observable('');
            self.NotificationsTypeList = ko.observableArray(NotificationsListArr);
            self.NotificationsUnitTypeList = ko.observableArray(NotificationsUnitListArr);
            if (typeof data !== 'undefined') {
                self.id(data.id);
                self.typeId(data.typeId);
                self.every(data.every);
                self.unit(data.unit);
            }
        }

        var questionTemplate = function (data) {
            var self = this;
            self.id = ko.observable('');
            self.name = ko.observable('');
            self.questionTypeId = ko.observable('');
            self.questionTypeList = ko.observableArray(questionTypeListArr);
            self.questionOption = ko.observableArray([]);
            self.optionType = ko.observable(1);
            self.PollTypeOnchange = function (obj, event) {
                
                var id = $(this)[0].questionTypeId();
                if (self.questionOption().length > 0) {
                    while (self.questionOption().length > 0) {
                        self.questionOption.remove(self.questionOption()[0]);
                    }
                }

                self.optionType(parseInt(id));                

                if (parseInt(id) == 1) {
                    self.questionOption.push(new optionTemplate({ id: '', name: 'Yes', filePath: '' },1));
                    self.questionOption.push(new optionTemplate({ id: '', name: 'No', filePath: '' },1));
                } else {
                    self.questionOption.push(new optionTemplate({ id: '', name: '', filePath: '' }, parseInt(id)));
                }
            };

            if (typeof data !== 'undefined') {
                self.name(data.name);
                self.questionTypeId(data.questionTypeId);
                self.id(data.id);
                self.optionType(data.questionTypeId);
                
                if (typeof data.questionOption != 'undefined' && data.questionOption.length > 0) {
                    $.each(data.questionOption, function (i, el) {
                        self.questionOption.push(new optionTemplate({ id: el.Id, name: el.Name, filePath: el.FilePath },data.questionTypeId));
                    });
                    
                }
            }

            self.removeOption = function (furniture) {
                self.questionOption.remove(furniture);               
            };

            self.addOption = function (questionTypeId) {                
                self.questionOption.push(new optionTemplate({ id: '', name: '', filePath: '' }, questionTypeId));                
            };
        }

        var optionTemplate = function (data,questionType) {
            var self = this;
            self.id = ko.observable('');
            self.name = ko.observable('');
            self.filePath = ko.observable('');
            self.fullpath = ko.observable('');
            self.optionType = ko.observable('');
            if (typeof data !== 'undefined') {
                self.id(data.id);
                self.name(data.name);
                self.filePath(data.filePath);
                self.optionType = ko.observable(questionType);
                self.fullpath('/DYF/' + companyId + '/poll/' + createdUserId + '/' + data.filePath);
            }
        }

        var SessionNotificationViewModel = function () {
            var self = this;
            self.notificationslistarr = ko.observableArray([]);
            self.questionlistarr = ko.observableArray([]);

            if (typeof sessionReminderArr !== 'undefined' && sessionReminderArr.length > 0) {

                $.each(sessionReminderArr, function (i, el) {
                    self.notificationslistarr.push(new NotificationsList({ id: el.Id, typeId: el.TypeId, every: el.Every, unit: el.Unit }));

                });

            } else {
                self.notificationslistarr.push(new NotificationsList({ id: "", typeId: "1", every: "4", unit: "2" }));
            }

            if (typeof pollQuestionArr !== 'undefined' && pollQuestionArr.length > 0) {

                $.each(pollQuestionArr, function (i, el) {
                    self.questionlistarr.push(new questionTemplate({ id: el.Id, name: el.Name, questionTypeId: el.QuestionTypeId, questionOption:el.PollQuestionOption }));
                });

            } else {
                self.questionlistarr.push(new questionTemplate({ id: '', name: '', questionTypeId: '', questionOption:[] }));
            }

            self.remove = function (data) {
                self.notificationslistarr.remove(data);
            };
            self.add = function () {
                self.notificationslistarr.push(new NotificationsList({ id: "", typeId: "1", every: "4", unit: "2" }));
            };

            self.removeQuestion = function (data) {
                self.questionlistarr.remove(data);
            };
            self.addQuestion = function () {
                self.questionlistarr.push(new questionTemplate({ id: '', name: '', questionTypeId: '', questionOption:[] }));
            };
        };

        function initilizeModel() {

            form = new Global.FormHelperWithFiles($("#frm-add-edit-poll").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

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

            $('#PollScheduler_StartDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var minDate = new Date(selected.date.valueOf());
                $('#PollScheduler_EndDate').datepicker('setStartDate', minDate);
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $('#PollScheduler_EndDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var maxDate = new Date(selected.date.valueOf());
                $('#PollScheduler_StartDate').datepicker('setEndDate', new Date(maxDate));
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });


            if ($('#PollScheduler_EndDate').val() != "") {
                var minDate = new Date(moment($("#PollScheduler_EndDate").val(), "DD/MM/YYYY"));
                $('#PollScheduler_StartDate').datepicker('setEndDate', minDate);
            }

            if ($('#PollScheduler_StartDate').val() != "") {
                var minDate = new Date(moment($("#PollScheduler_StartDate").val(), "DD/MM/YYYY"));
                $('#PollScheduler_EndDate').datepicker('setStartDate', minDate);
            }



            ko.applyBindings(new SessionNotificationViewModel());



            collapseRow();
            var navListItems = $('div.stepbox ul li a'),
                allWells = $('.tab-pane'),
                allNextBtn = $('.nextBtn');
            allBackBtn = $('.backBtn');

            navListItems.click(function (e) {
                e.preventDefault();
                var $target = $($(this).attr('href')),
                    $item = $(this);

                if (!$item.parent().hasClass('active')) {
                    allWells.removeClass('active');
                    $target.click();
                }


            });

            allNextBtn.click(function () {

                var curStep;
                $(".tab-content .tab-pane").each(function () {
                    if ($(this).hasClass('active')) {
                        curStep = $(this);
                    }
                });
                var curStepBtn = curStep.attr("id"),
                    nextStepWizard = $('div.stepbox ul li a[href="#' + curStepBtn + '"]').parent().next().children("a");
                nextStepWizard.trigger('click');

            });

            allBackBtn.click(function () {
                var curStep;
                $(".tab-content .tab-pane").each(function () {
                    if ($(this).hasClass('active')) {
                        curStep = $(this);
                    }
                });
                var curStepBtn = curStep.attr("id"),
                    nextStepWizard = $('div.stepbox ul li a[href="#' + curStepBtn + '"]').parent().prev().children("a");
                nextStepWizard.removeAttr('disabled').trigger('click');
            });



            $('.datepicker').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
            $('.datepicker').datepicker({
                autoclose: true,
                format: "dd/mm/yyyy"
            });



            // $('.timepicker').inputmask();
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



            $('input[type=radio][name="PollScheduler.Type"]').change(function () {
                if (this.value == '2') {
                    $(".lookschedulerrecurring").show();
                }
                else {
                    $(".lookschedulerrecurring").hide();
                }
            });

            $('#PollScheduler_Frequency').on('change', function () {

                $('.Daily').hide();
                $('.Weekly').hide();
                $('.Monthly').hide();
                if (parseInt($(this).val()) == 1) {
                    $('.Daily').show();
                } else if (parseInt($(this).val()) == 2) {
                    $('.Weekly').show();

                } else if (parseInt($(this).val()) == 3) {
                    $('.Monthly').show();

                }
            })

            $("#PollScheduler_DailyFrequency").on('change', function () {
                if (parseInt($(this).val()) == 1) {

                    $("#div_OccursEveryTimeUnit").hide();
                } else {

                    $("#div_OccursEveryTimeUnit").show();
                }
            })

            $("#PollScheduler_MonthlyOccurrence").on('change', function () {
                $("#div_ExactDateOfMonth").hide();
                $("#div_ExactWeekdayOfMonth").hide();
                $("#div_ExactWeekdayOfMonthEvery").hide();

                if (parseInt($(this).val()) == 1) {
                    $("#div_ExactDateOfMonth").show();
                } else if (parseInt($(this).val()) == 2) {
                    $("#div_ExactWeekdayOfMonth").show();
                    $("#div_ExactWeekdayOfMonthEvery").show();
                }
            });


            Global.ModelHelper($("#modal-action-delete-poll"), function () {
                form = new Global.FormHelper($("#modal-action-delete-poll").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-poll'
                }, function (response) {
                    window.location.href = response.redirectUrl
                }, null);

            }, null);


            $(document).on('click', '.choice_remove', function () {
                var chckId = $(this).data('id');
                $("#" + chckId).val(false);
                $("#" + chckId).prop('checked', false);

                var $divId = $("#div_add_mainpower tbody");
                $divId.empty();
                rowIndex = 0;
                $('.added:checked').each(function () {
                    CheckUnChekChild($(this));
                    rowIndex++;
                });
            });

            $(document).on('change', '.uploadfile', function () {
                var chckId = $(this).attr('id') + '_show';
                Global.ValidateImage(this, $(this).attr('id'), chckId);

            });

            //Global.DataTable('#uploadfile');

            $('.added1').off('change').change(function () {
                var $divId = $("#div_add_mainpower tbody");
                $divId.empty();
                rowIndex = 0;
                $('.added1:checked').each(function () {
                    CheckUnChekChild_entity($(this));
                    rowIndex++;
                });
            });


        }

        function collapseRow() {

            $('.added').off('change').change(function () {
                CheckUnChekChild($(this));

            });

            $('a[data-toggle="collapse"]').off('click').on('click', function () {
                var objectID = '.' + $(this).attr('id');
                if ($(objectID).hasClass('in')) {
                    $(objectID).removeClass('in');
                    $(this).html('<span class="fa fa-plus" ></span>');
                    $(objectID).hide();
                }
                else {
                    $(objectID).addClass('in');
                    $(this).html('<span class="fa fa-minus" ></span>');
                    $(objectID).show();
                }
            });
        }


        function CheckUnChekChild($data) {

            if ($data.is(":checked")) {
                var $divId = $("#" + $data.data('divid'));
                $('<label/>', {
                    'id': 'span_' + $data.data('type') + '_' + $data.data('id'),
                    'html': function () {
                        $('<label/>', {
                            html: $data.data('name')
                        }).appendTo(this);
                        $('<a/>', {
                            'href': 'javascript:void(0);',
                            'html': '<i class="fa fa-close"></i>',
                            'data-txt': $data.data('name'),
                            'data-id': $data.data('type') + '_' + $data.data('id'),
                            'class': 'choice_remove'

                        }).appendTo(this)
                    }
                }).appendTo($divId);
            } else {
                $("#span_" + $data.data('type') + '_' + $data.data('id')).remove();
            }
            var id = $data.attr('id');
            $("." + id).val($data.is(":checked"));
            $("." + id).prop('checked', $data.is(":checked"));
            $data.val($data.is(":checked"));

            $("." + id).each(function () {
                CheckUnChekChild($(this));
            });
        }

        function CheckUnChekChild_entity($data) {
            if ($data.is(":checked")) {
                var $divId = $("#" + $data.data('divid') + ' tbody');
                $('<tr/>', {
                    'id': 'tr_' + $data.data('type') + '_' + $data.data('id'),
                    'html': function () {
                        $('<td/>', {
                            html: $data.data('typename')
                        }).appendTo(this);
                        $('<td/>', {
                            html: $data.data('name')
                        }).appendTo(this);

                        $('<td/>', {
                            html: function () {

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'PollParticipants[' + rowIndex + '].typeid',
                                    'value': $data.data('typeid')
                                }).appendTo(this)

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'PollParticipants[' + rowIndex + '].userId',
                                    'value': $data.data('id')
                                }).appendTo(this)

                                $('<a/>', {
                                    'href': 'javascript:void(0);',
                                    'html': '<i class="fa fa-close"></i>',
                                    'data-txt': $data.data('name'),
                                    'data-id': $data.data('type') + '_' + $data.data('id'),
                                    'class': 'choice_remove'

                                }).appendTo(this)
                            }
                        }).appendTo(this);


                    }
                }).appendTo($divId);
            } else {
                $("#span_" + $data.data('type') + '_' + $data.data('id')).remove();
            }
            var id = $data.attr('id');
            $("." + id).val($data.is(":checked"));
            $("." + id).prop('checked', $data.is(":checked"));
            $data.val($data.is(":checked"));


        }

        function GetGameValue(groupArr, gameId) {
            if (gameId) {
                var result = $.grep(groupArr, function (e) { return e.GameId == gameId; });
                if (result != null && result.length > 0)
                    return result[0];
                else
                    return '';
            } else {
                return '';
            }
        }

        function GetTeamValue(groupArr, teamId) {
            if (teamId) {
                var result = $.grep(groupArr, function (e) { return e.TeamId == teamId; });
                if (result != null && result.length > 0)
                    return result[0];
                else
                    return '';
            } else {
                return '';
            }
        }

        function GetUserValue(groupArr, userId) {
            if (userId) {
                var result = $.grep(groupArr, function (e) { return e.UserId == userId; });
                if (result != null && result.length > 0)
                    return result[0];
                else
                    return '';
            } else {
                return '';
            }
        }



        function collapseRow() {

            $('.added').off('change').change(function () {
                CheckUnChekChild($(this));

            });

            $('a[data-toggle="collapse"]').off('click').on('click', function () {
                var objectID = '.' + $(this).attr('id');
                if ($(objectID).hasClass('in')) {
                    $(objectID).removeClass('in');
                    $(this).html('<span class="fa fa-plus" ></span>');
                    $(objectID).hide();
                }
                else {
                    $(objectID).addClass('in');
                    $(this).html('<span class="fa fa-minus" ></span>');
                    $(objectID).show();
                }
            });
        }


        function CheckUnChekChild($data) {

            if ($data.is(":checked")) {
                var $divId = $("#" + $data.data('divid'));
                $('<label/>', {
                    'id': 'span_' + $data.data('type') + '_' + $data.data('id'),
                    'html': function () {
                        $('<label/>', {
                            html: $data.data('name')
                        }).appendTo(this);
                        $('<a/>', {
                            'href': 'javascript:void(0);',
                            'html': '<i class="fa fa-close"></i>',
                            'data-txt': $data.data('name'),
                            'data-id': $data.data('type') + '_' + $data.data('id'),
                            'class': 'choice_remove'

                        }).appendTo(this)
                    }
                }).appendTo($divId);
            } else {
                $("#span_" + $data.data('type') + '_' + $data.data('id')).remove();
            }
            var id = $data.attr('id');
            $("." + id).val($data.is(":checked"));
            $("." + id).prop('checked', $data.is(":checked"));
            $data.val($data.is(":checked"));

            $("." + id).each(function () {
                CheckUnChekChild($(this));
            });
        }

        function DatatableBind() {

            //   Global.DataTableWithOutPage('#tbl_session');

            // Global.DataTableWithOutPage('#tbl_user');
        }

        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };
            DatatableBind();
            initilizeModel();
        }
    }

    $(function () {
        var self = new PollAddEdit();
        self.init();
    })
})(jQuery)