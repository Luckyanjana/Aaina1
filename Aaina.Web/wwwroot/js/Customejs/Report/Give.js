(function ($) {
    function SessionAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;



        function initilizeModel() {

            EventLoad();
            $("#add_new_row").on('click', function () {

                var rowIndex = $("#tbl_report_give tbody tr").length;
                var templateClone = $("#tr_report_clone").clone();
                templateClone.appendTo("#tbl_report_give tbody");

                $("#tbl_report_give #tr_report_clone td").each(function () {

                    for (var i = 0; i < $(this).find('input').length; i++) {
                        var nameProp = $($(this).find('input')[i]).attr('name');
                        nameProp = nameProp.replace('#userIndex#', rowIndex);
                        $($(this).find('input')[i]).attr('name', nameProp);

                        if ($($(this).find('input')[i]).attr('data-id') != undefined) {
                            var nameProp1 = $($(this).find('input')[i]).attr('data-id');
                            nameProp1 = nameProp1.replace('#userIndex#', rowIndex);
                            $($(this).find('input')[i]).attr('data-id', nameProp1);
                        }

                        if ($($(this).find('input')[i]).attr('id') != undefined) {
                            var nameProp1 = $($(this).find('input')[i]).attr('id');
                            nameProp1 = nameProp1.replace('#userIndex#', rowIndex);
                            $($(this).find('input')[i]).attr('id', nameProp1);
                        }
                    }

                    if ($(this).find('.range-value').length > 0) {
                        var nameProp1 = $(this).find('.range-value').attr('id');
                        nameProp1 = nameProp1.replace('#userIndex#', rowIndex);
                        $(this).find('.range-value').attr('id', nameProp1);
                    }

                    for (var i = 0; i < $(this).find('select').length; i++) {
                        var nameProp = $($(this).find('select')[i]).attr('name');
                        nameProp = nameProp.replace('#userIndex#', rowIndex);
                        $($(this).find('select')[i]).attr('name', nameProp);
                    }

                    if ($(this).find('span').length > 0) {
                        var nameProp = $(this).find('span').attr('data-valmsg-for');
                        if (nameProp != undefined) {
                            nameProp = nameProp.replace('#userIndex#', rowIndex);
                            $(this).find('span').attr('data-valmsg-for', nameProp);
                        }
                    }

                });
                $("#tbl_report_give #tr_report_clone").attr('id', '');

                EventLoad();
            });

            $(document).on('click', '#remove_new_row', function () {
                $(this).closest('tr').remove();
                BindRemoveButton();
            });

            $("#btn-reject").on('click', function () {
                $("#IsReject").val('true');
                $("#report_give_update_form").submit();
            })
            $("#btn-Submit-data").on('click', function () {
                $("#duplicate-popup").modal("show");
            })

            $("#btnDuplicate").on('click', function () {
                $("#IsDuplicate").val(true);
                $("#duplicate-popup").modal("hide");
                $("#report_give_update_form").submit();
            })
            $("#btnUpdaterecord").on('click', function () {
                $("#IsDuplicate").val(false);
                $("#duplicate-popup").modal("hide");
                $("#report_give_update_form").submit();
            })

        }

        function EventLoad() {
            $('.che_box').change(function () {
                $(this).val($(this).is(":checked"));
            });

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

            $(document).on('click', '.range_change', function () {
                var emotionId = $(this).attr('id');
                var view_emotion = $(this).data('id');
                var range = document.getElementById(emotionId),
                    rangeV = document.getElementById(view_emotion);
                const
                    newValue = Number((range.value - range.min) * 100 / (range.max - range.min)),
                    newPosition = 10 - (range.value * 5.5);
                rangeV.innerHTML = `<span><img src="${Global.GetEmojiName(range.value)}" class="imgemoji" /></span>`;
                rangeV.style.left = `calc(${newValue}% + (${newPosition}px))`;

            });

            BindRemoveButton();
        }

        function BindRemoveButton() {
            $(".remove_new_row").remove();
            var $tableBody = $('#tbl_report_give tbody tr:last').find('.td_action');
            $($tableBody).html('<button type="button" id="remove_new_row" class="btn btn-primary remove_new_row">Remove</button>');
        }




        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };

            initilizeModel();
        }
    }

    $(function () {
        var self = new SessionAddEdit();
        self.init();
    })
})(jQuery)