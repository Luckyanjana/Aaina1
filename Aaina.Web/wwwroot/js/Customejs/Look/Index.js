(function ($) {
    function UserAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        function initilizeModel() {

            BindGrid(
                {
                    controller: "look",
                    actionName: "index",
                    reportName: "Look",
                    companyId: companyId,
                    filter: true,
                    searching: false,
                    columns: [
                        { "data": "id", "title": "Id", "type": "int", "orderable": false, "searchable": false },
                        { "data": "name", "title": "Name", "type": "string", "orderable": true, "searchable": true },                        
                        { "data": "isActive", "title": "Status", "type": "bool", "orderable": true, "searchable": true },
                        { "data": "frequency", "title": "Frequency", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: scheduleFrequencyArr } },
                        { "data": "createdBy", "title": "Creted by", "type": "string", "orderable": false, "searchable": false },
                        { "data": "createdDate", "title": "Creted On", "type": "date", "orderable": true, "searchable": true },
                        {
                            "data": null, "title": "Action",
                            "targets": -1,
                            "class": "column-action text-left",
                            "sClass": "text-left",
                            "shorting": false,
                            "orderable": false,
                            "mRender": function (data, type, record) {
                                var btns = '';
                                if (currentUserObj.RoleId == 3) {
                                    if (currentMenuPermissionObj.IsEdit) {
                                        btns += "<a href='/" + tenant + "/look/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>";
                                        btns += "<a href='/" + tenant + "/look/Edit?copyId=" + record.id + "' title='Edit'><i class='fa fa-clone mtx'></i></a>";

                                    }
                                }

                                if (currentUserObj.RoleId == 2) {
                                    btns += "<a href='/" + tenant + "/look/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>";
                                    btns += "<a href='/" + tenant + "/look/Edit?copyId=" + record.id + "' title='Edit'><i class='fa fa-clone mtx'></i></a>";
                                    
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