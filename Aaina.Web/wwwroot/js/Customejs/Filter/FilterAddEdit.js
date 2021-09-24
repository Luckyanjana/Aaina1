(function ($) {
    function FilterAddEdit() {
        var $this = this, form;
        var validationSettings;

        function initilizeModel() {

            form = new Global.FormHelper($("#frm-add-edit-filter").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            $('.select2').select2();

            $('#StartDateTime').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var minDate = new Date(selected.date.valueOf());
                $('#EndDateTime').datepicker('setStartDate', minDate);
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $('#EndDateTime').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var maxDate = new Date(selected.date.valueOf());
                $('#StartDateTime').datepicker('setEndDate', new Date(maxDate));
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });



            if ($('#EndDateTime').val() != "") {
                var minDate = new Date(moment($("#EndDateTime").val(), "DD/MM/YYYY"));
                $('#StartDateTime').datepicker('setEndDate', minDate);
            }

            if ($('#StartDateTime').val() != "") {
                var minDate = new Date(moment($("#StartDateTime").val(), "DD/MM/YYYY"));
                $('#EndDateTime').datepicker('setStartDate', minDate);
            }

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

            $('.IsView').change(function () {
                $(this).val($(this).is(":checked"));
            });

            Global.ModelHelper($("#modal-action-delete-filter"), function () {
                form = new Global.FormHelper($("#modal-action-delete-filter").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-filter'
                }, null, null);

            }, null);

            $("#EmotionsFor").on('change', function () {
                var forId = $("#ForIds");
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

            $("#EmotionsFrom").on('change', function () {

                var fromId = $("#FromIds");
                fromId.empty();

                if (parseInt($(this).val()) == 1) {
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
                        }).appendTo(fromId)
                    }
                })
            })


            //Global.DataTableWithOutPage('#tbl_user');


        }


        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };

            
           // Global.DataTable('#tbl_filter');
            initilizeModel();
        }
    }

    $(function () {
        var self = new FilterAddEdit();
        self.init();
    })
})(jQuery)