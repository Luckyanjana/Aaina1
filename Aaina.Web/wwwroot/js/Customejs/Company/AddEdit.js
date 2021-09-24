(function ($) {
    function CompanyAddEdit() {
        var $this = this, form;

       
        function initilizeModel() {

            $("#modal-add-edit-company").on('loaded.bs.modal', function (e) {
                form = new Global.FormHelper($(this).find("form"), { updateTargetId: "validation-summary" }, null, null);
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });

            $("#modal-delete-Company").on('loaded.bs.modal', function (e) {
                form = new Global.FormHelper($(this).find("form"), { updateTargetId: "validation-summary" }, null, null);
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }

        $this.init = function () {
            initilizeModel();
        }
    }

    $(function () {
        var self = new CompanyAddEdit();
        self.init();
    })
})(jQuery)