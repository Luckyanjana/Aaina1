(function ($) {
    function UserAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        function initilizeModel() {
            BindGrid(
                {
                    controller: "report",
                    actionName: "index",
                    reportName: "Report",
                    companyId: companyId,
                    filter: true,
                    searching: false,
                    columns: [
                        { "data": "id", "title": "Id", "type": "int", "orderable": false, "searchable": false },
                        { "data": "name", "title": "Report Name", "type": "string", "orderable": true, "searchable": true },
                        { "data": "typeId", "title": "Report Type", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: typeListArr } },

                        {
                            "data": null, "title": "Action",
                            "targets": -1,
                            "class": "column-action text-left",
                            "sClass": "text-left",
                            "shorting": false,
                            "orderable": false,
                            "mRender": function (data, type, record) {
                                var btns = '';
                                btns += '<a href="/' + tenant + '/Report/ResultRange/' + record.id + '">Result</a>&nbsp;&nbsp;&nbsp;'


                                if (currentUserObj.RoleId == 3 && record.createdBy == currentUserObj.UserId) {
                                    if (currentMenuPermissionObj.IsEdit) {
                                        btns += "<a href='/" + tenant + "/Report/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>&nbsp;&nbsp;&nbsp;";
                                        btns += "<a href='/" + tenant + "/Report/Edit?copyId=" + record.id + "' title='Edit'><i class='fa fa-clone mtx'></i></a> &nbsp;&nbsp;&nbsp;";

                                    }
                                    if (currentMenuPermissionObj.IsDelete) {
                                        btns += '<a href="/' + tenant + '/Report/Delete/' + record.id + '" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-action-delete-report"><i class="fa fa-trash-o mtx"></i></a>&nbsp;&nbsp;&nbsp;'
                                    }
                                }

                                //if (currentUserObj.RoleId == 3 && record.isUpdateGive) {

                                //    btns += '<a href="/' + tenant + '/Report/GiveUpdate?reportId=' + record.id + '">' + (record.isCreator ? 'Edit' : 'Approve')+'</a>&nbsp;&nbsp;&nbsp;'

                                //}

                                btns += '<a href="/' + tenant + '/Report/GiveUpdate?reportId=' + record.id + '">' + (record.isCreator ? 'Edit' : 'Edit') + '</a>&nbsp;&nbsp;&nbsp;'

                                if (currentUserObj.RoleId == 3 && record.isCreator) {

                                    btns += '<a href="/' + tenant + '/Report/Give/' + record.id + '">Give</a>&nbsp;&nbsp;&nbsp;'

                                }

                                if (record.isView) {
                                    if (currentUserObj.RoleId == 3) {
                                        btns += '<a href="/' + tenant + '/Report/View/' + record.id + '">View</a>&nbsp;&nbsp;&nbsp;'
                                    } else {
                                        //btns += '<a href="/' + tenant + '/Report/ResultRange/' + record.id + '">Result</a>&nbsp;&nbsp;&nbsp;'
                                    }


                                }


                                if (currentUserObj.RoleId == 2) {
                                    //btns += "<a href='/" + tenant + "/Report/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>&nbsp;&nbsp;&nbsp;";
                                    btns += "<a href='/" + tenant + "/Report/Edit?copyId=" + record.id + "' title='Edit'><i class='fa fa-clone mtx'></i></a>&nbsp;&nbsp;&nbsp;";
                                    btns += '<a href="/' + tenant + '/Report/Delete/' + record.id + '" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-action-delete-report"><i class="fa fa-trash-o mtx"></i></a>&nbsp;&nbsp;&nbsp;'
                                }

                                return btns;
                            }
                        },
                    ]

                });

            Global.ModelHelper($("#modal-action-delete-report"), function () {
                form = new Global.FormHelper($("#modal-action-delete-report").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-report'
                }, null, null);

            }, null);
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