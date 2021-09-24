"use strict";



var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationUserHub?userId=" + userId).build();

connection.on("sendToUser", (articleHeading, articleContent) => {
    var heading = document.createElement("h5");
    heading.textContent = articleHeading;
    var p = document.createElement("p");
    p.innerText = articleContent;
    var div = document.createElement("div");
    div.appendChild(heading);
    div.appendChild(p);
    clearBox("articleList");
    document.getElementById("articleList").appendChild(div);
    $('#myModal').modal('show');
});

connection.on("sendStatusToUser", (statusId) => {
    var url = '/Common/StatusFeedback/' + statusId;
    $('#status_feedback_btn').attr('href', url);
    $('#status_feedback_btn').click();
});

connection.on("sessionNotification", (articleHeading, articleContent) => {

    Global.ToastrInfo("New Notification");
    showSessionPendingMsgCount();
});

connection.on("SendMessage", (userId, message, userName, profileImage,receiveDateTime) => {
    appendGroupChatMessage(userId, message, userName, profileImage, receiveDateTime);
});

connection.on("sendToJoinGroup", (preSessionIds, preSessionNames, isAdmin) => {
    
    var idArr = preSessionIds.split(',');
    var nameArr = preSessionNames.split(',');
    if (isAdmin) {
        $(".icon_close").parent().show()
        $(".icon_close").attr('data-presessionId', idArr[0]);
    }

    for (var i = 0; i < idArr.length; i++) {
        $("#chat_group_name").html(nameArr[i]);
        $("#hid_presessionId").val(idArr[i]);
        $("#hid_presessionName").val(nameArr[i]);
        connection.invoke('JoinGroup', nameArr[i]);
        $('#chat_window_1').show();
    }
    GetAndBindGroupChat();
});

connection.on("sendToRemoveGroup", (preSessionName) => {
    
    connection.invoke('RemoveGroup', preSessionName);    
    $("#chat_window_1").hide();
    $(".msg_container_base").empty();
});

$("#btnPrivateGroup").click(function (e) {

    connection.invoke('JoinGroup', "PrivateGroup");
    $('#chat_window_1').show();
    $(this).hide();
    e.preventDefault();
});

$("#btnSend").click(function (e) {
    let message = $('#btn-input-chat').val();
    let sender = $("#hid_senderId").val();
    let presessionId = $("#hid_presessionId").val();
    let group = $("#hid_presessionName").val();

    if (message != '') {
        $('#btn-input-chat').val('');
        connection.invoke('SendMessageToGroup', group, sender, message, presessionId);
    }
    e.preventDefault();
});

$('.panel-footer input.chat_input').on('focus', function (e) {
    var $this = $(this);
    if ($('#minim_chat_window').hasClass('panel-collapsed')) {
        $this.parents('.panel').find('.panel-body').slideDown();
        $('#minim_chat_window').removeClass('panel-collapsed');
        $('#minim_chat_window').removeClass('glyphicon-plus').addClass('glyphicon-minus');
    }
});

$('.panel-heading span.icon_minim').on('click', function (e) {
    var $this = $(this);
    if (!$this.hasClass('panel-collapsed')) {
        $this.parents('.panel').find('.panel-body').slideUp();
        $this.addClass('panel-collapsed');
        $this.removeClass('glyphicon-minus').addClass('glyphicon-plus');
    } else {
        $this.parents('.panel').find('.panel-body').slideDown();
        $this.removeClass('panel-collapsed');
        $this.removeClass('glyphicon-plus').addClass('glyphicon-minus');
    }
});

$(document).on('click', '.icon_close', function (e) {
    var presessionId = $(this).data('presessionid');    
    connection.invoke('EndGroupChat', `${presessionId}`);
    $.get('/58/Session/GetChatHistory?sessionGroupId=' + presessionId, function (result) {
        $.post('/58/Session/EndChat?sessionGroupId=' + presessionId, { htmlContent: result }, function () {

        })
    })
});

setTimeout(function () {    
    connection.invoke('CheckGroupAvail', `${userId}`);
}, 2500)

connection.start().catch(function (err) {
    return console.error(err.toString());
}).then(function () {
    connection.invoke('GetConnectionId').then(function (connectionId) {
        document.getElementById('signalRConnectionId').innerHTML = connectionId;

    });
});
function clearBox(elementID) {
    document.getElementById(elementID).innerHTML = "";
}

function GetAndBindGroupChat() {
    Global.ShowLoading();
    var presessionId = $("#hid_presessionId").val();
    
    $.ajax({
        url: `/Session/GetGroupChat/${presessionId}`,
        type: 'get',
        cache: false,
        success: function (response) {
            Global.HideLoading();
            for (var i = 0; i < response.length; i++) {
                var mess = response[i];
                appendGroupChatMessage(mess.userId, mess.message, mess.userName, mess.imagePath, moment(new Date(mess.sendDate)).format('DD/MM/YYYY hh:mm a'));
            }  
            var $messageDiv = $('.msg_container_base');
            
        }
    });
}
function appendGroupChatMessage(uid, message, userName, profileImage, receiveDateTime) {
    
    var $messageDiv = $('.msg_container_base');
    $('<div/>', {
        'class': `row msg_container ${(userId == parseInt(uid) ? 'base_sent' : 'base_receive')
            }`,
        html: function () {
            if (userId != parseInt(uid)) {
                $('<div/>', {
                    'class': 'col-md-2 col-xs-2 avatar',
                    html: `<img src="${profileImage}" class=" img-responsive ">`
                }).appendTo(this);
            }

            $('<div/>', {
                'class': 'col-md-10 col-xs-10',
                html: function () {
                    $('<div/>', {
                        'class': `messages  ${(userId == parseInt(uid) ? 'msg_sent' : 'msg_receive')}`,
                        html: function () {
                            $('<p/>', {
                                html: message
                            }).appendTo(this);
                            $('<time/>', {
                                'datetime': `${receiveDateTime}`,
                                html: `${userName} at ${receiveDateTime}`
                            }).appendTo(this);
                        }
                    }).appendTo(this);
                }
            }).appendTo(this);

            if (userId == parseInt(uid)) {
                $('<div/>', {
                    'class': 'col-md-2 col-xs-2 avatar',
                    html: `<img src="${profileImage}" class=" img-responsive ">`
                }).appendTo(this);
            }
        }

    }).appendTo($messageDiv);

    $messageDiv.scrollTop($messageDiv[0].scrollHeight);
};

