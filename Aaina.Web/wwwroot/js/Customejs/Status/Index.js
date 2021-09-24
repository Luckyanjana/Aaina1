(function ($) {
    function UserAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        function initilizeModel() {

            BindGrid(
                {
                    controller: "status",
                    actionName: "index",
                    reportName: "Status",
                    companyId: companyId,
                    filter: true,
                    searching: false,
                    columns: [
                        {
                            key: 'select',
                            data: 'id',
                            title: 'id',                            
                            "orderable": false, "searchable": false,
                            visible: currentUserObj.RoleId==3   ,
                            "mRender": function (data, type, record) {
                                var btns = '<input type="radio" name="status_radio" value="' + record.id + '"/>';
                                return btns;
                            }
                        },
                        { "data": "id", "title": "Id", "type": "int", "orderable": false, "searchable": false },
                        { "data": "name", "title": "Session Name", "type": "string", "orderable": true, "searchable": true },
                        { "data": "statusModeId", "title": "Status Mode", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: statusModeArr } },
                        { "data": "isActive", "title": "Status", "type": "bool", "orderable": true, "searchable": true },
                        {
                            "data": null, "title": "Action",
                            "targets": -1,
                            "class": "column-action text-left",
                            "sClass": "text-left",
                            "shorting": false,
                            "orderable": false,
                            "mRender": function (data, type, record) {
                                var btns = '';

                                if (currentUserObj.RoleId == 3 && record.createdBy == currentUserObj.UserId) {
                                    if (currentMenuPermissionObj.IsEdit) {
                                        btns += "<a href='/" + tenant + "/status/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>";
                                        btns += "<a href='/" + tenant + "/status/Edit?copyId=" + record.id + "' title='Edit'><i class='fa fa-clone mtx'></i></a>";

                                    }
                                    if (currentMenuPermissionObj.IsDelete) {
                                        btns += '<a href="/' + tenant + '/status/Delete/' + record.id + '" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-action-delete-status"><i class="fa fa-trash-o mtx"></i></a>'
                                    }
                                }

                                if (currentUserObj.RoleId == 2) {
                                    btns += "<a href='/" + tenant + "/status/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>";
                                    btns += "<a href='/" + tenant + "/status/Edit?copyId=" + record.id + "' title='Edit'><i class='fa fa-clone mtx'></i></a>";
                                    btns += '<a href="/' + tenant + '/status/Delete/' + record.id + '" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-action-delete-status"><i class="fa fa-trash-o mtx"></i></a>'
                                }

                                return btns;
                            }
                        },
                    ]

                });

            Global.ModelHelper($("#modal-action-delete-status"), function () {
                form = new Global.FormHelper($("#modal-action-delete-status").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-status'
                }, null, null);

            }, null);

            $(".status_give").on('click', function () {
                var selectedStatus = $('input[type=radio][name="status_radio"]:checked').val();
                if (parseInt(selectedStatus) > 0) {
                    var url = $(this).data('url') + '/' + selectedStatus;
                    $('#status_feedback_btn').attr('href', url);
                    $('#status_feedback_btn').click();
                } else {
                    var url = $(this).data('url')
                    $('#status_feedback_btn').attr('href', url);
                    $('#status_feedback_btn').click();
                }
            });
        }



        $this.init = function () {
            validationSettings = {
                ignore: '.ignore'
            };


            initilizeModel();
        }
    }

    $(function () {
        var self = new UserAddEdit();
        self.init();
    })
})(jQuery)