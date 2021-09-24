(function ($) {
    function SessionAddEdit() {
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

      

        var SessionNotificationViewModel = function () {
            var self = this;
            self.notificationslistarr = ko.observableArray([]);

            if (typeof sessionReminderArr !== 'undefined' && sessionReminderArr.length > 0) {

                $.each(sessionReminderArr, function (i, el) {
                    self.notificationslistarr.push(new NotificationsList({ id: el.Id, typeId: el.TypeId, every: el.Every, unit: el.Unit }));

                });

            } else {
                self.notificationslistarr.push(new NotificationsList({ id: "", typeId: "1", every: "4", unit: "2" }));
            }


            self.remove = function (data) {
                self.notificationslistarr.remove(data);
            };
            self.add = function () {
                self.notificationslistarr.push(new NotificationsList({ id: "", typeId: "1", every: "4", unit: "2" }));
            };
        };



        function initilizeModel() {

            form = new Global.FormHelperWithFiles($("#frm-add-edit-report").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            $('.select2').select2();

            $('#Scheduler_StartDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,                
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var minDate = new Date(selected.date.valueOf());
                $('#Scheduler_EndDate').datepicker('setStartDate', minDate);
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $('#Scheduler_EndDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,                
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var maxDate = new Date(selected.date.valueOf());
                $('#Scheduler_StartDate').datepicker('setEndDate', new Date(maxDate));
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

           

            if ($('#Scheduler_EndDate').val() != "") {                
                var minDate = new Date(moment($("#Scheduler_EndDate").val(), "DD/MM/YYYY"));
                $('#Scheduler_StartDate').datepicker('setEndDate', minDate);
            }

            if ($('#Scheduler_StartDate').val() != "") {
                var minDate = new Date(moment($("#Scheduler_StartDate").val(), "DD/MM/YYYY"));
                $('#Scheduler_EndDate').datepicker('setStartDate', minDate);
            }



            ko.applyBindings(new SessionNotificationViewModel());

            BindGameTable(allGame, '', '');
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

            $("#EntityType").on('change', function () {
                var forId = $("#EntityIds");
                forId.empty();
                var url = '';
                if (parseInt($(this).val()) == 1) {
                    url = '/game/GetGameist'
                } else if (parseInt($(this).val()) == 2) {
                    url = '/team/GetTeamList'
                } else {
                    url = '/user/GetUserList'
                }
                $.get(url, function (response) {

                    for (var i = 0; i < response.length; i++) {
                        var item = response[i];
                        $('<option/>', {
                            'value': item.id,
                            'html': item.name
                        }).appendTo(forId)
                    }
                })

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



            $('input[type=radio][name="Scheduler.Type"]').change(function () {
                if (this.value == '2') {
                    $(".lookschedulerrecurring").show();
                }
                else {
                    $(".lookschedulerrecurring").hide();
                }
            });

            $('#Scheduler_Frequency').on('change', function () {

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

            $("#Scheduler_DailyFrequency").on('change', function () {
                if (parseInt($(this).val()) == 1) {

                    $("#div_OccursEveryTimeUnit").hide();
                } else {

                    $("#div_OccursEveryTimeUnit").show();
                }
            })

            $("#Scheduler_MonthlyOccurrence").on('change', function () {
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

          
            Global.ModelHelper($("#modal-action-delete-report"), function () {
                form = new Global.FormHelper($("#modal-action-delete-report").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-report'
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

            //Global.DataTable('#tbl_game');

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
                                $('<select/>', {
                                    'class': 'form-control',
                                    'name': 'ReportJourney[' + rowIndex +'].AccountAbilityId',
                                    html: function () {
                                        for (var i = 0; i < ParticipantTypeListArr.length; i++) {
                                            var item = ParticipantTypeListArr[i];
                                            $('<option/>', {
                                                html: item.Name,
                                                'value': item.Id
                                            }).appendTo(this);
                                        }
                                    }
                                }).appendTo(this);
                            }
                        }).appendTo(this);

                        $('<td/>', {
                            html: function () {

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'ReportJourney[' + rowIndex+'].typeid',
                                    'value': $data.data('typeid')
                                }).appendTo(this)

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'ReportJourney[' + rowIndex +'].userId',
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

        function BindGameTable(allarr, extraSpace, style) {
            rowIndex++;
            let extraSpace1 = "";
            let style1 = 'display: none;'
            var sb = "";
            var $tbl = $("#tbl_game tbody");
            var $div = $("#div_add_game");
            for (var i = 0; i < allarr.length; i++) {
                var row = allarr[i];
                var isAdded = false;
                var gameRole = GetGameValue(lookGameList, row.Id);
                $('<tr/>', {
                    'class': row.ParentId,
                    'style': style,
                    html: function () {
                        $('<td/>', {
                            'html': function () {


                            }
                        }).appendTo(this);

                        $('<td/>', {
                            'html': extraSpace + ' ' + row.Name + (row.ChildGame.length > 0 ? "<a data-toggle='collapse' data-parent='accordion'  href='javascript:void(0)' id='" + row.Id + "'><span class='fa fa-plus'></span> </a>" : "")
                        }).appendTo(this);


                        $('<td/>', {
                            'html': function () {

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'Entity[' + (rowIndex - 1) + '].GameId',
                                    'value': row.Id
                                }).appendTo(this)

                                isAdded = gameRole != '' && gameRole.Id > 0;

                                if (isAdded) {
                                    $('<input/>', {
                                        'type': 'hidden',
                                        'name': 'Entity[' + (rowIndex - 1) + '].Id',
                                        'value': gameRole.Id
                                    }).appendTo(this)

                                    $('<input/>', {
                                        'type': "checkbox",
                                        'class': 'added ' + ' game_' + row.ParentId,
                                        'data-type': 'game',
                                        'id': 'game_' + row.Id,
                                        'data-id': row.Id,
                                        'data-name': row.Name,
                                        'value': isAdded,
                                        'name': 'Entity[' + (rowIndex - 1) + '].IsAdded',
                                        'data-divid': 'div_add_game',
                                        'checked': "checked"
                                    }).appendTo(this);
                                } else {

                                    $('<input/>', {
                                        'type': "checkbox",
                                        'class': 'added ' + ' game_' + row.ParentId,
                                        'data-type': 'game',
                                        'id': 'game_' + row.Id,
                                        'data-id': row.Id,
                                        'data-name': row.Name,
                                        'value': isAdded,
                                        'data-divid': 'div_add_game',
                                        'name': 'Entity[' + (rowIndex - 1) + '].IsAdded'
                                    }).appendTo(this);
                                }

                            }
                        }).appendTo(this);
                    }

                }).appendTo($tbl);

                isAdded = gameRole != '' && gameRole.Id > 0;
                if (isAdded) {
                    $('<label/>', {
                        'id': 'span_game_' + row.Id,
                        'html': function () {
                            $('<label/>', {
                                html: row.Name
                            }).appendTo(this);
                            $('<a/>', {
                                'href': 'javascript:void(0);',
                                'data-txt': row.Name,
                                'data-id': 'game_' + row.Id,
                                'class': 'choice_remove',
                                'html': '<i class="fa fa-close"></i>'
                            }).appendTo(this)
                        }
                    }).appendTo($div);
                }

                if (row.ChildGame.length > 0) {
                    extraSpace1 = extraSpace + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                }
                BindGameTable(row.ChildGame, extraSpace1, style1);

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
        var self = new SessionAddEdit();
        self.init();
    })
})(jQuery)