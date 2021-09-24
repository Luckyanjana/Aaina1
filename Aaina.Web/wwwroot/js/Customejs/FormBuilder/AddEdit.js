(function ($) {
    function FormBuilderAddEdit() {
        var validationSettings,$this = this;


        var attributeItemOptions = function (data) {
            var self = this;
            self.id = ko.observable('');
            self.optionName = ko.observable('');
            if (typeof data !== 'undefined') {
                self.id = data.id;
                self.optionName = data.optionName;
            }
        }

        var attributeItem = function (data) {
            var self = this;
            self.id = ko.observable('');
            self.attributeName = ko.observable('');
            self.dataType = ko.observable('');
            self.dbcolumnName = ko.observable('');
            self.isRequired = ko.observable(false);
            self.orderNo = ko.observable('');
            self.dataTypeList = ko.observableArray(dataTypeList);
            self.dbColumnList = ko.observableArray(dbColumnList);
            self.attributeItemOptions = ko.observableArray([]);
            self.DatatypeOnchange = function (obj, event) {                
                var index = $(event.target).data('index');
                var id = parseInt(this.dataType());
                if (id > 6) {
                    $("#attre_value_" + index).removeClass('hide');
                    if (self.attributeItemOptions().length == 0) {
                        self.attributeItemOptions.push(new attributeItemOptions({ id: '', optionName: '' }));
                    }
                } else {
                    $("#attre_value_" + index).addClass('hide');
                    while (self.attributeItemOptions().length > 0) {
                        self.attributeItemOptions.remove(self.attributeItemOptions()[0]);
                    }
                }
                self.dataType = ko.observable(id);
            };

            if (typeof data !== 'undefined') {
                self.id = data.id;
                self.attributeName = data.attributeName;
                self.dataType = ko.observable(data.dataType);
                self.dbcolumnName = ko.observable(data.dbcolumnName);                
                self.isRequired = data.isRequired;
                self.orderNo = data.orderNo;
                if (data.options != undefined && data.options.length > 0) {
                    for (var i = 0; i < data.options.length; i++) {
                        var item = data.options[i];
                        self.attributeItemOptions.push(new attributeItemOptions({ id: item.Id, optionName: item.OptionName}));
                    }
                    
                }
            }

            self.removeOption = function (item) {
                $('#lblMsg').html('');                
                if (item.id > 0) {
                    $.ajax({
                        type: 'POST',
                        url: '/FormBuilder/DeleteAttributeOptions?attributeId=' + item.id,
                        success: function (result) {
                            if (!result.isSuccess) {
                                $('#lblMsg').html("Attribute options is used in other places.");
                            }
                            else {
                                self.attributeItemOptions.remove(item);
                            }
                        }
                    });

                } else {
                    self.attributeItemOptions.remove(item);
                }

                
            };

            self.addOption = function () {
                
                self.attributeItemOptions.push(new attributeItemOptions({ id: '', optionName: '', }));
            };
        }

        var attributeViewModel = function () {
            var self = this;
            self.attributeItems = ko.observableArray([]);

            if (attributeList != null && attributeList.length > 0) {
                for (var i = 0; i < attributeList.length; i++) {
                   var item = attributeList[i];
                    self.attributeItems.push(new attributeItem({ id: item.Id, attributeName: item.AttributeName, dataType: item.DataType, dbcolumnName: item.dbcolumnName, isRequired: item.IsRequired, orderNo: item.OrderNo, options: item.FormBuilderAttributeLookUp }));
                }
            }

            self.removeItem = function (item) {                
                $('#lblMsg').html('');                
                if (item.id > 0) {
                    $.ajax({
                        type: 'POST',
                        url: '/FormBuilder/DeleteAttribute?attributeId=' + item.id,
                        success: function (result) {
                            if (!result.isSuccess) {
                                $('#lblMsg').html("Attribute is used in other places.");
                            }
                            else {
                                self.attributeItems.remove(item);
                            }
                        }
                    });

                } else {
                    self.attributeItems.remove(item);
                }
            };
            self.addItem = function () {
                self.attributeItems.push(new attributeItem({ id: "", attributeName: "", dataType: "", dbcolumnName:"", isRequired: false, orderNo: "", options:[]}));
            };
        };

  

        function initilizeForm() {
            form = new Global.FormHelper($("#frm-add-edit-formbuilder").find("form"), { updateTargetId: "validation-summary", validateSettings: validationSettings }, null, null);
            ko.applyBindings(new attributeViewModel());

            $(document).on('change','.isrequired_checkbox',  function () {
                $(this).val($(this).is(":checked"));
            })
        }

        $this.init = function () {
            initilizeForm();
        }
    }

    $(function () {
        var self = new FormBuilderAddEdit();
        self.init();
    })
})(jQuery)