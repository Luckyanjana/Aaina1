(function ($) {
    function profile() {
        var $this = this, form;

        function initilizeModel() {
            form = new Global.FormHelperWithFiles($("#frm-profile").find("form"), {
                updateTargetId: "validation-summary"
            }, null, null);

            form.find("#StateId").on('change', function () {
                var id = $(this).val();
                var $district = form.find('#DistrictId');
                $district.empty();
                $district.append('<option>Select</option>');
                if (id != '') {
                    $.ajax({
                        type: "POST", url: '/account/GetDistrict', data: { id: id }, success: function (response) {
                            if (response != null && response != undefined) {
                                if (response != null && response.data.length > 0) {
                                    for (var i = 0; i < response.data.length; i++) {
                                        $district.append('<option value="' + response.data[i].id + '">' + response.data[i].name + '</option>');
                                    }
                                }
                            }

                        }
                    });

                }
            });
        }

        $this.init = function () {
            initilizeModel();
        }
    }

    $(function () {
        var self = new profile();
        self.init();
    })
})(jQuery)