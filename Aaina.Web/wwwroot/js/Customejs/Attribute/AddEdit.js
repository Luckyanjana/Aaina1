(function ($) {
    function AttributeAddEdit() {
        var $this = this, form;

        var SubAtter = function (data) {
            var self = this;
            self.id = ko.observable(0);
            self.name = ko.observable('');
            self.weightage = ko.observable('');
            self.desciption = ko.observable('');
            self.isQuantity = ko.observable(false);
            self.unitId = ko.observable('');
            self.unitArr = ko.observable(unitListarr);
            if (typeof data !== 'undefined') {
                self.id(data.id);
                self.name(data.name);
                self.weightage(data.weightage);
                self.desciption(data.desciption);
                self.isQuantity(data.isQuantity);
                self.unitId(data.unitId);
            }
        }



        var SubAtterViewModel = function () {
            var self = this;
            self.subAtterlist = ko.observableArray([]);

            if (typeof subAttributeListarr !== 'undefined' && subAttributeListarr.length > 0) {

                $.each(subAttributeListarr, function (i, el) {
                    self.subAtterlist.push(new SubAtter({ id: el.Id, name: el.Name, weightage: el.Weightage, desciption: el.Desciption, isQuantity: el.IsQuantity, unitId: el.UnitId }));
                });

            } else {
                self.subAtterlist.push(new SubAtter({ id: "", name: "", weightage: "", desciption: "", isQuantity: false, unitId: "" }));
            }


            self.remove = function (data) {
                self.subAtterlist.remove(data);
            };
            self.add = function () {
                self.subAtterlist.push(new SubAtter({ id: "", name: "", weightage: "", desciption: "", isQuantity: false, unitId: "" }));
                CheckUnit();
            };
        };


        function initilizeModel() {

            BindGrid(
                {
                    controller: "attribute",
                    actionName: "index",
                    reportName: "Attribute",
                    companyId: companyId,
                    filter: true,
                    searching: false,
                    columns: [
                        { "data": "id", "title": "Id", "type": "int", "orderable": false, "searchable": false },
                        { "data": "name", "title": "Name", "type": "string", "orderable": true, "searchable": true },
                        { "data": "desciption", "title": "Desciption", "type": "string", "orderable": true, "searchable": false },                      
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

                                btns += '<a href="/Attribute/AddEdit/' + record.id + '" title="Edit" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-add-edit-Attribute"><i class="fa fa-pencil-square-o mtx"></i></a>';
                                btns += '<a href="/Attribute/AddEdit?copyId=' + record.id + '" title="Copy" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-add-edit-Attribute"><i class="fa fa-clone mtx"></i></a>';
                                btns += '<a href="/Attribute/Delete/' + record.id + '" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-delete-Attribute"><i class="fa fa-trash-o mtx"></i></a>';

                                return btns;
                            }
                        },
                    ]

                });

            Global.ModelHelper($("#modal-add-edit-Attribute"), function () {
                form = new Global.FormHelper($("#modal-add-edit-Attribute").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-add-edit-Attribute'
                }, null, null);

                ko.applyBindings(new SubAtterViewModel(), $('#sub_tbl')[0]);

                CheckUnit();


            }, null);

            Global.ModelHelper($("#modal-delete-Attribute"), function () {

                form = new Global.FormHelper($("#modal-delete-Attribute").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-delete-Attribute'
                }, null, null);


            }, null);



        }

        function CheckUnit() {

            $('.isquantity').each(function () {
                var isch = $(this).is(":checked");
                var $tr = $(this).closest('tr');
                if (!isch) {
                    $tr.find('.quantityunit').val('');
                    $tr.find('.quantityunit').hide()
                } else {
                    $tr.find('.quantityunit').show()
                }
            });


            $('.isquantity').on('change', function () {

                $(this).val($(this).is(":checked"));
                var isch = $(this).is(":checked");

                var $tr = $(this).closest('tr');

                if (!isch) {
                    $tr.find('.quantityunit').val('');
                    $tr.find('.quantityunit').hide()
                } else {
                    $tr.find('.quantityunit').show()
                }
            });
        }

        $this.init = function () {
            
            initilizeModel();
        }
    }

    $(function () {
        var self = new AttributeAddEdit();
        self.init();
    })
})(jQuery)