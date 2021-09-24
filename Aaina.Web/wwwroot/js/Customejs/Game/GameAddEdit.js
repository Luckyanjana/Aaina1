(function ($) {
    function GameAddEdit() {
        var $this = this, form;
        var validationSettings, game_chart, game_more_chart, player_chart;



        var gameLocation = function (data) {
            var self = this;
            self.location = ko.observable('');
            if (typeof data !== 'undefined') {
                self.location(data.location);
            }
        }



        var gameLocationViewModel = function () {
            var self = this;
            self.gameLocationListarr = ko.observableArray([]);

            if (gameLocationarr != null && gameLocationarr.length > 0) {
                for (var i = 0; i < gameLocationarr.length; i++) {
                    self.gameLocationListarr.push(new gameLocation({ location: gameLocationarr[i] }));
                }
            }

            self.remove = function (data) {
                self.gameLocationListarr.remove(data);
            };
            self.add = function () {
                self.gameLocationListarr.push(new gameLocation({ location: '' }));
            };
        };


        function initilizeModel() {
            Global.DataTable('#tbl_game1');



            form = new Global.FormHelper($("#frm-add-edit-game").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            ko.applyBindings(new gameLocationViewModel());
            $('#ColourCode').colorpicker().on('change', function () {
                $(this).css("background-color", $(this).val());
            });

            $('#FromDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var minDate = new Date(selected.date.valueOf());
                $('#Todate').datepicker('setStartDate', minDate);
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $('#Todate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var maxDate = new Date(selected.date.valueOf());
                $('#FromDate').datepicker('setEndDate', new Date(maxDate));
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });



            if ($('#Todate').val() != "") {
                var minDate = new Date(moment($("#Todate").val(), "DD/MM/YYYY"));
                $('#FromDate').datepicker('setEndDate', minDate);
            }

            if ($('#FromDate').val() != "") {
                var minDate = new Date(moment($("#FromDate").val(), "DD/MM/YYYY"));
                $('#Todate').datepicker('setStartDate', minDate);
            }

            $('#Sameasparent').change(function () {
                if ($(this).is(":checked")) {
                    var parentId = $("#ParentId").val();
                    if (parseInt(parentId) > 0) {
                        $.ajax('/game/getclientname/' + parentId, {
                            type: "GET",
                            success: function (result) {
                                if (result.applyForChild)
                                    $("#ClientName").val(result.clientName);
                            }
                        });
                    }
                }
            });


            $("#ParentId").on('change', function () {
                var parentId = $(this).val();
                if (parseInt(parentId) > 0) {
                    window.location.href = `/${parentId}/game/${$(this).data('action')}`;

                    //$.ajax('/admin/game/GetClientNameForChile/' + parentId, {
                    //    type: "GET",
                    //    success: function (result) {
                    //        if (result.applyForChild) {
                    //            if (result.game.fromDate != null) {

                    //                $("#FromDate").val(moment(result.game.fromDate).format('DD/MM/YYYY'));
                    //            }
                    //            if (result.game.todate != null) {

                    //                $("#Todate").val(moment(result.game.todate).format('DD/MM/YYYY'));
                    //            }

                    //            $("#ClientName").val(result.game.clientName);
                    //            $("#Location").val(result.game.location);
                    //            $("#ContactPerson").val(result.game.contactPerson);
                    //            $("#ContactNumber").val(result.game.contactNumber);
                    //            $('.roledrop').val('');
                    //            $('.added').prop('checked',false);
                    //            if (result.game.gamePlayers.length > 0) {
                    //                for (var i = 0; i < result.game.gamePlayers.length; i++) {
                    //                    var player = result.game.gamePlayers[i];
                    //                    var $tr = $("#tr_" + player.userId);                                        
                    //                    if ($tr) {
                    //                        $tr.find('.roledrop').val(player.roleId);
                    //                        $tr.find('.added').val(true);
                    //                        $tr.find('.added').prop('checked',true);
                    //                    }
                    //                }
                    //            }

                    //            $("#tbl_user tbody tr").each(function () {
                    //                PlayerChartUpdate($(this))
                    //            })

                    //        }
                    //    }
                    //});
                }
            });

            $('.added').change(function () {
                $(this).val($(this).is(":checked"));
                var $tr = $(this).closest('tr');
                PlayerChartUpdate($tr);
            });

            $(".roledrop").on('change', function () {
                var $tr = $(this).closest('tr');
                PlayerChartUpdate($tr);
            });

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



            game_chart = $('#gameorgChart').orgChart({
                data: gamedata,
                showControls: false,
                allowEdit: false,
                onClickNode: function (node) {
                    var id = node.data.id;
                    window.location.href = '/admin/game/addedit/' + id;
                }

            });

            game_more_chart = $('#gamemoreorgChart').orgChart({
                data: gamedata,
                showControls: false,
                allowEdit: false,
                onClickNode: function (node) {
                    var id = node.data.id;
                    window.location.href = '/admin/game/addedit/' + id;
                }
            });

            player_chart = $('#playerorgChart').orgChart({
                data: playerdata,
                showControls: false,
                allowEdit: false
            });

            $("#Name").on('change', function () {
                player_chart.UpdateNodeValueByIndex(-1, $(this).val());
            });



            Global.ModelHelper($("#modal-action-delete-game"), function () {
                form = new Global.FormHelper($("#modal-action-delete-game").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-game'
                }, null, null);

            }, null);



            Global.ModelHelper($("#modal-add-edit-user"), function () {

                form = new Global.FormHelper($("#modal-add-edit-user").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-add-edit-user'
                }, function (result) {
                    if (result.isSuccess) {
                        var index = $("#tbl_user tbody tr").length;
                        var $ddl = "";
                        for (var i = 0; i < roleListarr.length; i++) {
                            $ddl += '<option value="' + roleListarr[i].Id + '">' + roleListarr[i].Name + '</option>';
                        }

                        $("#tbl_user tbody").append('<tr><td>' + (index + 1) + '</td> <td><input type="hidden" name="GamePlayers[' + index + '].UserId" value="' + result.data.id + '">' + result.data.name + '</td><td><input type="hidden" name="GamePlayers[' + index + '].TypeId" value="' + result.data.playersTypeId + '">' + result.data.playersType + '</td><td><select name="GamePlayers[' + index + '].roleId" class="form-control roledrop ignore"> <option value="">Select</option> ' + $ddl + '</select></td><td><input type="checkbox" name="GamePlayers[' + index + '].IsAdded" class="added" value="false"></td></tr>');
                        $('.added').change(function () {
                            $(this).val($(this).is(":checked"));
                        });
                        $("#modal-add-edit-user").modal('hide').data('bs.modal', null);
                        Global.HideLoading();
                        Global.ToastrSuccess('User successfully added');
                    } else {
                        $("#validation-summary").html(JSON.parse(jqXHR.responseText).value.data);
                    }
                }, null);


            }, null);
        }

        function PlayerChartUpdate($tr) {
            var $role = $tr.find('.roledrop');
            var $check = $tr.find('.added');
            var roleId = parseInt($role.val());
            var isChecked = $check.is(":checked");
            var userId = parseInt($check.data('user'));
            var userName = $check.data('username');
            var nodeId = maxuserId + userId;

            if (roleId > 0 && isChecked) {
                if (player_chart.IsNodeExist(nodeId)) {
                    player_chart.deleteFromSource(nodeId);
                }

                player_chart.addNodeWithOutEdit({ id: nodeId, name: userName, parent: roleId });
            } else {
                if (player_chart.IsNodeExist(nodeId)) {
                    player_chart.deleteNode(nodeId);
                }
            }

        }

        function DatePicker() {
            $('.datepicker').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
            $('.datepicker').datepicker({
                autoclose: true,
                format: "dd/mm/yyyy"
            });

            $(".fromdatepicker").on('change', function () {
                var $tr = $(this).closest('tr');
                var fromDate = $(this).val();
                var $end = $tr.find('.todatepicker')
                var start = new Date(moment(fromDate, "DD/MM/YYYY").format("MM/DD/YYYY"));
                $end.datepicker('setStartDate', new Date(start));
            });

            $(".todatepicker").on('change', function () {
                var $tr = $(this).closest('tr');
                var fromDate = $(this).val();
                var $end = $tr.find('.fromdatepicker')
                var start = new Date(moment(fromDate, "DD/MM/YYYY").format("MM/DD/YYYY"));
                $end.datepicker('setEndDate', new Date(start));
            });
        }

        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };
           // Global.DataTableWithOutPage('#tbl_user');

            Global.DataTableWithOutPage('#tbl_game1');

            initilizeModel();
        }
    }

    $(function () {
        var self = new GameAddEdit();
        self.init();
    })
})(jQuery)