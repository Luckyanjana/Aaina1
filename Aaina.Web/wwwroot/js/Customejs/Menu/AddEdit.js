(function ($) {
    function MenuAddEdit() {
        var $this = this, form;

       
        function initilizeModel() {

            Global.ModelHelper($("#modal-add-edit-menu"), function () {
                form = new Global.FormHelper($("#modal-add-edit-menu").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-add-edit-role'
                }, null, null);               

            }, null);

            Global.ModelHelper($("#modal-delete-menu"), function () {
                form = new Global.FormHelper($("#modal-delete-menu").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-delete-menu'
                }, null, null);

            }, null);
        }

        $this.init = function () {
            Global.DataTableWithOutPage('#tbl_menu');
            initilizeModel();
        }
    }

    $(function () {
        var self = new MenuAddEdit();
        self.init();
    })
})(jQuery)