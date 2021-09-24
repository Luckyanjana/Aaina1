(function ($) {
    function WeightageAddEdit() {
        var $this = this, form;
       
        function initilizeModel() {

            $("#modal-add-edit-weightage").on('loaded.bs.modal', function (e) {
                form = new Global.FormHelperWithFiles($(this).find("form"), { updateTargetId: "validation-summary" }, null, null);
                $("#emojiImg").change(function () {
                    Global.ValidateImage(this, 'emojiImg', 'view_emojiImg');

                });
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });

            $("#modal-delete-weightage").on('loaded.bs.modal', function (e) {
                form = new Global.FormHelper($(this).find("form"), { updateTargetId: "validation-summary" }, null, null);
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }

        $this.init = function () {

            Global.DataTable('#tbl_emoji');

            initilizeModel();
        }
    }

    $(function () {
        var self = new WeightageAddEdit();
        self.init();
    })
})(jQuery)