(function ($) {
    function LookAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        var GroupPlayerList = function (data, isAdded) {
            var self = this;
            self.id = ko.observable(0);
            self.name = ko.observable('');
            self.userId = ko.observable('');
            self.isChecked = ko.observable(isAdded);
            if (typeof data !== 'undefined') {
                self.name(data.name);
                self.userId(data.id);
            }
        }

        var Group = function (data) {
            var self = this;
            self.id = ko.observable('');
            self.name = ko.observable('surendra');
            self.players = ko.observableArray([]);

            if (typeof data !== 'undefined') {
                self.name(data.name);
                self.id(data.id);
                if (typeof allUser !== 'undefined') {
                    $.each(allUser, function (i, el) {
                        var player = GetUserValue(data.groupPlayer, el.Id)
                        var result = player != '' && player.UserId > 0;
                        self.players.push(new GroupPlayerList({ id: el.Id, name: el.Name }, result));

                    });
                }
            } else {
                if (typeof allUser !== 'undefined') {
                    $.each(allUser, function (i, el) {

                        self.players.push(new GroupPlayerList({ id: el.Id, name: el.Name }, false));
                    });
                }
            }

        };

        var lookGroupViewModel = function (group) {
            var self = this;
            self.groups = ko.observableArray([]);


            if (typeof group !== 'undefined' && group !== null) {
                if (group.length == 0) {
                    self.groups.push(new Group({ id: '', name: '', groupPlayer: [] }));
                }

                $.each(group, function (i, el) {
                    self.groups.push(new Group({ id: el.Id, name: el.Name, groupPlayer: el.LookGroupPlayer }));

                });
            } else {
                self.groups.push(new Group({ id: '', name: '', groupPlayer: [] }));
            }

            self.remove = function (data) {
                self.groups.remove(data);
            };
            self.add = function () {
                self.groups.push(new Group({ id: '', name: '', groupPlayer: [] }));
                collapseRow();
            };
        };



        function initilizeModel() {

            form = new Global.FormHelperWithFiles($("#frm-add-edit-look").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            ko.applyBindings(new lookGroupViewModel(allGroup));

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


            $('#FromDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var minDate = new Date(selected.date.valueOf());
                $('#ToDate').datepicker('setStartDate', minDate);
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $('#ToDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var maxDate = new Date(selected.date.valueOf());
                $('#FromDate').datepicker('setEndDate', new Date(maxDate));
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });



            if ($('#ToDate').val() != "") {
                var minDate = new Date(moment($("#ToDate").val(), "DD/MM/YYYY"));
                $('#FromDate').datepicker('setEndDate', minDate);
            }

            if ($('#FromDate').val() != "") {
                var minDate = new Date(moment($("#FromDate").val(), "DD/MM/YYYY"));
                $('#ToDate').datepicker('setStartDate', minDate);
            }


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

            $('.IsSchedule').change(function () {
                if ($(this).is(":checked")) {
                    $(".lookschednotenable").show();
                    if ($('input[type=radio][name="LookScheduler.Type"]:checked').val() == "2") {
                        $(".lookschedulerrecurring").show();
                    }
                    else {
                        $(".lookschedulerrecurring").hide();
                    }
                } else {
                    $(".lookschednotenable").hide();
                    $(".lookschedulerrecurring").hide();
                }

            });

            $('input[type=radio][name="LookScheduler.Type"]').change(function () {
                if (this.value == '2') {
                    $(".lookschedulerrecurring").show();
                }
                else {
                    $(".lookschedulerrecurring").hide();
                }
            });

            $('#LookScheduler_Frequency').on('change', function () {

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

            $("#LookScheduler_DailyFrequency").on('change', function () {
                if (parseInt($(this).val()) == 1) {

                    $("#div_OccursEveryTimeUnit").hide();
                } else {

                    $("#div_OccursEveryTimeUnit").show();
                }
            })

            $("#LookScheduler_MonthlyOccurrence").on('change', function () {
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

            if (parseInt($("#TypeId").val()) == 1) {
                BindGameTable(allGame, '', '');
            } else if (parseInt($("#TypeId").val()) == 2) {
                BindTeamTable(allTeam)
            }
            else if (parseInt($("#TypeId").val()) == 3) {
                BindUserTable(allUser)
            }

            collapseRow();

            $(document).on('click', '.choice_remove', function () {
                var chckId = $(this).data('id');
                $("#" + chckId).val(false);
                $("#" + chckId).prop('checked', false);
                $(this).parent().remove();
            });

            //Global.DataTable('#tbl_game');

            $('.isview').off('change').change(function () {
                $(this).val($(this).is(":checked"));

            });

            $('.iscalculation').off('change').change(function () {
                $(this).val($(this).is(":checked"));

            });

            $("#TypeId").on('change', function () {
                var $tbl = $("#tbl_game tbody");
                var $div = $("#div_add_game");
                $tbl.empty();
                $div.empty();

                if (parseInt($(this).val()) == 1) {
                    $(".span_type").html('Game');
                    BindGameTable(allGame, '', '');

                } else if (parseInt($(this).val()) == 2) {
                    $(".span_type").html('Team');
                    BindTeamTable(allTeam)

                }
                else if (parseInt($(this).val()) == 3) {
                    $(".span_type").html('User');
                    BindUserTable(allUser)
                }

                collapseRow();
            })
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
                                    'name': 'LookGame[' + (rowIndex - 1) + '].GameId',
                                    'value': row.Id
                                }).appendTo(this)

                                isAdded = gameRole != '' && gameRole.Id > 0;

                                if (isAdded) {
                                    $('<input/>', {
                                        'type': 'hidden',
                                        'name': 'LookGame[' + (rowIndex - 1) + '].Id',
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
                                        'name': 'LookGame[' + (rowIndex - 1) + '].IsAdded',
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
                                        'name': 'LookGame[' + (rowIndex - 1) + '].IsAdded'
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

        function BindTeamTable(allarr) {
            rowIndex = 0;
            var $tbl = $("#tbl_game tbody");
            var $div = $("#div_add_game");
            for (var i = 0; i < allarr.length; i++) {
                rowIndex++;
                var row = allarr[i];
                var isAdded = false;
                var gameRole = GetTeamValue(lookTeamList, row.Id);
                $('<tr/>', {
                    html: function () {
                        $('<td/>', {
                            'html': rowIndex
                        }).appendTo(this);

                        $('<td/>', {
                            'html': row.Name
                        }).appendTo(this);


                        $('<td/>', {
                            'html': function () {

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'LookTeam[' + (rowIndex - 1) + '].TeamId',
                                    'value': row.Id
                                }).appendTo(this)

                                isAdded = gameRole != '' && gameRole.Id > 0;

                                if (isAdded) {
                                    $('<input/>', {
                                        'type': 'hidden',
                                        'name': 'LookTeam[' + (rowIndex - 1) + '].Id',
                                        'value': gameRole.Id
                                    }).appendTo(this)

                                    $('<input/>', {
                                        'type': "checkbox",
                                        'class': 'added',
                                        'data-type': 'team',
                                        'id': 'team_' + row.Id,
                                        'data-id': row.Id,
                                        'data-name': row.Name,
                                        'value': isAdded,
                                        'name': 'LookTeam[' + (rowIndex - 1) + '].IsAdded',
                                        'data-divid': 'div_add_game',
                                        'checked': "checked"
                                    }).appendTo(this);
                                } else {

                                    $('<input/>', {
                                        'type': "checkbox",
                                        'class': 'added',
                                        'data-type': 'team',
                                        'id': 'team_' + row.Id,
                                        'data-id': row.Id,
                                        'data-name': row.Name,
                                        'value': isAdded,
                                        'data-divid': 'div_add_game',
                                        'name': 'LookTeam[' + (rowIndex - 1) + '].IsAdded'
                                    }).appendTo(this);
                                }

                            }
                        }).appendTo(this);
                    }

                }).appendTo($tbl);

                isAdded = gameRole != '' && gameRole.Id > 0;
                if (isAdded) {
                    $('<label/>', {

                        'id': 'span_team_' + row.Id,
                        'html': function () {
                            $('<label/>', {
                                html: row.Name
                            }).appendTo(this);
                            $('<a/>', {
                                'href': 'javascript:void(0);',
                                'data-txt': row.Name,
                                'data-id': 'team_' + row.Id,
                                'class': 'choice_remove',
                                'html': '<i class="fa fa-close"></i>'
                            }).appendTo(this)
                        }
                    }).appendTo($div);
                }

            }

        }

        function BindUserTable(allarr) {
            rowIndex = 0;
            var $tbl = $("#tbl_game tbody");
            var $div = $("#div_add_game");

            for (var i = 0; i < allarr.length; i++) {
                rowIndex++;
                var row = allarr[i];
                var isAdded = false;
                var gameRole = GetUserValue(lookUserList, row.Id);
                $('<tr/>', {
                    html: function () {
                        $('<td/>', {
                            'html': rowIndex
                        }).appendTo(this);

                        $('<td/>', {
                            'html': row.Name
                        }).appendTo(this);


                        $('<td/>', {
                            'html': function () {

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'LookUser[' + (rowIndex - 1) + '].UserId',
                                    'value': row.Id
                                }).appendTo(this)

                                isAdded = gameRole != '' && gameRole.Id > 0;

                                if (isAdded) {
                                    $('<input/>', {
                                        'type': 'hidden',
                                        'name': 'LookUser[' + (rowIndex - 1) + '].Id',
                                        'value': gameRole.Id
                                    }).appendTo(this)
                                    $('<span/>', {
                                        //'style ':'padding-right: 10px;',
                                        html: row.Additional
                                    }).appendTo(this)
                                    $('<input/>', {
                                        'type': "checkbox",
                                        'class': 'added ',
                                        'data-type': 'user',
                                        'id': 'user_' + row.Id,
                                        'data-id': row.Id,
                                        'data-name': row.Name,
                                        'value': isAdded,
                                        'name': 'LookUser[' + (rowIndex - 1) + '].IsAdded',
                                        'data-divid': 'div_add_user',
                                        'checked': "checked"
                                    }).appendTo(this);
                                } else {

                                    $('<input/>', {
                                        'type': "checkbox",
                                        'class': 'added ',
                                        'data-type': 'user',
                                        'id': 'user_' + row.Id,
                                        'data-id': row.Id,
                                        'data-name': row.Name,
                                        'value': isAdded,
                                        'data-divid': 'div_add_game',
                                        'name': 'LookUser[' + (rowIndex - 1) + '].IsAdded'
                                    }).appendTo(this);
                                }

                            }
                        }).appendTo(this);
                    }

                }).appendTo($tbl);

                isAdded = gameRole != '' && gameRole.Id > 0;
                if (isAdded) {
                    $('<label/>', {
                        'id': 'span_user_' + row.Id,
                        'html': function () {
                            $('<label/>', {
                                html: row.Name
                            }).appendTo(this);
                            $('<a/>', {
                                'href': 'javascript:void(0);',
                                'data-txt': row.Name,
                                'data-id': 'user_' + row.Id,
                                'class': 'choice_remove',
                                'html': '<i class="fa fa-close"></i>'
                            }).appendTo(this)
                        }
                    }).appendTo($div);
                }

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

        function DatatableBind() {

           // Global.DataTable('#tbl_look');

           // Global.DataTableWithOutPage('#tbl_user');

           // Global.DataTable('#tbl_attribute');

           // Global.DataTable('#tbl_game');           

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
        var self = new LookAddEdit();
        self.init();
    })
})(jQuery)