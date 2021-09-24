(function ($) {
    function AdminAddEdit() {
        var $this = this, form;

       
        function initilizeModel() {

            $("#modal-add-edit-admin").on('loaded.bs.modal', function (e) {
                form = new Global.FormHelper($(this).find("form"), { updateTargetId: "validation-summary" }, null, null);
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });

            $("#modal-delete-admin").on('loaded.bs.modal', function (e) {
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
        var self = new AdminAddEdit();
        self.init();
    })
})(jQuery)