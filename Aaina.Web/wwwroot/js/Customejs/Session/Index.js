(function ($) {
    function UserAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        function initilizeModel() {

            BindGrid(
                {
                    controller: "session",
                    actionName: "index",
                    reportName: "Session",
                    companyId: companyId,
                    filter: true,
                    searching: false,
                    columns: [
                        { "data": "id", "title": "Id", "type": "int", "orderable": false, "searchable": false },
                        { "data": "name", "title": "Session Name", "type": "string", "orderable": true, "searchable": true },
                        { "data": "gameId", "title": "Game Name", "type": "string", "orderable": false, "searchable": false },
                        { "data": "typeId", "title": "Session Type", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: typeListArr } },
                        { "data": "modeId", "title": "Mode", "type": "string", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: modeListArr }},
                        { "data": "isActive", "title": "Status", "type": "bool", "orderable": true, "searchable": true },
                        { "data": "startDate", "title": "Start", "type": "date", "orderable": false, "searchable": false },
                        { "data": "endDate", "title": "End", "type": "date", "orderable": false, "searchable": false },
                        { "data": "createdDate", "title": "Created Date", "date": "int", "orderable": true, "searchable": true },

                        {
                            "data": null, "title": "Action",
                            "targets": -1,
                            "class": "column-action text-left",
                            "sClass": "text-left",
                            "shorting": false,
                            "orderable": false,
                            "mRender": function (data, type, record) {
                                var btns = '';
                                if (currentMenuPermissionObj.IsEdit) {
                                    btns += "<a href='/" + tenant + "/session/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>";
                                    btns += "<a href='/" + tenant + "/session/Edit?copyId=" + record.id + "' title='Edit'><i class='fa fa-clone mtx'></i></a>";

                                }
                                if (currentMenuPermissionObj.IsDelete) {
                                    btns += '<a href="/' + tenant + '/session/Delete/' + record.id + '" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-action-delete-session"><i class="fa fa-trash-o mtx"></i></a>'
                                }
                                return btns;
                            }
                        },
                    ]

                });

            Global.ModelHelper($("#modal-action-delete-session"), function () {
                form = new Global.FormHelper($("#modal-action-delete-session").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-game'
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