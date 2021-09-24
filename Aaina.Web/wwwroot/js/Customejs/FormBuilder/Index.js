(function ($) {
    function FormBuilderIndex() {
        var $this = this;

        function initilize() {

            Global.DataTable('#tbl_formbuilder');

            Global.ModelHelper($("#modal-delete-formbuilder"), function () {
                form = new Global.FormHelper($("#modal-delete-formbuilder").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-delete-formbuilder'
                }, null, null);

            }, null);

          
        }
       
       
        $this.init = function () {
            initilize();
        }
    }

    $(function () {
        var self = new FormBuilderIndex();
        self.init();
    })
})(jQuery)

//function onCopied($this) {
//   // debugger;
//    $('#IsCoppied').val(true);
//    $('#CopyId').val(parseInt($($this).attr('data-id')));
//    return true;
//    //debugger;
//    //window.location.href = "/admin/formbuilder/addedit";
//}