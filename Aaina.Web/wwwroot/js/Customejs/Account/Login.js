(function ($) {
    function AccountLogin() {
        var $this = this, form;



        function initilizeModel() {


            form = new Global.FormHelper($("#account_login").find("form"), { updateTargetId: "validation-summary" }, function (result) {

                if (result.isSuccess) {
                    window.location.href = result.redirectUrl;
                } else {
                    $("#validation-summary").html(result);
                }


            }
                , null
            );


            $(".signin").click(function () {
                if ($("#Password").val() != "") {
                    var key = CryptoJS.enc.Utf8.parse($("span.fa-key").data('id'));
                    var iv = CryptoJS.enc.Utf8.parse($("span.fa-venus").data('id'));
                    var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse($("#Password").val()), key,
                        { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
                    $("#Password").val(encryptedpassword);
                }
            })
        }

        $this.init = function () {

            initilizeModel();
        }
    }

    $(function () {
        var self = new AccountLogin();
        self.init();
    })
})(jQuery)