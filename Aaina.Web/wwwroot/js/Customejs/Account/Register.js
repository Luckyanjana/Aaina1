(function ($) {
    function AccountRegister() {
        var $this = this, form;

        function initilizeModel() {

            form = new Global.FormHelper($("#account_rigster").find("form"), { updateTargetId: "validation-summary" }, null, null);            

            $("#showpassword").on('click', function () {
                
                if ($(this).hasClass('password')) {
                    $("#Password").attr('type', 'text');
                    $("#ConfirmPassword").attr('type', 'text');
                    $(this).removeClass('password');
                } else {
                    $("#Password").attr('type', 'password');
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
        var self = new AccountRegister();
        self.init();
    })
})(jQuery)