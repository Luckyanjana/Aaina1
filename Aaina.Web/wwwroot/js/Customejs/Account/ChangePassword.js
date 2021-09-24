(function ($) {
    function ChangePassword() {
        var $this = this, form;

        function initilizeModel() {
            form = new Global.FormHelper($("#frm-changepassword").find("form"), {
                updateTargetId: "validation-summary"
            }, null, null);

            $("#showpassword").on('click', function () {
                if ($(this).hasClass('password')) {
                    $("#CurrentPassword").attr('type', 'text');
                    $("#NewPassword").attr('type', 'text');
                    $("#ConfirmPassword").attr('type', 'text');
                    $(this).removeClass('password');
                } else {
                    $("#CurrentPassword").attr('type', 'password');
                    $("#NewPassword").attr('type', 'password');
                    $("#ConfirmPassword").attr('type', 'password');
                    $(this).addClass('password');
                }
            });
        }

        $this.init = function () {
            initilizeModel();
        }
    }

    $(function () {
        var self = new ChangePassword();
        self.init();
    })
})(jQuery)