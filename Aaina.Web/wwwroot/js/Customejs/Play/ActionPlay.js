(function ($) {
    function ActionPlayIndex() {
        var $this = this, form;
        var validationSettings;

        function initilizeModel() {
            BindGrid(
                {
                    controller: "play",
                    actionName: "index",
                    reportName: "Play",
                    companyId: companyId,
                    parm1: iview + "," + ValueType,
                    parm2: play_userId,
                    Parm3: ValueType,
                    filter: true,
                    searching: false,
                    columns: [
                        {
                            key: 'select',
                            data: 'id',
                            title: 'id',
                            allowHTML: true, // to avoid HTML escaping
                            title: '<input type="checkbox" class="all_action_check"/>',
                            "orderable": false, "searchable": false,
                            visible: iview == 'today',
                            "mRender": function (data, type, record) {
                                var btns = '<input type="checkbox" class="action_check" value="' + record.id + '"/>';
                                return btns;
                            }
                        },
                        { "data": "id", "title": "Play Id", "type": "int", "orderable": false, "searchable": false },
                        { "data": "gameId", "title": "Game", "type": "string", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: gameListArr } },
                        { "data": "subGameId", "title": "Sub Game", "type": "string", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: gameListArr } },
                        { "data": "name", "title": "Play", "type": "string", "orderable": true, "searchable": true },
                        { "data": "parentId", "title": "Parent", "type": "string", "orderable": true, "searchable": true },
                        { "data": "description", "title": "Description", "type": "string", "orderable": true, "searchable": true },
                        { "data": "accountableId", "title": "Accountable Persion", "int": "string", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: accountableListArr } },
                        { "data": "dependancyId", "title": "Dependancy", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: accountableListArr } },
                        { "data": "priority", "title": "Priority", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: priorityListArr } },
                        {
                            "data": "status", "title": "Realtime Status", "type": "int", "orderable": true, "searchable": true, "bind": { "ctrl": "selectlist", data: statusListArr },
                            "mRender": function (data, type, record) {
                                var btns = '<select class="form-control action_play_status" data-id=' + record.id + '" data-accountable="' + record.accountableId + '" data-accountableid="' + record.accountable + 'd" data-text="' + record.status + '">';
                                for (var i = 0; i < statusListArr.length; i++) {
                                    var statusArr = statusListArr[i];
                                    if (statusArr.Text == record.status) {
                                        btns += '<option value="' + statusArr.Value + '" selected>' + statusArr.Text + '</option>';
                                    } else {
                                        btns += '<option value="' + statusArr.Value + '">' + statusArr.Text + '</option>';
                                    }
                                }
                                btns += '</select>'


                                return btns;
                            }
                        },
                        { "data": "startDate", "title": "Planned Start", "type": "date", "orderable": true, "searchable": true },
                        { "data": "deadlineDate", "title": "Deadline", "type": "date", "orderable": true, "searchable": true },
                        { "data": "actualEndDate", "title": "Actual Start", "type": "date", "orderable": true, "searchable": true },
                        { "data": "actualStartDate", "title": "Actual End", "type": "date", "orderable": true, "searchable": true },
                        {
                            "data": "emotion", "title": "Emotion", "type": "string", "orderable": false, "searchable": false, "mRender": function (data, type, record) {
                                var btns = '';
                                if (parseInt(record.emotion) > 0) {
                                    btns += `<img src="${Global.GetEmojiName(record.emotion)}" class="imgemoji" />`;
                                }

                                return btns;
                            }
                        },

                        {
                            "data": null, "title": "Action",
                            "targets": -1,
                            "class": "column-action text-left",
                            "sClass": "text-left",
                            "shorting": false,
                            "orderable": false,
                            "mRender": function (data, type, record) {
                                var btns = '<div style="width:100px;">';
                                btns += "<a href='/" + tenant + "/play/Detail/" + record.id + "' title='Info'><i class='fa fa-info mtx'></i></a>";
                                btns += "<a href='/" + tenant + "/play/Edit/" + record.id + "' title='Edit'><i class='fa fa-pencil-square-o mtx'></i></a>";
                                btns += '<a href="/' + tenant + '/play/Delete/' + record.id + '" data-toggle="modal" data-backdrop="static" data-keyboard="false" data-target="#modal-action-delete-play"><i class="fa fa-trash-o mtx"></i></a>'
                                if (record.role == 1 && record.isRequested == false && record.gameType == 2) {
                                    btns += "<a href='/" + tenant + "/play/Approve/" + record.id + "' title='Info'>Approve</a>";
                                }
                                if (record.role != 1 && ValueType=="Status") {
                                    if (record.isRequested == false) {
                                        btns += "<span style='Color:red'> Pending<span>";
                                    } else {
                                        btns += "<span style='Color:green'>Approved<span>";
                                    }
                                }
                                //btns += "<a href='/" + tenant + "/play/Approve?id=" + record.id + "' title='Info'>Approve</a>";
                                btns += '</div>';
                                return btns;
                            }
                        },
                    ]

                });

            $(document).on('change', '.all_action_check', function () {

                $(".action_check").prop('checked', $(this).is(":checked"))

            });
            $(document).on('change', '.action_play_status', function () {

                var statusId = $(this).val();
                var status = $(this).find("option:selected").text();
                var id = $(this).data('id');
                var text = $(this).data('text');
                var accountable = $(this).data('accountable');
                var accountableid = $(this).data('accountableid');

                $("#sub_title").html(`Change status ${text} to ${status}`);
                $("#playid").val(id);
                $("#playstatus").val(statusId);
                $("#accountableid").val(accountableid);
                $("#delegate_label").html(`Delegate from ${accountable} to`);
                $("#comments_status").val("");
                $("#delegate_status").val("");


                if (parseInt(statusId) == 2) {
                    $("#div_delegate").show();
                } else {
                    $("#div_delegate").hide();
                }

                $("#modal-add-edit-status").modal('show');

            });



            $(document).on('click', '#btnsubmit', function () {
                var url = $(this).data('url');
                var playid = $("#playid").val();
                var playstatus = $("#playstatus").val();
                var accountableid = $("#accountableid").val();
                var comments = $("#comments_status").val();
                var delegate = $("#delegate_status").val();

                if (comments == "") {
                    alert('comment is required');
                }

                if (parseInt(playstatus) == 2 && delegate == "") {
                    alert('Select delegate user');
                }

                if ((parseInt(playstatus) == 2 && delegate != "") || (parseInt(playstatus) != 2 && comments != "")) {
                    var data = {
                        PlayId: playid,
                        StatusId: playstatus,
                        AccountableId: accountableid,
                        DelegateId: delegate,
                        Description: comments
                    };
                    UpdateData(url, data)
                }


            });

            $(".buttons-excel").on('click', function () {
                var url = $(this).data('url');
                window.location.href = url;
            });

            $("#btn_add_today").on('click', function () {
                var url = $(this).data('url');
                var favorite = [];
                $.each($(".action_check:checked"), function () {
                    favorite.push($(this).val());

                });

                if (favorite.length > 0) {
                    UpdateData(url, { id: favorite.join(",") })
                }


            });

            Global.ModelHelper($("#modal-action-delete-play"), function () {
                form = new Global.FormHelper($("#modal-action-delete-play").find("form"), {
                    updateTargetId: "validation-summary",
                    refreshGrid: false,
                    modelId: 'modal-action-delete-game'
                }, null, null);

            }, null);

            $(".buttons-excel").on('click', function () {
                var url = $(this).data('url');
                //window.location.href = url;
                var id = $(this).data('id');
                var $html = $("#" + id).html();
                $html = $html.replace('display: none;', '');
                var newHtml = '<html><head><title>PDF Export</title></head><body >' + $html + '</body></html>';
                Global.Excel(newHtml);
            });

            $(".buttons-pdf").on('click', function () {
                var id = $(this).data('id');
                var $html = $("#" + id).html();
                $html = $html.replace('display: none;', '');
                var newHtml = '<html><head><title>PDF Export</title></head><body >' + $html + '</body></html>';
                Global.Pdf(newHtml);
            });

            $(".buttons-print").on('click', function () {
                var id = $(this).data('id');
                var $html = $("#" + id).html();
                Global.Print($html, "action-play");
            });

            $(".gameselect").on('change', function () {
                var url = $(this).data('url');
                var gId = $(this).val();
                url = '/' + gId + url;
                window.location.href = url;

            })
            $("#filter_user").on('change', function () {
                var uId = $(this).val();
                var url = window.location.href;
                var nreUrl = Global.AddUpdateQueryStringParameter(url, 'uid', uId);
                window.location.href = nreUrl;

            })


            

        }

        function UpdateData(url, dataRequest) {
            Global.ShowLoading();
            $.ajax(url, {
                type: "POST",
                data: dataRequest,
                success: function (result) {
                    window.location.reload();
                },
                error: function (jqXHR, status, error) {

                    if (onError !== null && onError !== undefined) {
                        onError(jqXHR, status, error);
                    } else {


                    }
                    Global.HideLoading();
                }, complete: function () {
                    Global.HideLoading();
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
        var self = new ActionPlayIndex();
        self.init();
    })
})(jQuery)