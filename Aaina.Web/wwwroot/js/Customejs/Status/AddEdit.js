(function ($) {
    function StatusAddEdit() {
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

      

        var StatusNotificationViewModel = function () {
            var self = this;
            self.notificationslistarr = ko.observableArray([]);

            if (typeof statusReminderArr !== 'undefined' && statusReminderArr.length > 0) {

                $.each(statusReminderArr, function (i, el) {
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

            form = new Global.FormHelperWithFiles($("#frm-add-edit-status").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);

            $('.select2').select2();

            $('#StatusScheduler_StartDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,                
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var minDate = new Date(selected.date.valueOf());
                $('#StatusScheduler_EndDate').datepicker('setStartDate', minDate);
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

            $('#StatusScheduler_EndDate').datepicker({
                keyboardNavigation: false,
                forceParse: false,
                toggleActive: false,
                autoclose: true,                
                format: 'dd/mm/yyyy'
            }).on('changeDate', function (selected) {
                var maxDate = new Date(selected.date.valueOf());
                $('#StatusScheduler_StartDate').datepicker('setEndDate', new Date(maxDate));
            }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

           

            if ($('#StatusScheduler_EndDate').val() != "") {                
                var minDate = new Date(moment($("#StatusScheduler_EndDate").val(), "DD/MM/YYYY"));
                $('#StatusScheduler_StartDate').datepicker('setEndDate', minDate);
            }

            if ($('#StatusScheduler_StartDate').val() != "") {
                var minDate = new Date(moment($("#StatusScheduler_StartDate").val(), "DD/MM/YYYY"));
                $('#StatusScheduler_EndDate').datepicker('setStartDate', minDate);
            }



            ko.applyBindings(new StatusNotificationViewModel());

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


            $('#StatusScheduler_ColorCode').colorpicker().on('change', function () {
                $(this).css("background-color", $(this).val());
            });

            $('input[type=radio][name="StatusScheduler.Type"]').change(function () {
                if (this.value == '2') {
                    $(".lookschedulerrecurring").show();
                }
                else {
                    $(".lookschedulerrecurring").hide();
                }
            });

            $('#StatusScheduler_Frequency').on('change', function () {

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

            $("#StatusScheduler_DailyFrequency").on('change', function () {
                if (parseInt($(this).val()) == 1) {

                    $("#div_OccursEveryTimeUnit").hide();
                } else {

                    $("#div_OccursEveryTimeUnit").show();
                }
            })

            $("#StatusScheduler_MonthlyOccurrence").on('change', function () {
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

          
            Global.ModelHelper($("#modal-action-delete-status"), function () {
                form = new Global.FormHelper($("#modal-action-delete-status").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-game'
                }, function (response) {
                        window.location.href ='/admin/status/AddEdit'
                }, null);

            }, null);
            

        
           
        }
      

        

        function DatatableBind() {

         
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
        var self = new StatusAddEdit();
        self.init();
    })
})(jQuery)