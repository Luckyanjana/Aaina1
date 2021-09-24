(function ($) {
    function UserAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        function initilizeModel() {

            form = new Global.FormHelperWithFiles($("#frm-add-edit-user").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            $("#ExpCertFile").change(function () {
                Global.DocumentValidate(this, 'ExpCertFile');
            });

            $("#IdProffFileFile").change(function () {
                Global.DocumentValidate(this, 'IdProffFileFile');
            });

            $("#PoliceVerificationFile").change(function () {
                Global.DocumentValidate(this, 'PoliceVerificationFile');
            });

            $("#EduCertFile").change(function () {
                Global.DocumentValidate(this, 'EduCertFile');
            });

            $("#OtherFile").change(function () {
                Global.DocumentValidate(this, 'OtherFile');
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


            team_chart = $('#gameorgChart').orgChart({
                data: gamedata,
                showControls: false,
                allowEdit: false
            });


            $('.datepicker').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
            $('.datepicker').datepicker({
                autoclose: true,
                format: "dd/mm/yyyy"
            });
            BindGameTable(allGame, '', '');
            collapseRow();

            $('.button_uploader').unbind().click(function () {                
                $("#AvatarUrlfile").trigger("click");
            });

            $("#AvatarUrlfile").change(function () {
                Global.ValidateImage(this, 'AvatarUrlfile', 'view_AvatarUrlfile');
                
            });

            // Global.DataTableWithOutPage('#tbl_user');
            
        }


        function BindGameTable(allarr, extraSpace, style) {
            rowIndex++;
            let extraSpace1 = "";
            let style1 = 'display: none;'
            var sb = "";
            var $tbl = $("#tbl_user tbody");
            for (var i = 0; i < allarr.length; i++) {
                var row = allarr[i];
                var isAdded = false;
                var gameRole = GetValueFromList(gameRoleList, row.Id);
                
                $('<tr/>', {
                    'class': row.ParentId,
                    'style': style,
                    html: function () {
                        $('<td/>', {
                            'html': function () {
                                $('<span/>', {                                   
                                    'html': row.Id
                                }).appendTo(this)
                                $('<input/>', {
                                    'type':'hidden',
                                    'name': 'GameRoleList[' + (rowIndex - 1) + '].GameId',
                                    'value': row.Id
                                }).appendTo(this)

                                $('<input/>', {
                                    'type': 'hidden',
                                    'name': 'GameRoleList[' + (rowIndex - 1) + '].Id',
                                    'value': (gameRole != '' && gameRole.RoleId > 0 ? gameRole.Id:'')
                                }).appendTo(this)
                            }
                        }).appendTo(this);

                        $('<td/>', {
                            'html': extraSpace + ' ' + row.Name + (row.ChildGame.length > 0 ? "<a data-toggle='collapse' data-parent='accordion'  href='javascript:void(0)' id='" + row.Id + "'><span class='fa fa-plus'></span> </a>" : "")
                        }).appendTo(this);

                        $('<td/>', {
                            'html': function () {
                                $('<select/>', {
                                    'class': 'form-control',
                                    'name': 'GameRoleList[' + (rowIndex - 1) + '].RoleId',
                                    'class': 'roleadd ' + ' game_' + row.ParentId,
                                    'data-type': 'game',
                                    'id': 'game_' + row.Id,
                                    'data-id': row.Id,                                    
                                    'html': function () {
                                        $('<option/>', {
                                            'text': '--Select--',
                                            'value': '',
                                        }).appendTo(this);

                                        for (var i = 0; i < roleList.length; i++) {
                                            var item = roleList[i];
                                            // isAdded = gameRoleList.some(code => code.GameId === row.Id && code.RoleId == item.Id);
                                            isAdded = gameRole != '' && gameRole.RoleId == item.Id;
                                            
                                            if (isAdded) {
                                                $('<option/>', {
                                                    'text': item.Name,
                                                    'value': item.Id,
                                                    'Selected': "Selected"
                                                }).appendTo(this);
                                            } else {
                                                $('<option/>', {
                                                    'text': item.Name,
                                                    'value': item.Id
                                                }).appendTo(this);
                                            }

                                        }
                                    }
                                }).appendTo(this);

                            }
                        }).appendTo(this);

                        $('<td/>', {
                            'html': function () {
                                var item = roleList[i];
                                //isAdded = gameRoleList.some(code => code.GameId === item.Id);
                                isAdded = gameRole != '' && gameRole.RoleId>0;

                                if (isAdded) {
                                    $('<input/>', {
                                        'type': "checkbox",
                                        'name': 'GameRoleList[' + (rowIndex - 1) + '].IsAdded',
                                        'class': 'added',
                                        'value': isAdded,
                                        'checked': "checked"
                                    }).appendTo(this);
                                } else {
                                    $('<input/>', {
                                        'type': "checkbox",
                                        'class': 'added',
                                        'value': isAdded,
                                        'name': 'GameRoleList[' + (rowIndex - 1) + '].IsAdded',
                                    }).appendTo(this);
                                }

                            }
                        }).appendTo(this);
                    }

                }).appendTo($tbl);

                if (row.ChildGame.length > 0) {
                    extraSpace1 = extraSpace + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    
                }
                BindGameTable(row.ChildGame, extraSpace1, style1);

            }
            
        }

        function collapseRow() {

            $('.added').off('change').change(function () {
                $(this).val($(this).is(":checked"));
            });

            $('.roleadd').off('change').change(function () {
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
            var selectedId = $data.val();            
            var id = $data.attr('id');
            $("." + id).val(selectedId);
            $("." + id).trigger('change');

            //$("." + id).each(function () {
            //    CheckUnChekChild($(this));
            //});
        }

        function GetValueFromList(groupArr, gameId) {
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

        

        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };

          
            initilizeModel();
        }
    }

    $(function () {
        var self = new UserAddEdit();
        self.init();
    })
})(jQuery)