(function ($) {
    function WeightagePreset() {
        var $this = this, form;
        var validationSettings, rowIndex = 0, rowColIndex;


        function initilizeModel() {

            form = new Global.FormHelperWithFiles($("#frm-add-edit-weightagepreset").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            $("#PresetList").on('change', function () {
                var id = $(this).val();
                var gameId = $("#GameId").val();
                window.location.href = `/admin/game/WeightagePreset?parentId=${gameId}&id=${id}`;
            });

            BindGameTable(gameList, '', '');

            Global.DataTable('#tbl_game');

        }
        rowColIndex = 0;
        function BindGameTable(allarr, extraSpace, style) {
            rowIndex++;
            
            let extraSpace1 = "";
            let style1 = ''

            var $tbl = $("#tbl_game tbody");

            for (var i = 0; i < allarr.length; i++) {
                var row = allarr[i];
                $('<tr/>', {
                    'class': row.ParentId,
                    html: function () {
                        $('<td/>', {
                            'html': rowIndex
                        }).appendTo(this);

                        $('<td/>', {
                            'html': extraSpace + ' ' + row.Name
                        }).appendTo(this);

                        for (var j = 0; j < playerList.length; j++) {

                            var detail = GetDetailValue(presetList, row.Id, parseInt(playerList[j].Id));
                            var isAdded = detail != '' && detail.Weightage > 0;

                            $('<td/>', {
                                'html': function () {
                                    if (isAdded) {
                                        rowColIndex++;
                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': 'WeightagePresetDetails[' + (rowColIndex - 1) + '].GameId',
                                            'value': row.Id
                                        }).appendTo(this)

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': 'WeightagePresetDetails[' + (rowColIndex - 1) + '].RoleId',
                                            'value': detail.RoleId
                                        }).appendTo(this)

                                        $('<input/>', {
                                            'type': 'hidden',
                                            'name': 'WeightagePresetDetails[' + (rowColIndex - 1) + '].UserId',
                                            'value': detail.UserId
                                        }).appendTo(this)
                                        $('<span/>', {
                                            'style': 'float:right;margin-top:-40px;font-size: 40px;color:' + detail.ColorCode,
                                            'html':'.'
                                        }).appendTo(this)

                                        $('<input/>', {
                                            'type': 'text',
                                            'class':'form-control',
                                            'name': 'WeightagePresetDetails[' + (rowColIndex - 1) + '].Weightage',
                                            'value': detail.Weightage,
                                            'style':'width:80%;'
                                        }).appendTo(this)

                                    } else {
                                        $('<span/>', {                                            
                                            'html': '-'
                                        }).appendTo(this)
                                    }
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

      


        function GetDetailValue(groupArr, gameId, userid) {
            if (gameId && userid) {
                var result = $.grep(groupArr, function (e) { return e.GameId == gameId && e.UserId == userid; });
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
        var self = new WeightagePreset();
        self.init();
    })
})(jQuery)