(function ($) {
    function RoleAddEdit() {
        var $this = this, form;

       
        function initilizeModel() {

            Global.ModelHelper($("#modal-add-edit-role"), function () {
                form = new Global.FormHelper($("#modal-add-edit-role").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-add-edit-role'
                }, null, null);

                $('#ColorCode').colorpicker().on('change', function () {
                    $(this).css("background-color", $(this).val());
                });

            }, null);

            Global.ModelHelper($("#modal-delete-role"), function () {
                form = new Global.FormHelper($("#modal-delete-role").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-delete-role'
                }, null, null);

            }, null);
        }

        $this.init = function () {
            Global.DataTableWithOutPage('#tbl_role');
            initilizeModel();
        }
    }

    $(function () {
        var self = new RoleAddEdit();
        self.init();
    })
})(jQuery)