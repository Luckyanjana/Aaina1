"use strict";

var statusCls = '.contact-status';
var online = 'online';
var offline = 'offline';
var unread = 'unread';
var group = 'group';
var userArr = new Array();
var userGrpArr = new Array();
var selectedUser = {};
selectedUser.senderid = userId;
selectedUser.sendername = userName;
selectedUser.companyId = companyId;
selectedUser.senderprofile = userAvatar;
//var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub?userId=" + userId).build();
Global.ShowLoading();
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/ChatHub?userId=" + userId)
    .build();

connection.start()
    .catch(err => alert(err.toString()));
document.getElementsByClassName("submit").disabled = true;


//start Connection
connection.start().then(function () {
    document.getElementsByClassName("submit").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on('UserConnected', (connectedUsers) => {

    for (var i = 0; i < connectedUsers.length; i++) {
        var objIndex = userArr.findIndex((obj => obj.id == connectedUsers[i] && obj.type == '1'));

        if (objIndex >= 0) {
            userArr[objIndex].isOnline = true;
            var $li = $('#user_' + connectedUsers[i]);
            if (userArr[objIndex].unreadMessage == 0) {
                $li.find(statusCls).removeClass(offline).addClass(online);
            } else {
                $li.find(statusCls).removeClass(offline).removeClass(online).addClass(unread);
            }
        }
    }

});

connection.on('UserDisconnected', (connectedUsers) => {
    for (var i = 0; i < connectedUsers.length; i++) {
        var objIndex = userArr.findIndex((obj => obj.id == connectedUsers[i] && obj.type == '1'));
        if (objIndex >= 0) {
            userArr[objIndex].isOnline = false;
            var $li = $('#user_' + connectedUsers[i]);
            if (userArr[objIndex].unreadMessage == 0) {
                $li.find(statusCls).removeClass(online).addClass(offline);
            } else {
                $li.find(statusCls).removeClass(offline).removeClass(online).addClass(unread);
            }
        }
    }

});

connection.on("UserAddedToGroup", (createdId, groupName, groupId, userIds, groupProfile) => {
    userArr.push({
        id: groupId,
        isAdmin: createdId == userId,
        isOnline: false,
        lastMessage: '',
        name: groupName,
        profileImage: groupProfile,
        sendDate: null,
        senderName: '',
        type: 2,
        unreadMessage: 0,
        userIds: userIds,
        userType: 0
    });
    PreBindUser(userArr[objIndex]);
});

connection.on("ReceiveMessageToUser", (senderid, sendername, message, receivertype,
    senderprofile, receiveDateTime) => {
    receiveUserNewMessage(senderid, sendername, message, receivertype, senderprofile, receiveDateTime);
    PlaySound();
});

connection.on("ReceiveMessageToGroup", (senderid, sendername, senderprofile, receiverId, message, receiveDateTime) => {

    receiveGroupNewMessage(senderid, sendername, message, 2, senderprofile, receiveDateTime, receiverId);
    PlaySound();
});

connection.on("UserRemoveFromGroup", (groupId) => {

    $("#group_" + groupId).remove();
    userArr = jQuery.grep(userArr, function (value) {
        return !(value.id == groupId && value.type == 2);
    });
});

connection.on("UpdateGroupName", (groupId, groupName, userIds) => {

    var objIndex = userArr.findIndex((obj => obj.id == groupId && obj.type == 2));
    if (objIndex >= 0) {
        userArr[objIndex].name = groupName;
        userArr[objIndex].userIds = userIds;
        PreBindUser(userArr[objIndex]);
    }
});

//$(".messages").animate({ scrollTop: $(document).height() }, "fast");
var $messageDiv = $('.messages');
$messageDiv.scrollTop($messageDiv[0].scrollHeight);

$("#profile-img").click(function () {
    $("#status-options").toggleClass("active");
});

$(document).on('click', ".contacts-list li.chat", function () {
    $('.message-input').show();
    var $li = $(this);
    $('.contact').removeClass('active');
    $li.addClass('active');
    $li.find('span.contact-status').removeClass('unread').addClass('offline').html('')
    var type = $li.data('type');
    var id = $li.data('id');
    var name = $li.data('name');
    var profile = $li.data('profile');
    var isAdmin = $li.data('isadmin');
    var userIds = $li.data('userids');
    var admin = $li.data('admin');
    $(".profile-user-main-img").attr('src', profile);
    $(".users-main-name").html(name + `${type == 2 ? `-(${admin})` : ''}`);
    if (parseInt(type) == 2) {
        if (isAdmin) {
            $('.delete-group').show();
            $('.edit-group').show();
            $('.delete-user').hide();
        } else {
            $('.delete-user').show();
            $('.delete-group').hide();
            $('.edit-group').hide();
        }
        $('.group-info').show();
    }
    else {
        $('.delete-group').hide();
        $('.delete-user').hide();
        $('.edit-group').hide();
        $('.group-info').hide();
    }
    $(".contact-profile-div").show();
    $(".group-info-header").hide();

    $(".pdf-export").show();
    selectedUser.receiverid = `${id}`;
    selectedUser.receivertype = `${type}`;
    selectedUser.receivername = name;
    selectedUser.receiverprofile = profile;
    selectedUser.userIds = userIds;

    var objIndex = userArr.findIndex((obj => obj.id == selectedUser.receiverid && obj.type == selectedUser.receivertype));
    userArr[objIndex].unreadMessage = 0;
    $(".messages ul").empty();
    connection.invoke('GetChatHistory', selectedUser.senderid, selectedUser.receiverid, selectedUser.receivertype).then(function (response) {

        for (var i = 0; i < response.length; i++) {
            var mess = response[i];
            appendChatMessage(mess.senderId, mess.message, mess.senderName, mess.profileImage, moment(new Date(mess.sendDate)).format('DD/MM/YYYY hh:mm a'));
        }
        //$(".messages").animate({ scrollTop: $(document).height() }, "fast");
        var $messageDiv = $('.messages');
        $messageDiv.scrollTop($messageDiv[0].scrollHeight);
    });
});

$(document).on('click', ".contacts-list li.group_add1", function () {
    var $li = $(this);
    $li.find('.grpCheck').prop('checked', !$li.find('.grpCheck').is(':checked'));
});

$(document).on('click', '.delete-user', function () {
    let groupId = selectedUser.receiverid;
    let user_Id = userId;
    var result = confirm('Are you want to remove this group!')
    if (result) {

        connection.invoke('DeleteUser', `${groupId}`, `${user_Id}`);
        $(".message-input").hide();
    }
});

$(document).on('click', '.delete-group', function () {
    let groupId = selectedUser.receiverid;

    var result = confirm('Are you want to remove this group and all users!')
    if (result) {
        connection.invoke('DeleteGroup', `${groupId}`);
    }
});

$(document).on('click', '.edit-group', function () {
    var objIndex = userArr.findIndex((obj => obj.id == selectedUser.receiverid && obj.type == 2));
    let usergroup = userArr[objIndex];
    $(".txtgroupname").val(usergroup.name);
    $(".txtgroupid").val(usergroup.id);
    $(".chat_panel").css('display', 'none');
    $('.group_create_panel').css('display', 'block');
    BindGroupUsers(selectedUser.userIds, false);
});

function newMessage() {
    var $inputType = $(".message-input input");
    let message = $inputType.val();
    if ($.trim(message) == '') {
        return false;
    }

    if (parseInt(selectedUser.receivertype) == 1) {

        $('<li class="sent"><img src="' + selectedUser.senderprofile + '" alt="" /><h4> ' + selectedUser.sendername + ' </h4><p>' + message + '</p></li>').appendTo($('.messages ul'));
        $('.contact.active .preview').html('<span>You: </span>' + message);
        //$(".messages").animate({ scrollTop: $messageDiv[0].scrollHeight }, "fast");
        var $messageDiv = $('.messages');
        $messageDiv.scrollTop($messageDiv[0].scrollHeight);
    }
    $('.message-input input').val(null);
    if (message != '') {
        var objIndex = userArr.findIndex((obj => obj.id == selectedUser.receiverid && obj.type == selectedUser.receivertype));
        userArr[objIndex].senderName = 'You';
        userArr[objIndex].lastMessage = message;
        userArr[objIndex].sendDate = new Date();
        if (parseInt(selectedUser.receivertype) == 1) {
            connection.invoke('SendMessageToUser', selectedUser.senderid, selectedUser.sendername, selectedUser.senderprofile, selectedUser.receiverid, selectedUser.receivertype, message);
        } else {
            connection.invoke('SendMessageToGroup', selectedUser.receiverid, selectedUser.senderid, selectedUser.sendername, selectedUser.senderprofile, message);
        }
        PreBindUser(userArr[objIndex]);
    }

};

function receiveUserNewMessage(senderid, sendername, message, receivertype,
    senderprofile, receiveDateTime) {

    var objIndex = userArr.findIndex((obj => obj.id == senderid && obj.type == receivertype));
    userArr[objIndex].senderName = sendername;
    userArr[objIndex].lastMessage = message;
    userArr[objIndex].sendDate = new Date();
    if (senderid == selectedUser.receiverid) {
        $('<li class="replies"><img src="' + senderprofile + '" alt="" /> <h4 style="float:right;"> ' + sendername + ' </h4> <br/><p>' + message + '</p></li>').appendTo($('.messages ul'));
        $('.contact.active .preview').html(`<span>${sendername}: </span>' ${message}`);
        var $messageDiv = $('.messages');
        $messageDiv.scrollTop($messageDiv[0].scrollHeight);
    } else {
        userArr[objIndex].unreadMessage = userArr[objIndex].unreadMessage + 1;
        PlaySound();
    }
    PreBindUser(userArr[objIndex]);
};

function receiveGroupNewMessage(senderid, sendername, message, receivertype,
    senderprofile, receiveDateTime, receiverId) {
    var objIndex = userArr.findIndex((obj => obj.id == receiverId && obj.type == receivertype));
    userArr[objIndex].senderName = sendername;
    userArr[objIndex].lastMessage = message;
    userArr[objIndex].sendDate = new Date();
    if (receiverId == selectedUser.receiverid) {
        if (parseInt(senderid) != userId) {
            $('<li class="replies"><img src="' + senderprofile + '" alt="" /> <h4 style="float:right;"> ' + sendername + ' </h4> <br/> <p>' + message + '</p></li>').appendTo($('.messages ul'));
            $('.contact.active .preview').html(`<span>${sendername}: </span>' ${message}`);
        } else {
            $('<li class="sent"><img src="' + senderprofile + '" alt="" /> <h4> ' + sendername + ' </h4><p>' + message + '</p></li>').appendTo($('.messages ul'));
            $('.contact.active .preview').html(`<span>You: </span>' ${message}`);
        }
        var $messageDiv = $('.messages');
        $messageDiv.scrollTop($messageDiv[0].scrollHeight);
    } else {
        userArr[objIndex].unreadMessage = userArr[objIndex].unreadMessage + 1;
    }
    PreBindUser(userArr[objIndex]);
};

$('.submit').click(function (e) {
    newMessage();
    e.preventDefault();
});

function appendChatMessage(uid, message, userName, profileImage, receiveDateTime) {

    var $messageDiv = $('.messages ul');

    if (userId != parseInt(uid)) {
        $('<li/>', {
            'class': 'replies',
            html: `<img src="${profileImage}" alt="${userName}"> <h4 style="float:right;"> ${userName} </h4> <br/> <p> ${message} </p>`
        }).appendTo($messageDiv);
    }

    if (userId == parseInt(uid)) {
        $('<li/>', {
            'class': 'sent',
            html: `<img src="${profileImage}" alt="${userName}"><h4> ${userName} </h4> <p> ${message} </p>`
        }).appendTo($messageDiv);
    }

    var $messageDiv = $('.messages');
    $messageDiv.scrollTop($messageDiv[0].scrollHeight);
};

$(window).on('keydown', function (e) {
    if (e.which == 13) {
        newMessage();
        e.preventDefault();
        return false;
    }
});

setTimeout(function () {
    connection.invoke('GetUserList', companyId, userId).then(function (users) {
        userArr = users;
        for (var i = 0; i < userArr.length; i++) {
            if (userArr[i].sendDate != null) {
                userArr[i].sendDate = new Date(userArr[i].sendDate);
            }
            if (userArr[i].type == 2) {
                connection.invoke('Joinroup', `${userArr[i].id}`)
            }
        }
        BindUsers();
        Global.HideLoading();
    });
}, 1500)

function BindUsers() {
    var $ul = $(".contacts-list");
    $ul.empty();
    let userTypefilter = $(".usertype_checkbox").is(":checked");
    userArr = userArr.sort((a, b) => (a.sendDate < b.sendDate) ? 1 : -1)
    for (var i = 0; i < userArr.length; i++) {
        var user = userArr[i];
        if ((userTypefilter && user.userType == 3) || (!userTypefilter && user.userType == 2) || user.userType == 0) {
            $('<li/>', {
                'class': 'contact chat',
                'data-type': user.type,
                'data-id': user.id,
                'id': `${user.type == 1 ? 'user' : 'group'}_${user.id}`,
                'data-isAdmin': user.isAdmin,
                'data-lastMessage': user.lastMessage,
                'data-name': user.name,
                'data-userIds': user.userIds,
                'data-profile': user.profileImage,
                'data-unreadMessage': user.unreadMessage,
                'data-admin': user.admin,
                html: function () {
                    $('<div/>', {
                        'class': 'wrap',
                        html: function () {
                            if (user.unreadMessage > 0) {
                                $('<span/>', {
                                    'class': `contact-status ${unread}`,
                                    html: user.unreadMessage
                                }).appendTo(this);
                            } else if (user.type == 2) {
                                $('<span/>', {
                                    'class': `contact-status ${group}`
                                }).appendTo(this);
                            } else {
                                $('<span/>', {
                                    'class': `contact-status ${user.isOnline ? online : offline}`
                                }).appendTo(this);
                            }

                            $('<img/>', {
                                'src': user.profileImage
                            }).appendTo(this);

                            $('<div/>', {
                                'class': 'meta',
                                html: function () {
                                    $('<p/>', {
                                        'class': 'name',
                                        html: user.name
                                    }).appendTo(this);


                                    $('<p/>', {
                                        'class': 'preview',
                                        html: user.senderName != '' ? `<span>${user.senderName}: </span> ${user.lastMessage}` : ''
                                    }).appendTo(this);
                                }
                            }).appendTo(this);
                        }
                    }).appendTo(this);
                }
            }).appendTo($ul);
        }
    }

}

function PreBindUser(user) {
    var $ul = $(".contacts-list");
    let li_id = `${user.type == 1 ? 'user' : 'group'}_${user.id}`;
    $("#" + li_id).remove();

    $('<li/>', {
        'class': 'contact chat',
        'data-type': user.type,
        'data-id': user.id,
        'id': `${user.type == 1 ? 'user' : 'group'}_${user.id}`,
        'data-isAdmin': user.isAdmin,
        'data-lastMessage': user.lastMessage,
        'data-name': user.name,
        'data-userIds': user.userIds,
        'data-profile': user.profileImage,
        'data-unreadMessage': user.unreadMessage,
        'data-admin': user.admin,
        html: function () {
            $('<div/>', {
                'class': 'wrap',
                html: function () {
                    if (user.unreadMessage > 0) {
                        $('<span/>', {
                            'class': `contact-status ${unread}`,
                            html: user.unreadMessage
                        }).appendTo(this);
                    } else if (user.type == 2) {
                        $('<span/>', {
                            'class': `contact-status ${group}`
                        }).appendTo(this);
                    } else {
                        $('<span/>', {
                            'class': `contact-status ${user.isOnline ? online : offline}`
                        }).appendTo(this);
                    }

                    $('<img/>', {
                        'src': user.profileImage
                    }).appendTo(this);

                    $('<div/>', {
                        'class': 'meta',
                        html: function () {
                            $('<p/>', {
                                'class': 'name',
                                html: user.name
                            }).appendTo(this);


                            $('<p/>', {
                                'class': 'preview',
                                html: user.senderName != '' ? `<span>${user.senderName}: </span> ${user.lastMessage}` : ''
                            }).appendTo(this);
                        }
                    }).appendTo(this);
                }
            }).appendTo(this);
        }
    }).prependTo($ul);



}

function BindGroupUsers(userIds, isAdd) {
    var $ul = $(".group-contacts-list");
    $ul.empty();
    userArr = userArr.sort((a, b) => (a.sendDate < b.sendDate) ? 1 : -1)
    let geoupUserId = new Array();
    if (!isAdd) {
        var uArr = userIds.split(',');
        for (var i = 0; i < uArr.length; i++) {
            geoupUserId.push(uArr[i]);
        }
    }

    for (var i = 0; i < userArr.length; i++) {
        var user = userArr[i];
        if (user.type == 1) {
            $('<li/>', {
                'class': 'contact group_add',
                'data-id': user.id,
                'data-name': user.name,
                'id': `${user.type == 1 ? 'user' : 'group'}_${user.id}`,
                'data-ischeck': false,
                html: function () {
                    $('<div/>', {
                        'class': 'wrap',
                        html: function () {
                            $('<span/>', {
                                'class': `contact-status ${group}`,
                                html: function () {
                                    let isChecked = $.grep(geoupUserId, function (n, i) {
                                        return (n == user.id);
                                    });
                                    if (isChecked.length > 0) {
                                        $('<input/>', {
                                            'type': 'checkbox',
                                            'checked': 'checked',
                                            'class': 'grpCheck',
                                            'value': user.id
                                        }).appendTo(this);
                                    } else {
                                        $('<input/>', {
                                            'type': 'checkbox',
                                            'class': 'grpCheck',
                                            'value': user.id
                                        }).appendTo(this);
                                    }
                                }
                            }).appendTo(this);

                            $('<img/>', {
                                'src': user.profileImage
                            }).appendTo(this);
                            $('<div/>', {
                                'class': 'meta',
                                html: function () {
                                    $('<p/>', {
                                        'class': 'name',
                                        html: user.name
                                    }).appendTo(this);


                                    $('<p/>', {
                                        'class': 'preview',
                                        html: user.senderName != '' ? `<span>${user.senderName}: </span> ${user.lastMessage}` : ''
                                    }).appendTo(this);
                                }
                            }).appendTo(this);
                        }
                    }).appendTo(this);
                }
            }).appendTo($ul);
        }
    }

}

function userSearch(e, panel) {
    var filter, a, txtValue;

    filter = e.value.toUpperCase();
    // Loop through all list items, and hide those who don't match the search query
    $.each($(panel).find(".contacts-list li"), function () {
        a = $(this).data('name');
        txtValue = a;
        if (txtValue.toUpperCase().indexOf(filter) > -1) {
            $(this).css('display', '');
        } else {
            $(this).css('display', 'none');
        }
    });
}

$('.btnCreateGrp').on('click', function () {
    $(".chat_panel").css('display', 'none');
    $('.group_create_panel').css('display', 'block');
    BindGroupUsers('', true);
});

$('.btnCancel').on('click', function () {
    $(".chat_panel").css('display', 'block');
    $('.group_create_panel').css('display', 'none');
    $('.group-contacts-list').empty();
});

$(".usertype_checkbox").on('change', function () {
    BindUsers();
})

$('.btnCreate').on('click', function () {
    var userIds = '';

    var groupName = $('.txtgroupname').val();
    var groupId = $(".txtgroupid").val();
    if ($.trim(groupName) == '') {
        return false;
    }
    var $userli = $(".grpCheck:checked");
    for (var i = 0; i < $userli.length; i++) {

        if (userIds) {
            userIds += ", " + $userli[i].value;
        }
        else {
            userIds = $userli[i].value;
        }
    }

    if ($.trim(userIds) == '') {
        return false;
    }

    connection.invoke('CreateGroup', selectedUser.senderid, groupName, groupId, userIds);
    $('.txtgroupname').val('');
    $('.txtgroupid').val('');
    $(".chat_panel").css('display', 'block');
    $('.group_create_panel').css('display', 'none');
    $('.group-contacts-list').empty();
});


$(".group-info").on('click', function () {
    $('.messages ul').empty();
    $(".contact-profile-div").hide();
    $(".group-info-header").show();
    $(".pdf-export-div").hide();
    $('.message-input').hide();
    $(".group-info-back").show();
    var objIndex = userArr.findIndex((obj => obj.id == selectedUser.receiverid && obj.type == 2));
    var usIds = userArr[objIndex].userIds.split(',');
    $('<li class="sent"><img src="' + userAvatar + '" alt=""> <p> You </p></li>').appendTo($('.messages ul'));
    for (var i = 0; i < usIds.length; i++) {
        var uIndex = userArr.findIndex((obj => obj.id == usIds[i] && obj.type == 1));
        if (uIndex > 0) {
            let user = userArr[uIndex];
            $('<li class="sent"><img src="' + user.profileImage + '" alt=""> <p> ' + user.name + ' </p></li>').appendTo($('.messages ul'));
        }
    }
})

$(".group-info-back").on('click', function () {
    $('.messages ul').empty();
    $(".contact-profile-div").show();
    $(".group-info-header").hide();
    $(".pdf-export-div").hide();
    $('.message-input').show();
    $(".group-info-back").hide();
    var objIndex = userArr.findIndex((obj => obj.id == selectedUser.receiverid && obj.type == selectedUser.receivertype));
    userArr[objIndex].unreadMessage = 0;
    $(".messages ul").empty();
    connection.invoke('GetChatHistory', selectedUser.senderid, selectedUser.receiverid, selectedUser.receivertype).then(function (response) {

        for (var i = 0; i < response.length; i++) {
            var mess = response[i];
            appendChatMessage(mess.senderId, mess.message, mess.senderName, mess.profileImage, moment(new Date(mess.sendDate)).format('DD/MM/YYYY hh:mm a'));
        }
        //$(".messages").animate({ scrollTop: $(document).height() }, "fast");
        var $messageDiv = $('.messages');
        $messageDiv.scrollTop($messageDiv[0].scrollHeight);
    });
})

$(".pdf-export").on('click', function () {
    $(".contact-profile-div").hide();
    $(".group-info-header").hide();
    $(".pdf-export-div").show();
})

$(".pdf-export-pdf").on('click', function () {
    let fromdate = $(".fromdate").val();
    let todate = $(".todate").val();
    if (fromdate != '' && todate != '') {
        $(".contact-profile-div").show();
        $(".pdf-export-div").hide();
        $(".group-info-header").hide();
        $(".fromdate").val('');
        $(".todate").val('');
        window.open(`/chat/export/${selectedUser.senderid}?r=${selectedUser.receiverid}&t=${selectedUser.receivertype}&fr=${fromdate}&to=${todate}&rn=${selectedUser.receivername}&pi=${selectedUser.receiverprofile}`, '_blank');
    }

})

$(".pdf-export-close").on('click', function () {
    $(".contact-profile-div").show();
    $(".pdf-export-div").hide();
    $(".group-info-header").hide();
    $(".fromdate").val('');
    $(".todate").val('');

})

function PlaySound() {
    $('.audio_beep').remove();
    var id = moment(new Date()).format('DDMMYYYYhhmma') + (new Date().getMilliseconds());
    $('<audio/>', {
        'id': id,
        'class': 'audio_beep',
        'src': '/pristine.mp3',
        'autostart': 'false',
        'style': 'display:none;'
    }).appendTo('body')
    var sound = document.getElementById(id);
    sound.play()
   // $("#" + id).remove();
}
//document.onkeyup = function (e) {
//    var e = e || window.event; // for IE to cover IEs window object
//    debugger;
//    if (e.altKey && e.which == 65) {
//        alert('Keyboard shortcut working!');
//        return false;
//    }
//}



