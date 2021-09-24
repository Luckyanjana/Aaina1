(function ($) {
    function TeamAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, player_chart;

        function initilizeModel() {

            form = new Global.FormHelper($("#frm-add-edit-game").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

           
            AddChartRow();

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


            if (gamedata.length > 0) {
                team_chart = $('#gameorgChart').orgChart({
                    data: gamedata,
                    showControls: false,
                    allowEdit: false
                });
            }

            player_chart = $('#playerorgChart').orgChart({
                data: playerdata,
                showControls: false,
                allowEdit: false
            });          

            $("#Name").on('change', function () {
                player_chart.UpdateNodeValueByIndex(-1, $(this).val());
            });          

            Global.ModelHelper($("#modal-action-delete-team"), function () {
                form = new Global.FormHelper($("#modal-action-delete-team").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-team'
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

                        $("#tbl_user tbody").append('<tr><td>' + (index + 1) + '</td> <td><input type="hidden" name="TeamPlayer[' + index + '].UserId" value="' + result.data.id + '">' + result.data.name + '</td><td><input type="hidden" name="TeamPlayer[' + index + '].TypeId" value="' + result.data.playersTypeId + '">' + result.data.playersType + '</td><td><select name="TeamPlayer[' + index + '].roleId" class="form-control roledrop ignore"> <option value="">Select</option> ' + $ddl + '</select></td><td><input type="checkbox" name="TeamPlayer[' + index + '].IsAdded" class="added" data-user="' + result.data.id + '" data-username="' + result.data.name + '" value="false"></td></tr>');
                        AddChartRow();
                        $("#modal-add-edit-user").modal('hide').data('bs.modal', null);
                        Global.HideLoading();
                        Global.ToastrSuccess('User successfully added');
                    } else {
                        $("#validation-summary").html(JSON.parse(jqXHR.responseText).value.data);
                    }
                }, null);

            }, null);

           
        }

        function AddChartRow() {
            $('.added').change(function () {
                $(this).val($(this).is(":checked"));
                var $tr = $(this).closest('tr');
                PlayerChartUpdate($tr);
            });

            $(".roledrop").on('change', function () {
                var $tr = $(this).closest('tr');
                PlayerChartUpdate($tr);
            });
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
        
        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };

            Global.DataTableWithOutPage('#tbl_user');

            Global.DataTableWithOutPage('#tbl_team');
            initilizeModel();
        }
    }

    $(function () {
        var self = new TeamAddEdit();
        self.init();
    })
})(jQuery)