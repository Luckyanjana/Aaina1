(function ($) {
    function UserAddEdit() {
        var $this = this, form;
        var validationSettings, team_chart, rowIndex = 0;

        function initilizeModel() {

            BindGrid(
                {
                    controller: "user",
                    actionName: "index",
                    reportName: "user",                    
                    companyId: companyId,
                    filter: true,
                    searching: false,
                    columns: [
                        { "data": "id", "title": "Id", "type": "int", "orderable": false, "searchable": false },
                        { "data": "fname", "title": "First Name", "type": "string", "orderable": true, "searchable": true },
                        { "data": "lname", "title": "Last Name", "type": "string", "orderable": true, "searchable": true },
                        { "data": "playerType", "title": "Type", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: playersTypeListArr } },
                        { "data": "userName", "title": "User Name", "type": "string", "orderable": true, "searchable": true },
                        { "data": "isActive", "title": "IsActive", "type": "bool", "orderable": true, "searchable": true },

                        {
                            "data": null, "title": "Action",
                            "targets": -1,
                            "class": "column-action text-left",
                            "sClass": "text-left",
                            "shorting": false,
                            "orderable": false,
                            "mRender": function (data, type, record) {
                                var btns = '';
                                btns += "<a href='/" + tenant + "/user/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>";

                                return btns;
                            }
                        },
                    ]

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