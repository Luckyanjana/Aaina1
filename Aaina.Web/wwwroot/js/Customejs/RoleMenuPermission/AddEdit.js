(function ($) {
    function RoleMenuPermissinAddEdit() {
        var $this = this, form;


        function initilizeModel() {
            $("#save").click(function () {
                var checkboxarr = [];
                $("#tbl_role tbody tr").each(function (index, value) {
                    
                    var roleid = $(this).find(".roleid");
                    var menuid = $(this).find(".menuid");
                    
                        var RoleMenuPermissionDto = {
                            MenuId: menuid.val(),
                            RoleId: roleid.val(),
                            IsList: $(this).find('.IsList').is(":checked"),
                            IsView: $(this).find('.IsView').is(":checked"),
                            IsAdd: $(this).find('.IsAdd').is(":checked"),
                            IsEdit: $(this).find('.IsEdit').is(":checked"),
                            IsDelete: $(this).find('.IsDelete').is(":checked"),
                            IsMain: $(this).find('.IsMain').val(),
                        }
                        checkboxarr.push(RoleMenuPermissionDto);
                    

                })
                console.log(checkboxarr);
                $.ajax({
                    type: "POST",
                    url: "/RoleMenuPermission/Save",
                    data: {
                        "model": checkboxarr
                    },
                    success: function (r) {
                        window.location.reload();
                    }
                });
            });

            $("#roleId").on('change', function () {
                var roleId = $(this).val();
                window.location.href = '/admin/RoleMenuPermission/index?roleId=' + roleId
            });

            $(".parentCheck").on('click', function () {
                var dataId = $(this).data('id');
                $('.' + dataId).prop('checked', $(this).is(":checked"));
            });

            $("#selectAll").on('click', function () {
                $('.IsList').prop('checked', $(this).is(":checked"));
                $('.IsView').prop('checked', $(this).is(":checked"));
                $('.IsAdd').prop('checked', $(this).is(":checked"));
                $('.IsEdit').prop('checked', $(this).is(":checked"));
                $('.IsDelete').prop('checked', $(this).is(":checked"));
            });
        }

        $this.init = function () {

            initilizeModel();
        }
    }

    $(function () {
        var self = new RoleMenuPermissinAddEdit();
        self.init();
    })
})(jQuery)